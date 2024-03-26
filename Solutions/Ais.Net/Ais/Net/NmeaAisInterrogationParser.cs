﻿// <copyright file="NmeaAisInterrogationParser.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Ais.Net
{
    using System;

    /// <summary>
    /// Enables fields to be extracted from an Interrogation.
    /// </summary>
    public readonly ref struct NmeaAisInterrogationParser
    {
        private readonly NmeaAisBitVectorParser bits;

        /// <summary>
        /// Create an <see cref="NmeaAisInterrogationParser"/>.
        /// </summary>
        /// <param name="ascii">The ASCII-encoded message payload.</param>
        /// <param name="padding">The number of bits of padding in this payload.</param>
        public NmeaAisInterrogationParser(ReadOnlySpan<byte> ascii, uint padding)
        {
            this.bits = new NmeaAisBitVectorParser(ascii, padding);
        }

        /// <summary>
        /// Gets the.
        /// </summary>
        public uint Length => this.bits.BitCount;

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
        /// Gets the MMSI number of first interrogated station.
        /// </summary>
        public uint DestinationMmsi1 => this.bits.GetUnsignedInteger(30, 40);

        /// <summary>
        /// Gets the first requested message type from first interrogated station.
        /// </summary>
        public uint MessageType11 => this.bits.GetUnsignedInteger(6, 70);

        /// <summary>
        /// Gets the response slot offset for first requested message from first interrogated
        /// station.
        /// </summary>
        public uint SlotOffset11 => this.bits.GetUnsignedInteger(12, 76);

        // ----- Nullable section -----

        /// <summary>
        /// Gets the value of the bits in this message for which no standard meaning is currently
        /// defined.
        /// </summary>
        public uint? SpareBits88 => this.bits.BitCount < 90 ? null : this.bits.GetUnsignedInteger(2, 88);

        /// <summary>
        /// Gets the second requested message type from first interrogated station.
        /// </summary>
        public uint? MessageType12 => this.bits.BitCount < 96 ? null : this.bits.GetUnsignedInteger(6, 90);

        /// <summary>
        /// Gets the response slot offset for second requested message from first interrogated
        /// station.
        /// </summary>
        public uint? SlotOffset12 => this.bits.BitCount < 108 ? null : this.bits.GetUnsignedInteger(12, 96);

        /// <summary>
        /// Gets the value of the bits in this message for which no standard meaning is currently
        /// defined.
        /// </summary>
        public uint? SpareBits108 => this.bits.BitCount < 110 ? null : this.bits.GetUnsignedInteger(2, 108);

        /// <summary>
        /// Gets the MMSI number of second interrogated station.
        /// </summary>
        public uint? DestinationMmsi2 => this.bits.BitCount < 140 ? null : this.bits.GetUnsignedInteger(30, 110);

        /// <summary>
        /// Gets the requested message type from second interrogated station.
        /// </summary>
        public uint? MessageType21 => this.bits.BitCount < 146 ? null : this.bits.GetUnsignedInteger(6, 140);

        /// <summary>
        /// Gets the response slot offset for requested message from second interrogated station.
        /// </summary>
        public uint? SlotOffset21 => this.bits.BitCount < 158 ? null : this.bits.GetUnsignedInteger(12, 146);

        /// <summary>
        /// Gets the value of the bits in this message for which no standard meaning is currently
        /// defined.
        /// </summary>
        public uint? SpareBits158 => this.bits.BitCount < 160 ? null : this.bits.GetUnsignedInteger(2, 158);

        // ----------------------------
    }
}