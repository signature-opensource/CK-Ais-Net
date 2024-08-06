using System;

namespace Ais.Net
{
    /// <summary>
    /// Enables fields to be extracted from an AIS Addressed Safety Related Message.
    /// It parses the content of messages 12.
    /// </summary>
    public readonly ref struct NmeaAisAddressedSafetyRelatedMessageParser
    {
        readonly NmeaAisBitVectorParser _bits;

        /// <summary>
        /// Create an <see cref="NmeaAisAddressedSafetyRelatedMessageParser"/>.
        /// </summary>
        /// <param name="ascii">The ASCII-encoded message payload.</param>
        /// <param name="padding">The number of bits of padding in this payload.</param>
        public NmeaAisAddressedSafetyRelatedMessageParser( ReadOnlySpan<byte> ascii, uint padding )
        {
            _bits = new NmeaAisBitVectorParser( ascii, padding );
        }

        /// <summary>
        /// Gets the message type.
        /// </summary>
        public MessageType MessageType => (MessageType)_bits.GetUnsignedInteger( 6, 0 );

        /// <summary>
        /// Gets the number of times this message had been repeated on this broadcast.
        /// </summary>
        /// <remarks>
        /// When stations retransmit messages with a view to enabling them to get around hills and
        /// other obstacles, this should be incremented. When it reaches 3, no more attempts should
        /// be made to retransmit it.
        /// </remarks>
        public uint RepeatIndicator => _bits.GetUnsignedInteger( 2, 6 );

        /// <summary>
        /// Gets the unique identifier assigned to the transponder that sent this message.
        /// </summary>
        public uint Mmsi => _bits.GetUnsignedInteger( 30, 8 );

        /// <summary>
        /// Gets the sequence number.
        /// </summary>
        public uint SequenceNumber => _bits.GetUnsignedInteger( 2, 38 );

        /// <summary>
        /// Gets the unique identifier assigned to the transponder who the message is for.
        /// </summary>
        public uint DestinationMmsi => _bits.GetUnsignedInteger( 30, 40 );

        /// <summary>
        /// Gets a value indicating whether the message is retransmited.
        /// </summary>
        public bool Retransmit => _bits.GetBit( 70 );

        /// <summary>
        /// Gets a value indicating whether the spare bit at offset 71 is set.
        /// </summary>
        public bool SpareBit71 => _bits.GetBit( 71 );

        /// <summary>
        /// Gets the safety related text.
        /// </summary>
        public NmeaAisTextFieldParser SafetyRelatedText => new NmeaAisTextFieldParser( _bits, _bits.BitCount - 72, 72 );
    }
}
