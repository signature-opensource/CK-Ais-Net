using NUnit.Framework;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace Ais.Net.Specs;

[Binding]
public class NmeaTagBlockParserSpecsSteps
{
    readonly StringBuilder _content = new();
    readonly NmeaAisMessageStreamProcessorBindings _messageProcessor = new();

    ParserMaker? _makeParser;
    ExtraParserMaker? _makeExtraParser;

    delegate NmeaTagBlockParser<DefaultExtraFieldParser> ParserMaker();
    delegate NmeaTagBlockParser<ExtraFieldParser> ExtraParserMaker();

    delegate void ParserTest( NmeaTagBlockParser<DefaultExtraFieldParser> parser );
    delegate void ParserTestExtra( NmeaTagBlockParser<ExtraFieldParser> parser );

    [Given( "the line '(.*)'" )]
    public void GivenALine( string line )
    {
        _content.Append( line );
        _content.Append( '\n' );
    }

    [When( "I parse '(.*)' with throwWhenTagBlockContainsUnknownFields of (.*) and tagBlockStandard of (.*) as a NMEA tag block parser" )]
    public void WhenIParseWithThrowWhenTagBlockContainsUnknownFieldsOfAndTagBlockStandardOfAsANMEATagBlockParser( string messageLine, bool throwWhenTagBlockContainsUnknownFields, TagBlockStandard tagBlockStandard )
    {
        When( messageLine, throwWhenTagBlockContainsUnknownFields, tagBlockStandard, false );
    }

    [When( "I parse the content by message with throwWhenTagBlockContainsUnknownFields of (.*) and tagBlockStandard of (.*)" )]
    public async Task WhenIParseTheContentByMessageAsync( bool throwWhenTagBlockContainsUnknownFields, TagBlockStandard tagBlockStandard )
    {
        await NmeaStreamParser.ParseStreamAsync(
            new MemoryStream( Encoding.ASCII.GetBytes( _content.ToString() ) ),
            new NmeaLineToAisStreamAdapter<DefaultExtraFieldParser>( _messageProcessor.Processor ),
            new NmeaParserOptions { ThrowWhenTagBlockContainsUnknownFields = throwWhenTagBlockContainsUnknownFields, TagBlockStandard = tagBlockStandard } ).ConfigureAwait( false );
    }

    [When( "I parse '(.*)' with throwWhenTagBlockContainsUnknownFields of (.*), tagBlockStandard of (.*) and extra parser" )]
    public void WhenIParseTheContentByMessageWithExtraFieldParser( string messageLine, bool throwWhenTagBlockContainsUnknownFields, TagBlockStandard tagBlockStandard )
    {
        WhenExtra( messageLine, throwWhenTagBlockContainsUnknownFields, tagBlockStandard, false );
    }

    [When( "I parse '(.*)' with allowTagBlockEmptyFields of (.*) and throwWhenTagBlockContainsUnknownFields of (.*)" )]
    public void WhenIParsePayloadWithAllowTagBlockEmptyFieldsOf( string messageLine, bool allowTagBlockEmptyFields, bool throwWhenTagBlockContainsUnknownFields )
    {
        When( messageLine, throwWhenTagBlockContainsUnknownFields, TagBlockStandard.Unspecified, allowTagBlockEmptyFields );
    }

    [Then( "the Source is '(.*)'" )]
    public void ThenSourceIs( string source )
    {
        Then( parser => Assert.AreEqual( source, Encoding.ASCII.GetString( parser.Source ) ) );
    }

    [Then( "the Source is empty" )]
    public void ThenSourceIsEmpty()
    {
        Then( parser => Assert.IsTrue( parser.Source.IsEmpty ) );
    }

    [Then( "the Timestamp is '(.*)'" )]
    public void ThenTimestampIs( double source )
    {
        Then( parser =>
        {
            Assert.IsTrue( parser.UnixTimestamp.HasValue );
            Assert.AreEqual( source, parser.UnixTimestamp!.Value );
        } );
    }

    [Then( "the SentenceGrouping is (.*) (.*) (.*)" )]
    public void ThenSentenceGroupingIs( int sentenceNumber, int sentencesInGroup, int groupId )
    {
        Then( parser =>
        {
            Assert.IsTrue( parser.SentenceGrouping.HasValue );
            Assert.AreEqual( sentenceNumber, parser.SentenceGrouping!.Value.SentenceNumber );
            Assert.AreEqual( sentencesInGroup, parser.SentenceGrouping.Value.SentencesInGroup );
            Assert.AreEqual( groupId, parser.SentenceGrouping.Value.GroupId );
        } );
    }

    [Then( "the SentenceGrouping is null" )]
    public void ThenSentenceGroupingIsNull()
    {
        Then( parser => Assert.IsFalse( parser.SentenceGrouping.HasValue ) );
    }

    [Then( "there are no error" )]
    public void ThereAreNoError()
    {
        Assert.Zero( _messageProcessor.OnErrorCalls.Count );
    }

    [Then( "the Timestamp is null" )]
    public void ThenTimestampIsNull()
    {
        Then( parser => Assert.IsFalse( parser.UnixTimestamp.HasValue ) );
    }

    [Then( "the message error report (.*) should include the error message '(.*)'" )]
    public void ThenTheLineErrorReportShouldIncludeTheProblematicLine( int errorCallNumber, string errorMessage )
    {
        var call = _messageProcessor.OnErrorCalls[errorCallNumber];
        Assert.AreEqual( errorMessage, call.Error.Message );
    }

    [Then( "the TextString is '(.*)'" )]
    public void TheTheTextStringIs( string text )
    {
        Then( parser => Assert.AreEqual( text, Encoding.ASCII.GetString( parser.TextString ) ) );
    }

    [Then( "the TextString is empty" )]
    public void TheTheTextStringIsEmpty()
    {
        Then( parser => Assert.IsTrue( parser.TextString.IsEmpty ) );
    }

    [Then( "the extra field parser q value is '(.*)'" )]
    public void ThenExtraFieldParserQValueIs( string value )
    {
        ThenExtra( parser => Assert.AreEqual( value, Encoding.ASCII.GetString( parser.ExtraFieldParser.GetQValue( parser.OriginalSpan ) ) ) );
    }

    [Then( "the extra field parser v value is '(.*)'" )]
    public void ThenExtraFieldParserVValueIs( string value )
    {
        ThenExtra( parser => Assert.AreEqual( value, Encoding.ASCII.GetString( parser.ExtraFieldParser.GetVValue( parser.OriginalSpan ) ) ) );
    }

    [Then( "the parser throw an error message '(.*)'" )]
    public void ThenTheParserThrowAnErrorMessage( string message )
    {
        try
        {
            Debug.Assert( _makeParser is not null );
            _makeParser();
        }
        catch( Exception e )
        {
            Assert.AreEqual( e.Message, message );
        }
    }

    void When( string messageLine, bool throwWhenTagBlockContainsUnknownFields, TagBlockStandard tagBlockStandard, bool allowEmptyTagBlockFields )
    {
        _makeParser = () => new NmeaTagBlockParser<DefaultExtraFieldParser>( Encoding.ASCII.GetBytes( messageLine ), throwWhenTagBlockContainsUnknownFields, tagBlockStandard, allowEmptyTagBlockFields );
    }

    void WhenExtra( string messageLine, bool throwWhenTagBlockContainsUnknownFields, TagBlockStandard tagBlockStandard, bool allowEmptyTagBlockFields )
    {
        _makeExtraParser = () => new NmeaTagBlockParser<ExtraFieldParser>( Encoding.ASCII.GetBytes( messageLine ), throwWhenTagBlockContainsUnknownFields, tagBlockStandard, allowEmptyTagBlockFields );
    }

    void Then( ParserTest test )
    {
        if( _makeParser is null ) throw new InvalidOperationException( $"Then step must be called." );
        NmeaTagBlockParser<DefaultExtraFieldParser> parser = _makeParser();
        test( parser );
    }

    void ThenExtra( ParserTestExtra test )
    {
        if( _makeExtraParser is null ) throw new InvalidOperationException( $"Then Extra step must be called." );
        NmeaTagBlockParser<ExtraFieldParser> parser = _makeExtraParser();
        test( parser );
    }

    struct ExtraFieldParser : INmeaTagBlockExtraFieldParser
    {
        public Range QValue { get; private set; }

        public Range VValue { get; private set; }

        public bool TryParseField( ReadOnlySpan<byte> source, ReadOnlySpan<byte> field, int fieldOffset )
        {
            Debug.Assert( source[new Range()].IsEmpty, "Proves that default range applied to any ReadOnlySpan gives an empty ReadOnlySpan." );
            if( field.Length < 2 || field[1] != ':' ) return false;
            switch( field[0] )
            {
                case (byte)'q':
                    QValue = new Range( fieldOffset + 2, fieldOffset + field.Length );
                    return true;
                case (byte)'v':
                    VValue = new Range( fieldOffset + 2, fieldOffset + field.Length );
                    return true;
                default:
                    return false;
            }
        }

        public readonly ReadOnlySpan<byte> GetQValue( ReadOnlySpan<byte> originalSpan ) => GetValue( originalSpan, QValue );

        public readonly ReadOnlySpan<byte> GetVValue( ReadOnlySpan<byte> originalSpan ) => GetValue( originalSpan, VValue );

        //  TODO: Faire un commentaire : Voilà un exemple de comment récupérer le span à partir de l'orignal span et de la range.
        static ReadOnlySpan<byte> GetValue( ReadOnlySpan<byte> originalSpan, Range qValue ) => originalSpan[qValue];
    }
}
