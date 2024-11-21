using NUnit.Framework;
using System;
using System.Text;
using TechTalk.SpecFlow;

namespace Ais.Net.Specs.AisMessageTypes;

[Binding]
public class SingleSlotBinaryParserSpecsSteps
{
    ParserMaker? _makeParser;

    delegate NmeaAisSingleSlotBinaryParser ParserMaker();

    delegate void ParserTest( NmeaAisSingleSlotBinaryParser parser );

    [When( "I parse '(.*)' with padding (.*) as a Single Slot Binary Message" )]
    public void WhenIParseWithNmeaAisSingleSlotBinaryParser( string payload, uint padding )
    {
        When( () => new NmeaAisSingleSlotBinaryParser( Encoding.ASCII.GetBytes( payload ), padding ) );
    }

    [Then( @"NmeaAisSingleSlotBinaryParser\.Type is (.*)" )]
    public void ThenNmeaAisSingleSlotBinaryParser_TypeIs( MessageType messageType )
    {
        Then( parser =>
        {
            Assert.AreEqual( messageType, parser.MessageType );
        } );
    }

    [Then( @"NmeaAisSingleSlotBinaryParser\.RepeatIndicator is (.*)" )]
    public void ThenNmeaAisSingleSlotBinaryParser_RepeatIndicatorIs( uint repeatCount )
    {
        Then( parser => Assert.AreEqual( repeatCount, parser.RepeatIndicator ) );
    }

    [Then( @"NmeaAisSingleSlotBinaryParser\.Mmsi is (.*)" )]
    public void ThenNmeaAisSingleSlotBinaryParser_MmsiIs( uint mmsi )
    {
        Then( parser => Assert.AreEqual( mmsi, parser.Mmsi ) );
    }

    [Then( @"NmeaAisSingleSlotBinaryParser\.DestinationIndicator is (.*)" )]
    public void ThenNmeaAisSingleSlotBinaryParser_DestinationIndicatorIs( DestinationIndicator value )
    {
        Then( parser => Assert.AreEqual( value, parser.DestinationIndicator ) );
    }

    [Then( @"NmeaAisSingleSlotBinaryParser\.BinaryDataFlag is (.*)" )]
    public void ThenNmeaAisSingleSlotBinaryParser_BinaryDataFlagIs( bool value )
    {
        Then( parser => Assert.AreEqual( value, parser.BinaryDataFlag ) );
    }

    [Then( @"NmeaAisSingleSlotBinaryParser\.DestinationMmsi is (.*)" )]
    public void ThenNmeaAisSingleSlotBinaryParser_DestinationMmsiIs( uint? value )
    {
        Then( parser => Assert.AreEqual( value, parser.DestinationMmsi ) );
    }

    [Then( @"NmeaAisSingleSlotBinaryParser\.SpareBits70 is (.*)" )]
    public void ThenNmeaAisSingleSlotBinaryParser_SpareBits70Is( uint? value )
    {
        Then( parser => Assert.AreEqual( value, parser.SpareBits70 ) );
    }

    [Then( @"NmeaAisSingleSlotBinaryParser\.DAC is (.*)" )]
    public void ThenNmeaAisSingleSlotBinaryParser_DACIs( uint? value )
    {
        Then( parser => Assert.AreEqual( value, parser.DAC ) );
    }

    [Then( @"NmeaAisSingleSlotBinaryParser\.FI is (.*)" )]
    public void ThenNmeaAisSingleSlotBinaryParser_FIIs( uint? value )
    {
        Then( parser => Assert.AreEqual( value, parser.FI ) );
    }

    [Then( @"NmeaAisSingleSlotBinaryParser\.ApplicationDataPadding is (.*)" )]
    public void ThenNmeaAisSingleSlotBinaryParser_ApplicationDataPaddingIs( uint value )
    {
        Then( parser => Assert.AreEqual( value, parser.ApplicationDataPaddingBefore ) );
    }

    [Then( @"NmeaAisSingleSlotBinaryParser\.ApplicationData is (.*)" )]
    public void ThenNmeaAisSingleSlotBinaryParser_ApplicationDataIs( string value )
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
        NmeaAisSingleSlotBinaryParser parser = _makeParser();
        test( parser );
    }
}
