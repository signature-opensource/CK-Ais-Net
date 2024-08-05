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
        public static Task ParseFileAsync<TExtraFieldParser>( string path, INmeaAisMessageStreamProcessor<TExtraFieldParser> processor )
            where TExtraFieldParser : struct, INmeaTagBlockExtraFieldParser
        {
            return ParseFileAsync( path, processor, new NmeaParserOptions() );
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
        public static async Task ParseFileAsync<TExtraFieldParser>( string path,
                                                 INmeaAisMessageStreamProcessor<TExtraFieldParser> processor,
                                                 NmeaParserOptions options )
            where TExtraFieldParser : struct, INmeaTagBlockExtraFieldParser
        {
            using var adapter = new NmeaLineToAisStreamAdapter<TExtraFieldParser>( processor, options );
            await ParseFileAsync( path, adapter, options ).ConfigureAwait( false );
        }

        /// <summary>
        /// Process the contents of a file.
        /// </summary>
        /// <param name="path">Path of the file to process.</param>
        /// <param name="processor">Handler for the parsed lines.</param>
        /// <returns>A task that completes when the stream has been processed.</returns>
        public static Task ParseFileAsync<TExtraFieldParser>( string path, INmeaLineStreamProcessor<TExtraFieldParser> processor )
            where TExtraFieldParser : struct, INmeaTagBlockExtraFieldParser
        {
            return ParseFileAsync( path, processor, new NmeaParserOptions() );
        }

        /// <summary>
        /// Process the contents of a file.
        /// </summary>
        /// <param name="path">Path of the file to process.</param>
        /// <param name="processor">Handler for the parsed lines.</param>
        /// <param name="options">Configures parser behaviour.</param>
        /// <returns>A task that completes when the stream has been processed.</returns>
        public static async Task ParseFileAsync<TExtraFieldParser>( string path,
                                                 INmeaLineStreamProcessor<TExtraFieldParser> processor,
                                                 NmeaParserOptions options )
            where TExtraFieldParser : struct, INmeaTagBlockExtraFieldParser
        {
            // This turns off internal file stream buffering
            using var file = new FileStream( path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, bufferSize: 1, useAsync: true );
            await ParseStreamAsync( file, processor, options ).ConfigureAwait( false );
        }

        /// <summary>
        /// Process the contents of a stream.
        /// </summary>
        /// <param name="stream">The stream to process.</param>
        /// <param name="processor">Handler for the parsed lines.</param>
        /// <returns>A task that completes when the stream has been processed.</returns>
        public static Task ParseStreamAsync<TExtraFieldParser>( Stream stream, INmeaLineStreamProcessor<TExtraFieldParser> processor )
            where TExtraFieldParser : struct, INmeaTagBlockExtraFieldParser
        {
            return ParseStreamAsync( stream, processor, new NmeaParserOptions() );
        }

        /// <summary>
        /// Process the contents of a stream.
        /// </summary>
        /// <param name="stream">The stream to process.</param>
        /// <param name="processor">Handler for the parsed lines.</param>
        /// <param name="options">Configures parser behaviour.</param>
        /// <returns>A task that completes when the stream has been processed.</returns>
        public static Task ParseStreamAsync<TExtraFieldParser>( Stream stream,
                                             INmeaLineStreamProcessor<TExtraFieldParser> processor,
                                             NmeaParserOptions options )
            where TExtraFieldParser : struct, INmeaTagBlockExtraFieldParser
        {
            var reader = PipeReader.Create( stream, new StreamPipeReaderOptions( bufferSize: 64 * 1024 ) );
            return ParseAsync( reader, processor, options );
        }

        /// <summary>
        /// Process the contents of a pipe reader.
        /// </summary>
        /// <param name="reader">The reader to process.</param>
        /// <param name="processor">Handler for the parsed lines.</param>
        /// <param name="options">Configures parser behaviour.</param>
        /// <returns>A task that completes when the stream has been processed.</returns>
        public static async Task ParseAsync<TExtraFieldParser>( PipeReader reader,
                                             INmeaLineStreamProcessor<TExtraFieldParser> processor,
                                             NmeaParserOptions options )
            where TExtraFieldParser : struct, INmeaTagBlockExtraFieldParser
        {
            int lines = 0;
            int ticksAtStart = Environment.TickCount;
            int ticksAtLastLineCount = ticksAtStart;

            try
            {
                byte[] splitLineBuffer = new byte[1000];
                while( true )
                {
                    ReadResult result = await reader.ReadAsync().ConfigureAwait( false );
                    ReadOnlySequence<byte> remainingSequence = ProcessBuffer( result );

                    reader.AdvanceTo( remainingSequence.Start, remainingSequence.End );

                    if( result.IsCompleted )
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
                    finalTicks - ticksAtLastLineCount );

                ReadOnlySequence<byte> ProcessBuffer(
                    in ReadResult result )
                {
                    ReadOnlySpan<byte> lineSpan;
                    SequencePosition? position = null;

                    ReadOnlySequence<byte> remainingSequence = result.Buffer;

                    while( (position = remainingSequence.PositionOf( (byte)'\n' ) ?? (remainingSequence.IsEmpty || !result.IsCompleted ? default( SequencePosition? ) : remainingSequence.End)) != null )
                    {
                        ReadOnlySequence<byte> line = remainingSequence.Slice( remainingSequence.Start, position.Value );

                        if( line.IsSingleSegment )
                        {
                            lineSpan = line.First.Span;
                        }
                        else
                        {
                            Span<byte> reassemblySpan = splitLineBuffer;
                            line.CopyTo( reassemblySpan );
                            lineSpan = reassemblySpan.Slice( 0, (int)line.Length );
                        }

                        if( lineSpan.Length > 0 && lineSpan[lineSpan.Length - 1] == (byte)'\r' )
                        {
                            lineSpan = lineSpan.Slice( 0, lineSpan.Length - 1 );
                        }

                        if( lineSpan.Length > 0 )
                        {
                            try
                            {
                                var parsedLine = new NmeaLineParser<TExtraFieldParser>( lineSpan, options.ThrowWhenTagBlockContainsUnknownFields, options.TagBlockStandard, options.EmptyGroupTolerance );

                                processor.OnNext( parsedLine, lines + 1 );
                            }
                            catch( Exception x )
                            {
                                processor.OnError( lineSpan, x, lines + 1 );
                            }
                        }

                        remainingSequence = position.Value.Equals( remainingSequence.End )
                            ? remainingSequence.Slice( remainingSequence.End )
                            : remainingSequence.Slice( remainingSequence.GetPosition( 1, position.Value ) );

                        if( ++lines % LineCountInterval == 0 )
                        {
                            int currentTicks = Environment.TickCount;
                            int ticksSinceLastLineCount = currentTicks - ticksAtLastLineCount;

                            processor.Progress(
                                false,
                                lines,
                                currentTicks - ticksAtStart,
                                LineCountInterval,
                                ticksSinceLastLineCount );
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
    }
}
