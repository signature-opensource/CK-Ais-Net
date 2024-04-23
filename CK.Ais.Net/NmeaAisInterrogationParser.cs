namespace Ais.Net
{
    using System;

    /// <summary>
    /// Enables fields to be extracted from an Interrogation.
    /// It parses the content of messages 15.
    /// </summary>
    public readonly ref struct NmeaAisInterrogationParser
    {
        readonly NmeaAisBitVectorParser _bits;

        /// <summary>
        /// Create an <see cref="NmeaAisInterrogationParser"/>.
        /// </summary>
        /// <param name="ascii">The ASCII-encoded message payload.</param>
        /// <param name="padding">The number of bits of padding in this payload.</param>
        public NmeaAisInterrogationParser( ReadOnlySpan<byte> ascii, uint padding )
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
        /// Gets the value of the bits in this message for which no standard meaning is currently
        /// defined.
        /// </summary>
        public uint SpareBits38 => _bits.GetUnsignedInteger( 2, 38 );

        /// <summary>
        /// Gets the MMSI number of first interrogated station.
        /// </summary>
        public uint DestinationMmsi1 => _bits.GetUnsignedInteger( 30, 40 );

        /// <summary>
        /// Gets the first requested message type from first interrogated station.
        /// </summary>
        public MessageType MessageType11 => (MessageType)_bits.GetUnsignedInteger( 6, 70 );

        /// <summary>
        /// Gets the response slot offset for first requested message from first interrogated
        /// station.
        /// </summary>
        public uint SlotOffset11 => _bits.GetUnsignedInteger( 12, 76 );

        /// <summary>
        /// Gets the value of the bits in this message for which no standard meaning is currently
        /// defined.
        /// </summary>
        public uint? SpareBits88 => _bits.BitCount < 90
            ? null
            : _bits.GetUnsignedInteger( 2, 88 );

        /// <summary>
        /// Gets the second requested message type from first interrogated station.
        /// </summary>
        public MessageType? MessageType12 => _bits.BitCount < 96
            ? null :
            (MessageType)_bits.GetUnsignedInteger( 6, 90 );

        /// <summary>
        /// Gets the response slot offset for second requested message from first interrogated
        /// station.
        /// </summary>
        public uint? SlotOffset12 => _bits.BitCount < 108
            ? null :
            _bits.GetUnsignedInteger( 12, 96 );

        /// <summary>
        /// Gets the value of the bits in this message for which no standard meaning is currently
        /// defined.
        /// </summary>
        public uint? SpareBits108 => _bits.BitCount < 110
            ? null
            : _bits.GetUnsignedInteger( 2, 108 );

        /// <summary>
        /// Gets the MMSI number of second interrogated station.
        /// </summary>
        public uint? DestinationMmsi2 => _bits.BitCount < 140
            ? null
            : _bits.GetUnsignedInteger( 30, 110 );

        /// <summary>
        /// Gets the requested message type from second interrogated station.
        /// </summary>
        public MessageType? MessageType21 => _bits.BitCount < 146
            ? null :
            (MessageType)_bits.GetUnsignedInteger( 6, 140 );

        /// <summary>
        /// Gets the response slot offset for requested message from second interrogated station.
        /// </summary>
        public uint? SlotOffset21 => _bits.BitCount < 158
            ? null
            : _bits.GetUnsignedInteger( 12, 146 );

        /// <summary>
        /// Gets the value of the bits in this message for which no standard meaning is currently
        /// defined.
        /// </summary>
        public uint? SpareBits158 => _bits.BitCount < 160
            ? null
            : _bits.GetUnsignedInteger( 2, 158 );
    }
}
