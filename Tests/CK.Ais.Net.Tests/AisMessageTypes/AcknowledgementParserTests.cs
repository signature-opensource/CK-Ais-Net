// <copyright file="AcknowledgementParserTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using NUnit.Framework;
using Shouldly;
using System.Text;

namespace Ais.Net.Tests.AisMessageTypes;

// NmeaAisAcknowledgementParser decodes the payload of message type 13 (Safety Related Acknowledgement):
// the sender MMSI followed by 1 to 4 (destination MMSI, sequence number) pairs.
[TestFixture]
public class AcknowledgementParserTests
{
    // The first 6 bits carry the message type (13).
    [Test]
    public void MessageType_is_decoded()
    {
        var parser = new NmeaAisAcknowledgementParser( Encoding.ASCII.GetBytes( "=8156b@iuus2" ), 0 );
        parser.MessageType.ShouldBe( MessageType.SafetyRelatedAcknoledgement );
    }

    // Every field of real messages carrying 1, 2 and 4 acknowledged destinations: absent destinations are null.
    [TestCase( "=8156b@iuus2", 0u, MessageType.SafetyRelatedAcknoledgement, 0u, 538003113u, 0u, 209582000u, 2u, null, null, null, null, null, null )]
    [TestCase( "=5@jof1HMrCPF7NTph", 0u, MessageType.SafetyRelatedAcknoledgement, 0u, 353155000u, 0u, 371059000u, 0u, 371059000u, 3u, null, null, null, null )]
    [TestCase( "=6D2aSc?SAghBHVwTP0<0;<4:AOwhgs17wpEF:NuHP8oBsti@Rep80klilPCQ830BiH8888", 0u, MessageType.SafetyRelatedAcknoledgement, 0u, 423668110u, 2u, 870532860u, 0u, 308441060u, 2u, 196652u, 3u, 17450495u, 3u )]
    public void Full_message_is_decoded( string payload,
                                         uint padding,
                                         MessageType type,
                                         uint repeatIndicator,
                                         uint mmsi,
                                         uint spare38,
                                         uint destination1,
                                         uint sequence1,
                                         uint? destination2,
                                         uint? sequence2,
                                         uint? destination3,
                                         uint? sequence3,
                                         uint? destination4,
                                         uint? sequence4 )
    {
        var parser = new NmeaAisAcknowledgementParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.MessageType.ShouldBe( type );
        parser.RepeatIndicator.ShouldBe( repeatIndicator );
        parser.Mmsi.ShouldBe( mmsi );
        parser.SpareBits38.ShouldBe( spare38 );
        parser.DestinationMmsi1.ShouldBe( destination1 );
        parser.SequenceNumberMmsi1.ShouldBe( sequence1 );
        parser.DestinationMmsi2.ShouldBe( destination2 );
        parser.SequenceNumberMmsi2.ShouldBe( sequence2 );
        parser.DestinationMmsi3.ShouldBe( destination3 );
        parser.SequenceNumberMmsi3.ShouldBe( sequence3 );
        parser.DestinationMmsi4.ShouldBe( destination4 );
        parser.SequenceNumberMmsi4.ShouldBe( sequence4 );
    }
}
