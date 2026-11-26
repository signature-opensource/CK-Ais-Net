// <copyright file="StaticAndVoyageRelatedDataParserTests.cs" company="Endjin Limited">
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

// NmeaAisStaticAndVoyageRelatedDataParser decodes the payload of message type 5 (Static and Voyage Related Data):
// a 424 bits message (71 characters with 2 padding bits) sent by class A transponders.
// Each test checks one field, against synthetic payloads (only the bits of the field are set: zero,
// lowest bit, bit patterns, maximum value) and real messages from ais.kystverket.no.
[TestFixture]
public class StaticAndVoyageRelatedDataParserTests
{
    // Message type: bits 0-5.
    [Test]
    public void MessageType_is_StaticAndVoyageRelatedData()
    {
        var parser = new NmeaAisStaticAndVoyageRelatedDataParser( Encoding.ASCII.GetBytes( "53m89J81SR44hdhL0004eH5A84q<00000000001@4P8825LD08j0DTm0A00000000000000" ), 2 );   // ais.kystverket.no
        parser.MessageType.ShouldBe( MessageType.StaticAndVoyageRelatedData );
    }

    // Repeat indicator: 2 bits (6-7), from 0 to 3.
    [TestCase( "50000000000000000000000000000000000000000000000000000000000000000000000", 2u, 0u )]
    [TestCase( "5@000000000000000000000000000000000000000000000000000000000000000000000", 2u, 1u )]
    [TestCase( "5P000000000000000000000000000000000000000000000000000000000000000000000", 2u, 2u )]
    [TestCase( "5h000000000000000000000000000000000000000000000000000000000000000000000", 2u, 3u )]
    [TestCase( "53m89J81SR44hdhL0004eH5A84q<00000000001@4P8825LD08j0DTm0A00000000000000", 2u, 0u )]   // ais.kystverket.no
    [TestCase( "5C@oTQT2Ad5duL9W:21=@EHq>085A@hE:222220l00P745m>0>S3kQiF@DPVAC`88888880", 2u, 1u )]   // ais.kystverket.no
    [TestCase( "5SmkPJ02AmrLh=PV2208t60t@Tr222222222220l2@B785j:0BU4SkQ21BCH88888888880", 2u, 2u )]   // ais.kystverket.no
    public void RepeatIndicator_is_decoded( string payload, uint padding, uint expected )
    {
        var parser = new NmeaAisStaticAndVoyageRelatedDataParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.RepeatIndicator.ShouldBe( expected );
    }

    // MMSI: 30 bits (8-37).
    [TestCase( "50000000000000000000000000000000000000000000000000000000000000000000000", 2u, 0u )]
    [TestCase( "500000@0000000000000000000000000000000000000000000000000000000000000000", 2u, 1u )]
    [TestCase( "500000P0000000000000000000000000000000000000000000000000000000000000000", 2u, 2u )]
    [TestCase( "5>eq`d@0000000000000000000000000000000000000000000000000000000000000000", 2u, 987654321u )]
    [TestCase( "53m89J81SR44hdhL0004eH5A84q<00000000001@4P8825LD08j0DTm0A00000000000000", 2u, 257034600u )]   // ais.kystverket.no
    [TestCase( "5C@oTQT2Ad5duL9W:21=@EHq>085A@hE:222220l00P745m>0>S3kQiF@DPVAC`88888880", 2u, 219014278u )]   // ais.kystverket.no
    [TestCase( "5SmkPJ02AmrLh=PV2208t60t@Tr222222222220l2@B785j:0BU4SkQ21BCH88888888880", 2u, 257745000u )]   // ais.kystverket.no
    public void Mmsi_is_decoded( string payload, uint padding, uint expected )
    {
        var parser = new NmeaAisStaticAndVoyageRelatedDataParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.Mmsi.ShouldBe( expected );
    }

    // AIS version indicator: 2 bits (38-39), from 0 to 3.
    [TestCase( "50000000000000000000000000000000000000000000000000000000000000000000000", 2u, 0u )]
    [TestCase( "50000040000000000000000000000000000000000000000000000000000000000000000", 2u, 1u )]
    [TestCase( "50000080000000000000000000000000000000000000000000000000000000000000000", 2u, 2u )]
    [TestCase( "500000<0000000000000000000000000000000000000000000000000000000000000000", 2u, 3u )]
    [TestCase( "5SmkPJ02AmrLh=PV2208t60t@Tr222222222220l2@B785j:0BU4SkQ21BCH88888888880", 2u, 0u )]   // ais.kystverket.no
    [TestCase( "5C@oTQT2Ad5duL9W:21=@EHq>085A@hE:222220l00P745m>0>S3kQiF@DPVAC`88888880", 2u, 1u )]   // ais.kystverket.no
    [TestCase( "53m89J81SR44hdhL0004eH5A84q<00000000001@4P8825LD08j0DTm0A00000000000000", 2u, 2u )]   // ais.kystverket.no
    public void AisVersion_is_decoded( string payload, uint padding, uint expected )
    {
        var parser = new NmeaAisStaticAndVoyageRelatedDataParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.AisVersion.ShouldBe( expected );
    }

    // IMO number: 30 bits (40-69). 0 means "not available".
    [TestCase( "50000000000000000000000000000000000000000000000000000000000000000000000", 2u, 0u )]
    [TestCase( "50000000000500000000000000000000000000000000000000000000000000000000000", 2u, 1u )]
    [TestCase( "5000003cNJ;500000000000000000000000000000000000000000000000000000000000", 2u, 987654321u )]
    [TestCase( "53m6;7`00000hEAP000488D0000000000000000l0h<335m907RCQ4QH22PDU23k3@00000", 2u, 0u )]   // ais.kystverket.no
    [TestCase( "53m89J81SR44hdhL0004eH5A84q<00000000001@4P8825LD08j0DTm0A00000000000000", 2u, 6522945u )]   // ais.kystverket.no
    public void ImoNumber_is_decoded( string payload, uint padding, uint expected )
    {
        var parser = new NmeaAisStaticAndVoyageRelatedDataParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.ImoNumber.ShouldBe( expected );
    }

    // Call sign: 7 six-bit characters (bits 70-111). '@' is the standard padding ("@@@@@@@" means "not available"),
    // a shorter expected value is padded with spaces (see AisStringHelper).
    [TestCase( "50000000000000000000000000000000000000000000000000000000000000000000000", 2u, "@@@@@@@" )]
    [TestCase( "50000000000100000000000000000000000000000000000000000000000000000000000", 2u, "P@@@@@@" )]
    [TestCase( "50000000000300000000000000000000000000000000000000000000000000000000000", 2u, "0@@@@@@" )]
    [TestCase( "50000000000000000040000000000000000000000000000000000000000000000000000", 2u, "@@@@@@A" )]
    [TestCase( "5C@oTQT2Ad5duL9W:21=@EHq>085A@hE:222220l00P745m>0>S3kQiF@DPVAC`88888880", 2u, "OWBY2" )]   // ais.kystverket.no
    [TestCase( "53JIbD42BlHOTP7;WSIHth622222222222222216D@L;B5n8NITSm51DQ0CH88888888880", 2u, "9HA2986" )]   // ais.kystverket.no
    [TestCase( "53m89J81SR44hdhL0004eH5A84q<00000000001@4P8825LD08j0DTm0A00000000000000", 2u, "LKLG@@@" )]   // ais.kystverket.no
    [TestCase( "53mr4E42EBp00000001HU<Ttr0tJ1@PF0H`u8A<t00b5:40Ht3h00000000000000000000", 2u, "@@@@@@@" )]   // ais.kystverket.no
    public void CallSign_is_decoded( string payload, uint padding, string expected )
    {
        var parser = new NmeaAisStaticAndVoyageRelatedDataParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.CallSign.ShouldBeAisString( expected, 7 );
    }

    // Vessel name: 20 six-bit characters (bits 112-231). '@' is the standard padding, but real transponders often pad with spaces.
    [TestCase( "50000000000000000000000000000000000000000000000000000000000000000000000", 2u, "@@@@@@@@@@@@@@@@@@@@" )]
    [TestCase( "50000000000000000010000000000000000000000000000000000000000000000000000", 2u, "P@@@@@@@@@@@@@@@@@@@" )]
    [TestCase( "50000000000000000030000000000000000000000000000000000000000000000000000", 2u, "0@@@@@@@@@@@@@@@@@@@" )]
    [TestCase( "50000000000000000000000000000000000000400000000000000000000000000000000", 2u, "@@@@@@@@@@@@@@@@@@@A" )]
    [TestCase( "5C@oTQT2Ad5duL9W:21=@EHq>085A@hE:222220l00P745m>0>S3kQiF@DPVAC`88888880", 2u, "STEVNS BATTLER" )]   // ais.kystverket.no
    [TestCase( "5SmkPJ02AmrLh=PV2208t60t@Tr222222222220l2@B785j:0BU4SkQ21BCH88888888880", 2u, "BOA ODIN" )]   // ais.kystverket.no
    [TestCase( "53m89J81SR44hdhL0004eH5A84q<00000000001@4P8825LD08j0DTm0A00000000000000", 2u, "AKVATRANS@@@@@@@@@@@" )]   // ais.kystverket.no
    [TestCase( "53mr4E42EBp00000001HU<Ttr0tJ1@PF0H`u8A<t00b5:40Ht3h00000000000000000000", 2u, "VISION OF THE FJORDS" )]   // ais.kystverket.no
    public void VesselName_is_decoded( string payload, uint padding, string expected )
    {
        var parser = new NmeaAisStaticAndVoyageRelatedDataParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.VesselName.ShouldBeAisString( expected, 20 );
    }

    // Ship type: 8 bits (232-239). 0 means "not available".
    [TestCase( "50000000000000000000000000000000000000000000000000000000000000000000000", 2u, ShipType.NotAvailable )]
    [TestCase( "500000000000000000000000000000000000000E0000000000000000000000000000000", 2u, ShipType.WingInGroundHazardousCategoryX )]
    [TestCase( "53mg2o400000hOSGOJ18E=@hE=>0<P4hhDpLE:0Q0H<6640008hj<M`1Sl`2CQSp8888880", 2u, ShipType.Dredger )]   // ais.kystverket.no
    [TestCase( "53mFoV000000hf3C3S08`u8pH`Dhh0000000000m1@6334rdR0888888888888888888880", 2u, ShipType.PortOrFishTender )]   // ais.kystverket.no
    [TestCase( "53JIbD42BlHOTP7;WSIHth622222222222222216D@L;B5n8NITSm51DQ0CH88888888880", 2u, ShipType.CargoAll )]   // ais.kystverket.no
    [TestCase( "548dvb02<<pTiT8l0008DhLT61<D5L5U<00000183jJC65mE0?DiAkm0000000000000000", 2u, ShipType.CargoHazardousCategoryY )]   // ais.kystverket.no
    [TestCase( "53m7TH800000hS;3?P0Hu<p604ltp0000000001?0P;5340Ht5531Wu=N=eN=u000000000", 2u, ShipType.CargoNoAdditionalInformation )]   // ais.kystverket.no
    [TestCase( "53QI:t02=oDi`@Q3B20dEV0l58Tr22222222221@8PD765lcN>4SkSRCQiC`88888888880", 2u, ShipType.TankerAll )]   // ais.kystverket.no
    public void ShipType_is_decoded( string payload, uint padding, ShipType expected )
    {
        var parser = new NmeaAisStaticAndVoyageRelatedDataParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.ShipType.ShouldBe( expected );
    }

    // Dimension to bow: 9 bits (240-248), in metres, from 0 (not available) to 511 (511 m or more).
    [TestCase( "50000000000000000000000000000000000000000000000000000000000000000000000", 2u, 0u )]
    [TestCase( "50000000000000000000000000000000000000000800000000000000000000000000000", 2u, 1u )]
    [TestCase( "5000000000000000000000000000000000000000wp00000000000000000000000000000", 2u, 511u )]
    [TestCase( "5C@oTQT2Ad5duL9W:21=@EHq>085A@hE:222220l00P745m>0>S3kQiF@DPVAC`88888880", 2u, 0u )]   // ais.kystverket.no
    [TestCase( "53m6;7`00000hEAP000488D0000000000000000l0h<335m907RCQ4QH22PDU23k3@00000", 2u, 6u )]   // ais.kystverket.no
    [TestCase( "53JIbD42BlHOTP7;WSIHth622222222222222216D@L;B5n8NITSm51DQ0CH88888888880", 2u, 162u )]   // ais.kystverket.no
    public void DimensionToBow_is_decoded( string payload, uint padding, uint expected )
    {
        var parser = new NmeaAisStaticAndVoyageRelatedDataParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.DimensionToBow.ShouldBe( expected );
    }

    // Dimension to stern: 9 bits (249-257), in metres, from 0 (not available) to 511 (511 m or more).
    [TestCase( "50000000000000000000000000000000000000000000000000000000000000000000000", 2u, 0u )]
    [TestCase( "50000000000000000000000000000000000000000010000000000000000000000000000", 2u, 1u )]
    [TestCase( "500000000000000000000000000000000000000007w0000000000000000000000000000", 2u, 511u )]
    [TestCase( "53mFoV000000hf3C3S08`u8pH`Dhh0000000000m1@6334rdR0888888888888888888880", 2u, 6u )]   // ais.kystverket.no
    [TestCase( "548dvb02<<pTiT8l0008DhLT61<D5L5U<00000183jJC65mE0?DiAkm0000000000000000", 2u, 154u )]   // ais.kystverket.no
    public void DimensionToStern_is_decoded( string payload, uint padding, uint expected )
    {
        var parser = new NmeaAisStaticAndVoyageRelatedDataParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.DimensionToStern.ShouldBe( expected );
    }

    // Dimension to port: 6 bits (258-263), in metres, from 0 (not available) to 63 (63 m or more).
    [TestCase( "50000000000000000000000000000000000000000000000000000000000000000000000", 2u, 0u )]
    [TestCase( "50000000000000000000000000000000000000000001000000000000000000000000000", 2u, 1u )]
    [TestCase( "5000000000000000000000000000000000000000000w000000000000000000000000000", 2u, 63u )]
    [TestCase( "53mFoV000000hf3C3S08`u8pH`Dhh0000000000m1@6334rdR0888888888888888888880", 2u, 3u )]   // ais.kystverket.no
    [TestCase( "548dvb02<<pTiT8l0008DhLT61<D5L5U<00000183jJC65mE0?DiAkm0000000000000000", 2u, 19u )]   // ais.kystverket.no
    public void DimensionToPort_is_decoded( string payload, uint padding, uint expected )
    {
        var parser = new NmeaAisStaticAndVoyageRelatedDataParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.DimensionToPort.ShouldBe( expected );
    }

    // Dimension to starboard: 6 bits (264-269), in metres, from 0 (not available) to 63 (63 m or more).
    [TestCase( "50000000000000000000000000000000000000000000000000000000000000000000000", 2u, 0u )]
    [TestCase( "50000000000000000000000000000000000000000000100000000000000000000000000", 2u, 1u )]
    [TestCase( "50000000000000000000000000000000000000000000w00000000000000000000000000", 2u, 63u )]
    [TestCase( "53m89J81SR44hdhL0004eH5A84q<00000000001@4P8825LD08j0DTm0A00000000000000", 2u, 2u )]   // ais.kystverket.no
    [TestCase( "53JIbD42BlHOTP7;WSIHth622222222222222216D@L;B5n8NITSm51DQ0CH88888888880", 2u, 18u )]   // ais.kystverket.no
    public void DimensionToStarboard_is_decoded( string payload, uint padding, uint expected )
    {
        var parser = new NmeaAisStaticAndVoyageRelatedDataParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.DimensionToStarboard.ShouldBe( expected );
    }

    // Type of electronic position fixing device: 4 bits (270-273). Undefined (0) means "not available".
    // Oddity kept from the SpecFlow source: these rows use padding 0 instead of 2 like the rest of the file,
    // so the parser sees 426 bits instead of 424. This is harmless here: the field lies well before the
    // end of the message, so the test stays meaningful (each synthetic row sets a distinct value in bits 270-273).
    // The last row expects the numeric value 15, which is EpfdFixType.InternalGnss.
    [TestCase( "50000000000000000000000000000000000000000000000000000000000000000000000", 0u, EpfdFixType.Undefined )]
    [TestCase( "50000000000000000000000000000000000000000000040000000000000000000000000", 0u, EpfdFixType.Gps )]
    [TestCase( "50000000000000000000000000000000000000000000080000000000000000000000000", 0u, EpfdFixType.Glonass )]
    [TestCase( "500000000000000000000000000000000000000000000<0000000000000000000000000", 0u, EpfdFixType.CombinedGpsGlonass )]
    [TestCase( "500000000000000000000000000000000000000000000@0000000000000000000000000", 0u, EpfdFixType.LoranC )]
    [TestCase( "500000000000000000000000000000000000000000000D0000000000000000000000000", 0u, EpfdFixType.Chayka )]
    [TestCase( "500000000000000000000000000000000000000000000H0000000000000000000000000", 0u, EpfdFixType.IntegratedNavigationSystem )]
    [TestCase( "500000000000000000000000000000000000000000000L0000000000000000000000000", 0u, EpfdFixType.Surveyed )]
    [TestCase( "500000000000000000000000000000000000000000000P0000000000000000000000000", 0u, EpfdFixType.Galileo )]
    [TestCase( "53m8:d`2F;v4hHQR220PE8l4pr0a:2222222221J0`?6600Ht8kCR81RDj1PDSDp8888880", 0u, EpfdFixType.Undefined )]   // ais.kystverket.no
    [TestCase( "53m89J81SR44hdhL0004eH5A84q<00000000001@4P8825LD08j0DTm0A00000000000000", 0u, EpfdFixType.Gps )]   // ais.kystverket.no
    [TestCase( "544MR0827oeaD<u0000lDdP4pTf0duAA<uH000167pF=2=nG0:0DRj0CQiC4jh000000000", 0u, EpfdFixType.CombinedGpsGlonass )]   // ais.kystverket.no
    [TestCase( "53mE09400000hoC3301<4pAV222222200000000N0h:23t0Ht6CP@000000000000000000", 0u, (EpfdFixType)15 )]   // ais.kystverket.no
    public void PositionFixType_is_decoded( string payload, uint padding, EpfdFixType expected )
    {
        var parser = new NmeaAisStaticAndVoyageRelatedDataParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.PositionFixType.ShouldBe( expected );
    }

    // ETA month: 4 bits (274-277), from 1 to 12. 0 means "not available".
    [TestCase( "50000000000000000000000000000000000000000000000000000000000000000000000", 2u, 0u )]
    [TestCase( "5000000000000000000000000000000000000000000000@000000000000000000000000", 2u, 1u )]
    [TestCase( "5000000000000000000000000000000000000000000000P000000000000000000000000", 2u, 2u )]
    [TestCase( "50000000000000000000000000000000000000000000010000000000000000000000000", 2u, 4u )]
    [TestCase( "5000000000000000000000000000000000000000000001@000000000000000000000000", 2u, 5u )]
    [TestCase( "50000000000000000000000000000000000000000000020000000000000000000000000", 2u, 8u )]
    [TestCase( "50000000000000000000000000000000000000000000030000000000000000000000000", 2u, 12u )]
    [TestCase( "53mr4E42EBp00000001HU<Ttr0tJ1@PF0H`u8A<t00b5:40Ht3h00000000000000000000", 2u, 0u )]   // ais.kystverket.no
    [TestCase( "53m89J81SR44hdhL0004eH5A84q<00000000001@4P8825LD08j0DTm0A00000000000000", 2u, 5u )]   // ais.kystverket.no
    [TestCase( "544MR0827oeaD<u0000lDdP4pTf0duAA<uH000167pF=2=nG0:0DRj0CQiC4jh000000000", 2u, 7u )]   // ais.kystverket.no
    public void EtaMonth_is_decoded( string payload, uint padding, uint expected )
    {
        var parser = new NmeaAisStaticAndVoyageRelatedDataParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.EtaMonth.ShouldBe( expected );
    }

    // ETA day: 5 bits (278-282), from 1 to 31. 0 means "not available".
    [TestCase( "50000000000000000000000000000000000000000000000000000000000000000000000", 2u, 0u )]
    [TestCase( "50000000000000000000000000000000000000000000000P00000000000000000000000", 2u, 1u )]
    [TestCase( "50000000000000000000000000000000000000000000001000000000000000000000000", 2u, 2u )]
    [TestCase( "50000000000000000000000000000000000000000000001P00000000000000000000000", 2u, 3u )]
    [TestCase( "50000000000000000000000000000000000000000000002000000000000000000000000", 2u, 4u )]
    [TestCase( "50000000000000000000000000000000000000000000004000000000000000000000000", 2u, 8u )]
    [TestCase( "50000000000000000000000000000000000000000000008000000000000000000000000", 2u, 16u )]
    [TestCase( "5000000000000000000000000000000000000000000000?000000000000000000000000", 2u, 30u )]
    [TestCase( "5000000000000000000000000000000000000000000000?P00000000000000000000000", 2u, 31u )]
    [TestCase( "53mr4E42EBp00000001HU<Ttr0tJ1@PF0H`u8A<t00b5:40Ht3h00000000000000000000", 2u, 0u )]   // ais.kystverket.no
    [TestCase( "53QI:t02=oDi`@Q3B20dEV0l58Tr22222222221@8PD765lcN>4SkSRCQiC`88888888880", 2u, 9u )]   // ais.kystverket.no
    [TestCase( "55AQcl42D>PTQ3G7C63NmL5HE>2222222222221J28B545en08im@DQ2CQp43k0D`1CD4cP", 2u, 27u )]   // ais.kystverket.no
    public void EtaDay_is_decoded( string payload, uint padding, uint expected )
    {
        var parser = new NmeaAisStaticAndVoyageRelatedDataParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.EtaDay.ShouldBe( expected );
    }

    // ETA hour: 5 bits (283-287), from 0 to 23. 24 means "not available".
    [TestCase( "50000000000000000000000000000000000000000000000000000000000000000000000", 2u, 0u )]
    [TestCase( "50000000000000000000000000000000000000000000000100000000000000000000000", 2u, 1u )]
    [TestCase( "50000000000000000000000000000000000000000000000200000000000000000000000", 2u, 2u )]
    [TestCase( "50000000000000000000000000000000000000000000000@00000000000000000000000", 2u, 16u )]
    [TestCase( "50000000000000000000000000000000000000000000000G00000000000000000000000", 2u, 23u )]
    [TestCase( "50000000000000000000000000000000000000000000000H00000000000000000000000", 2u, 24u )]
    [TestCase( "53mg2o400000hOSGOJ18E=@hE=>0<P4hhDpLE:0Q0H<6640008hj<M`1Sl`2CQSp8888880", 2u, 0u )]   // ais.kystverket.no
    [TestCase( "53JIbD42BlHOTP7;WSIHth622222222222222216D@L;B5n8NITSm51DQ0CH88888888880", 2u, 8u )]   // ais.kystverket.no
    [TestCase( "53mFoV000000hf3C3S08`u8pH`Dhh0000000000m1@6334rdR0888888888888888888880", 2u, 12u )]   // ais.kystverket.no
    [TestCase( "544MR0827oeaD<u0000lDdP4pTf0duAA<uH000167pF=2=nG0:0DRj0CQiC4jh000000000", 2u, 23u )]   // ais.kystverket.no
    [TestCase( "53mr4E42EBp00000001HU<Ttr0tJ1@PF0H`u8A<t00b5:40Ht3h00000000000000000000", 2u, 24u )]   // ais.kystverket.no
    public void EtaHour_is_decoded( string payload, uint padding, uint expected )
    {
        var parser = new NmeaAisStaticAndVoyageRelatedDataParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.EtaHour.ShouldBe( expected );
    }

    // ETA minute: 6 bits (288-293), from 0 to 59. 60 means "not available".
    [TestCase( "50000000000000000000000000000000000000000000000000000000000000000000000", 2u, 0u )]
    [TestCase( "50000000000000000000000000000000000000000000000010000000000000000000000", 2u, 1u )]
    [TestCase( "500000000000000000000000000000000000000000000000:0000000000000000000000", 2u, 10u )]
    [TestCase( "500000000000000000000000000000000000000000000000s0000000000000000000000", 2u, 59u )]
    [TestCase( "500000000000000000000000000000000000000000000000t0000000000000000000000", 2u, 60u )]
    [TestCase( "53m89J81SR44hdhL0004eH5A84q<00000000001@4P8825LD08j0DTm0A00000000000000", 2u, 0u )]   // ais.kystverket.no
    [TestCase( "53mFoV000000hf3C3S08`u8pH`Dhh0000000000m1@6334rdR0888888888888888888880", 2u, 34u )]   // ais.kystverket.no
    [TestCase( "53mr4E42EBp00000001HU<Ttr0tJ1@PF0H`u8A<t00b5:40Ht3h00000000000000000000", 2u, 60u )]   // ais.kystverket.no
    public void EtaMinute_is_decoded( string payload, uint padding, uint expected )
    {
        var parser = new NmeaAisStaticAndVoyageRelatedDataParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.EtaMinute.ShouldBe( expected );
    }

    // Draught: 8 bits (294-301), in tenths of metres, from 0 (not available) to 255 (25.5 m or more).
    [TestCase( "50000000000000000000000000000000000000000000000000000000000000000000000", 2u, 0u )]
    [TestCase( "50000000000000000000000000000000000000000000000000@00000000000000000000", 2u, 1u )]
    [TestCase( "50000000000000000000000000000000000000000000000000P00000000000000000000", 2u, 2u )]
    [TestCase( "50000000000000000000000000000000000000000000000000h00000000000000000000", 2u, 3u )]
    [TestCase( "50000000000000000000000000000000000000000000000001000000000000000000000", 2u, 4u )]
    [TestCase( "5000000000000000000000000000000000000000000000000P000000000000000000000", 2u, 128u )]
    [TestCase( "5000000000000000000000000000000000000000000000000wh00000000000000000000", 2u, 255u )]
    [TestCase( "53mFoV000000hf3C3S08`u8pH`Dhh0000000000m1@6334rdR0888888888888888888880", 2u, 0u )]   // ais.kystverket.no
    [TestCase( "53mr4E42EBp00000001HU<Ttr0tJ1@PF0H`u8A<t00b5:40Ht3h00000000000000000000", 2u, 15u )]   // ais.kystverket.no
    [TestCase( "53JIbD42BlHOTP7;WSIHth622222222222222216D@L;B5n8NITSm51DQ0CH88888888880", 2u, 102u )]   // ais.kystverket.no
    public void Draught10thMetres_is_decoded( string payload, uint padding, uint expected )
    {
        var parser = new NmeaAisStaticAndVoyageRelatedDataParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.Draught10thMetres.ShouldBe( expected );
    }

    // Destination: 20 six-bit characters (bits 302-421). '@' is the standard padding, but real transponders often pad
    // with spaces: the empty expected value of the last row means 20 spaces.
    [TestCase( "50000000000000000000000000000000000000000000000000000000000000000000000", 2u, "@@@@@@@@@@@@@@@@@@@@" )]
    [TestCase( "50000000000000000000000000000000000000000000000000<00000000000000000000", 2u, "0@@@@@@@@@@@@@@@@@@@" )]
    [TestCase( "5000000000000000000000000000000000000000000000000000000000000000000000@", 2u, "@@@@@@@@@@@@@@@@@@@A" )]
    [TestCase( "53mr4E42EBp00000001HU<Ttr0tJ1@PF0H`u8A<t00b5:40Ht3h00000000000000000000", 2u, "@@@@@@@@@@@@@@@@@@@@" )]   // ais.kystverket.no
    [TestCase( "53m89J81SR44hdhL0004eH5A84q<00000000001@4P8825LD08j0DTm0A00000000000000", 2u, "HARSTAD@@@@@@@@@@@@@" )]   // ais.kystverket.no
    [TestCase( "5SmkPJ02AmrLh=PV2208t60t@Tr222222222220l2@B785j:0BU4SkQ21BCH88888888880", 2u, "TRONDHEIM" )]   // ais.kystverket.no
    [TestCase( "53mFoV000000hf3C3S08`u8pH`Dhh0000000000m1@6334rdR0888888888888888888880", 2u, "" )]   // ais.kystverket.no
    public void Destination_is_decoded( string payload, uint padding, string expected )
    {
        var parser = new NmeaAisStaticAndVoyageRelatedDataParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.Destination.ShouldBeAisString( expected, 20 );
    }

    // DTE (Data Terminal Equipment) flag: bit 422. Set means "not ready".
    [TestCase( "50000000000000000000000000000000000000000000000000000000000000000000000", 2u, false )]
    [TestCase( "50000000000000000000000000000000000000000000000000000000000000000000008", 2u, true )]
    [TestCase( "53m89J81SR44hdhL0004eH5A84q<00000000001@4P8825LD08j0DTm0A00000000000000", 2u, false )]   // ais.kystverket.no
    [TestCase( "53m:<F82FRLThI1:220puH60l5=@E:222222221J0`D460rdR8p88888888888888888888", 2u, true )]   // ais.kystverket.no
    public void IsDteNotReady_is_decoded( string payload, uint padding, bool expected )
    {
        var parser = new NmeaAisStaticAndVoyageRelatedDataParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.IsDteNotReady.ShouldBe( expected );
    }

    // Spare: bit 423, the last bit of the message.
    [TestCase( "50000000000000000000000000000000000000000000000000000000000000000000000", 2u, false )]
    [TestCase( "50000000000000000000000000000000000000000000000000000000000000000000004", 2u, true )]
    [TestCase( "53m89J81SR44hdhL0004eH5A84q<00000000001@4P8825LD08j0DTm0A00000000000000", 2u, false )]   // ais.kystverket.no
    [TestCase( "5=7LHAT000000000000L58A<uT6085B3@000000U0@21140Ht000000000000000000000<", 2u, true )]   // ais.kystverket.no
    public void SpareBit423_is_decoded( string payload, uint padding, bool expected )
    {
        var parser = new NmeaAisStaticAndVoyageRelatedDataParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.SpareBit423.ShouldBe( expected );
    }
}
