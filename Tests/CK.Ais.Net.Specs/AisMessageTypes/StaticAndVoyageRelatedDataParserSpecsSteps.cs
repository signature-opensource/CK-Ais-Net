// <copyright file="StaticAndVoyageRelatedDataParserSpecsSteps.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using NUnit.Framework;
using System;
using System.Text;
using TechTalk.SpecFlow;

namespace Ais.Net.Specs.AisMessageTypes;

[Binding]
public class StaticAndVoyageRelatedDataParserSpecsSteps
{
    ParserMaker? _makeParser;

    delegate NmeaAisStaticAndVoyageRelatedDataParser ParserMaker();

    delegate void ParserTest( NmeaAisStaticAndVoyageRelatedDataParser parser );

    [When( "I parse '(.*)' with padding (.*) as Static and Voyage Related Data" )]
    public void WhenIParseWithPaddingAsStaticAndVoyageRelatedData( string payload, uint padding )
    {
        When( () => new NmeaAisStaticAndVoyageRelatedDataParser( Encoding.ASCII.GetBytes( payload ), padding ) );
    }

    [Then( @"NmeaAisStaticAndVoyageRelatedDataParser\.Type is (.*)" )]
    public void ThenNmeaAisStaticAndVoyageRelatedDataParser_TypeIs( MessageType messageType )
    {
        Then( parser => Assert.AreEqual( messageType, parser.MessageType ) );
    }

    [Then( @"NmeaAisStaticAndVoyageRelatedDataParser\.RepeatIndicator is (.*)" )]
    public void ThenNmeaAisStaticAndVoyageRelatedDataParser_RepeatIndicatorIs( uint repeatCount )
    {
        Then( parser => Assert.AreEqual( repeatCount, parser.RepeatIndicator ) );
    }

    [Then( @"NmeaAisStaticAndVoyageRelatedDataParser\.Mmsi is (.*)" )]
    public void ThenNmeaAisStaticAndVoyageRelatedDataParser_MmsiIs( int mmsi )
    {
        Then( parser => Assert.AreEqual( mmsi, parser.Mmsi ) );
    }

    [Then( @"NmeaAisStaticAndVoyageRelatedDataParser\.AisVersion is (.*)" )]
    public void ThenNmeaAisStaticAndVoyageRelatedDataParser_AisVersionIs( int aisVersion )
    {
        Then( parser => Assert.AreEqual( aisVersion, parser.AisVersion ) );
    }

    [Then( @"NmeaAisStaticAndVoyageRelatedDataParser\.ImoNumber is (.*)" )]
    public void ThenNmeaAisStaticAndVoyageRelatedDataParser_ImoNumberIs( int imoNumber )
    {
        Then( parser => Assert.AreEqual( imoNumber, parser.ImoNumber ) );
    }

    [Then( @"NmeaAisStaticAndVoyageRelatedDataParser\.CallSign is (.*)" )]
    public void ThenNmeaAisStaticAndVoyageRelatedDataParser_CallSignIs( string callSign )
    {
        Then( parser => AisStringsSpecsSteps.TestString( callSign, 7, parser.CallSign ) );
    }

    [Then( @"NmeaAisStaticAndVoyageRelatedDataParser\.VesselName is (.*)" )]
    public void ThenNmeaAisStaticAndVoyageRelatedDataParser_VesselNameIs( string vesselName )
    {
        Then( parser => AisStringsSpecsSteps.TestString( vesselName, 20, parser.VesselName ) );
    }

    [Then( @"NmeaAisStaticAndVoyageRelatedDataParser\.ShipType is (.*)" )]
    public void ThenNmeaAisStaticAndVoyageRelatedDataParser_ShipTypeIs( ShipType type )
    {
        Then( parser => Assert.AreEqual( type, parser.ShipType ) );
    }

    [Then( @"NmeaAisStaticAndVoyageRelatedDataParser\.DimensionToBow is (.*)" )]
    public void ThenNmeaAisStaticAndVoyageRelatedDataParser_DimensionToBowIs( int size )
    {
        Then( parser => Assert.AreEqual( size, parser.DimensionToBow ) );
    }

    [Then( @"NmeaAisStaticAndVoyageRelatedDataParser\.DimensionToStern is (.*)" )]
    public void ThenNmeaAisStaticAndVoyageRelatedDataParser_DimensionToSternIs( int size )
    {
        Then( parser => Assert.AreEqual( size, parser.DimensionToStern ) );
    }

    [Then( @"NmeaAisStaticAndVoyageRelatedDataParser\.DimensionToPort is (.*)" )]
    public void ThenNmeaAisStaticAndVoyageRelatedDataParser_DimensionToPortIs( int size )
    {
        Then( parser => Assert.AreEqual( size, parser.DimensionToPort ) );
    }

    [Then( @"NmeaAisStaticAndVoyageRelatedDataParser\.DimensionToStarboard is (.*)" )]
    public void ThenNmeaAisStaticAndVoyageRelatedDataParser_DimensionToStarboardIs( int size )
    {
        Then( parser => Assert.AreEqual( size, parser.DimensionToStarboard ) );
    }

    [Then( @"NmeaAisStaticAndVoyageRelatedDataParser\.PositionFixType is (.*)" )]
    public void ThenNmeaAisStaticAndVoyageRelatedDataParser_PositionFixTypeIsUndefined( EpfdFixType epfd )
    {
        Then( parser => Assert.AreEqual( epfd, parser.PositionFixType ) );
    }

    [Then( @"NmeaAisStaticAndVoyageRelatedDataParser\.EtaMonth is (.*)" )]
    public void ThenNmeaAisStaticAndVoyageRelatedDataParser_EtaMonthIs( int month )
    {
        Then( parser => Assert.AreEqual( month, parser.EtaMonth ) );
    }

    [Then( @"NmeaAisStaticAndVoyageRelatedDataParser\.EtaDay is (.*)" )]
    public void ThenNmeaAisStaticAndVoyageRelatedDataParser_EtaDayIs( int day )
    {
        Then( parser => Assert.AreEqual( day, parser.EtaDay ) );
    }

    [Then( @"NmeaAisStaticAndVoyageRelatedDataParser\.EtaHour is (.*)" )]
    public void ThenNmeaAisStaticAndVoyageRelatedDataParser_EtaHourIs( int hour )
    {
        Then( parser => Assert.AreEqual( hour, parser.EtaHour ) );
    }

    [Then( @"NmeaAisStaticAndVoyageRelatedDataParser\.EtaMinute is (.*)" )]
    public void ThenNmeaAisStaticAndVoyageRelatedDataParser_EtaMinuteIs( int minute )
    {
        Then( parser => Assert.AreEqual( minute, parser.EtaMinute ) );
    }

    [Then( @"NmeaAisStaticAndVoyageRelatedDataParser\.Draught10thMetres is (.*)" )]
    public void ThenNmeaAisStaticAndVoyageRelatedDataParser_DraughtthMetresIs( int draught )
    {
        Then( parser => Assert.AreEqual( draught, parser.Draught10thMetres ) );
    }

    [Then( @"NmeaAisStaticAndVoyageRelatedDataParser\.Destination is (.*)" )]
    public void ThenNmeaAisStaticAndVoyageRelatedDataParser_DestinationIs( string destination )
    {
        Then( parser => AisStringsSpecsSteps.TestString( destination, 20, parser.Destination ) );
    }

    [Then( @"NmeaAisStaticAndVoyageRelatedDataParser\.DteNotReady is (.*)" )]
    public void ThenNmeaAisStaticAndVoyageRelatedDataParser_DteNotReadyIsFalse( bool notReady )
    {
        Then( parser => Assert.AreEqual( notReady, parser.IsDteNotReady ) );
    }

        [Then( @"NmeaAisStaticAndVoyageRelatedDataParser\.SpareBit423 is (.*)" )]
        public void ThenNmeaAisStaticAndVoyageRelatedDataParser_SpareBitIs( bool spare )
    {
            Then( parser => Assert.AreEqual( spare, parser.SpareBit423 ) );
    }

    void When( ParserMaker makeParser )
    {
        _makeParser = makeParser;
    }

    void Then( ParserTest test )
    {
        if( _makeParser is null ) throw new InvalidOperationException( $"When step must be called." );
        NmeaAisStaticAndVoyageRelatedDataParser parser = _makeParser();
        test( parser );
    }
}
