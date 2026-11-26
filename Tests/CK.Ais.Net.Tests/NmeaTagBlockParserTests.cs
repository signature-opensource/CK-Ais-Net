// <copyright file="NmeaTagBlockParserTests.cs" company="Endjin Limited">
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
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Ais.Net.Tests;

// NmeaTagBlockParser parses the tag block of an NMEA line (the '\...\' section, without delimiters):
// source, timestamp, text string and sentence grouping, according to the TagBlockStandard
// (Unspecified accepts both, IEC 62320-1 and NMEA 4.10 reject the other one's grouping and text fields),
// non-standard fields handled by an INmeaTagBlockExtraFieldParser, and the AllowTagBlockEmptyFields option.
// Errors raised when a whole line is parsed by NmeaStreamParser are checked on the TestProcessor's OnError calls.
[TestFixture]
public class NmeaTagBlockParserTests
{
    const string MissingColonMessage = "Tag block entries should start with a type character followed by a colon, and there was no colon";

    // Without a standard, a tag block without group gives its source and timestamp.
    [TestCase( "s:ASS,c:1706745485*72", "ASS", 1706745485L )]
    [TestCase( "s:AIS,c:1706800491*63", "AIS", 1706800491L )]
    public void unspecified_standard_tag_block( string tagBlock, string source, long timestamp )
    {
        var parser = Parse( tagBlock, TagBlockStandard.Unspecified );
        Encoding.ASCII.GetString( parser.Source ).ShouldBe( source );
        parser.UnixTimestamp.ShouldBe( timestamp );
        parser.SentenceGrouping.HasValue.ShouldBeFalse();
    }

    // Without a standard, both the NMEA ('g:1-2-7764') and the IEC ('1G2:7764') groupings are accepted.
    // Fixed: the SpecFlow step hard-coded 'g:1-2-7764,s:AIS,c:1706800480*13' instead of '<payload>',
    // so the IEC row ('1G2:7764,...') was never parsed. Each row now parses its own tag block.
    [TestCase( "g:1-2-7764,s:AIS,c:1706800480*13", "AIS", 1706800480L, 1, 2, 7764 )]
    [TestCase( "1G2:7764,s:AIS,c:1706800480*33", "AIS", 1706800480L, 1, 2, 7764 )]
    public void unspecified_standard_tag_block_with_group( string tagBlock, string source, long timestamp, int sentence, int total, int groupId )
    {
        var parser = Parse( tagBlock, TagBlockStandard.Unspecified );
        Encoding.ASCII.GetString( parser.Source ).ShouldBe( source );
        parser.UnixTimestamp.ShouldBe( timestamp );
        CheckGrouping( parser, sentence, total, groupId );
    }

    // The IEC standard parses a tag block without group.
    [TestCase( "s:ASS,c:1706745485*72", "ASS", 1706745485L )]
    [TestCase( "s:AIS,c:1706800491*63", "AIS", 1706800491L )]
    public void IEC_tag_block_single_line( string tagBlock, string source, long timestamp )
    {
        var parser = Parse( tagBlock, TagBlockStandard.IEC );
        Encoding.ASCII.GetString( parser.Source ).ShouldBe( source );
        parser.UnixTimestamp.ShouldBe( timestamp );
        parser.SentenceGrouping.HasValue.ShouldBeFalse();
    }

    // The IEC standard parses its '<int>G<int>:<int>' grouping.
    // (The checksum value is not validated by ChecksumOption.ValidateStandardFormat, only its format.)
    [Test]
    public void IEC_tag_block_single_line_with_group()
    {
        var parser = Parse( "1G2:7764,s:AIS,c:1706800480*13", TagBlockStandard.IEC );
        Encoding.ASCII.GetString( parser.Source ).ShouldBe( "AIS" );
        parser.UnixTimestamp.ShouldBe( 1706800480L );
        CheckGrouping( parser, 1, 2, 7764 );
    }

    // The IEC standard reports an error for an NMEA 'g:' grouping.
    [Test]
    public async Task IEC_tag_block_but_Nmea_group_Async()
    {
        var processor = await ParseLineByMessageAsync( @"\g:1-2-9628,s:AIS,c:1701650788*13\!AIVDM,2,1,2,B,55Mv3A`00001L=SKOG9@tlmV0F2222222222220l189446lgN5j3mDm3kc56,0*4E",
                                                       throwWhenTagBlockContainsUnknownFields: false,
                                                       TagBlockStandard.IEC );
        processor.OnErrorCalls[0].Error.Message.ShouldBe( "Tag block sentence grouping should be <int>G<int>:<int>, but first part was not a decimal integer" );
    }

    // The NMEA standard parses a tag block without group.
    [TestCase( "s:ASS,c:1706745485*72", "ASS", 1706745485L )]
    [TestCase( "s:AIS,c:1706800491*63", "AIS", 1706800491L )]
    public void Nmea_tag_block_single_line( string tagBlock, string source, long timestamp )
    {
        var parser = Parse( tagBlock, TagBlockStandard.Nmea );
        Encoding.ASCII.GetString( parser.Source ).ShouldBe( source );
        parser.UnixTimestamp.ShouldBe( timestamp );
        parser.SentenceGrouping.HasValue.ShouldBeFalse();
    }

    // The NMEA standard parses its 'g:<int>-<int>-<int>' grouping.
    [Test]
    public void Nmea_tag_block_single_line_with_group()
    {
        var parser = Parse( "g:1-2-7764,s:AIS,c:1706800480*13", TagBlockStandard.Nmea );
        Encoding.ASCII.GetString( parser.Source ).ShouldBe( "AIS" );
        parser.UnixTimestamp.ShouldBe( 1706800480L );
        CheckGrouping( parser, 1, 2, 7764 );
    }

    // The NMEA standard reports an error for an IEC '1G2:...' grouping.
    [Test]
    public async Task Nmea_tag_block_but_IEC_group_Async()
    {
        var processor = await ParseLineByMessageAsync( @"\1G2:9628,s:AIS,c:1701650788*13\!AIVDM,2,1,2,B,55Mv3A`00001L=SKOG9@tlmV0F2222222222220l189446lgN5j3mDm3kc56,0*4E",
                                                       throwWhenTagBlockContainsUnknownFields: false,
                                                       TagBlockStandard.Nmea );
        processor.OnErrorCalls[0].Error.Message.ShouldBe( "Tag block sentence grouping should be <int>-<int>-<int>, but first part was not a decimal integer" );
    }

    // The IEC standard reads the text string from the 'i:' field.
    [TestCase( "c:1673149953,i:<O>ES</O>*1F", "<O>ES</O>" )]
    [TestCase( "c:1673149955,i:<O>GI</O>*01", "<O>GI</O>" )]
    public void IEC_tag_block_TextString( string tagBlock, string text )
    {
        var parser = Parse( tagBlock, TagBlockStandard.IEC );
        Encoding.ASCII.GetString( parser.TextString ).ShouldBe( text );
    }

    // The NMEA standard reads the text string from the 't:' field.
    [TestCase( "c:1673149951,t:<O>ES</O>*00", "<O>ES</O>" )]
    [TestCase( "c:1673149954,t:<O>RO</O>*0E", "<O>RO</O>" )]
    public void Nmea_tag_block_TextString( string tagBlock, string text )
    {
        var parser = Parse( tagBlock, TagBlockStandard.Nmea );
        Encoding.ASCII.GetString( parser.TextString ).ShouldBe( text );
    }

    // The IEC standard reports an error for the NMEA 't:' text field.
    [Test]
    public async Task IEC_tag_block_TextString_but_Nmea_TextString_Async()
    {
        var processor = await ParseLineByMessageAsync( @"\c:1673149951,t:<O>ES</O>*00\!AIVDM,2,1,2,B,55Mv3A`00001L=SKOG9@tlmV0F2222222222220l189446lgN5j3mDm3kc56,0*4E",
                                                       throwWhenTagBlockContainsUnknownFields: false,
                                                       TagBlockStandard.IEC );
        processor.OnErrorCalls[0].Error.Message.ShouldBe( "Unknown field type in IEC tag block: t" );
    }

    // The NMEA standard reports an error for the IEC 'i:' text field.
    [Test]
    public async Task Nmea_tag_block_TextString_but_IEC_TextString_Async()
    {
        var processor = await ParseLineByMessageAsync( @"\c:1673149953,i:<O>ES</O>*1F\!AIVDM,2,1,2,B,55Mv3A`00001L=SKOG9@tlmV0F2222222222220l189446lgN5j3mDm3kc56,0*4E",
                                                       throwWhenTagBlockContainsUnknownFields: false,
                                                       TagBlockStandard.Nmea );
        processor.OnErrorCalls[0].Error.Message.ShouldBe( "Unknown field type in Nmea tag block: i" );
    }

    // A non-standard field ('q:') is an error when unknown fields must throw and the default extra field parser is used.
    [Test]
    public async Task Nmea_tag_block_extra_fields_but_no_parser_Async()
    {
        var processor = await ParseLineByMessageAsync( @"\s:KIN1B,c:1716810431,q:mt-pt-ct-st-kt*78\!AIVDM,1,1,,B,100BkthL2fmlG@@iC=w2CQfF0Gh8,0*38",
                                                       throwWhenTagBlockContainsUnknownFields: true,
                                                       TagBlockStandard.Unspecified );
        processor.OnErrorCalls[0].Error.Message.ShouldBe( "Unknown field type: q" );
    }

    // A custom extra field parser captures the non-standard fields it knows ('q:' and 'v:').
    [TestCase( "s:KIN1B,c:1716810431,q:mt-pt-ct-st-kt*78", "mt-pt-ct-st-kt", "" )]
    [TestCase( "s:KIN1B,c:1716810431,q:mt-pt-ct-st-kt,v:123*28", "mt-pt-ct-st-kt", "123" )]
    public void Nmea_tag_block_extra_fields_with_extra_parser( string tagBlock, string qValue, string vValue )
    {
        var parser = new NmeaTagBlockParser<ExtraFieldParser>( Encoding.ASCII.GetBytes( tagBlock ),
                                                               throwWhenTagBlockContainsUnknownFields: true,
                                                               TagBlockStandard.Unspecified,
                                                               allowTagBlockEmptyFields: false,
                                                               ChecksumOption.ValidateStandardFormat );
        Encoding.ASCII.GetString( parser.ExtraFieldParser.GetQValue( parser.OriginalSpan ) ).ShouldBe( qValue );
        Encoding.ASCII.GetString( parser.ExtraFieldParser.GetVValue( parser.OriginalSpan ) ).ShouldBe( vValue );
    }

    // With AllowTagBlockEmptyFields, every known field can be empty (whatever its position) and gives empty/null values.
    // Fixed: the SpecFlow step "there are no error" checked the OnErrorCalls of a processor that this scenario
    // never fed (so it was always empty). The check is now that building the parser does not throw.
    [TestCase( "c:,i:,t:,d:,n:,r:,x:,s:*21" )]
    [TestCase( "s:,i:,t:,d:,n:,r:,x:,c:*21" )]
    [TestCase( "s:,c:,t:,d:,n:,r:,x:,i:*21" )]
    [TestCase( "s:,c:,i:,d:,n:,r:,x:,t:*21" )]
    [TestCase( "s:,c:,i:,t:,n:,r:,x:,d:*21" )]
    [TestCase( "s:,c:,i:,t:,d:,r:,x:,n:*21" )]
    [TestCase( "s:,c:,i:,t:,d:,n:,x:,r:*21" )]
    [TestCase( "s:,c:,i:,t:,d:,n:,r:,x:*21" )]
    public void allow_tag_block_empty_fields( string tagBlock )
    {
        Util.Invokable( () => { _ = Parse( tagBlock, TagBlockStandard.Unspecified, allowTagBlockEmptyFields: true ); } ).ShouldNotThrow();
        var parser = Parse( tagBlock, TagBlockStandard.Unspecified, allowTagBlockEmptyFields: true );
        parser.Source.IsEmpty.ShouldBeTrue();
        parser.UnixTimestamp.HasValue.ShouldBeFalse();
        parser.TextString.IsEmpty.ShouldBeTrue();
        parser.SentenceGrouping.HasValue.ShouldBeFalse();
    }

    // Even with AllowTagBlockEmptyFields, a field without its ':' (first or last) is rejected.
    // Fixed: the SpecFlow step "the parser throw an error message" only asserted inside a catch, so it also
    // passed when nothing was thrown. It now requires an ArgumentException with the expected message.
    // The rows whose malformed field is 'd', 'n', 'r' or 'x' are [Explicit]: the library skips these unsupported
    // fields (when ThrowWhenTagBlockContainsUnknownFields is false) without validating them, so nothing is thrown.
    [TestCase( "c:,i:,t:,d:,n:,r:,x:,s*21" )]
    [TestCase( "s:,i:,t:,d:,n:,r:,x:,c*21" )]
    [TestCase( "s:,c:,t:,d:,n:,r:,x:,i*21" )]
    [TestCase( "s:,c:,i:,d:,n:,r:,x:,t*21" )]
    [TestCase( "s:,c:,i:,t:,n:,r:,x:,d*21", Explicit = true, Reason = UnvalidatedUnsupportedField )]
    [TestCase( "s:,c:,i:,t:,d:,r:,x:,n*21", Explicit = true, Reason = UnvalidatedUnsupportedField )]
    [TestCase( "s:,c:,i:,t:,d:,n:,x:,r*21", Explicit = true, Reason = UnvalidatedUnsupportedField )]
    [TestCase( "s:,c:,i:,t:,d:,n:,r:,x*21", Explicit = true, Reason = UnvalidatedUnsupportedField )]
    [TestCase( "s,c:,i:,t:,d:,n:,r:,x:*21" )]
    [TestCase( "c,s:,i:,t:,d:,n:,r:,x:*21" )]
    [TestCase( "i,s:,c:,t:,d:,n:,r:,x:*21" )]
    [TestCase( "t,s:,c:,i:,d:,n:,r:,x:*21" )]
    [TestCase( "d,s:,c:,i:,t:,n:,r:,x:*21", Explicit = true, Reason = UnvalidatedUnsupportedField )]
    [TestCase( "n,s:,c:,i:,t:,d:,r:,x:*21", Explicit = true, Reason = UnvalidatedUnsupportedField )]
    [TestCase( "r,s:,c:,i:,t:,d:,n:,x:*21", Explicit = true, Reason = UnvalidatedUnsupportedField )]
    [TestCase( "x,s:,c:,i:,t:,d:,n:,r:*21", Explicit = true, Reason = UnvalidatedUnsupportedField )]
    public void allow_tag_block_empty_fields_but_invalid_field( string tagBlock )
    {
        Util.Invokable( () => { _ = Parse( tagBlock, TagBlockStandard.Unspecified, allowTagBlockEmptyFields: true ); } )
            .ShouldThrow<ArgumentException>()
            .Message.ShouldBe( MissingColonMessage );
    }

    // Without AllowTagBlockEmptyFields, an empty field ('s:') or a field without ':' ('s') is rejected.
    // Fixed: same as above, the SpecFlow step passed when nothing was thrown.
    // The rows whose field is 'd', 'n', 'r' or 'x' are [Explicit]: the library skips these unsupported
    // fields (when ThrowWhenTagBlockContainsUnknownFields is false) without validating them, so nothing is thrown
    // although the AllowTagBlockEmptyFields documentation says that empty fields are invalid when it is false.
    [TestCase( "s:*49" )]
    [TestCase( "c:*59" )]
    [TestCase( "i:*53" )]
    [TestCase( "t:*4E" )]
    [TestCase( "d:*5E", Explicit = true, Reason = UnvalidatedUnsupportedField )]
    [TestCase( "n:*54", Explicit = true, Reason = UnvalidatedUnsupportedField )]
    [TestCase( "r:*48", Explicit = true, Reason = UnvalidatedUnsupportedField )]
    [TestCase( "x:*42", Explicit = true, Reason = UnvalidatedUnsupportedField )]
    [TestCase( "s*49" )]
    [TestCase( "c*59" )]
    [TestCase( "i*53" )]
    [TestCase( "t*4E" )]
    [TestCase( "d*5E", Explicit = true, Reason = UnvalidatedUnsupportedField )]
    [TestCase( "n*54", Explicit = true, Reason = UnvalidatedUnsupportedField )]
    [TestCase( "r*48", Explicit = true, Reason = UnvalidatedUnsupportedField )]
    [TestCase( "x*42", Explicit = true, Reason = UnvalidatedUnsupportedField )]
    public void disallow_tag_block_empty_fields( string tagBlock )
    {
        Util.Invokable( () => { _ = Parse( tagBlock, TagBlockStandard.Unspecified, allowTagBlockEmptyFields: false ); } )
            .ShouldThrow<ArgumentException>()
            .Message.ShouldBe( MissingColonMessage );
    }

    // A 'g:' grouping that ends the tag block is parsed, with AllowTagBlockEmptyFields.
    // Fixed: the SpecFlow step "no error message reported" checked a processor that this scenario never fed.
    // The check is now that building the parser does not throw.
    [Test]
    public void group_field_is_the_last_of_the_tag_block()
    {
        const string tagBlock = "i:<O>IRL</O>,c:1738367940,g:2-2-1470*2F";
        Util.Invokable( () => { _ = Parse( tagBlock, TagBlockStandard.Unspecified, allowTagBlockEmptyFields: true ); } ).ShouldNotThrow();
        var parser = Parse( tagBlock, TagBlockStandard.Unspecified, allowTagBlockEmptyFields: true );
        Encoding.ASCII.GetString( parser.TextString ).ShouldBe( "<O>IRL</O>" );
        parser.UnixTimestamp.ShouldBe( 1738367940L );
        CheckGrouping( parser, 2, 2, 1470 );
    }

    // A 'g:' grouping that ends the tag block is also parsed without AllowTagBlockEmptyFields: it is not an empty field.
    // Fixed: the SpecFlow scenario was named "...throw an error when allowTagBlockEmptyFields is false" but checked
    // that there is no error (with the commented-out expectation of the MissingColonMessage error). Throwing here was
    // the bug fixed by commit 0105c02 ("Fix error thrown when g: is the last field of the tag block"): the field is
    // complete ("g:2-2-1470"), so no error is the correct behaviour and the test has been renamed accordingly.
    // As above, the "no error message reported" step is replaced by checking that building the parser does not throw.
    [Test]
    public void group_field_is_the_last_of_the_tag_block_and_is_parsed_when_allowTagBlockEmptyFields_is_false()
    {
        const string tagBlock = "i:<O>IRL</O>,c:1738367940,g:2-2-1470*2F";
        Util.Invokable( () => { _ = Parse( tagBlock, TagBlockStandard.Unspecified, allowTagBlockEmptyFields: false ); } ).ShouldNotThrow();
        var parser = Parse( tagBlock, TagBlockStandard.Unspecified, allowTagBlockEmptyFields: false );
        Encoding.ASCII.GetString( parser.TextString ).ShouldBe( "<O>IRL</O>" );
        parser.UnixTimestamp.ShouldBe( 1738367940L );
        CheckGrouping( parser, 2, 2, 1470 );
    }

    const string UnvalidatedUnsupportedField = "Library defect: unsupported 'd', 'n', 'r' and 'x' fields are skipped without checking their 'x:' form, so no error is raised.";

    static NmeaTagBlockParser<DefaultExtraFieldParser> Parse( string tagBlock, TagBlockStandard tagBlockStandard, bool allowTagBlockEmptyFields = false )
    {
        return new NmeaTagBlockParser<DefaultExtraFieldParser>( Encoding.ASCII.GetBytes( tagBlock ),
                                                                throwWhenTagBlockContainsUnknownFields: false,
                                                                tagBlockStandard,
                                                                allowTagBlockEmptyFields,
                                                                ChecksumOption.ValidateStandardFormat );
    }

    static void CheckGrouping( in NmeaTagBlockParser<DefaultExtraFieldParser> parser, int sentenceNumber, int sentencesInGroup, int groupId )
    {
        parser.SentenceGrouping.HasValue.ShouldBeTrue();
        parser.SentenceGrouping!.Value.SentenceNumber.ShouldBe( sentenceNumber );
        parser.SentenceGrouping.Value.SentencesInGroup.ShouldBe( sentencesInGroup );
        parser.SentenceGrouping.Value.GroupId.ShouldBe( groupId );
    }

    // Parses a whole NMEA line with NmeaStreamParser (by message) and returns the processor that received the errors.
    static async Task<TestProcessor> ParseLineByMessageAsync( string line, bool throwWhenTagBlockContainsUnknownFields, TagBlockStandard tagBlockStandard )
    {
        var processor = new TestProcessor();
        await NmeaStreamParser.ParseStreamAsync(
            new MemoryStream( Encoding.ASCII.GetBytes( line + "\n" ) ),
            new NmeaLineToAisStreamAdapter<DefaultExtraFieldParser>( processor ),
            new NmeaParserOptions { ThrowWhenTagBlockContainsUnknownFields = throwWhenTagBlockContainsUnknownFields, TagBlockStandard = tagBlockStandard } ).ConfigureAwait( false );
        return processor;
    }

    // Example of an extra field parser for the non-standard 'q' and 'v' fields: it only stores
    // the ranges of the values, that are then read from the tag block parser's OriginalSpan.
    struct ExtraFieldParser : INmeaTagBlockExtraFieldParser
    {
        public Range QValue { get; private set; }

        public Range VValue { get; private set; }

        public bool TryParseField( ReadOnlySpan<byte> source, ReadOnlySpan<byte> field, int fieldOffset )
        {
            Debug.Assert( source[new Range()].IsEmpty, "Proves that default range applied to any ReadOnlySpan gives an empty ReadOnlySpan." );
            if( field.Length < 2 || field[1] != ':' ) return false;
            switch( field[0] )
            {
                case (byte)'q':
                    QValue = new Range( fieldOffset + 2, fieldOffset + field.Length );
                    return true;
                case (byte)'v':
                    VValue = new Range( fieldOffset + 2, fieldOffset + field.Length );
                    return true;
                default:
                    return false;
            }
        }

        public readonly ReadOnlySpan<byte> GetQValue( ReadOnlySpan<byte> originalSpan ) => GetValue( originalSpan, QValue );

        public readonly ReadOnlySpan<byte> GetVValue( ReadOnlySpan<byte> originalSpan ) => GetValue( originalSpan, VValue );

        // Gets a field value from the original span (the whole tag block) and the range captured while parsing.
        static ReadOnlySpan<byte> GetValue( ReadOnlySpan<byte> originalSpan, Range range ) => originalSpan[range];
    }
}
