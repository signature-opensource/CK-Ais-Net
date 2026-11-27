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
//   by the AIS sentence group id otherwise; the fragments may be received in any order and interleaved messages are
//   supported, within MaximumUnmatchedFragmentAge lines, after which the incomplete message is an error (as is an
//   incomplete message at the end of the stream);
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
    public void Non_fragmented_message_received()
    {
        var h = new AdapterHarness();
        // ais.kystverket.no
        h.Receive( @"\s:42,c:1567684904*38\!AIVDM,1,1,,B,33m9UtPP@50wwE:VJW6LS67H01<@,0*3C" );
        var p = h.Complete();
        p.OnNextCalls.Count.ShouldBe( 1 );
        ShouldBeMessage( p.OnNextCalls[0], "33m9UtPP@50wwE:VJW6LS67H01<@", 0, 42, 1567684904 );
    }

    // Only the first fragment (with tag block grouping) of a 2-fragment message, then the end of the stream:
    // no message, and the incomplete message is reported as an error when the stream completes (the same error as
    // when it exceeds the MaximumUnmatchedFragmentAge).
    [Test]
    public void First_fragment_of_two_part_message_received()
    {
        var h = new AdapterHarness();
        // ais.kystverket.no
        h.Receive( @"\g:1-2-8055,s:99,c:1567685556*4E\!AIVDM,2,1,6,B,53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4l,0*72" );
        var p = h.Complete();
        p.OnNextCalls.ShouldBeEmpty();
        // Added: before, the incomplete message was silently dropped at the end of the stream.
        p.OnErrorCalls.Count.ShouldBe( 1 );
        // ais.kystverket.no
        ShouldBeIncompleteMessageError( p.OnErrorCalls[0], @"\g:1-2-8055,s:99,c:1567685556*4E\!AIVDM,2,1,6,B,53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4l,0*72", 1 );
    }

    // Only the first fragment (without tag block grouping) of a 2-fragment message, then the end of the stream:
    // no message, and the incomplete message is reported as an error when the stream completes.
    [Test]
    public void First_fragment_of_two_part_message_without_grouping_in_header_received()
    {
        var h = new AdapterHarness();
        // ais.kystverket.no
        h.Receive( @"\s:99,c:1567685556*34\!AIVDM,2,1,6,B,53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4l,0*72" );
        var p = h.Complete();
        p.OnNextCalls.ShouldBeEmpty();
        // Added: before, the incomplete message was silently dropped at the end of the stream.
        p.OnErrorCalls.Count.ShouldBe( 1 );
        // ais.kystverket.no
        ShouldBeIncompleteMessageError( p.OnErrorCalls[0], @"\s:99,c:1567685556*34\!AIVDM,2,1,6,B,53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4l,0*72", 1 );
    }

    // Two incomplete messages (one grouped by the tag block, one by the AIS sentence) and a complete one at the end
    // of the stream: the complete message is reported, and each incomplete one is reported as an error, in the order
    // of their first fragment, before the processor is completed.
    [Test]
    public void End_of_stream_with_several_incomplete_messages()
    {
        var h = new AdapterHarness();
        // ais.kystverket.no
        h.Receive( @"\s:99,c:1567685556*34\!AIVDM,2,1,6,B,53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4l,0*72" );
        // ais.kystverket.no
        h.Receive( @"\g:1-2-8055,s:99,c:1567685556*4E\!AIVDM,2,1,6,B,53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4l,0*72" );
        // ais.kystverket.no
        h.Receive( @"\s:42,c:1567684904*38\!AIVDM,1,1,,B,33m9UtPP@50wwE:VJW6LS67H01<@,0*3C" );
        var p = h.Complete();
        p.OnNextCalls.Count.ShouldBe( 1 );
        // ais.kystverket.no
        ShouldBeMessage( p.OnNextCalls[0], "33m9UtPP@50wwE:VJW6LS67H01<@", 0, 42, 1567684904 );
        p.OnErrorCalls.Count.ShouldBe( 2 );
        // ais.kystverket.no
        ShouldBeIncompleteMessageError( p.OnErrorCalls[0], @"\s:99,c:1567685556*34\!AIVDM,2,1,6,B,53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4l,0*72", 1 );
        // ais.kystverket.no
        ShouldBeIncompleteMessageError( p.OnErrorCalls[1], @"\g:1-2-8055,s:99,c:1567685556*4E\!AIVDM,2,1,6,B,53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4l,0*72", 2 );
    }

    // The 2 fragments (with tag block grouping) one after the other: one reassembled message
    // (padding of the last fragment, source and timestamp of the first line).
    [Test]
    public void Two_fragment_message_fragments_received_adjacently()
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
    public void Two_fragment_message_fragments_without_grouping_in_header_received_adjacently()
    {
        var h = new AdapterHarness();
        // ais.kystverket.no
        h.Receive( @"\s:99,c:1567685556*34\!AIVDM,2,1,6,B,53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4l,0*72" );
        // ais.kystverket.no
        h.Receive( @"\s:99*49\!AIVDM,2,2,6,B,j`888888880,2*2B" );
        var p = h.Complete();
        p.OnNextCalls.Count.ShouldBe( 1 );
        // ais.kystverket.no
        ShouldBeMessage( p.OnNextCalls[0], "53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4lj`888888880", 2, 99, 1567685556 );
    }

    // A single-fragment message between the 2 fragments (with tag block grouping): the single one comes first,
    // then the reassembled one.
    [Test]
    public void Two_fragment_message_fragments_received_with_single_fragment_message_in_the_middle()
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
    public void Two_fragment_message_fragments_without_grouping_in_header_received_with_single_fragment_message_in_the_middle()
    {
        var h = new AdapterHarness();
        // ais.kystverket.no
        h.Receive( @"\s:99,c:1567685556*34\!AIVDM,2,1,6,B,53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4l,0*72" );
        // ais.kystverket.no
        h.Receive( @"\s:42,c:1567684904*38\!AIVDM,1,1,,B,33m9UtPP@50wwE:VJW6LS67H01<@,0*3C" );
        // ais.kystverket.no
        h.Receive( @"\s:99*49\!AIVDM,2,2,6,B,j`888888880,2*2B" );
        var p = h.Complete();
        p.OnNextCalls.Count.ShouldBe( 2 );
        // ais.kystverket.no
        ShouldBeMessage( p.OnNextCalls[0], "33m9UtPP@50wwE:VJW6LS67H01<@", 0, 42, 1567684904 );
        // ais.kystverket.no
        ShouldBeMessage( p.OnNextCalls[1], "53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4lj`888888880", 2, 99, 1567685556 );
    }

    // The 3 fragments (with tag block grouping) one after the other: one reassembled message.
    [Test]
    public void Three_fragment_message_fragments_received_adjacently()
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
    public void Three_fragment_message_fragments_without_grouping_in_header_received_adjacently()
    {
        var h = new AdapterHarness();
        // ais.kystverket.no
        h.Receive( @"\s:27,c:1567686150*30\!AIVDM,3,1,9,A,544MR0827oeaD<u0000lDdP4pTf0duAA,0*17" );
        // ais.kystverket.no
        h.Receive( @"\s:27*4C\!AIVDM,3,2,9,A,<uH000167pF=2=nG0:0DRj0CQiC4jh00,0*4A" );
        // ais.kystverket.no
        h.Receive( @"\s:27*4C\!AIVDM,3,3,9,A,0000000,0*2F" );
        var p = h.Complete();
        p.OnNextCalls.Count.ShouldBe( 1 );
        // ais.kystverket.no
        ShouldBeMessage( p.OnNextCalls[0], "544MR0827oeaD<u0000lDdP4pTf0duAA<uH000167pF=2=nG0:0DRj0CQiC4jh000000000", 0, 27, 1567686150 );
    }

    // A 3-fragment and a 2-fragment message interleaved (with tag block grouping): the 2-fragment one completes
    // first and is reported first.
    [Test]
    public void Interleaved_multi_fragment_messages()
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
    public void Interleaved_multi_fragment_messages_without_grouping_in_header()
    {
        var h = new AdapterHarness();
        // ais.kystverket.no
        h.Receive( @"\s:27,c:1567686150*30\!AIVDM,3,1,9,A,544MR0827oeaD<u0000lDdP4pTf0duAA,0*17" );
        // ais.kystverket.no
        h.Receive( @"\s:99,c:1567685556*34\!AIVDM,2,1,6,B,53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4l,0*72" );
        // ais.kystverket.no
        h.Receive( @"\s:27,*60\!AIVDM,3,2,9,A,<uH000167pF=2=nG0:0DRj0CQiC4jh00,0*4A" );
        // ais.kystverket.no
        h.Receive( @"\s:99,*65\!AIVDM,2,2,6,B,j`888888880,2*2B" );
        // ais.kystverket.no
        h.Receive( @"\s:27*4C\!AIVDM,3,3,9,A,0000000,0*2F" );
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
    public void Interleaved_multi_fragment_messages_distinguished_only_by_grouping_in_header()
    {
        var h = new AdapterHarness();
        // ais.kystverket.no
        h.Receive( @"\g:1-3-3451,s:27,c:1567686150*40\!AIVDM,3,1,9,A,544MR0827oeaD<u0000lDdP4pTf0duAA,0*17" );
        // ais.kystverket.no
        h.Receive( @"\g:1-2-8055,s:99,c:1567685556*4E\!AIVDM,2,1,9,B,53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4l,0*7D" );
        // ais.kystverket.no
        h.Receive( @"\g:2-3-3451*5F\!AIVDM,3,2,9,A,<uH000167pF=2=nG0:0DRj0CQiC4jh00,0*4A" );
        // ais.kystverket.no
        h.Receive( @"\g:2-2-8055*55\!AIVDM,2,2,9,B,j`888888880,2*24" );
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
    public void Interleaved_single_and_multi_fragment_messages()
    {
        var h = new AdapterHarness( new NmeaParserOptions { MaximumUnmatchedFragmentAge = 10 } );
        // ais.kystverket.no
        h.Receive( @"\s:42,c:1567684904*38\!AIVDM,1,1,,B,33m9UtPP@50wwE:VJW6LS67H01<@,0*3C" );
        // ais.kystverket.no
        h.Receive( @"\g:1-3-3451,s:27,c:1567686150*40\!AIVDM,3,1,9,A,544MR0827oeaD<u0000lDdP4pTf0duAA,0*17" );
        // ais.kystverket.no
        h.Receive( @"\s:42,c:1567684904*38\!AIVDM,1,1,,A,B3m:H900AP@b:79ae6:<OwnUoP06,0*78" );
        // ais.kystverket.no
        h.Receive( @"\s:3,c:1567692251*01\!AIVDM,1,1,,A,13m9WS001d0K==pR=D?HB6WD0pJV,0*4D" );
        // ais.kystverket.no
        h.Receive( @"\g:1-2-8055,s:99,c:1567685556*4E\!AIVDM,2,1,6,B,53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4l,0*72" );
        // ais.kystverket.no
        h.Receive( @"\s:24,c:1567692878*35\!AIVDM,1,1,,B,13o`9@701j1Ej3vc;o3q@7SJ0D02,0*3B" );
        // ais.kystverket.no
        h.Receive( @"\g:2-3-3451*5F\!AIVDM,3,2,9,A,<uH000167pF=2=nG0:0DRj0CQiC4jh00,0*4A" );
        // ais.kystverket.no
        h.Receive( @"\s:67,c:1567693000*34\!AIVDM,1,1,,,3CnWHf50000ga40TCHE0D0@R003B,0*0A" );
        // ais.kystverket.no
        h.Receive( @"\s:772,c:1567693246*07\!AIVDM,1,1,,,13o7g2001P0Lv<rSdVHf2h3N0000,0*25" );
        // ais.kystverket.no
        h.Receive( @"\g:2-2-8055*55\!AIVDM,2,2,6,B,j`888888880,2*2B" );
        // ais.kystverket.no
        h.Receive( @"\s:722,c:1567693372*04\!AIVDM,1,1,,A,13m63A?P00P`@GFTK3s>4?wR20Sf,0*6B" );
        // ais.kystverket.no
        h.Receive( @"\g:3-3-3451*5E\!AIVDM,3,3,9,A,0000000,0*2F" );
        // ais.kystverket.no
        h.Receive( @"\s:808,c:1567693618*0A\!AIVDM,1,1,,B,B3o8B<00F8:0h694gOtbgwqUoP06,0*69" );
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
    public void Progress_reports()
    {
        var h = new AdapterHarness( new NmeaParserOptions { MaximumUnmatchedFragmentAge = 10 } );
        // ais.kystverket.no
        h.Receive( @"\s:42,c:1567684904*38\!AIVDM,1,1,,B,33m9UtPP@50wwE:VJW6LS67H01<@,0*3C" );
        // ais.kystverket.no
        h.Receive( @"\g:1-3-3451,s:27,c:1567686150*40\!AIVDM,3,1,9,A,544MR0827oeaD<u0000lDdP4pTf0duAA,0*17" );
        // ais.kystverket.no
        h.Receive( @"\s:42,c:1567684904*38\!AIVDM,1,1,,A,B3m:H900AP@b:79ae6:<OwnUoP06,0*78" );
        // ais.kystverket.no
        h.Receive( @"\s:3,c:1567692251*01\!AIVDM,1,1,,A,13m9WS001d0K==pR=D?HB6WD0pJV,0*4D" );
        // ais.kystverket.no
        h.Receive( @"\g:1-2-8055,s:99,c:1567685556*4E\!AIVDM,2,1,6,B,53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4l,0*72" );
        // ais.kystverket.no
        h.Receive( @"\s:24,c:1567692878*35\!AIVDM,1,1,,B,13o`9@701j1Ej3vc;o3q@7SJ0D02,0*3B" );
        h.ReceiveProgress( false, 6, 1234, 6, 1234 );
        // ais.kystverket.no
        h.Receive( @"\g:2-3-3451*5F\!AIVDM,3,2,9,A,<uH000167pF=2=nG0:0DRj0CQiC4jh00,0*4A" );
        // ais.kystverket.no
        h.Receive( @"\s:67,c:1567693000*34\!AIVDM,1,1,,,3CnWHf50000ga40TCHE0D0@R003B,0*0A" );
        // ais.kystverket.no
        h.Receive( @"\s:772,c:1567693246*07\!AIVDM,1,1,,,13o7g2001P0Lv<rSdVHf2h3N0000,0*25" );
        // ais.kystverket.no
        h.Receive( @"\g:2-2-8055*55\!AIVDM,2,2,6,B,j`888888880,2*2B" );
        h.ReceiveProgress( false, 10, 2345, 4, 1111 );
        // ais.kystverket.no
        h.Receive( @"\s:722,c:1567693372*04\!AIVDM,1,1,,A,13m63A?P00P`@GFTK3s>4?wR20Sf,0*6B" );
        // ais.kystverket.no
        h.Receive( @"\g:3-3-3451*5E\!AIVDM,3,3,9,A,0000000,0*2F" );
        // ais.kystverket.no
        h.Receive( @"\s:808,c:1567693618*0A\!AIVDM,1,1,,B,B3o8B<00F8:0h694gOtbgwqUoP06,0*69" );
        h.ReceiveProgress( true, 13, 2445, 3, 100 );
        var p = h.Complete();
        p.ProgressCalls.Count.ShouldBe( 3 );
        p.ProgressCalls[0].ShouldBe( new TestProcessor.ProgressReport( false, 6, 4, 1234, 6, 4, 1234 ) );
        p.ProgressCalls[1].ShouldBe( new TestProcessor.ProgressReport( false, 10, 7, 2345, 4, 3, 1111 ) );
        p.ProgressCalls[2].ShouldBe( new TestProcessor.ProgressReport( true, 13, 10, 2445, 3, 3, 100 ) );
    }

    // An error reported by the line parser ('foobar', line 42) is forwarded as is (same line, same exception, same line number).
    [Test]
    public void Line_stream_parser_reports_error_in_non_fragmented_message()
    {
        var h = new AdapterHarness();
        var error = h.ReceiveErrorReport( "foobar", 42 );
        var p = h.Complete();
        p.OnErrorCalls.Count.ShouldBe( 1 );
        ShouldHaveForwardedError( p.OnErrorCalls[0], error );
    }

    // An error reported by the line parser, then a fragment: the error is forwarded as is. The fragment is never
    // completed: it is reported as an incomplete message when the stream completes.
    [Test]
    public void Line_stream_parser_reports_error_in_first_fragment_of_two_part_message()
    {
        var h = new AdapterHarness();
        var error = h.ReceiveErrorReport( "foobar", 42 );
        // The SpecFlow scenario sent the last fragment of a three-part message here, despite its title.
        // ais.kystverket.no
        h.Receive( @"\g:1-2-8055,s:99,c:1567685556*4E\!AIVDM,2,1,6,B,53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4l,0*72" );
        var p = h.Complete();
        p.OnErrorCalls.Count.ShouldBe( 2 );
        ShouldHaveForwardedError( p.OnErrorCalls[0], error );
        // ais.kystverket.no
        ShouldBeIncompleteMessageError( p.OnErrorCalls[1], @"\g:1-2-8055,s:99,c:1567685556*4E\!AIVDM,2,1,6,B,53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4l,0*72", 1 );
    }

    // The first fragment of a 2-fragment message, then an error reported by the line parser: the error is forwarded as is.
    // The fragment is never completed: it is reported as an incomplete message when the stream completes.
    [Test]
    public void Line_stream_parser_reports_error_in_second_fragment_of_two_part_message()
    {
        var h = new AdapterHarness();
        h.Receive( @"\g:1-2-8055,s:99,c:1567685556*4E\!AIVDM,2,1,6,B,53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4l,0*72" );
        var error = h.ReceiveErrorReport( "foobar", 42 );
        var p = h.Complete();
        p.OnErrorCalls.Count.ShouldBe( 2 );
        ShouldHaveForwardedError( p.OnErrorCalls[0], error );
        // ais.kystverket.no
        ShouldBeIncompleteMessageError( p.OnErrorCalls[1], @"\g:1-2-8055,s:99,c:1567685556*4E\!AIVDM,2,1,6,B,53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4l,0*72", 1 );
    }

    // A first fragment with a padding of 1: an "only on the final message" error on the first line (line 1).
    [Test]
    public void Line_stream_parser_passes_a_message_with_padding_that_is_the_first_of_a_two_part_message()
    {
        var h = new AdapterHarness();
        // ais.kystverket.no
        h.Receive( @"\g:1-2-8055,s:99,c:1567685556*4E\!AIVDM,2,1,6,B,53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4l,1*73" );
        // ais.kystverket.no
        h.Receive( @"\g:2-2-8055*55\!AIVDM,2,2,6,B,j`888888880,2*2B" );
        var p = h.Complete();
        p.OnErrorCalls.Count.ShouldBe( 1 );
        // ais.kystverket.no
        p.OnErrorCalls[0].Line.ShouldBe( @"\g:1-2-8055,s:99,c:1567685556*4E\!AIVDM,2,1,6,B,53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4l,1*73" );
        p.OnErrorCalls[0].Error.ShouldBeAssignableTo<ArgumentException>()!.Message.ShouldBe( "Can only handle non-zero padding on the final message in a fragment" );
        p.OnErrorCalls[0].LineNumber.ShouldBe( 1 );
        // Added: the old scenario did not check that the faulty message is not reported.
        p.OnNextCalls.ShouldBeEmpty();
    }

    // The first fragment received twice (same group id, same position): an "Already received sentence" error on the
    // second one (line 2). The second one replaces the first one, and the message is never completed: it is reported
    // as an incomplete message (with the second line and its line number) when the stream completes.
    [Test]
    public void Line_stream_parser_passes_the_same_sentence_of_a_two_part_message_twice()
    {
        var h = new AdapterHarness();
        // ais.kystverket.no
        h.Receive( @"\g:1-2-8055,s:99,c:1567685556*4E\!AIVDM,2,1,6,B,53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4l,0*72" );
        // ais.kystverket.no
        h.Receive( @"\g:1-2-8055*56\!AIVDM,2,1,6,B,j`888888880,0*2A" );
        var p = h.Complete();
        p.OnErrorCalls.Count.ShouldBe( 2 );
        // ais.kystverket.no
        p.OnErrorCalls[0].Line.ShouldBe( @"\g:1-2-8055*56\!AIVDM,2,1,6,B,j`888888880,0*2A" );
        p.OnErrorCalls[0].Error.ShouldBeAssignableTo<ArgumentException>()!.Message.ShouldStartWith( "Already received sentence ", caseSensitivity: Case.Sensitive );
        p.OnErrorCalls[0].LineNumber.ShouldBe( 2 );
        // ais.kystverket.no
        ShouldBeIncompleteMessageError( p.OnErrorCalls[1], @"\g:1-2-8055*56\!AIVDM,2,1,6,B,j`888888880,0*2A", 2 );
    }

    // MaximumUnmatchedFragmentAge = 1: two single-fragment messages between the 2 fragments. The single ones are
    // reported, the first fragment expires on the 3rd line: an "incomplete fragmented message" error with the first
    // line and line number 1. The 2nd fragment then starts a new message that is never completed: it is reported as an
    // incomplete message (line number 4) when the stream completes.
    [Test]
    public void Two_fragment_message_fragments_received_too_many_sentences_in_the_middle()
    {
        var h = new AdapterHarness( new NmeaParserOptions { MaximumUnmatchedFragmentAge = 1 } );
        // ais.kystverket.no
        h.Receive( @"\g:1-2-8055,s:99,c:1567685556*4E\!AIVDM,2,1,6,B,53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4l,0*72" );
        // ais.kystverket.no
        h.Receive( @"\s:42,c:1567684904*38\!AIVDM,1,1,,B,33m9UtPP@50wwE:VJW6LS67H01<@,0*3C" );
        // ais.kystverket.no
        h.Receive( @"\s:3,c:1567692251*01\!AIVDM,1,1,,A,13m9WS001d0K==pR=D?HB6WD0pJV,0*4D" );
        // ais.kystverket.no
        h.Receive( @"\g:2-2-8055*55\!AIVDM,2,2,6,B,j`888888880,2*2B" );
        var p = h.Complete();
        p.OnNextCalls.Count.ShouldBe( 2 );
        // ais.kystverket.no
        ShouldBeMessage( p.OnNextCalls[0], "33m9UtPP@50wwE:VJW6LS67H01<@", 0, 42, 1567684904 );
        // ais.kystverket.no
        ShouldBeMessage( p.OnNextCalls[1], "13m9WS001d0K==pR=D?HB6WD0pJV", 0, 3, 1567692251 );
        p.OnErrorCalls.Count.ShouldBe( 2 );
        // ais.kystverket.no
        ShouldBeIncompleteMessageError( p.OnErrorCalls[0], @"\g:1-2-8055,s:99,c:1567685556*4E\!AIVDM,2,1,6,B,53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4l,0*72", 1 );
        // ais.kystverket.no
        ShouldBeIncompleteMessageError( p.OnErrorCalls[1], @"\g:2-2-8055*55\!AIVDM,2,2,6,B,j`888888880,2*2B", 4 );
    }

    // The 2nd fragment (with tag block grouping) received alone, then the end of the stream: no message, the
    // 2nd fragment is reported as an incomplete message (line number 1) when the stream completes.
    [Test]
    public void Second_fragment_received_without_the_first_one()
    {
        var h = new AdapterHarness();
        // ais.kystverket.no
        h.Receive( @"\g:2-2-8055*55\!AIVDM,2,2,6,B,j`888888880,2*2B" );
        var p = h.Complete();
        p.OnNextCalls.ShouldBeEmpty();
        p.OnErrorCalls.Count.ShouldBe( 1 );
        // ais.kystverket.no
        ShouldBeIncompleteMessageError( p.OnErrorCalls[0], @"\g:2-2-8055*55\!AIVDM,2,2,6,B,j`888888880,2*2B", 1 );
    }

    // MaximumUnmatchedFragmentAge = 1: the 2nd fragment, two single-fragment messages, then the 1st fragment.
    // The single ones are reported; the 2nd fragment expires on the 3rd line: an "incomplete fragmented message" error
    // with the 2nd fragment and line number 1. The 1st fragment, too late, is not reassembled with the expired 2nd one:
    // it starts a new message that is reported as incomplete (line number 4) when the stream completes.
    [Test]
    public void Second_fragment_received_without_the_first_one_expires()
    {
        var h = new AdapterHarness( new NmeaParserOptions { MaximumUnmatchedFragmentAge = 1 } );
        // ais.kystverket.no
        h.Receive( @"\g:2-2-8055*55\!AIVDM,2,2,6,B,j`888888880,2*2B" );
        // ais.kystverket.no
        h.Receive( @"\s:42,c:1567684904*38\!AIVDM,1,1,,B,33m9UtPP@50wwE:VJW6LS67H01<@,0*3C" );
        // ais.kystverket.no
        h.Receive( @"\s:3,c:1567692251*01\!AIVDM,1,1,,A,13m9WS001d0K==pR=D?HB6WD0pJV,0*4D" );
        // ais.kystverket.no
        h.Receive( @"\g:1-2-8055,s:99,c:1567685556*4E\!AIVDM,2,1,6,B,53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4l,0*72" );
        var p = h.Complete();
        p.OnNextCalls.Count.ShouldBe( 2 );
        // ais.kystverket.no
        ShouldBeMessage( p.OnNextCalls[0], "33m9UtPP@50wwE:VJW6LS67H01<@", 0, 42, 1567684904 );
        // ais.kystverket.no
        ShouldBeMessage( p.OnNextCalls[1], "13m9WS001d0K==pR=D?HB6WD0pJV", 0, 3, 1567692251 );
        p.OnErrorCalls.Count.ShouldBe( 2 );
        // ais.kystverket.no
        ShouldBeIncompleteMessageError( p.OnErrorCalls[0], @"\g:2-2-8055*55\!AIVDM,2,2,6,B,j`888888880,2*2B", 1 );
        // ais.kystverket.no
        ShouldBeIncompleteMessageError( p.OnErrorCalls[1], @"\g:1-2-8055,s:99,c:1567685556*4E\!AIVDM,2,1,6,B,53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4l,0*72", 4 );
    }

    // The 2 fragments (with tag block grouping) in reverse order: the fragments are stored by their number, not by
    // their arrival order, so the message is reassembled as when they are received in order (padding of the last
    // fragment, source and timestamp of the 1st fragment, even if it is the last received line).
    [Test]
    public void Two_fragment_message_fragments_received_in_reverse_order()
    {
        var h = new AdapterHarness();
        // ais.kystverket.no
        h.Receive( @"\g:2-2-8055*55\!AIVDM,2,2,6,B,j`888888880,2*2B" );
        // ais.kystverket.no
        h.Receive( @"\g:1-2-8055,s:99,c:1567685556*4E\!AIVDM,2,1,6,B,53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4l,0*72" );
        var p = h.Complete();
        p.OnNextCalls.Count.ShouldBe( 1 );
        // ais.kystverket.no
        ShouldBeMessage( p.OnNextCalls[0], "53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4lj`888888880", 2, 99, 1567685556 );
        p.OnErrorCalls.ShouldBeEmpty();
    }

    // The 2 fragments (without tag block grouping, grouped by the AIS sentence) in reverse order: one reassembled
    // message. The 2nd fragment has no timestamp: the one of the message comes from the 1st fragment.
    [Test]
    public void Two_fragment_message_fragments_without_grouping_in_header_received_in_reverse_order()
    {
        var h = new AdapterHarness();
        // ais.kystverket.no
        h.Receive( @"\s:99*49\!AIVDM,2,2,6,B,j`888888880,2*2B" );
        // ais.kystverket.no
        h.Receive( @"\s:99,c:1567685556*34\!AIVDM,2,1,6,B,53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4l,0*72" );
        var p = h.Complete();
        p.OnNextCalls.Count.ShouldBe( 1 );
        // ais.kystverket.no
        ShouldBeMessage( p.OnNextCalls[0], "53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4lj`888888880", 2, 99, 1567685556 );
        p.OnErrorCalls.ShouldBeEmpty();
    }

    // Only the first 2 fragments of a 3-fragment message, a single-fragment message in the middle, then the end of
    // the stream: the incomplete message is reported with its 2nd fragment and the line number of this fragment (3).
    // (Before, the line number was the one of the 1st fragment (1), which did not match the reported line.)
    [Test]
    public void Incomplete_message_is_reported_with_the_line_number_of_the_reported_fragment()
    {
        var h = new AdapterHarness();
        // ais.kystverket.no
        h.Receive( @"\g:1-3-3451,s:27,c:1567686150*40\!AIVDM,3,1,9,A,544MR0827oeaD<u0000lDdP4pTf0duAA,0*17" );
        // ais.kystverket.no
        h.Receive( @"\s:42,c:1567684904*38\!AIVDM,1,1,,B,33m9UtPP@50wwE:VJW6LS67H01<@,0*3C" );
        // ais.kystverket.no
        h.Receive( @"\g:2-3-3451*5F\!AIVDM,3,2,9,A,<uH000167pF=2=nG0:0DRj0CQiC4jh00,0*4A" );
        var p = h.Complete();
        p.OnNextCalls.Count.ShouldBe( 1 );
        // ais.kystverket.no
        ShouldBeMessage( p.OnNextCalls[0], "33m9UtPP@50wwE:VJW6LS67H01<@", 0, 42, 1567684904 );
        p.OnErrorCalls.Count.ShouldBe( 1 );
        // ais.kystverket.no
        ShouldBeIncompleteMessageError( p.OnErrorCalls[0], @"\g:2-3-3451*5F\!AIVDM,3,2,9,A,<uH000167pF=2=nG0:0DRj0CQiC4jh00,0*4A", 3 );
    }

    // EmptyGroupTolerance.AutoFix: a 3-sentence group where only the first line has a (complete, 1/1) sentence and the
    // 2 others are empty: one message, IsFixedMessage is true and the grouping is removed (SentencesInGroup is 0).
    [Test]
    public void Three_fragment_message_with_only_one_sentence_and_auto_fix_group()
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
    public void Three_fragment_message_with_two_sentences_and_auto_fix_group()
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
    public void Three_fragment_message_with_only_one_sentence_and_allow_empty_sentence()
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
    // them as errors, they are forwarded and no message is reported. The group of the first line is never completed:
    // it is reported as an incomplete message when the stream completes.
    // (The source feature had commented-out steps here that did the same through "I parse the content by message".)
    [Test]
    public void Three_fragment_message_with_only_one_sentence_and_empty_sentence_is_not_allowed()
    {
        var h = new AdapterHarness( new NmeaParserOptions { EmptyGroupTolerance = EmptyGroupTolerance.None } );
        h.Receive( @"\g:1-3-1481,s:99,c:1718701847*41\!AIVDM,1,1,,A,13Ef?<3006wt9WlEMhi1S6uJ00Rq,0*41" );
        h.ReceiveErrorReportForInvalidContent( @"\g:2-3-1481*50\", 2 );
        h.ReceiveErrorReportForInvalidContent( @"\g:3-3-1481*51\", 3 );
        var p = h.Complete();
        p.OnNextCalls.ShouldBeEmpty();
        p.OnErrorCalls.Count.ShouldBe( 3 );
        p.OnErrorCalls[0].Line.ShouldBe( @"\g:2-3-1481*50\" );
        p.OnErrorCalls[0].Error.Message.ShouldBe( "Invalid data. The message appears to be missing some characters - it may have been corrupted or truncated." );
        p.OnErrorCalls[1].Line.ShouldBe( @"\g:3-3-1481*51\" );
        p.OnErrorCalls[1].Error.Message.ShouldBe( "Invalid data. The message appears to be missing some characters - it may have been corrupted or truncated." );
        ShouldBeIncompleteMessageError( p.OnErrorCalls[2], @"\g:1-3-1481,s:99,c:1718701847*41\!AIVDM,1,1,,A,13Ef?<3006wt9WlEMhi1S6uJ00Rq,0*41", 1 );
    }

    // AllowNonZeroPaddingInNonLastFragment = false: a first fragment with a padding of 2 is an error, no message.
    [Test]
    public void Multiple_lines_with_non_zero_padding_in_non_last_fragment_disallowed()
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
    public void Multiple_lines_with_non_zero_padding_in_non_last_fragment_allowed()
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
    public void Sentence_without_exclamation_mark_is_disallowed()
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
    public void Sentence_without_exclamation_mark_is_allowed()
    {
        var h = new AdapterHarness( new NmeaParserOptions { ThrowWhenNoExclamationMark = false } );
        h.Receive( @"\s:808,c:1567693618*0A\$AIVDM,1,1,,B,B3o8B<00F8:0h694gOtbgwqUoP06,0*69" );
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

    static void ShouldHaveForwardedError( TestProcessor.ErrorReport e, Exception error )
    {
        e.Line.ShouldBe( "foobar" );
        e.Error.ShouldBeSameAs( error );
        e.LineNumber.ShouldBe( 42 );
    }

    // An incomplete fragmented message is reported with its received fragment of highest number and the line number
    // of this fragment.
    static void ShouldBeIncompleteMessageError( TestProcessor.ErrorReport e, string fragment, int fragmentLineNumber )
    {
        e.Line.ShouldBe( fragment );
        e.Error.ShouldBeAssignableTo<ArgumentException>()!.Message.ShouldBe( "Received incomplete fragmented message." );
        e.LineNumber.ShouldBe( fragmentLineNumber );
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
