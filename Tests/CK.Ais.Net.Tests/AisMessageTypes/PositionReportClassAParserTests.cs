// <copyright file="PositionReportClassAParserTests.cs" company="Endjin Limited">
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

// NmeaAisPositionReportClassAParser decodes message types 1, 2 and 3 (Position Report Class A,
// scheduled / assigned / response to interrogation): 168 bits, every field is checked with
// synthetic payloads (only the field under test is set, including its extreme values) and real ones.
[TestFixture]
public class PositionReportClassAParserTests
{
    // Message type: bits 0-5.
    [TestCase( "1000000000000000000000000000", 0u, MessageType.PositionReport1 )]
    [TestCase( "1Co`hD000`0unrRcusDEcTOL0P00", 0u, MessageType.PositionReport1 )]   // ais.kystverket.no
    public void MessageType_is_decoded( string payload, uint padding, MessageType type )
    {
        var parser = new NmeaAisPositionReportClassAParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.MessageType.ShouldBe( type );
    }

    // Repeat indicator: bits 6-7 (0 to 3).
    [TestCase( "1000000000000000000000000000", 0u, 0u )]
    [TestCase( "1@00000000000000000000000000", 0u, 1u )]
    [TestCase( "1P00000000000000000000000000", 0u, 2u )]
    [TestCase( "1h00000000000000000000000000", 0u, 3u )]
    [TestCase( "13nW5<00000IoPlSbE`:P8EH0534", 0u, 0u )]   // ais.kystverket.no
    [TestCase( "1Co`hD000`0unrRcusDEcTOL0P00", 0u, 1u )]   // ais.kystverket.no
    [TestCase( "1Smbi200020cNi@TKh96@BEL0000", 0u, 2u )]   // ais.kystverket.no
    [TestCase( "1kmbi20P@10cNiLTKgi6Wld>0000", 0u, 3u )]   // ais.kystverket.no
    public void RepeatIndicator_is_decoded( string payload, uint padding, uint repeatCount )
    {
        var parser = new NmeaAisPositionReportClassAParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.RepeatIndicator.ShouldBe( repeatCount );
    }

    // MMSI: bits 8-37 (30 bits unsigned).
    [TestCase( "1000000000000000000000000000", 0u, 0u )]
    [TestCase( "100000@000000000000000000000", 0u, 1u )]
    [TestCase( "100000P000000000000000000000", 0u, 2u )]
    [TestCase( "1>eq`d@000000000000000000000", 0u, 987654321u )]   // ais.kystverket.no
    [TestCase( "13nW5<00000IoPlSbE`:P8EH0534", 0u, 258590000u )]   // ais.kystverket.no
    [TestCase( "24c`1`001kPEGSLR98IP00462D0s", 0u, 314180000u )]   // ais.kystverket.no
    public void Mmsi_is_decoded( string payload, uint padding, uint mmsi )
    {
        var parser = new NmeaAisPositionReportClassAParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.Mmsi.ShouldBe( mmsi );
    }

    // Navigation status: bits 38-41, every one of the 16 values (including the reserved ones).
    [TestCase( "1000000000000000000000000000", 0u, NavigationStatus.UnderwayUsingEngine )]
    [TestCase( "1000001000000000000000000000", 0u, NavigationStatus.AtAnchor )]
    [TestCase( "1000002000000000000000000000", 0u, NavigationStatus.NotUnderCommand )]
    [TestCase( "1000003000000000000000000000", 0u, NavigationStatus.RestrictedManoeuverability )]
    [TestCase( "1000004000000000000000000000", 0u, NavigationStatus.ConstrainedByHerDraught )]
    [TestCase( "1000005000000000000000000000", 0u, NavigationStatus.Moored )]
    [TestCase( "1000006000000000000000000000", 0u, NavigationStatus.Aground )]
    [TestCase( "1000007000000000000000000000", 0u, NavigationStatus.EngagedInFishing )]
    [TestCase( "1000008000000000000000000000", 0u, NavigationStatus.UnderWaySailing )]
    [TestCase( "1000009000000000000000000000", 0u, NavigationStatus.ReservedForFutureAmendmentOfNavigationalStatusForHsc )]
    [TestCase( "100000:000000000000000000000", 0u, NavigationStatus.ReservedForFutureAmendmentOfNavigationalStatusForWig )]
    [TestCase( "100000;000000000000000000000", 0u, NavigationStatus.ReservedForFutureUse11 )]
    [TestCase( "100000<000000000000000000000", 0u, NavigationStatus.ReservedForFutureUse12 )]
    [TestCase( "100000=000000000000000000000", 0u, NavigationStatus.ReservedForFutureUse13 )]
    [TestCase( "100000>000000000000000000000", 0u, NavigationStatus.AisSartIsActive )]
    [TestCase( "100000?000000000000000000000", 0u, NavigationStatus.NotDefined )]
    [TestCase( "13nW5<00000IoPlSbE`:P8EH0534", 0u, NavigationStatus.UnderwayUsingEngine )]   // ais.kystverket.no
    [TestCase( "13@oSF101;PTR`fPLn2:U8S`0534", 0u, NavigationStatus.AtAnchor )]   // ais.kystverket.no
    [TestCase( "13mVWJ2P01P`jBlTMPsN4?v22<0G", 0u, NavigationStatus.NotUnderCommand )]   // ais.kystverket.no
    [TestCase( "13oFwV3P?w<tSF0l4Q@>4?wv0PSu", 0u, NavigationStatus.RestrictedManoeuverability )]   // ais.kystverket.no
    [TestCase( "15CIo>401f0jO4hQ0KN<F9n:0D<P", 0u, NavigationStatus.ConstrainedByHerDraught )]   // ais.kystverket.no
    [TestCase( "13n53M50001P2jv`4iFe@rJ<0000", 0u, NavigationStatus.Moored )]   // ais.kystverket.no
    [TestCase( "13mTRV701F23hFp`VlU4ul><0<0;", 0u, NavigationStatus.EngagedInFishing )]   // ais.kystverket.no
    [TestCase( "13B6BP80000jt=tPk:Isf:F@0538", 0u, NavigationStatus.UnderWaySailing )]   // ais.kystverket.no
    [TestCase( "13m`0o9P4hPTEKLQ>f<:ROvb0003", 0u, NavigationStatus.ReservedForFutureAmendmentOfNavigationalStatusForHsc )]   // ais.kystverket.no
    [TestCase( "13mClB:Oi<Pi:U4U5n?J;8CD00Rq", 0u, NavigationStatus.ReservedForFutureAmendmentOfNavigationalStatusForWig )]   // ais.kystverket.no
    [TestCase( "13m6;7cP020HJ`JRs9l0A?v`08<9", 0u, NavigationStatus.ReservedForFutureUse11 )]   // ais.kystverket.no
    [TestCase( "13m9`rdw01QAv`<WiMAbF3ap2H1a", 0u, NavigationStatus.ReservedForFutureUse12 )]   // ais.kystverket.no
    [TestCase( "13P=N7M1@10Ph14SqI7pAW@j0L0G", 0u, NavigationStatus.ReservedForFutureUse13 )]   // ais.kystverket.no
    [TestCase( "13@njcg000PeVI>Pu9Hnul9D0D1M", 0u, NavigationStatus.NotDefined )]   // ais.kystverket.no
    public void NavigationStatus_is_decoded( string payload, uint padding, NavigationStatus navigationStatus )
    {
        var parser = new NmeaAisPositionReportClassAParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.NavigationStatus.ShouldBe( navigationStatus );
    }

    // Rate of turn: bits 42-49, signed (-128 means "not available", +/-127 means turning faster than 5 deg/30s).
    [TestCase( "1000000000000000000000000000", 0u, 0 )]
    [TestCase( "1000000wh0000000000000000000", 0u, -1 )]
    [TestCase( "1000000Oh0000000000000000000", 0u, 127 )]
    [TestCase( "1000000P00000000000000000000", 0u, -128 )]
    [TestCase( "13nW5<00000IoPlSbE`:P8EH0534", 0u, 0 )]   // ais.kystverket.no
    [TestCase( "13oHtV7OhN0=B9bQch;WqnCp0W3h", 0u, 127 )]   // ais.kystverket.no
    [TestCase( "13mCIp0P00PFnJBSHS1>4?wH2@JB", 0u, -128 )]   // ais.kystverket.no
    [TestCase( "13P=N7M1@10Ph14SqI7pAW@j0L0G", 0u, 5 )]   // ais.kystverket.no
    public void RateOfTurn_is_decoded( string payload, uint padding, int rateOfTurn )
    {
        var parser = new NmeaAisPositionReportClassAParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.RateOfTurn.ShouldBe( rateOfTurn );
    }

    // Speed over ground: bits 50-59 in 1/10 knot (1023 means "not available").
    [TestCase( "1000000000000000000000000000", 0u, 0u )]
    [TestCase( "1000000001000000000000000000", 0u, 1u )]
    [TestCase( "10000000?w000000000000000000", 0u, 1023u )]
    [TestCase( "13nW5<00000IoPlSbE`:P8EH0534", 0u, 0u )]   // ais.kystverket.no
    [TestCase( "1kmbi20P@10cNiLTKgi6Wld>0000", 0u, 1u )]   // ais.kystverket.no
    [TestCase( "13@oSF101;PTR`fPLn2:U8S`0534", 0u, 75u )]   // ais.kystverket.no
    [TestCase( "13m`0o9P4hPTEKLQ>f<:ROvb0003", 0u, 304u )]   // ais.kystverket.no
    [TestCase( "13oFwV3P?w<tSF0l4Q@>4?wv0PSu", 0u, 1023u )]   // ais.kystverket.no
    public void SpeedOverGround_is_decoded( string payload, uint padding, uint speedOverGround )
    {
        var parser = new NmeaAisPositionReportClassAParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.SpeedOverGroundTenths.ShouldBe( speedOverGround );
    }

    // Position accuracy: bit 60.
    [TestCase( "1000000000000000000000000000", 0u, false )]
    [TestCase( "1000000000P00000000000000000", 0u, true )]
    [TestCase( "13nW5<00000IoPlSbE`:P8EH0534", 0u, false )]   // ais.kystverket.no
    [TestCase( "13@oSF101;PTR`fPLn2:U8S`0534", 0u, true )]   // ais.kystverket.no
    public void PositionAccuracy_is_decoded( string payload, uint padding, bool positionAccuracy )
    {
        var parser = new NmeaAisPositionReportClassAParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.PositionAccuracy.ShouldBe( positionAccuracy );
    }

    // Longitude (bits 61-88, 28 bits signed) and latitude (bits 89-115, 27 bits signed) in 1/10000 minute.
    // 108600000 / 54600000 (181 / 91 degrees) mean "not available".
    [TestCase( "1000000000000000000000000000", 0u, 0, 0 )]
    [TestCase( "1000000000000020000000000000", 0u, 1, 0 )]
    [TestCase( "1000000000000000000@00000000", 0u, 0, 1 )]
    [TestCase( "1000000000Owwwv0000000000000", 0u, -1, 0 )]
    [TestCase( "100000000000001wwwwh00000000", 0u, 0, -1 )]
    [TestCase( "13nW5<00000IoPlSbE`:P8EH0534", 0u, 3390490, 37393824 )]   // ais.kystverket.no
    [TestCase( "1Co`hD000`0unrRcusDEcTOL0P00", 0u, 8107857, 46103377 )]   // ais.kystverket.no
    [TestCase( "1Smbi200020cNi@TKh96@BEL0000", 0u, 5699112, 38203428 )]   // ais.kystverket.no
    [TestCase( "13oFwV3P?w<tSF0l4Q@>4?wv0PSu", 0u, 108600000, 54600000 )]   // ais.kystverket.no
    public void Longitude_and_latitude_are_decoded( string payload, uint padding, int longitude, int latitude )
    {
        var parser = new NmeaAisPositionReportClassAParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.Longitude10000thMins.ShouldBe( longitude );
        parser.Latitude10000thMins.ShouldBe( latitude );
    }

    // Course over ground: bits 116-127 in 1/10 degree (3600 means "not available").
    [TestCase( "1000000000000000000000000000", 0u, 0u )]
    [TestCase( "100000000000000000000@000000", 0u, 1u )]
    [TestCase( "1000000000000000000>40000000", 0u, 3600u )]
    [TestCase( "1000000000000000000?wh000000", 0u, 4095u )]
    [TestCase( "13mSjt7P001KOu6`6:3@0?wJ0<03", 0u, 0u )]   // ais.kystverket.no
    [TestCase( "13m6;7cP020HJ`JRs9l0A?v`08<9", 0u, 68u )]   // ais.kystverket.no
    [TestCase( "1Smbi200020cNi@TKh96@BEL0000", 0u, 1601u )]   // ais.kystverket.no
    [TestCase( "13oO7800000FH>>S6rpur`P200S<", 0u, 3562u )]   // ais.kystverket.no
    [TestCase( "13mVWJ2P01P`jBlTMPsN4?v22<0G", 0u, 3600u )]   // ais.kystverket.no
    public void CourseOverGround_is_decoded( string payload, uint padding, uint courseOverGround )
    {
        var parser = new NmeaAisPositionReportClassAParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.CourseOverGround10thDegrees.ShouldBe( courseOverGround );
    }

    // True heading: bits 128-136 in degrees (511 means "not available").
    [TestCase( "1000000000000000000000000000", 0u, 0u )]
    [TestCase( "1000000000000000000000200000", 0u, 1u )]
    [TestCase( "1000000000000000000000v00000", 0u, 31u )]
    [TestCase( "1000000000000000000001000000", 0u, 32u )]
    [TestCase( "100000000000000000000?v00000", 0u, 511u )]
    [TestCase( "1Smbi200020cNi@TKh96@BEL0000", 0u, 74u )]   // ais.kystverket.no
    [TestCase( "13B6BP80000jt=tPk:Isf:F@0538", 0u, 331u )]   // ais.kystverket.no
    [TestCase( "13mCIp0P00PFnJBSHS1>4?wH2@JB", 0u, 511u )]   // ais.kystverket.no
    public void TrueHeading_is_decoded( string payload, uint padding, uint trueHeading )
    {
        var parser = new NmeaAisPositionReportClassAParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.TrueHeadingDegrees.ShouldBe( trueHeading );
    }

    // Time stamp: bits 137-142, UTC second (60 to 63 are special "not available" / mode values).
    [TestCase( "1000000000000000000000000000", 0u, 0u )]
    [TestCase( "1000000000000000000000020000", 0u, 1u )]
    [TestCase( "10000000000000000000001n0000", 0u, 59u )]
    [TestCase( "10000000000000000000001p0000", 0u, 60u )]
    [TestCase( "10000000000000000000001v0000", 0u, 63u )]
    [TestCase( "13oO7800000FH>>S6rpur`P200S<", 0u, 1u )]   // ais.kystverket.no
    [TestCase( "13nW5<00000IoPlSbE`:P8EH0534", 0u, 44u )]   // ais.kystverket.no
    [TestCase( "13oHtV7OhN0=B9bQch;WqnCp0W3h", 0u, 60u )]   // ais.kystverket.no
    [TestCase( "13oFwV3P?w<tSF0l4Q@>4?wv0PSu", 0u, 63u )]   // ais.kystverket.no
    public void TimeStamp_is_decoded( string payload, uint padding, uint timeStamp )
    {
        var parser = new NmeaAisPositionReportClassAParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.TimeStampSecond.ShouldBe( timeStamp );
    }

    // Manoeuvre indicator: bits 143-144.
    [TestCase( "1000000000000000000000000000", 0u, ManoeuvreIndicator.NotAvailable )]
    [TestCase( "100000000000000000000000P000", 0u, ManoeuvreIndicator.NoSpecialManoeuvre )]
    [TestCase( "1000000000000000000000010000", 0u, ManoeuvreIndicator.SpecialManoeuvre )]
    [TestCase( "100000000000000000000001P000", 0u, ManoeuvreIndicator.NotDefinedBySpec )]
    [TestCase( "13nW5<00000IoPlSbE`:P8EH0534", 0u, ManoeuvreIndicator.NotAvailable )]   // ais.kystverket.no
    [TestCase( "13n@kJ0000PIk0@Sc2==PDQd`53L", 0u, ManoeuvreIndicator.NoSpecialManoeuvre )]   // ais.kystverket.no
    [TestCase( "13m69r00AfPlnG6U;43aD7MO0534", 0u, ManoeuvreIndicator.SpecialManoeuvre )]   // ais.kystverket.no
    public void ManoeuvreIndicator_is_decoded( string payload, uint padding, ManoeuvreIndicator manoeuvre )
    {
        var parser = new NmeaAisPositionReportClassAParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.ManoeuvreIndicator.ShouldBe( manoeuvre );
    }

    // Spare bits: bits 145-147.
    [TestCase( "1000000000000000000000000000", 0u, 0u )]
    [TestCase( "1000000000000000000000004000", 0u, 1u )]
    [TestCase( "1000000000000000000000008000", 0u, 2u )]
    [TestCase( "100000000000000000000000<000", 0u, 3u )]
    [TestCase( "100000000000000000000000@000", 0u, 4u )]
    [TestCase( "100000000000000000000000D000", 0u, 5u )]
    [TestCase( "100000000000000000000000H000", 0u, 6u )]
    [TestCase( "100000000000000000000000L000", 0u, 7u )]
    [TestCase( "13nW5<00000IoPlSbE`:P8EH0534", 0u, 0u )]   // ais.kystverket.no
    [TestCase( "13n@kJ0000PIk0@Sc2==PDQd`53L", 0u, 2u )]   // ais.kystverket.no
    public void SpareBits145_are_decoded( string payload, uint padding, uint spare )
    {
        var parser = new NmeaAisPositionReportClassAParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.SpareBits145.ShouldBe( spare );
    }

    // RAIM flag: bit 148.
    [TestCase( "1000000000000000000000000000", 0u, false )]
    [TestCase( "1000000000000000000000002000", 0u, true )]
    [TestCase( "13nW5<00000IoPlSbE`:P8EH0534", 0u, false )]   // ais.kystverket.no
    [TestCase( "13oHtV7OhN0=B9bQch;WqnCp0W3h", 0u, false )]   // ais.kystverket.no
    [TestCase( "13mCIp0P00PFnJBSHS1>4?wH2@JB", 0u, true )]   // ais.kystverket.no
    public void RaimFlag_is_decoded( string payload, uint padding, bool flag )
    {
        var parser = new NmeaAisPositionReportClassAParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.RaimFlag.ShouldBe( flag );
    }

    // Radio status, sync state: bits 149-150.
    [TestCase( "1000000000000000000000000000", 0u, RadioSyncState.UtcDirect )]
    [TestCase( "1000000000000000000000000P00", 0u, RadioSyncState.UtcIndirect )]
    [TestCase( "1000000000000000000000001000", 0u, RadioSyncState.ToBaseStation )]
    [TestCase( "1000000000000000000000001P00", 0u, RadioSyncState.ToOtherStation )]
    [TestCase( "1Smbi200020cNi@TKh96@BEL0000", 0u, RadioSyncState.UtcDirect )]   // ais.kystverket.no
    [TestCase( "1Co`hD000`0unrRcusDEcTOL0P00", 0u, RadioSyncState.UtcIndirect )]   // ais.kystverket.no
    [TestCase( "13oaO<5000PG1VhS?rC3ajf>1000", 0u, RadioSyncState.ToBaseStation )]   // ais.kystverket.no
    public void RadioSyncState_is_decoded( string payload, uint padding, RadioSyncState state )
    {
        var parser = new NmeaAisPositionReportClassAParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.RadioSyncState.ShouldBe( state );
    }

    // Radio status, slot timeout: bits 151-153.
    [TestCase( "1000000000000000000000000000", 0u, 0u )]
    [TestCase( "1000000000000000000000000400", 0u, 1u )]
    [TestCase( "1000000000000000000000000800", 0u, 2u )]
    [TestCase( "1000000000000000000000000<00", 0u, 3u )]
    [TestCase( "1000000000000000000000000@00", 0u, 4u )]
    [TestCase( "1000000000000000000000000D00", 0u, 5u )]
    [TestCase( "1000000000000000000000000H00", 0u, 6u )]
    [TestCase( "1000000000000000000000000L00", 0u, 7u )]
    [TestCase( "13oFwV3P?w<tSF0l4Q@>4?wv0PSu", 0u, 0u )]   // ais.kystverket.no
    [TestCase( "13@oSF101;PTR`fPLn2:U8S`0534", 0u, 1u )]   // ais.kystverket.no
    [TestCase( "13m6;7cP020HJ`JRs9l0A?v`08<9", 0u, 2u )]   // ais.kystverket.no
    [TestCase( "13mTRV701F23hFp`VlU4ul><0<0;", 0u, 3u )]   // ais.kystverket.no
    [TestCase( "13`j@B001h0lICBPg476:TlB0@5:", 0u, 4u )]   // ais.kystverket.no
    [TestCase( "13@njcg000PeVI>Pu9Hnul9D0D1M", 0u, 5u )]   // ais.kystverket.no
    [TestCase( "13m9`rdw01QAv`<WiMAbF3ap2H1a", 0u, 6u )]   // ais.kystverket.no
    [TestCase( "13P=N7M1@10Ph14SqI7pAW@j0L0G", 0u, 7u )]   // ais.kystverket.no
    public void RadioSlotTimeout_is_decoded( string payload, uint padding, uint timeout )
    {
        var parser = new NmeaAisPositionReportClassAParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.RadioSlotTimeout.ShouldBe( timeout );
    }

    // Radio status, sub message: bits 154-167 (14 bits).
    [TestCase( "1000000000000000000000000000", 0u, 0u )]
    [TestCase( "1000000000000000000000000200", 0u, 8192u )]
    [TestCase( "10000000000000000000000003ww", 0u, 16383u )]
    [TestCase( "13mClB:Oi<Pi:U4U5n?J;8CD00Rq", 0u, 2233u )]   // ais.kystverket.no
    public void RadioSubMessage_is_decoded( string payload, uint padding, uint message )
    {
        var parser = new NmeaAisPositionReportClassAParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.RadioSubMessage.ShouldBe( message );
    }
}
