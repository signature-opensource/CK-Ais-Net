using NUnit.Framework;
using System;
using System.Text;
using TechTalk.SpecFlow;

namespace Ais.Net.Specs.AisMessageTypes;

[Binding]
public class BaseStationReportParserSpecsSteps
{
    ParserMaker? _makeParser;

    delegate NmeaAisBaseStationReportParser ParserMaker();

    delegate void ParserTest( NmeaAisBaseStationReportParser parser );

    [When( "I parse '(.*)' with padding (.*) as a Base Station Report" )]
    public void WhenIParseWithPaddingAsALongRangeAisBroadcast( string payload, uint padding )
    {
        When( () => new NmeaAisBaseStationReportParser( Encoding.ASCII.GetBytes( payload ), padding ) );
    }

    [Then( @"NmeaAisBaseStationReportParser\.Type is (.*)" )]
    public void ThenNmeaAisBaseStationReportParser_TypeIs( MessageType messageType )
    {
        Then( parser => Assert.AreEqual( messageType, parser.MessageType ) );
    }

    [Then( @"NmeaAisBaseStationReportParser\.RepeatIndicator is (.*)" )]
    public void ThenNmeaAisBaseStationReportParser_RepeatIndicatorIs( int repeatCount )
    {
        Then( parser => Assert.AreEqual( repeatCount, parser.RepeatIndicator ) );
    }

    [Then( @"NmeaAisBaseStationReportParser\.Mmsi is (.*)" )]
    public void ThenNmeaAisBaseStationReportParser_MmsiIs( int mmsi )
    {
        Then( parser => Assert.AreEqual( mmsi, parser.Mmsi ) );
    }

    [Then( @"NmeaAisBaseStationReportParser\.UtcYear is (.*)" )]
    public void ThenNmeaAisBaseStationReportParser_UtcYearIs( int utcYear )
    {
        Then( parser => Assert.AreEqual( utcYear, parser.UtcYear ) );
    }

    [Then( @"NmeaAisBaseStationReportParser\.UtcMonth is (.*)" )]
    public void ThenNmeaAisBaseStationReportParser_UtcMonthIs( int utcMonth )
    {
        Then( parser => Assert.AreEqual( utcMonth, parser.UtcMonth ) );
    }

    [Then( @"NmeaAisBaseStationReportParser\.UtcDay is (.*)" )]
    public void ThenNmeaAisBaseStationReportParser_UtcDayIs( int utcDay )
    {
        Then( parser => Assert.AreEqual( utcDay, parser.UtcDay ) );
    }

    [Then( @"NmeaAisBaseStationReportParser\.UtcHour is (.*)" )]
    public void ThenNmeaAisBaseStationReportParser_UtcHourIs( int utcHour )
    {
        Then( parser => Assert.AreEqual( utcHour, parser.UtcHour ) );
    }

    [Then( @"NmeaAisBaseStationReportParser\.UtcMinute is (.*)" )]
    public void ThenNmeaAisBaseStationReportParser_UtcMinuteIs( int utcMinute )
    {
        Then( parser => Assert.AreEqual( utcMinute, parser.UtcMinute ) );
    }

    [Then( @"NmeaAisBaseStationReportParser\.UtcSecond is (.*)" )]
    public void ThenNmeaAisBaseStationReportParser_UtcSecondIs( int utcSecond )
    {
        Then( parser => Assert.AreEqual( utcSecond, parser.UtcSecond ) );
    }

    [Then( @"NmeaAisBaseStationReportParser\.PositionAccuracy is (.*)" )]
    public void ThenNmeaAisBaseStationReportParser_PositionAccuracyIs( bool positionAccuracy )
    {
        Then( parser => Assert.AreEqual( positionAccuracy, parser.PositionAccuracy ) );
    }

    [Then( @"NmeaAisBaseStationReportParser\.Longitude10000thMins is (.*)" )]
    public void ThenNmeaAisBaseStationReportParser_Longitude10000thMinsIs( double longitude )
    {
        Then( parser => Assert.AreEqual( longitude, parser.Longitude10000thMins / 600_000d ) );
    }

    [Then( @"NmeaAisBaseStationReportParser\.Latitude10000thMins is (.*)" )]
    public void ThenNmeaAisBaseStationReportParser_Latitude10000thMinsIs( double latitude )
    {
        Then( parser => Assert.AreEqual( latitude, parser.Latitude10000thMins / 600_000d ) );
    }

    [Then( @"NmeaAisBaseStationReportParser\.PositionFixType is (.*)" )]
    public void ThenNmeaAisBaseStationReportParser_PositionFixTypeIs( EpfdFixType positionFixType )
    {
        Then( parser => Assert.AreEqual( positionFixType, parser.PositionFixType ) );
    }

    [Then( @"NmeaAisBaseStationReportParser\.TransmissionControlForLongRangeBroadcastMessage is (.*)" )]
    public void ThenNmeaAisBaseStationReportParser_TransmissionControlForLongRangeBroadcastMessageIs( bool transmissionControlForLongRangeBroadcastMessage )
    {
        Then( parser => Assert.AreEqual( transmissionControlForLongRangeBroadcastMessage, parser.TransmissionControlForLongRangeBroadcastMessage ) );
    }

    [Then( @"NmeaAisBaseStationReportParser\.SpareBits139 is (.*)" )]
    public void ThenNmeaAisBaseStationReportParser_SpareBits139Is( uint spareBits139 )
    {
        Then( parser => Assert.AreEqual( spareBits139, parser.SpareBits139 ) );
    }

    [Then( @"NmeaAisBaseStationReportParser\.RaimFlag is (.*)" )]
    public void ThenNmeaAisBaseStationReportParser_PositionFixTypeIs( bool raimFlag )
    {
        Then( parser => Assert.AreEqual( raimFlag, parser.RaimFlag ) );
    }

    [Then( @"NmeaAisBaseStationReportParser\.CommunicationState is (.*)" )]
    public void ThenNmeaAisBaseStationReportParser_PositionFixTypeIs( uint communicationState )
    {
        Then( parser => Assert.AreEqual( communicationState, parser.CommunicationState ) );
    }

    void When( ParserMaker makeParser )
    {
        _makeParser = makeParser;
    }

    void Then( ParserTest test )
    {
        if( _makeParser is null ) throw new InvalidOperationException( $"When step must be called." );
        NmeaAisBaseStationReportParser parser = _makeParser();
        test( parser );
    }
}
