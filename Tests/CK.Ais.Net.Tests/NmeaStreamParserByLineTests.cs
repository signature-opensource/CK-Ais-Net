// <copyright file="NmeaStreamParserByLineTests.cs" company="Endjin Limited">
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
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Ais.Net.Tests;

// NmeaStreamParser.ParseStreamAsync with an INmeaLineStreamProcessor: the stream is split in lines
// (LF, CR, CRLF or an unterminated last line), blank lines are skipped, each NMEA line is reported
// through OnNext (tag block and sentence) and each unparseable line through OnError (line, exception
// and 1-based line number). OnCompleted is always called once at the end.
// The NmeaParserOptions related to the line layer are also covered here: AllowUnreconizedTalkerId,
// AllowUnreconizedDataOrigin and ChecksumOption (format and validity of the tag block and sentence checksums).
[TestFixture]
public class NmeaStreamParserByLineTests
{
    // ais.kystverket.no
    const string RealLine0 = @"\s:42,c:1567684904*38\!AIVDM,1,1,,A,B3m:H900AP@b:79ae6:<OwnUoP06,0*78";
    // ais.kystverket.no
    const string RealLine1 = @"\s:3,c:1567692251*01\!AIVDM,1,1,,A,13m9WS001d0K==pR=D?HB6WD0pJV,0*4D";
    // ais.kystverket.no
    const string RealLine2 = @"\s:24,c:1567692878*35\!AIVDM,1,1,,B,13o`9@701j1Ej3vc;o3q@7SJ0D02,0*3B";
    // ais.kystverket.no
    const string RealLine3 = @"\s:772,c:1567693246*07\!AIVDM,1,1,,,13o7g2001P0Lv<rSdVHf2h3N0000,0*25";

    // An empty stream: no line, OnCompleted called.
    [Test]
    public async Task Empty_file_Async()
    {
        var p = await ParseByLineAsync( "" );
        p.OnNextCalls.ShouldBeEmpty();
        p.IsComplete.ShouldBeTrue();
    }

    // A single blank CRLF line: no line, OnCompleted called.
    [Test]
    public async Task Single_CRLF_blank_line_only_Async()
    {
        var p = await ParseByLineAsync( CrLf( "" ) );
        p.OnNextCalls.ShouldBeEmpty();
        p.IsComplete.ShouldBeTrue();
    }

    // A single blank LF line: no line, OnCompleted called.
    [Test]
    public async Task Single_LF_blank_line_only_Async()
    {
        var p = await ParseByLineAsync( Lf( "" ) );
        p.OnNextCalls.ShouldBeEmpty();
        p.IsComplete.ShouldBeTrue();
    }

    // A single blank CR line: no line, OnCompleted called.
    [Test]
    public async Task Single_CR_blank_line_only_Async()
    {
        var p = await ParseByLineAsync( Cr( "" ) );
        p.OnNextCalls.ShouldBeEmpty();
        p.IsComplete.ShouldBeTrue();
    }

    // Three blank CRLF lines: no line, OnCompleted called.
    [Test]
    public async Task Multiple_CRLF_blank_lines_only_Async()
    {
        var p = await ParseByLineAsync( CrLf( "" ) + CrLf( "" ) + CrLf( "" ) );
        p.OnNextCalls.ShouldBeEmpty();
        p.IsComplete.ShouldBeTrue();
    }

    // Three blank LF lines: no line, OnCompleted called.
    [Test]
    public async Task Multiple_LF_blank_lines_only_Async()
    {
        var p = await ParseByLineAsync( Lf( "" ) + Lf( "" ) + Lf( "" ) );
        p.OnNextCalls.ShouldBeEmpty();
        p.IsComplete.ShouldBeTrue();
    }

    // Three blank CR lines: no line, OnCompleted called.
    [Test]
    public async Task Multiple_CR_blank_lines_only_Async()
    {
        var p = await ParseByLineAsync( Cr( "" ) + Cr( "" ) + Cr( "" ) );
        p.OnNextCalls.ShouldBeEmpty();
        p.IsComplete.ShouldBeTrue();
    }

    // Blank lines with mixed terminators (CRLF, LF, CRLF, CR): no line, OnCompleted called.
    [Test]
    public async Task Multiple_mixed_blank_lines_only_Async()
    {
        var p = await ParseByLineAsync( CrLf( "" ) + Lf( "" ) + CrLf( "" ) + Cr( "" ) );
        p.OnNextCalls.ShouldBeEmpty();
        p.IsComplete.ShouldBeTrue();
    }

    // One LF line: one line reported, split in its tag block and its sentence.
    [Test]
    public async Task Single_line_Async()
    {
        var p = await ParseByLineAsync( Lf( RealLine0 ) );
        p.OnNextCalls.Count.ShouldBe( 1 );
        // ais.kystverket.no
        p.ShouldHaveLine( 0, "s:42,c:1567684904*38", "!AIVDM,1,1,,A,B3m:H900AP@b:79ae6:<OwnUoP06,0*78" );
        p.IsComplete.ShouldBeTrue();
    }

    // One line without terminator: it is reported like a terminated one.
    [Test]
    public async Task Single_line_without_newline_only_Async()
    {
        var p = await ParseByLineAsync( RealLine0 );
        p.OnNextCalls.Count.ShouldBe( 1 );
        // ais.kystverket.no
        p.ShouldHaveLine( 0, "s:42,c:1567684904*38", "!AIVDM,1,1,,A,B3m:H900AP@b:79ae6:<OwnUoP06,0*78" );
        p.IsComplete.ShouldBeTrue();
    }

    // Four LF lines: the four lines are reported in order.
    [Test]
    public async Task Multiple_lines_Async()
    {
        var p = await ParseByLineAsync( Lf( RealLine0 ) + Lf( RealLine1 ) + Lf( RealLine2 ) + Lf( RealLine3 ) );
        ShouldHaveTheFourRealLines( p );
    }

    // Four CR lines: the four lines are reported in order.
    [Test]
    public async Task Multiple_CR_lines_Async()
    {
        var p = await ParseByLineAsync( Cr( RealLine0 ) + Cr( RealLine1 ) + Cr( RealLine2 ) + Cr( RealLine3 ) );
        ShouldHaveTheFourRealLines( p );
    }

    // Four lines, the last one unterminated: the four lines are reported in order.
    [Test]
    public async Task Multiple_lines_where_final_line_has_no_newline_Async()
    {
        var p = await ParseByLineAsync( Lf( RealLine0 ) + Lf( RealLine1 ) + Lf( RealLine2 ) + RealLine3 );
        ShouldHaveTheFourRealLines( p );
    }

    // Three blank lines then four lines: the blank lines are skipped.
    [Test]
    public async Task Multiple_lines_with_blanks_at_start_Async()
    {
        var p = await ParseByLineAsync( Lf( "" ) + Lf( "" ) + Lf( "" )
                                        + Lf( RealLine0 ) + Lf( RealLine1 ) + Lf( RealLine2 ) + Lf( RealLine3 ) );
        ShouldHaveTheFourRealLines( p );
    }

    // Three blank lines between the 2nd and the 3rd line: the blank lines are skipped.
    [Test]
    public async Task Multiple_lines_with_blanks_in_middle_Async()
    {
        var p = await ParseByLineAsync( Lf( RealLine0 ) + Lf( RealLine1 )
                                        + Lf( "" ) + Lf( "" ) + Lf( "" )
                                        + Lf( RealLine2 ) + Lf( RealLine3 ) );
        ShouldHaveTheFourRealLines( p );
    }

    // Four lines then three blank lines: the blank lines are skipped.
    [Test]
    public async Task Multiple_lines_with_blanks_at_end_Async()
    {
        var p = await ParseByLineAsync( Lf( RealLine0 ) + Lf( RealLine1 ) + Lf( RealLine2 ) + Lf( RealLine3 )
                                        + Lf( "" ) + Lf( "" ) + Lf( "" ) );
        ShouldHaveTheFourRealLines( p );
    }

    // A line that is not NMEA: no line, one error with the line, the "expected '!'" exception and line number 1.
    [Test]
    public async Task Single_unparseable_line_Async()
    {
        var p = await ParseByLineAsync( Lf( "I am not an NMEA message" ) );
        p.OnNextCalls.ShouldBeEmpty();
        p.OnErrorCalls.Count.ShouldBe( 1 );
        p.OnErrorCalls[0].Line.ShouldBe( "I am not an NMEA message" );
        p.OnErrorCalls[0].Error.ShouldBeAssignableTo<ArgumentException>()!.Message.ShouldBe( "Invalid data. Expected '!' at sentence start" );
        p.OnErrorCalls[0].LineNumber.ShouldBe( 1 );
        p.IsComplete.ShouldBeTrue();
    }

    // A good line then a line that is not NMEA: the good line is reported, the bad one is an error on line 2.
    [Test]
    public async Task One_unparseable_line_and_one_good_line_Async()
    {
        var p = await ParseByLineAsync( Lf( RealLine0 ) + Lf( "I am not an NMEA message" ) );
        p.OnNextCalls.Count.ShouldBe( 1 );
        // ais.kystverket.no
        p.ShouldHaveLine( 0, "s:42,c:1567684904*38", "!AIVDM,1,1,,A,B3m:H900AP@b:79ae6:<OwnUoP06,0*78" );
        p.OnErrorCalls.Count.ShouldBe( 1 );
        p.OnErrorCalls[0].Line.ShouldBe( "I am not an NMEA message" );
        p.OnErrorCalls[0].Error.ShouldBeAssignableTo<ArgumentException>()!.Message.ShouldBe( "Invalid data. Expected '!' at sentence start" );
        p.OnErrorCalls[0].LineNumber.ShouldBe( 2 );
        p.IsComplete.ShouldBeTrue();
    }

    // AllowUnreconizedTalkerId = true: a line with the unknown "B2" talker id is reported without error.
    [Test]
    public async Task Allow_unreconized_talker_id_Async()
    {
        var options = new NmeaParserOptions { AllowUnreconizedTalkerId = true };
        var p = await ParseByLineAsync( Lf( @"\i:<O>NOR</O>,s:2573105,c:1668997143*00\!B2VDM,1,1,9,A,H3mWb5P5HT4pD00000000000000,2*0A" ), options );
        p.OnNextCalls.Count.ShouldBe( 1 );
        p.OnErrorCalls.ShouldBeEmpty();
        p.IsComplete.ShouldBeTrue();
    }

    // AllowUnreconizedTalkerId = false: the "B1" and "B2" talker ids are errors that name the invalid last char (as a byte value).
    [Test]
    public async Task Disallow_unreconized_talker_id_Async()
    {
        var options = new NmeaParserOptions { AllowUnreconizedTalkerId = false };
        var p = await ParseByLineAsync( Lf( @"\i:<O>NOR</O>,s:2573105,c:1668997147*04\!B1VDM,2,1,9,A,53m6J4p00000dp=@E=@dp=@E=@0000000000000000000t00000P00000000,0*20" )
                                        + Lf( @"\i:<O>NOR</O>,s:2573105,c:1668997143*00\!B2VDM,1,1,9,A,H3mWb5P5HT4pD00000000000000,2*0A" ),
                                        options );
        p.OnNextCalls.ShouldBeEmpty();
        p.OnErrorCalls.Count.ShouldBe( 2 );
        p.OnErrorCalls[0].Error.Message.ShouldBe( "Invalid data. Unrecognized talker id - cannot end with 49" );
        p.OnErrorCalls[1].Error.Message.ShouldBe( "Invalid data. Unrecognized talker id - cannot end with 50" );
        p.IsComplete.ShouldBeTrue();
    }

    // Scenario title: "Invalid talker id with AllowUnreconizedTalkerId to false throw an error".
    // The title is misleading: the option set is AllowUnreconizedDataOrigin (and "AIVDI" has an invalid data origin "VDI",
    // not an invalid talker id). AllowUnreconizedDataOrigin = false: the line is an error about the data origin.
    [Test]
    public async Task Invalid_data_origin_with_AllowUnreconizedDataOrigin_to_false_is_an_error_Async()
    {
        var options = new NmeaParserOptions { AllowUnreconizedDataOrigin = false };
        var p = await ParseByLineAsync( Lf( @"\c:1725231098,t:TER*78\!AIVDI,1,1,,B,37Oftm3Oh:bcIrUkmmv5`Qfp01o0,0*05" ), options );
        p.OnNextCalls.ShouldBeEmpty();
        p.OnErrorCalls.Count.ShouldBe( 1 );
        p.OnErrorCalls[0].Error.Message.ShouldBe( "Invalid data. Unrecognized origin in AIS talker ID - must be VDM or VDO" );
        p.IsComplete.ShouldBeTrue();
    }

    // Scenario title: "Invalid talker id with AllowUnreconizedTalkerId to true not throw error".
    // AllowUnreconizedDataOrigin = AllowUnreconizedTalkerId = true: lines with an unknown talker id and/or data origin
    // are reported (no error) with the expected AisTalker, DataOrigin and SentenceFormatter.
    [TestCase( @"\c:1725231098,t:TER*78\!AIVDI,1,1,,B,37Oftm3Oh:bcIrUkmmv5`Qfp01o0,0*05", TalkerId.MobileStation, VesselDataOrigin.Unreconized, "!AIVDI" )]
    [TestCase( @"\c:1725173550,t:TER*7C\!AIDMV,1,1,,B,15?dMCh01`9ORmnCKTLQ<hnj0<2?,0*4A", TalkerId.MobileStation, VesselDataOrigin.Unreconized, "!AIDMV" )]
    [TestCase( @"\c:1725176210,t:TER*7A\!AIvDM,1,1,,A,402;bK1vR@wTgPbmd4HVqs700l76,0*7A", TalkerId.MobileStation, VesselDataOrigin.Unreconized, "!AIvDM" )]
    [TestCase( @"\c:1725181228,t:TERA*38\!AIVFM,1,1,,A,33f?cT?P00PfpT8I6=:>4?vl2>`<,0*08", TalkerId.MobileStation, VesselDataOrigin.Unreconized, "!AIVFM" )]
    [TestCase( @"\i:<O>NOR</O>,s:2573105,c:1668997143*00\!B2VDM,1,1,9,A,H3mWb5P5HT4pD00000000000000,2*0A", TalkerId.Unreconized, VesselDataOrigin.Vdm, "!B2VDM" )]
    public async Task Unreconized_talker_id_or_data_origin_is_allowed_when_configured_Async( string line,
                                                                                             TalkerId talkerId,
                                                                                             VesselDataOrigin dataOrigin,
                                                                                             string sentenceFormatter )
    {
        var options = new NmeaParserOptions { AllowUnreconizedDataOrigin = true, AllowUnreconizedTalkerId = true };
        var p = await ParseByLineAsync( Lf( line ), options );
        p.OnNextCalls.Count.ShouldBe( 1 );
        p.OnNextCalls[0].TalkerId.ShouldBe( talkerId );
        p.OnNextCalls[0].DataOrigin.ShouldBe( dataOrigin );
        p.OnNextCalls[0].SentenceFormatter.ShouldBe( sentenceFormatter );
        p.OnErrorCalls.ShouldBeEmpty();
        p.IsComplete.ShouldBeTrue();
    }

    // ChecksumOption.ValidateStandardFormat: a tag block without checksum and a sentence without checksum
    // are both "checksum not present" errors.
    [Test]
    public async Task Missing_checksum_Async()
    {
        var options = new NmeaParserOptions { ChecksumOption = ChecksumOption.ValidateStandardFormat };
        var p = await ParseByLineAsync( Lf( @"\s:rMT9999,c:1747907742\!BSVDM,1,1,,A,33mI?D5P00PkU>BU6kw`:gwB2Dmr,0*09" )
                                        + Lf( @"\s:rMT9999,c:1747907742*5D\!BSVDM,1,1,,A,33mI?D5P00PkU>BU6kw`:gwB2Dmr,0" ),
                                        options );
        // Added: the old scenario only checked the errors, not that no line was reported.
        p.OnNextCalls.ShouldBeEmpty();
        p.OnErrorCalls.Count.ShouldBe( 2 );
        p.OnErrorCalls[0].Error.Message.ShouldBe( "Invalid data. Payload checksum not present - the message may have been corrupted or truncated" );
        p.OnErrorCalls[1].Error.Message.ShouldBe( "Invalid data. Payload checksum not present - the message may have been corrupted or truncated" );
        p.IsComplete.ShouldBeTrue();
    }

    // ChecksumOption.CheckValidity (without ValidateStandardFormat): a checksum that is empty or longer than 2 chars
    // (in the tag block or in the sentence) is an "*X or *XX" format error.
    [TestCase( @"\s:rMT9999,c:1747907742*\!BSVDM,1,1,,A,33mI?D5P00PkU>BU6kw`:gwB2Dmr,0*09" )]
    [TestCase( @"\s:rMT9999,c:1747907742*5DD\!BSVDM,1,1,,A,33mI?D5P00PkU>BU6kw`:gwB2Dmr,0*09" )]
    [TestCase( @"\s:rMT9999,c:1747907742*5D\!BSVDM,1,1,,A,33mI?D5P00PkU>BU6kw`:gwB2Dmr,0*" )]
    [TestCase( @"\s:rMT9999,c:1747907742*5D\!BSVDM,1,1,,A,33mI?D5P00PkU>BU6kw`:gwB2Dmr,0*099" )]
    public async Task Invalid_checksum_format_Async( string line )
    {
        var options = new NmeaParserOptions { ChecksumOption = ChecksumOption.CheckValidity };
        var p = await ParseByLineAsync( Lf( line ), options );
        p.OnErrorCalls.Count.ShouldBe( 1 );
        p.OnErrorCalls[0].Error.Message.ShouldBe( "Line section should end with *X or *XX hexadecimal checksum." );
        p.IsComplete.ShouldBeTrue();
    }

    // ChecksumOption.ValidateStandardFormat: a checksum that is not exactly 2 chars (in the tag block or in the sentence)
    // is a "*XX" format error.
    [TestCase( @"\s:rMT9999,c:1747907742*\!BSVDM,1,1,,A,33mI?D5P00PkU>BU6kw`:gwB2Dmr,0*09" )]
    [TestCase( @"\s:rMT9999,c:1747907742*5\!BSVDM,1,1,,A,33mI?D5P00PkU>BU6kw`:gwB2Dmr,0*09" )]
    [TestCase( @"\s:rMT9999,c:1747907742*5DD\!BSVDM,1,1,,A,33mI?D5P00PkU>BU6kw`:gwB2Dmr,0*09" )]
    [TestCase( @"\s:rMT9999,c:1747907742*5D\!BSVDM,1,1,,A,33mI?D5P00PkU>BU6kw`:gwB2Dmr,0*" )]
    [TestCase( @"\s:rMT9999,c:1747907742*5D\!BSVDM,1,1,,A,33mI?D5P00PkU>BU6kw`:gwB2Dmr,0*0" )]
    [TestCase( @"\s:rMT9999,c:1747907742*5D\!BSVDM,1,1,,A,33mI?D5P00PkU>BU6kw`:gwB2Dmr,0*099" )]
    public async Task Invalid_strict_checksum_format_Async( string line )
    {
        var options = new NmeaParserOptions { ChecksumOption = ChecksumOption.ValidateStandardFormat };
        var p = await ParseByLineAsync( Lf( line ), options );
        p.OnErrorCalls.Count.ShouldBe( 1 );
        p.OnErrorCalls[0].Error.Message.ShouldBe( "Line section should end with *XX hexadecimal checksum." );
        p.IsComplete.ShouldBeTrue();
    }

    // ChecksumOption.AllowInvalidFormat: badly formatted checksums are ignored, the line is reported without error.
    [TestCase( @"\s:rMT9999,c:1747907742*\!BSVDM,1,1,,A,33mI?D5P00PkU>BU6kw`:gwB2Dmr,0*09" )]
    [TestCase( @"\s:rMT9999,c:1747907742*5\!BSVDM,1,1,,A,33mI?D5P00PkU>BU6kw`:gwB2Dmr,0*09" )]
    [TestCase( @"\s:rMT9999,c:1747907742*5DD\!BSVDM,1,1,,A,33mI?D5P00PkU>BU6kw`:gwB2Dmr,0*09" )]
    [TestCase( @"\s:rMT9999,c:1747907742*5D\!BSVDM,1,1,,A,33mI?D5P00PkU>BU6kw`:gwB2Dmr,0*" )]
    [TestCase( @"\s:rMT9999,c:1747907742*5D\!BSVDM,1,1,,A,33mI?D5P00PkU>BU6kw`:gwB2Dmr,0*0" )]
    [TestCase( @"\s:rMT9999,c:1747907742*5D\!BSVDM,1,1,,A,33mI?D5P00PkU>BU6kw`:gwB2Dmr,0*099" )]
    public async Task Invalid_checksum_skipped_Async( string line )
    {
        var options = new NmeaParserOptions { ChecksumOption = ChecksumOption.AllowInvalidFormat };
        var p = await ParseByLineAsync( Lf( line ), options );
        p.OnErrorCalls.ShouldBeEmpty();
        // Added: "no error" alone is satisfied by a parser that silently drops the line. The line must be reported.
        p.OnNextCalls.Count.ShouldBe( 1 );
        p.IsComplete.ShouldBeTrue();
    }

    // ChecksumOption.ValidateStandardFormat | CheckValidity: a non hexadecimal char in a checksum is an error that names it.
    [TestCase( @"\s:rMT9999,c:1747907742*0Z\!BSVDM,1,1,,A,33mI?D5P00PkU>BU6kw`:gwB2Dmr,0*09", 'Z' )]
    [TestCase( @"\s:rMT9999,c:1747907742*H5\!BSVDM,1,1,,A,33mI?D5P00PkU>BU6kw`:gwB2Dmr,0*09", 'H' )]
    [TestCase( @"\s:rMT9999,c:1747907742*5D\!BSVDM,1,1,,A,33mI?D5P00PkU>BU6kw`:gwB2Dmr,0*0X", 'X' )]
    [TestCase( @"\s:rMT9999,c:1747907742*5D\!BSVDM,1,1,,A,33mI?D5P00PkU>BU6kw`:gwB2Dmr,0*O0", 'O' )]
    public async Task Invalid_checksum_value_Async( string line, char invalidChar )
    {
        var options = new NmeaParserOptions { ChecksumOption = ChecksumOption.ValidateStandardFormat | ChecksumOption.CheckValidity };
        var p = await ParseByLineAsync( Lf( line ), options );
        p.OnErrorCalls.Count.ShouldBe( 1 );
        p.OnErrorCalls[0].Error.Message.ShouldBe( $"Section checksum should contains hexadecimal digit but receice '{invalidChar}'." );
        p.IsComplete.ShouldBeTrue();
    }

    // With CheckValidity (strict 2-digit format or not), a checksum that does not match the content is an error
    // with the computed and the expected (read) values. In non strict mode, a 1-digit checksum is read as "0X".
    [TestCase( @"\s:rMT9999,c:1747907742*E3\!BSVDM,1,1,,A,33mI?D5P00PkU>BU6kw`:gwB2Dmr,0*09", "5D", "E3", ChecksumOption.ValidateStandardFormat | ChecksumOption.CheckValidity )] // Intentionally invalid checksum.
    [TestCase( @"\s:rMT9999,c:1747907742*0B\!BSVDM,1,1,,A,33mI?D5P00PkU>BU6kw`:gwB2Dmr,0*09", "5D", "0B", ChecksumOption.ValidateStandardFormat | ChecksumOption.CheckValidity )] // Intentionally invalid checksum.
    [TestCase( @"\s:rMT9999,c:1747907742*5D\!BSVDM,1,1,,A,33mI?D5P00PkU>BU6kw`:gwB2Dmr,0*03", "09", "03", ChecksumOption.ValidateStandardFormat | ChecksumOption.CheckValidity )] // Intentionally invalid checksum.
    [TestCase( @"\s:rMT9999,c:1747907742*5D\!BSVDM,1,1,,A,33mI?D5P00PkU>BU6kw`:gwB2Dmr,0*AA", "09", "AA", ChecksumOption.ValidateStandardFormat | ChecksumOption.CheckValidity )] // Intentionally invalid checksum.
    [TestCase( @"\s:rMT9999,c:1747907742*3\!BSVDM,1,1,,A,33mI?D5P00PkU>BU6kw`:gwB2Dmr,0*09", "5D", "03", ChecksumOption.CheckValidity )]
    [TestCase( @"\s:rMT9999,c:1747907742*B\!BSVDM,1,1,,A,33mI?D5P00PkU>BU6kw`:gwB2Dmr,0*09", "5D", "0B", ChecksumOption.CheckValidity )]
    [TestCase( @"\s:rMT9999,c:1747907742*5D\!BSVDM,1,1,,A,33mI?D5P00PkU>BU6kw`:gwB2Dmr,0*3", "09", "03", ChecksumOption.CheckValidity )]
    [TestCase( @"\s:rMT9999,c:1747907742*5D\!BSVDM,1,1,,A,33mI?D5P00PkU>BU6kw`:gwB2Dmr,0*A", "09", "0A", ChecksumOption.CheckValidity )]
    public async Task Invalid_checksum_result_Async( string line, string result, string expect, ChecksumOption checksumOption )
    {
        var options = new NmeaParserOptions { ChecksumOption = checksumOption };
        var p = await ParseByLineAsync( Lf( line ), options );
        p.OnErrorCalls.Count.ShouldBe( 1 );
        p.OnErrorCalls[0].Error.Message.ShouldBe( $"Section checksum not match checksum. Exepcted {expect} but value is {result}." );
        p.IsComplete.ShouldBeTrue();
    }

    // ChecksumOption.CheckValidity (non strict format): a valid 1-digit checksum ("*9" for 0x09) is accepted.
    [TestCase( @"\s:rMT9999,c:1747907742*5D\!BSVDM,1,1,,A,33mI?D5P00PkU>BU6kw`:gwB2Dmr,0*9" )]
    public async Task Validate_checksum_with_one_digit_Async( string line )
    {
        var options = new NmeaParserOptions { ChecksumOption = ChecksumOption.CheckValidity };
        var p = await ParseByLineAsync( Lf( line ), options );
        p.OnErrorCalls.ShouldBeEmpty();
        // Added: "no error" alone is satisfied by a parser that silently drops the line. The line must be reported.
        p.OnNextCalls.Count.ShouldBe( 1 );
        p.IsComplete.ShouldBeTrue();
    }

    // ChecksumOption.ValidateStandardFormat | CheckValidity: valid 2-digit checksums are accepted.
    [TestCase( @"\s:rMT9999,c:1747907742*5D\!BSVDM,1,1,,A,33mI?D5P00PkU>BU6kw`:gwB2Dmr,0*09" )]
    [TestCase( @"\s:rMT7304,c:1747704784*5A\!AIVDM,1,1,,A,23:q:j0001WaoVN616Vm?hml082e,0*7F" )]
    [TestCase( @"\s:rMT1667,c:1747704784*5C\!AIVDM,1,1,,A,ENkb;0<Hah@@@@@@@@@@@@@@@@@;Wd6F:ewt800003vP003,2*2A" )]
    [TestCase( @"\s:rMT7800,c:1747704786*57\!AIVDM,1,1,,A,8@2<HW@0BkdhF0dcH50h3j8412VrGwdfwwwwwwwwwwwwwwwwwwwwwwwwwt0,2*3F" )]
    [TestCase( @"\s:rMT5465,c:1747908456*56\!AIVDM,1,1,,A,Dh400QAMe>fp,0*2E" )]
    public async Task Validate_checksum_with_two_digits_Async( string line )
    {
        var options = new NmeaParserOptions { ChecksumOption = ChecksumOption.ValidateStandardFormat | ChecksumOption.CheckValidity };
        var p = await ParseByLineAsync( Lf( line ), options );
        p.OnErrorCalls.ShouldBeEmpty();
        // Added: "no error" alone is satisfied by a parser that silently drops the line. The line must be reported.
        p.OnNextCalls.Count.ShouldBe( 1 );
        p.IsComplete.ShouldBeTrue();
    }

    static string Lf( string line ) => line + "\n";

    static string Cr( string line ) => line + "\r";

    static string CrLf( string line ) => line + "\r\n";

    static async Task<LineProcessor> ParseByLineAsync( string content, NmeaParserOptions? options = null )
    {
        var processor = new LineProcessor();
        await NmeaStreamParser.ParseStreamAsync( new MemoryStream( Encoding.ASCII.GetBytes( content ) ),
                                                 processor,
                                                 options ?? new NmeaParserOptions() ).ConfigureAwait( false );
        return processor;
    }

    static void ShouldHaveTheFourRealLines( LineProcessor p )
    {
        p.OnNextCalls.Count.ShouldBe( 4 );
        // ais.kystverket.no
        p.ShouldHaveLine( 0, "s:42,c:1567684904*38", "!AIVDM,1,1,,A,B3m:H900AP@b:79ae6:<OwnUoP06,0*78" );
        // ais.kystverket.no
        p.ShouldHaveLine( 1, "s:3,c:1567692251*01", "!AIVDM,1,1,,A,13m9WS001d0K==pR=D?HB6WD0pJV,0*4D" );
        // ais.kystverket.no
        p.ShouldHaveLine( 2, "s:24,c:1567692878*35", "!AIVDM,1,1,,B,13o`9@701j1Ej3vc;o3q@7SJ0D02,0*3B" );
        // ais.kystverket.no
        p.ShouldHaveLine( 3, "s:772,c:1567693246*07", "!AIVDM,1,1,,,13o7g2001P0Lv<rSdVHf2h3N0000,0*25" );
        p.IsComplete.ShouldBeTrue();
    }

    /// <summary>
    /// Fake <see cref="INmeaLineStreamProcessor{TExtraFieldParser}"/> that records the lines and the errors.
    /// Like <see cref="TestProcessor"/>, it refuses <see cref="OnNext"/> after <see cref="OnCompleted"/>
    /// and a second <see cref="OnCompleted"/>.
    /// </summary>
    sealed class LineProcessor : INmeaLineStreamProcessor<DefaultExtraFieldParser>
    {
        public sealed record Line( string TagBlock, string Sentence, TalkerId TalkerId, VesselDataOrigin DataOrigin, string SentenceFormatter );

        public sealed record ErrorReport( string Line, Exception Error, int LineNumber );

        public bool IsComplete { get; private set; }

        public List<Line> OnNextCalls { get; } = [];

        public List<ErrorReport> OnErrorCalls { get; } = [];

        public void ShouldHaveLine( int index, string tagBlock, string sentence )
        {
            OnNextCalls[index].TagBlock.ShouldBe( tagBlock );
            OnNextCalls[index].Sentence.ShouldBe( sentence );
        }

        public void OnCompleted()
        {
            if( IsComplete )
            {
                throw new InvalidOperationException( $"Must not call {nameof( OnCompleted )} more than once" );
            }
            IsComplete = true;
        }

        public void OnError( in ReadOnlySpan<byte> line, Exception error, int lineNumber )
        {
            OnErrorCalls.Add( new ErrorReport( Encoding.ASCII.GetString( line ), error, lineNumber ) );
        }

        public void OnNext( in NmeaLineParser<DefaultExtraFieldParser> value, int lineNumber )
        {
            if( IsComplete )
            {
                throw new InvalidOperationException( $"Must not call {nameof( OnNext )} after calling {nameof( OnCompleted )}" );
            }
            OnNextCalls.Add( new Line( Encoding.ASCII.GetString( value.TagBlockAsciiWithoutDelimiters ),
                                       Encoding.ASCII.GetString( value.Sentence ),
                                       value.AisTalker,
                                       value.DataOrigin,
                                       Encoding.ASCII.GetString( value.SentenceFormatter ) ) );
        }

        public void Progress( bool done, int totalLines, int totalTicks, int linesSinceLastUpdate, int ticksSinceLastUpdate )
        {
            // Progress is not tested at the line level.
        }
    }
}
