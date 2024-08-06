// <copyright file="NmeaAisLongRangeBroadcastMessageParser.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System;

namespace Ais.Net
{
    /// <summary>
    /// Enables fields to be extracted from a Long-range Automatic Identifcation System Broadcast Message.
    /// It parses the content of messages 27.
    /// </summary>
    public readonly ref struct NmeaAisLongRangeBroadcastMessageParser
    {
        readonly NmeaAisBitVectorParser _bits;

        /// <summary>
        /// Create an <see cref="NmeaAisLongRangeBroadcastMessageParser"/>.
        /// </summary>
        /// <param name="ascii">The ASCII-encoded message payload.</param>
        /// <param name="padding">The number of bits of padding in this payload.</param>
        public NmeaAisLongRangeBroadcastMessageParser( ReadOnlySpan<byte> ascii, uint padding )
        {
            _bits = new NmeaAisBitVectorParser( ascii, padding );
        }

        /// <summary>
        /// Gets the message type.
        /// </summary>
        public MessageType MessageType => (MessageType)_bits.GetUnsignedInteger( 6, 0 );

        /// <summary>
        /// Gets the number of times this message had been repeated on this broadcast.
        /// </summary>
        /// <remarks>
        /// When stations retransmit messages with a view to enabling them to get around hills and
        /// other obstacles, this should be incremented. When it reaches 3, no more attempts should
        /// be made to retransmit it.
        /// </remarks>
        public uint RepeatIndicator => _bits.GetUnsignedInteger( 2, 6 );

        /// <summary>
        /// Gets the unique identifier assigned to the transponder that sent this message.
        /// </summary>
        public uint Mmsi => _bits.GetUnsignedInteger( 30, 8 );

        /// <summary>
        /// Gets a value indicating whether the position information is of DGPS quality.
        /// </summary>
        /// <remarks>
        /// If <c>true</c>, location information is DGPS-quality (less than 10m). If false, it is
        /// of unaugmented GNSS accuracy.
        /// </remarks>
        public bool PositionAccuracy => _bits.GetBit( 38 );

        /// <summary>
        /// Gets a value indicating whether Receiver Autonomous Integrity Monitoring is in use.
        /// </summary>
        public bool RaimFlag => _bits.GetBit( 39 );

        /// <summary>
        /// Gets the vessel's navigation status.
        /// </summary>
        public NavigationStatus NavigationStatus => (NavigationStatus)_bits.GetUnsignedInteger( 4, 40 );

        /// <summary>
        /// Gets the reported longitude, in units of 1/10 arc minutes.
        /// </summary>
        public int Longitude10thMins => _bits.GetSignedInteger( 18, 44 );

        /// <summary>
        /// Gets the reported latitude, in units of 1/10 arc minutes.
        /// </summary>
        public int Latitude10thMins => _bits.GetSignedInteger( 17, 62 );

        /// <summary>
        /// Gets the vessel's speed over ground, in tenths of a knot.
        /// </summary>
        public uint SpeedOverGround => _bits.GetUnsignedInteger( 6, 79 );

        /// <summary>
        /// Gets the vessel's course over ground.
        /// </summary>
        public uint CourseOverGround => _bits.GetUnsignedInteger( 9, 85 );

        /// <summary>
        /// Gets a value indicating whether the reported position latency is greater than 5 seconds.
        /// </summary>
        public bool PositionLatency => _bits.GetBit( 94 );

        /// <summary>
        /// Gets a value indicating whether the spare bit at offset 94 is set.
        /// </summary>
        public bool SpareBit94 => _bits.GetBit( 95 );
    }
}
