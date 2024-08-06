// <copyright file="StaticDataReportParserSpecsSteps.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using NUnit.Framework;
using System;
using System.Text;
using TechTalk.SpecFlow;

namespace Ais.Net.Specs.AisMessageTypes
{
    [Binding]
    public class StaticDataReportParserSpecsSteps
    {
        InitialParserMaker? _makeInitialParser;
        PartAParserMaker? _makePartAParser;
        PartBParserMaker? _makePartBParser;
        Exception? _exception;

        delegate uint InitialParserMaker();

        delegate NmeaAisStaticDataReportParserPartA PartAParserMaker();

        delegate NmeaAisStaticDataReportParserPartB PartBParserMaker();

        delegate void InitialParserTest( uint partNumber );

        delegate void PartAParserTest( NmeaAisStaticDataReportParserPartA parser );

        delegate void PartBParserTest( NmeaAisStaticDataReportParserPartB parser );

        [When( "I inspect the Static Data Report part of '(.*)' with padding (.*)" )]
        public void WhenIInspectTheStaticDataReportPartOfWithPadding( string payload, uint padding )
        {
            WhenInitial( () => NmeaAisStaticDataReportParser.GetPartNumber( Encoding.ASCII.GetBytes( payload ), padding ) );
        }

        [When( "I parse '(.*)' with padding (.*) as Static Data Report Part A" )]
        public void WhenIParseWithPaddingAsStaticDataReportPartA( string payload, uint padding )
        {
            WhenPartA( () => new NmeaAisStaticDataReportParserPartA( Encoding.ASCII.GetBytes( payload ), padding ) );
        }

        [When( "I parse '(.*)' with padding (.*) as Static Data Report Part B" )]
        public void WhenIParseWithPaddingAsStaticDataReportPartB( string payload, uint padding )
        {
            WhenPartB( () => new NmeaAisStaticDataReportParserPartB( Encoding.ASCII.GetBytes( payload ), padding ) );
        }

        [When( "I parse '(.*)' with padding (.*) as Static Data Report Part A catching exception" )]
        public void WhenIParseWithPaddingAsStaticDataReportPartACatchingException( string payload, uint padding )
        {
            try
            {
                var _ = new NmeaAisStaticDataReportParserPartA( Encoding.ASCII.GetBytes( payload ), padding );
                Assert.Fail( "Was expecting an exception" );
            }
            catch( Exception x )
            {
                _exception = x;
            }
        }

        [When( "I parse '(.*)' with padding (.*) as Static Data Report Part B catching exception" )]
        public void WhenIParseWithPaddingAsStaticDataReportPartBCatchingException( string payload, uint padding )
        {
            try
            {
                var _ = new NmeaAisStaticDataReportParserPartB( Encoding.ASCII.GetBytes( payload ), padding );
                Assert.Fail( "Was expecting an exception" );
            }
            catch( Exception x )
            {
                _exception = x;
            }
        }

        [Then( @"NmeaAisStaticDataReportParser\.GetPartNumber returns (.*)" )]
        public void ThenNmeaAisStaticDataReportParser_GetPartNumberReturns( uint partNumber )
        {
            ThenInitial( r => Assert.AreEqual( partNumber, r ) );
        }

        [Then( "the constructor throws ArgumentException" )]
        public void ThenTheConstructorThrowsArgumentException()
        {
            Assert.IsInstanceOf<ArgumentException>( _exception );
        }

        [Then( @"NmeaAisStaticDataReportParserPartA\.Type is (.*)" )]
        public void ThenNmeaAisStaticDataReportParserPartA_TypeIs( MessageType messageType )
        {
            ThenPartA( parser => Assert.AreEqual( messageType, parser.MessageType ) );
        }

        [Then( @"NmeaAisStaticDataReportParserPartB\.Type is (.*)" )]
        public void ThenNmeaAisStaticDataReportParserPartB_TypeIs( MessageType messageType )
        {
            ThenPartB( parser => Assert.AreEqual( messageType, parser.MessageType ) );
        }

        [Then( @"NmeaAisStaticDataReportParserPartA\.RepeatIndicator is (.*)" )]
        public void ThenNmeaAisStaticDataReportParserPartA_RepeatIndicatorIs( uint repeatCount )
        {
            ThenPartA( parser => Assert.AreEqual( repeatCount, parser.RepeatIndicator ) );
        }

        [Then( @"NmeaAisStaticDataReportParserPartB\.RepeatIndicator is (.*)" )]
        public void ThenNmeaAisStaticDataReportParserPartB_RepeatIndicatorIs( uint repeatCount )
        {
            ThenPartB( parser => Assert.AreEqual( repeatCount, parser.RepeatIndicator ) );
        }

        [Then( @"NmeaAisStaticDataReportParserPartA\.Mmsi is (.*)" )]
        public void ThenNmeaAisStaticDataReportParserPartA_MmsiIs( int mmsi )
        {
            ThenPartA( parser => Assert.AreEqual( mmsi, parser.Mmsi ) );
        }

        [Then( @"NmeaAisStaticDataReportParserPartB\.Mmsi is (.*)" )]
        public void ThenNmeaAisStaticDataReportParserPartB_MmsiIs( int mmsi )
        {
            ThenPartB( parser => Assert.AreEqual( mmsi, parser.Mmsi ) );
        }

        [Then( @"NmeaAisStaticDataReportParserPartA\.PartNumber is (.*)" )]
        public void ThenNmeaAisStaticDataReportParserPartA_PartNumberIs( int partNumber )
        {
            ThenPartA( parser => Assert.AreEqual( partNumber, parser.PartNumber ) );
        }

        [Then( @"NmeaAisStaticDataReportParserPartB\.PartNumber is (.*)" )]
        public void ThenNmeaAisStaticDataReportParserPartB_PartNumberIs( int partNumber )
        {
            ThenPartB( parser => Assert.AreEqual( partNumber, parser.PartNumber ) );
        }

        [Then( @"NmeaAisStaticDataReportParserPartA\.VesselName is (.*)" )]
        public void ThenNmeaAisStaticDataReportParserPartA_VesselNameIs( string vesselName )
        {
            ThenPartA( parser => AisStringsSpecsSteps.TestString( vesselName, 20, parser.VesselName ) );
        }

        [Then( @"NmeaAisStaticDataReportParserPartA\.Spare is (.*)" )]
        public void ThenNmeaAisStaticDataReportParserPartA_SpareIs( uint spare )
        {
            ThenPartA( parser => Assert.AreEqual( spare, parser.Spare160 ) );
        }

        [Then( @"NmeaAisStaticDataReportParserPartB\.ShipType is (.*)" )]
        public void ThenNmeaAisStaticDataReportParserPartB_ShipTypeIs( ShipType type )
        {
            ThenPartB( parser => Assert.AreEqual( type, parser.ShipType ) );
        }

        [Then( @"NmeaAisStaticDataReportParserPartB\.VendorIdRev3 is (.*)" )]
        public void ThenNmeaAisStaticDataReportParserPartB_VendorIdRev3Is( string vendorId )
        {
            ThenPartB( parser => AisStringsSpecsSteps.TestString( vendorId, 7, parser.VendorIdRev3 ) );
        }

        [Then( @"NmeaAisStaticDataReportParserPartB\.VendorIdRev4 is (.*)" )]
        public void ThenNmeaAisStaticDataReportParserPartB_VendorIdRev4Is( string vendorId )
        {
            ThenPartB( parser => AisStringsSpecsSteps.TestString( vendorId, 3, parser.VendorIdRev4 ) );
        }

        [Then( @"NmeaAisStaticDataReportParserPartB\.UnitModelCode is (.*)" )]
        public void ThenNmeaAisStaticDataReportParserPartB_UnitModelCodeIs( uint unitModelCode )
        {
            ThenPartB( parser => Assert.AreEqual( unitModelCode, parser.UnitModelCode ) );
        }

        [Then( @"NmeaAisStaticDataReportParserPartB\.SerialNumber is (.*)" )]
        public void ThenNmeaAisStaticDataReportParserPartB_SerialNumberIs( uint serialNumber )
        {
            ThenPartB( parser => Assert.AreEqual( serialNumber, parser.SerialNumber ) );
        }

        [Then( @"NmeaAisStaticDataReportParserPartB\.CallSign is (.*)" )]
        public void ThenNmeaAisStaticDataReportParserPartB_CallSignIs( string callSign )
        {
            ThenPartB( parser => AisStringsSpecsSteps.TestString( callSign, 7, parser.CallSign ) );
        }

        [Then( @"NmeaAisStaticDataReportParserPartB\.DimensionToBow is (.*)" )]
        public void ThenNmeaAisStaticDataReportParserPartB_DimensionToBowIs( uint size )
        {
            ThenPartB( parser => Assert.AreEqual( size, parser.DimensionToBow ) );
        }

        [Then( @"NmeaAisStaticDataReportParserPartB\.DimensionToStern is (.*)" )]
        public void ThenNmeaAisStaticDataReportParserPartB_DimensionToSternIs( uint size )
        {
            ThenPartB( parser => Assert.AreEqual( size, parser.DimensionToStern ) );
        }

        [Then( @"NmeaAisStaticDataReportParserPartB\.DimensionToPort is (.*)" )]
        public void ThenNmeaAisStaticDataReportParserPartB_DimensionToPortIs( uint size )
        {
            ThenPartB( parser => Assert.AreEqual( size, parser.DimensionToPort ) );
        }

        [Then( @"NmeaAisStaticDataReportParserPartB\.DimensionToStarboard is (.*)" )]
        public void ThenNmeaAisStaticDataReportParserPartB_DimensionToStarboardIs( uint size )
        {
            ThenPartB( parser => Assert.AreEqual( size, parser.DimensionToStarboard ) );
        }

        [Then( @"NmeaAisStaticDataReportParserPartB\.MothershipMmsi is (.*)" )]
        public void ThenNmeaAisStaticDataReportParserPartB_MothershipMmsiIs( uint mmsi )
        {
            ThenPartB( parser => Assert.AreEqual( mmsi, parser.MothershipMmsi ) );
        }

        [Then( @"NmeaAisStaticDataReportParserPartB\.EpfdFixType is (.*)" )]
        public void ThenNmeaAisStaticDataReportParserPartB_EpfdFixTypeIs( EpfdFixType mmsi )
        {
            ThenPartB( parser => Assert.AreEqual( mmsi, parser.EpfdFixType ) );
        }

        [Then( @"NmeaAisStaticDataReportParserPartB\.Spare is (.*)" )]
        public void ThenNmeaAisStaticDataReportParserPartB_SpareIs( uint spare )
        {
            ThenPartB( parser => Assert.AreEqual( spare, parser.Spare162 ) );
        }

        void WhenInitial( InitialParserMaker makeParser )
        {
            _makeInitialParser = makeParser;
        }

        void WhenPartA( PartAParserMaker makeParser )
        {
            _makePartAParser = makeParser;
        }

        void WhenPartB( PartBParserMaker makeParser )
        {
            _makePartBParser = makeParser;
        }

        void ThenInitial( InitialParserTest test )
        {
            if( _makeInitialParser is null ) throw new InvalidOperationException( $"Then initial step must be called." );
            uint messagePart = _makeInitialParser();
            test( messagePart );
        }

        void ThenPartA( PartAParserTest test )
        {
            if( _makePartAParser is null ) throw new InvalidOperationException( $"Then Part A step must be called." );
            NmeaAisStaticDataReportParserPartA parser = _makePartAParser();
            test( parser );
        }

        void ThenPartB( PartBParserTest test )
        {
            if( _makePartBParser is null ) throw new InvalidOperationException( $"Then Part B step must be called." );
            NmeaAisStaticDataReportParserPartB parser = _makePartBParser();
            test( parser );
        }
    }
}
