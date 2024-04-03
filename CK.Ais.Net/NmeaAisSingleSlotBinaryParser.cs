// <copyright file="NmeaAisSingleSlotBinaryParser.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Ais.Net
{
    using System;

    /// <summary>
    /// Enables fields to be extracted from an AIS Single Slot Binary Message.
    /// It parses the content of messages 25.
    /// </summary>
    public readonly ref struct NmeaAisSingleSlotBinaryParser
    {
        private readonly NmeaAisBitVectorParser bits;
        private readonly bool hasDestination;

        /// <summary>
        /// Create an <see cref="NmeaAisSingleSlotBinaryParser"/>.
        /// </summary>
        /// <param name="ascii">The ASCII-encoded message payload.</param>
        /// <param name="padding">The number of bits of padding in this payload.</param>
        public NmeaAisSingleSlotBinaryParser(ReadOnlySpan<byte> ascii, uint padding)
        {
            this.bits = new NmeaAisBitVectorParser(ascii, padding);
            this.hasDestination = this.DestinationIndicator == DestinationIndicator.Addressed;

            switch ((this.hasDestination, this.BinaryDataFlag))
            {
                case (true, true):
                    this.ApplicationDataPadding = 4;
                    this.ApplicationData = ascii.Slice(14);
                    break;
                case (true, false):
                    this.ApplicationDataPadding = 0;
                    this.ApplicationData = ascii.Slice(12);
                    break;
                case (false, true):
                    this.ApplicationDataPadding = 2;
                    this.ApplicationData = ascii.Slice(9);
                    break;
                case (false, false):
                    this.ApplicationDataPadding = 4;
                    this.ApplicationData = ascii.Slice(6);
                    break;
            }
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
        /// Gets a value indicating whether the <see cref="DestinationMmsi"/> is used.
        /// </summary>
        public DestinationIndicator DestinationIndicator => (DestinationIndicator)this.bits.GetUnsignedInteger(1, 38);

        /// <summary>
        /// Gets a value indicating whether the application identifier (<see cref="DAC"/> and <see cref="FI"/>) are used.
        /// </summary>
        public bool BinaryDataFlag => this.bits.GetBit(39);

        /// <summary>
        /// Gets the unique identifier assigned to the transponder who the message is for.
        /// </summary>
        public uint? DestinationMmsi => this.hasDestination
            ? this.bits.GetUnsignedInteger(30, 40)
            : null;

        /// <summary>
        /// Gets the value of the bits in this message for which no standard meaning is currently
        /// defined.
        /// </summary>
        public uint? SpareBits70 => this.hasDestination
            ? this.bits.GetUnsignedInteger(2, 70)
            : null;

        /// <summary>
        /// Gets the Designated area code (DAC).
        /// </summary>
        public uint? DAC => this.BinaryDataFlag
            ? this.bits.GetUnsignedInteger(10, this.hasDestination ? 72u : 40u)
            : null;

        /// <summary>
        /// Gets the Function identifier (FI).
        /// </summary>
        public uint? FI => this.BinaryDataFlag
            ? this.bits.GetUnsignedInteger(6, this.hasDestination ? 82u : 50u)
            : null;

        /// <summary>
        /// Gets the padding before the data in the <see cref="ApplicationData"/>.
        /// </summary>
        public uint ApplicationDataPadding { get; }

        /// <summary>
        /// Gets the application specific data.
        /// </summary>
        public ReadOnlySpan<byte> ApplicationData { get; }
    }
}
