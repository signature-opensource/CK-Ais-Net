// <copyright file="NmeaLineParser.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System;
using System.Text;

namespace Ais.Net
{
    /// <summary>
    /// Parses a line of ASCII-encoded text containing an NMEA message.
    /// </summary>
    public readonly ref struct NmeaLineParser<TExtraFieldParser>
        where TExtraFieldParser : struct, INmeaTagBlockExtraFieldParser
    {
        const byte _exclamationMark = (byte)'!';
        const byte _tagBlockMarker = (byte)'\\';
        static readonly byte[] _vdmAscii = Encoding.ASCII.GetBytes( "VDM" );
        static readonly byte[] _vdoAscii = Encoding.ASCII.GetBytes( "VDO" );

        /// <summary>
        /// Creates a <see cref="NmeaLineParser"/>.
        /// </summary>
        /// <param name="line">The ASCII-encoded text containing the NMEA message.</param>
        public NmeaLineParser( ReadOnlySpan<byte> line )
            : this( line, false, TagBlockStandard.Unspecified, EmptyGroupTolerance.None )
        {
        }

        /// <summary>
        /// Creates a <see cref="NmeaLineParser"/>.
        /// </summary>
        /// <param name="line">The ASCII-encoded text containing the NMEA message.</param>
        /// <param name="throwWhenTagBlockContainsUnknownFields">
        /// Ignore non-standard and unsupported tag block field types. Useful when working with
        /// data sources that add non-standard fields.
        /// </param>
        /// <param name="tagBlockStandard">Defined in whick standard the tag block is.</param>
        /// <param name="emptyGroupTolerance">The empty group tolerance.</param>
        public NmeaLineParser( ReadOnlySpan<byte> line, bool throwWhenTagBlockContainsUnknownFields, TagBlockStandard tagBlockStandard, EmptyGroupTolerance emptyGroupTolerance )
        {
            Line = line;

            int sentenceStartIndex = 0;

            if( line[0] == _tagBlockMarker )
            {
                int tagBlockEndIndex = line.Slice( 1 ).IndexOf( _tagBlockMarker );

                if( tagBlockEndIndex < 0 )
                {
                    throw new ArgumentException( "Invalid data. Unclosed tag block" );
                }

                TagBlockAsciiWithoutDelimiters = line.Slice( 1, tagBlockEndIndex );
                TagBlock = new NmeaTagBlockParser<TExtraFieldParser>( TagBlockAsciiWithoutDelimiters, throwWhenTagBlockContainsUnknownFields, tagBlockStandard );

                sentenceStartIndex = tagBlockEndIndex + 2;
            }
            else
            {
                if( line.IndexOf( _tagBlockMarker ) > 0 )
                {
                    throw new NotSupportedException( "Can't handle tag block unless at start" );
                }

                TagBlockAsciiWithoutDelimiters = ReadOnlySpan<byte>.Empty;
            }

            Sentence = line.Slice( sentenceStartIndex );

            if( !TagBlockAsciiWithoutDelimiters.IsEmpty &&
                TagBlock.SentenceGrouping.HasValue &&
                !HasSentence( Sentence ) &&
                emptyGroupTolerance >= EmptyGroupTolerance.Allow )
            {
                Sentence = ReadOnlySpan<byte>.Empty;
                Payload = ReadOnlySpan<byte>.Empty;
                return;
            }

            // Need at least the exclamation mark, talker, and origin (e.g. !AIVDM), then
            // the two fragment fields are non-optional. The multi-sequence message ID is
            // optional for non-fragmented messages, and apparently so is the channel.
            // so prior to the payload, we've got at least this much:
            // !AIVDM,1,1,,,
            // Then there will need to be the final comma, the padding, the * and the checksum, e.g.
            // ,0*3C
            // So that's 18 characters before we get to any payload.
            if( Sentence.Length < 18 )
            {
                throw new ArgumentException( "Invalid data. The message appears to be missing some characters - it may have been corrupted or truncated." );
            }

            if( Sentence[0] != _exclamationMark )
            {
                throw new ArgumentException( "Invalid data. Expected '!' at sentence start" );
            }

            byte talkerFirstChar = Sentence[1];
            byte talkerSecondChar = Sentence[2];

            AisTalker = talkerFirstChar switch
            {
                (byte)'A' => talkerSecondChar switch
                {
                    (byte)'I' => TalkerId.MobileStation,
                    (byte)'B' => TalkerId.BaseStation,
                    (byte)'D' => TalkerId.DependentBaseStation,
                    (byte)'N' => TalkerId.AidToNavigationStation,
                    (byte)'R' => TalkerId.ReceivingStation,
                    (byte)'S' => TalkerId.LimitedBaseStation,
                    (byte)'T' => TalkerId.TransmittingStation,
                    (byte)'X' => TalkerId.RepeaterStation,
                    _ => throw new ArgumentException( "Invalid data. Unrecognized talker id - cannot start with " + talkerFirstChar ),
                },

                (byte)'B' => talkerSecondChar switch
                {
                    (byte)'S' => TalkerId.DeprecatedBaseStation,
                    _ => throw new ArgumentException( "Invalid data. Unrecognized talker id - cannot end with " + talkerSecondChar ),
                },

                (byte)'S' => talkerSecondChar switch
                {
                    (byte)'A' => TalkerId.PhysicalShoreStation,
                    _ => throw new ArgumentException( "Invalid data. Unrecognized talker id - cannot end with " + talkerSecondChar ),
                },

                _ => throw new ArgumentException( "Invalid data. Unrecognized talker id" ),
            };
            if( Sentence.Slice( 3, 3 ).SequenceEqual( _vdmAscii ) )
            {
                DataOrigin = VesselDataOrigin.Vdm;
            }
            else if( Sentence.Slice( 3, 3 ).SequenceEqual( _vdoAscii ) )
            {
                DataOrigin = VesselDataOrigin.Vdo;
            }
            else
            {
                throw new ArgumentException( "Invalid data. Unrecognized origin in AIS talker ID - must be VDM or VDO" );
            }

            if( Sentence[6] != (byte)',' )
            {
                throw new ArgumentException( "Invalid data. Talker ID must be followed by ','" );
            }

            ReadOnlySpan<byte> remainingFields = Sentence.Slice( 7 );

            TotalFragmentCount = GetSingleDigitField( ref remainingFields, true );
            FragmentNumberOneBased = GetSingleDigitField( ref remainingFields, true );

            int nextComma = remainingFields.IndexOf( (byte)',' );

            MultiSequenceMessageId = remainingFields.Slice( 0, nextComma );

            remainingFields = remainingFields.Slice( nextComma + 1 );
            nextComma = remainingFields.IndexOf( (byte)',' );

            if( nextComma > 1 )
            {
                throw new ArgumentException( "Invalid data. Channel code must be only one character" );
            }

            ChannelCode = nextComma == 0 ? default : (char)remainingFields[0];

            remainingFields = remainingFields.Slice( nextComma + 1 );
            nextComma = remainingFields.IndexOf( (byte)',' );

            if( nextComma < 0 || remainingFields.Length <= (nextComma + 1) || !char.IsDigit( (char)remainingFields[nextComma + 1] ) )
            {
                throw new ArgumentException( "Invalid data. Payload padding field not present - the message may have been corrupted or truncated" );
            }

            Payload = remainingFields.Slice( 0, nextComma );

            remainingFields = remainingFields.Slice( nextComma + 1 );

            if( remainingFields.Length < 4 )
            {
                throw new ArgumentException( "Invalid data. Payload checksum not present - the message may have been corrupted or truncated" );
            }

            Padding = (uint)GetSingleDigitField( ref remainingFields, true );
        }

        /// <summary>
        /// Gets the talker ID that produced the message.
        /// </summary>
        public TalkerId AisTalker { get; }

        /// <summary>
        /// Gets the radio channel code, if present.
        /// </summary>
        public char? ChannelCode { get; }

        /// <summary>
        /// Gets the origin of the data (VDM or VDO).
        /// </summary>
        public VesselDataOrigin DataOrigin { get; }

        /// <summary>
        /// Gets the fragment number of this message, with 1 being the first fragment.
        /// </summary>
        public int FragmentNumberOneBased { get; }

        /// <summary>
        /// Gets the underlying data that was passed in during construction.
        /// </summary>
        public ReadOnlySpan<byte> Line { get; }

        /// <summary>
        /// Gets the multisequence message id if present (and an empty range if not).
        /// </summary>
        public ReadOnlySpan<byte> MultiSequenceMessageId { get; }

        /// <summary>
        /// Gets the number of bits of padding present in the payload.
        /// </summary>
        /// <remarks>
        /// The 6-bit ASCII encoding NMEA uses for the AIS payload can only encode data in
        /// multiples of 6 bits. If the underlying message is not a multiple of 6 bits long,
        /// the encoding will result in the payload having some extra bits on the end. This
        /// property reports how many of those bits there are.
        /// </remarks>
        public uint Padding { get; }

        /// <summary>
        /// Gets the 6-bit-ASCII-encoded payload. (This is the underlying AIS message.)
        /// </summary>
        public ReadOnlySpan<byte> Payload { get; }

        /// <summary>
        /// Gets the AIVDM/AIVDO sentence part of the underlying data.
        /// </summary>
        /// <remarks>
        /// In cases where no tag block is present, this will be the same as <see cref="Line"/>.
        /// But if a tag block was present, this provides just the 'sentence' part of the line.
        /// </remarks>
        public ReadOnlySpan<byte> Sentence { get; }

        /// <summary>
        /// Gets the details from the tag block.
        /// </summary>
        public NmeaTagBlockParser<TExtraFieldParser> TagBlock { get; }

        /// <summary>
        /// Gets the tag block part of the underlying data (excluding the delimiting '/'
        /// characters), or an empty span if no tag block was present.
        /// </summary>
        public ReadOnlySpan<byte> TagBlockAsciiWithoutDelimiters { get; }

        /// <summary>
        /// Gets the total number of message fragments in the set of messages of which this is a
        /// part.
        /// </summary>
        public int TotalFragmentCount { get; }

        /// <summary>
        /// Indicate if the message is a fragment from a group with fragments with empty sentence.
        /// </summary>
        public bool IsFixedMessage { get; }

        internal static NmeaLineParser<TExtraFieldParser> OverrideGrouping( NmeaLineParser<TExtraFieldParser> lineParser, NmeaTagBlockSentenceGrouping? grouping )
            => new NmeaLineParser<TExtraFieldParser>( lineParser, grouping );

        private NmeaLineParser( NmeaLineParser<TExtraFieldParser> parser, NmeaTagBlockSentenceGrouping? grouping )
        {
            AisTalker = parser.AisTalker;
            ChannelCode = parser.ChannelCode;
            DataOrigin = parser.DataOrigin;
            FragmentNumberOneBased = grouping.HasValue ? grouping.Value.SentenceNumber : 1;
            Line = parser.Line;
            MultiSequenceMessageId = parser.MultiSequenceMessageId;
            Padding = parser.Padding;
            Payload = parser.Payload;
            Sentence = parser.Sentence;
            TagBlock = NmeaTagBlockParser<TExtraFieldParser>.OverrideGrouping( parser.TagBlock, grouping );
            TagBlockAsciiWithoutDelimiters = parser.TagBlockAsciiWithoutDelimiters;
            TotalFragmentCount = grouping.HasValue ? grouping.Value.SentencesInGroup : 1;
            IsFixedMessage = true;
        }

        static int GetSingleDigitField( ref ReadOnlySpan<byte> fields, bool required )
        {
            if( fields[0] == ',' )
            {
                if( required )
                {
                    throw new ArgumentException( "Field must not be empty" );
                }

                fields = fields.Slice( 1 );

                return 0;
            }

            int result = fields[0] - '0';

            if( result is < 0 or > 9 )
            {
                throw new NotSupportedException( "Cannot handle multi-digit field" );
            }

            fields = fields.Slice( 2 );

            return result;
        }

        /// <summary>
        /// This method checks that the sentence (after the tag block) is not empty.
        /// When we parse the data from the source stream, the <paramref name="sentence"/> will go to the end of the NMEA sentence.
        /// When the frame is in a group, it well be in a `byte` provided by the `ArrayPool`, but the `byte` array may exceed the originale message size.
        /// Here a sentence is considered to exist when the <paramref name="sentence"/> is not empty and all its bytes are not at theur default value.
        /// <param name="sentence">The part following the tag block.</param>
        static bool HasSentence( ReadOnlySpan<byte> sentence )
        {
            for( int i = 0; i < sentence.Length; i++ )
            {
                if( sentence[i] is not default( byte ) ) return true;
            }
            return false;
        }
    }
}
