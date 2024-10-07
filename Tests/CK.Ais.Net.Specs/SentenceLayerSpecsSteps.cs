// <copyright file="SentenceLayerSpecsSteps.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using NUnit.Framework;
using System;
using System.Text;
using TechTalk.SpecFlow;

namespace Ais.Net.Specs;

[Binding]
public class SentenceLayerSpecsSteps
{
    ParserMaker? _makeParser;

    delegate NmeaLineParser<DefaultExtraFieldParser> ParserMaker();

    delegate void ParserTest( NmeaLineParser<DefaultExtraFieldParser> parser );

    [When( "I parse a message with no tag block" )]
    public void WhenIParseAMessageWithNoTagBlock()
    {
        When( AivdmExamples.SomeSortOfAivdmMessageRenameThisWhenWeKnowMore );
    }

    [When( "I parse a message with a tag block" )]
    public void WhenIParseAMessageWithATagBlock()
    {
        When( AivdmExamples.SomeSortOfAivdmMessageRenameThisWhenWeKnowMoreWithTagBlock );
    }

    [When( "I parse a message with a packet tag field of '(.*)'" )]
    public void WhenIParseAMessageWithAPacketTagFieldOf( string tag )
    {
        When( string.Format( AivdmExamples.MessageWithTaAGPLaceholderFormat, tag ) );
    }

    [When( "I parse a message fragment part (.*) of (.*) with message id (.*) and sentence group id (.*)" )]
    public void WhenIParseAMessageFragmentPartOfWithMessageIdAndSentenceGroupId(
        int currentFragment, int totalFragments, string sequentialMessageId, string sentenceGroupId )
    {
        When( string.Format( AivdmExamples.MessageFragmentFormat, currentFragment, totalFragments, sequentialMessageId, sentenceGroupId ) );
    }

    [When( "I parse a message fragment part (.*) of (.*) with message id (.*) and no sentence group id" )]
    public void WhenIParseAMessageFragmentPartOfWithMessageIdAndNoSentenceGroupId(
        int currentFragment, int totalFragments, string sequentialMessageId )
    {
        When( string.Format( AivdmExamples.MessageFragmentWithoutGroupInHeaderFormat, currentFragment, totalFragments, sequentialMessageId ) );
    }

    [When( "I parse a non-fragmented message" )]
    public void WhenIParseANon_FragmentedMessage()
    {
        When( AivdmExamples.NonFragmentedMessage );
    }

    [When( "I parse a message with a radio channel code of '(.*)'" )]
    public void WhenIParseAMessageWithARadioChannelCodeOf( string channel )
    {
        When( string.Format( AivdmExamples.MessageWithRadioChannelPlaceholderFormat, channel ) );
    }

    [When( "I parse a message with a payload of '(.*)'" )]
    public void WhenIParseAMessageWithAPayloadOf( string payload )
    {
        When( string.Format( AivdmExamples.MessageWithPayloadPlaceholderFormat, payload ) );
    }

    [When( "I parse a message with padding of (.*)" )]
    public void WhenIParseAMessageWithAPaddingOf( int padding )
    {
        When( string.Format( AivdmExamples.MessageWithPaddinAGPLaceholderFormat, padding ) );
    }

    [Then( "the TagBlockWithoutDelimiters property's Length should be (.*)" )]
    public void ThenTheTagBlockWithoutDelimitersLengthShouldBe( int expectedLength )
    {
        Then( parser => Assert.AreEqual( expectedLength, parser.TagBlockAsciiWithoutDelimiters.Length ) );
    }

    [Then( "the TagBlockWithoutDelimiters property should match the tag block without the delimiters" )]
    public void ThenTheTagBlockWithoutDelimitersPropertyShouldMatchTheTagBlockWithoutTheDelimiters()
    {
        Then( parser =>
        {
            string parsedTagBlock = Encoding.ASCII.GetString( parser.TagBlockAsciiWithoutDelimiters );
            Assert.AreEqual( AivdmExamples.SimpleTagBlockWithoutDelimiters, parsedTagBlock );
        } );
    }

    [Then( "the AisTalker is '(.*)'" )]
    public void ThenTheAisTalkerIs( TalkerId talkerId )
    {
        Then( parser => Assert.AreEqual( talkerId, parser.AisTalker ) );
    }

    [Then( "the DataOrigin is '(.*)'" )]
    public void ThenTheDataOriginIs( VesselDataOrigin dataOrigin )
    {
        Then( parser => Assert.AreEqual( dataOrigin, parser.DataOrigin ) );
    }

    [Then( "the TotalFragmentCount is '(.*)'" )]
    public void ThenTheTotalFragmentCountIs( int totalFragments )
    {
        Then( parser => Assert.AreEqual( totalFragments, parser.TotalFragmentCount ) );
    }

    [Then( "the FragmentNumberOneBased is '(.*)'" )]
    public void ThenTheFragmentNumberOneBasedIs( int currentFragment )
    {
        Then( parser => Assert.AreEqual( currentFragment, parser.FragmentNumberOneBased ) );
    }

    [Then( "the MultiSequenceMessageId is '(.*)'" )]
    public void ThenTheMultiSequenceMessageIdIs( string sequentialMessageId )
    {
        Then( parser =>
        {
            string parsedMessageId = Encoding.ASCII.GetString( parser.MultiSequenceMessageId );
            Assert.AreEqual( sequentialMessageId, parsedMessageId );
        } );
    }

    [Then( "the MultiSequenceMessageId is empty" )]
    public void ThenTheMultiSequenceMessageIdIsEmpty()
    {
        Then( parser => Assert.IsTrue( parser.MultiSequenceMessageId.IsEmpty ) );
    }

    [Then( "the TagBlockSentenceGrouping is not present" )]
    public void ThenTheTagBlockSentenceGroupingIsNotPresent()
    {
        Then( parser => Assert.IsFalse( parser.TagBlock.SentenceGrouping.HasValue ) );
    }

    [Then( "the SentenceGroupId is '(.*)'" )]
    public void ThenTheSentenceGroupIdIs( int sentenceGroupId )
    {
        Then( parser =>
        {
            Assert.IsTrue( parser.TagBlock.SentenceGrouping.HasValue );
            Assert.AreEqual( sentenceGroupId, parser.TagBlock.SentenceGrouping!.Value.GroupId );
        } );
    }

    [Then( "the ChannelCode is '(.*)'" )]
    public void ThenTheChannelCodeIs( char channelCode )
    {
        Then( parser => Assert.AreEqual( channelCode, parser.ChannelCode ) );
    }

    [Then( "the payload is '(.*)'" )]
    public void ThenThePayloadIs( string payload )
    {
        Then( parser =>
        {
            string parsedPayload = Encoding.ASCII.GetString( parser.Payload );
            Assert.AreEqual( payload, parsedPayload );
        } );
    }

    [Then( "the padding is (.*)" )]
    public void ThenThePaddingIs( int padding )
    {
        Then( parser => Assert.AreEqual( padding, parser.Padding ) );
    }

    void When( string messageLine )
    {
        When( () => new NmeaLineParser<DefaultExtraFieldParser>( Encoding.ASCII.GetBytes( messageLine ) ) );
    }

    void When( ParserMaker makeParser )
    {
        _makeParser = makeParser;
    }

    void Then( ParserTest test )
    {
        if( _makeParser is null ) throw new InvalidOperationException( $"When step must be called." );
        NmeaLineParser<DefaultExtraFieldParser> parser = _makeParser();
        test( parser );
    }
}
