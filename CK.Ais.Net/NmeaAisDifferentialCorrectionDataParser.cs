// <copyright file="NmeaAisDifferentialCorrectionDataParser.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Ais.Net
{
    using System;

    /// <summary>
    /// Enables fields to be extracted from the Differential Correction Data
    /// into an AIS Global Navigation-Satellite System Broadcast Binary Message (message 17).
    /// </summary>
    public readonly ref struct NmeaAisDifferentialCorrectionDataParser
    {
        private readonly NmeaAisBitVectorParser bits;
        private readonly uint padding;

        /// <summary>
        /// Create an <see cref="NmeaAisDifferentialCorrectionDataParser"/>.
        /// </summary>
        /// <param name="ascii">The ASCII-encoded message payload.</param>
        /// <param name="paddingBegin">The number of bits of padding befort the data content in the <paramref name="ascii"/>.
        /// </param>
        /// <param name="paddingEnd">The number of bits of padding in this payload.</param>
        public NmeaAisDifferentialCorrectionDataParser(ReadOnlySpan<byte> ascii, uint paddingBegin, uint paddingEnd)
        {
            this.bits = new NmeaAisBitVectorParser(ascii, paddingEnd);
            this.padding = paddingBegin;
        }

        /// <summary>
        /// Gets the message type.
        /// </summary>
        public MessageType MessageType => (MessageType)this.bits.GetUnsignedInteger(6, this.padding);

        /// <summary>
        /// Gets the station identifier.
        /// </summary>
        public uint Station => this.bits.GetUnsignedInteger(10, this.padding + 6);

        /// <summary>
        /// Gets the time value in 0.6 s.
        /// </summary>
        public uint ZCount => this.bits.GetUnsignedInteger(13, this.padding + 16);

        /// <summary>
        /// Gets the message sequence number.
        /// </summary>
        public uint SequenceNumber => this.bits.GetUnsignedInteger(3, this.padding + 29);

        /// <summary>
        /// Gets the number of DGNSS data words following the two-word header.
        /// </summary>
        public uint DgnssDataWordCount => this.bits.GetUnsignedInteger(5, this.padding + 32);

        /// <summary>
        /// Gets the reference station health.
        /// </summary>
        public uint Health => this.bits.GetUnsignedInteger(3, this.padding + 37);

        /// <summary>
        /// Writes the Dgnss data world into a buffer.
        /// </summary>
        /// <param name="dataWordCount">The target buffer.</param>
        /// <remarks>
        /// The number of data words is specified in <see cref="DgnssDataWordCount"/>.
        /// </remarks>
        public void WriteDgnssDataWord(in Span<uint> dataWordCount)
        {
            int count = Math.Min((int)this.DgnssDataWordCount, dataWordCount.Length);
            uint position = this.padding + 40;

            for (int i = 0; i < count; i++)
            {
                dataWordCount[i] = this.bits.GetUnsignedInteger(24, position);
                position += 24;
            }
        }
    }
}
