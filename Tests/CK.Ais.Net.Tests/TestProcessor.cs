// <copyright file="TestProcessor.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Text;

namespace Ais.Net.Tests;

/// <summary>
/// Fake <see cref="INmeaAisMessageStreamProcessor{TExtraFieldParser}"/> that records every call
/// so that tests can check what the parsers and adapters reported.
/// </summary>
/// <remarks>
/// Like a real processor, it refuses <see cref="OnNext"/> after <see cref="OnCompleted"/>
/// and a second <see cref="OnCompleted"/>: this checks that the producers respect the protocol.
/// </remarks>
public sealed class TestProcessor : INmeaAisMessageStreamProcessor<DefaultExtraFieldParser>
{
    /// <summary>
    /// A reassembled AIS message, with the tag block data of its first NMEA line.
    /// </summary>
    public sealed record Message(
        long? UnixTimestamp,
        int? Source,
        string AsciiPayload,
        uint Padding,
        bool IsFixedMessage,
        int SentencesInGroup );

    /// <summary>
    /// The arguments of a <see cref="Progress"/> call.
    /// </summary>
    public sealed record ProgressReport(
        bool Done,
        int TotalNmeaLines,
        int TotalAisMessages,
        int TotalTicks,
        int NmeaLinesSinceLastUpdate,
        int AisMessagesSinceLastUpdate,
        int TicksSinceLastUpdate );

    /// <summary>
    /// The arguments of an <see cref="OnError"/> call.
    /// </summary>
    public sealed record ErrorReport( string Line, Exception Error, int LineNumber );

    public List<Message> OnNextCalls { get; } = [];

    public List<ProgressReport> ProgressCalls { get; } = [];

    public List<ErrorReport> OnErrorCalls { get; } = [];

    public bool IsComplete { get; private set; }

    public void OnCompleted()
    {
        if( IsComplete )
        {
            throw new InvalidOperationException( $"Must not call {nameof( OnCompleted )} more than once" );
        }
        IsComplete = true;
    }

    public void OnError( in ReadOnlySpan<byte> line, Exception error, int lineNumber )
    {
        OnErrorCalls.Add( new ErrorReport( Encoding.ASCII.GetString( line ), error, lineNumber ) );
    }

    public void OnNext( in NmeaLineParser<DefaultExtraFieldParser> firstLine, in ReadOnlySpan<byte> asciiPayload, uint padding )
    {
        if( IsComplete )
        {
            throw new InvalidOperationException( $"Must not call {nameof( OnNext )} after calling {nameof( OnCompleted )}" );
        }

        OnNextCalls.Add( new Message(
            firstLine.TagBlock.UnixTimestamp,
            firstLine.TagBlock.Source.IsEmpty
                ? null
                : Utf8Parser.TryParse( firstLine.TagBlock.Source, out int sourceId, out _ )
                    ? sourceId
                    : throw new ArgumentException( "Test must supply valid source" ),
            Encoding.ASCII.GetString( asciiPayload ),
            padding,
            firstLine.IsFixedMessage,
            firstLine.TagBlock.SentenceGrouping.HasValue ? firstLine.TagBlock.SentenceGrouping.Value.SentencesInGroup : 0 ) );
    }

    public void Progress( bool done,
                          int totalNmeaLines,
                          int totalAisMessages,
                          int totalTicks,
                          int nmeaLinesSinceLastUpdate,
                          int aisMessagesSinceLastUpdate,
                          int ticksSinceLastUpdate )
    {
        ProgressCalls.Add( new ProgressReport( done, totalNmeaLines, totalAisMessages, totalTicks, nmeaLinesSinceLastUpdate, aisMessagesSinceLastUpdate, ticksSinceLastUpdate ) );
    }
}
