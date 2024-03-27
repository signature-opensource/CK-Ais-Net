// <copyright file="NmeaAisStandardSearchAndRescueAircraftPositionReportParser.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Ais.Net
{
    using System;

    /// <summary>
    /// Enables fields to be extracted from an AIS Standard Search and Rescue Aircraft Position Report.
    /// </summary>
    public readonly ref struct NmeaAisStandardSearchAndRescueAircraftPositionReportParser
    {
        private readonly NmeaAisBitVectorParser bits;

        /// <summary>
        /// Create an <see cref="NmeaAisStandardSearchAndRescueAircraftPositionReportParser"/>.
        /// </summary>
        /// <param name="ascii">The ASCII-encoded message payload.</param>
        /// <param name="padding">The number of bits of padding in this payload.</param>
        public NmeaAisStandardSearchAndRescueAircraftPositionReportParser(ReadOnlySpan<byte> ascii, uint padding)
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
        /// Gets the altitude, derived from GNSS or barometric.
        /// </summary>
        public uint Altitude => this.bits.GetUnsignedInteger(12, 38);

        /// <summary>
        /// Gets the vessel's speed over ground, in tenths of a knot.
        /// </summary>
        public uint SpeedOverGround => this.bits.GetUnsignedInteger(10, 50);

        /// <summary>
        /// Gets a value indicating whether the position information is of DGPS quality.
        /// </summary>
        /// <remarks>
        /// If <c>true</c>, location information is DGPS-quality (less than 10m). If false, it is
        /// of unaugmented GNSS accuracy.
        /// </remarks>
        public bool PositionAccuracy => this.bits.GetBit(60);

        /// <summary>
        /// Gets the reported longitude, in units of 1/10000 arc minutes.
        /// </summary>
        public int Longitude10000thMins => this.bits.GetSignedInteger(28, 61);

        /// <summary>
        /// Gets the reported latitude, in units of 1/10000 arc minutes.
        /// </summary>
        public int Latitude10000thMins => this.bits.GetSignedInteger(27, 89);

        /// <summary>
        /// Gets the vessel's course over ground in units of one tenth of a degree.
        /// </summary>
        public uint CourseOverGround10thDegrees => this.bits.GetUnsignedInteger(12, 116);

        /// <summary>
        /// Gets the seconds part of the (UTC) time at which the location was recorded.
        /// </summary>
        public uint TimeStampSecond => this.bits.GetUnsignedInteger(6, 128);

        /// <summary>
        /// Gets the type of altitude sensor.
        /// </summary>
        public AltitudeSensor AltitudeSensor => (AltitudeSensor)this.bits.GetUnsignedInteger(1, 134);

        /// <summary>
        /// Gets the value of the bits in this message for which no standard meaning is currently
        /// defined.
        /// </summary>
        public uint SpareBits135 => this.bits.GetUnsignedInteger(7, 135);

        /// <summary>
        /// Gets a value indicating whether the data terminal ready is not available.
        /// </summary>
        public bool DTE => this.bits.GetBit(142);

        /// <summary>
        /// Gets the value of the bits in this message for which no standard meaning is currently
        /// defined.
        /// </summary>
        public uint SpareBits143 => this.bits.GetUnsignedInteger(3, 143);

        /// <summary>
        /// Gets a value indicating whether the station operate in assigned mode.
        /// </summary>
        public bool AssignedMode => this.bits.GetBit(146);

        /// <summary>
        /// Gets a value indicating whether Receiver Autonomous Integrity Monitoring is in use.
        /// </summary>
        public bool RaimFlag => this.bits.GetBit(147);

        /// <summary>
        /// Gets the communication state follows that is used.
        /// </summary>
        public CommunicationStateSelector CommunicationStateSelector => (CommunicationStateSelector)this.bits.GetUnsignedInteger(1, 148);

        /// <summary>
        /// Gets the communication state, based on <see cref="CommunicationStateSelector"/>.
        /// </summary>
        public uint CommunicationState => this.bits.GetUnsignedInteger(19, 149);
    }
}
