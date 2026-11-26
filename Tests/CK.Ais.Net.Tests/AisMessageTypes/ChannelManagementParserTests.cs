// <copyright file="ChannelManagementParserTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using NUnit.Framework;
using System.Text;

namespace Ais.Net.Tests.AisMessageTypes;

// NmeaAisChannelManagementParser decodes the payload of message type 22 (Channel Management):
// channels, Tx/Rx mode, power, the two corners of the area (or addressed stations), bandwidths and zone size.
[TestFixture]
public class ChannelManagementParserTests
{
    // The first 6 bits carry the message type (22).
    [Test]
    public void MessageType_is_decoded()
    {
        var parser = new NmeaAisChannelManagementParser( Encoding.ASCII.GetBytes( "F028n@R2N2P3D73EB6`>6bT20000" ), 0 );
        parser.MessageType.ShouldBe( MessageType.ChannelManagement );
    }

    // Every field of a broadcast (geographic area) and of an addressed message.
    [TestCase( "F028n@R2N2P3D73EB6`>6bT20000", 0u, MessageType.ChannelManagement, 0u, 2242114u, 0u, 2087u, 2088u, 0u, false, 108600, 54600, 108600, 54600, DestinationIndicator.Brocast, false, false, 4u, 0u )]
    [TestCase( "FM5293Ppsrh1S?SmKP0>0BOkJ0t0", 0u, MessageType.ChannelManagement, 1u, 877693198u, 0u, 910u, 4012u, 0u, false, 50812, 62830, 56, 2367, DestinationIndicator.Addressed, false, false, 6u, 6819584u )]
    public void Full_message_is_decoded( string payload,
                                         uint padding,
                                         MessageType type,
                                         uint repeatIndicator,
                                         uint mmsi,
                                         uint spare38,
                                         uint channelA,
                                         uint channelB,
                                         uint txRx,
                                         bool lowPower,
                                         int longitude1,
                                         int latitude1,
                                         int longitude2,
                                         int latitude2,
                                         DestinationIndicator indicator,
                                         bool bandwidthA,
                                         bool bandwidthB,
                                         uint zoneSize,
                                         uint spare145 )
    {
        var parser = new NmeaAisChannelManagementParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.MessageType.ShouldBe( type );
        parser.RepeatIndicator.ShouldBe( repeatIndicator );
        parser.Mmsi.ShouldBe( mmsi );
        parser.SpareBits38.ShouldBe( spare38 );
        parser.ChannelA.ShouldBe( channelA );
        parser.ChannelB.ShouldBe( channelB );
        parser.TxRxMode.ShouldBe( txRx );
        parser.LowPower.ShouldBe( lowPower );
        parser.Longitude10thMins1.ShouldBe( longitude1 );
        parser.Latitude10thMins1.ShouldBe( latitude1 );
        parser.Longitude10thMins2.ShouldBe( longitude2 );
        parser.Latitude10thMins2.ShouldBe( latitude2 );
        parser.DestinationIndicator.ShouldBe( indicator );
        parser.ChannelABandwidth.ShouldBe( bandwidthA );
        parser.ChannelBBandwidth.ShouldBe( bandwidthB );
        parser.TransitionalZoneSize.ShouldBe( zoneSize );
        parser.SpareBits145.ShouldBe( spare145 );
    }
}
