// <copyright file="NmeaLineToAisStreamAdapterSpecsSteps.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using NUnit.Framework;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Text;
using TechTalk.SpecFlow;

namespace Ais.Net.Specs;

[Binding]
public class NmeaAisMessageStreamProcessorBindings
{
    readonly MessageProcessor<DefaultExtraFieldParser> _processor;

    public NmeaAisMessageStreamProcessorBindings()
    {
        _processor = new MessageProcessor<DefaultExtraFieldParser>( this );
    }

    public INmeaAisMessageStreamProcessor<DefaultExtraFieldParser> Processor => _processor;

    public List<Message> OnNextCalls { get; } = [];

    public List<ProgressReport> ProgressCalls { get; } = [];

    public List<ErrorReport> OnErrorCalls { get; } = [];

    public bool IsComplete { get; private set; }

    [Then( "INmeaAisMessageStreamProcessor.OnNext should have been called (.*) time" )]
    [Then( "INmeaAisMessageStreamProcessor.OnNext should have been called (.*) times" )]
    public void ThenTheAisMessageProcessorShouldReceiveMessages( int messageCount )
    {
        Assert.AreEqual( messageCount, OnNextCalls.Count );
    }

    [Then( "in ais message (.*) the payload should be '(.*)' with padding of (.*)" )]
    public void ThenAisPayloadShouldBeWithPaddingOf( int callIndex, string payloadAscii, int padding )
    {
        Assert.AreEqual( payloadAscii, OnNextCalls[callIndex].AsciiPayload );
        Assert.AreEqual( padding, OnNextCalls[callIndex].Padding );
    }

    [Then( "in ais message (.*) the source from the first NMEA line should be (.*)" )]
    public void ThenInAisMessageTheSourceFromTheFirstNMEALineShouldBe( int callIndex, int source )
    {
        Assert.IsTrue( OnNextCalls[callIndex].Source.HasValue );
        Assert.AreEqual( source, OnNextCalls[callIndex].Source!.Value );
    }

    [Then( "in ais message (.*) the timestamp from the first NMEA line should be (.*)" )]
    public void ThenInAisMessageTheTimestampFromTheFirstNMEALineShouldBe( int callIndex, int timestamp )
    {
        Assert.AreEqual( timestamp, OnNextCalls[callIndex].UnixTimestamp );
    }

    [Then( "in ais message (.*) the isfixedmessage from the first NMEA line should be (.*)" )]
    public void ThenInAisMessageTheIsfixedmessageFromTheFirstNMEALineShouldBe( int callIndex, bool isfixedmessage )
    {
        Assert.AreEqual( isfixedmessage, OnNextCalls[callIndex].IsFixedMessage );
    }

    [Then( "in ais message (.*) the sentencesingroup from the first NMEA line should be (.*)" )]
    public void ThenInAisMessageTheSentencesInGroupFromTheFirstNMEALineShouldBe( int callIndex, int sentencesInGroup )
    {
        Assert.AreEqual( sentencesInGroup, OnNextCalls[callIndex].SentencesInGroup );
    }

    [Then( "in ais")]

    [Then( "INmeaAisMessageStreamProcessor.Progress should have been called (.*) times" )]
    public void ThenTheAisMessageProcessorShouldReceiveProgressReports( int callCount )
    {
        Assert.AreEqual( callCount, ProgressCalls.Count );
    }

    [Then( "INmeaAisMessageStreamProcessor.OnError should have been called (.*) times" )]
    [Then( "INmeaAisMessageStreamProcessor.OnError should have been called (.*) time" )]
    public void ThenTheAisMessageProcessorShouldReceiveAnErrorReport( int errorCount )
    {
        Assert.AreEqual( errorCount, OnErrorCalls.Count );
    }

    [Then( "the message error report (.*) should include the problematic line '(.*)'" )]
    public void ThenTheMessageErrorReportShouldIncludeTheProblematicLine( int errorCallNumber, string line )
    {
        ErrorReport call = OnErrorCalls[errorCallNumber];
        Assert.AreEqual( line, call.Line );
    }

    [Then( "the message error report (.*) should include an exception reporting unexpected padding on a non-terminal message fragment" )]
    public void ThenTheMessageErrorReportShouldIncludeAnExceptionReportingUnexpectedPaddingOnANon_TerminalMessageFragment( int errorCallNumber )
    {
        ErrorReport call = OnErrorCalls[errorCallNumber];
        Assert.IsInstanceOf<ArgumentException>( call.Error );

        var e = (ArgumentException)call.Error;
        Assert.AreEqual( "Can only handle non-zero padding on the final message in a fragment", e.Message );
    }

    [Then( "the message error report (.*) should include an exception reporting that it has received two message fragments with the same group id and position" )]
    public void ThenTheMessageErrorReportShouldIncludeAnExceptionReportingThatItHasReceivedTwoMessageFragmentsWithTheSameGroupIdAndPosition( int errorCallNumber )
    {
        ErrorReport call = OnErrorCalls[errorCallNumber];
        Assert.IsInstanceOf<ArgumentException>( call.Error );

        var e = (ArgumentException)call.Error;
        const string expectedStart = "Already received sentence ";
        Assert.AreEqual( expectedStart, e.Message.Substring( 0, expectedStart.Length ) );
    }

    [Then( "the message error report (.*) should include an exception reporting that it received an incomplete set of fragments for a message" )]
    public void ThenTheMessageErrorReportShouldIncludeAnExceptionReportingThatItReceivedAnIncompleteSetOfFragmentsForAMessage( int errorCallNumber )
    {
        ErrorReport call = OnErrorCalls[errorCallNumber];
        Assert.IsInstanceOf<ArgumentException>( call.Error );

        var e = (ArgumentException)call.Error;
        Assert.AreEqual( "Received incomplete fragmented message.", e.Message );
    }

    [Then( "the message error report (.*) should include the line number (.*)" )]
    public void ThenTheMessageErrorReportShouldIncludeTheLineNumber( int errorCallNumber, int lineNumber )
    {
        ErrorReport call = OnErrorCalls[errorCallNumber];
        Assert.AreEqual( lineNumber, call.LineNumber );
    }

    [Then( "the message error report (.*) should include an exception reporting that the message appears to be missing some characters" )]
    public void ThenTheMessageErrorReportShouldIncludeAnExceptionReportingThatTheMessageAppearsToBeMissingSomeCharacters( int errorCallNumber )
    {
        ErrorReport call = OnErrorCalls[errorCallNumber];
        Assert.AreEqual( "Invalid data. The message appears to be missing some characters - it may have been corrupted or truncated.", call.Error.Message );
    }

    [Then( "progress report (.*) was (.*), (.*), (.*), (.*), (.*), (.*), (.*)" )]
    public void ThenProgressReportWasFalse(
        int callIndex,
        bool done,
        int totalNmeaLines,
        int totalAisMessages,
        int totalTicks,
        int nmeaLinesSinceLastUpdate,
        int aisMessagesSinceLastUpdate,
        int ticksSinceLastUpdate )
    {
        ProgressReport call = ProgressCalls[callIndex];
        Assert.AreEqual( done, call.Done );
        Assert.AreEqual( totalNmeaLines, call.TotalNmeaLines );
        Assert.AreEqual( totalAisMessages, call.TotalAisMessages );
        Assert.AreEqual( totalTicks, call.TotalTicks );
        Assert.AreEqual( nmeaLinesSinceLastUpdate, call.NmeaLinesSinceLastUpdate );
        Assert.AreEqual( aisMessagesSinceLastUpdate, call.AisMessagesSinceLastUpdate );
        Assert.AreEqual( ticksSinceLastUpdate, call.TicksSinceLastUpdate );
    }

    public record Message(
        long? UnixTimestamp,
        int? Source,
        string AsciiPayload,
        uint Padding,
        bool IsFixedMessage,
        int SentencesInGroup );

    public record ProgressReport(
            bool Done,
            int TotalNmeaLines,
            int TotalAisMessages,
            int TotalTicks,
            int NmeaLinesSinceLastUpdate,
            int AisMessagesSinceLastUpdate,
            int TicksSinceLastUpdate );

    public class ErrorReport
    {
        public ErrorReport( string line, Exception error, int lineNumber )
        {
            Line = line;
            Error = error;
            LineNumber = lineNumber;
        }

        public string Line { get; }

        public Exception Error { get; }

        public int LineNumber { get; }
    }

    class MessageProcessor<TExtraFieldParser> : INmeaAisMessageStreamProcessor<TExtraFieldParser>
        where TExtraFieldParser : struct, INmeaTagBlockExtraFieldParser
    {
        readonly NmeaAisMessageStreamProcessorBindings _parent;

        public MessageProcessor( NmeaAisMessageStreamProcessorBindings nmeaAisMessageStreamProcessorBindings )
        {
            _parent = nmeaAisMessageStreamProcessorBindings;
        }

        public void OnCompleted()
        {
            if( _parent.IsComplete )
            {
                throw new InvalidOperationException( $"Must not call {nameof( OnCompleted )} more than once" );
            }

            _parent.IsComplete = true;
        }

        public void OnError( in ReadOnlySpan<byte> line, Exception error, int lineNumber )
        {
            _parent.OnErrorCalls.Add( new ErrorReport( Encoding.ASCII.GetString( line ), error, lineNumber ) );
        }

        public void OnNext(
            in NmeaLineParser<TExtraFieldParser> firstLine,
            in ReadOnlySpan<byte> asciiPayload,
            uint padding )
        {
            if( _parent.IsComplete )
            {
                throw new InvalidOperationException( $"Must not call {nameof( OnNext )} after calling {nameof( OnCompleted )}" );
            }

            _parent.OnNextCalls.Add( new Message(
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

        public void Progress(
            bool done,
            int totalNmeaLines,
            int totalAisMessages,
            int totalTicks,
            int nmeaLinesSinceLastUpdate,
            int aisMessagesSinceLastUpdate,
            int ticksSinceLastUpdate )
        {
            _parent.ProgressCalls.Add( new ProgressReport( done, totalNmeaLines, totalAisMessages, totalTicks, nmeaLinesSinceLastUpdate, aisMessagesSinceLastUpdate, ticksSinceLastUpdate ) );
        }
    }
}
