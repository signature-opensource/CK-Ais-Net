// <copyright file="NmeaAisAddressedBinaryMessageParser.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Ais.Net
{
    using System;

    /// <summary>
    /// Enables fields to be extracted from an Addressed Binary Message.
    /// It parses the content of messages 6.
    /// </summary>
    public readonly ref struct NmeaAisAddressedBinaryMessageParser
    {
        private readonly NmeaAisBitVectorParser bits;

        /// <summary>
        /// Create an <see cref="NmeaAisAddressedBinaryMessageParser"/>.
        /// </summary>
        /// <param name="ascii">The ASCII-encoded message payload.</param>
        /// <param name="padding">The number of bits of padding in this payload.</param>
        public NmeaAisAddressedBinaryMessageParser(ReadOnlySpan<byte> ascii, uint padding)
        {
            this.bits = new NmeaAisBitVectorParser(ascii, padding);
            this.ApplicationData = ascii.Slice(14);
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
        /// Gets the Designated area code (DAC).
        /// </summary>
        public uint DAC => this.bits.GetUnsignedInteger(10, 72);

        /// <summary>
        /// Gets the Function identifier (FI).
        /// </summary>
        public uint FI => this.bits.GetUnsignedInteger(6, 82);

        /// <summary>
        /// Gets the padding before the data in the <see cref="ApplicationData"/>.
        /// </summary>
        public uint ApplicationDataPadding => 4;

        /// <summary>
        /// Gets the application specific data.
        /// </summary>
        public ReadOnlySpan<byte> ApplicationData { get; }
    }
}
