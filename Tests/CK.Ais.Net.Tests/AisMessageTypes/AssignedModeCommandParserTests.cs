// <copyright file="AssignedModeCommandParserTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using NUnit.Framework;
using System.Text;

namespace Ais.Net.Tests.AisMessageTypes;

// NmeaAisAssignedModeCommandParser decodes the payload of message type 16 (Assigned Mode Command):
// one or two (destination MMSI, offset, increment) assignments.
[TestFixture]
public class AssignedModeCommandParserTests
{
    // The first 6 bits carry the message type (16).
    [Test]
    public void MessageType_is_decoded()
    {
        var parser = new NmeaAisAssignedModeCommandParser( Encoding.ASCII.GetBytes( "@02=VgPoD@C43h00" ), 0 );
        parser.MessageType.ShouldBe( MessageType.AssignmentModeCommand );
    }

    // Every field of a short (one assignment, B fields null, 4 spare bits at the end) and of a long
    // (two assignments, no spare bits so SpareBitsAtEnd is null) message.
    [TestCase( "@02=VgPoD@C43h00", 0u, MessageType.AssignmentModeCommand, 0u, 2320062u, 0u, 232014129u, 60u, 0u, null, null, null, 0u )]
    [TestCase( "@02a4KQD2cFP<P0000000500", 0u, MessageType.AssignmentModeCommand, 0u, 2770030u, 0u, 352497000u, 200u, 0u, 0u, 20u, 0u, null )]
    public void Full_message_is_decoded( string payload,
                                         uint padding,
                                         MessageType type,
                                         uint repeatIndicator,
                                         uint mmsi,
                                         uint spare38,
                                         uint destinationA,
                                         uint offsetA,
                                         uint incrementA,
                                         uint? destinationB,
                                         uint? offsetB,
                                         uint? incrementB,
                                         uint? spareEnd )
    {
        var parser = new NmeaAisAssignedModeCommandParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.MessageType.ShouldBe( type );
        parser.RepeatIndicator.ShouldBe( repeatIndicator );
        parser.Mmsi.ShouldBe( mmsi );
        parser.SpareBits38.ShouldBe( spare38 );
        parser.DestinationMmsiA.ShouldBe( destinationA );
        parser.OffsetA.ShouldBe( offsetA );
        parser.IncrementA.ShouldBe( incrementA );
        parser.DestinationMmsiB.ShouldBe( destinationB );
        parser.OffsetB.ShouldBe( offsetB );
        parser.IncrementB.ShouldBe( incrementB );
        parser.SpareBitsAtEnd.ShouldBe( spareEnd );
    }
}
