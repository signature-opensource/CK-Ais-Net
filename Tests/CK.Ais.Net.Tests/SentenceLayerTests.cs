// <copyright file="SentenceLayerTests.cs" company="Endjin Limited">
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

namespace Ais.Net.Tests;

// NmeaLineParser identifies the main parts of a single NMEA line (as found in an nm4 file):
// the optional tag block, the talker id and data origin, the fragment information
// (from both the AIVDM/AIVDO sentence layer and the tag block 'g' field), the radio channel,
// the payload and the padding.
[TestFixture]
public class SentenceLayerTests
{
    // Without a leading '\...\' section, the tag block is empty.
    [Test]
    public void TagBlock_is_empty_when_not_present()
    {
        var parser = Parse( AivdmExamples.ClassBPositionReportLine );
        parser.TagBlockAsciiWithoutDelimiters.Length.ShouldBe( 0 );
    }

    // The tag block is exposed without its '\' delimiters.
    [Test]
    public void TagBlock_is_exposed_without_delimiters()
    {
        var parser = Parse( AivdmExamples.ClassBPositionReportLineWithTagBlock );
        Encoding.ASCII.GetString( parser.TagBlockAsciiWithoutDelimiters ).ShouldBe( AivdmExamples.SimpleTagBlockWithoutDelimiters );
    }

    // The first two characters of the packet tag identify the AIS talker.
    [TestCase( "ABVDM", TalkerId.BaseStation )]
    [TestCase( "ADVDM", TalkerId.DependentBaseStation )]
    [TestCase( "AIVDM", TalkerId.MobileStation )]
    [TestCase( "ANVDM", TalkerId.AidToNavigationStation )]
    [TestCase( "ARVDM", TalkerId.ReceivingStation )]
    [TestCase( "ASVDM", TalkerId.LimitedBaseStation )]
    [TestCase( "ATVDM", TalkerId.TransmittingStation )]
    [TestCase( "AXVDM", TalkerId.RepeaterStation )]
    [TestCase( "BSVDM", TalkerId.DeprecatedBaseStation )]
    [TestCase( "SAVDM", TalkerId.PhysicalShoreStation )]
    public void AIS_talker_id( string tag, TalkerId expected )
    {
        var parser = Parse( string.Format( AivdmExamples.MessageWithTalkerTagPlaceholderFormat, tag ) );
        parser.AisTalker.ShouldBe( expected );
    }

    // The sentence type (VDM or VDO) tells whether the data comes from another vessel or from our own.
    [TestCase( "AIVDM", VesselDataOrigin.Vdm )]
    [TestCase( "AIVDO", VesselDataOrigin.Vdo )]
    public void AIS_origin( string tag, VesselDataOrigin expected )
    {
        var parser = Parse( string.Format( AivdmExamples.MessageWithTalkerTagPlaceholderFormat, tag ) );
        parser.DataOrigin.ShouldBe( expected );
    }

    // ais.kystverket.no
    // \s:42,c:1567684904*38\!AIVDM,1,1,,A,B3m:H900AP@b:79ae6:<OwnUoP06,0*78
    // A single-sentence message is fragment 1 of 1, without message id nor tag block grouping.
    [Test]
    public void Non_fragmented_message_is_fragment_1_of_1()
    {
        var parser = Parse( AivdmExamples.NonFragmentedMessage );
        parser.TotalFragmentCount.ShouldBe( 1 );
        parser.FragmentNumberOneBased.ShouldBe( 1 );
        parser.MultiSequenceMessageId.IsEmpty.ShouldBeTrue();
        parser.TagBlock.SentenceGrouping.HasValue.ShouldBeFalse();
    }

    // E.g.
    // ais.kystverket.no
    // \g:1-2-8055,s:99,c:1567685556*4E\!AIVDM,2,1,6,B,53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4l,0*72
    // ais.kystverket.no
    // \g:2-2-8055*55\!AIVDM,2,2,6,B,j`888888880,2*2B
    // NMEA 4.10 defines the 'g' tag in the tag block as being <sentence-number>-<total-sentences-in-group>-<group-id>.
    // So in the example above we have:
    //   Sentence number     Total Sentences     Group id
    //       1                   2                   8055
    //       2                   2                   8055
    //
    // The AIVDM/AIVDO sentence layer has the same concept but slightly different nomenclature (and arbitrarily swaps the
    // order of the first two fields, so for those same two messages we'd say:
    //   Total fragments     Fragment number     Message ID
    //       2                   1                   6
    //       2                   2                   6
    //
    // Both the sentence layer fragment information and the tag block group id are exposed.
    [TestCase( 2, 1, "6", 8055 )]
    [TestCase( 2, 2, "6", 8055 )]
    [TestCase( 3, 1, "0", 3451 )]
    [TestCase( 3, 2, "0", 3451 )]
    [TestCase( 3, 3, "0", 3451 )]
    public void Fragment_fields_are_decoded( int totalFragments, int currentFragment, string sequentialMessageId, int sentenceGroupId )
    {
        var parser = Parse( string.Format( AivdmExamples.MessageFragmentFormat, currentFragment, totalFragments, sequentialMessageId, sentenceGroupId ) );
        parser.TotalFragmentCount.ShouldBe( totalFragments );
        parser.FragmentNumberOneBased.ShouldBe( currentFragment );
        Encoding.ASCII.GetString( parser.MultiSequenceMessageId ).ShouldBe( sequentialMessageId );
        parser.TagBlock.SentenceGrouping.HasValue.ShouldBeTrue();
        parser.TagBlock.SentenceGrouping!.Value.GroupId.ShouldBe( sentenceGroupId );
    }

    // Some sources produce fragmented messages without the sentence group IDs in the header, so we have to rely purely on
    // the fragment IDs in the AIVDM/AIVDO layer.
    [TestCase( 2, 1, "6" )]
    [TestCase( 2, 2, "6" )]
    [TestCase( 3, 1, "0" )]
    [TestCase( 3, 2, "0" )]
    [TestCase( 3, 3, "0" )]
    public void Fragment_fields_are_decoded_without_group_in_tag_block( int totalFragments, int currentFragment, string sequentialMessageId )
    {
        var parser = Parse( string.Format( AivdmExamples.MessageFragmentWithoutGroupInHeaderFormat, currentFragment, totalFragments, sequentialMessageId ) );
        parser.TotalFragmentCount.ShouldBe( totalFragments );
        parser.FragmentNumberOneBased.ShouldBe( currentFragment );
        Encoding.ASCII.GetString( parser.MultiSequenceMessageId ).ShouldBe( sequentialMessageId );
    }

    // The radio channel code is exposed as is.
    [TestCase( 'A' )]
    [TestCase( 'B' )]
    [TestCase( '1' )]
    [TestCase( '2' )]
    public void RadioChannelCode_is_decoded( char channelCode )
    {
        var parser = Parse( string.Format( AivdmExamples.MessageWithRadioChannelPlaceholderFormat, channelCode ) );
        parser.ChannelCode.ShouldBe( channelCode );
    }

    // The armored payload is exposed as is.
    [Test]
    public void Payload_is_exposed_as_is()
    {
        const string payload = "B3m:H900AP@b:79ae6:<OwnUoP06"; // ais.kystverket.no
        var parser = Parse( string.Format( AivdmExamples.MessageWithPayloadPlaceholderFormat, payload ) );
        Encoding.ASCII.GetString( parser.Payload ).ShouldBe( payload );
    }

    // The number of padding bits is read from the field after the payload.
    [Test]
    public void Padding_is_decoded()
    {
        var parser = Parse( string.Format( AivdmExamples.MessageWithPaddingPlaceholderFormat, 3 ) );
        parser.Padding.ShouldBe( 3u );
    }

    static NmeaLineParser<DefaultExtraFieldParser> Parse( string line ) => new( Encoding.ASCII.GetBytes( line ) );
}
