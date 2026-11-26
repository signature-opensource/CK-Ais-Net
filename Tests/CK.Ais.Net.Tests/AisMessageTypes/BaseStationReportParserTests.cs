// <copyright file="BaseStationReportParserTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using NUnit.Framework;
using System.Text;

namespace Ais.Net.Tests.AisMessageTypes;

// NmeaAisBaseStationReportParser decodes the payload of message type 4 (Base Station Report):
// UTC date and time, position, fix type and communication state of a base station.
[TestFixture]
public class BaseStationReportParserTests
{
    // The first 6 bits carry the message type (4).
    [Test]
    public void MessageType_is_decoded()
    {
        var parser = new NmeaAisBaseStationReportParser( Encoding.ASCII.GetBytes( "4028j;iu<JAU80>f7>H0elQ00000" ), 0 );
        parser.MessageType.ShouldBe( MessageType.BaseStationReport );
    }

    // Every field of real messages; longitude and latitude are checked in degrees (raw 1/10000 min / 600000).
    [TestCase( "4028j;iu<JAU80>f7>H0elQ00000", 0u, MessageType.BaseStationReport, 0u, 2241071u, 2003u, 1u, 20u, 17u, 37u, 8u, false, 3.215745, 41.96259, EpfdFixType.Gps, false, 0u, false, 0u )]
    [TestCase( "44`Uu;AvJEF`g14>V0DV@MQ00000", 0u, MessageType.BaseStationReport, 0u, 311000365u, 2022u, 9u, 10u, 22u, 40u, 47u, false, 14.90464, 35.99721, EpfdFixType.Gps, false, 0u, false, 0u )]
    public void Full_message_is_decoded( string payload,
                                         uint padding,
                                         MessageType type,
                                         uint repeatIndicator,
                                         uint mmsi,
                                         uint utcYear,
                                         uint utcMonth,
                                         uint utcDay,
                                         uint utcHour,
                                         uint utcMinute,
                                         uint utcSecond,
                                         bool positionAccuracy,
                                         double longitude,
                                         double latitude,
                                         EpfdFixType positionFixType,
                                         bool transmissionControlForLongRangeBroadcastMessage,
                                         uint spareBits139,
                                         bool raimFlag,
                                         uint communicationState )
    {
        var parser = new NmeaAisBaseStationReportParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.MessageType.ShouldBe( type );
        parser.RepeatIndicator.ShouldBe( repeatIndicator );
        parser.Mmsi.ShouldBe( mmsi );
        parser.UtcYear.ShouldBe( utcYear );
        parser.UtcMonth.ShouldBe( utcMonth );
        parser.UtcDay.ShouldBe( utcDay );
        parser.UtcHour.ShouldBe( utcHour );
        parser.UtcMinute.ShouldBe( utcMinute );
        parser.UtcSecond.ShouldBe( utcSecond );
        parser.PositionAccuracy.ShouldBe( positionAccuracy );
        // The SpecFlow binding compared these computed doubles exactly: a tolerance is now used.
        (parser.Longitude10000thMins / 600_000d).ShouldBe( longitude, tolerance: 1e-9 );
        (parser.Latitude10000thMins / 600_000d).ShouldBe( latitude, tolerance: 1e-9 );
        parser.PositionFixType.ShouldBe( positionFixType );
        parser.TransmissionControlForLongRangeBroadcastMessage.ShouldBe( transmissionControlForLongRangeBroadcastMessage );
        parser.SpareBits139.ShouldBe( spareBits139 );
        parser.RaimFlag.ShouldBe( raimFlag );
        parser.CommunicationState.ShouldBe( communicationState );
    }
}
