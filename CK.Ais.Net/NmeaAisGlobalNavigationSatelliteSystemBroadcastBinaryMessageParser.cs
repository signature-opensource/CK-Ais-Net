using System;

namespace Ais.Net
{
    /// <summary>
    /// Enables fields to be extracted from an AIS Global Navigation-Satellite System Broadcast Binary Message.
    /// It parses the content of messages 17.
    /// </summary>
    public readonly ref struct NmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser
    {
        readonly NmeaAisBitVectorParser _bits;

        /// <summary>
        /// Create an <see cref="NmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser"/>.
        /// </summary>
        /// <param name="ascii">The ASCII-encoded message payload.</param>
        /// <param name="padding">The number of bits of padding in this payload.</param>
        public NmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser( ReadOnlySpan<byte> ascii, uint padding )
        {
            _bits = new NmeaAisBitVectorParser( ascii, padding );
            DifferentialCorrectionData = _bits.BitCount > 80
                ? ascii.Slice( 13 )
                : ReadOnlySpan<byte>.Empty;
            DifferentialCorrectionDataPaddingAfter = padding;
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
        /// Gets the reported longitude, in units of 1/10 arc minutes.
        /// </summary>
        public int Longitude10thMins => _bits.GetSignedInteger( 18, 40 );

        /// <summary>
        /// Gets the reported latitude, in units of 1/10 arc minutes.
        /// </summary>
        public int Latitude10thMins => _bits.GetSignedInteger( 17, 58 );

        /// <summary>
        /// Gets the value of the bits in this message for which no standard meaning is currently
        /// defined.
        /// </summary>
        public uint SpareBits75 => _bits.GetUnsignedInteger( 5, 75 );

        /// <summary>
        /// Gets the padding before the data in the <see cref="DifferentialCorrectionData"/>.
        /// </summary>
#pragma warning disable CA1822 // Mark members as static
        public uint DifferentialCorrectionDataPaddingBefore => 2;
#pragma warning restore CA1822 // Mark members as static

        public readonly uint DifferentialCorrectionDataPaddingAfter;

        /// <summary>
        /// Gets the differential correlation data. It should be parsed with the <see cref="NmeaAisDifferentialCorrectionDataParser"/>.
        /// </summary>
        public readonly ReadOnlySpan<byte> DifferentialCorrectionData;
    }
}
