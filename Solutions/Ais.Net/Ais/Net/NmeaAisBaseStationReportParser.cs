// <copyright file="NmeaAisBaseStationReportParser.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Ais.Net
{
    using System;

    /// <summary>
    /// Enables fields to be extracted from an AIS Base Station Report.
    /// </summary>
    public readonly ref struct NmeaAisBaseStationReportParser
    {
        private readonly NmeaAisBitVectorParser bits;

        /// <summary>
        /// Create an <see cref="NmeaAisBaseStationReportParser"/>.
        /// </summary>
        /// <param name="ascii">The ASCII-encoded message payload.</param>
        /// <param name="padding">The number of bits of padding in this payload.</param>
        public NmeaAisBaseStationReportParser(ReadOnlySpan<byte> ascii, uint padding)
        {
            this.bits = new NmeaAisBitVectorParser(ascii, padding);
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

#pragma warning disable CS1591, SA1600 // XML comments. The names are from the spec, and they're all the information we have
        public uint UtcYear => this.bits.GetUnsignedInteger(14, 38);

        public uint UtcMonth => this.bits.GetUnsignedInteger(4, 52);

        public uint UtcDay => this.bits.GetUnsignedInteger(5, 56);

        public uint UtcHour => this.bits.GetUnsignedInteger(5, 61);

        public uint UtcMinute => this.bits.GetUnsignedInteger(6, 66);

        public uint UtcSecond => this.bits.GetUnsignedInteger(6, 72);
#pragma warning restore CS1591, SA1600 // XML comments. The names are from the spec, and they're all the information we have

        /// <summary>
        /// Gets a value indicating whether the position information is of DGPS quality.
        /// </summary>
        /// <remarks>
        /// If <c>true</c>, location information is DGPS-quality (less than 10m). If false, it is
        /// of unaugmented GNSS accuracy.
        /// </remarks>
        public bool PositionAccuracy => this.bits.GetBit(78);

        /// <summary>
        /// Gets the reported longitude, in units of 1/10000 arc minutes.
        /// </summary>
        public int Longitude10000thMins => this.bits.GetSignedInteger(28, 79);

        /// <summary>
        /// Gets the reported latitude, in units of 1/10000 arc minutes.
        /// </summary>
        public int Latitude10000thMins => this.bits.GetSignedInteger(27, 107);

        /// <summary>
        /// Gets the position fix type.
        /// </summary>
        public EpfdFixType PositionFixType => (EpfdFixType)this.bits.GetUnsignedInteger(4, 134);

        /// <summary>
        /// Gets a value indicating whether transmission control for long-range broadcast message.
        /// </summary>
        public bool TransmissionControlForLongRangeBroadcastMessage => this.bits.GetBit(138);

        /// <summary>
        /// Gets the value of the bits in this message for which no standard meaning is currently
        /// defined.
        /// </summary>
        public uint SpareBits139 => this.bits.GetUnsignedInteger(9, 139);

        /// <summary>
        /// Gets a value indicating whether Receiver Autonomous Integrity Monitoring is in use.
        /// </summary>
        public bool RaimFlag => this.bits.GetBit(148);

        /// <summary>
        /// Gets the SOTDMA communication state.
        /// </summary>
        public uint CommunicationState => this.bits.GetUnsignedInteger(19, 149);
    }
}
