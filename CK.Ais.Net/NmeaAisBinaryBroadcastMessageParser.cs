// <copyright file="NmeaAisBinaryBroadcastMessageParser.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Ais.Net
{
    using System;

    /// <summary>
    /// Enables fields to be extracted from an AIS Binary Broadcast Message.
    /// It parses the content of messages 8.
    /// </summary>
    public readonly ref struct NmeaAisBinaryBroadcastMessageParser
    {
        private readonly NmeaAisBitVectorParser bits;

        /// <summary>
        /// Create an <see cref="NmeaAisBinaryBroadcastMessageParser"/>.
        /// </summary>
        /// <param name="ascii">The ASCII-encoded message payload.</param>
        /// <param name="padding">The number of bits of padding in this payload.</param>
        public NmeaAisBinaryBroadcastMessageParser(ReadOnlySpan<byte> ascii, uint padding)
        {
            this.bits = new NmeaAisBitVectorParser(ascii, padding);
            this.ApplicationData = ascii.Slice(9);
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
        /// Gets the Designated area code (DAC).
        /// </summary>
        public uint DAC => this.bits.GetUnsignedInteger(10, 40);

        /// <summary>
        /// Gets the Function identifier (FI).
        /// </summary>
        public uint FI => this.bits.GetUnsignedInteger(6, 50);

        /// <summary>
        /// Gets the padding before the data in the <see cref="ApplicationData"/>.
        /// </summary>
        public uint ApplicationDataPadding => 2;

        /// <summary>
        /// Gets the application specific data.
        /// </summary>
        public ReadOnlySpan<byte> ApplicationData { get; }
    }
}
