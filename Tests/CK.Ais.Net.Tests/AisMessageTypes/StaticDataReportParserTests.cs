// <copyright file="StaticDataReportParserTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>
//
// Contains data under the Norwegian licence for Open Government data (NLOD) distributed by
// the Norwegian Costal Administration - https://ais.kystverket.no/
// The license can be found at https://data.norge.no/nlod/en/2.0
// The lines in this file that contain data from this source are annotated with a comment containing "ais.kystverket.no"
// The NLOD applies only to the data in these annotated lines. The license under which you are using this software
// (either the AGPLv3, or a commercial license) applies to the whole file.

using CK.Core;
using NUnit.Framework;
using Shouldly;
using System;
using System.Text;

namespace Ais.Net.Tests.AisMessageTypes;

// Message type 24 (Static Data Report), sent by class B transponders, comes in two forms selected by
// the part number (bits 38-39): Part A (0) carries the vessel name, Part B (1) carries the ship type,
// vendor id, call sign and dimensions (or the mothership MMSI for auxiliary craft).
// Covered here: NmeaAisStaticDataReportParser.GetPartNumber, the rejection of the wrong part by the
// NmeaAisStaticDataReportParserPartA/PartB constructors, and each field of both parts, against synthetic
// payloads (only the bits of the field are set) and real messages from ais.kystverket.no.
//
// Apparently the spec says all of these should be 168 bits long. However, in practice,
// it is common for transmitters to sent only 160 bits for Part A messages, leaving out
// the final 'Spare' 8 bits. In fact, in all the sample data we have I've not been able
// to find a single example of a Part A message that is 168 bits long. But allegedly
// that's what they're supposed to look like
// That's why our synthesized examples contain both 160 and 168 bit Part A messages
// (27 characters with 2 padding bits, or 28 characters without padding),
// but our real examples contain only a 160 bit Part A message.
[TestFixture]
public class StaticDataReportParserTests
{
    // GetPartNumber reads the part number (bits 38-39) without choosing a parser: 0 is Part A, 1 is Part B,
    // 2 and 3 are not defined by the spec but are returned as is.
    [TestCase( "H00000000000000000000000000", 2u, 0u )]
    [TestCase( "H000000000000000000000000000", 0u, 0u )]
    [TestCase( "H000004000000000000000000000", 0u, 1u )]
    [TestCase( "H000008000000000000000000000", 0u, 2u )]
    [TestCase( "H00000<000000000000000000000", 0u, 3u )]
    [TestCase( "H3mhO30PDT@V04pU@4000000000", 2u, 0u )]   // ais.kystverket.no
    [TestCase( "H3m<KD4NC=D5l@<<:F;000204240", 0u, 1u )]   // ais.kystverket.no
    public void GetPartNumber_returns_the_part_number( string payload, uint padding, uint expected )
    {
        NmeaAisStaticDataReportParser.GetPartNumber( Encoding.ASCII.GetBytes( payload ), padding ).ShouldBe( expected );
    }

    // The Part B parser rejects a Part A message.
    // Fixed: the SpecFlow step called Assert.Fail inside a try/catch( Exception ) block, so the AssertionException
    // raised when no exception was thrown was caught and stored like the expected exception.
    [Test]
    public void Reading_PartA_as_PartB_throws()
    {
        Util.Invokable( () => { _ = new NmeaAisStaticDataReportParserPartB( Encoding.ASCII.GetBytes( "H3mhO30PDT@V04pU@4000000000" ), 2 ); } )   // ais.kystverket.no
            .ShouldThrow<ArgumentException>();
    }

    // The Part A parser rejects a Part B message.
    // Fixed: the SpecFlow step called Assert.Fail inside a try/catch( Exception ) block, so the AssertionException
    // raised when no exception was thrown was caught and stored like the expected exception.
    [Test]
    public void Reading_PartB_as_PartA_throws()
    {
        Util.Invokable( () => { _ = new NmeaAisStaticDataReportParserPartA( Encoding.ASCII.GetBytes( "H3m<KD4NC=D5l@<<:F;000204240" ), 0 ); } )   // ais.kystverket.no
            .ShouldThrow<ArgumentException>();
    }

    // Part A: message type (bits 0-5) is 24 and part number (bits 38-39) is 0.
    // Fixed: the SpecFlow scenario only checked parser.PartNumber, which can never fail since the constructor throws
    // when the part number is not 0. The test now also checks that the Part B parser rejects the payload.
    [TestCase( "H00000000000000000000000000", 2u )]
    [TestCase( "H000000000000000000000000000", 0u )]
    [TestCase( "H3mhO30PDT@V04pU@4000000000", 2u )]   // ais.kystverket.no
    public void PartA_MessageType_and_PartNumber_are_decoded( string payload, uint padding )
    {
        var parser = new NmeaAisStaticDataReportParserPartA( Encoding.ASCII.GetBytes( payload ), padding );
        parser.MessageType.ShouldBe( MessageType.StaticDataReport );
        parser.PartNumber.ShouldBe( 0u );
        Util.Invokable( () => { _ = new NmeaAisStaticDataReportParserPartB( Encoding.ASCII.GetBytes( payload ), padding ); } )
            .ShouldThrow<ArgumentException>();
    }

    // Part B: message type (bits 0-5) is 24 and part number (bits 38-39) is 1.
    // Fixed: the SpecFlow scenario only checked parser.PartNumber, which can never fail since the constructor throws
    // when the part number is not 1. The test now also checks that the Part A parser rejects the payload.
    [TestCase( "H000004000000000000000000000", 0u )]
    [TestCase( "H3m<KD4NC=D5l@<<:F;000204240", 0u )]   // ais.kystverket.no
    public void PartB_MessageType_and_PartNumber_are_decoded( string payload, uint padding )
    {
        var parser = new NmeaAisStaticDataReportParserPartB( Encoding.ASCII.GetBytes( payload ), padding );
        parser.MessageType.ShouldBe( MessageType.StaticDataReport );
        parser.PartNumber.ShouldBe( 1u );
        Util.Invokable( () => { _ = new NmeaAisStaticDataReportParserPartA( Encoding.ASCII.GetBytes( payload ), padding ); } )
            .ShouldThrow<ArgumentException>();
    }

    // Part A: repeat indicator, 2 bits (6-7), from 0 to 3, in both the 160 and 168 bits forms.
    [TestCase( "H00000000000000000000000000", 2u, 0u )]
    [TestCase( "H000000000000000000000000000", 0u, 0u )]
    [TestCase( "H@0000000000000000000000000", 2u, 1u )]
    [TestCase( "H@00000000000000000000000000", 0u, 1u )]
    [TestCase( "HP0000000000000000000000000", 2u, 2u )]
    [TestCase( "Hh0000000000000000000000000", 2u, 3u )]
    [TestCase( "H3mhO30PDT@V04pU@4000000000", 2u, 0u )]   // ais.kystverket.no
    public void PartA_RepeatIndicator_is_decoded( string payload, uint padding, uint expected )
    {
        var parser = new NmeaAisStaticDataReportParserPartA( Encoding.ASCII.GetBytes( payload ), padding );
        parser.RepeatIndicator.ShouldBe( expected );
    }

    // Part B: repeat indicator, 2 bits (6-7), from 0 to 3.
    [TestCase( "H000004000000000000000000000", 0u, 0u )]
    [TestCase( "H@00004000000000000000000000", 0u, 1u )]
    [TestCase( "HP00004000000000000000000000", 0u, 2u )]
    [TestCase( "Hh00004000000000000000000000", 0u, 3u )]
    [TestCase( "H3m<KD4NC=D5l@<<:F;000204240", 0u, 0u )]   // ais.kystverket.no
    public void PartB_RepeatIndicator_is_decoded( string payload, uint padding, uint expected )
    {
        var parser = new NmeaAisStaticDataReportParserPartB( Encoding.ASCII.GetBytes( payload ), padding );
        parser.RepeatIndicator.ShouldBe( expected );
    }

    // Part A: MMSI, 30 bits (8-37).
    [TestCase( "H00000000000000000000000000", 2u, 0u )]
    [TestCase( "H00000@00000000000000000000", 2u, 1u )]
    [TestCase( "H00000P00000000000000000000", 2u, 2u )]
    [TestCase( "H>eq`d@00000000000000000000", 2u, 987654321u )]
    [TestCase( "H3mhO30PDT@V04pU@4000000000", 2u, 257695500u )]   // ais.kystverket.no
    [TestCase( "H3m8;Q1A8Tt0000000000000000", 2u, 257035140u )]   // ais.kystverket.no
    public void PartA_Mmsi_is_decoded( string payload, uint padding, uint expected )
    {
        var parser = new NmeaAisStaticDataReportParserPartA( Encoding.ASCII.GetBytes( payload ), padding );
        parser.Mmsi.ShouldBe( expected );
    }

    // Part B: MMSI, 30 bits (8-37).
    [TestCase( "H000004000000000000000000000", 0u, 0u )]
    [TestCase( "H00000D000000000000000000000", 0u, 1u )]
    [TestCase( "H00000T000000000000000000000", 0u, 2u )]
    [TestCase( "H3m<KD4NC=D5l@<<:F;000204240", 0u, 257104720u )]   // ais.kystverket.no
    [TestCase( "H3m76H4N@B?>1F0<;mnoh0107320", 0u, 257017440u )]   // ais.kystverket.no
    public void PartB_Mmsi_is_decoded( string payload, uint padding, uint expected )
    {
        var parser = new NmeaAisStaticDataReportParserPartB( Encoding.ASCII.GetBytes( payload ), padding );
        parser.Mmsi.ShouldBe( expected );
    }

    // Part A: vessel name, 20 six-bit characters (bits 40-159), '@' padded: first and last characters are checked.
    [TestCase( "H00000000000000000000000000", 2u, "@@@@@@@@@@@@@@@@@@@@" )]
    [TestCase( "H00000100000000000000000000", 2u, "P@@@@@@@@@@@@@@@@@@@" )]
    [TestCase( "H00000300000000000000000000", 2u, "0@@@@@@@@@@@@@@@@@@@" )]
    [TestCase( "H00000000000000000000000010", 2u, "@@@@@@@@@@@@@@@@@@@P" )]
    [TestCase( "H00000000000000000000000004", 2u, "@@@@@@@@@@@@@@@@@@@A" )]
    [TestCase( "H3mhO30PDT@V04pU@4000000000", 2u, "HEIDI ANITA@@@@@@@@@" )]   // ais.kystverket.no
    [TestCase( "H3m8;Q1A8Tt0000000000000000", 2u, "TRIO@@@@@@@@@@@@@@@@" )]   // ais.kystverket.no
    public void PartA_VesselName_is_decoded( string payload, uint padding, string expected )
    {
        var parser = new NmeaAisStaticDataReportParserPartA( Encoding.ASCII.GetBytes( payload ), padding );
        parser.VesselName.ShouldBeAisString( expected, 20 );
    }

    // Part A: spare, 8 bits (160-167). The truncated 160 bits form (27 characters, padding 2) has no spare: 0 is returned.
    // No real examples available because all the Part A examples we have use the
    // non-standard but apparently ubiquitous truncated 160-bit message form.
    [TestCase( "H00000000000000000000000000", 2u, 0u )]
    [TestCase( "H000000000000000000000000000", 0u, 0u )]
    [TestCase( "H000000000000000000000000001", 0u, 1u )]
    [TestCase( "H00000000000000000000000000w", 0u, 63u )]
    [TestCase( "H000000000000000000000000010", 0u, 64u )]
    [TestCase( "H000000000000000000000000020", 0u, 128u )]
    [TestCase( "H00000000000000000000000003w", 0u, 255u )]
    public void PartA_Spare_is_decoded( string payload, uint padding, uint expected )
    {
        var parser = new NmeaAisStaticDataReportParserPartA( Encoding.ASCII.GetBytes( payload ), padding );
        parser.Spare160.ShouldBe( expected );
    }

    // Part B: ship type, 8 bits (40-47). 0 means "not available".
    [TestCase( "H000004000000000000000000000", 0u, ShipType.NotAvailable )]
    [TestCase( "H000004E00000000000000000000", 0u, ShipType.WingInGroundHazardousCategoryX )]
    [TestCase( "H3m<KD4NC=D5l@<<:F;000204240", 0u, ShipType.Fishing )]   // ais.kystverket.no
    [TestCase( "H3n0Vd561=30000<7qploP1@4430", 0u, ShipType.CargoAll )]   // ais.kystverket.no
    [TestCase( "H3uG2nTUCBD5l0Q00000001@4210", 0u, ShipType.PleasureMotorCraft )]   // ais.kystverket.no
    public void PartB_ShipType_is_decoded( string payload, uint padding, ShipType expected )
    {
        var parser = new NmeaAisStaticDataReportParserPartB( Encoding.ASCII.GetBytes( payload ), padding );
        parser.ShipType.ShouldBe( expected );
    }

    // Part B: vendor id as defined by ITU-R M.1371-3, 7 six-bit characters (bits 48-89), '@' padded.
    // A shorter expected value is padded with spaces (the '!' row has spaces after the first character).
    [TestCase( "H000004000000000000000000000", 0u, "@@@@@@@" )]
    [TestCase( "H0000040Q0000000000000000000", 0u, "!@@@@@@" )]
    [TestCase( "H0000040QPPPPPP0000000000000", 0u, "!" )]
    [TestCase( "H000004012345670000000000000", 0u, "ABCDEFG" )]
    [TestCase( "H3m<KD4NC=D5l@<<:F;000204240", 0u, "SMTE4PL" )]   // ais.kystverket.no
    [TestCase( "H3n0Vd561=30000<7qploP1@4430", 0u, "AMC@@@@" )]   // ais.kystverket.no
    public void PartB_VendorIdRev3_is_decoded( string payload, uint padding, string expected )
    {
        var parser = new NmeaAisStaticDataReportParserPartB( Encoding.ASCII.GetBytes( payload ), padding );
        parser.VendorIdRev3.ShouldBeAisString( expected, 7 );
    }

    // Part B: vendor id as defined by ITU-R M.1371-4, 3 six-bit characters (bits 48-65), followed by
    // the unit model code and the serial number.
    [TestCase( "H000004000000000000000000000", 0u, "@@@" )]
    [TestCase( "H0000040Q0000000000000000000", 0u, "!@@" )]
    [TestCase( "H0000040QPP00000000000000000", 0u, "!" )]
    [TestCase( "H000004012300000000000000000", 0u, "ABC" )]
    [TestCase( "H3m<KD4NC=D5l@<<:F;000204240", 0u, "SMT" )]   // ais.kystverket.no
    [TestCase( "H3n0Vd561=30000<7qploP1@4430", 0u, "AMC" )]   // ais.kystverket.no
    public void PartB_VendorIdRev4_is_decoded( string payload, uint padding, string expected )
    {
        var parser = new NmeaAisStaticDataReportParserPartB( Encoding.ASCII.GetBytes( payload ), padding );
        parser.VendorIdRev4.ShouldBeAisString( expected, 3 );
    }

    // Part B: unit model code (ITU-R M.1371-4), 4 bits (66-69), from 0 to 15.
    [TestCase( "H000004000000000000000000000", 0u, 0u )]
    [TestCase( "H000004000040000000000000000", 0u, 1u )]
    [TestCase( "H000004000080000000000000000", 0u, 2u )]
    [TestCase( "H0000040000<0000000000000000", 0u, 3u )]
    [TestCase( "H0000040000P0000000000000000", 0u, 8u )]
    [TestCase( "H0000040000t0000000000000000", 0u, 15u )]
    public void PartB_UnitModelCode_is_decoded( string payload, uint padding, uint expected )
    {
        var parser = new NmeaAisStaticDataReportParserPartB( Encoding.ASCII.GetBytes( payload ), padding );
        parser.UnitModelCode.ShouldBe( expected );
    }

    // Part B: serial number (ITU-R M.1371-4), 20 bits (70-89), from 0 to 1048575.
    [TestCase( "H000004000000000000000000000", 0u, 0u )]
    [TestCase( "H000004000020000000000000000", 0u, 524288u )]
    [TestCase( "H000004000000010000000000000", 0u, 1u )]
    [TestCase( "H000004000000020000000000000", 0u, 2u )]
    [TestCase( "H00000400003www0000000000000", 0u, 1048575u )]
    public void PartB_SerialNumber_is_decoded( string payload, uint padding, uint expected )
    {
        var parser = new NmeaAisStaticDataReportParserPartB( Encoding.ASCII.GetBytes( payload ), padding );
        parser.SerialNumber.ShouldBe( expected );
    }

    // Part B: call sign, 7 six-bit characters (bits 90-131), '@' padded; real transponders may pad with spaces.
    [TestCase( "H000004000000000000000000000", 0u, "@@@@@@@" )]
    [TestCase( "H00000400000000Q000000000000", 0u, "!@@@@@@" )]
    [TestCase( "H00000400000000QPPPPPP000000", 0u, "!" )]
    [TestCase( "H000004000000001234567000000", 0u, "ABCDEFG" )]
    [TestCase( "H3m<KD4NC=D5l@<<:F;000204240", 0u, "LJVK@@@" )]   // ais.kystverket.no
    [TestCase( "H3n0Vd561=30000<7qploP1@4430", 0u, "LG9847" )]   // ais.kystverket.no
    public void PartB_CallSign_is_decoded( string payload, uint padding, string expected )
    {
        var parser = new NmeaAisStaticDataReportParserPartB( Encoding.ASCII.GetBytes( payload ), padding );
        parser.CallSign.ShouldBeAisString( expected, 7 );
    }

    // Part B: dimension to bow, 9 bits (132-140), in metres, from 0 (not available) to 511.
    [TestCase( "H000004000000000000000000000", 0u, 0u )]
    [TestCase( "H000004000000000000000080000", 0u, 1u )]
    [TestCase( "H000004000000000000000wp0000", 0u, 511u )]
    [TestCase( "H3m<KD4NC=D5l@<<:F;000204240", 0u, 16u )]   // ais.kystverket.no
    [TestCase( "H3n0Vd561=30000<7qploP1@4430", 0u, 10u )]   // ais.kystverket.no
    public void PartB_DimensionToBow_is_decoded( string payload, uint padding, uint expected )
    {
        var parser = new NmeaAisStaticDataReportParserPartB( Encoding.ASCII.GetBytes( payload ), padding );
        parser.DimensionToBow.ShouldBe( expected );
    }

    // Part B: dimension to stern, 9 bits (141-149), in metres, from 0 (not available) to 511.
    [TestCase( "H000004000000000000000000000", 0u, 0u )]
    [TestCase( "H000004000000000000000001000", 0u, 1u )]
    [TestCase( "H00000400000000000000007w000", 0u, 511u )]
    [TestCase( "H3m<KD4NC=D5l@<<:F;000204240", 0u, 4u )]   // ais.kystverket.no
    [TestCase( "H3n0Vd561=30000<7qploP1@4430", 0u, 4u )]   // ais.kystverket.no
    public void PartB_DimensionToStern_is_decoded( string payload, uint padding, uint expected )
    {
        var parser = new NmeaAisStaticDataReportParserPartB( Encoding.ASCII.GetBytes( payload ), padding );
        parser.DimensionToStern.ShouldBe( expected );
    }

    // Part B: dimension to port, 6 bits (150-155), in metres, from 0 (not available) to 63.
    [TestCase( "H000004000000000000000000000", 0u, 0u )]
    [TestCase( "H000004000000000000000000100", 0u, 1u )]
    [TestCase( "H000004000000000000000000w00", 0u, 63u )]
    [TestCase( "H3m<KD4NC=D5l@<<:F;000204240", 0u, 2u )]   // ais.kystverket.no
    [TestCase( "H3n0Vd561=30000<7qploP1@4430", 0u, 4u )]   // ais.kystverket.no
    public void PartB_DimensionToPort_is_decoded( string payload, uint padding, uint expected )
    {
        var parser = new NmeaAisStaticDataReportParserPartB( Encoding.ASCII.GetBytes( payload ), padding );
        parser.DimensionToPort.ShouldBe( expected );
    }

    // Part B: dimension to starboard, 6 bits (156-161), in metres, from 0 (not available) to 63.
    [TestCase( "H000004000000000000000000000", 0u, 0u )]
    [TestCase( "H000004000000000000000000010", 0u, 1u )]
    [TestCase( "H0000040000000000000000000w0", 0u, 63u )]
    [TestCase( "H3m<KD4NC=D5l@<<:F;000204240", 0u, 4u )]   // ais.kystverket.no
    [TestCase( "H3n0Vd561=30000<7qploP1@4430", 0u, 3u )]   // ais.kystverket.no
    public void PartB_DimensionToStarboard_is_decoded( string payload, uint padding, uint expected )
    {
        var parser = new NmeaAisStaticDataReportParserPartB( Encoding.ASCII.GetBytes( payload ), padding );
        parser.DimensionToStarboard.ShouldBe( expected );
    }

    // Part B: mothership MMSI, 30 bits (132-161). Auxiliary craft use these bits instead of the dimensions,
    // so the same payloads decode as both (e.g. starboard 1 is mothership MMSI 1).
    [TestCase( "H000004000000000000000000000", 0u, 0u )]
    [TestCase( "H000004000000000000000000010", 0u, 1u )]
    [TestCase( "H000004000000000000000roVRi0", 0u, 987654321u )]
    public void PartB_MothershipMmsi_is_decoded( string payload, uint padding, uint expected )
    {
        var parser = new NmeaAisStaticDataReportParserPartB( Encoding.ASCII.GetBytes( payload ), padding );
        parser.MothershipMmsi.ShouldBe( expected );
    }

    // Part B: type of electronic position fixing device, 4 bits (162-165).
    // Both rows expect Undefined (0): the second one sets the lowest spare bit (167) and checks that it does not leak
    // into the field. There is no row with a non-zero fix type (kept as in the SpecFlow source).
    [TestCase( "H000004000000000000000000000", 0u, (EpfdFixType)0 )]
    [TestCase( "H000004000000000000000000001", 0u, (EpfdFixType)0 )]
    public void PartB_EpfdFixType_is_decoded( string payload, uint padding, EpfdFixType expected )
    {
        var parser = new NmeaAisStaticDataReportParserPartB( Encoding.ASCII.GetBytes( payload ), padding );
        parser.EpfdFixType.ShouldBe( expected );
    }

    // Part B: spare, 2 bits (166-167).
    [TestCase( "H000004000000000000000000000", 0u, 0u )]
    [TestCase( "H000004000000000000000000001", 0u, 1u )]
    public void PartB_Spare_is_decoded( string payload, uint padding, uint expected )
    {
        var parser = new NmeaAisStaticDataReportParserPartB( Encoding.ASCII.GetBytes( payload ), padding );
        parser.Spare162.ShouldBe( expected );
    }
}
