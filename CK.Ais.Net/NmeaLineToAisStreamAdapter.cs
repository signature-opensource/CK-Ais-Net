// <copyright file="NmeaLineToAisStreamAdapter.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;

namespace Ais.Net;

public interface INmeaLineToAisStreamAdapter : INmeaLineStreamProcessor, IDisposable
{
}

/// <summary>
/// Processes NMEA message lines, and passes their payloads as complete AIS messages to an
/// <see cref="INmeaAisMessageStreamProcessor"/>, reassembling any messages that were split
/// across multiple NMEA lines.
/// </summary>
public class NmeaLineToAisStreamAdapter<TExtraFieldParser> : INmeaLineStreamProcessor<TExtraFieldParser>, INmeaLineToAisStreamAdapter
    where TExtraFieldParser : struct, INmeaTagBlockExtraFieldParser
{
    readonly INmeaAisMessageStreamProcessor<TExtraFieldParser> _messageProcessor;
    readonly NmeaParserOptions _options;
    readonly Dictionary<int, FragmentedMessage> _messageFragments = [];
    int _messagesProcessed = 0;
    int _messagesProcessedAtLastUpdate = 0;
    readonly bool _clearReturedBuffer;

    /// <summary>
    /// Creates a <see cref="NmeaLineToAisStreamAdapter"/>.
    /// </summary>
    /// <param name="messageProcessor">
    /// The message process to which complete AIS messages are to be passed.
    /// </param>
    public NmeaLineToAisStreamAdapter( INmeaAisMessageStreamProcessor<TExtraFieldParser> messageProcessor )
        : this( messageProcessor, new NmeaParserOptions() )
    {
    }

    /// <summary>
    /// Creates a <see cref="NmeaLineToAisStreamAdapter"/>.
    /// </summary>
    /// <param name="messageProcessor">
    /// The message process to which complete AIS messages are to be passed.
    /// </param>
    /// <param name="options">Configures parser behaviour.</param>
    public NmeaLineToAisStreamAdapter( INmeaAisMessageStreamProcessor<TExtraFieldParser> messageProcessor, NmeaParserOptions options )
    {
        _messageProcessor = messageProcessor;
        _options = options.Copy();
        _clearReturedBuffer = (options.ChecksumOption & (ChecksumOption.ValidateStandardFormat | ChecksumOption.CheckValidity)) != 0;
    }

    /// <inheritdoc/>
    public void OnCompleted()
    {
        _messageProcessor.OnCompleted();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        FreeRentedBuffers();
    }

    /// <inheritdoc/>
    public void OnNext( in NmeaLineParser<TExtraFieldParser> parsedLine, int lineNumber )
    {
        // Work out whether this is a fragmented message.
        // There are two different ways to indicate fragmentation: in the AIS sentence, or in
        // the NMEA header. Some systems do not provide any NMEA-header-level fragmentation
        // information, in which case we must rely on the AIS level. The AIS layer will always
        // report fragmentation, so you might think that it would be OK always to use that, and
        // to ignore the NMEA header grouping information. However, in cases where both are
        // available, we prefer the NMEA header grouping information.
        // The NMEA header is prefereable because in an AIS sentence, the group IDs can only
        // be in the range 1-9, whereas the NMEA header allows longer IDs.
        // There's a possibility of false aliases (fragments of different messages having the
        // same id) when we rely on the AIS id. Base stations that combine and relay messages
        // should take steps to avoid that, but in cases where the base station has added a
        // group ID in the sentence header, it might be relying on that for uniqueness, and
        // therefore not bothering to adjust within the AIS layer.
        bool sentenceGroupHeaderPresent =
            parsedLine.TagBlockAsciiWithoutDelimiters.Length > 0 &&
            parsedLine.TagBlock.SentenceGrouping.HasValue;
        bool sentenceIsFragment = sentenceGroupHeaderPresent || parsedLine.TotalFragmentCount > 1;

        if( sentenceIsFragment )
        {
            bool isLastSentenceInGroup;
            int groupId;
            int sentencesInGroup;
            int oneBasedSentenceNumber;
            bool isDesyncGroup = false;
            if( sentenceGroupHeaderPresent )
            {
                NmeaTagBlockSentenceGrouping sentenceGrouping = parsedLine.TagBlock.SentenceGrouping!.Value;

                groupId = sentenceGrouping.GroupId;
                oneBasedSentenceNumber = sentenceGrouping.SentenceNumber;
                sentencesInGroup = sentenceGrouping.SentencesInGroup;
                isDesyncGroup = sentencesInGroup != parsedLine.TotalFragmentCount;
            }
            else
            {
                groupId = parsedLine.MultiSequenceMessageId[0];
                oneBasedSentenceNumber = parsedLine.FragmentNumberOneBased;
                sentencesInGroup = parsedLine.TotalFragmentCount;
            }

            isLastSentenceInGroup = oneBasedSentenceNumber == sentencesInGroup;

            if( !isDesyncGroup && !isLastSentenceInGroup && parsedLine.Padding != 0 && !_options.AllowNonZeroPaddingInNonLastFragment )
            {
                _messageProcessor.OnError(
                    parsedLine.Line,
                    new ArgumentException( "Can only handle non-zero padding on the final message in a fragment" ),
                    lineNumber );
                // To not throw an "Received incomplete fragmented message." error, collect all fragments
                // from this group and when all fragments are received, just not call OnNext.
            }

            if( !_messageFragments.TryGetValue( groupId, out FragmentedMessage fragments ) )
            {
                fragments = new FragmentedMessage( sentencesInGroup, lineNumber );
                _messageFragments.Add( groupId, fragments );
            }

            Span<byte[]> fragmentBuffers = fragments.Buffers;
            if( fragmentBuffers[oneBasedSentenceNumber - 1] != null )
            {
                _messageProcessor.OnError(
                    parsedLine.Line,
                    new ArgumentException( $"Already received sentence {oneBasedSentenceNumber} for group {groupId}" ),
                    lineNumber );
            }

            byte[] buffer = ArrayPool<byte>.Shared.Rent( parsedLine.Line.Length );
            fragmentBuffers[oneBasedSentenceNumber - 1] = buffer;
            parsedLine.Line.CopyTo( buffer );

            bool allFragmentsReceived = true;
            int totalPayloadSize = 0;
            int fragmentsWithSentences = 0;
            for( int i = 0; i < fragmentBuffers.Length; ++i )
            {
                if( fragmentBuffers[i] == null )
                {
                    allFragmentsReceived = false;
                    break;
                }

                var storedParsedLine = new NmeaLineParser<TExtraFieldParser>( fragmentBuffers[i], _options );
                totalPayloadSize += storedParsedLine.Payload.Length;

                if( storedParsedLine.Sentence.Length > 0 ) fragmentsWithSentences++;
            }

            if( allFragmentsReceived )
            {
                bool fixGrouping = false;
                NmeaTagBlockSentenceGrouping? customGroup = null;
                if( fragmentsWithSentences < fragmentBuffers.Length && _options.EmptyGroupTolerance == EmptyGroupTolerance.AutoFix )
                {
                    fixGrouping = true;
                    if( fragmentsWithSentences > 1 ) customGroup = new NmeaTagBlockSentenceGrouping( 1, fragmentsWithSentences, groupId );
                }

                byte[]? reassemblyUnderlyingArray = null;
                try
                {
                    reassemblyUnderlyingArray = ArrayPool<byte>.Shared.Rent( totalPayloadSize );
                    int reassemblyIndex = 0;
                    Span<byte> reassemblyBuffer = reassemblyUnderlyingArray.AsSpan().Slice( 0, totalPayloadSize );
                    uint finalPadding = 0;
                    bool nonLastFragmentHaveNonZeroPadding = false;

                    for( int i = 0; i < fragmentBuffers.Length; ++i )
                    {
                        var storedParsedLine = new NmeaLineParser<TExtraFieldParser>( fragmentBuffers[i], _options );

                        // If a non last fragment have a non zero padding, disallow it and not in fix grouping mode,
                        // then not populate reassemblyUnderlyingArray and not call OnNext.
                        if( !_options.AllowNonZeroPaddingInNonLastFragment && i < fragmentBuffers.Length - 1 && storedParsedLine.Padding != 0 && !fixGrouping )
                        {
                            nonLastFragmentHaveNonZeroPadding = true;
                            break;
                        }

                        ReadOnlySpan<byte> payload = storedParsedLine.Payload;
                        payload.CopyTo( reassemblyBuffer.Slice( reassemblyIndex, payload.Length ) );
                        reassemblyIndex += payload.Length;
                        if( payload.Length > 0 ) finalPadding = storedParsedLine.Padding;
                    }

                    if( !nonLastFragmentHaveNonZeroPadding )
                    {
                        var lineParser = new NmeaLineParser<TExtraFieldParser>( fragmentBuffers[0], _options );
                        if( fixGrouping ) lineParser = NmeaLineParser<TExtraFieldParser>.OverrideGrouping( lineParser, customGroup );

                        _messageProcessor.OnNext(
                            lineParser,
                            reassemblyBuffer.Slice( 0, totalPayloadSize ),
                            finalPadding );
                        _messagesProcessed += 1;
                    }
                }
                finally
                {
                    if( reassemblyUnderlyingArray is not null )
                    {
                        ArrayPool<byte>.Shared.Return( reassemblyUnderlyingArray, _clearReturedBuffer );
                    }
                }

                FreeMessageFragments( groupId );
            }
        }
        else
        {
            _messageProcessor.OnNext( parsedLine, parsedLine.Payload, parsedLine.Padding );
            _messagesProcessed += 1;
        }

        if( _messageFragments.Count > 0 )
        {
            Span<int> fragmentGroupIdsToRemove = stackalloc int[_messageFragments.Count];
            int fragmentToRemoveCount = 0;
            foreach( KeyValuePair<int, FragmentedMessage> kv in _messageFragments )
            {
                int sentencesSinceFirstFragment = lineNumber - kv.Value.LineNumber;
                if( sentencesSinceFirstFragment > _options.MaximumUnmatchedFragmentAge )
                {
                    fragmentGroupIdsToRemove[fragmentToRemoveCount++] = kv.Key;
                }
            }

            for( int i = 0; i < fragmentToRemoveCount; ++i )
            {
                int groupId = fragmentGroupIdsToRemove[i];
                FragmentedMessage fragmentedMessage = _messageFragments[groupId];
                Span<byte[]> fragmentBuffers = fragmentedMessage.Buffers;

                // Find the last non-null entry in the list. (It would be easier to use LINQ's
                // Last operator, but this we way avoid the allocations inherent in Last,
                // although since this is a case we don't expect to hit much in normal
                // operation, it's not clear how much that matters.)
                byte[]? lastFragmentBuffer = null;
                for( int o = fragmentBuffers.Length - 1; lastFragmentBuffer is null; --o )
                {
                    lastFragmentBuffer = fragmentBuffers[o];
                }

                ReadOnlySpan<byte> line = lastFragmentBuffer;
                int endOfMessages = line.IndexOf( (byte)0 );
                if( endOfMessages >= 0 )
                {
                    line = line.Slice( 0, endOfMessages );
                }

                _messageProcessor.OnError(
                    line,
                    new ArgumentException( "Received incomplete fragmented message." ),
                    fragmentedMessage.LineNumber );

                FreeMessageFragments( groupId );
            }
        }
    }

    /// <inheritdoc/>
    public void OnError( in ReadOnlySpan<byte> line, Exception error, int lineNumber )
    {
        _messageProcessor.OnError( line, error, lineNumber );
    }

    /// <inheritdoc/>
    public void Progress( bool done, int totalLines, int totalTicks, int linesSinceLastUpdate, int ticksSinceLastUpdate )
    {
        _messageProcessor.Progress(
            done,
            totalLines,
            _messagesProcessed,
            totalTicks,
            linesSinceLastUpdate,
            _messagesProcessed - _messagesProcessedAtLastUpdate,
            ticksSinceLastUpdate );
        _messagesProcessedAtLastUpdate = _messagesProcessed;
    }

    void FreeRentedBuffers()
    {
        if( _messageFragments.Count > 0 )
        {
            int[] groupIds = _messageFragments.Keys.ToArray();
            Console.WriteLine( $"{groupIds.Length} message groups with missing fragments" );
            for( int i = 0; i < groupIds.Length; ++i )
            {
                Console.WriteLine( $"Partial message, group id {groupIds[i]}" );

                FreeMessageFragments( groupIds[i] );
            }
        }
    }

    void FreeMessageFragments( int groupId )
    {
        FragmentedMessage fragmentedMessage = _messageFragments[groupId];
        Span<byte[]> fragmentBuffers = fragmentedMessage.Buffers;
        for( int i = 0; i < fragmentBuffers.Length; ++i )
        {
            if( fragmentBuffers[i] != null )
            {
                ArrayPool<byte>.Shared.Return( fragmentBuffers[i], _clearReturedBuffer );
            }
        }

        ArrayPool<byte[]>.Shared.Return( fragmentedMessage.RentedBufferArray, _clearReturedBuffer );

        _messageFragments.Remove( groupId );
    }

    readonly struct FragmentedMessage
    {
        public FragmentedMessage(
            int count,
            int lineNumber )
        {
            RentedBufferArray = ArrayPool<byte[]>.Shared.Rent( count );
            BufferCount = count;
            LineNumber = lineNumber;

            Buffers.Clear();
        }

        /// <summary>
        /// Gets the underlying array provided by the array pool that backs <see cref="Buffer"/>.
        /// </summary>
        /// <remarks>
        /// This might be larger than we need, because array pool only guarantees to return arrays
        /// that are at least as large as you want. The <see cref="Buffer"/> property provides
        /// correctly range-limited access via a span, but we need to hold onto the underlying
        /// array directly so that we can return it to the pool when we're done.
        /// </remarks>
        public byte[][] RentedBufferArray { get; }

        public int BufferCount { get; }

        /// <summary>
        /// Gets the span for holding references to the buffers for the fragments of this
        /// message.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The entries in this span are null after construction, with buffers obtained for
        /// each fragment as they arrive. (The first will be put in place immediately after
        /// construction, because we only know that we've got a fragmented message as a result
        /// of seeing one of its fragments. But any further fragments get added as they
        /// arrive.)
        /// </para>
        /// <para>
        /// The byte arrays in here are all obtained from <see cref="ArrayPool{T}.Shared"/>,
        /// which has two implications. First, it means we must eventually return them to the
        /// pool, which happens in <see cref="FreeMessageFragments(int)"/>. Second, it means
        /// that each array may well be large than required. This is also the case for the
        /// underlying <see cref="RentedBufferArray"/> that backs the span returned by this
        /// property, but unlike here, where we range-limit the span based on the number of
        /// buffers (<see cref="BufferCount"/>) we don't keep track of the real length of
        /// each individual fragment. That's because we don't have to: we can infer it by
        /// inspecting the fragment contents. When the time for reassembly comes (i.e., once
        /// all the fragments have arrived), we use a <see cref="NmeaLineParser"/> to pull
        /// out the fragment payload, and the parser doesn't care if the buffer containing
        /// the line is longer than it needs to be.
        /// </para>
        /// </remarks>
        public readonly Span<byte[]> Buffers => RentedBufferArray.AsSpan().Slice( 0, BufferCount );

        public int LineNumber { get; }
    }
}
