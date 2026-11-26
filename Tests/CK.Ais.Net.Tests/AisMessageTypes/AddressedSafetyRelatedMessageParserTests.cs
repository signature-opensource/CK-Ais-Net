// <copyright file="AddressedSafetyRelatedMessageParserTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using NUnit.Framework;
using System.Text;

namespace Ais.Net.Tests.AisMessageTypes;

// NmeaAisAddressedSafetyRelatedMessageParser decodes the payload of message type 12 (Addressed Safety Related Message):
// header, destination MMSI and the free 6-bit text that fills the rest of the message.
[TestFixture]
public class AddressedSafetyRelatedMessageParserTests
{
    // The first 6 bits carry the message type (12).
    [Test]
    public void MessageType_is_decoded()
    {
        var parser = new NmeaAisAddressedSafetyRelatedMessageParser( Encoding.ASCII.GetBytes( "<37ood0ne>80P@<CP3853;P=IP19CP41D1P1>4PB5@<I" ), 0 );
        parser.MessageType.ShouldBe( MessageType.AddressedSafetyRelatedMessage );
    }

    // Every field of real messages, including the whole text (bit 72 to the end, '@' padding included).
    [TestCase( "<37ood0ne>80P@<CP3853;P=IP19CP41D1P1>4PB5@<I", 0u, MessageType.AddressedSafetyRelatedMessage, 0u, 209582000u, 0u, 229456000u, false, false, " PLS CHECK MY AIS DATA AND REPLY" )]
    [TestCase( "<815N7n0AABp19CPD5CDP@<CP13;QQ0", 0u, MessageType.AddressedSafetyRelatedMessage, 0u, 538009119u, 1u, 538002734u, false, false, "AIS TEST PLS ACK!!@" )]
    [TestCase( "<9N`1nBD@=8L7??4P41IQP5D1PD?PBFPilrhh<DP`ED3Pckrhha0", 0u, MessageType.AddressedSafetyRelatedMessage, 0u, 636092889u, 0u, 621819015u, false, false, "GOOD DAY! ETA TO RV 14:00LT (UTC +3:00)@" )]
    public void Full_message_is_decoded( string payload,
                                         uint padding,
                                         MessageType type,
                                         uint repeatIndicator,
                                         uint mmsi,
                                         uint sequenceNumber,
                                         uint destinationMmsi,
                                         bool retransmit,
                                         bool spare71,
                                         string safetyRelatedText )
    {
        var parser = new NmeaAisAddressedSafetyRelatedMessageParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.MessageType.ShouldBe( type );
        parser.RepeatIndicator.ShouldBe( repeatIndicator );
        parser.Mmsi.ShouldBe( mmsi );
        parser.SequenceNumber.ShouldBe( sequenceNumber );
        parser.DestinationMmsi.ShouldBe( destinationMmsi );
        parser.Retransmit.ShouldBe( retransmit );
        parser.SpareBit71.ShouldBe( spare71 );
        // The SpecFlow binding compared the expected string to the raw byte[] written by WriteAsAscii:
        // this only passed thanks to NUnit loose (element-wise) equality. The bytes are now decoded to a string.
        var text = new byte[parser.SafetyRelatedText.CharacterCount];
        parser.SafetyRelatedText.WriteAsAscii( text );
        Encoding.ASCII.GetString( text ).ShouldBe( safetyRelatedText );
    }
}
