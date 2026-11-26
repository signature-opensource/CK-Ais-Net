// <copyright file="NmeaLineToAisStreamAdapterTests.cs" company="Endjin Limited">
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

namespace Ais.Net.Tests;

// NmeaLineToAisStreamAdapter receives NMEA lines (as an INmeaLineStreamProcessor) and gives complete AIS messages
// to an INmeaAisMessageStreamProcessor (here a TestProcessor):
// - non-fragmented messages are forwarded as is;
// - multi-fragment messages are reassembled, the fragments being grouped by the tag block "g:" grouping when present,
//   by the AIS sentence group id otherwise; interleaved messages are supported, within MaximumUnmatchedFragmentAge
//   lines, after which the incomplete message is an error;
// - the reassembled message has the payload of all the fragments, the padding of the last one and the tag block of
//   the first line (source, timestamp, grouping);
// - errors reported by the line parser are forwarded, and the adapter reports its own errors (padding on a non-last
//   fragment, duplicated fragment, incomplete message);
// - progress reports are forwarded with the number of AIS messages added;
// - the EmptyGroupTolerance, AllowNonZeroPaddingInNonLastFragment and ThrowWhenNoExclamationMark options.
// Each test ends with AdapterHarness.Complete() that calls the adapter's OnCompleted (the old SpecFlow steps did it
// implicitly in their Dispose) and checks that the processor has been completed exactly once (TestProcessor checks
// that OnNext is never called after OnCompleted and that OnCompleted is not called twice).
[TestFixture]
public class NmeaLineToAisStreamAdapterTests
{
    // A non-fragmented line: one message with its payload, padding, source and timestamp.
    [Test]
    public void non_fragmented_message_received()
    {
        var h = new AdapterHarness();
        // ais.kystverket.no
        h.Receive( @"\s:42,c:1567684904*38\!AIVDM,1,1,,B,33m9UtPP@50wwE:VJW6LS67H01<@,0*3C" );
        var p = h.Complete();
        p.OnNextCalls.Count.ShouldBe( 1 );
        ShouldBeMessage( p.OnNextCalls[0], "33m9UtPP@50wwE:VJW6LS67H01<@", 0, 42, 1567684904 );
    }

    // Only the first fragment (with tag block grouping) of a 2-fragment message: no message.
    [Test]
    public void first_fragment_of_two_part_message_received()
    {
        var h = new AdapterHarness();
        // ais.kystverket.no
        h.Receive( @"\g:1-2-8055,s:99,c:1567685556*4E\!AIVDM,2,1,6,B,53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4l,0*72" );
        var p = h.Complete();
        p.OnNextCalls.ShouldBeEmpty();
    }

    // Only the first fragment (without tag block grouping) of a 2-fragment message: no message.
    [Test]
    public void first_fragment_of_two_part_message_without_grouping_in_header_received()
    {
        var h = new AdapterHarness();
        // ais.kystverket.no
        h.Receive( @"\s:99,c:1567685556*4E\!AIVDM,2,1,6,B,53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4l,0*72" );
        var p = h.Complete();
        p.OnNextCalls.ShouldBeEmpty();
    }

    // The 2 fragments (with tag block grouping) one after the other: one reassembled message
    // (padding of the last fragment, source and timestamp of the first line).
    [Test]
    public void two_fragment_message_fragments_received_adjacently()
    {
        var h = new AdapterHarness();
        // ais.kystverket.no
        h.Receive( @"\g:1-2-8055,s:99,c:1567685556*4E\!AIVDM,2,1,6,B,53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4l,0*72" );
        // ais.kystverket.no
        h.Receive( @"\g:2-2-8055*55\!AIVDM,2,2,6,B,j`888888880,2*2B" );
        var p = h.Complete();
        p.OnNextCalls.Count.ShouldBe( 1 );
        // ais.kystverket.no
        ShouldBeMessage( p.OnNextCalls[0], "53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4lj`888888880", 2, 99, 1567685556 );
    }

    // The 2 fragments (without tag block grouping) one after the other: one reassembled message.
    [Test]
    public void two_fragment_message_fragments_without_grouping_in_header_received_adjacently()
    {
        var h = new AdapterHarness();
        // ais.kystverket.no
        h.Receive( @"\s:99,c:1567685556*4E\!AIVDM,2,1,6,B,53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4l,0*72" );
        // ais.kystverket.no
        h.Receive( @"\s:99*55\!AIVDM,2,2,6,B,j`888888880,2*2B" );
        var p = h.Complete();
        p.OnNextCalls.Count.ShouldBe( 1 );
        // ais.kystverket.no
        ShouldBeMessage( p.OnNextCalls[0], "53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4lj`888888880", 2, 99, 1567685556 );
    }

    // A single-fragment message between the 2 fragments (with tag block grouping): the single one comes first,
    // then the reassembled one.
    [Test]
    public void two_fragment_message_fragments_received_with_single_fragment_message_in_the_middle()
    {
        var h = new AdapterHarness();
        // ais.kystverket.no
        h.Receive( @"\g:1-2-8055,s:99,c:1567685556*4E\!AIVDM,2,1,6,B,53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4l,0*72" );
        // ais.kystverket.no
        h.Receive( @"\s:42,c:1567684904*38\!AIVDM,1,1,,B,33m9UtPP@50wwE:VJW6LS67H01<@,0*3C" );
        // ais.kystverket.no
        h.Receive( @"\g:2-2-8055*55\!AIVDM,2,2,6,B,j`888888880,2*2B" );
        var p = h.Complete();
        p.OnNextCalls.Count.ShouldBe( 2 );
        // ais.kystverket.no
        ShouldBeMessage( p.OnNextCalls[0], "33m9UtPP@50wwE:VJW6LS67H01<@", 0, 42, 1567684904 );
        // ais.kystverket.no
        ShouldBeMessage( p.OnNextCalls[1], "53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4lj`888888880", 2, 99, 1567685556 );
    }

    // A single-fragment message between the 2 fragments (without tag block grouping): the single one comes first,
    // then the reassembled one.
    [Test]
    public void two_fragment_message_fragments_without_grouping_in_header_received_with_single_fragment_message_in_the_middle()
    {
        var h = new AdapterHarness();
        // ais.kystverket.no
        h.Receive( @"\s:99,c:1567685556*4E\!AIVDM,2,1,6,B,53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4l,0*72" );
        // ais.kystverket.no
        h.Receive( @"\s:42,c:1567684904*38\!AIVDM,1,1,,B,33m9UtPP@50wwE:VJW6LS67H01<@,0*3C" );
        // ais.kystverket.no
        h.Receive( @"\s:99*55\!AIVDM,2,2,6,B,j`888888880,2*2B" );
        var p = h.Complete();
        p.OnNextCalls.Count.ShouldBe( 2 );
        // ais.kystverket.no
        ShouldBeMessage( p.OnNextCalls[0], "33m9UtPP@50wwE:VJW6LS67H01<@", 0, 42, 1567684904 );
        // ais.kystverket.no
        ShouldBeMessage( p.OnNextCalls[1], "53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4lj`888888880", 2, 99, 1567685556 );
    }

    // The 3 fragments (with tag block grouping) one after the other: one reassembled message.
    [Test]
    public void three_fragment_message_fragments_received_adjacently()
    {
        var h = new AdapterHarness();
        // ais.kystverket.no
        h.Receive( @"\g:1-3-3451,s:27,c:1567686150*40\!AIVDM,3,1,9,A,544MR0827oeaD<u0000lDdP4pTf0duAA,0*17" );
        // ais.kystverket.no
        h.Receive( @"\g:2-3-3451*5F\!AIVDM,3,2,9,A,<uH000167pF=2=nG0:0DRj0CQiC4jh00,0*4A" );
        // ais.kystverket.no
        h.Receive( @"\g:3-3-3451*5E\!AIVDM,3,3,9,A,0000000,0*2F" );
        var p = h.Complete();
        p.OnNextCalls.Count.ShouldBe( 1 );
        // ais.kystverket.no
        ShouldBeMessage( p.OnNextCalls[0], "544MR0827oeaD<u0000lDdP4pTf0duAA<uH000167pF=2=nG0:0DRj0CQiC4jh000000000", 0, 27, 1567686150 );
    }

    // The 3 fragments (without tag block grouping) one after the other: one reassembled message.
    [Test]
    public void three_fragment_message_fragments_without_grouping_in_header_received_adjacently()
    {
        var h = new AdapterHarness();
        // ais.kystverket.no
        h.Receive( @"\s:27,c:1567686150*40\!AIVDM,3,1,9,A,544MR0827oeaD<u0000lDdP4pTf0duAA,0*17" );
        // ais.kystverket.no
        h.Receive( @"\s:27*5F\!AIVDM,3,2,9,A,<uH000167pF=2=nG0:0DRj0CQiC4jh00,0*4A" );
        // ais.kystverket.no
        h.Receive( @"\s:27*5E\!AIVDM,3,3,9,A,0000000,0*2F" );
        var p = h.Complete();
        p.OnNextCalls.Count.ShouldBe( 1 );
        // ais.kystverket.no
        ShouldBeMessage( p.OnNextCalls[0], "544MR0827oeaD<u0000lDdP4pTf0duAA<uH000167pF=2=nG0:0DRj0CQiC4jh000000000", 0, 27, 1567686150 );
    }

    // A 3-fragment and a 2-fragment message interleaved (with tag block grouping): the 2-fragment one completes
    // first and is reported first.
    [Test]
    public void interleaved_multi_fragment_messages()
    {
        var h = new AdapterHarness();
        // ais.kystverket.no
        h.Receive( @"\g:1-3-3451,s:27,c:1567686150*40\!AIVDM,3,1,9,A,544MR0827oeaD<u0000lDdP4pTf0duAA,0*17" );
        // ais.kystverket.no
        h.Receive( @"\g:1-2-8055,s:99,c:1567685556*4E\!AIVDM,2,1,6,B,53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4l,0*72" );
        // ais.kystverket.no
        h.Receive( @"\g:2-3-3451*5F\!AIVDM,3,2,9,A,<uH000167pF=2=nG0:0DRj0CQiC4jh00,0*4A" );
        // ais.kystverket.no
        h.Receive( @"\g:2-2-8055*55\!AIVDM,2,2,6,B,j`888888880,2*2B" );
        // ais.kystverket.no
        h.Receive( @"\g:3-3-3451*5E\!AIVDM,3,3,9,A,0000000,0*2F" );
        var p = h.Complete();
        p.OnNextCalls.Count.ShouldBe( 2 );
        // ais.kystverket.no
        ShouldBeMessage( p.OnNextCalls[0], "53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4lj`888888880", 2, 99, 1567685556 );
        // ais.kystverket.no
        ShouldBeMessage( p.OnNextCalls[1], "544MR0827oeaD<u0000lDdP4pTf0duAA<uH000167pF=2=nG0:0DRj0CQiC4jh000000000", 0, 27, 1567686150 );
    }

    // A 3-fragment and a 2-fragment message interleaved (without tag block grouping, the AIS group ids 9 and 6
    // distinguish them): the 2-fragment one completes first and is reported first.
    [Test]
    public void interleaved_multi_fragment_messages_without_grouping_in_header()
    {
        var h = new AdapterHarness();
        // ais.kystverket.no
        h.Receive( @"\s:27,c:1567686150*40\!AIVDM,3,1,9,A,544MR0827oeaD<u0000lDdP4pTf0duAA,0*17" );
        // ais.kystverket.no
        h.Receive( @"\s:99,c:1567685556*4E\!AIVDM,2,1,6,B,53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4l,0*72" );
        // ais.kystverket.no
        h.Receive( @"\s:27,*5F\!AIVDM,3,2,9,A,<uH000167pF=2=nG0:0DRj0CQiC4jh00,0*4A" );
        // ais.kystverket.no
        h.Receive( @"\s:99,*55\!AIVDM,2,2,6,B,j`888888880,2*2B" );
        // ais.kystverket.no
        h.Receive( @"\s:27*5E\!AIVDM,3,3,9,A,0000000,0*2F" );
        var p = h.Complete();
        p.OnNextCalls.Count.ShouldBe( 2 );
        // ais.kystverket.no
        ShouldBeMessage( p.OnNextCalls[0], "53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4lj`888888880", 2, 99, 1567685556 );
        // ais.kystverket.no
        ShouldBeMessage( p.OnNextCalls[1], "544MR0827oeaD<u0000lDdP4pTf0duAA<uH000167pF=2=nG0:0DRj0CQiC4jh000000000", 0, 27, 1567686150 );
    }

    // A 3-fragment and a 2-fragment message interleaved, both with the same AIS group id 9: only the tag block
    // grouping (3451 and 8055) distinguishes them, and it takes precedence.
    [Test]
    public void interleaved_multi_fragment_messages_distinguished_only_by_grouping_in_header()
    {
        var h = new AdapterHarness();
        // ais.kystverket.no
        h.Receive( @"\g:1-3-3451,s:27,c:1567686150*40\!AIVDM,3,1,9,A,544MR0827oeaD<u0000lDdP4pTf0duAA,0*17" );
        // ais.kystverket.no
        h.Receive( @"\g:1-2-8055,s:99,c:1567685556*4E\!AIVDM,2,1,9,B,53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4l,0*72" );
        // ais.kystverket.no
        h.Receive( @"\g:2-3-3451*5F\!AIVDM,3,2,9,A,<uH000167pF=2=nG0:0DRj0CQiC4jh00,0*4A" );
        // ais.kystverket.no
        h.Receive( @"\g:2-2-8055*55\!AIVDM,2,2,9,B,j`888888880,2*2B" );
        // ais.kystverket.no
        h.Receive( @"\g:3-3-3451*5E\!AIVDM,3,3,9,A,0000000,0*2F" );
        var p = h.Complete();
        p.OnNextCalls.Count.ShouldBe( 2 );
        // ais.kystverket.no
        ShouldBeMessage( p.OnNextCalls[0], "53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4lj`888888880", 2, 99, 1567685556 );
        // ais.kystverket.no
        ShouldBeMessage( p.OnNextCalls[1], "544MR0827oeaD<u0000lDdP4pTf0duAA<uH000167pF=2=nG0:0DRj0CQiC4jh000000000", 0, 27, 1567686150 );
    }

    // MaximumUnmatchedFragmentAge = 10 (this interleaves more than ever seen in reality): 8 single-fragment messages
    // interleaved with a 3-fragment and a 2-fragment message give 10 messages in completion order.
    [Test]
    public void interleaved_single_and_multi_fragment_messages()
    {
        var h = new AdapterHarness( new NmeaParserOptions { MaximumUnmatchedFragmentAge = 10 } );
        // ais.kystverket.no
        h.Receive( @"\s:42,c:1567684904*38\!AIVDM,1,1,,B,33m9UtPP@50wwE:VJW6LS67H01<@,0*3C" );
        // ais.kystverket.no
        h.Receive( @"\g:1-3-3451,s:27,c:1567686150*40\!AIVDM,3,1,9,A,544MR0827oeaD<u0000lDdP4pTf0duAA,0*17" );
        // ais.kystverket.no
        h.Receive( @"\s:42,c:1567684904*38\!AIVDM,1,1,,A,B3m:H900AP@b:79ae6:<OwnUoP06,0*78" );
        // ais.kystverket.no
        h.Receive( @"\s:3,c:1567692251*01\!AIVDM,1,1,,A,13m9WS001d0K==pR=D?HB6WD0pJV,0*54" );
        // ais.kystverket.no
        h.Receive( @"\g:1-2-8055,s:99,c:1567685556*4E\!AIVDM,2,1,6,B,53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4l,0*72" );
        // ais.kystverket.no
        h.Receive( @"\s:24,c:1567692878*35\!AIVDM,1,1,,B,13o`9@701j1Ej3vc;o3q@7SJ0D02,0*21" );
        // ais.kystverket.no
        h.Receive( @"\g:2-3-3451*5F\!AIVDM,3,2,9,A,<uH000167pF=2=nG0:0DRj0CQiC4jh00,0*4A" );
        // ais.kystverket.no
        h.Receive( @"\s:67,c:1567693000*34\!AIVDM,1,1,,,3CnWHf50000ga40TCHE0D0@R003B,0*4B" );
        // ais.kystverket.no
        h.Receive( @"\s:772,c:1567693246*07\!AIVDM,1,1,,,13o7g2001P0Lv<rSdVHf2h3N0000,0*25" );
        // ais.kystverket.no
        h.Receive( @"\g:2-2-8055*55\!AIVDM,2,2,6,B,j`888888880,2*2B" );
        // ais.kystverket.no
        h.Receive( @"\s:722,c:1567693372*04\!AIVDM,1,1,,A,13m63A?P00P`@GFTK3s>4?wR20Sf,0*71" );
        // ais.kystverket.no
        h.Receive( @"\g:3-3-3451*5E\!AIVDM,3,3,9,A,0000000,0*2F" );
        // ais.kystverket.no
        h.Receive( @"\s:808,c:1567693618*0A\!AIVDM,1,1,,B,B3o8B<00F8:0h694gOtbgwqUoP06,0*73" );
        var p = h.Complete();
        p.OnNextCalls.Count.ShouldBe( 10 );
        // ais.kystverket.no
        ShouldBeMessage( p.OnNextCalls[0], "33m9UtPP@50wwE:VJW6LS67H01<@", 0, 42, 1567684904 );
        // ais.kystverket.no
        ShouldBeMessage( p.OnNextCalls[1], "B3m:H900AP@b:79ae6:<OwnUoP06", 0, 42, 1567684904 );
        // ais.kystverket.no
        ShouldBeMessage( p.OnNextCalls[2], "13m9WS001d0K==pR=D?HB6WD0pJV", 0, 3, 1567692251 );
        // ais.kystverket.no
        ShouldBeMessage( p.OnNextCalls[3], "13o`9@701j1Ej3vc;o3q@7SJ0D02", 0, 24, 1567692878 );
        // ais.kystverket.no
        ShouldBeMessage( p.OnNextCalls[4], "3CnWHf50000ga40TCHE0D0@R003B", 0, 67, 1567693000 );
        // ais.kystverket.no
        ShouldBeMessage( p.OnNextCalls[5], "13o7g2001P0Lv<rSdVHf2h3N0000", 0, 772, 1567693246 );
        // ais.kystverket.no
        ShouldBeMessage( p.OnNextCalls[6], "53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4lj`888888880", 2, 99, 1567685556 );
        // ais.kystverket.no
        ShouldBeMessage( p.OnNextCalls[7], "13m63A?P00P`@GFTK3s>4?wR20Sf", 0, 722, 1567693372 );
        // ais.kystverket.no
        ShouldBeMessage( p.OnNextCalls[8], "544MR0827oeaD<u0000lDdP4pTf0duAA<uH000167pF=2=nG0:0DRj0CQiC4jh000000000", 0, 27, 1567686150 );
        // ais.kystverket.no
        ShouldBeMessage( p.OnNextCalls[9], "B3o8B<00F8:0h694gOtbgwqUoP06", 0, 808, 1567693618 );
    }

    // Same interleaved lines with 3 progress reports from the line parser (after 6, 10 and 13 lines): each
    // forwarded report adds the total number of AIS messages and the number of messages since the previous report.
    [Test]
    public void progress_reports()
    {
        var h = new AdapterHarness( new NmeaParserOptions { MaximumUnmatchedFragmentAge = 10 } );
        // ais.kystverket.no
        h.Receive( @"\s:42,c:1567684904*38\!AIVDM,1,1,,B,33m9UtPP@50wwE:VJW6LS67H01<@,0*3C" );
        // ais.kystverket.no
        h.Receive( @"\g:1-3-3451,s:27,c:1567686150*40\!AIVDM,3,1,9,A,544MR0827oeaD<u0000lDdP4pTf0duAA,0*17" );
        // ais.kystverket.no
        h.Receive( @"\s:42,c:1567684904*38\!AIVDM,1,1,,A,B3m:H900AP@b:79ae6:<OwnUoP06,0*78" );
        // Odd data, kept as is: compared to the same line elsewhere, ",0*63" has been appended. This looks like an
        // unintentional copy/paste. It does not matter here: the extra field is ignored and the default
        // ChecksumOption (ValidateStandardFormat) only checks the checksum format, not its value.
        // ais.kystverket.no
        h.Receive( @"\s:3,c:1567692251*01\!AIVDM,1,1,,A,13m9WS001d0K==pR=D?HB6WD0pJV,0*54,0*63" );
        // ais.kystverket.no
        h.Receive( @"\g:1-2-8055,s:99,c:1567685556*4E\!AIVDM,2,1,6,B,53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4l,0*72" );
        // Odd data, kept as is: the real checksum of this line is *21 (see the other tests), not *07. This also
        // looks unintentional and is accepted because checksum values are not checked by default.
        // ais.kystverket.no
        h.Receive( @"\s:24,c:1567692878*35\!AIVDM,1,1,,B,13o`9@701j1Ej3vc;o3q@7SJ0D02,0*07" );
        h.ReceiveProgress( false, 6, 1234, 6, 1234 );
        // ais.kystverket.no
        h.Receive( @"\g:2-3-3451*5F\!AIVDM,3,2,9,A,<uH000167pF=2=nG0:0DRj0CQiC4jh00,0*4A" );
        // ais.kystverket.no
        h.Receive( @"\s:67,c:1567693000*34\!AIVDM,1,1,,,3CnWHf50000ga40TCHE0D0@R003B,0*4B" );
        // ais.kystverket.no
        h.Receive( @"\s:772,c:1567693246*07\!AIVDM,1,1,,,13o7g2001P0Lv<rSdVHf2h3N0000,0*25" );
        // ais.kystverket.no
        h.Receive( @"\g:2-2-8055*55\!AIVDM,2,2,6,B,j`888888880,2*2B" );
        h.ReceiveProgress( false, 10, 2345, 4, 1111 );
        // ais.kystverket.no
        h.Receive( @"\s:722,c:1567693372*04\!AIVDM,1,1,,A,13m63A?P00P`@GFTK3s>4?wR20Sf,0*71" );
        // ais.kystverket.no
        h.Receive( @"\g:3-3-3451*5E\!AIVDM,3,3,9,A,0000000,0*2F" );
        // ais.kystverket.no
        h.Receive( @"\s:808,c:1567693618*0A\!AIVDM,1,1,,B,B3o8B<00F8:0h694gOtbgwqUoP06,0*73" );
        h.ReceiveProgress( true, 13, 2445, 3, 100 );
        var p = h.Complete();
        p.ProgressCalls.Count.ShouldBe( 3 );
        p.ProgressCalls[0].ShouldBe( new TestProcessor.ProgressReport( false, 6, 4, 1234, 6, 4, 1234 ) );
        p.ProgressCalls[1].ShouldBe( new TestProcessor.ProgressReport( false, 10, 7, 2345, 4, 3, 1111 ) );
        p.ProgressCalls[2].ShouldBe( new TestProcessor.ProgressReport( true, 13, 10, 2445, 3, 3, 100 ) );
    }

    // An error reported by the line parser ('foobar', line 42) is forwarded as is (same line, same exception, same line number).
    [Test]
    public void line_stream_parser_reports_error_in_non_fragmented_message()
    {
        var h = new AdapterHarness();
        var error = h.ReceiveErrorReport( "foobar", 42 );
        var p = h.Complete();
        ShouldHaveForwardedError( p, error );
    }

    // An error reported by the line parser, then a fragment: the error is forwarded as is.
    [Test]
    public void line_stream_parser_reports_error_in_first_fragment_of_two_part_message()
    {
        var h = new AdapterHarness();
        var error = h.ReceiveErrorReport( "foobar", 42 );
        // Odd data, kept as is: the title speaks of a two-part message but this is the last fragment of a
        // three-part one. The intent (an error, then a fragment of the same message) holds anyway.
        // ais.kystverket.no
        h.Receive( @"\g:3-3-3451*5E\!AIVDM,3,3,9,A,0000000,0*2F" );
        var p = h.Complete();
        ShouldHaveForwardedError( p, error );
    }

    // The first fragment of a 2-fragment message, then an error reported by the line parser: the error is forwarded as is.
    [Test]
    public void line_stream_parser_reports_error_in_second_fragment_of_two_part_message()
    {
        var h = new AdapterHarness();
        h.Receive( @"\g:1-2-8055,s:99,c:1567685556*4E\!AIVDM,2,1,6,B,53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4l,0*72" );
        var error = h.ReceiveErrorReport( "foobar", 42 );
        var p = h.Complete();
        ShouldHaveForwardedError( p, error );
    }

    // A first fragment with a padding of 1: an "only on the final message" error on the first line (line 1).
    [Test]
    public void line_stream_parser_passes_a_message_with_padding_that_is_the_first_of_a_two_part_message()
    {
        var h = new AdapterHarness();
        // ais.kystverket.no
        h.Receive( @"\g:1-2-8055,s:99,c:1567685556*4E\!AIVDM,2,1,6,B,53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4l,1*72" );
        // ais.kystverket.no
        h.Receive( @"\g:2-2-8055*55\!AIVDM,2,2,6,B,j`888888880,2*2B" );
        var p = h.Complete();
        p.OnErrorCalls.Count.ShouldBe( 1 );
        // ais.kystverket.no
        p.OnErrorCalls[0].Line.ShouldBe( @"\g:1-2-8055,s:99,c:1567685556*4E\!AIVDM,2,1,6,B,53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4l,1*72" );
        p.OnErrorCalls[0].Error.ShouldBeAssignableTo<ArgumentException>()!.Message.ShouldBe( "Can only handle non-zero padding on the final message in a fragment" );
        p.OnErrorCalls[0].LineNumber.ShouldBe( 1 );
        // Added: the old scenario did not check that the faulty message is not reported.
        p.OnNextCalls.ShouldBeEmpty();
    }

    // The first fragment received twice (same group id, same position): an "Already received sentence" error on the
    // second one (line 2).
    [Test]
    public void line_stream_parser_passes_the_same_sentence_of_a_two_part_message_twice()
    {
        var h = new AdapterHarness();
        // ais.kystverket.no
        h.Receive( @"\g:1-2-8055,s:99,c:1567685556*4E\!AIVDM,2,1,6,B,53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4l,0*72" );
        // ais.kystverket.no
        h.Receive( @"\g:1-2-8055*55\!AIVDM,2,1,6,B,j`888888880,0*2B" );
        var p = h.Complete();
        p.OnErrorCalls.Count.ShouldBe( 1 );
        // ais.kystverket.no
        p.OnErrorCalls[0].Line.ShouldBe( @"\g:1-2-8055*55\!AIVDM,2,1,6,B,j`888888880,0*2B" );
        p.OnErrorCalls[0].Error.ShouldBeAssignableTo<ArgumentException>()!.Message.ShouldStartWith( "Already received sentence ", caseSensitivity: Case.Sensitive );
        p.OnErrorCalls[0].LineNumber.ShouldBe( 2 );
    }

    // MaximumUnmatchedFragmentAge = 1: two single-fragment messages between the 2 fragments. The single ones are
    // reported, the first fragment expires on the 3rd line: an "incomplete fragmented message" error with the first
    // line and line number 1. (The 2nd fragment then starts a new incomplete message that is never reported.)
    [Test]
    public void two_fragment_message_fragments_received_too_many_sentences_in_the_middle()
    {
        var h = new AdapterHarness( new NmeaParserOptions { MaximumUnmatchedFragmentAge = 1 } );
        // ais.kystverket.no
        h.Receive( @"\g:1-2-8055,s:99,c:1567685556*4E\!AIVDM,2,1,6,B,53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4l,0*72" );
        // ais.kystverket.no
        h.Receive( @"\s:42,c:1567684904*38\!AIVDM,1,1,,B,33m9UtPP@50wwE:VJW6LS67H01<@,0*3C" );
        // Odd data, kept as is: ",0*63" appended, as in progress_reports. Probably unintentional and harmless
        // (the extra field is ignored, the payload and padding are the expected ones).
        // ais.kystverket.no
        h.Receive( @"\s:3,c:1567692251*01\!AIVDM,1,1,,A,13m9WS001d0K==pR=D?HB6WD0pJV,0*54,0*63" );
        // ais.kystverket.no
        h.Receive( @"\g:2-2-8055*55\!AIVDM,2,2,6,B,j`888888880,2*2B" );
        var p = h.Complete();
        p.OnNextCalls.Count.ShouldBe( 2 );
        // ais.kystverket.no
        ShouldBeMessage( p.OnNextCalls[0], "33m9UtPP@50wwE:VJW6LS67H01<@", 0, 42, 1567684904 );
        // ais.kystverket.no
        ShouldBeMessage( p.OnNextCalls[1], "13m9WS001d0K==pR=D?HB6WD0pJV", 0, 3, 1567692251 );
        p.OnErrorCalls.Count.ShouldBe( 1 );
        // ais.kystverket.no
        p.OnErrorCalls[0].Line.ShouldBe( @"\g:1-2-8055,s:99,c:1567685556*4E\!AIVDM,2,1,6,B,53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4l,0*72" );
        p.OnErrorCalls[0].Error.ShouldBeAssignableTo<ArgumentException>()!.Message.ShouldBe( "Received incomplete fragmented message." );
        p.OnErrorCalls[0].LineNumber.ShouldBe( 1 );
    }

    // EmptyGroupTolerance.AutoFix: a 3-sentence group where only the first line has a (complete, 1/1) sentence and the
    // 2 others are empty: one message, IsFixedMessage is true and the grouping is removed (SentencesInGroup is 0).
    [Test]
    public void tree_fragment_message_with_only_one_sentence_and_auto_fix_group()
    {
        var h = new AdapterHarness( new NmeaParserOptions { EmptyGroupTolerance = EmptyGroupTolerance.AutoFix } );
        h.Receive( @"\g:1-3-1481,s:99,c:1718701847*41\!AIVDM,1,1,,A,13Ef?<3006wt9WlEMhi1S6uJ00Rq,0*41" );
        h.Receive( @"\g:2-3-1481*50\" );
        h.Receive( @"\g:3-3-1481*51\" );
        var p = h.Complete();
        p.OnNextCalls.Count.ShouldBe( 1 );
        ShouldBeMessage( p.OnNextCalls[0], "13Ef?<3006wt9WlEMhi1S6uJ00Rq", 0, 99, 1718701847 );
        p.OnNextCalls[0].IsFixedMessage.ShouldBeTrue();
        p.OnNextCalls[0].SentencesInGroup.ShouldBe( 0 );
    }

    // EmptyGroupTolerance.AutoFix: a 3-sentence group where the first 2 lines hold a 2-fragment message and the 3rd
    // is empty: one reassembled message, IsFixedMessage is true and the grouping is fixed to 2 sentences.
    [Test]
    public void tree_fragment_message_with_two_sentences_and_auto_fix_group()
    {
        var h = new AdapterHarness( new NmeaParserOptions { EmptyGroupTolerance = EmptyGroupTolerance.AutoFix } );
        h.Receive( @"\g:1-3-1481,s:99,c:1718701847*41\!AIVDM,2,1,8,A,13Ef?<3006wt9WlEMhi1S6uJ00Rq,0*7A" );
        h.Receive( @"\g:2-3-1481*50\!AIVDM,2,2,8,B,T`0@n888880,2*3D" );
        h.Receive( @"\g:3-3-1481*51\" );
        var p = h.Complete();
        p.OnNextCalls.Count.ShouldBe( 1 );
        ShouldBeMessage( p.OnNextCalls[0], "13Ef?<3006wt9WlEMhi1S6uJ00RqT`0@n888880", 2, 99, 1718701847 );
        p.OnNextCalls[0].IsFixedMessage.ShouldBeTrue();
        p.OnNextCalls[0].SentencesInGroup.ShouldBe( 2 );
    }

    // EmptyGroupTolerance.Allow: same lines as the "only one sentence and auto fix group" test: one message, not fixed,
    // with the original grouping (3 sentences).
    [Test]
    public void tree_fragment_message_with_only_one_sentence_and_allow_empty_sentence()
    {
        var h = new AdapterHarness( new NmeaParserOptions { EmptyGroupTolerance = EmptyGroupTolerance.Allow } );
        h.Receive( @"\g:1-3-1481,s:99,c:1718701847*41\!AIVDM,1,1,,A,13Ef?<3006wt9WlEMhi1S6uJ00Rq,0*41" );
        h.Receive( @"\g:2-3-1481*50\" );
        h.Receive( @"\g:3-3-1481*51\" );
        var p = h.Complete();
        p.OnNextCalls.Count.ShouldBe( 1 );
        ShouldBeMessage( p.OnNextCalls[0], "13Ef?<3006wt9WlEMhi1S6uJ00Rq", 0, 99, 1718701847 );
        p.OnNextCalls[0].IsFixedMessage.ShouldBeFalse();
        p.OnNextCalls[0].SentencesInGroup.ShouldBe( 3 );
    }

    // EmptyGroupTolerance.None: the empty lines cannot be parsed ("missing some characters"); the line parser reports
    // them as errors, they are forwarded and no message is reported.
    // (The source feature had commented-out steps here that did the same through "I parse the content by message".)
    [Test]
    public void tree_fragment_message_with_only_one_sentence_and_empty_sentence_is_not_allowed()
    {
        var h = new AdapterHarness( new NmeaParserOptions { EmptyGroupTolerance = EmptyGroupTolerance.None } );
        h.Receive( @"\g:1-3-1481,s:99,c:1718701847*41\!AIVDM,1,1,,A,13Ef?<3006wt9WlEMhi1S6uJ00Rq,0*41" );
        h.ReceiveErrorReportForInvalidContent( @"\g:2-3-1481*50\", 2 );
        h.ReceiveErrorReportForInvalidContent( @"\g:3-3-1481*51\", 3 );
        var p = h.Complete();
        p.OnNextCalls.ShouldBeEmpty();
        p.OnErrorCalls.Count.ShouldBe( 2 );
        p.OnErrorCalls[0].Line.ShouldBe( @"\g:2-3-1481*50\" );
        p.OnErrorCalls[0].Error.Message.ShouldBe( "Invalid data. The message appears to be missing some characters - it may have been corrupted or truncated." );
        p.OnErrorCalls[1].Line.ShouldBe( @"\g:3-3-1481*51\" );
        p.OnErrorCalls[1].Error.Message.ShouldBe( "Invalid data. The message appears to be missing some characters - it may have been corrupted or truncated." );
    }

    // AllowNonZeroPaddingInNonLastFragment = false: a first fragment with a padding of 2 is an error, no message.
    [Test]
    public void multiple_lines_with_non_zero_padding_in_non_last_fragment_disallowed()
    {
        var h = new AdapterHarness( new NmeaParserOptions { AllowNonZeroPaddingInNonLastFragment = false } );
        h.Receive( @"\c:1717977721*54\!AIVDM,2,1,2,B,53P7hnD00000LQLF220<P4hhDpLF22200000000N1h4245Ra001RDj2CQp1l,2*15" );
        h.Receive( @"\c:1717977721*54\!AIVDM,2,2,2,B,SmCQ4p00000,2*4D" );
        var p = h.Complete();
        p.OnNextCalls.ShouldBeEmpty();
        p.OnErrorCalls.Count.ShouldBe( 1 );
        p.OnErrorCalls[0].Line.ShouldBe( @"\c:1717977721*54\!AIVDM,2,1,2,B,53P7hnD00000LQLF220<P4hhDpLF22200000000N1h4245Ra001RDj2CQp1l,2*15" );
        p.OnErrorCalls[0].Error.ShouldBeAssignableTo<ArgumentException>()!.Message.ShouldBe( "Can only handle non-zero padding on the final message in a fragment" );
    }

    // AllowNonZeroPaddingInNonLastFragment = true: the same 2 fragments give one reassembled message, no error.
    [Test]
    public void multiple_lines_with_non_zero_padding_in_non_last_fragment_allowed()
    {
        var h = new AdapterHarness( new NmeaParserOptions { AllowNonZeroPaddingInNonLastFragment = true } );
        h.Receive( @"\c:1717977721*54\!AIVDM,2,1,2,B,53P7hnD00000LQLF220<P4hhDpLF22200000000N1h4245Ra001RDj2CQp1l,2*15" );
        h.Receive( @"\c:1717977721*54\!AIVDM,2,2,2,B,SmCQ4p00000,2*4D" );
        var p = h.Complete();
        p.OnNextCalls.Count.ShouldBe( 1 );
        p.OnNextCalls[0].AsciiPayload.ShouldBe( "53P7hnD00000LQLF220<P4hhDpLF22200000000N1h4245Ra001RDj2CQp1lSmCQ4p00000" );
        p.OnNextCalls[0].Padding.ShouldBe( 2u );
        p.OnErrorCalls.ShouldBeEmpty();
    }

    // ThrowWhenNoExclamationMark = true: a sentence starting with '$' cannot be parsed ("expected '!'"); the line parser
    // reports it as an error, it is forwarded and no message is reported.
    [Test]
    public void sentence_without_exclamation_mark_is_disallowed()
    {
        var h = new AdapterHarness( new NmeaParserOptions { ThrowWhenNoExclamationMark = true } );
        h.ReceiveErrorReportForInvalidContent( @"\c:1717977721*54\$AIVDM,2,1,2,B,53P7hnD00000LQLF220<P4hhDpLF22200000000N1h4245Ra001RDj2CQp1l,2*15", 1 );
        var p = h.Complete();
        p.OnNextCalls.ShouldBeEmpty();
        p.OnErrorCalls.Count.ShouldBe( 1 );
        p.OnErrorCalls[0].Line.ShouldBe( @"\c:1717977721*54\$AIVDM,2,1,2,B,53P7hnD00000LQLF220<P4hhDpLF22200000000N1h4245Ra001RDj2CQp1l,2*15" );
        p.OnErrorCalls[0].Error.ShouldBeAssignableTo<ArgumentException>()!.Message.ShouldBe( "Invalid data. Expected '!' at sentence start" );
    }

    // ThrowWhenNoExclamationMark = false: a sentence starting with '$' is accepted, one message, no error.
    [Test]
    public void sentence_without_exclamation_mark_is_allowed()
    {
        var h = new AdapterHarness( new NmeaParserOptions { ThrowWhenNoExclamationMark = false } );
        h.Receive( @"\s:808,c:1567693618*0A\$AIVDM,1,1,,B,B3o8B<00F8:0h694gOtbgwqUoP06,0*73" );
        var p = h.Complete();
        p.OnNextCalls.Count.ShouldBe( 1 );
        // Added: the old scenario only counted the messages.
        p.OnNextCalls[0].AsciiPayload.ShouldBe( "B3o8B<00F8:0h694gOtbgwqUoP06" );
        p.OnErrorCalls.ShouldBeEmpty();
    }

    static void ShouldBeMessage( TestProcessor.Message m, string payload, uint padding, int source, long timestamp )
    {
        m.AsciiPayload.ShouldBe( payload );
        m.Padding.ShouldBe( padding );
        m.Source.ShouldBe( source );
        m.UnixTimestamp.ShouldBe( timestamp );
    }

    static void ShouldHaveForwardedError( TestProcessor p, Exception error )
    {
        p.OnErrorCalls.Count.ShouldBe( 1 );
        p.OnErrorCalls[0].Line.ShouldBe( "foobar" );
        p.OnErrorCalls[0].Error.ShouldBeSameAs( error );
        p.OnErrorCalls[0].LineNumber.ShouldBe( 42 );
    }

    /// <summary>
    /// Drives a <see cref="NmeaLineToAisStreamAdapter{TExtraFieldParser}"/> over a <see cref="TestProcessor"/>
    /// like a line stream parser would do.
    /// </summary>
    sealed class AdapterHarness
    {
        readonly NmeaParserOptions _options;
        readonly NmeaLineToAisStreamAdapter<DefaultExtraFieldParser> _adapter;
        int _lineNumber = 1;

        public AdapterHarness( NmeaParserOptions? options = null )
        {
            _options = options ?? new NmeaParserOptions();
            Processor = new TestProcessor();
            _adapter = new NmeaLineToAisStreamAdapter<DefaultExtraFieldParser>( Processor, _options );
        }

        public TestProcessor Processor { get; }

        /// <summary>
        /// Parses the line and gives it to the adapter. Line numbers start at 1 and are incremented
        /// by this method only (error reports use their own explicit line number).
        /// </summary>
        public void Receive( string line )
        {
            byte[] ascii = Encoding.ASCII.GetBytes( line );
            _adapter.OnNext( new NmeaLineParser<DefaultExtraFieldParser>( ascii, _options ), _lineNumber++ );
        }

        /// <summary>
        /// Gives the adapter an error report with an arbitrary exception and returns this exception.
        /// </summary>
        public Exception ReceiveErrorReport( string content, int lineNumber )
        {
            var error = new ArgumentException( "That was never 5 minutes" );
            _adapter.OnError( Encoding.ASCII.GetBytes( content ), error, lineNumber );
            return error;
        }

        /// <summary>
        /// Checks that the content cannot be parsed and gives the adapter the parser's exception.
        /// </summary>
        public Exception ReceiveErrorReportForInvalidContent( string content, int lineNumber )
        {
            byte[] ascii = Encoding.ASCII.GetBytes( content );
            var options = _options;
            // Fixed: the old step called Assert.Fail inside a try { } catch( Exception ) block: the AssertionException
            // thrown by Assert.Fail was caught, then given to the adapter as if it were the parser's error. A content
            // that did not fail to parse was therefore not detected by this step.
            var error = Util.Invokable( () => { var parser = new NmeaLineParser<DefaultExtraFieldParser>( ascii, options ); } ).ShouldThrow<ArgumentException>();
            _adapter.OnError( ascii, error, lineNumber );
            return error;
        }

        public void ReceiveProgress( bool done, int totalLines, int totalTicks, int linesSinceLastUpdate, int ticksSinceLastUpdate )
        {
            _adapter.Progress( done, totalLines, totalTicks, linesSinceLastUpdate, ticksSinceLastUpdate );
        }

        /// <summary>
        /// Completes the adapter (the old SpecFlow steps did it implicitly in Dispose), disposes it
        /// and checks that the processor has been completed.
        /// </summary>
        public TestProcessor Complete()
        {
            _adapter.OnCompleted();
            _adapter.Dispose();
            Processor.IsComplete.ShouldBeTrue();
            return Processor;
        }
    }
}
