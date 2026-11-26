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

    static string EmptyFieldMessage( char field ) => $"Tag block field '{field}' is empty, and empty fields are not allowed";

    // Without a standard, a tag block without group gives its source and timestamp.
    [TestCase( "s:ASS,c:1706745485*72", "ASS", 1706745485L )]
    [TestCase( "s:AIS,c:1706800491*63", "AIS", 1706800491L )]
    public void Unspecified_standard_tag_block_gives_source_and_timestamp( string tagBlock, string source, long timestamp )
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
    public void Unspecified_standard_tag_block_with_group_is_decoded( string tagBlock, string source, long timestamp, int sentence, int total, int groupId )
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
    [Test]
    public void IEC_tag_block_single_line_with_group()
    {
        var parser = Parse( "1G2:7764,s:AIS,c:1706800480*33", TagBlockStandard.IEC );
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
        var processor = await ParseLineByMessageAsync( @"\1G2:9628,s:AIS,c:1701650788*33\!AIVDM,2,1,2,B,55Mv3A`00001L=SKOG9@tlmV0F2222222222220l189446lgN5j3mDm3kc56,0*4E",
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
    public void AllowTagBlockEmptyFields_accepts_empty_fields( string tagBlock )
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
    // The 'd', 'n', 'r' and 'x' fields are ignored (ThrowWhenTagBlockContainsUnknownFields is false here) but their format
    // is checked like the other ones: the library used to skip them without any check.
    [TestCase( "c:,i:,t:,d:,n:,r:,x:,s*1B" )]
    [TestCase( "s:,i:,t:,d:,n:,r:,x:,c*1B" )]
    [TestCase( "s:,c:,t:,d:,n:,r:,x:,i*1B" )]
    [TestCase( "s:,c:,i:,d:,n:,r:,x:,t*1B" )]
    [TestCase( "s:,c:,i:,t:,n:,r:,x:,d*1B" )]
    [TestCase( "s:,c:,i:,t:,d:,r:,x:,n*1B" )]
    [TestCase( "s:,c:,i:,t:,d:,n:,x:,r*1B" )]
    [TestCase( "s:,c:,i:,t:,d:,n:,r:,x*1B" )]
    [TestCase( "s,c:,i:,t:,d:,n:,r:,x:*1B" )]
    [TestCase( "c,s:,i:,t:,d:,n:,r:,x:*1B" )]
    [TestCase( "i,s:,c:,t:,d:,n:,r:,x:*1B" )]
    [TestCase( "t,s:,c:,i:,d:,n:,r:,x:*1B" )]
    [TestCase( "d,s:,c:,i:,t:,n:,r:,x:*1B" )]
    [TestCase( "n,s:,c:,i:,t:,d:,r:,x:*1B" )]
    [TestCase( "r,s:,c:,i:,t:,d:,n:,x:*1B" )]
    [TestCase( "x,s:,c:,i:,t:,d:,n:,r:*1B" )]
    public void AllowTagBlockEmptyFields_still_rejects_field_without_colon( string tagBlock )
    {
        Util.Invokable( () => { _ = Parse( tagBlock, TagBlockStandard.Unspecified, allowTagBlockEmptyFields: true ); } )
            .ShouldThrow<ArgumentException>()
            .Message.ShouldBe( MissingColonMessage );
    }

    // Without AllowTagBlockEmptyFields, an empty field ('s:') is rejected with a dedicated message.
    // Fixed: same as above, the SpecFlow step passed when nothing was thrown.
    // As above, the ignored 'd', 'n', 'r' and 'x' fields are checked too.
    [TestCase( "s:*49", 's' )]
    [TestCase( "c:*59", 'c' )]
    [TestCase( "i:*53", 'i' )]
    [TestCase( "t:*4E", 't' )]
    [TestCase( "d:*5E", 'd' )]
    [TestCase( "n:*54", 'n' )]
    [TestCase( "r:*48", 'r' )]
    [TestCase( "x:*42", 'x' )]
    public void Empty_fields_are_rejected_without_AllowTagBlockEmptyFields( string tagBlock, char emptyField )
    {
        Util.Invokable( () => { _ = Parse( tagBlock, TagBlockStandard.Unspecified, allowTagBlockEmptyFields: false ); } )
            .ShouldThrow<ArgumentException>()
            .Message.ShouldBe( EmptyFieldMessage( emptyField ) );
    }

    // Without AllowTagBlockEmptyFields, a field without ':' ('s') is rejected (as it is with the option).
    // Fixed: same as above, the SpecFlow step passed when nothing was thrown.
    [TestCase( "s*73" )]
    [TestCase( "c*63" )]
    [TestCase( "i*69" )]
    [TestCase( "t*74" )]
    [TestCase( "d*64" )]
    [TestCase( "n*6E" )]
    [TestCase( "r*72" )]
    [TestCase( "x*78" )]
    public void Fields_without_colon_are_rejected_without_AllowTagBlockEmptyFields( string tagBlock )
    {
        Util.Invokable( () => { _ = Parse( tagBlock, TagBlockStandard.Unspecified, allowTagBlockEmptyFields: false ); } )
            .ShouldThrow<ArgumentException>()
            .Message.ShouldBe( MissingColonMessage );
    }

    // Without AllowTagBlockEmptyFields, an empty field is rejected wherever it is, not only when it ends the tag block
    // (the library used to only reject an empty last field: 's:,c:1567684904' was accepted with an empty source).
    [TestCase( "s:,c:1567684904*3E", 's' )]
    [TestCase( "c:1567684904,s:,i:abc*21", 's' )]
    [TestCase( "i:,s:42*30", 'i' )]
    [TestCase( "d:,s:42*3D", 'd' )]
    [TestCase( "s:42,x:,c:1567684904*56", 'x' )]
    [TestCase( "g:,s:42*3E", 'g' )]
    public void Empty_fields_are_rejected_without_AllowTagBlockEmptyFields_whatever_their_position( string tagBlock, char emptyField )
    {
        Util.Invokable( () => { _ = Parse( tagBlock, TagBlockStandard.Unspecified, allowTagBlockEmptyFields: false ); } )
            .ShouldThrow<ArgumentException>()
            .Message.ShouldBe( EmptyFieldMessage( emptyField ) );
    }

    // With AllowTagBlockEmptyFields, the same tag blocks are accepted and the other fields are decoded.
    [TestCase( "s:,c:1567684904*3E", "", 1567684904L )]
    [TestCase( "c:1567684904,s:,i:abc*21", "", 1567684904L )]
    [TestCase( "i:,s:42*30", "42", null )]
    [TestCase( "d:,s:42*3D", "42", null )]
    [TestCase( "s:42,x:,c:1567684904*56", "42", 1567684904L )]
    [TestCase( "g:,s:42*3E", "42", null )]
    public void Empty_fields_are_accepted_with_AllowTagBlockEmptyFields_whatever_their_position( string tagBlock, string source, long? timestamp )
    {
        var parser = Parse( tagBlock, TagBlockStandard.Unspecified, allowTagBlockEmptyFields: true );
        Encoding.ASCII.GetString( parser.Source ).ShouldBe( source );
        parser.UnixTimestamp.ShouldBe( timestamp );
    }

    // A 'g:' grouping that ends the tag block is parsed, with AllowTagBlockEmptyFields.
    // Fixed: the SpecFlow step "no error message reported" checked a processor that this scenario never fed.
    // The check is now that building the parser does not throw.
    [Test]
    public void Group_field_at_the_end_of_tag_block_is_parsed()
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
    public void Group_field_at_the_end_of_tag_block_is_parsed_without_AllowTagBlockEmptyFields()
    {
        const string tagBlock = "i:<O>IRL</O>,c:1738367940,g:2-2-1470*2F";
        Util.Invokable( () => { _ = Parse( tagBlock, TagBlockStandard.Unspecified, allowTagBlockEmptyFields: false ); } ).ShouldNotThrow();
        var parser = Parse( tagBlock, TagBlockStandard.Unspecified, allowTagBlockEmptyFields: false );
        Encoding.ASCII.GetString( parser.TextString ).ShouldBe( "<O>IRL</O>" );
        parser.UnixTimestamp.ShouldBe( 1738367940L );
        CheckGrouping( parser, 2, 2, 1470 );
    }

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
