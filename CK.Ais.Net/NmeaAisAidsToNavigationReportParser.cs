// <copyright file="NmeaAisAidsToNavigationReportParser.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Ais.Net
{
    using System;

    /// <summary>
    /// Enables fields to be extracted from an AIS Aids to Navigation Report.
    /// It parses the content of messages 21.
    /// </summary>
    public readonly ref struct NmeaAisAidsToNavigationReportParser
    {
        private readonly NmeaAisBitVectorParser bits;

        /// <summary>
        /// Create an <see cref="NmeaAisAidsToNavigationReportParser"/>.
        /// </summary>
        /// <param name="ascii">The ASCII-encoded message payload.</param>
        /// <param name="padding">The number of bits of padding in this payload.</param>
        public NmeaAisAidsToNavigationReportParser(ReadOnlySpan<byte> ascii, uint padding)
        {
            this.bits = new NmeaAisBitVectorParser(ascii, padding);
        }

        /// <summary>
        /// Gets the message type.
        /// </summary>
        public MessageType MessageType => (MessageType)this.bits.GetUnsignedInteger(6, 0);

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
        /// Gets the type of aids to navigation that is used.
        /// </summary>
        public AidsToNavigationType AidsToNavigationType => (AidsToNavigationType)this.bits.GetUnsignedInteger(5, 38);

        /// <summary>
        /// Gets the name of Aids-to-Navigation.
        /// </summary>
        public NmeaAisTextFieldParser NameOfAidsToNavigation => new NmeaAisTextFieldParser(this.bits, 120, 43);

        /// <summary>
        /// Gets a value indicating whether the position information is of DGPS quality.
        /// </summary>
        /// <remarks>
        /// If <c>true</c>, location information is DGPS-quality (less than 10m). If false, it is
        /// of unaugmented GNSS accuracy.
        /// </remarks>
        public bool PositionAccuracy => this.bits.GetBit(163);

        /// <summary>
        /// Gets the reported longitude, in units of 1/10000 arc minutes.
        /// </summary>
        public int Longitude10000thMins => this.bits.GetSignedInteger(28, 164);

        /// <summary>
        /// Gets the reported latitude, in units of 1/10000 arc minutes.
        /// </summary>
        public int Latitude10000thMins => this.bits.GetSignedInteger(27, 192);

        /// <summary>
        /// Gets the reference point for reported position.
        /// </summary>
        public uint ReferenceForPositionA => this.bits.GetUnsignedInteger(9, 219);

        /// <summary>
        /// Gets the reference point for reported position.
        /// </summary>
        public uint ReferenceForPositionB => this.bits.GetUnsignedInteger(9, 228);

        /// <summary>
        /// Gets the reference point for reported position.
        /// </summary>
        public uint ReferenceForPositionC => this.bits.GetUnsignedInteger(6, 237);

        /// <summary>
        /// Gets the reference point for reported position.
        /// </summary>
        public uint ReferenceForPositionD => this.bits.GetUnsignedInteger(6, 243);

        /// <summary>
        /// Gets the position fix type.
        /// </summary>
        public EpfdFixType EpfdFixType => (EpfdFixType)this.bits.GetUnsignedInteger(4, 249);

        /// <summary>
        /// Gets the seconds part of the (UTC) time at which the location was recorded.
        /// </summary>
        public uint TimeStampSecond => this.bits.GetUnsignedInteger(6, 253);

        /// <summary>
        /// Gets a value indicating whether the AtoN is off position.
        /// </summary>
        public bool OffPositionIndicator => this.bits.GetBit(259);

        /// <summary>
        /// Gets the indication of the AtoN status.
        /// </summary>
        public uint AtoNStatus => this.bits.GetUnsignedInteger(8, 260);

        /// <summary>
        /// Gets a value indicating whether Receiver Autonomous Integrity Monitoring is in use.
        /// </summary>
        public bool RaimFlag => this.bits.GetBit(268);

        /// <summary>
        /// Gets a value indicating whether the AtoN is virtual.
        /// </summary>
        public bool VirtualAtoN => this.bits.GetBit(269);

        /// <summary>
        /// Gets a value indicating whether the station operate in assigned mode.
        /// </summary>
        public bool AssignedMode => this.bits.GetBit(270);

        /// <summary>
        /// Gets a value indicating whether the spare bit at offset 241 is set.
        /// </summary>
        public bool SpareBit241 => this.bits.GetBit(271);

        /// <summary>
        /// Gets the extension of the <see cref="NameOfAidsToNavigation"/>.
        /// </summary>
        public NmeaAisTextFieldParser NameOfAidToNavigationExtension => new NmeaAisTextFieldParser(this.bits, (this.bits.BitCount - 272) / 6 * 6, 272);

        /// <summary>
        /// Gets the value of the bits in this message for which no standard meaning is currently
        /// defined.
        /// </summary>
        public uint SpareBitsAtEnd => this.bits.GetUnsignedInteger((this.bits.BitCount - 272) % 6, 272 + ((this.bits.BitCount - 272) / 6 * 6));
    }
}
