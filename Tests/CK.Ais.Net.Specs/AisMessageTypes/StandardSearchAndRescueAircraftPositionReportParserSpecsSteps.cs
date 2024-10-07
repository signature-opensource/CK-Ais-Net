using NUnit.Framework;
using System;
using System.Text;
using TechTalk.SpecFlow;

namespace Ais.Net.Specs.AisMessageTypes;

[Binding]
public class StandardSearchAndRescueAircraftPositionReportParserSpecsSteps
{
    ParserMaker? _makeParser;

    delegate NmeaAisStandardSearchAndRescueAircraftPositionReportParser ParserMaker();

    delegate void ParserTest( NmeaAisStandardSearchAndRescueAircraftPositionReportParser parser );

    [When( "I parse '(.*)' with padding (.*) as a Standard Search and Rescue Aircraft Position Report" )]
    public void WhenIParseWithNmeaAisStandardSearchAndRescueAircraftPositionReportParser( string payload, uint padding )
    {
        When( () => new NmeaAisStandardSearchAndRescueAircraftPositionReportParser( Encoding.ASCII.GetBytes( payload ), padding ) );
    }

    [Then( @"NmeaAisStandardSearchAndRescueAircraftPositionReportParser\.Type is (.*)" )]
    public void ThenNmeaAisStandardSearchAndRescueAircraftPositionReportParser_TypeIs( MessageType messageType )
    {
        Then( parser => Assert.AreEqual( messageType, parser.MessageType ) );
    }

    [Then( @"NmeaAisStandardSearchAndRescueAircraftPositionReportParser\.RepeatIndicator is (.*)" )]
    public void ThenNmeaAisStandardSearchAndRescueAircraftPositionReportParser_RepeatIndicatorIs( uint repeatCount )
    {
        Then( parser => Assert.AreEqual( repeatCount, parser.RepeatIndicator ) );
    }

    [Then( @"NmeaAisStandardSearchAndRescueAircraftPositionReportParser\.Mmsi is (.*)" )]
    public void ThenNmeaAisStandardSearchAndRescueAircraftPositionReportParser_MmsiIs( uint mmsi )
    {
        Then( parser => Assert.AreEqual( mmsi, parser.Mmsi ) );
    }

    [Then( @"NmeaAisStandardSearchAndRescueAircraftPositionReportParser\.Altitude is (.*)" )]
    public void ThenNmeaAisStandardSearchAndRescueAircraftPositionReportParser_AltitudeIs( uint value )
    {
        Then( parser => Assert.AreEqual( value, parser.Altitude ) );
    }

    [Then( @"NmeaAisStandardSearchAndRescueAircraftPositionReportParser\.SpeedOverGround is (.*)" )]
    public void ThenNmeaAisStandardSearchAndRescueAircraftPositionReportParser_SpeedOverGroundIs( uint value )
    {
        Then( parser => Assert.AreEqual( value, parser.SpeedOverGround ) );
    }

    [Then( @"NmeaAisStandardSearchAndRescueAircraftPositionReportParser\.PositionAccuracy is (.*)" )]
    public void ThenNmeaAisStandardSearchAndRescueAircraftPositionReportParser_PositionAccuracyIs( bool value )
    {
        Then( parser => Assert.AreEqual( value, parser.PositionAccuracy ) );
    }

    [Then( @"NmeaAisStandardSearchAndRescueAircraftPositionReportParser\.Longitude10000thMins is (.*)" )]
    public void ThenNmeaAisStandardSearchAndRescueAircraftPositionReportParser_Is( int value )
    {
        Then( parser => Assert.AreEqual( value, parser.Longitude10000thMins ) );
    }

    [Then( @"NmeaAisStandardSearchAndRescueAircraftPositionReportParser\.Latitude10000thMins is (.*)" )]
    public void ThenNmeaAisStandardSearchAndRescueAircraftPositionReportParser_Latitude10000thMinsIs( int value )
    {
        Then( parser => Assert.AreEqual( value, parser.Latitude10000thMins ) );
    }

    [Then( @"NmeaAisStandardSearchAndRescueAircraftPositionReportParser\.CourseOverGround10thDegrees is (.*)" )]
    public void ThenNmeaAisStandardSearchAndRescueAircraftPositionReportParser_CourseOverGround10thDegreesIs( uint value )
    {
        Then( parser => Assert.AreEqual( value, parser.CourseOverGround10thDegrees ) );
    }

    [Then( @"NmeaAisStandardSearchAndRescueAircraftPositionReportParser\.TimeStampSecond is (.*)" )]
    public void ThenNmeaAisStandardSearchAndRescueAircraftPositionReportParser_TimeStampSecondIs( uint value )
    {
        Then( parser => Assert.AreEqual( value, parser.TimeStampSecond ) );
    }

    [Then( @"NmeaAisStandardSearchAndRescueAircraftPositionReportParser\.AltitudeSensor is (.*)" )]
    public void ThenNmeaAisStandardSearchAndRescueAircraftPositionReportParser_AltitudeSensorIs( AltitudeSensor value )
    {
        Then( parser => Assert.AreEqual( value, parser.AltitudeSensor ) );
    }

    [Then( @"NmeaAisStandardSearchAndRescueAircraftPositionReportParser\.SpareBits135 is (.*)" )]
    public void ThenNmeaAisStandardSearchAndRescueAircraftPositionReportParser_SpareBits135Is( uint value )
    {
        Then( parser => Assert.AreEqual( value, parser.SpareBits135 ) );
    }

    [Then( @"NmeaAisStandardSearchAndRescueAircraftPositionReportParser\.DTE is (.*)" )]
    public void ThenNmeaAisStandardSearchAndRescueAircraftPositionReportParser_DTEIs( bool value )
    {
        Then( parser => Assert.AreEqual( value, parser.DTE ) );
    }

    [Then( @"NmeaAisStandardSearchAndRescueAircraftPositionReportParser\.SpareBits143 is (.*)" )]
    public void ThenNmeaAisStandardSearchAndRescueAircraftPositionReportParser_SpareBits143Is( uint value )
    {
        Then( parser => Assert.AreEqual( value, parser.SpareBits143 ) );
    }

    [Then( @"NmeaAisStandardSearchAndRescueAircraftPositionReportParser\.AssignedMode is (.*)" )]
    public void ThenNmeaAisStandardSearchAndRescueAircraftPositionReportParser_AssignedModeIs( bool value )
    {
        Then( parser => Assert.AreEqual( value, parser.AssignedMode ) );
    }

    [Then( @"NmeaAisStandardSearchAndRescueAircraftPositionReportParser\.RaimFlag is (.*)" )]
    public void ThenNmeaAisStandardSearchAndRescueAircraftPositionReportParser_RaimFlagIs( bool value )
    {
        Then( parser => Assert.AreEqual( value, parser.RaimFlag ) );
    }

    [Then( @"NmeaAisStandardSearchAndRescueAircraftPositionReportParser\.CommunicationStateSelector is (.*)" )]
    public void ThenNmeaAisStandardSearchAndRescueAircraftPositionReportParser_CommunicationStateSelectorIs( CommunicationStateSelector value )
    {
        Then( parser => Assert.AreEqual( value, parser.CommunicationStateSelector ) );
    }

    [Then( @"NmeaAisStandardSearchAndRescueAircraftPositionReportParser\.CommunicationState is (.*)" )]
    public void ThenNmeaAisStandardSearchAndRescueAircraftPositionReportParser_CommunicationStateIs( uint value )
    {
        Then( parser => Assert.AreEqual( value, parser.CommunicationState ) );
    }

    void When( ParserMaker makeParser )
    {
        _makeParser = makeParser;
    }

    void Then( ParserTest test )
    {
        if( _makeParser is null ) throw new InvalidOperationException( $"When step must be called." );
        NmeaAisStandardSearchAndRescueAircraftPositionReportParser parser = _makeParser();
        test( parser );
    }
}
