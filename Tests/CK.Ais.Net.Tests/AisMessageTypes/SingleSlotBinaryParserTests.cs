// <copyright file="SingleSlotBinaryParserTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using NUnit.Framework;
using Shouldly;
using System.Text;

namespace Ais.Net.Tests.AisMessageTypes;

// NmeaAisSingleSlotBinaryParser decodes the payload of message type 25 (Single Slot Binary Message):
// the optional destination and application identifier, depending on the addressed and structured flags,
// and the raw (still 6-bit ASCII armored) application data.
[TestFixture]
public class SingleSlotBinaryParserTests
{
    // The first 6 bits carry the message type (25).
    [Test]
    public void MessageType_is_decoded()
    {
        var parser = new NmeaAisSingleSlotBinaryParser( Encoding.ASCII.GetBytes( "I>M4ej0" ), 0 );
        parser.MessageType.ShouldBe( MessageType.SingleSlotBinaryMessage );
    }

    // Every field of broadcast, unstructured messages: destination and DAC/FI are absent (null).
    [TestCase( "I>M4ej0", 0u, MessageType.SingleSlotBinaryMessage, 0u, 970010056u, DestinationIndicator.Brocast, false, null, null, null, null, 4u, "0" )]
    [TestCase( "I8;@OB0", 0u, MessageType.SingleSlotBinaryMessage, 0u, 548675400u, DestinationIndicator.Brocast, false, null, null, null, null, 4u, "0" )]
    public void Full_message_is_decoded( string payload,
                                         uint padding,
                                         MessageType type,
                                         uint repeatIndicator,
                                         uint mmsi,
                                         DestinationIndicator destinationIndicator,
                                         bool binaryFlag,
                                         uint? destination,
                                         uint? spare70,
                                         uint? dac,
                                         uint? fi,
                                         uint applicationDataPadding,
                                         string applicationData )
    {
        var parser = new NmeaAisSingleSlotBinaryParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.MessageType.ShouldBe( type );
        parser.RepeatIndicator.ShouldBe( repeatIndicator );
        parser.Mmsi.ShouldBe( mmsi );
        parser.DestinationIndicator.ShouldBe( destinationIndicator );
        parser.BinaryDataFlag.ShouldBe( binaryFlag );
        parser.DestinationMmsi.ShouldBe( destination );
        parser.SpareBits70.ShouldBe( spare70 );
        parser.DAC.ShouldBe( dac );
        parser.FI.ShouldBe( fi );
        parser.ApplicationDataPaddingBefore.ShouldBe( applicationDataPadding );
        Encoding.ASCII.GetString( parser.ApplicationData ).ShouldBe( applicationData );
    }
}
