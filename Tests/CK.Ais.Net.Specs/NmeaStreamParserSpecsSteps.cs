// <copyright file="NmeaStreamParserSpecsSteps.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace Ais.Net.Specs;

[Binding]
public class NmeaStreamParserSpecsSteps
{
    readonly StringBuilder _content = new();
    readonly LineProcessor<DefaultExtraFieldParser> _lineProcessor = new();
    readonly NmeaAisMessageStreamProcessorBindings _messageProcessor;
    readonly NmeaParserOptions _parserOptions = new();

    public NmeaStreamParserSpecsSteps( NmeaAisMessageStreamProcessorBindings messageProcessor )
    {
        _messageProcessor = messageProcessor;
    }

    [Given( "no content" )]
    public static void GivenNoContent()
    {
        // Nothing to do here.
    }

    [Given( "a line '(.*)'" )]
    public void GivenALine( string line )
    {
        _content.Append( line );
        _content.Append( '\n' );
    }

    [Given( "a CR line '(.*)'" )]
    public void GivenACrLine( string line )
    {
        _content.Append( line );
        _content.Append( '\r' );
    }

    [Given( "a CRLF line '(.*)'" )]
    public void GivenACrlfLine( string line )
    {
        _content.Append( line );
        _content.Append( "\r\n" );
    }

    [Given( "an unterminated line '(.*)'" )]
    public void GivenAnUnterminatedLine( string line )
    {
        _content.Append( line );
    }

    [When( "I parse the content by line" )]
    public async Task WhenIParseTheContentByLineAsync()
    {
        await NmeaStreamParser.ParseStreamAsync(
            new MemoryStream( Encoding.ASCII.GetBytes( _content.ToString() ) ),
            _lineProcessor, _parserOptions ).ConfigureAwait( false );
    }

    [Given( "I have configured a AllowUnreconizedTalkerId of (.*)" )]
    public void GivenIHaveConfiguredAAllowUnreconizedTalkerIdOf( bool value )
    {
        _parserOptions.AllowUnreconizedTalkerId = value;
    }

    [Given( "I have configured a AllowUnreconizedDataOrigin of (.*)" )]
    public void GivenIHaveConfiguredAAllowUnreconizedDataOriginOf( bool value )
    {
        _parserOptions.AllowUnreconizedDataOrigin = value;
    }

    [When( "I parse the content by message" )]
    public async Task WhenIParseTheContentByMessageAsync()
    {
        await NmeaStreamParser.ParseStreamAsync(
            new MemoryStream( Encoding.ASCII.GetBytes( _content.ToString() ) ),
            new NmeaLineToAisStreamAdapter<DefaultExtraFieldParser>( _messageProcessor.Processor ) ).ConfigureAwait( false );
    }

    [When( "I parse the content by message with exceptions disabled" )]
    public async Task WhenIParseTheContentByMessageWithExceptionsDisabledAsync()
    {
        var options = new NmeaParserOptions { ThrowWhenTagBlockContainsUnknownFields = false };
        await NmeaStreamParser.ParseStreamAsync(
            new MemoryStream( Encoding.ASCII.GetBytes( _content.ToString() ) ),
            new NmeaLineToAisStreamAdapter<DefaultExtraFieldParser>( _messageProcessor.Processor, options ),
            options ).ConfigureAwait( false );
    }

    [Then( @"INmeaLineStreamProcessor\.OnComplete should have been called" )]
    public void ThenOnCompleteShouldHaveBeenCalled()
    {
        Assert.IsTrue( _lineProcessor.IsComplete );
    }

    [Then( @"INmeaAisMessageStreamProcessor\.OnComplete should have been called" )]
    public void ThenINmeaAisMessageStreamProcessor_OnCompleteShouldHaveBeenCalled()
    {
        Assert.IsTrue( _messageProcessor.IsComplete );
    }

    [Then( "INmeaLineStreamProcessor.OnNext should have been called (.*) times" )]
    [Then( "INmeaLineStreamProcessor.OnNext should have been called (.*) time" )]
    public void ThenOnNextShouldHaveBeenCalledTimes( int count )
    {
        Assert.AreEqual( count, _lineProcessor.OnNextCalls.Count );
    }

    [Then( "OnError should have been called (.*) times" )]
    [Then( "OnError should have been called (.*) time" )]
    public void ThenOnErrorShouldHaveBeenCalledTimes( int count )
    {
        Assert.AreEqual( count, _lineProcessor.OnErrorCalls.Count );
    }

    [Then( "line (.*) should have a tag block of '(.*)' and a sentence of '(.*)'" )]
    public void ThenLineShouldHaveATagBlockOfAndASentenceOf( int line, string tagBlock, string sentence )
    {
        LineProcessor<DefaultExtraFieldParser>.Line call = _lineProcessor.OnNextCalls[line];
        Assert.AreEqual( tagBlock, call.TagBlock );
        Assert.AreEqual( sentence, call.Sentence );
    }

    [Then( "line (.*) should AisTakler of (.*), a DataOrigin of (.*) and a SentenceFormatter of (.*)" )]
    public void ThenLineShouldHaveAAisTalkerOfADataOrigineOfAndASentenceFormatterOf( int line, TalkerId talkerId, VesselDataOrigin dataOrigin, string sentenceFormatter )
    {
        LineProcessor<DefaultExtraFieldParser>.Line call = _lineProcessor.OnNextCalls[line];
        Assert.AreEqual( talkerId, call.TalkerId );
        Assert.AreEqual( dataOrigin, call.DataOrigin );
        Assert.AreEqual( sentenceFormatter, call.SentenceFormatter );
    }

    [Then( "the line error report (.*) should include the problematic line '(.*)'" )]
    public void ThenTheLineErrorReportShouldIncludeTheProblematicLine( int errorCallNumber, string line )
    {
        LineProcessor<DefaultExtraFieldParser>.ErrorReport call = _lineProcessor.OnErrorCalls[errorCallNumber];
        Assert.AreEqual( line, call.Line );
    }

    [Then( "the line error report (.*) should include an exception reporting that the expected exclamation mark is missing" )]
    public void ThenTheLineErrorReportShouldIncludeAnExceptionReportingThatTheExpectedExclamationMarkIsMissing( int errorCallNumber )
    {
        LineProcessor<DefaultExtraFieldParser>.ErrorReport call = _lineProcessor.OnErrorCalls[errorCallNumber];
        Assert.IsInstanceOf<ArgumentException>( call.Error );

        var e = (ArgumentException)call.Error;
        Assert.AreEqual( "Invalid data. Expected '!' at sentence start", e.Message );
    }

    [Then( "the message error report (.*) should include an exception reporting that the message appears to be incomplete" )]
    public void ThenTheMessageErrorReportShouldIncludeAnExceptionReportingThatTheMessageAppearsToBeTruncated( int errorCallNumber )
    {
        NmeaAisMessageStreamProcessorBindings.ErrorReport call = _messageProcessor.OnErrorCalls[errorCallNumber];
        Assert.IsInstanceOf<ArgumentException>( call.Error );

        var e = (ArgumentException)call.Error;
        Assert.AreEqual( "Invalid data. The message appears to be missing some characters - it may have been corrupted or truncated.", e.Message );
    }

    [Then( "the message error report (.*) should include an exception reporting that the padding is missing" )]
    public void ThenTheMessageErrorReportShouldIncludeAnExceptionReportingThatThePaddingIsMissing( int errorCallNumber )
    {
        NmeaAisMessageStreamProcessorBindings.ErrorReport call = _messageProcessor.OnErrorCalls[errorCallNumber];
        Assert.IsInstanceOf<ArgumentException>( call.Error );

        var e = (ArgumentException)call.Error;
        Assert.AreEqual( "Invalid data. Payload padding field not present - the message may have been corrupted or truncated", e.Message );
    }

    [Then( "the message error report (.*) should include an exception reporting that the checksum is missing" )]
    public void ThenTheMessageErrorReportShouldIncludeAnExceptionReportingThatTheChecksumIsMissing( int errorCallNumber )
    {
        NmeaAisMessageStreamProcessorBindings.ErrorReport call = _messageProcessor.OnErrorCalls[errorCallNumber];
        Assert.IsInstanceOf<ArgumentException>( call.Error );

        var e = (ArgumentException)call.Error;
        Assert.AreEqual( "Invalid data. Payload checksum not present - the message may have been corrupted or truncated", e.Message );
    }

    [Then( "the message error report (.*) should include an exception reporting that the expected exclamation mark is missing" )]
    public void ThenTheMessageErrorReportShouldIncludeAnExceptionReportingThatTheExpectedExclamationMarkIsMissing( int errorCallNumber )
    {
        NmeaAisMessageStreamProcessorBindings.ErrorReport call = _messageProcessor.OnErrorCalls[errorCallNumber];
        Assert.IsInstanceOf<ArgumentException>( call.Error );

        var e = (ArgumentException)call.Error;
        Assert.AreEqual( "Invalid data. Expected '!' at sentence start", e.Message );
    }

    [Then( "the message error report (.*) should include an exception reporting that an unrecognized field is present" )]
    public void WhenTheMessageErrorReportShouldIncludeAnExceptionReportingThatAnUnrecognizedFieldIsPresent( int errorCallNumber )
    {
        NmeaAisMessageStreamProcessorBindings.ErrorReport call = _messageProcessor.OnErrorCalls[errorCallNumber];
        Assert.IsInstanceOf<ArgumentException>( call.Error );

        var e = (ArgumentException)call.Error;
        const string expectedStart = "Unknown field type:";
        Assert.AreEqual( expectedStart, e.Message.Substring( 0, expectedStart.Length ) );
    }

    [Then( "the message error report (.*) should include an exception reporting that an unsupported field is present" )]
    public void ThenTheMessageErrorReportShouldIncludeAnExceptionReportingThatAnUnsupportedFieldIsPresent( int errorCallNumber )
    {
        NmeaAisMessageStreamProcessorBindings.ErrorReport call = _messageProcessor.OnErrorCalls[errorCallNumber];
        Assert.IsInstanceOf<NotSupportedException>( call.Error );

        var e = (NotSupportedException)call.Error;
        const string expectedStart = "Unsupported field type:";
        Assert.AreEqual( expectedStart, e.Message.Substring( 0, expectedStart.Length ) );
    }

    [Then( "the line error report (.*) should include the line number (.*)" )]
    public void ThenTheLineErrorReportShouldIncludeTheLineNumber( int errorCallNumber, int lineNumber )
    {
        LineProcessor<DefaultExtraFieldParser>.ErrorReport call = _lineProcessor.OnErrorCalls[errorCallNumber];
        Assert.AreEqual( lineNumber, call.LineNumber );
    }

    [Then( "the line error report (.*) should include an exception reporting an invalid talker data origin" )]
    public void ThenTheMessageErrorReportShouldIncludeAnExceptionReportingAnInvalidTalkerDataOrigin( int errorCallNumber )
    {
        var call = _lineProcessor.OnErrorCalls[errorCallNumber];
        Assert.AreEqual( "Invalid data. Unrecognized origin in AIS talker ID - must be VDM or VDO", call.Error.Message );
    }

    [Then( "the line error report (.*) should include an exception reporting an invalid talker id with invalid char '(.*)'" )]
    public void ThenTheMessageErrorReportShouldIncludeAnExceptionReportingAnInvalidTalkerId( int errorCallNumber, string invalidChar )
    {
        var call = _lineProcessor.OnErrorCalls[errorCallNumber];
        Assert.AreEqual( "Invalid data. Unrecognized talker id - cannot end with " + invalidChar, call.Error.Message );
    }

    class LineProcessor<TExtraFieldParser> : INmeaLineStreamProcessor<TExtraFieldParser>
        where TExtraFieldParser : struct, INmeaTagBlockExtraFieldParser
    {
        public bool IsComplete { get; private set; }

        public List<Line> OnNextCalls { get; } = [];

        public List<ErrorReport> OnErrorCalls { get; } = [];

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

        public void OnNext( in NmeaLineParser<TExtraFieldParser> value, int lineNumber )
        {
            if( IsComplete )
            {
                throw new InvalidOperationException( $"Must not call {nameof( OnNext )} after calling {nameof( OnCompleted )}" );
            }

            OnNextCalls.Add( new Line(
                Encoding.ASCII.GetString( value.TagBlockAsciiWithoutDelimiters ),
                Encoding.ASCII.GetString( value.Sentence ),
                value.AisTalker,
                value.DataOrigin,
                Encoding.ASCII.GetString( value.SentenceFormatter ) ) );
        }

        public void Progress( bool done, int totalLines, int totalTicks, int linesSinceLastUpdate, int ticksSinceLastUpdate )
        {
        }

        public record Line( string TagBlock, string Sentence, TalkerId TalkerId, VesselDataOrigin DataOrigin, string SentenceFormatter );

        public record ErrorReport( string Line, Exception Error, int LineNumber );
    }
}
