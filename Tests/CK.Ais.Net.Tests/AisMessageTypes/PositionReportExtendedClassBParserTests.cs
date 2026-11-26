// <copyright file="PositionReportExtendedClassBParserTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using NUnit.Framework;
using System.Text;

namespace Ais.Net.Tests.AisMessageTypes;

// NmeaAisPositionReportExtendedClassBParser decodes message type 19 (Extended Class B CS Position Report):
// 312 bits, every field is checked with synthetic payloads where only the field under test is set.
[TestFixture]
public class PositionReportExtendedClassBParserTests
{
    // Message type: bits 0-5.
    [Test]
    public void MessageType_is_decoded()
    {
        var parser = new NmeaAisPositionReportExtendedClassBParser( Encoding.ASCII.GetBytes( "C>eq`d@000000000000000000000000000000000000000000000" ), 0 );
        parser.MessageType.ShouldBe( MessageType.ExtendedClassBEquipmentPositionReport );
    }

    // Repeat indicator: bits 6-7 (0 to 3).
    [TestCase( "C000000000000000000000000000000000000000000000000000", 0u, 0u )]
    [TestCase( "C@00000000000000000000000000000000000000000000000000", 0u, 1u )]
    [TestCase( "CP00000000000000000000000000000000000000000000000000", 0u, 2u )]
    [TestCase( "Ch00000000000000000000000000000000000000000000000000", 0u, 3u )]
    public void RepeatIndicator_is_decoded( string payload, uint padding, uint repeatCount )
    {
        var parser = new NmeaAisPositionReportExtendedClassBParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.RepeatIndicator.ShouldBe( repeatCount );
    }

    // MMSI: bits 8-37 (30 bits unsigned).
    [TestCase( "C000000000000000000000000000000000000000000000000000", 0u, 0u )]
    [TestCase( "C00000@000000000000000000000000000000000000000000000", 0u, 1u )]
    [TestCase( "C00000P000000000000000000000000000000000000000000000", 0u, 2u )]
    [TestCase( "C>eq`d@000000000000000000000000000000000000000000000", 0u, 987654321u )]
    public void Mmsi_is_decoded( string payload, uint padding, uint mmsi )
    {
        var parser = new NmeaAisPositionReportExtendedClassBParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.Mmsi.ShouldBe( mmsi );
    }

    // Spare bits: bits 38-45 (8 bits, "regional reserved").
    [TestCase( "C000000000000000000000000000000000000000000000000000", 0u, 0u )]
    [TestCase( "C000000400000000000000000000000000000000000000000000", 0u, 1u )]
    [TestCase( "C000001000000000000000000000000000000000000000000000", 0u, 16u )]
    [TestCase( "C00000?t00000000000000000000000000000000000000000000", 0u, 255u )]
    public void SpareBits38_are_decoded( string payload, uint padding, uint spare )
    {
        var parser = new NmeaAisPositionReportExtendedClassBParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.SpareBits38.ShouldBe( spare );
    }

    // Speed over ground: bits 46-55 in 1/10 knot (1023 means "not available").
    [TestCase( "C000000000000000000000000000000000000000000000000000", 0u, 0u )]
    [TestCase( "C00000000@000000000000000000000000000000000000000000", 0u, 1u )]
    [TestCase( "C0000003wh000000000000000000000000000000000000000000", 0u, 1023u )]
    public void SpeedOverGround_is_decoded( string payload, uint padding, uint speedOverGround )
    {
        var parser = new NmeaAisPositionReportExtendedClassBParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.SpeedOverGroundTenths.ShouldBe( speedOverGround );
    }

    // Position accuracy: bit 56.
    [TestCase( "C000000000000000000000000000000000000000000000000000", 0u, false )]
    [TestCase( "C000000008000000000000000000000000000000000000000000", 0u, true )]
    public void PositionAccuracy_is_decoded( string payload, uint padding, bool positionAccuracy )
    {
        var parser = new NmeaAisPositionReportExtendedClassBParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.PositionAccuracy.ShouldBe( positionAccuracy );
    }

    // Longitude (bits 57-84, 28 bits signed) and latitude (bits 85-111, 27 bits signed) in 1/10000 minute.
    [TestCase( "C000000000000000000000000000000000000000000000000000", 0u, 0, 0 )]
    [TestCase( "C0000000000000P0000000000000000000000000000000000000", 0u, 1, 0 )]
    [TestCase( "C000000000000000004000000000000000000000000000000000", 0u, 0, 1 )]
    [TestCase( "C000000007wwwwP0000000000000000000000000000000000000", 0u, -1, 0 )]
    [TestCase( "C0000000000000Owwwt000000000000000000000000000000000", 0u, 0, -1 )]
    public void Longitude_and_latitude_are_decoded( string payload, uint padding, int longitude, int latitude )
    {
        var parser = new NmeaAisPositionReportExtendedClassBParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.Longitude10000thMins.ShouldBe( longitude );
        parser.Latitude10000thMins.ShouldBe( latitude );
    }

    // Course over ground: bits 112-123 in 1/10 degree (3600 means "not available").
    [TestCase( "C000000000000000000000000000000000000000000000000000", 0u, 0u )]
    [TestCase( "C000000000000000000040000000000000000000000000000000", 0u, 1u )]
    [TestCase( "C000000000000000003Pt0000000000000000000000000000000", 0u, 3599u )]
    [TestCase( "C000000000000000003Q00000000000000000000000000000000", 0u, 3600u )]
    public void CourseOverGround_is_decoded( string payload, uint padding, uint courseOverGround )
    {
        var parser = new NmeaAisPositionReportExtendedClassBParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.CourseOverGround10thDegrees.ShouldBe( courseOverGround );
    }

    // True heading: bits 124-132 in degrees (511 means "not available").
    [TestCase( "C000000000000000000000000000000000000000000000000000", 0u, 0u )]
    [TestCase( "C000000000000000000000P00000000000000000000000000000", 0u, 1u )]
    [TestCase( "C00000000000000000003wP00000000000000000000000000000", 0u, 511u )]
    public void TrueHeading_is_decoded( string payload, uint padding, uint trueHeading )
    {
        var parser = new NmeaAisPositionReportExtendedClassBParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.TrueHeadingDegrees.ShouldBe( trueHeading );
    }

    // Time stamp: bits 133-138, UTC second.
    [TestCase( "C000000000000000000000000000000000000000000000000000", 0u, 0u )]
    [TestCase( "C0000000000000000000000P0000000000000000000000000000", 0u, 1u )]
    [TestCase( "C000000000000000000000MP0000000000000000000000000000", 0u, 59u )]
    public void TimeStamp_is_decoded( string payload, uint padding, uint timeStamp )
    {
        var parser = new NmeaAisPositionReportExtendedClassBParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.TimeStampSecond.ShouldBe( timeStamp );
    }

    // Spare bits: bits 139-142 (4 bits, "regional reserved").
    [TestCase( "C000000000000000000000000000000000000000000000000000", 0u, 0u )]
    [TestCase( "C000000000000000000000020000000000000000000000000000", 0u, 1u )]
    [TestCase( "C000000000000000000000040000000000000000000000000000", 0u, 2u )]
    [TestCase( "C000000000000000000000080000000000000000000000000000", 0u, 4u )]
    [TestCase( "C0000000000000000000000@0000000000000000000000000000", 0u, 8u )]
    [TestCase( "C0000000000000000000000H0000000000000000000000000000", 0u, 12u )]
    [TestCase( "C0000000000000000000000N0000000000000000000000000000", 0u, 15u )]
    public void SpareBits139_are_decoded( string payload, uint padding, uint spare )
    {
        var parser = new NmeaAisPositionReportExtendedClassBParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.SpareBits139.ShouldBe( spare );
    }

    // Ship name: bits 143-262, 20 six-bit characters. The payloads walk through the whole 6-bit
    // character table; unused characters are the '@' padding (written explicitly in the expectations).
    [TestCase( "C000000000000000000000000000000000000000000000000000", 0u, "@@@@@@@@@@@@@@@@@@@@" )]
    [TestCase( "C000000000000000000000002000000000000000000000000000", 0u, "A@@@@@@@@@@@@@@@@@@@" )]
    [TestCase( "C000000000000000000000002468:<>@BDFHJLNPRTV`00000000", 0u, "ABCDEFGHIJKLMNOPQRST" )]
    [TestCase( "C00000000000000000000000bdfhjlnprtw13579;=?A00000000", 0u, "UVWXYZ[\\]^_ !\"#$%&'(" )]
    [TestCase( "C00000000000000000000001CEGIKMOQSUWacegikmoq00000000", 0u, ")*+,-./0123456789:;<" )]
    [TestCase( "C00000000000000000000001suv0000000000000000000000000", 0u, "=>?@@@@@@@@@@@@@@@@@" )]
    public void ShipName_is_decoded( string payload, uint padding, string name )
    {
        var parser = new NmeaAisPositionReportExtendedClassBParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.ShipName.ShouldBeAisString( name, 20 );
    }

    // Ship type: bits 263-270.
    [TestCase( "C000000000000000000000000000000000000000000000000000", 0u, ShipType.NotAvailable )]
    [TestCase( "C0000000000000000000000000000000000000000000:P000000", 0u, ShipType.WingInGroundHazardousCategoryX )]
    public void ShipType_is_decoded( string payload, uint padding, ShipType shipType )
    {
        var parser = new NmeaAisPositionReportExtendedClassBParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.ShipType.ShouldBe( shipType );
    }

    // Dimension to bow: bits 271-279 in meters.
    [TestCase( "C000000000000000000000000000000000000000000000000000", 0u, 0u )]
    [TestCase( "C000000000000000000000000000000000000000000000400000", 0u, 1u )]
    [TestCase( "C00000000000000000000000000000000000000000000Ot00000", 0u, 511u )]
    public void DimensionToBow_is_decoded( string payload, uint padding, uint size )
    {
        var parser = new NmeaAisPositionReportExtendedClassBParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.DimensionToBow.ShouldBe( size );
    }

    // Dimension to stern: bits 280-288 in meters.
    [TestCase( "C000000000000000000000000000000000000000000000000000", 0u, 0u )]
    [TestCase( "C00000000000000000000000000000000000000000000000P000", 0u, 1u )]
    [TestCase( "C0000000000000000000000000000000000000000000003wP000", 0u, 511u )]
    public void DimensionToStern_is_decoded( string payload, uint padding, uint size )
    {
        var parser = new NmeaAisPositionReportExtendedClassBParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.DimensionToStern.ShouldBe( size );
    }

    // Dimension to port: bits 289-294 in meters.
    [TestCase( "C000000000000000000000000000000000000000000000000000", 0u, 0u )]
    [TestCase( "C000000000000000000000000000000000000000000000000P00", 0u, 1u )]
    [TestCase( "C00000000000000000000000000000000000000000000000OP00", 0u, 63u )]
    public void DimensionToPort_is_decoded( string payload, uint padding, uint size )
    {
        var parser = new NmeaAisPositionReportExtendedClassBParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.DimensionToPort.ShouldBe( size );
    }

    // Dimension to starboard: bits 295-300 in meters.
    [TestCase( "C000000000000000000000000000000000000000000000000000", 0u, 0u )]
    [TestCase( "C0000000000000000000000000000000000000000000000000P0", 0u, 1u )]
    [TestCase( "C000000000000000000000000000000000000000000000000OP0", 0u, 63u )]
    public void DimensionToStarboard_is_decoded( string payload, uint padding, uint size )
    {
        var parser = new NmeaAisPositionReportExtendedClassBParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.DimensionToStarboard.ShouldBe( size );
    }

    // Position fix type (EPFD): bits 301-304.
    [TestCase( "C000000000000000000000000000000000000000000000000000", 0u, EpfdFixType.Undefined )]
    [TestCase( "C000000000000000000000000000000000000000000000000020", 0u, EpfdFixType.Gps )]
    [TestCase( "C000000000000000000000000000000000000000000000000040", 0u, EpfdFixType.Glonass )]
    [TestCase( "C000000000000000000000000000000000000000000000000060", 0u, EpfdFixType.CombinedGpsGlonass )]
    [TestCase( "C000000000000000000000000000000000000000000000000080", 0u, EpfdFixType.LoranC )]
    [TestCase( "C0000000000000000000000000000000000000000000000000:0", 0u, EpfdFixType.Chayka )]
    [TestCase( "C0000000000000000000000000000000000000000000000000<0", 0u, EpfdFixType.IntegratedNavigationSystem )]
    [TestCase( "C0000000000000000000000000000000000000000000000000>0", 0u, EpfdFixType.Surveyed )]
    [TestCase( "C0000000000000000000000000000000000000000000000000@0", 0u, EpfdFixType.Galileo )]
    public void PositionFixType_is_decoded( string payload, uint padding, EpfdFixType epfd )
    {
        var parser = new NmeaAisPositionReportExtendedClassBParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.PositionFixType.ShouldBe( epfd );
    }

    // RAIM flag: bit 305.
    [TestCase( "C000000000000000000000000000000000000000000000000000", 0u, false )]
    [TestCase( "C000000000000000000000000000000000000000000000000010", 0u, true )]
    public void RaimFlag_is_decoded( string payload, uint padding, bool raim )
    {
        var parser = new NmeaAisPositionReportExtendedClassBParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.RaimFlag.ShouldBe( raim );
    }

    // DTE flag: bit 306 (true means the data terminal is NOT ready).
    [TestCase( "C000000000000000000000000000000000000000000000000000", 0u, false )]
    [TestCase( "C00000000000000000000000000000000000000000000000000P", 0u, true )]
    public void IsDteNotReady_is_decoded( string payload, uint padding, bool notReady )
    {
        var parser = new NmeaAisPositionReportExtendedClassBParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.IsDteNotReady.ShouldBe( notReady );
    }

    // Assigned mode flag: bit 307.
    [TestCase( "C000000000000000000000000000000000000000000000000000", 0u, false )]
    [TestCase( "C00000000000000000000000000000000000000000000000000@", 0u, true )]
    public void IsAssigned_is_decoded( string payload, uint padding, bool isAssigned )
    {
        var parser = new NmeaAisPositionReportExtendedClassBParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.IsAssigned.ShouldBe( isAssigned );
    }

    // Spare bits: bits 308-311.
    [TestCase( "C000000000000000000000000000000000000000000000000000", 0u, 0u )]
    [TestCase( "C000000000000000000000000000000000000000000000000001", 0u, 1u )]
    [TestCase( "C000000000000000000000000000000000000000000000000002", 0u, 2u )]
    [TestCase( "C000000000000000000000000000000000000000000000000007", 0u, 7u )]
    [TestCase( "C000000000000000000000000000000000000000000000000008", 0u, 8u )]
    [TestCase( "C00000000000000000000000000000000000000000000000000>", 0u, 14u )]
    [TestCase( "C00000000000000000000000000000000000000000000000000?", 0u, 15u )]
    public void SpareBits308_are_decoded( string payload, uint padding, uint spare )
    {
        var parser = new NmeaAisPositionReportExtendedClassBParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.SpareBits308.ShouldBe( spare );
    }
}
