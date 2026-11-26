// <copyright file="CoordinatedUniversalTimeAndDateResponseParserTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using NUnit.Framework;
using Shouldly;
using System.Text;

namespace Ais.Net.Tests.AisMessageTypes;

// NmeaAisCoordinatedUniversalTimeAndDateResponseParser decodes the payload of message type 11
// (UTC and Date Response), which has the same layout as message type 4 (Base Station Report).
// The test payloads are the base station report ones with the type character changed.
[TestFixture]
public class CoordinatedUniversalTimeAndDateResponseParserTests
{
    // The first 6 bits carry the message type (11).
    [Test]
    public void MessageType_is_decoded()
    {
        var parser = new NmeaAisCoordinatedUniversalTimeAndDateResponseParser( Encoding.ASCII.GetBytes( ";028j;iu<JAU80>f7>H0elQ00000" ), 0 );
        parser.MessageType.ShouldBe( MessageType.UTCDateResponse );
    }

    // Every field; longitude and latitude are checked in degrees (raw 1/10000 min / 600000).
    [TestCase( ";028j;iu<JAU80>f7>H0elQ00000", 0u, MessageType.UTCDateResponse, 0u, 2241071u, 2003u, 1u, 20u, 17u, 37u, 8u, false, 3.215745, 41.96259, EpfdFixType.Gps, false, 0u, false, 0u )]
    [TestCase( ";4`Uu;AvJEF`g14>V0DV@MQ00000", 0u, MessageType.UTCDateResponse, 0u, 311000365u, 2022u, 9u, 10u, 22u, 40u, 47u, false, 14.90464, 35.99721, EpfdFixType.Gps, false, 0u, false, 0u )]
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
        var parser = new NmeaAisCoordinatedUniversalTimeAndDateResponseParser( Encoding.ASCII.GetBytes( payload ), padding );
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
