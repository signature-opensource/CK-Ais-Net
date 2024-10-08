// <copyright file="NmeaAisStaticAndVoyageRelatedDataParser.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System;

namespace Ais.Net
{
    /// <summary>
    /// Enables fields to be extracted from an AIS Static and Voyage Related Data payload in an
    /// NMEA sentence.
    /// It parses the content of messages 5.
    /// </summary>
    public readonly ref struct NmeaAisStaticAndVoyageRelatedDataParser
    {
        readonly NmeaAisBitVectorParser _bits;

        /// <summary>
        /// Create an <see cref="NmeaAisStaticAndVoyageRelatedDataParser"/>.
        /// </summary>
        /// <param name="ascii">The ASCII-encoded message payload.</param>
        /// <param name="padding">The number of bits of padding in this payload.</param>
        public NmeaAisStaticAndVoyageRelatedDataParser( ReadOnlySpan<byte> ascii, uint padding )
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
        /// Gets the AIS Version field at bit offset 38.
        /// </summary>
        /// <remarks>
        /// According to the docs, 0 meants ITU1371 and 1-3 are reserved for future editions, but
        /// in practice we see all 4 possible values here.
        /// </remarks>
        public uint AisVersion => _bits.GetUnsignedInteger( 2, 38 );

        /// <summary>
        /// Gets the IMO Ship ID number.
        /// </summary>
        public uint ImoNumber => _bits.GetUnsignedInteger( 30, 40 );

        /// <summary>
        /// Gets the Call Sign field.
        /// </summary>
        public NmeaAisTextFieldParser CallSign => new NmeaAisTextFieldParser( _bits, 42, 70 );

        /// <summary>
        /// Gets the Call Sign field.
        /// </summary>
        public NmeaAisTextFieldParser VesselName => new NmeaAisTextFieldParser( _bits, 120, 112 );

        /// <summary>
        /// Gets the ship and cargo type.
        /// </summary>
        public ShipType ShipType => (ShipType)_bits.GetUnsignedInteger( 8, 232 );

        /// <summary>
        /// Gets the distance in metres from the unit to the bow.
        /// </summary>
        public uint DimensionToBow => _bits.GetUnsignedInteger( 9, 240 );

        /// <summary>
        /// Gets the distance in metres from the unit to the stern.
        /// </summary>
        public uint DimensionToStern => _bits.GetUnsignedInteger( 9, 249 );

        /// <summary>
        /// Gets the distance in metres from the unit to port.
        /// </summary>
        public uint DimensionToPort => _bits.GetUnsignedInteger( 6, 258 );

        /// <summary>
        /// Gets the distance in metres from the unit to starboard.
        /// </summary>
        public uint DimensionToStarboard => _bits.GetUnsignedInteger( 6, 264 );

        /// <summary>
        /// Gets the position fix type.
        /// </summary>
        public EpfdFixType PositionFixType => (EpfdFixType)_bits.GetUnsignedInteger( 4, 270 );

        /// <summary>
        /// Gets the month of the estimated time to arrival, or 0 if not available.
        /// </summary>
        public uint EtaMonth => _bits.GetUnsignedInteger( 4, 274 );

        /// <summary>
        /// Gets the day of the estimated time to arrival, or 0 if not available.
        /// </summary>
        public uint EtaDay => _bits.GetUnsignedInteger( 5, 278 );

        /// <summary>
        /// Gets the hour of the estimated time to arrival, or 0 if not available.
        /// </summary>
        public uint EtaHour => _bits.GetUnsignedInteger( 5, 283 );

        /// <summary>
        /// Gets the minute of the estimated time to arrival, or 0 if not available.
        /// </summary>
        public uint EtaMinute => _bits.GetUnsignedInteger( 6, 288 );

        /// <summary>
        /// Gets the vessel's draught in tenths of a metre.
        /// </summary>
        public uint Draught10thMetres => _bits.GetUnsignedInteger( 8, 294 );

        /// <summary>
        /// Gets the Destination field.
        /// </summary>
        public NmeaAisTextFieldParser Destination => new NmeaAisTextFieldParser( _bits, 120, 302 );

        /// <summary>
        /// Gets a value indicating whether the data terminal is in a not ready state.
        /// </summary>
        public bool IsDteNotReady => _bits.GetBit( 422 );

        /// <summary>
        /// Gets the value of the 'spare' bit at 423.
        /// </summary>
        public bool SpareBit423 => _bits.GetBit( 423 );
    }
}
