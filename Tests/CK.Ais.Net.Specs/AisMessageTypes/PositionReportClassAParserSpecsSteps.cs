// <copyright file="PositionReportClassAParserSpecsSteps.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using NUnit.Framework;
using System;
using System.Text;
using TechTalk.SpecFlow;

namespace Ais.Net.Specs.AisMessageTypes;

[Binding]
public class PositionReportClassAParserSpecsSteps
{
    ParserMaker? _makeParser;

    delegate NmeaAisPositionReportClassAParser ParserMaker();

    delegate void ParserTest( NmeaAisPositionReportClassAParser parser );

    [When( "I parse '(.*)' with padding (.*) as a Position Report Class A" )]
    public void WhenIParseWithPaddingAsAPositionReportClassA( string payload, uint padding )
    {
        When( () => new NmeaAisPositionReportClassAParser( Encoding.ASCII.GetBytes( payload ), padding ) );
    }

    [Then( @"AisPositionReportClassAParser\.Type is (.*)" )]
    public void ThenAisPositionReportClassAParser_TypeIs( MessageType messageType )
    {
        Then( parser => Assert.AreEqual( messageType, parser.MessageType ) );
    }

    [Then( @"AisPositionReportClassAParser\.RepeatIndicator is (.*)" )]
    public void ThenAisPositionReportClassAParser_RepeatIndicatorIs( int repeatCount )
    {
        Then( parser => Assert.AreEqual( repeatCount, parser.RepeatIndicator ) );
    }

    [Then( @"AisPositionReportClassAParser\.Mmsi is '(.*)'" )]
    public void ThenAisPositionReportClassAParser_MmsiIs( int mmsi )
    {
        Then( parser => Assert.AreEqual( mmsi, parser.Mmsi ) );
    }

    [Then( @"AisPositionReportClassAParser\.NavigationStatus is (.*)" )]
    public void ThenAisPositionReportClassAParser_NavigationStatusIs( NavigationStatus navigationStatus )
    {
        Then( parser => Assert.AreEqual( navigationStatus, parser.NavigationStatus ) );
    }

    [Then( @"AisPositionReportClassAParser\.RateOfTurn is (.*)" )]
    public void ThenAisPositionReportClassAParser_RateOfTurnIs( int rateOfTurn )
    {
        Then( parser => Assert.AreEqual( rateOfTurn, parser.RateOfTurn ) );
    }

    [Then( @"AisPositionReportClassAParser\.SpeedOverGroundTenths is (.*)" )]
    public void ThenAisPositionReportClassAParser_SpeedOverGroundTenthsIs( int speedOverGround )
    {
        Then( parser => Assert.AreEqual( speedOverGround, parser.SpeedOverGroundTenths ) );
    }

    [Then( @"AisPositionReportClassAParser\.PositionAccuracy is (.*)" )]
    public void ThenAisPositionReportClassAParser_PositionAccuracyIsTrue( bool positionAccuracy )
    {
        Then( parser => Assert.AreEqual( positionAccuracy, parser.PositionAccuracy ) );
    }

    [Then( @"AisPositionReportClassAParser\.Longitude10000thMins is (.*)" )]
    public void ThenAisPositionReportClassAParser_LongitudeIs( int longitude )
    {
        Then( parser => Assert.AreEqual( longitude, parser.Longitude10000thMins ) );
    }

    [Then( @"AisPositionReportClassAParser\.Latitude10000thMins is (.*)" )]
    public void ThenAisPositionReportClassAParser_LatitudeIs( int latitude )
    {
        Then( parser => Assert.AreEqual( latitude, parser.Latitude10000thMins ) );
    }

    [Then( @"AisPositionReportClassAParser\.CourseOverGround10thDegrees is (.*)" )]
    public void ThenAisPositionReportClassAParser_CourseOverGroundthDegreesIs( int courseOverGround )
    {
        Then( parser => Assert.AreEqual( courseOverGround, parser.CourseOverGround10thDegrees ) );
    }

    [Then( @"AisPositionReportClassAParser\.TrueHeadingDegrees is (.*)" )]
    public void ThenAisPositionReportClassAParser_TrueHeadingDegreesIs( int trueHeading )
    {
        Then( parser => Assert.AreEqual( trueHeading, parser.TrueHeadingDegrees ) );
    }

    [Then( @"AisPositionReportClassAParser\.TimeStampSecond is (.*)" )]
    public void ThenAisPositionReportClassAParser_TimeStampSecondIs( int timeStamp )
    {
        Then( parser => Assert.AreEqual( timeStamp, parser.TimeStampSecond ) );
    }

    [Then( @"AisPositionReportClassAParser\.ManoeuvreIndicator is (.*)" )]
    public void ThenAisPositionReportClassAParser_ManoeuvreIndicatorIsNotAvailable( ManoeuvreIndicator manoeuvre )
    {
        Then( parser => Assert.AreEqual( manoeuvre, parser.ManoeuvreIndicator ) );
    }

    [Then( @"AisPositionReportClassAParser\.SpareBits145 is (.*)" )]
    public void ThenAisPositionReportClassAParser_SpareBitsIs( int value )
    {
        Then( parser => Assert.AreEqual( value, parser.SpareBits145 ) );
    }

    [Then( @"AisPositionReportClassAParser\.RaimFlag is (.*)" )]
    public void ThenAisPositionReportClassAParser_RaimFlagIs( bool value )
    {
        Then( parser => Assert.AreEqual( value, parser.RaimFlag ) );
    }

    [Then( @"AisPositionReportClassAParser\.RadioSyncState is (.*)" )]
    public void ThenAisPositionReportClassAParser_RadioSyncStateIsUtcDirect( RadioSyncState state )
    {
        Then( parser => Assert.AreEqual( state, parser.RadioSyncState ) );
    }

    [Then( @"AisPositionReportClassAParser\.RadioSlotTimeout is (.*)" )]
    public void ThenAisPositionReportClassAParser_RadioSlotTimeoutIs( uint timeout )
    {
        Then( parser => Assert.AreEqual( timeout, parser.RadioSlotTimeout ) );
    }

    [Then( @"AisPositionReportClassAParser\.RadioSubMessage is (.*)" )]
    public void ThenAisPositionReportClassAParser_RadioSubMessageIs( uint message )
    {
        Then( parser => Assert.AreEqual( message, parser.RadioSubMessage ) );
    }

    void When( ParserMaker makeParser )
    {
        _makeParser = makeParser;
    }

    void Then( ParserTest test )
    {
        if( _makeParser is null ) throw new InvalidOperationException( $"When step must be called not called." );
        NmeaAisPositionReportClassAParser parser = _makeParser();
        test( parser );
    }
}
