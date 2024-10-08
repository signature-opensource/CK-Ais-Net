using NUnit.Framework;
using System;
using System.Text;
using TechTalk.SpecFlow;

namespace Ais.Net.Specs.AisMessageTypes;

[Binding]
public class GlobalNavigationSatelliteSystemBroadcastBinaryMessageParserSpecsSteps
{
    ParserMaker? _makeParser;

    delegate NmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser ParserMaker();

    delegate void ParserTest( NmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser parser );

    [When( "I parse '(.*)' with padding (.*) as a Global Navigation-Satellite System Broadcast Binary Message" )]
    public void WhenIParseWithNmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser( string payload, uint padding )
    {
        When( () => new NmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser( Encoding.ASCII.GetBytes( payload ), padding ) );
    }

    [Then( @"NmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser\.Type is (.*)" )]
    public void ThenNmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser_TypeIs( MessageType messageType )
    {
        Then( parser => Assert.AreEqual( messageType, parser.MessageType ) );
    }

    [Then( @"NmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser\.RepeatIndicator is (.*)" )]
    public void ThenNmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser_RepeatIndicatorIs( uint repeatCount )
    {
        Then( parser => Assert.AreEqual( repeatCount, parser.RepeatIndicator ) );
    }

    [Then( @"NmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser\.Mmsi is (.*)" )]
    public void ThenNmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser_MmsiIs( uint mmsi )
    {
        Then( parser => Assert.AreEqual( mmsi, parser.Mmsi ) );
    }

    [Then( @"NmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser\.SpareBits38 is (.*)" )]
    public void ThenNmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser_SpareBits38Is( uint value )
    {
        Then( parser => Assert.AreEqual( value, parser.SpareBits38 ) );
    }

    [Then( @"NmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser\.Longitude10thMins is (.*)" )]
    public void ThenNmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser_Longitude10thMinsIs( int value )
    {
        Then( parser => Assert.AreEqual( value, parser.Longitude10thMins ) );
    }

    [Then( @"NmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser\.Latitude10thMins is (.*)" )]
    public void ThenNmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser_Latitude10thMinsIs( int value )
    {
        Then( parser => Assert.AreEqual( value, parser.Latitude10thMins ) );
    }

    [Then( @"NmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser\.SpareBits75 is (.*)" )]
    public void ThenNmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser_SpareBits75Is( uint value )
    {
        Then( parser => Assert.AreEqual( value, parser.SpareBits75 ) );
    }

    [Then( @"NmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser\.DifferentialCorrectionDataPadding is (.*)" )]
    public void ThenNmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser_DifferentialCorrectionDataPaddingIs( uint value )
    {
        Then( parser => Assert.AreEqual( value, parser.DifferentialCorrectionDataPaddingBefore ) );
    }

    [Then( @"NmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser\.DifferentialCorrectionData is (.*)" )]
    public void ThenNmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser_DifferentialCorrectionDataIs( string value )
    {
        Then( parser => Assert.AreEqual( value, Encoding.ASCII.GetString( parser.DifferentialCorrectionData ) ) );
    }

    void When( ParserMaker makeParser )
    {
        _makeParser = makeParser;
    }

    void Then( ParserTest test )
    {
        if( _makeParser is null ) throw new InvalidOperationException( $"When step must be called not called." );
        NmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser parser = _makeParser();
        test( parser );
    }
}
