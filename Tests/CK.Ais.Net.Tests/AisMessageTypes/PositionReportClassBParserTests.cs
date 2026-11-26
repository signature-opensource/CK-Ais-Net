// <copyright file="PositionReportClassBParserTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>
//
// Contains data under the Norwegian licence for Open Government data (NLOD) distributed by
// the Norwegian Costal Administration - https://ais.kystverket.no/
// The license can be found at https://data.norge.no/nlod/en/2.0
// The lines in this file that contain data from this source are annotated with a comment containing "ais.kystverket.no"
// The NLOD applies only to the data in these annotated lines. The license under which you are using this software
// (either the AGPLv3, or a commercial license) applies to the whole file.

using NUnit.Framework;
using Shouldly;
using System.Text;

namespace Ais.Net.Tests.AisMessageTypes;

// NmeaAisPositionReportClassBParser decodes message type 18 (Standard Class B CS Position Report):
// 168 bits, every field is checked with synthetic payloads (only the field under test is set) and real ones.
[TestFixture]
public class PositionReportClassBParserTests
{
    // Message type: bits 0-5.
    [Test]
    public void MessageType_is_decoded()
    {
        var parser = new NmeaAisPositionReportClassBParser( Encoding.ASCII.GetBytes( "B3mR5u000HFQD;av`1arKwt5oP06" ), 0 );
        parser.MessageType.ShouldBe( MessageType.StandardClassBEquipmentPositionReport );
    }

    // Repeat indicator: bits 6-7 (0 to 3).
    [TestCase( "B000000000000000000000000000", 0u, 0u )]
    [TestCase( "B@00000000000000000000000000", 0u, 1u )]
    [TestCase( "BP00000000000000000000000000", 0u, 2u )]
    [TestCase( "Bh00000000000000000000000000", 0u, 3u )]
    [TestCase( "B3m?;p0008<q:naAtLh03wTUoP06", 0u, 0u )]   // ais.kystverket.no
    [TestCase( "BCm?;p0008<q:naAtLh03wTUoP06", 0u, 1u )]   // ais.kystverket.no
    [TestCase( "BSm?;p0008<q:naAtLh03wTUoP06", 0u, 2u )]   // ais.kystverket.no
    [TestCase( "Bkm?;p0008<q:naAtLh03wTUoP06", 0u, 3u )]   // ais.kystverket.no
    public void RepeatIndicator_is_decoded( string payload, uint padding, uint repeatCount )
    {
        var parser = new NmeaAisPositionReportClassBParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.RepeatIndicator.ShouldBe( repeatCount );
    }

    // MMSI: bits 8-37 (30 bits unsigned).
    [TestCase( "B000000000000000000000000000", 0u, 0u )]
    [TestCase( "B00000@000000000000000000000", 0u, 1u )]
    [TestCase( "B00000P000000000000000000000", 0u, 2u )]
    [TestCase( "B>eq`d@000000000000000000000", 0u, 987654321u )]   // ais.kystverket.no
    [TestCase( "B3q35T005h<0h@`Dd:i;gwRUoP06", 0u, 261146000u )]   // ais.kystverket.no
    [TestCase( "B3m?;p0008<q:naAtLh03wTUoP06", 0u, 257149920u )]   // ais.kystverket.no
    public void Mmsi_is_decoded( string payload, uint padding, uint mmsi )
    {
        var parser = new NmeaAisPositionReportClassBParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.Mmsi.ShouldBe( mmsi );
    }

    // Spare bits: bits 38-45 (8 bits, "regional reserved").
    // These cases were NOT executed by the SpecFlow suite: the step was spelled "SpareBits38 is" whereas
    // the binding expected "SPareBits38 is", so SpecFlow reported them as NotExecuted.
    [TestCase( "B000000000000000000000000000", 0u, 0u )]
    [TestCase( "B000000400000000000000000000", 0u, 1u )]
    [TestCase( "B000001000000000000000000000", 0u, 16u )]
    [TestCase( "B00000?t00000000000000000000", 0u, 255u )]
    [TestCase( "B3m?;p0008<q:naAtLh03wTUoP06", 0u, 0u )]   // ais.kystverket.no
    public void SpareBits38_are_decoded( string payload, uint padding, uint spare )
    {
        var parser = new NmeaAisPositionReportClassBParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.SpareBits38.ShouldBe( spare );
    }

    // Speed over ground: bits 46-55 in 1/10 knot (1023 means "not available").
    [TestCase( "B000000000000000000000000000", 0u, 0u )]
    [TestCase( "B00000000@000000000000000000", 0u, 1u )]
    [TestCase( "B0000003wh000000000000000000", 0u, 1023u )]
    [TestCase( "B3m?;p0008<q:naAtLh03wTUoP06", 0u, 0u )]   // ais.kystverket.no
    [TestCase( "B3q35T005h<0h@`Dd:i;gwRUoP06", 0u, 23u )]   // ais.kystverket.no
    public void SpeedOverGround_is_decoded( string payload, uint padding, uint speedOverGround )
    {
        var parser = new NmeaAisPositionReportClassBParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.SpeedOverGroundTenths.ShouldBe( speedOverGround );
    }

    // Position accuracy: bit 56.
    [TestCase( "B000000000000000000000000000", 0u, false )]
    [TestCase( "B000000008000000000000000000", 0u, true )]
    [TestCase( "B3q35T005h<0h@`Dd:i;gwRUoP06", 0u, false )]   // ais.kystverket.no
    [TestCase( "B3m?;p0008<q:naAtLh03wTUoP06", 0u, true )]   // ais.kystverket.no
    public void PositionAccuracy_is_decoded( string payload, uint padding, bool positionAccuracy )
    {
        var parser = new NmeaAisPositionReportClassBParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.PositionAccuracy.ShouldBe( positionAccuracy );
    }

    // Longitude (bits 57-84, 28 bits signed) and latitude (bits 85-111, 27 bits signed) in 1/10000 minute.
    [TestCase( "B000000000000000000000000000", 0u, 0, 0 )]
    [TestCase( "B0000000000000P0000000000000", 0u, 1, 0 )]
    [TestCase( "B000000000000000004000000000", 0u, 0, 1 )]
    [TestCase( "B000000007wwwwP0000000000000", 0u, -1, 0 )]
    [TestCase( "B0000000000000Owwwt000000000", 0u, 0, -1 )]
    [TestCase( "B3m6D@P0005r0R``WaiC;wgUkP06", 0u, 3096645, 36216476 )]   // ais.kystverket.no
    public void Longitude_and_latitude_are_decoded( string payload, uint padding, int longitude, int latitude )
    {
        var parser = new NmeaAisPositionReportClassBParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.Longitude10000thMins.ShouldBe( longitude );
        parser.Latitude10000thMins.ShouldBe( latitude );
    }

    // Course over ground: bits 112-123 in 1/10 degree (3600 means "not available").
    [TestCase( "B000000000000000000000000000", 0u, 0u )]
    [TestCase( "B000000000000000000040000000", 0u, 1u )]
    [TestCase( "B000000000000000003Pt0000000", 0u, 3599u )]
    [TestCase( "B000000000000000003Q00000000", 0u, 3600u )]
    [TestCase( "B3m?;p0008<q:naAtLh03wTUoP06", 0u, 0u )]   // ais.kystverket.no
    [TestCase( "B3q35T005h<0h@`Dd:i;gwRUoP06", 0u, 1211u )]   // ais.kystverket.no
    [TestCase( "B3m6D@P0005r0R``WaiC;wgUkP06", 0u, 1330u )]   // ais.kystverket.no
    [TestCase( "B3mnUc0000@LTtaWw7CQ3wVTkP06", 0u, 3600u )]   // ais.kystverket.no
    public void CourseOverGround_is_decoded( string payload, uint padding, uint courseOverGround )
    {
        var parser = new NmeaAisPositionReportClassBParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.CourseOverGround10thDegrees.ShouldBe( courseOverGround );
    }

    // True heading: bits 124-132 in degrees (511 means "not available").
    [TestCase( "B000000000000000000000000000", 0u, 0u )]
    [TestCase( "B000000000000000000000P00000", 0u, 1u )]
    [TestCase( "B00000000000000000003wP00000", 0u, 511u )]
    [TestCase( "B3m:<b0008<B<s8TO0SQ2j11nDeJ", 0u, 356u )]   // ais.kystverket.no
    [TestCase( "B3q35T005h<0h@`Dd:i;gwRUoP06", 0u, 511u )]   // ais.kystverket.no
    public void TrueHeading_is_decoded( string payload, uint padding, uint trueHeading )
    {
        var parser = new NmeaAisPositionReportClassBParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.TrueHeadingDegrees.ShouldBe( trueHeading );
    }

    // Time stamp: bits 133-138, UTC second.
    [TestCase( "B000000000000000000000000000", 0u, 0u )]
    [TestCase( "B0000000000000000000000P0000", 0u, 1u )]
    [TestCase( "B000000000000000000000MP0000", 0u, 59u )]
    [TestCase( "B3m:<b0008<B<s8TO0SQ2j11nDeJ", 0u, 2u )]   // ais.kystverket.no
    [TestCase( "B3m?;p0008<q:naAtLh03wTUoP06", 0u, 9u )]   // ais.kystverket.no
    [TestCase( "B3mnUc0000@LTtaWw7CQ3wVTkP06", 0u, 13u )]   // ais.kystverket.no
    [TestCase( "B3m6D@P0005r0R``WaiC;wgUkP06", 0u, 31u )]   // ais.kystverket.no
    public void TimeStamp_is_decoded( string payload, uint padding, uint timeStamp )
    {
        var parser = new NmeaAisPositionReportClassBParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.TimeStampSecond.ShouldBe( timeStamp );
    }

    // Spare bits: bits 139-140 (2 bits, "regional reserved").
    // These cases were NOT executed by the SpecFlow suite: the step was spelled "SpareBits139is" (missing space)
    // whereas the binding expected "SpareBits139 is", so SpecFlow reported them as NotExecuted.
    [TestCase( "B000000000000000000000000000", 0u, 0u )]
    [TestCase( "B000000000000000000000080000", 0u, 1u )]
    [TestCase( "B0000000000000000000000@0000", 0u, 2u )]
    [TestCase( "B0000000000000000000000H0000", 0u, 3u )]
    [TestCase( "B3q35T005h<0h@`Dd:i;gwRUoP06", 0u, 0u )]   // ais.kystverket.no
    public void SpareBits139_are_decoded( string payload, uint padding, uint spare )
    {
        var parser = new NmeaAisPositionReportClassBParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.SpareBits139.ShouldBe( spare );
    }

    // CS unit flag: bit 141 (0: SOTDMA Class B+, 1: CSTDMA Class B).
    [TestCase( "B000000000000000000000000000", 0u, ClassBUnit.Sotdma )]
    [TestCase( "B000000000000000000000040000", 0u, ClassBUnit.Cstdma )]
    [TestCase( "B3m:<b0008<B<s8TO0SQ2j11nDeJ", 0u, ClassBUnit.Sotdma )]   // ais.kystverket.no
    [TestCase( "B3mnUc0000@LTtaWw7CQ3wVTkP06", 0u, ClassBUnit.Cstdma )]   // ais.kystverket.no
    public void CsUnit_is_decoded( string payload, uint padding, ClassBUnit unit )
    {
        var parser = new NmeaAisPositionReportClassBParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.CsUnit.ShouldBe( unit );
    }

    // Display flag: bit 142.
    [TestCase( "B000000000000000000000000000", 0u, false )]
    [TestCase( "B000000000000000000000020000", 0u, true )]
    [TestCase( "B3q35T005h<0h@`Dd:i;gwRUoP06", 0u, false )]   // ais.kystverket.no
    [TestCase( "B5RVdt0000;I;08?A>MOKws7kP06", 0u, true )]   // ais.kystverket.no
    public void HasDisplay_is_decoded( string payload, uint padding, bool hasDisplay )
    {
        var parser = new NmeaAisPositionReportClassBParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.HasDisplay.ShouldBe( hasDisplay );
    }

    // DSC flag: bit 143.
    [TestCase( "B000000000000000000000000000", 0u, false )]
    [TestCase( "B000000000000000000000010000", 0u, true )]
    [TestCase( "B3mnUc0000@LTtaWw7CQ3wVTkP06", 0u, false )]   // ais.kystverket.no
    [TestCase( "B3q35T005h<0h@`Dd:i;gwRUoP06", 0u, true )]   // ais.kystverket.no
    public void IsDscAttached_is_decoded( string payload, uint padding, bool isDscAttached )
    {
        var parser = new NmeaAisPositionReportClassBParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.IsDscAttached.ShouldBe( isDscAttached );
    }

    // Band flag: bit 144.
    [TestCase( "B000000000000000000000000000", 0u, false )]
    [TestCase( "B00000000000000000000000P000", 0u, true )]
    [TestCase( "B3q35T005h<0h@`Dd:i;gwRUoP06", 0u, true )]   // ais.kystverket.no
    public void CanSwitchBands_is_decoded( string payload, uint padding, bool canSwitchBands )
    {
        var parser = new NmeaAisPositionReportClassBParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.CanSwitchBands.ShouldBe( canSwitchBands );
    }

    // Message 22 flag: bit 145.
    [TestCase( "B000000000000000000000000000", 0u, false )]
    [TestCase( "B00000000000000000000000@000", 0u, true )]
    [TestCase( "B3m>so00088pPR91r=JfOwV5WP06", 0u, false )]   // ais.kystverket.no
    [TestCase( "B3q35T005h<0h@`Dd:i;gwRUoP06", 0u, true )]   // ais.kystverket.no
    public void CanAcceptMessage22ChannelAssignment_is_decoded( string payload, uint padding, bool canAcceptMessage22ChannelAssignment )
    {
        var parser = new NmeaAisPositionReportClassBParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.CanAcceptMessage22ChannelAssignment.ShouldBe( canAcceptMessage22ChannelAssignment );
    }

    // Assigned mode flag: bit 146.
    [TestCase( "B000000000000000000000000000", 0u, false )]
    [TestCase( "B000000000000000000000008000", 0u, true )]
    [TestCase( "B3q35T005h<0h@`Dd:i;gwRUoP06", 0u, false )]   // ais.kystverket.no
    public void IsAssigned_is_decoded( string payload, uint padding, bool isAssigned )
    {
        var parser = new NmeaAisPositionReportClassBParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.IsAssigned.ShouldBe( isAssigned );
    }

    // RAIM flag: bit 147.
    [TestCase( "B000000000000000000000000000", 0u, false )]
    [TestCase( "B000000000000000000000004000", 0u, true )]
    [TestCase( "B3m6D@P0005r0R``WaiC;wgUkP06", 0u, false )]   // ais.kystverket.no
    [TestCase( "B3q35T005h<0h@`Dd:i;gwRUoP06", 0u, true )]   // ais.kystverket.no
    public void RaimFlag_is_decoded( string payload, uint padding, bool raim )
    {
        var parser = new NmeaAisPositionReportClassBParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.RaimFlag.ShouldBe( raim );
    }

    // Communication state selector flag: bit 148 (0: SOTDMA, 1: ITDMA).
    [TestCase( "B000000000000000000000000000", 0u, ClassBRadioStatusType.Sotdma )]
    [TestCase( "B000000000000000000000002000", 0u, ClassBRadioStatusType.Itdma )]
    [TestCase( "B3q35T005h<0h@`Dd:i;gwRUoP06", 0u, ClassBRadioStatusType.Itdma )]   // ais.kystverket.no
    public void RadioStatusType_is_decoded( string payload, uint padding, ClassBRadioStatusType radioStatusType )
    {
        var parser = new NmeaAisPositionReportClassBParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.RadioStatusType.ShouldBe( radioStatusType );
    }
}
