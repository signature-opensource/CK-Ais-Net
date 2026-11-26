// <copyright file="BinaryBroadcastMessageParserTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using NUnit.Framework;
using Shouldly;
using System.Text;

namespace Ais.Net.Tests.AisMessageTypes;

// NmeaAisBinaryBroadcastMessageParser decodes the payload of message type 8 (Binary Broadcast Message):
// header, DAC/FI application identifier and the raw (still 6-bit ASCII armored) application data.
[TestFixture]
public class BinaryBroadcastMessageParserTests
{
    // The first 6 bits carry the message type (8).
    [Test]
    public void MessageType_is_decoded()
    {
        var parser = new NmeaAisBinaryBroadcastMessageParser( Encoding.ASCII.GetBytes( "8:U>M;P0G@:?>G1?6600" ), 0 );
        parser.MessageType.ShouldBe( MessageType.BinaryBroadcastMessage );
    }

    // Every field of real messages; the application data is the payload slice that follows the FI field
    // (it starts with 2 bits belonging to the header).
    [TestCase( "88uQwV00G@:?>G1?6600", 0u, MessageType.BinaryBroadcastMessage, 0u, 601391000u, 0u, 1u, 29u, 2u, "@:?>G1?6600" )]
    [TestCase( "85PH6giKf;2=fq`s>8IP:=I4EKA@AUU=KCjl;ndkut;MvLM45ebS", 0u, MessageType.BinaryBroadcastMessage, 0u, 369493695u, 0u, 366u, 56u, 2u, ";2=fq`s>8IP:=I4EKA@AUU=KCjl;ndkut;MvLM45ebS" )]
    [TestCase( "84eJBS0j2d<<<<<<<0HPOg`50000", 0u, MessageType.BinaryBroadcastMessage, 0u, 316052108u, 0u, 200u, 10u, 2u, "d<<<<<<<0HPOg`50000" )]
    public void Full_message_is_decoded( string payload,
                                         uint padding,
                                         MessageType type,
                                         uint repeatIndicator,
                                         uint mmsi,
                                         uint spare38,
                                         uint dac,
                                         uint fi,
                                         uint applicationDataPadding,
                                         string applicationData )
    {
        var parser = new NmeaAisBinaryBroadcastMessageParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.MessageType.ShouldBe( type );
        parser.RepeatIndicator.ShouldBe( repeatIndicator );
        parser.Mmsi.ShouldBe( mmsi );
        parser.SpareBits38.ShouldBe( spare38 );
        parser.DAC.ShouldBe( dac );
        parser.FI.ShouldBe( fi );
        parser.ApplicationDataPaddingBefore.ShouldBe( applicationDataPadding );
        Encoding.ASCII.GetString( parser.ApplicationData ).ShouldBe( applicationData );
    }
}
