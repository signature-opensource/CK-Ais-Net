// <copyright file="DataLinkManagementMessageParserTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using NUnit.Framework;
using Shouldly;
using System.Text;

namespace Ais.Net.Tests.AisMessageTypes;

// NmeaAisDataLinkManagementMessageParser decodes the payload of message type 20 (Data Link Management Message):
// 1 to 4 slot reservations (offset, number of slots, timeout, increment).
[TestFixture]
public class DataLinkManagementMessageParserTests
{
    // The first 6 bits carry the message type (20).
    [Test]
    public void MessageType_is_decoded()
    {
        var parser = new NmeaAisDataLinkManagementMessageParser( Encoding.ASCII.GetBytes( "D02;bMhRl@fq6DA6DB0i6D0" ), 0 );
        parser.MessageType.ShouldBe( MessageType.DataLinkManagementMessage );
    }

    // Every field of real messages carrying 3 reservations: the 4th reservation and the trailing spare bits are null.
    [TestCase( "D02;bMhRl@fq6DA6DB0i6D0", 0u, MessageType.DataLinkManagementMessage, 0u, 2288247u, 0u, 557u, 1u, 0u, 750u, 1125u, 1u, 0u, 1125u, 288u, 3u, 0u, 1125u, null, null, null, null, null )]
    [TestCase( "D02;bMR0tLfp00M6EpDu6D0", 0u, MessageType.DataLinkManagementMessage, 0u, 2288246u, 0u, 2063u, 1u, 6u, 750u, 0u, 1u, 6u, 1125u, 1925u, 3u, 6u, 1125u, null, null, null, null, null )]
    public void Full_message_is_decoded( string payload,
                                         uint padding,
                                         MessageType type,
                                         uint repeatIndicator,
                                         uint mmsi,
                                         uint spare38,
                                         uint offset1,
                                         uint slot1,
                                         uint timeout1,
                                         uint increment1,
                                         uint? offset2,
                                         uint? slot2,
                                         uint? timeout2,
                                         uint? increment2,
                                         uint? offset3,
                                         uint? slot3,
                                         uint? timeout3,
                                         uint? increment3,
                                         uint? offset4,
                                         uint? slot4,
                                         uint? timeout4,
                                         uint? increment4,
                                         uint? spareEnd )
    {
        var parser = new NmeaAisDataLinkManagementMessageParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.MessageType.ShouldBe( type );
        parser.RepeatIndicator.ShouldBe( repeatIndicator );
        parser.Mmsi.ShouldBe( mmsi );
        parser.SpareBits38.ShouldBe( spare38 );
        parser.Offset1.ShouldBe( offset1 );
        parser.SlotNumber1.ShouldBe( slot1 );
        parser.Timeout1.ShouldBe( timeout1 );
        parser.Increment1.ShouldBe( increment1 );
        parser.Offset2.ShouldBe( offset2 );
        parser.SlotNumber2.ShouldBe( slot2 );
        parser.Timeout2.ShouldBe( timeout2 );
        parser.Increment2.ShouldBe( increment2 );
        parser.Offset3.ShouldBe( offset3 );
        parser.SlotNumber3.ShouldBe( slot3 );
        parser.Timeout3.ShouldBe( timeout3 );
        parser.Increment3.ShouldBe( increment3 );
        parser.Offset4.ShouldBe( offset4 );
        parser.SlotNumber4.ShouldBe( slot4 );
        parser.Timeout4.ShouldBe( timeout4 );
        parser.Increment4.ShouldBe( increment4 );
        parser.SpareBitsAtEnd.ShouldBe( spareEnd );
    }
}
