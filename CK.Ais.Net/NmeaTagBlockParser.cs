// <copyright file="NmeaTagBlockParser.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System;
using System.Buffers.Text;

namespace Ais.Net
{
    /// <summary>
    /// Extracts data from the Tag Block section of an NMEA message.
    /// </summary>
    public readonly ref struct NmeaTagBlockParser<TExtraFieldParser>
        where TExtraFieldParser : struct, INmeaTagBlockExtraFieldParser
    {
        /// <summary>
        /// Creates a <see cref="NmeaTagBlockParser"/>.
        /// </summary>
        /// <param name="span">The ASCII-encoded tag block, without the leading and trailing
        /// <c>/</c> delimiters.
        /// </param>
        public NmeaTagBlockParser( ReadOnlySpan<byte> span )
            : this( span, false, TagBlockStandard.Unspecified )
        {
        }

        /// <summary>
        /// Creates a <see cref="NmeaTagBlockParser{TExtraFieldParser}"/>.
        /// </summary>
        /// <param name="span">The ASCII-encoded tag block, without the leading and trailing
        /// <c>/</c> delimiters.
        /// </param>
        /// <param name="throwWhenTagBlockContainsUnknownFields">
        /// Ignore non-standard and unsupported tag block field types. Useful when working with
        /// data sources that add non-standard fields.
        /// </param>
        /// <param name="tagBlockStandard">Defined in whick standard the tag block is.</param>
        public NmeaTagBlockParser( ReadOnlySpan<byte> span, bool throwWhenTagBlockContainsUnknownFields, TagBlockStandard tagBlockStandard )
        {
            OriginalSpan = span;
            SentenceGrouping = default;
            Source = ReadOnlySpan<byte>.Empty;
            UnixTimestamp = default;
            TextString = ReadOnlySpan<byte>.Empty;
            // For the moment, we use the default constructor of the extra field parser. 
            // If we need to use a specific instance of the extra field parser, with a constructor that take parameters,
            // we will need to add a TExtraFieldParser parameter in the constructor of the NmeaTagBlockParser.
            ExtraFieldParser = default;

            if( span[^3] != (byte)'*' )
            {
                throw new ArgumentException( "Tag blocks should end with *XX where XX is a two-digit hexadecimal checksum" );
            }

            span = span.Slice( 0, span.Length - 3 );

            while( span.Length > 0 )
            {
                char fieldType = (char)span[0];

                switch( fieldType )
                {
                    case >= '1' and <= '9':
                        if( tagBlockStandard == TagBlockStandard.Nmea )
                        {
                            throw new ArgumentException( "Tag block sentence grouping should be <int>-<int>-<int>, but first part was not a decimal integer" );
                        }

                        SentenceGrouping = ParseIECSentenceGrouping( ref span );
                        break;

                    case 'g':
                        if( tagBlockStandard == TagBlockStandard.IEC )
                        {
                            throw new ArgumentException( "Tag block sentence grouping should be <int>G<int>:<int>, but first part was not a decimal integer" );
                        }

                        MoveAfterFieldKey( ref span );
                        SentenceGrouping = ParseNmeaSentenceGrouping( ref span );
                        break;

                    case 's':
                        MoveAfterFieldKey( ref span );
                        Source = AdvanceToNextField( ref span );

                        break;

                    case 'c':
                        MoveAfterFieldKey( ref span );
                        if( !ParseDelimitedLong( ref span, out long timestamp ) )
                        {
                            throw new ArgumentException( "Tag block timestamp should be int" );
                        }

                        UnixTimestamp = timestamp;
                        break;

                    case 'i':
                        if( tagBlockStandard == TagBlockStandard.Nmea )
                        {
                            throw new ArgumentException( "Unknown field type in Nmea tag block: i" );
                        }

                        MoveAfterFieldKey( ref span );
                        TextString = AdvanceToNextField( ref span );
                        break;

                    case 't':
                        if( tagBlockStandard == TagBlockStandard.IEC )
                        {
                            throw new ArgumentException( "Unknown field type in IEC tag block: t" );
                        }

                        MoveAfterFieldKey( ref span );
                        TextString = AdvanceToNextField( ref span );
                        break;

                    // Both
                    case 'd':
                    // Nmea
                    case 'n':
                    case 'r':
                    // IEC
                    case 'x':
                        if( throwWhenTagBlockContainsUnknownFields )
                        {
                            if( tagBlockStandard == TagBlockStandard.Nmea && fieldType is 'x' )
                            {
                                throw new ArgumentException( "Unknown field type in Nmea tag block: " + fieldType );
                            }
                            else if( tagBlockStandard == TagBlockStandard.IEC && fieldType is 'n' or 'r' )
                            {
                                throw new ArgumentException( "Unknown field type in IEC tag block: " + fieldType );
                            }

                            throw new NotSupportedException( "Unsupported field type: " + fieldType );
                        }
                        AdvanceToNextField( ref span );
                        break;

                    default:
                        var field = AdvanceToNextField( ref span );
                        var offset = OriginalSpan.Length - (field.Length + span.Length + 3 /* checksum */);
                        if( span.Length > 0 )
                        {
                            // Include the ',' between field and span.
                            offset--;
                        }
                        if( !ExtraFieldParser.TryParseField( OriginalSpan, field, offset )
                            && throwWhenTagBlockContainsUnknownFields )
                        {
                            throw new ArgumentException( "Unknown field type: " + fieldType );
                        }
                        break;
                }

                static void MoveAfterFieldKey( ref ReadOnlySpan<byte> source )
                {
                    if( source.Length < 3 || source[1] != (byte)':' )
                    {
                        throw new ArgumentException( "Tag block entries should start with a type character followed by a colon, and there was no colon" );
                    }

                    source = source.Slice( 2 );
                }

                static ReadOnlySpan<byte> AdvanceToNextField( scoped ref ReadOnlySpan<byte> source )
                {
                    ReadOnlySpan<byte> result;
                    int next = source.IndexOf( (byte)',' );
                    if( next < 0 )
                    {
                        result = source;
                        source = ReadOnlySpan<byte>.Empty;
                    }
                    else
                    {
                        result = source.Slice( 0, next );
                        source = source.Slice( next + 1 );
                    }

                    return result;
                }
            }
        }

        internal static NmeaTagBlockParser<TExtraFieldParser> OverrideGrouping( NmeaTagBlockParser<TExtraFieldParser> parser, NmeaTagBlockSentenceGrouping? grouping )
            => new NmeaTagBlockParser<TExtraFieldParser>( parser, grouping );

        NmeaTagBlockParser( NmeaTagBlockParser<TExtraFieldParser> parser, NmeaTagBlockSentenceGrouping? grouping )
        {
            SentenceGrouping = grouping;
            Source = parser.Source;
            UnixTimestamp = parser.UnixTimestamp;
            TextString = parser.TextString;
        }

        /// <summary>
        /// Gets the original span ending with the *XX (checksum).
        /// </summary>
        public ReadOnlySpan<byte> OriginalSpan { get; }

        /// <summary>
        /// Gets the sentence grouping information for fragmented messages, if present, null otherwise.
        /// </summary>
        public NmeaTagBlockSentenceGrouping? SentenceGrouping { get; }

        /// <summary>
        /// Gets the 's' tag content (can be empty).
        /// </summary>
        public ReadOnlySpan<byte> Source { get; }

        /// <summary>
        /// Gets the unix timestamp, if present, null otherwise.
        /// </summary>
        public long? UnixTimestamp { get; }

        /// <summary>
        /// Gets the text string.
        /// </summary>
        public ReadOnlySpan<byte> TextString { get; }

        /// <summary>
        /// Gets the extra field parser, if a non standard field type is found and can be parsed.
        /// </summary>
        public readonly TExtraFieldParser ExtraFieldParser;

        static bool GetEnd( ref ReadOnlySpan<byte> source, char? delimiter, out int length )
        {
            if( delimiter.HasValue )
            {
                length = source.IndexOf( (byte)delimiter.Value );

                if( length < 0 )
                {
                    return false;
                }

                source = source.Slice( length + 1 );
            }
            else
            {
                length = source.IndexOf( (byte)',' );
                bool isLastField = length < 0;

                if( isLastField )
                {
                    length = source.Length;
                    source = ReadOnlySpan<byte>.Empty;
                }
                else
                {
                    source = source.Slice( length + 1 );
                }
            }

            return true;
        }

        static bool ParseDelimitedInt( ref ReadOnlySpan<byte> source, out int result, char? delimiter = null )
        {
            result = default;

            ReadOnlySpan<byte> original = source;
            if( !GetEnd( ref source, delimiter, out int length ) )
            {
                return false;
            }

            if( !Utf8Parser.TryParse( original, out result, out int consumed )
                || consumed != length )
            {
                return false;
            }

            return true;
        }

        static bool ParseDelimitedLong( ref ReadOnlySpan<byte> source, out long result, char? delimiter = null )
        {
            result = default;

            ReadOnlySpan<byte> original = source;
            if( !GetEnd( ref source, delimiter, out int length ) )
            {
                return false;
            }

            if( !Utf8Parser.TryParse( original, out result, out int consumed )
                || consumed != length )
            {
                return false;
            }

            return true;
        }

        static NmeaTagBlockSentenceGrouping ParseNmeaSentenceGrouping( ref ReadOnlySpan<byte> source )
        {
            if( !ParseDelimitedInt( ref source, out int sentenceNumber, '-' ) )
            {
                throw new ArgumentException( "Tag block sentence grouping should be <int>-<int>-<int>, but first part was not a decimal integer" );
            }

            if( !ParseDelimitedInt( ref source, out int totalSentences, '-' ) )
            {
                throw new ArgumentException( "Tag block sentence grouping should be <int>-<int>-<int>, but second part was not a decimal integer" );
            }

            if( !ParseDelimitedInt( ref source, out int groupId ) )
            {
                throw new ArgumentException( "Tag block sentence grouping should be <int>-<int>-<int>, but third part was not a decimal integer" );
            }

            return new NmeaTagBlockSentenceGrouping( sentenceNumber, totalSentences, groupId );
        }

        static NmeaTagBlockSentenceGrouping ParseIECSentenceGrouping( ref ReadOnlySpan<byte> source )
        {
            if( !ParseDelimitedInt( ref source, out int sentenceNumber, 'G' ) )
            {
                throw new ArgumentException( "Tag block sentence grouping should be <int>G<int>:<int>, but first part was not a decimal integer" );
            }

            if( !ParseDelimitedInt( ref source, out int totalSentences, ':' ) )
            {
                throw new ArgumentException( "Tag block sentence grouping should be <int>G<int>:<int>, but second part was not a decimal integer" );
            }

            if( !ParseDelimitedInt( ref source, out int groupId ) )
            {
                throw new ArgumentException( "Tag block sentence grouping should be <int>G<int>:<int>, but third part was not a decimal integer" );
            }

            return new NmeaTagBlockSentenceGrouping( sentenceNumber, totalSentences, groupId );
        }
    }
}
