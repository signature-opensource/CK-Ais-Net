// <copyright file="NmeaAisAddressedSafetyRelatedMessageParser.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Ais.Net
{
    using System;

    /// <summary>
    /// Enables fields to be extracted from an AIS Addressed Safety Related Message.
    /// </summary>
    public readonly ref struct NmeaAisAddressedSafetyRelatedMessageParser
    {
        private readonly NmeaAisBitVectorParser bits;

        /// <summary>
        /// Create an <see cref="NmeaAisAddressedSafetyRelatedMessageParser"/>.
        /// </summary>
        /// <param name="ascii">The ASCII-encoded message payload.</param>
        /// <param name="padding">The number of bits of padding in this payload.</param>
        public NmeaAisAddressedSafetyRelatedMessageParser(ReadOnlySpan<byte> ascii, uint padding)
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

        /// <summary>
        /// Gets the sequence number.
        /// </summary>
        public uint SequenceNumber => this.bits.GetUnsignedInteger(2, 38);

        /// <summary>
        /// Gets the unique identifier assigned to the transponder who the message is for.
        /// </summary>
        public uint DestinationMmsi => this.bits.GetUnsignedInteger(30, 40);

        /// <summary>
        /// Gets a value indicating whether the message is retransmited.
        /// </summary>
        public bool Retransmit => this.bits.GetBit(70);

        /// <summary>
        /// Gets a value indicating whether the spare bit at offset 71 is set.
        /// </summary>
        public bool SpareBit71 => this.bits.GetBit(71);

        /// <summary>
        /// Gets the safety related text.
        /// </summary>
        public NmeaAisTextFieldParser SafetyRelatedText => new NmeaAisTextFieldParser(this.bits, this.bits.BitCount - 72, 72);
    }
}
