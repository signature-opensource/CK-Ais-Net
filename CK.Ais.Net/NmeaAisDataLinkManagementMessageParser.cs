// <copyright file="NmeaAisDataLinkManagementMessageParser.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Ais.Net
{
    using System;

    /// <summary>
    /// Enables fields to be extracted from a Data link management message.
    /// It parses the content of messages 20.
    /// </summary>
    public readonly ref struct NmeaAisDataLinkManagementMessageParser
    {
        private readonly NmeaAisBitVectorParser bits;

        /// <summary>
        /// Create an <see cref="NmeaAisDataLinkManagementMessageParser"/>.
        /// </summary>
        /// <param name="ascii">The ASCII-encoded message payload.</param>
        /// <param name="padding">The number of bits of padding in this payload.</param>
        public NmeaAisDataLinkManagementMessageParser(ReadOnlySpan<byte> ascii, uint padding)
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
        /// Gets the value of the bits in this message for which no standard meaning is currently
        /// defined.
        /// </summary>
        public uint SpareBits38 => this.bits.GetUnsignedInteger(2, 38);

        /// <summary>
        /// Gets the reserved offset number.
        /// </summary>
        public uint Offset1 => this.bits.GetUnsignedInteger(12, 40);

        /// <summary>
        /// Gets the number of reserved consecutive slots.
        /// </summary>
        public uint SlotNumber1 => this.bits.GetUnsignedInteger(4, 52);

        /// <summary>
        /// Gets the time-out value in minutes.
        /// </summary>
        public uint Timeout1 => this.bits.GetUnsignedInteger(3, 56);

        /// <summary>
        /// Gets the increment to repeat reservation block 1.
        /// </summary>
        public uint Increment1 => this.bits.GetUnsignedInteger(11, 59);

        /// <summary>
        /// Gets the reserved offset number.
        /// </summary>
        public uint? Offset2 => this.bits.BitCount >= 82
            ? this.bits.GetUnsignedInteger(12, 70)
            : null;

        /// <summary>
        /// Gets the number of reserved consecutive slots.
        /// </summary>
        public uint? SlotNumber2 => this.bits.BitCount >= 86
            ? this.bits.GetUnsignedInteger(4, 82)
            : null;

        /// <summary>
        /// Gets the time-out value in minutes.
        /// </summary>
        public uint? Timeout2 => this.bits.BitCount >= 89
            ? this.bits.GetUnsignedInteger(3, 86)
            : null;

        /// <summary>
        /// Gets the increment to repeat reservation block 2.
        /// </summary>
        public uint? Increment2 => this.bits.BitCount >= 100
            ? this.bits.GetUnsignedInteger(11, 89)
            : null;

        /// <summary>
        /// Gets the reserved offset number.
        /// </summary>
        public uint? Offset3 => this.bits.BitCount >= 112
            ? this.bits.GetUnsignedInteger(12, 100)
            : null;

        /// <summary>
        /// Gets the number of reserved consecutive slots.
        /// </summary>
        public uint? SlotNumber3 => this.bits.BitCount >= 116
            ? this.bits.GetUnsignedInteger(4, 112)
            : null;

        /// <summary>
        /// Gets the time-out value in minutes.
        /// </summary>
        public uint? Timeout3 => this.bits.BitCount >= 119
            ? this.bits.GetUnsignedInteger(3, 116)
            : null;

        /// <summary>
        /// Gets the increment to repeat reservation block 3.
        /// </summary>
        public uint? Increment3 => this.bits.BitCount >= 130
            ? this.bits.GetUnsignedInteger(11, 119)
            : null;

        /// <summary>
        /// Gets the reserved offset number.
        /// </summary>
        public uint? Offset4 => this.bits.BitCount >= 142
            ? this.bits.GetUnsignedInteger(12, 130)
            : null;

        /// <summary>
        /// Gets the number of reserved consecutive slots.
        /// </summary>
        public uint? SlotNumber4 => this.bits.BitCount >= 146
            ? this.bits.GetUnsignedInteger(4, 142)
            : null;

        /// <summary>
        /// Gets the time-out value in minutes.
        /// </summary>
        public uint? Timeout4 => this.bits.BitCount >= 149
            ? this.bits.GetUnsignedInteger(3, 146)
            : null;

        /// <summary>
        /// Gets the increment to repeat reservation block 4.
        /// </summary>
        public uint? Increment4 => this.bits.BitCount >= 160
            ? this.bits.GetUnsignedInteger(11, 149)
            : null;

        /// <summary>
        /// Gets the value of the bits in this message for which no standard meaning is currently
        /// defined.
        /// </summary>
        public uint? SpareBitsAtEnd => this.bits.BitCount switch
        {
            72 => this.bits.GetUnsignedInteger(2, 70),
            102 => this.bits.GetUnsignedInteger(2, 100),
            132 => this.bits.GetUnsignedInteger(2, 130),
            _ => null
        };
    }
}
