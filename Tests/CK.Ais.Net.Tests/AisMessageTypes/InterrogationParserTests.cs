// <copyright file="InterrogationParserTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using NUnit.Framework;
using Shouldly;
using System.Text;

namespace Ais.Net.Tests.AisMessageTypes;

// NmeaAisInterrogationParser decodes the payload of message type 15 (Interrogation): a first station
// interrogated for one or two message types, and an optional second station interrogated for one message type.
[TestFixture]
public class InterrogationParserTests
{
    // The first 6 bits carry the message type (15).
    [Test]
    public void MessageType_is_decoded()
    {
        var parser = new NmeaAisInterrogationParser( Encoding.ASCII.GetBytes( "?913QK18Uf;0D00" ), 0 );
        parser.MessageType.ShouldBe( MessageType.Interrigation );
    }

    // Every field of real messages interrogating one station for one or two message types:
    // the fields of the missing requests are null.
    [TestCase( "?913QK18Uf;0D00", 0u, MessageType.Interrigation, 0u, 605086060u, 0u, 304462000u, MessageType.StaticAndVoyageRelatedData, 0u, 0u, null, null, null, null, null, null, null )]
    [TestCase( "?1b60U0kNVOP<005000", 0u, MessageType.Interrigation, 0u, 111247508u, 0u, 215915000u, MessageType.PositionReport3, 0u, 0u, MessageType.StaticAndVoyageRelatedData, 0u, 0u, null, null, null, null )]
    [TestCase( "?77>7w1iprE@D00", 0u, MessageType.Interrigation, 0u, 477333500u, 0u, 477686100u, MessageType.StaticAndVoyageRelatedData, 0u, 0u, null, null, null, null, null, null, null )]
    public void Full_message_is_decoded( string payload,
                                         uint padding,
                                         MessageType type,
                                         uint repeatIndicator,
                                         uint mmsi,
                                         uint spare38,
                                         uint destinationMmsi1,
                                         MessageType messageType11,
                                         uint slotOffset11,
                                         uint? spare88,
                                         MessageType? messageType12,
                                         uint? slotOffset12,
                                         uint? spare108,
                                         uint? destinationMmsi2,
                                         MessageType? messageType21,
                                         uint? slotOffset21,
                                         uint? spare158 )
    {
        var parser = new NmeaAisInterrogationParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.MessageType.ShouldBe( type );
        parser.RepeatIndicator.ShouldBe( repeatIndicator );
        parser.Mmsi.ShouldBe( mmsi );
        parser.SpareBits38.ShouldBe( spare38 );
        parser.DestinationMmsi1.ShouldBe( destinationMmsi1 );
        parser.MessageType11.ShouldBe( messageType11 );
        parser.SlotOffset11.ShouldBe( slotOffset11 );
        parser.SpareBits88.ShouldBe( spare88 );
        parser.MessageType12.ShouldBe( messageType12 );
        parser.SlotOffset12.ShouldBe( slotOffset12 );
        parser.SpareBits108.ShouldBe( spare108 );
        parser.DestinationMmsi2.ShouldBe( destinationMmsi2 );
        // The SpecFlow binding took a uint? here while the property is a MessageType?: it only "worked"
        // because every example expects null. The expected value now has the property's type.
        parser.MessageType21.ShouldBe( messageType21 );
        parser.SlotOffset21.ShouldBe( slotOffset21 );
        parser.SpareBits158.ShouldBe( spare158 );
    }
}
