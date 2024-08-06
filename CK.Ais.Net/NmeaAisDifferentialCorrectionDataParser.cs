using System;

namespace Ais.Net
{
    /// <summary>
    /// Enables fields to be extracted from the Differential Correction Data
    /// into an AIS Global Navigation-Satellite System Broadcast Binary Message (message 17).
    /// </summary>
    public readonly ref struct NmeaAisDifferentialCorrectionDataParser
    {
        readonly NmeaAisBitVectorParser _bits;
        readonly uint _padding;

        /// <summary>
        /// Create an <see cref="NmeaAisDifferentialCorrectionDataParser"/>.
        /// </summary>
        /// <param name="ascii">The ASCII-encoded message payload.</param>
        /// <param name="paddingBegin">The number of bits of padding befort the data content in the <paramref name="ascii"/>.
        /// </param>
        /// <param name="paddingEnd">The number of bits of padding in this payload.</param>
        public NmeaAisDifferentialCorrectionDataParser( ReadOnlySpan<byte> ascii, uint paddingBegin, uint paddingEnd )
        {
            _bits = new NmeaAisBitVectorParser( ascii, paddingEnd );
            _padding = paddingBegin;
        }

        /// <summary>
        /// Gets the message type.
        /// </summary>
        public MessageType MessageType => (MessageType)_bits.GetUnsignedInteger( 6, _padding );

        /// <summary>
        /// Gets the station identifier.
        /// </summary>
        public uint Station => _bits.GetUnsignedInteger( 10, _padding + 6 );

        /// <summary>
        /// Gets the time value in 0.6 s.
        /// </summary>
        public uint ZCount => _bits.GetUnsignedInteger( 13, _padding + 16 );

        /// <summary>
        /// Gets the message sequence number.
        /// </summary>
        public uint SequenceNumber => _bits.GetUnsignedInteger( 3, _padding + 29 );

        /// <summary>
        /// Gets the number of DGNSS data words following the two-word header.
        /// </summary>
        public uint DgnssDataWordCount => _bits.GetUnsignedInteger( 5, _padding + 32 );

        /// <summary>
        /// Gets the reference station health.
        /// </summary>
        public uint Health => _bits.GetUnsignedInteger( 3, _padding + 37 );

        /// <summary>
        /// Writes the Dgnss data world into a buffer.
        /// </summary>
        /// <param name="dataWordCount">The target buffer.</param>
        /// <remarks>
        /// The number of data words is specified in <see cref="DgnssDataWordCount"/>.
        /// </remarks>
        public void WriteDgnssDataWord( in Span<uint> dataWordCount )
        {
            int count = Math.Min( (int)DgnssDataWordCount, dataWordCount.Length );
            uint position = _padding + 40;

            for( int i = 0; i < count; i++ )
            {
                dataWordCount[i] = _bits.GetUnsignedInteger( 24, position );
                position += 24;
            }
        }
    }
}
