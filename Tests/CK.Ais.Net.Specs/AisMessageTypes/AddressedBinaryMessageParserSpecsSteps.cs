using NUnit.Framework;
using System;
using System.Text;
using TechTalk.SpecFlow;

namespace Ais.Net.Specs.AisMessageTypes;

[Binding]
public class AddressedBinaryMessageParserSpecsSteps
{
    ParserMaker? _makeParser;

    delegate NmeaAisAddressedBinaryMessageParser ParserMaker();

    delegate void ParserTest( NmeaAisAddressedBinaryMessageParser parser );

    [When( "I parse '(.*)' with padding (.*) as a Addressed Binary Message" )]
    public void WhenIParseWithNmeaAisAddressedBinaryMessageParser( string payload, uint padding )
    {
        When( () => new NmeaAisAddressedBinaryMessageParser( Encoding.ASCII.GetBytes( payload ), padding ) );
    }

    [Then( @"NmeaAisAddressedBinaryMessageParser\.Type is (.*)" )]
    public void ThenNmeaAisAddressedBinaryMessageParser_TypeIs( MessageType messageType )
    {
        Then( parser => Assert.AreEqual( messageType, parser.MessageType ) );
    }

    [Then( @"NmeaAisAddressedBinaryMessageParser\.RepeatIndicator is (.*)" )]
    public void ThenNmeaAisAddressedBinaryMessageParser_RepeatIndicatorIs( uint repeatCount )
    {
        Then( parser => Assert.AreEqual( repeatCount, parser.RepeatIndicator ) );
    }

    [Then( @"NmeaAisAddressedBinaryMessageParser\.Mmsi is (.*)" )]
    public void ThenNmeaAisAddressedBinaryMessageParser_MmsiIs( uint mmsi )
    {
        Then( parser => Assert.AreEqual( mmsi, parser.Mmsi ) );
    }

    [Then( @"NmeaAisAddressedBinaryMessageParser\.SequenceNumber is (.*)" )]
    public void ThenNmeaAisAddressedBinaryMessageParser_SequenceNumberIs( uint value )
    {
        Then( parser => Assert.AreEqual( value, parser.SequenceNumber ) );
    }

    [Then( @"NmeaAisAddressedBinaryMessageParser\.DestinationMmsi is (.*)" )]
    public void ThenNmeaAisAddressedBinaryMessageParser_DestinationMmsiIs( uint value )
    {
        Then( parser => Assert.AreEqual( value, parser.DestinationMmsi ) );
    }

    [Then( @"NmeaAisAddressedBinaryMessageParser\.Retransmit is (.*)" )]
    public void ThenNmeaAisAddressedBinaryMessageParser_Is( bool value )
    {
        Then( parser => Assert.AreEqual( value, parser.Retransmit ) );
    }

    [Then( @"NmeaAisAddressedBinaryMessageParser\.SpareBit71 is (.*)" )]
    public void ThenNmeaAisAddressedBinaryMessageParser_SpareBit71Is( bool value )
    {
        Then( parser => Assert.AreEqual( value, parser.SpareBit71 ) );
    }

    [Then( @"NmeaAisAddressedBinaryMessageParser\.DAC is (.*)" )]
    public void ThenNmeaAisAddressedBinaryMessageParser_DACIs( uint value )
    {
        Then( parser => Assert.AreEqual( value, parser.DAC ) );
    }

    [Then( @"NmeaAisAddressedBinaryMessageParser\.FI is (.*)" )]
    public void ThenNmeaAisAddressedBinaryMessageParser_FIIs( uint value )
    {
        Then( parser => Assert.AreEqual( value, parser.FI ) );
    }

    [Then( @"NmeaAisAddressedBinaryMessageParser\.ApplicationDataPaddingBefore is (.*)" )]
    public void ThenNmeaAisAddressedBinaryMessageParser_ApplicationDataPaddingBeforeIs( uint value )
    {
        Then( parser => Assert.AreEqual( value, parser.ApplicationDataPaddingBefore ) );
    }

    [Then( @"NmeaAisAddressedBinaryMessageParser\.ApplicationData is (.*)" )]
    public void ThenNmeaAisAddressedBinaryMessageParser_ApplicationDataIs( string value )
    {
        Then( parser => Assert.AreEqual( value, Encoding.ASCII.GetString( parser.ApplicationData ) ) );
    }

    void When( ParserMaker makeParser )
    {
        _makeParser = makeParser;
    }

    void Then( ParserTest test )
    {
        if( _makeParser is null ) throw new InvalidOperationException( $"When step must be called." );
        NmeaAisAddressedBinaryMessageParser parser = _makeParser();
        test( parser );
    }
}
