// <copyright file="SafetyRelatedBroadcastParserTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using NUnit.Framework;
using Shouldly;
using System.Text;

namespace Ais.Net.Tests.AisMessageTypes;

// NmeaAisSafetyRelatedBroadcastParser decodes the payload of message type 14 (Safety Related Broadcast Message):
// header and the free 6-bit text that fills the rest of the message.
[TestFixture]
public class SafetyRelatedBroadcastParserTests
{
    // The first 6 bits carry the message type (14).
    [Test]
    public void MessageType_is_decoded()
    {
        var parser = new NmeaAisSafetyRelatedBroadcastParser( Encoding.ASCII.GetBytes( ">>l:N8hp5HTL5@Ttp4j1L58pTpN2b`9Eb37:bb0U>0tHJ10u<U@Ttp0" ), 0 );
        parser.MessageType.ShouldBe( MessageType.SafetyRelatedBroadcastMessage );
    }

    // Every field of real messages, including the whole text (bit 40 to the end, '@' padding included).
    [TestCase( ">3@pJu04U>10ib04<f1@qR1<4HF1HuT4LF0", 0u, MessageType.SafetyRelatedBroadcastMessage, 0u, 219028212u, 0u, "AIS PLZ ACK TNX SAFE VOYAGE " )]
    [TestCase( ">>m>cDPp5HTL5@Ttp4j1L58pTpN2ba0EA8tr1<8nbb0U>0H4ThTpL0", 0u, MessageType.SafetyRelatedBroadcastMessage, 0u, 995339090u, 0u, "NAVIGATIONAL WARNING **PETRON SBM** IS FAILING@" )]
    [TestCase( ">8HweV1@E=@m<N0", 0u, MessageType.SafetyRelatedBroadcastMessage, 0u, 563080600u, 0u, "TESTMSG " )]
    public void Full_message_is_decoded( string payload,
                                         uint padding,
                                         MessageType type,
                                         uint repeatIndicator,
                                         uint mmsi,
                                         uint spare38,
                                         string safetyRelatedText )
    {
        var parser = new NmeaAisSafetyRelatedBroadcastParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.MessageType.ShouldBe( type );
        parser.RepeatIndicator.ShouldBe( repeatIndicator );
        parser.Mmsi.ShouldBe( mmsi );
        parser.SpareBits38.ShouldBe( spare38 );
        // The SpecFlow binding compared the expected string to the raw byte[] written by WriteAsAscii:
        // this only passed thanks to NUnit loose (element-wise) equality. The bytes are now decoded to a string.
        var text = new byte[parser.SafetyRelatedText.CharacterCount];
        parser.SafetyRelatedText.WriteAsAscii( text );
        Encoding.ASCII.GetString( text ).ShouldBe( safetyRelatedText );
    }
}
