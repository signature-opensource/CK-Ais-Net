// <copyright file="PositionReportClassBParserSpecsSteps.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using NUnit.Framework;
using System;
using System.Text;
using TechTalk.SpecFlow;

namespace Ais.Net.Specs.AisMessageTypes
{
    [Binding]
    public class PositionReportClassBParserSpecsSteps
    {
        ParserMaker? _makeParser;

        delegate NmeaAisPositionReportClassBParser ParserMaker();

        delegate void ParserTest( NmeaAisPositionReportClassBParser parser );

        [When( "I parse '(.*)' with padding (.*) as a Position Report Class B" )]
        public void WhenIParseWithPaddingAsAPositionReportClassB( string payload, uint padding )
        {
            When( () => new NmeaAisPositionReportClassBParser( Encoding.ASCII.GetBytes( payload ), padding ) );
        }

        [Then( @"AisPositionReportClassBParser\.Type is (.*)" )]
        public void ThenAisPositionReportClassBParser_TypeIs( MessageType messageType )
        {
            Then( parser => Assert.AreEqual( messageType, parser.MessageType ) );
        }

        [Then( @"AisPositionReportClassBParser\.RepeatIndicator is (.*)" )]
        public void ThenAisPositionReportClassBParser_RepeatIndicatorIs( int repeatCount )
        {
            Then( parser => Assert.AreEqual( repeatCount, parser.RepeatIndicator ) );
        }

        [Then( @"AisPositionReportClassBParser\.Mmsi is (.*)" )]
        public void ThenAisPositionReportClassBParser_MmsiIs( int mmsi )
        {
            Then( parser => Assert.AreEqual( mmsi, parser.Mmsi ) );
        }

        [Then( @"AisPositionReportClassBParser\.SPareBits38 is (.*)" )]
        public void ThenAisPositionReportClassBParser_SpareBitsIs( uint spare )
        {
            Then( parser => Assert.AreEqual( spare, parser.SpareBits38 ) );
        }

        [Then( @"AisPositionReportClassBParser\.SpeedOverGroundTenths is (.*)" )]
        public void ThenAisPositionReportClassBParser_SpeedOverGroundTenthsIs( int speedOverGround )
        {
            Then( parser => Assert.AreEqual( speedOverGround, parser.SpeedOverGroundTenths ) );
        }

        [Then( @"AisPositionReportClassBParser\.PositionAccuracy is (.*)" )]
        public void ThenAisPositionReportClassBParser_PositionAccuracyIs( bool positionAccuracy )
        {
            Then( parser => Assert.AreEqual( positionAccuracy, parser.PositionAccuracy ) );
        }

        [Then( @"AisPositionReportClassBParser\.Longitude10000thMins is (.*)" )]
        public void ThenAisPositionReportClassBParser_LongitudeIs( int longitude )
        {
            Then( parser => Assert.AreEqual( longitude, parser.Longitude10000thMins ) );
        }

        [Then( @"AisPositionReportClassBParser\.Latitude10000thMins is (.*)" )]
        public void ThenAisPositionReportClassBParser_LatitudeIs( int latitude )
        {
            Then( parser => Assert.AreEqual( latitude, parser.Latitude10000thMins ) );
        }

        [Then( @"AisPositionReportClassBParser\.CourseOverGround10thDegrees is (.*)" )]
        public void ThenAisPositionReportClassBParser_CourseOverGroundIs( int courseOverGround )
        {
            Then( parser => Assert.AreEqual( courseOverGround, parser.CourseOverGround10thDegrees ) );
        }

        [Then( @"AisPositionReportClassBParser\.TrueHeadingDegrees is (.*)" )]
        public void ThenAisPositionReportClassBParser_TrueHeadingDegreesIs( int trueHeading )
        {
            Then( parser => Assert.AreEqual( trueHeading, parser.TrueHeadingDegrees ) );
        }

        [Then( @"AisPositionReportClassBParser\.TimeStampSecond is (.*)" )]
        public void ThenAisPositionReportClassBParser_TimeStampSecondIs( int timeStamp )
        {
            Then( parser => Assert.AreEqual( timeStamp, parser.TimeStampSecond ) );
        }

        [Then( @"AisPositionReportClassBParser\.SpareBits139 is (.*)" )]
        public void ThenAisPositionReportClassBParser_SpareBits139Is( uint spare )
        {
            Then( parser => Assert.AreEqual( spare, parser.SpareBits139 ) );
        }

        [Then( @"AisPositionReportClassBParser\.CsUnit is (.*)" )]
        public void ThenAisPositionReportClassBParser_CsUnitIsSotdma( ClassBUnit unit )
        {
            Then( parser => Assert.AreEqual( unit, parser.CsUnit ) );
        }

        [Then( @"AisPositionReportClassBParser\.HasDisplay is (.*)" )]
        public void ThenAisPositionReportClassBParser_HasDisplayIs( bool hasDisplay )
        {
            Then( parser => Assert.AreEqual( hasDisplay, parser.HasDisplay ) );
        }

        [Then( @"AisPositionReportClassBParser\.IsDscAttached is (.*)" )]
        public void ThenAisPositionReportClassBParser_IsDscAttached( bool isDscAttached )
        {
            Then( parser => Assert.AreEqual( isDscAttached, parser.IsDscAttached ) );
        }

        [Then( @"AisPositionReportClassBParser\.CanSwitchBands is (.*)" )]
        public void ThenAisPositionReportClassBParser_CanSwitchBands( bool canSwitchBands )
        {
            Then( parser => Assert.AreEqual( canSwitchBands, parser.CanSwitchBands ) );
        }

        [Then( @"AisPositionReportClassBParser\.CanAcceptMessage22ChannelAssignment is (.*)" )]
        public void ThenAisPositionReportClassBParser_CanAcceptMessage22ChannelAssignment( bool canAcceptMessage22ChannelAssignment )
        {
            Then( parser => Assert.AreEqual( canAcceptMessage22ChannelAssignment, parser.CanAcceptMessage22ChannelAssignment ) );
        }

        [Then( @"AisPositionReportClassBParser\.IsAssigned is (.*)" )]
        public void ThenAisPositionReportClassBParser_IsAssigned( bool isAssigned )
        {
            Then( parser => Assert.AreEqual( isAssigned, parser.IsAssigned ) );
        }

        [Then( @"AisPositionReportClassBParser\.RaimFlag is (.*)" )]
        public void ThenAisPositionReportClassBParser_RaimFlag( bool raim )
        {
            Then( parser => Assert.AreEqual( raim, parser.RaimFlag ) );
        }

        [Then( @"AisPositionReportClassBParser\.RadioStatusType is (.*)" )]
        public void ThenAisPositionReportClassBParser_RadioStatusTypeIsSOTDMA( ClassBRadioStatusType radioStatusType )
        {
            Then( parser => Assert.AreEqual( radioStatusType, parser.RadioStatusType ) );
        }

        void When( ParserMaker makeParser )
        {
            _makeParser = makeParser;
        }

        void Then( ParserTest test )
        {
            if( _makeParser is null ) throw new InvalidOperationException( $"When step must be called not called." );
            NmeaAisPositionReportClassBParser parser = _makeParser();
            test( parser );
        }
    }
}
