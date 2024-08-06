// <copyright file="PositionReportExtendedClassBParserSpecsSteps.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using NUnit.Framework;
using System;
using System.Text;
using TechTalk.SpecFlow;

namespace Ais.Net.Specs.AisMessageTypes
{
    [Binding]
    public class PositionReportExtendedClassBParserSpecsSteps
    {
        ParserMaker? _makeParser;

        delegate NmeaAisPositionReportExtendedClassBParser ParserMaker();

        delegate void ParserTest( NmeaAisPositionReportExtendedClassBParser parser );

        [When( "I parse '(.*)' with padding (.*) as a Position Report Extended Class B" )]
        public void WhenIParseWithPaddingAsAPositionReportExtendedClassB( string payload, uint padding )
        {
            When( () => new NmeaAisPositionReportExtendedClassBParser( Encoding.ASCII.GetBytes( payload ), padding ) );
        }

        [Then( @"NmeaAisPositionReportExtendedClassBParser\.Type is (.*)" )]
        public void ThenNmeaAisPositionReportExtendedClassBParser_TypeIs( MessageType messageType )
        {
            Then( parser => Assert.AreEqual( messageType, parser.MessageType ) );
        }

        [Then( @"NmeaAisPositionReportExtendedClassBParser\.RepeatIndicator is (.*)" )]
        public void ThenNmeaAisPositionReportExtendedClassBParser_RepeatIndicatorIs( int repeatCount )
        {
            Then( parser => Assert.AreEqual( repeatCount, parser.RepeatIndicator ) );
        }

        [Then( @"NmeaAisPositionReportExtendedClassBParser\.Mmsi is (.*)" )]
        public void ThenNmeaAisPositionReportExtendedClassBParser_MmsiIs( int mmsi )
        {
            Then( parser => Assert.AreEqual( mmsi, parser.Mmsi ) );
        }

        [Then( @"NmeaAisPositionReportExtendedClassBParser\.RegionalReserved38 is (.*)" )]
        public void ThenNmeaAisPositionReportExtendedClassBParser_RegionalReserve38dIs( int reserved )
        {
            Then( parser => Assert.AreEqual( reserved, parser.RegionalReserved38 ) );
        }

        [Then( @"NmeaAisPositionReportExtendedClassBParser\.SpeedOverGroundTenths is (.*)" )]
        public void ThenNmeaAisPositionReportExtendedClassBParser_SpeedOverGroundTenthsIs( int speedOverGround )
        {
            Then( parser => Assert.AreEqual( speedOverGround, parser.SpeedOverGroundTenths ) );
        }

        [Then( @"NmeaAisPositionReportExtendedClassBParser\.PositionAccuracy is (.*)" )]
        public void ThenNmeaAisPositionReportExtendedClassBParser_PositionAccuracyIs( bool positionAccuracy )
        {
            Then( parser => Assert.AreEqual( positionAccuracy, parser.PositionAccuracy ) );
        }

        [Then( @"NmeaAisPositionReportExtendedClassBParser\.Longitude10000thMins is (.*)" )]
        public void ThenNmeaAisPositionReportExtendedClassBParser_LongitudeIs( int longitude )
        {
            Then( parser => Assert.AreEqual( longitude, parser.Longitude10000thMins ) );
        }

        [Then( @"NmeaAisPositionReportExtendedClassBParser\.Latitude10000thMins is (.*)" )]
        public void ThenNmeaAisPositionReportExtendedClassBParser_LatitudeIs( int latitude )
        {
            Then( parser => Assert.AreEqual( latitude, parser.Latitude10000thMins ) );
        }

        [Then( @"NmeaAisPositionReportExtendedClassBParser\.CourseOverGround10thDegrees is (.*)" )]
        public void ThenNmeaAisPositionReportExtendedClassBParser_CourseOverGroundIs( int courseOverGround )
        {
            Then( parser => Assert.AreEqual( courseOverGround, parser.CourseOverGround10thDegrees ) );
        }

        [Then( @"NmeaAisPositionReportExtendedClassBParser\.TrueHeadingDegrees is (.*)" )]
        public void ThenNmeaAisPositionReportExtendedClassBParser_TrueHeadingDegreesIs( int trueHeading )
        {
            Then( parser => Assert.AreEqual( trueHeading, parser.TrueHeadingDegrees ) );
        }

        [Then( @"NmeaAisPositionReportExtendedClassBParser\.TimeStampSecond is (.*)" )]
        public void ThenNmeaAisPositionReportExtendedClassBParser_TimeStampSecondIs( int timeStamp )
        {
            Then( parser => Assert.AreEqual( timeStamp, parser.TimeStampSecond ) );
        }

        [Then( @"NmeaAisPositionReportExtendedClassBParser\.RegionalReserved139 is (.*)" )]
        public void ThenNmeaAisPositionReportExtendedClassBParser_RegionalReserved139( uint reserved )
        {
            Then( parser => Assert.AreEqual( reserved, parser.RegionalReserved139 ) );
        }

        [Then( @"NmeaAisPositionReportExtendedClassBParser\.Name is '(.*)'" )]
        public void ThenNmeaAisPositionReportExtendedClassBParser_NameIs( string name )
        {
            Then( parser => AisStringsSpecsSteps.TestString( name, 20, parser.ShipName ) );
        }

        [Then( @"NmeaAisPositionReportExtendedClassBParser\.ShipType is (.*)" )]
        public void ThenNmeaAisPositionReportExtendedClassBParser_ShipTypeIs( ShipType shipType )
        {
            Then( parser => Assert.AreEqual( shipType, parser.ShipType ) );
        }

        [Then( @"NmeaAisPositionReportExtendedClassBParser\.DimensionToBow is (.*)" )]
        public void ThenNmeaAisPositionReportExtendedClassBParser_DimensionToBowIs( int size )
        {
            Then( parser => Assert.AreEqual( size, parser.DimensionToBow ) );
        }

        [Then( @"NmeaAisPositionReportExtendedClassBParser\.DimensionToStern is (.*)" )]
        public void ThenNmeaAisPositionReportExtendedClassBParser_DimensionToSternIs( int size )
        {
            Then( parser => Assert.AreEqual( size, parser.DimensionToStern ) );
        }

        [Then( @"NmeaAisPositionReportExtendedClassBParser\.DimensionToPort is (.*)" )]
        public void ThenNmeaAisPositionReportExtendedClassBParser_DimensionToPortIs( int size )
        {
            Then( parser => Assert.AreEqual( size, parser.DimensionToPort ) );
        }

        [Then( @"NmeaAisPositionReportExtendedClassBParser\.DimensionToStarboard is (.*)" )]
        public void ThenNmeaAisPositionReportExtendedClassBParser_DimensionToStarboardIs( int size )
        {
            Then( parser => Assert.AreEqual( size, parser.DimensionToStarboard ) );
        }

        [Then( @"NmeaAisPositionReportExtendedClassBParser\.PositionFixType is (.*)" )]
        public void ThenNmeaAisPositionReportExtendedClassBParser_PositionFixTypeIs( EpfdFixType epfd )
        {
            Then( parser => Assert.AreEqual( epfd, parser.PositionFixType ) );
        }

        [Then( @"NmeaAisPositionReportExtendedClassBParser\.RaimFlag is (.*)" )]
        public void ThenNmeaAisPositionReportExtendedClassBParser_RaimFlagIs( bool raim )
        {
            Then( parser => Assert.AreEqual( raim, parser.RaimFlag ) );
        }

        [Then( @"NmeaAisPositionReportExtendedClassBParser\.DteNotReady is (.*)" )]
        public void ThenNmeaAisPositionReportExtendedClassBParser_DteNotReadyIs( bool isDteNotReady )
        {
            Then( parser => Assert.AreEqual( isDteNotReady, parser.IsDteNotReady ) );
        }

        [Then( @"NmeaAisPositionReportExtendedClassBParser\.IsAssigned is (.*)" )]
        public void ThenNmeaAisPositionReportExtendedClassBParser_IsAssigned( bool isAssigned )
        {
            Then( parser => Assert.AreEqual( isAssigned, parser.IsAssigned ) );
        }

        [Then( @"NmeaAisPositionReportExtendedClassBParser\.Spare308 is (.*)" )]
        public void ThenNmeaAisPositionReportExtendedClassBParser_Spare308( int spare )
        {
            Then( parser => Assert.AreEqual( spare, parser.Spare308 ) );
        }

        void When( ParserMaker makeParser )
        {
            _makeParser = makeParser;
        }

        void Then( ParserTest test )
        {
            if( _makeParser is null ) throw new InvalidOperationException( $"When step must be called not called." );
            NmeaAisPositionReportExtendedClassBParser parser = _makeParser();
            test( parser );
        }
    }
}
