// <copyright file="NmeaStreamParserByMessageTests.cs" company="Endjin Limited">
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
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Ais.Net.Tests;

// NmeaStreamParser.ParseStreamAsync with a NmeaLineToAisStreamAdapter over a TestProcessor: the stream is read
// line by line (LF, CR, CRLF or an unterminated last line, blank lines skipped) and each AIS message is reported
// through INmeaAisMessageStreamProcessor.OnNext (payload and padding). Invalid lines (not NMEA, truncated, missing
// padding or checksum, unknown or unsupported tag block fields) are reported through OnError with the line,
// the exception and the 1-based line number. OnCompleted is always called once at the end.
// ThrowWhenTagBlockContainsUnknownFields = false makes the unknown/unsupported tag block fields acceptable.
[TestFixture]
public class NmeaStreamParserByMessageTests
{
    // ais.kystverket.no
    const string RealLine0 = @"\s:42,c:1567684904*38\!AIVDM,1,1,,A,B3m:H900AP@b:79ae6:<OwnUoP06,0*78";
    // ais.kystverket.no
    const string RealLine1 = @"\s:3,c:1567692251*01\!AIVDM,1,1,,A,13m9WS001d0K==pR=D?HB6WD0pJV,0*4D";
    // ais.kystverket.no
    const string RealLine2 = @"\s:24,c:1567692878*35\!AIVDM,1,1,,B,13o`9@701j1Ej3vc;o3q@7SJ0D02,0*3B";
    // ais.kystverket.no
    const string RealLine3 = @"\s:772,c:1567693246*07\!AIVDM,1,1,,,13o7g2001P0Lv<rSdVHf2h3N0000,0*25";

    // An empty stream: no message, OnCompleted called.
    [Test]
    public async Task Empty_file_Async()
    {
        var p = await ParseByMessageAsync( "" );
        p.OnNextCalls.ShouldBeEmpty();
        p.IsComplete.ShouldBeTrue();
    }

    // A single blank CRLF line: no message, OnCompleted called.
    [Test]
    public async Task Single_CRLF_blank_line_only_Async()
    {
        var p = await ParseByMessageAsync( CrLf( "" ) );
        p.OnNextCalls.ShouldBeEmpty();
        p.IsComplete.ShouldBeTrue();
    }

    // A single blank LF line: no message, OnCompleted called.
    [Test]
    public async Task Single_LF_blank_line_only_Async()
    {
        var p = await ParseByMessageAsync( Lf( "" ) );
        p.OnNextCalls.ShouldBeEmpty();
        p.IsComplete.ShouldBeTrue();
    }

    // A single blank CR line: no message, OnCompleted called.
    [Test]
    public async Task Single_CR_blank_line_only_Async()
    {
        var p = await ParseByMessageAsync( Cr( "" ) );
        p.OnNextCalls.ShouldBeEmpty();
        p.IsComplete.ShouldBeTrue();
    }

    // Three blank CRLF lines: no message, OnCompleted called.
    [Test]
    public async Task Multiple_CRLF_blank_lines_only_Async()
    {
        var p = await ParseByMessageAsync( CrLf( "" ) + CrLf( "" ) + CrLf( "" ) );
        p.OnNextCalls.ShouldBeEmpty();
        p.IsComplete.ShouldBeTrue();
    }

    // Three blank LF lines: no message, OnCompleted called.
    [Test]
    public async Task Multiple_LF_blank_lines_only_Async()
    {
        var p = await ParseByMessageAsync( Lf( "" ) + Lf( "" ) + Lf( "" ) );
        p.OnNextCalls.ShouldBeEmpty();
        p.IsComplete.ShouldBeTrue();
    }

    // Blank lines with mixed terminators (CRLF, LF, CRLF, CR): no message, OnCompleted called.
    [Test]
    public async Task Multiple_mixed_blank_lines_only_Async()
    {
        var p = await ParseByMessageAsync( CrLf( "" ) + Lf( "" ) + CrLf( "" ) + Cr( "" ) );
        p.OnNextCalls.ShouldBeEmpty();
        p.IsComplete.ShouldBeTrue();
    }

    // One LF line: one message with its payload and padding.
    [Test]
    public async Task Single_line_Async()
    {
        var p = await ParseByMessageAsync( Lf( RealLine0 ) );
        p.OnNextCalls.Count.ShouldBe( 1 );
        // ais.kystverket.no
        ShouldHavePayload( p, 0, "B3m:H900AP@b:79ae6:<OwnUoP06", 0 );
        p.IsComplete.ShouldBeTrue();
    }

    // One line without terminator: the message is reported like with a terminated line.
    [Test]
    public async Task Single_line_without_newline_only_Async()
    {
        var p = await ParseByMessageAsync( RealLine0 );
        p.OnNextCalls.Count.ShouldBe( 1 );
        // ais.kystverket.no
        ShouldHavePayload( p, 0, "B3m:H900AP@b:79ae6:<OwnUoP06", 0 );
        p.IsComplete.ShouldBeTrue();
    }

    // Four LF lines: the four messages are reported in order.
    [Test]
    public async Task Multiple_lines_Async()
    {
        var p = await ParseByMessageAsync( Lf( RealLine0 ) + Lf( RealLine1 ) + Lf( RealLine2 ) + Lf( RealLine3 ) );
        ShouldHaveTheFourRealMessages( p );
    }

    // Four CR lines: the four messages are reported in order.
    [Test]
    public async Task Multiple_CR_lines_Async()
    {
        var p = await ParseByMessageAsync( Cr( RealLine0 ) + Cr( RealLine1 ) + Cr( RealLine2 ) + Cr( RealLine3 ) );
        ShouldHaveTheFourRealMessages( p );
    }

    // Four lines, the last one unterminated: the four messages are reported in order.
    [Test]
    public async Task Multiple_lines_where_final_line_has_no_newline_Async()
    {
        var p = await ParseByMessageAsync( Lf( RealLine0 ) + Lf( RealLine1 ) + Lf( RealLine2 ) + RealLine3 );
        ShouldHaveTheFourRealMessages( p );
    }

    // Three blank lines then four lines: the blank lines are skipped.
    [Test]
    public async Task Multiple_lines_with_blanks_at_start_Async()
    {
        var p = await ParseByMessageAsync( Lf( "" ) + Lf( "" ) + Lf( "" )
                                           + Lf( RealLine0 ) + Lf( RealLine1 ) + Lf( RealLine2 ) + Lf( RealLine3 ) );
        ShouldHaveTheFourRealMessages( p );
    }

    // Three blank lines between the 2nd and the 3rd line: the blank lines are skipped.
    [Test]
    public async Task Multiple_lines_with_blanks_in_middle_Async()
    {
        var p = await ParseByMessageAsync( Lf( RealLine0 ) + Lf( RealLine1 )
                                           + Lf( "" ) + Lf( "" ) + Lf( "" )
                                           + Lf( RealLine2 ) + Lf( RealLine3 ) );
        ShouldHaveTheFourRealMessages( p );
    }

    // Four lines then three blank lines: the blank lines are skipped.
    [Test]
    public async Task Multiple_lines_with_blanks_at_end_Async()
    {
        var p = await ParseByMessageAsync( Lf( RealLine0 ) + Lf( RealLine1 ) + Lf( RealLine2 ) + Lf( RealLine3 )
                                           + Lf( "" ) + Lf( "" ) + Lf( "" ) );
        ShouldHaveTheFourRealMessages( p );
    }

    // A line that is not NMEA: no message, one error with the line, the "expected '!'" exception and line number 1.
    [Test]
    public async Task Single_unparseable_line_Async()
    {
        var p = await ParseByMessageAsync( Lf( "I am not an NMEA message" ) );
        p.OnNextCalls.ShouldBeEmpty();
        ShouldHaveSingleError( p, "I am not an NMEA message", "Invalid data. Expected '!' at sentence start", 1 );
        p.IsComplete.ShouldBeTrue();
    }

    // A line truncated in its payload: no message, one "message appears to be missing some characters" error.
    [Test]
    public async Task Single_truncated_line_Async()
    {
        // ais.kystverket.no
        const string line = @"\s:42,c:1567684904*38\!AIVDM,1,1,,A,B";
        var p = await ParseByMessageAsync( Lf( line ) );
        p.OnNextCalls.ShouldBeEmpty();
        ShouldHaveSingleError( p, line, "Invalid data. The message appears to be missing some characters - it may have been corrupted or truncated.", 1 );
        p.IsComplete.ShouldBeTrue();
    }

    // A line without padding field: no message, one "padding field not present" error.
    [Test]
    public async Task Single_line_where_padding_is_missing_Async()
    {
        // ais.kystverket.no
        const string line = @"\s:42,c:1567684904*38\!AIVDM,1,1,,A,B3m:H900AP@b:79ae6:<OwnUoP06*78";
        var p = await ParseByMessageAsync( Lf( line ) );
        p.OnNextCalls.ShouldBeEmpty();
        ShouldHaveSingleError( p, line, "Invalid data. Payload padding field not present - the message may have been corrupted or truncated", 1 );
        p.IsComplete.ShouldBeTrue();
    }

    // A line with an empty padding field (",*78"): no message, one "padding field not present" error.
    [Test]
    public async Task Single_line_where_padding_comma_present_but_value_missing_Async()
    {
        // ais.kystverket.no
        const string line = @"\s:42,c:1567684904*38\!AIVDM,1,1,,A,B3m:H900AP@b:79ae6:<OwnUoP06,*78";
        var p = await ParseByMessageAsync( Lf( line ) );
        p.OnNextCalls.ShouldBeEmpty();
        ShouldHaveSingleError( p, line, "Invalid data. Payload padding field not present - the message may have been corrupted or truncated", 1 );
        p.IsComplete.ShouldBeTrue();
    }

    // A line that ends right after the comma that precedes the padding: no message, one "padding field not present" error.
    [Test]
    public async Task Single_line_where_line_truncated_at_padding_comma_Async()
    {
        // ais.kystverket.no
        const string line = @"\s:42,c:1567684904*38\!AIVDM,1,1,,A,B3m:H900AP@b:79ae6:<OwnUoP06,";
        var p = await ParseByMessageAsync( Lf( line ) );
        p.OnNextCalls.ShouldBeEmpty();
        ShouldHaveSingleError( p, line, "Invalid data. Payload padding field not present - the message may have been corrupted or truncated", 1 );
        p.IsComplete.ShouldBeTrue();
    }

    // A line without sentence checksum: no message, one "checksum not present" error.
    [Test]
    public async Task Single_line_where_checksum_is_missing_Async()
    {
        // ais.kystverket.no
        const string line = @"\s:42,c:1567684904*38\!AIVDM,1,1,,A,B3m:H900AP@b:79ae6:<OwnUoP06,0";
        var p = await ParseByMessageAsync( Lf( line ) );
        p.OnNextCalls.ShouldBeEmpty();
        ShouldHaveSingleError( p, line, "Invalid data. Payload checksum not present - the message may have been corrupted or truncated", 1 );
        p.IsComplete.ShouldBeTrue();
    }

    // A good line then a line that is not NMEA: the message is reported, the bad line is an error on line 2.
    [Test]
    public async Task One_unparseable_line_and_one_good_line_Async()
    {
        var p = await ParseByMessageAsync( Lf( RealLine0 ) + Lf( "I am not an NMEA message" ) );
        p.OnNextCalls.Count.ShouldBe( 1 );
        // ais.kystverket.no
        ShouldHavePayload( p, 0, "B3m:H900AP@b:79ae6:<OwnUoP06", 0 );
        ShouldHaveSingleError( p, "I am not an NMEA message", "Invalid data. Expected '!' at sentence start", 2 );
        p.IsComplete.ShouldBeTrue();
    }

    // A tag block with the non-standard "q:u" field and default options (ThrowWhenTagBlockContainsUnknownFields = true):
    // no message, one ArgumentException "Unknown field type:..." error.
    [Test]
    public async Task Single_line_with_non_standard_tag_block_field_with_exceptions_enabled_Async()
    {
        // ais.kystverket.no
        const string line = @"\s:42,c:1567684904,q:u*2A\!AIVDM,1,1,,A,B3m:H900AP@b:79ae6:<OwnUoP06,0*78";
        var p = await ParseByMessageAsync( Lf( line ) );
        p.OnNextCalls.ShouldBeEmpty();
        p.OnErrorCalls.Count.ShouldBe( 1 );
        p.OnErrorCalls[0].Line.ShouldBe( line );
        p.OnErrorCalls[0].Error.ShouldBeAssignableTo<ArgumentException>()!.Message.ShouldStartWith( "Unknown field type:", caseSensitivity: Case.Sensitive );
        p.OnErrorCalls[0].LineNumber.ShouldBe( 1 );
        p.IsComplete.ShouldBeTrue();
    }

    // Same line with ThrowWhenTagBlockContainsUnknownFields = false: the message is reported.
    [Test]
    public async Task Single_line_with_non_standard_tag_block_field_with_exceptions_disabled_Async()
    {
        // ais.kystverket.no
        var p = await ParseByMessageAsync( Lf( @"\s:42,c:1567684904,q:u*2A\!AIVDM,1,1,,A,B3m:H900AP@b:79ae6:<OwnUoP06,0*78" ),
                                           new NmeaParserOptions { ThrowWhenTagBlockContainsUnknownFields = false } );
        p.OnNextCalls.Count.ShouldBe( 1 );
        // ais.kystverket.no
        ShouldHavePayload( p, 0, "B3m:H900AP@b:79ae6:<OwnUoP06", 0 );
        // Added: the old scenario did not check that nothing was reported as an error.
        p.OnErrorCalls.ShouldBeEmpty();
        p.IsComplete.ShouldBeTrue();
    }

    // A tag block with the standard but unsupported "n:1" field and default options: no message,
    // one NotSupportedException "Unsupported field type:..." error.
    [Test]
    public async Task Single_line_with_standard_but_unsupported_tag_block_field_with_exceptions_enabled_Async()
    {
        // ais.kystverket.no
        const string line = @"\s:42,c:1567684904,n:1*71\!AIVDM,1,1,,A,B3m:H900AP@b:79ae6:<OwnUoP06,0*78";
        var p = await ParseByMessageAsync( Lf( line ) );
        p.OnNextCalls.ShouldBeEmpty();
        p.OnErrorCalls.Count.ShouldBe( 1 );
        p.OnErrorCalls[0].Line.ShouldBe( line );
        p.OnErrorCalls[0].Error.ShouldBeAssignableTo<NotSupportedException>()!.Message.ShouldStartWith( "Unsupported field type:", caseSensitivity: Case.Sensitive );
        p.OnErrorCalls[0].LineNumber.ShouldBe( 1 );
        p.IsComplete.ShouldBeTrue();
    }

    // Same line with ThrowWhenTagBlockContainsUnknownFields = false: the message is reported.
    [Test]
    public async Task Single_line_with_standard_but_unsupported_tag_block_field_with_exceptions_disabled_Async()
    {
        // ais.kystverket.no
        var p = await ParseByMessageAsync( Lf( @"\s:42,c:1567684904,n:1*71\!AIVDM,1,1,,A,B3m:H900AP@b:79ae6:<OwnUoP06,0*78" ),
                                           new NmeaParserOptions { ThrowWhenTagBlockContainsUnknownFields = false } );
        p.OnNextCalls.Count.ShouldBe( 1 );
        // ais.kystverket.no
        ShouldHavePayload( p, 0, "B3m:H900AP@b:79ae6:<OwnUoP06", 0 );
        // Added: the old scenario did not check that nothing was reported as an error.
        p.OnErrorCalls.ShouldBeEmpty();
        p.IsComplete.ShouldBeTrue();
    }

    static string Lf( string line ) => line + "\n";

    static string Cr( string line ) => line + "\r";

    static string CrLf( string line ) => line + "\r\n";

    // Fixed: the old "When I parse the content by message" step created the adapter and called ParseStreamAsync
    // without the NmeaParserOptions configured by the "Given" steps: any configured option (like ChecksumOption)
    // was silently ignored and the defaults were used. The options are now given to both the adapter and the parser.
    // (No scenario of this feature configures an option through a "Given" step: only the "with exceptions disabled"
    // step used its own options, which it did pass correctly. So this fix changes no expectation here.)
    // The old step also never disposed the adapter.
    static async Task<TestProcessor> ParseByMessageAsync( string content, NmeaParserOptions? options = null )
    {
        options ??= new NmeaParserOptions();
        var processor = new TestProcessor();
        using var adapter = new NmeaLineToAisStreamAdapter<DefaultExtraFieldParser>( processor, options );
        await NmeaStreamParser.ParseStreamAsync( new MemoryStream( Encoding.ASCII.GetBytes( content ) ),
                                                 adapter,
                                                 options ).ConfigureAwait( false );
        return processor;
    }

    static void ShouldHavePayload( TestProcessor p, int index, string payload, uint padding )
    {
        p.OnNextCalls[index].AsciiPayload.ShouldBe( payload );
        p.OnNextCalls[index].Padding.ShouldBe( padding );
    }

    static void ShouldHaveSingleError( TestProcessor p, string line, string argumentExceptionMessage, int lineNumber )
    {
        p.OnErrorCalls.Count.ShouldBe( 1 );
        p.OnErrorCalls[0].Line.ShouldBe( line );
        p.OnErrorCalls[0].Error.ShouldBeAssignableTo<ArgumentException>()!.Message.ShouldBe( argumentExceptionMessage );
        p.OnErrorCalls[0].LineNumber.ShouldBe( lineNumber );
    }

    static void ShouldHaveTheFourRealMessages( TestProcessor p )
    {
        p.OnNextCalls.Count.ShouldBe( 4 );
        // ais.kystverket.no
        ShouldHavePayload( p, 0, "B3m:H900AP@b:79ae6:<OwnUoP06", 0 );
        // ais.kystverket.no
        ShouldHavePayload( p, 1, "13m9WS001d0K==pR=D?HB6WD0pJV", 0 );
        // ais.kystverket.no
        ShouldHavePayload( p, 2, "13o`9@701j1Ej3vc;o3q@7SJ0D02", 0 );
        // ais.kystverket.no
        ShouldHavePayload( p, 3, "13o7g2001P0Lv<rSdVHf2h3N0000", 0 );
        p.IsComplete.ShouldBeTrue();
    }
}
