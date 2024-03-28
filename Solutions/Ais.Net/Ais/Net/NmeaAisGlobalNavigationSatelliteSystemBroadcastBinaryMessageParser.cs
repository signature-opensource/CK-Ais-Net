// <copyright file="NmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Ais.Net
{
    using System;

    /// <summary>
    /// Enables fields to be extracted from an AIS Global Navigation-Satellite System Broadcast Binary Message.
    /// </summary>
    public readonly ref struct NmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser
    {
        private readonly NmeaAisBitVectorParser bits;

        /// <summary>
        /// Create an <see cref="NmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser"/>.
        /// </summary>
        /// <param name="ascii">The ASCII-encoded message payload.</param>
        /// <param name="padding">The number of bits of padding in this payload.</param>
        public NmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser(ReadOnlySpan<byte> ascii, uint padding)
        {
            this.bits = new NmeaAisBitVectorParser(ascii, padding);
            this.DifferentialCorrectionData = this.bits.BitCount > 80
                ? ascii.Slice(13)
                : ReadOnlySpan<byte>.Empty;
        }

        /// <summary>
        /// Gets the message type.
        /// </summary>
        public uint MessageType => this.bits.GetUnsignedInteger(6, 0);

        /// <summary>
        /// Gets the number of times this message had been repeated on this broadcast.
        /// </summary>
        /// <remarks>
        /// When stations retransmit messages with a view to enabling them to get around hills and
        /// other obstacles, this should be incremented. When it reaches 3, no more attempts should
        /// be made to retransmit it.
        /// </remarks>
        public uint RepeatIndicator => this.bits.GetUnsignedInteger(2, 6);

        /// <summary>
        /// Gets the unique identifier assigned to the transponder that sent this message.
        /// </summary>
        public uint Mmsi => this.bits.GetUnsignedInteger(30, 8);

        /// <summary>
        /// Gets the value of the bits in this message for which no standard meaning is currently
        /// defined.
        /// </summary>
        public uint SpareBits38 => this.bits.GetUnsignedInteger(2, 38);

        /// <summary>
        /// Gets the reported longitude, in units of 1/10 arc minutes.
        /// </summary>
        public int Longitude10thMins => this.bits.GetSignedInteger(18, 40);

        /// <summary>
        /// Gets the reported latitude, in units of 1/10 arc minutes.
        /// </summary>
        public int Latitude10thMins => this.bits.GetSignedInteger(17, 58);

        /// <summary>
        /// Gets the value of the bits in this message for which no standard meaning is currently
        /// defined.
        /// </summary>
        public uint SpareBits75 => this.bits.GetUnsignedInteger(5, 75);

        /// <summary>
        /// Gets the padding before the data in the <see cref="DifferentialCorrectionData"/>.
        /// </summary>
        public uint DifferentialCorrectionDataPadding => 2;

        /// <summary>
        /// Gets the differential correlation data. It should be parsed with the <see cref="NmeaAisDifferentialCorrectionDataParser"/>.
        /// </summary>
        public ReadOnlySpan<byte> DifferentialCorrectionData { get; }
    }
}
