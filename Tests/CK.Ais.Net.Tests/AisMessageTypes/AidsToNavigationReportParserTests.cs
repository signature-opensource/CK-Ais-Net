// <copyright file="AidsToNavigationReportParserTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using NUnit.Framework;
using System.Text;

namespace Ais.Net.Tests.AisMessageTypes;

// NmeaAisAidsToNavigationReportParser decodes the payload of message type 21 (Aids-to-Navigation Report):
// AtoN type and name, position, position references, status flags and the optional name extension.
[TestFixture]
public class AidsToNavigationReportParserTests
{
    // The first 6 bits carry the message type (21).
    [Test]
    public void MessageType_is_decoded()
    {
        var parser = new NmeaAisAidsToNavigationReportParser( Encoding.ASCII.GetBytes( "Evlt<Cf50QUaWW@97QUP0000000D8U=0r5W0P00003jP10" ), 0 );
        parser.MessageType.ShouldBe( MessageType.AidsToNavigationReport );
    }

    // Every field of real messages: the 20-char name is read as-is ('@' padding included), the name
    // extension is empty when the message stops at bit 272.
    [TestCase( "Evlt<Cf50QUaWW@97QUP0000000D8U=0r5W0P00003jP10", 0u, MessageType.AidsToNavigationReport, 3u, 995036238u, AidsToNavigationType.IsolatedDanger, "JACKSON ROCK@@@@@@@@", true, 69358400, -12399100, 0u, 0u, 0u, 0u, EpfdFixType.Surveyed, 37u, false, 0u, false, true, false, false, "", 0u )]
    [TestCase( "E>nRFnO77h0W1T7a9hFh84`2V4W@3AEb1fHgh00003aP11H0DQ@H>@", 0u, MessageType.AidsToNavigationReport, 0u, 996710105u, AidsToNavigationType.SpecialMark, "NO ANCHORS - PIPELIN", true, 857450, 3617150, 0u, 0u, 0u, 0u, EpfdFixType.Surveyed, 19u, false, 0u, false, true, false, false, "E AREA 9", 0u )]
    [TestCase( "E>nlfV1`:Rab7h;4Sh<h1WW:@:9MbW`ghDbC@10888gh20A`0UAC`4m@iDh", 0u, MessageType.AidsToNavigationReport, 0u, 997011096u, AidsToNavigationType.FixedStructure, "PUESTO VIG Y CONT TR", true, -39159249, -32877414, 1u, 1u, 1u, 1u, EpfdFixType.Gps, 31u, true, 0u, true, false, false, false, "AF BUEN SUCES", 0u )]
    public void Full_message_is_decoded( string payload,
                                         uint padding,
                                         MessageType type,
                                         uint repeatIndicator,
                                         uint mmsi,
                                         AidsToNavigationType atonType,
                                         string nameAton,
                                         bool accuracy,
                                         int longitude,
                                         int latitude,
                                         uint positionA,
                                         uint positionB,
                                         uint positionC,
                                         uint positionD,
                                         EpfdFixType epfdFixType,
                                         uint timestamp,
                                         bool offPosition,
                                         uint atonStatus,
                                         bool raimFlag,
                                         bool virtualAton,
                                         bool assignedMode,
                                         bool spare241,
                                         string nameAtonExtension,
                                         uint spareEnd )
    {
        var parser = new NmeaAisAidsToNavigationReportParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.MessageType.ShouldBe( type );
        parser.RepeatIndicator.ShouldBe( repeatIndicator );
        parser.Mmsi.ShouldBe( mmsi );
        parser.AidsToNavigationType.ShouldBe( atonType );
        ToAscii( parser.NameOfAidsToNavigation ).ShouldBe( nameAton );
        parser.PositionAccuracy.ShouldBe( accuracy );
        parser.Longitude10000thMins.ShouldBe( longitude );
        parser.Latitude10000thMins.ShouldBe( latitude );
        parser.ReferenceForPositionA.ShouldBe( positionA );
        parser.ReferenceForPositionB.ShouldBe( positionB );
        parser.ReferenceForPositionC.ShouldBe( positionC );
        parser.ReferenceForPositionD.ShouldBe( positionD );
        parser.EpfdFixType.ShouldBe( epfdFixType );
        parser.TimeStampSecond.ShouldBe( timestamp );
        parser.OffPositionIndicator.ShouldBe( offPosition );
        parser.AtoNStatus.ShouldBe( atonStatus );
        parser.RaimFlag.ShouldBe( raimFlag );
        parser.VirtualAtoN.ShouldBe( virtualAton );
        parser.AssignedMode.ShouldBe( assignedMode );
        parser.SpareBit241.ShouldBe( spare241 );
        ToAscii( parser.NameOfAidToNavigationExtension ).ShouldBe( nameAtonExtension );
        parser.SpareBitsAtEnd.ShouldBe( spareEnd );
    }

    // Messages that stop before the optional extension (no spare bit at 271 / padding 5): the trailing
    // fields must still be read without overflowing (empty extension, no spare bits).
    [TestCase( "E>jN6CQcb3hKH@80cSh;TW26WV2P9`dh?2aHhu7gg`N000", 5u, false, "", 0u )]
    [TestCase( "E>k`sFdJUP00000000000000000=31wP7up8H00000N000", 5u, false, "", 0u )]
    [TestCase( "E>kaTLdqK000000000000000000<t0K78WdMp00000N010", 5u, false, "", 0u )]
    public void Message_without_spare_bit_at_271_is_decoded( string payload, uint padding, bool spare241, string nameAtonExtension, uint spareEnd )
    {
        var parser = new NmeaAisAidsToNavigationReportParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.SpareBit241.ShouldBe( spare241 );
        ToAscii( parser.NameOfAidToNavigationExtension ).ShouldBe( nameAtonExtension );
        parser.SpareBitsAtEnd.ShouldBe( spareEnd );
    }

    static string ToAscii( NmeaAisTextFieldParser text )
    {
        var bytes = new byte[text.CharacterCount];
        text.WriteAsAscii( bytes );
        return Encoding.ASCII.GetString( bytes );
    }
}
