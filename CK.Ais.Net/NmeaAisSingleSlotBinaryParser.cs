using System;

namespace Ais.Net
{
    /// <summary>
    /// Enables fields to be extracted from an AIS Single Slot Binary Message.
    /// It parses the content of messages 25.
    /// </summary>
    public readonly ref struct NmeaAisSingleSlotBinaryParser
    {
        readonly NmeaAisBitVectorParser _bits;
        readonly bool _hasDestination;

        /// <summary>
        /// Create an <see cref="NmeaAisSingleSlotBinaryParser"/>.
        /// </summary>
        /// <param name="ascii">The ASCII-encoded message payload.</param>
        /// <param name="padding">The number of bits of padding in this payload.</param>
        public NmeaAisSingleSlotBinaryParser( ReadOnlySpan<byte> ascii, uint padding )
        {
            _bits = new NmeaAisBitVectorParser( ascii, padding );
            _hasDestination = DestinationIndicator == DestinationIndicator.Addressed;

            switch( (_hasDestination, BinaryDataFlag) )
            {
                case (true, true):
                    ApplicationDataPaddingBefore = 4;
                    ApplicationData = ascii.Slice( 14 );
                    break;
                case (true, false):
                    ApplicationDataPaddingBefore = 0;
                    ApplicationData = ascii.Slice( 12 );
                    break;
                case (false, true):
                    ApplicationDataPaddingBefore = 2;
                    ApplicationData = ascii.Slice( 9 );
                    break;
                case (false, false):
                    ApplicationDataPaddingBefore = 4;
                    ApplicationData = ascii.Slice( 6 );
                    break;
            }
            ApplicationDataPaddingAfter = padding;
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
        /// Gets a value indicating whether the <see cref="DestinationMmsi"/> is used.
        /// </summary>
        public DestinationIndicator DestinationIndicator => (DestinationIndicator)_bits.GetUnsignedInteger( 1, 38 );

        /// <summary>
        /// Gets a value indicating whether the application identifier (<see cref="DAC"/> and <see cref="FI"/>) are used.
        /// </summary>
        public bool BinaryDataFlag => _bits.GetBit( 39 );

        /// <summary>
        /// Gets the unique identifier assigned to the transponder who the message is for.
        /// </summary>
        public uint? DestinationMmsi => _hasDestination
            ? _bits.GetUnsignedInteger( 30, 40 )
            : null;

        /// <summary>
        /// Gets the value of the bits in this message for which no standard meaning is currently
        /// defined.
        /// </summary>
        public uint? SpareBits70 => _hasDestination
            ? _bits.GetUnsignedInteger( 2, 70 )
            : null;

        /// <summary>
        /// Gets the Designated area code (DAC).
        /// </summary>
        public uint? DAC => BinaryDataFlag
            ? _bits.GetUnsignedInteger( 10, _hasDestination ? 72u : 40u )
            : null;

        /// <summary>
        /// Gets the Function identifier (FI).
        /// </summary>
        public uint? FI => BinaryDataFlag
            ? _bits.GetUnsignedInteger( 6, _hasDestination ? 82u : 50u )
            : null;

        /// <summary>
        /// Gets the padding before the data in the <see cref="ApplicationData"/>.
        /// </summary>
        public readonly uint ApplicationDataPaddingBefore;

        /// <summary>
        /// Gets the padding after the data in the <see cref="ApplicationData"/>.
        /// </summary>
        public readonly uint ApplicationDataPaddingAfter;

        /// <summary>
        /// Gets the application specific data.
        /// </summary>
        public readonly ReadOnlySpan<byte> ApplicationData;
    }
}
