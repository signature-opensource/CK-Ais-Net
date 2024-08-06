// <copyright file="NmeaAisPositionReportExtendedClassBParser.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System;

namespace Ais.Net
{
    /// <summary>
    /// Enables fields to be extracted from an AIS Extended Class B CS Position Report payload in an NMEA
    /// sentence.
    /// It parses the content of messages 19.
    /// </summary>
    public readonly ref struct NmeaAisPositionReportExtendedClassBParser
    {
        readonly NmeaAisBitVectorParser _bits;

        /// <summary>
        /// Create an <see cref="NmeaAisPositionReportExtendedClassBParser"/>.
        /// </summary>
        /// <param name="ascii">The ASCII-encoded message payload.</param>
        /// <param name="padding">The number of bits of padding in this payload.</param>
        public NmeaAisPositionReportExtendedClassBParser( ReadOnlySpan<byte> ascii, uint padding )
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
        /// Gets the 8 bits of 'regional reserved' data starting at bit 38.
        /// </summary>
        public byte RegionalReserved38 => (byte)_bits.GetUnsignedInteger( 8, 38 );

        /// <summary>
        /// Gets the vessel's speed over ground, in tenths of a knot.
        /// </summary>
        public uint SpeedOverGroundTenths => _bits.GetUnsignedInteger( 10, 46 );

        /// <summary>
        /// Gets a value indicating whether the position information is of DGPS quality.
        /// </summary>
        /// <remarks>
        /// If <c>true</c>, location information is DGPS-quality (less than 10m). If false, it is
        /// of unaugmented GNSS accuracy.
        /// </remarks>
        public bool PositionAccuracy => _bits.GetBit( 56 );

        /// <summary>
        /// Gets the reported longitude, in units of 1/10000 arc minutes.
        /// </summary>
        public int Longitude10000thMins => _bits.GetSignedInteger( 28, 57 );

        /// <summary>
        /// Gets the reported latitude, in units of 1/10000 arc minutes.
        /// </summary>
        public int Latitude10000thMins => _bits.GetSignedInteger( 27, 85 );

        /// <summary>
        /// Gets the vessel's course over ground in units of one tenth of a degree.
        /// </summary>
        public uint CourseOverGround10thDegrees => _bits.GetUnsignedInteger( 12, 112 );

        /// <summary>
        /// Gets the vessel's heading in degrees.
        /// </summary>
        public uint TrueHeadingDegrees => _bits.GetUnsignedInteger( 9, 124 );

        /// <summary>
        /// Gets the seconds part of the (UTC) time at which the location was recorded.
        /// </summary>
        public uint TimeStampSecond => _bits.GetUnsignedInteger( 6, 133 );

        /// <summary>
        /// Gets the 4 bits of 'regional reserved' data starting at bit 38.
        /// </summary>
        public byte RegionalReserved139 => (byte)_bits.GetUnsignedInteger( 4, 139 );

        /// <summary>
        /// Gets the ship name field.
        /// </summary>
        public NmeaAisTextFieldParser ShipName => new NmeaAisTextFieldParser( _bits, 120, 143 );

        /// <summary>
        /// Gets the ship and cargo type.
        /// </summary>
        public ShipType ShipType => (ShipType)_bits.GetUnsignedInteger( 8, 263 );

        /// <summary>
        /// Gets the distance in metres from the unit to the bow.
        /// </summary>
        public uint DimensionToBow => _bits.GetUnsignedInteger( 9, 271 );

        /// <summary>
        /// Gets the distance in metres from the unit to the stern.
        /// </summary>
        public uint DimensionToStern => _bits.GetUnsignedInteger( 9, 280 );

        /// <summary>
        /// Gets the distance in metres from the unit to port.
        /// </summary>
        public uint DimensionToPort => _bits.GetUnsignedInteger( 6, 289 );

        /// <summary>
        /// Gets the distance in metres from the unit to starboard.
        /// </summary>
        public uint DimensionToStarboard => _bits.GetUnsignedInteger( 6, 295 );

        /// <summary>
        /// Gets the position fix type.
        /// </summary>
        public EpfdFixType PositionFixType => (EpfdFixType)_bits.GetUnsignedInteger( 4, 301 );

        /// <summary>
        /// Gets a value indicating whether Receiver Autonomous Integrity Monitoring is in use.
        /// </summary>
        public bool RaimFlag => _bits.GetBit( 305 );

        /// <summary>
        /// Gets a value indicating whether the data terminal is in a not ready state.
        /// </summary>
        public bool IsDteNotReady => _bits.GetBit( 306 );

        /// <summary>
        /// Gets a value indicating whether the unit is running in assigned mode. If false, the
        /// unit is in autonomous mode.
        /// </summary>
        public bool IsAssigned => _bits.GetBit( 307 );

        /// <summary>
        /// Gets the value of the 'spare' bits from 308 to 311.
        /// </summary>
        public uint Spare308 => _bits.GetUnsignedInteger( 4, 308 );
    }
}
