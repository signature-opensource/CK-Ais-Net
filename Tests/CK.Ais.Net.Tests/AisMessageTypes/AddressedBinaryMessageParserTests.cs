// <copyright file="AddressedBinaryMessageParserTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using NUnit.Framework;
using Shouldly;
using System.Text;

namespace Ais.Net.Tests.AisMessageTypes;

// NmeaAisAddressedBinaryMessageParser decodes the payload of message type 6 (Addressed Binary Message):
// header, destination, DAC/FI application identifier and the raw (still 6-bit ASCII armored) application data.
[TestFixture]
public class AddressedBinaryMessageParserTests
{
    // The first 6 bits carry the message type (6).
    [Test]
    public void MessageType_is_decoded()
    {
        var parser = new NmeaAisAddressedBinaryMessageParser( Encoding.ASCII.GetBytes( "6>kKL=000000>d`u06?D000" ), 0 );
        parser.MessageType.ShouldBe( MessageType.BinaryAddressedMessage );
    }

    // Every field of real messages; the application data is the payload slice that follows the FI field
    // (it starts with 4 bits belonging to the header).
    [TestCase( "6>oHhp000000>da30000@00", 0u, MessageType.BinaryAddressedMessage, 0u, 997601504u, 0u, 0u, false, false, 235u, 10u, 4u, "a30000@00" )]
    [TestCase( "6>oNma0JOJtL078NuS1`s6mQ0vCv048@0002P1401`0000000000", 0u, MessageType.BinaryAddressedMessage, 0u, 997701028u, 0u, 111111111u, false, false, 1u, 50u, 4u, "8NuS1`s6mQ0vCv048@0002P1401`0000000000" )]
    [TestCase( "6>p?paQREindIt4<", 0u, MessageType.BinaryAddressedMessage, 0u, 998504614u, 0u, 412469099u, false, false, 415u, 1u, 4u, "4<" )]
    public void Full_message_is_decoded( string payload,
                                         uint padding,
                                         MessageType type,
                                         uint repeatIndicator,
                                         uint mmsi,
                                         uint sequenceNumber,
                                         uint destinationMmsi,
                                         bool retransmit,
                                         bool spare71,
                                         uint dac,
                                         uint fi,
                                         uint applicationDataPadding,
                                         string applicationData )
    {
        var parser = new NmeaAisAddressedBinaryMessageParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.MessageType.ShouldBe( type );
        parser.RepeatIndicator.ShouldBe( repeatIndicator );
        parser.Mmsi.ShouldBe( mmsi );
        parser.SequenceNumber.ShouldBe( sequenceNumber );
        parser.DestinationMmsi.ShouldBe( destinationMmsi );
        parser.Retransmit.ShouldBe( retransmit );
        parser.SpareBit71.ShouldBe( spare71 );
        parser.DAC.ShouldBe( dac );
        parser.FI.ShouldBe( fi );
        parser.ApplicationDataPaddingBefore.ShouldBe( applicationDataPadding );
        Encoding.ASCII.GetString( parser.ApplicationData ).ShouldBe( applicationData );
    }
}
