// <copyright file="CoordinatedUniversalTimeAndDateInquiryParserTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using NUnit.Framework;
using Shouldly;
using System.Text;

namespace Ais.Net.Tests.AisMessageTypes;

// NmeaAisCoordinatedUniversalTimeAndDateInquiryParser decodes the payload of message type 10
// (UTC and Date Inquiry): sender and destination MMSI.
[TestFixture]
public class CoordinatedUniversalTimeAndDateInquiryParserTests
{
    // The first 6 bits carry the message type (10).
    [Test]
    public void MessageType_is_decoded()
    {
        var parser = new NmeaAisCoordinatedUniversalTimeAndDateInquiryParser( Encoding.ASCII.GetBytes( ":815J6hrsEKP" ), 0 );
        parser.MessageType.ShouldBe( MessageType.UTCDateInquiry );
    }

    // Bits 6-7: every possible repeat indicator value.
    [TestCase( ":00000000000", 0u, 0u )]
    [TestCase( ":@0000000000", 0u, 1u )]
    [TestCase( ":P0000000000", 0u, 2u )]
    [TestCase( ":h0000000000", 0u, 3u )]
    public void RepeatIndicator_is_decoded( string payload, uint padding, uint repeatCount )
    {
        var parser = new NmeaAisCoordinatedUniversalTimeAndDateInquiryParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.RepeatIndicator.ShouldBe( repeatCount );
    }

    // Bits 8-37: the lowest MMSI bits.
    [TestCase( "K00000000000", 0u, 0u )]
    [TestCase( "K00000@00000", 0u, 1u )]
    [TestCase( "K00000P00000", 0u, 2u )]
    public void Mmsi_is_decoded( string payload, uint padding, uint mmsi )
    {
        var parser = new NmeaAisCoordinatedUniversalTimeAndDateInquiryParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.Mmsi.ShouldBe( mmsi );
    }

    // Every field of real messages.
    [TestCase( ":815J6hrsEKP", 0u, MessageType.UTCDateInquiry, 0u, 538008091u, 0u, 247158200u, 0u )]
    [TestCase( ":9NWsc1sgBDL", 0u, MessageType.UTCDateInquiry, 0u, 636091308u, 0u, 518998343u, 0u )]
    [TestCase( ":5Tjij10WiuP", 0u, MessageType.UTCDateInquiry, 0u, 374125000u, 0u, 271042520u, 0u )]
    public void Full_message_is_decoded( string payload,
                                         uint padding,
                                         MessageType type,
                                         uint repeatIndicator,
                                         uint mmsi,
                                         uint spare38,
                                         uint destinationMmsi,
                                         uint spare70 )
    {
        var parser = new NmeaAisCoordinatedUniversalTimeAndDateInquiryParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.MessageType.ShouldBe( type );
        parser.RepeatIndicator.ShouldBe( repeatIndicator );
        parser.Mmsi.ShouldBe( mmsi );
        parser.SpareBits38.ShouldBe( spare38 );
        parser.DestinationMmsi.ShouldBe( destinationMmsi );
        parser.SpareBits70.ShouldBe( spare70 );
    }
}
