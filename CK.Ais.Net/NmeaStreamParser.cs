// <copyright file="NmeaStreamParser.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Ais.Net
{
    using System;
    using System.Buffers;
    using System.IO;
    using System.IO.Pipelines;
    using System.Threading.Tasks;

    /// <summary>
    /// Processes streams containing lines of ASCII-encoded text, each containing an NMEA message.
    /// </summary>
    public static class NmeaStreamParser
    {
        /// <summary>
        /// Process the contents of a file one AIS message at a time.
        /// </summary>
        /// <param name="path">Path of the file to process.</param>
        /// <param name="processor">Handler for the AIS messages.</param>
        /// <returns>A task that completes when the stream has been processed.</returns>
        /// <remarks>
        /// This reassembles AIS messages that have been split over multiple NMEA lines.
        /// </remarks>
        public static Task ParseFileAsync(
            string path,
            INmeaAisMessageStreamProcessor processor)
        {
            return ParseFileAsync(path, processor, new NmeaParserOptions());
        }

        /// <summary>
        /// Process the contents of a file one AIS message at a time.
        /// </summary>
        /// <param name="path">Path of the file to process.</param>
        /// <param name="processor">Handler for the AIS messages.</param>
        /// <param name="options">Configures parser behaviour.</param>
        /// <returns>A task that completes when the stream has been processed.</returns>
        /// <remarks>
        /// This reassembles AIS messages that have been split over multiple NMEA lines.
        /// </remarks>
        public static async Task ParseFileAsync(
            string path,
            INmeaAisMessageStreamProcessor processor,
            NmeaParserOptions options)
        {
            using var adapter = new NmeaLineToAisStreamAdapter(processor, options);
            await ParseFileAsync(path, adapter, options).ConfigureAwait(false);
        }

        /// <summary>
        /// Process the contents of a file.
        /// </summary>
        /// <param name="path">Path of the file to process.</param>
        /// <param name="processor">Handler for the parsed lines.</param>
        /// <returns>A task that completes when the stream has been processed.</returns>
        public static Task ParseFileAsync(
            string path,
            INmeaLineStreamProcessor processor)
        {
            return ParseFileAsync(path, processor, new NmeaParserOptions());
        }

        /// <summary>
        /// Process the contents of a file.
        /// </summary>
        /// <param name="path">Path of the file to process.</param>
        /// <param name="processor">Handler for the parsed lines.</param>
        /// <param name="options">Configures parser behaviour.</param>
        /// <returns>A task that completes when the stream has been processed.</returns>
        public static async Task ParseFileAsync(
            string path,
            INmeaLineStreamProcessor processor,
            NmeaParserOptions options)
        {
            // This turns off internal file stream buffering
            using var file = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, bufferSize: 1, useAsync: true);
            await ParseStreamAsync(file, processor, options).ConfigureAwait(false);
        }

        /// <summary>
        /// Process the contents of a stream.
        /// </summary>
        /// <param name="stream">The stream to process.</param>
        /// <param name="processor">Handler for the parsed lines.</param>
        /// <returns>A task that completes when the stream has been processed.</returns>
        public static Task ParseStreamAsync(
            Stream stream,
            INmeaLineStreamProcessor processor)
        {
            return ParseStreamAsync(stream, processor, new NmeaParserOptions());
        }

        /// <summary>
        /// Process the contents of a stream.
        /// </summary>
        /// <param name="stream">The stream to process.</param>
        /// <param name="processor">Handler for the parsed lines.</param>
        /// <param name="options">Configures parser behaviour.</param>
        /// <returns>A task that completes when the stream has been processed.</returns>
        public static async Task ParseStreamAsync(
            Stream stream,
            INmeaLineStreamProcessor processor,
            NmeaParserOptions options)
        {
            int lines = 0;
            int ticksAtStart = Environment.TickCount;
            int ticksAtLastLineCount = ticksAtStart;

            try
            {
                PipeReader reader = CreateFileReader(stream);

                byte[] splitLineBuffer = new byte[1000];
                while (true)
                {
                    ReadResult result = await reader.ReadAsync().ConfigureAwait(false);
                    ReadOnlySequence<byte> remainingSequence = ProcessBuffer(result);

                    reader.AdvanceTo(remainingSequence.Start, remainingSequence.End);

                    if (result.IsCompleted)
                    {
                        break;
                    }
                }

                const int LineCountInterval = 100000;
                int finalTicks = Environment.TickCount;
                int totalTicks = finalTicks - ticksAtStart;

                processor.Progress(
                    true,
                    lines,
                    totalTicks,
                    lines % LineCountInterval,
                    finalTicks - ticksAtLastLineCount);

                ReadOnlySequence<byte> ProcessBuffer(
                    in ReadResult result)
                {
                    ReadOnlySpan<byte> lineSpan;
                    SequencePosition? position = null;

                    ReadOnlySequence<byte> remainingSequence = result.Buffer;

                    while ((position = remainingSequence.PositionOf((byte)'\n') ?? (remainingSequence.IsEmpty || !result.IsCompleted ? default(SequencePosition?) : remainingSequence.End)) != null)
                    {
                        ReadOnlySequence<byte> line = remainingSequence.Slice(remainingSequence.Start, position.Value);

                        if (line.IsSingleSegment)
                        {
                            lineSpan = line.First.Span;
                        }
                        else
                        {
                            Span<byte> reassemblySpan = splitLineBuffer;
                            line.CopyTo(reassemblySpan);
                            lineSpan = reassemblySpan.Slice(0, (int)line.Length);
                        }

                        if (lineSpan.Length > 0 && lineSpan[lineSpan.Length - 1] == (byte)'\r')
                        {
                            lineSpan = lineSpan.Slice(0, lineSpan.Length - 1);
                        }

                        if (lineSpan.Length > 0)
                        {
                            try
                            {
                                var parsedLine = new NmeaLineParser(lineSpan, options.ThrowWhenTagBlockContainsUnknownFields, options.TagBlockStandard);

                                processor.OnNext(parsedLine, lines + 1);
                            }
                            catch (Exception x)
                            {
                                processor.OnError(lineSpan, x, lines + 1);
                            }
                        }

                        remainingSequence = position.Value.Equals(remainingSequence.End)
                            ? remainingSequence.Slice(remainingSequence.End)
                            : remainingSequence.Slice(remainingSequence.GetPosition(1, position.Value));

                        if (++lines % LineCountInterval == 0)
                        {
                            int currentTicks = Environment.TickCount;
                            int ticksSinceLastLineCount = currentTicks - ticksAtLastLineCount;

                            processor.Progress(
                                false,
                                lines,
                                currentTicks - ticksAtStart,
                                LineCountInterval,
                                ticksSinceLastLineCount);
                            ticksAtLastLineCount = currentTicks;
                        }
                    }

                    return remainingSequence;
                }
            }
            finally
            {
                processor.OnCompleted();
            }
        }

        private static PipeReader CreateFileReader(Stream file)
        {
            async Task ProcessFileAsync(PipeWriter writer)
            {
                try
                {
                    while( true )
                    {
                        Memory<byte> memory = writer.GetMemory();

                        // See https://github.com/dotnet/corefx/blob/master/src/Common/src/CoreLib/System/IO/Stream.cs#L379
                        // for how Stream.ReadAsync handles Memory<byte>.
#if NETSTANDARD2_0
                        System.Runtime.InteropServices.MemoryMarshal.TryGetArray(memory, out ArraySegment<byte> memoryArray);
                        int read = await file.ReadAsync(memoryArray.Array, memoryArray.Offset, memoryArray.Count).ConfigureAwait(false);
#else
                        int read = await file.ReadAsync( memory ).ConfigureAwait( false );
#endif
                        if( read == 0 )
                        {
                            break;
                        }

                        writer.Advance( read );

                        await writer.FlushAsync().ConfigureAwait( false );
                    }
                }
                catch( Exception ex )
                {
                    await writer.CompleteAsync( ex );
                }
                finally
                {
                    await writer.CompleteAsync();
                }
            }

            var pipe = new Pipe(new PipeOptions(minimumSegmentSize: 64 * 1024));
            _ = ProcessFileAsync(pipe.Writer);

            return pipe.Reader;
        }
    }
}
