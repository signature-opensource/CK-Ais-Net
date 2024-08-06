using System;

namespace Ais.Net
{
    /// <summary>
    /// Enables fields to be extracted from an AIS Multiple Slot Binary Message With Communications State Message.
    /// It parses the content of messages 26.
    /// </summary>
    public readonly ref struct NmeaAisMultipleSlotBinaryMessageWithCommunicationsStateParser
    {
        readonly NmeaAisBitVectorParser _bits;
        readonly bool _hasDestination;

        /// <summary>
        /// Create an <see cref="NmeaAisMultipleSlotBinaryMessageWithCommunicationsStateParser"/>.
        /// </summary>
        /// <param name="ascii">The ASCII-encoded message payload.</param>
        /// <param name="padding">The number of bits of padding in this payload.</param>
        /// <param name="slotsCount">The number of slots used to contain the message.</param>
        public NmeaAisMultipleSlotBinaryMessageWithCommunicationsStateParser( ReadOnlySpan<byte> ascii, uint padding, int slotsCount )
        {
            _bits = new NmeaAisBitVectorParser( ascii, padding );
            _hasDestination = DestinationIndicator == DestinationIndicator.Addressed;

            // TODO: gets the first slot length and the number of slots.
            int firstSlotLength = 0;

            switch( (_hasDestination, BinaryDataFlag) )
            {
                case (true, true ):
                    ApplicationDataPadding = 4;
                    ApplicationData = ascii.Slice( 14 - firstSlotLength );
                    break;
                case (true, false ):
                    ApplicationDataPadding = 0;
                    ApplicationData = ascii.Slice( 12 - firstSlotLength );
                    break;
                case (false, true ):
                    ApplicationDataPadding = 2;
                    ApplicationData = ascii.Slice( 9 - firstSlotLength );
                    break;
                case (false, false ):
                    ApplicationDataPadding = 4;
                    ApplicationData = ascii.Slice( 6 - firstSlotLength );
                    break;
            }
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
        public uint ApplicationDataPadding { get; }

        /// <summary>
        /// Gets the application specific data.
        /// </summary>
        public ReadOnlySpan<byte> ApplicationData { get; }

        /// <summary>
        /// Gets the binary data from the second slot.
        /// </summary>
        public ReadOnlySpan<byte> BinaryDataSlot2 { get; } = ReadOnlySpan<byte>.Empty;

        /// <summary>
        /// Gets the binary data from the third slot.
        /// </summary>
        public ReadOnlySpan<byte> BinaryDataSlot3 { get; } = ReadOnlySpan<byte>.Empty;

        /// <summary>
        /// Gets the binary data from the fourth slot.
        /// </summary>
        public ReadOnlySpan<byte> BinaryDataSlot4 { get; } = ReadOnlySpan<byte>.Empty;

        /// <summary>
        /// Gets the binary data from the fifth slot.
        /// </summary>
        public ReadOnlySpan<byte> BinaryDataSlot5 { get; } = ReadOnlySpan<byte>.Empty;
        /*
        /// <summary>
        /// Gets the needed spare bits for alignment.
        /// </summary>
        public uint SpareBitsXXX => _bits.GetUnsignedInteger(4, );

        /// <summary>
        /// Gets the communication state follows that is used.
        /// </summary>
        public CommunicationStateSelector CommunicationStateSelector => (CommunicationStateSelector)_bits.GetUnsignedInteger(1, );

        /// <summary>
        /// Gets the communication state, based on <see cref="CommunicationStateSelector"/>.
        /// </summary>
        public uint CommunicationState => _bits.GetUnsignedInteger(19, );
        */
    }
}
