using NUnit.Framework;
using System;
using System.Text;
using TechTalk.SpecFlow;

namespace Ais.Net.Specs.AisMessageTypes;

[Binding]
public class AcknowledgementParserSpecsSteps
{
    ParserMaker? _makeParser;

    delegate NmeaAisAcknowledgementParser ParserMaker();

    delegate void ParserTest( NmeaAisAcknowledgementParser parser );

    [When( "I parse '(.*)' with padding (.*) as a Acknowledgement Message" )]
    public void WhenIParseWithNmeaAisAcknowledgementParser( string payload, uint padding )
    {
        When( () => new NmeaAisAcknowledgementParser( Encoding.ASCII.GetBytes( payload ), padding ) );
    }

    [Then( @"NmeaAisAcknowledgementParser\.Type is (.*)" )]
    public void ThenNmeaAisAcknowledgementParser_TypeIs( MessageType messageType )
    {
        Then( parser => Assert.AreEqual( messageType, parser.MessageType ) );
    }

    [Then( @"NmeaAisAcknowledgementParser\.RepeatIndicator is (.*)" )]
    public void ThenNmeaAisAcknowledgementParser_RepeatIndicatorIs( uint repeatCount )
    {
        Then( parser => Assert.AreEqual( repeatCount, parser.RepeatIndicator ) );
    }

    [Then( @"NmeaAisAcknowledgementParser\.Mmsi is (.*)" )]
    public void ThenNmeaAisAcknowledgementParser_MmsiIs( uint mmsi )
    {
        Then( parser => Assert.AreEqual( mmsi, parser.Mmsi ) );
    }

    [Then( @"NmeaAisAcknowledgementParser\.SpareBits38 is (.*)" )]
    public void ThenNmeaAisAcknowledgementParser_Is( uint value )
    {
        Then( parser => Assert.AreEqual( value, parser.SpareBits38 ) );
    }

    [Then( @"NmeaAisAcknowledgementParser\.DestinationMmsi1 is (.*)" )]
    public void ThenNmeaAisAcknowledgementParser_DestinationMmsi1Is( uint value )
    {
        Then( parser => Assert.AreEqual( value, parser.DestinationMmsi1 ) );
    }

    [Then( @"NmeaAisAcknowledgementParser\.SequenceNumberMmsi1 is (.*)" )]
    public void ThenNmeaAisAcknowledgementParser_SequenceNumberMmsi1Is( uint value )
    {
        Then( parser => Assert.AreEqual( value, parser.SequenceNumberMmsi1 ) );
    }

    [Then( @"NmeaAisAcknowledgementParser\.DestinationMmsi2 is (.*)" )]
    public void ThenNmeaAisAcknowledgementParser_DestinationMms21Is( uint? value )
    {
        Then( parser => Assert.AreEqual( value, parser.DestinationMmsi2 ) );
    }

    [Then( @"NmeaAisAcknowledgementParser\.SequenceNumberMmsi2 is (.*)" )]
    public void ThenNmeaAisAcknowledgementParser_SequenceNumberMmsi2Is( uint? value )
    {
        Then( parser => Assert.AreEqual( value, parser.SequenceNumberMmsi2 ) );
    }

    [Then( @"NmeaAisAcknowledgementParser\.DestinationMmsi3 is (.*)" )]
    public void ThenNmeaAisAcknowledgementParser_DestinationMmsi3Is( uint? value )
    {
        Then( parser => Assert.AreEqual( value, parser.DestinationMmsi3 ) );
    }

    [Then( @"NmeaAisAcknowledgementParser\.SequenceNumberMmsi3 is (.*)" )]
    public void ThenNmeaAisAcknowledgementParser_SequenceNumberMmsi3Is( uint? value )
    {
        Then( parser => Assert.AreEqual( value, parser.SequenceNumberMmsi3 ) );
    }

    [Then( @"NmeaAisAcknowledgementParser\.DestinationMmsi4 is (.*)" )]
    public void ThenNmeaAisAcknowledgementParser_DestinationMmsi4Is( uint? value )
    {
        Then( parser => Assert.AreEqual( value, parser.DestinationMmsi4 ) );
    }

    [Then( @"NmeaAisAcknowledgementParser\.SequenceNumberMmsi4 is (.*)" )]
    public void ThenNmeaAisAcknowledgementParser_SequenceNumberMmsi4Is( uint? value )
    {
        Then( parser => Assert.AreEqual( value, parser.SequenceNumberMmsi4 ) );
    }

    void When( ParserMaker makeParser )
    {
        _makeParser = makeParser;
    }

    void Then( ParserTest test )
    {
        if( _makeParser is null ) throw new InvalidOperationException( $"When step must be called." );
        NmeaAisAcknowledgementParser parser = _makeParser();
        test( parser );
    }
}
