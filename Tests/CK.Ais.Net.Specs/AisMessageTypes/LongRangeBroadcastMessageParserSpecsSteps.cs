using NUnit.Framework;
using System;
using System.Text;
using TechTalk.SpecFlow;

namespace Ais.Net.Specs.AisMessageTypes;

[Binding]
public class LongRangeBroadcastMessageParserSpecsSteps
{
    ParserMaker? _makeParser;

    delegate NmeaAisLongRangeBroadcastMessageParser ParserMaker();

    delegate void ParserTest( NmeaAisLongRangeBroadcastMessageParser parser );

    [When( "I parse '(.*)' with padding (.*) as a Long-range Automatic Identifcation System Broadcast Message" )]
    public void WhenIParseWithNmeaAisLongRangeBroadcastMessageParser( string payload, uint padding )
    {
        When( () => new NmeaAisLongRangeBroadcastMessageParser( Encoding.ASCII.GetBytes( payload ), padding ) );
    }

    [Then( @"NmeaAisLongRangeBroadcastMessageParser\.Type is (.*)" )]
    public void ThenNmeaAisLongRangeBroadcastMessageParser_TypeIs( MessageType messageType )
    {
        Then( parser => Assert.AreEqual( messageType, parser.MessageType ) );
    }

    [Then( @"NmeaAisLongRangeBroadcastMessageParser\.RepeatIndicator is (.*)" )]
    public void ThenNmeaAisLongRangeBroadcastMessageParser_RepeatIndicatorIs( uint repeatCount )
    {
        Then( parser => Assert.AreEqual( repeatCount, parser.RepeatIndicator ) );
    }

    [Then( @"NmeaAisLongRangeBroadcastMessageParser\.Mmsi is (.*)" )]
    public void ThenNmeaAisLongRangeBroadcastMessageParser_MmsiIs( uint mmsi )
    {
        Then( parser => Assert.AreEqual( mmsi, parser.Mmsi ) );
    }

    [Then( @"NmeaAisLongRangeBroadcastMessageParser\.PositionAccuracy is (.*)" )]
    public void ThenNmeaAisLongRangeBroadcastMessageParser_PositionAccuracyIs( bool value )
    {
        Then( parser => Assert.AreEqual( value, parser.PositionAccuracy ) );
    }

    [Then( @"NmeaAisLongRangeBroadcastMessageParser\.RaimFlag is (.*)" )]
    public void ThenNmeaAisLongRangeBroadcastMessageParser_RaimFlagIs( bool value )
    {
        Then( parser => Assert.AreEqual( value, parser.RaimFlag ) );
    }

    [Then( @"NmeaAisLongRangeBroadcastMessageParser\.NavigationStatus is (.*)" )]
    public void ThenNmeaAisLongRangeBroadcastMessageParser_NavigationStatusIs( NavigationStatus value )
    {
        Then( parser => Assert.AreEqual( value, parser.NavigationStatus ) );
    }

    [Then( @"NmeaAisLongRangeBroadcastMessageParser\.Longitude10thMins is (.*)" )]
    public void ThenNmeaAisLongRangeBroadcastMessageParser_Longitude10thMinsIs( int value )
    {
        Then( parser => Assert.AreEqual( value, parser.Longitude10thMins ) );
    }

    [Then( @"NmeaAisLongRangeBroadcastMessageParser\.Latitude10thMins is (.*)" )]
    public void ThenNmeaAisLongRangeBroadcastMessageParser_Latitude10thMinsIs( int value )
    {
        Then( parser => Assert.AreEqual( value, parser.Latitude10thMins ) );
    }

    [Then( @"NmeaAisLongRangeBroadcastMessageParser\.SpeedOverGround is (.*)" )]
    public void ThenNmeaAisLongRangeBroadcastMessageParser_SpeedOverGroundIs( uint value )
    {
        Then( parser => Assert.AreEqual( value, parser.SpeedOverGround ) );
    }

    [Then( @"NmeaAisLongRangeBroadcastMessageParser\.CourseOverGround is (.*)" )]
    public void ThenNmeaAisLongRangeBroadcastMessageParser_CourseOverGroundIs( uint value )
    {
        Then( parser => Assert.AreEqual( value, parser.CourseOverGround ) );
    }

    [Then( @"NmeaAisLongRangeBroadcastMessageParser\.PositionLatency is (.*)" )]
    public void ThenNmeaAisLongRangeBroadcastMessageParser_PositionLatencyIs( bool value )
    {
        Then( parser => Assert.AreEqual( value, parser.PositionLatency ) );
    }

    [Then( @"NmeaAisLongRangeBroadcastMessageParser\.SpareBit95 is (.*)" )]
    public void ThenNmeaAisLongRangeBroadcastMessageParser_SpareBit95Is( bool value )
    {
        Then( parser => Assert.AreEqual( value, parser.SpareBit95 ) );
    }

    void When( ParserMaker makeParser )
    {
        _makeParser = makeParser;
    }

    void Then( ParserTest test )
    {
        if( _makeParser is null ) throw new InvalidOperationException( $"When step must be called not called." );
        NmeaAisLongRangeBroadcastMessageParser parser = _makeParser();
        test( parser );
    }
}
