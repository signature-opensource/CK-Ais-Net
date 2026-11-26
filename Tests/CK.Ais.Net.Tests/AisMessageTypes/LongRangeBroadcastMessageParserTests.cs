// <copyright file="LongRangeBroadcastMessageParserTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using NUnit.Framework;
using System.Text;

namespace Ais.Net.Tests.AisMessageTypes;

// NmeaAisLongRangeBroadcastMessageParser decodes the payload of message type 27 (Long Range AIS Broadcast):
// navigation status, low resolution position (1/10 min), speed, course and position latency.
[TestFixture]
public class LongRangeBroadcastMessageParserTests
{
    // The first 6 bits carry the message type (27).
    [Test]
    public void MessageType_is_decoded()
    {
        var parser = new NmeaAisLongRangeBroadcastMessageParser( Encoding.ASCII.GetBytes( "KC5E2b@U19PFdLbMuc5=ROv62<7m" ), 0 );
        parser.MessageType.ShouldBe( MessageType.PositionReportForLongRangeApplication );
    }

    // Every field of real messages (the first one is longer than the standard 96 bits).
    [TestCase( "KC5E2b@U19PFdLbMuc5=ROv62<7m", 0u, MessageType.PositionReportForLongRangeApplication, 1u, 206914217u, false, false, NavigationStatus.NotUnderCommand, 82214, 2904, 57u, 167u, false, true )]
    [TestCase( "Kp15Li@1D=MeT5T@", 0u, MessageType.PositionReportForLongRangeApplication, 3u, 538008773u, false, false, NavigationStatus.UnderwayUsingEngine, 21557, -18744, 11u, 68u, false, false )]
    [TestCase( "Kk=>UB03fbh:R`7d", 0u, MessageType.PositionReportForLongRangeApplication, 3u, 215197000u, false, false, NavigationStatus.UnderwayUsingEngine, 61099, 1349, 16u, 123u, false, false )]
    public void Full_message_is_decoded( string payload,
                                         uint padding,
                                         MessageType type,
                                         uint repeatIndicator,
                                         uint mmsi,
                                         bool accuracy,
                                         bool raimFlag,
                                         NavigationStatus navigationStatus,
                                         int longitude,
                                         int latitude,
                                         uint sog,
                                         uint cog,
                                         bool latency,
                                         bool spare95 )
    {
        var parser = new NmeaAisLongRangeBroadcastMessageParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.MessageType.ShouldBe( type );
        parser.RepeatIndicator.ShouldBe( repeatIndicator );
        parser.Mmsi.ShouldBe( mmsi );
        parser.PositionAccuracy.ShouldBe( accuracy );
        parser.RaimFlag.ShouldBe( raimFlag );
        parser.NavigationStatus.ShouldBe( navigationStatus );
        parser.Longitude10thMins.ShouldBe( longitude );
        parser.Latitude10thMins.ShouldBe( latitude );
        parser.SpeedOverGround.ShouldBe( sog );
        parser.CourseOverGround.ShouldBe( cog );
        parser.PositionLatency.ShouldBe( latency );
        parser.SpareBit95.ShouldBe( spare95 );
    }
}
