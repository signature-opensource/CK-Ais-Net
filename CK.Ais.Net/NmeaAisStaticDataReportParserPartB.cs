// <copyright file="NmeaAisStaticDataReportParserPartB.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System;

namespace Ais.Net;

/// <summary>
/// Enables fields to be extracted from an AIS Static Data Report Part B payload in an
/// NMEA sentence.
/// It parses the content of messages 24 part B.
/// </summary>
public readonly ref struct NmeaAisStaticDataReportParserPartB
{
    readonly NmeaAisBitVectorParser _bits;

    /// <summary>
    /// Create an <see cref="NmeaAisStaticDataReportParserPartB"/>.
    /// </summary>
    /// <param name="ascii">The ASCII-encoded message payload.</param>
    /// <param name="padding">The number of bits of padding in this payload.</param>
    public NmeaAisStaticDataReportParserPartB( ReadOnlySpan<byte> ascii, uint padding )
    {
        _bits = new NmeaAisBitVectorParser( ascii, padding );
        if( PartNumber != 1 )
        {
            throw new ArgumentException( $"This is a parser for Part B (1) messages, but the part number of the message supplied is {PartNumber}" );
        }
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
    /// Gets the Part Number field.
    /// </summary>
    public uint PartNumber => _bits.GetUnsignedInteger( 2, 38 );

    /// <summary>
    /// Gets the ship and cargo type.
    /// </summary>
    public ShipType ShipType => (ShipType)_bits.GetUnsignedInteger( 8, 40 );

    #region Vendor Identification Field

    /// <summary>
    /// Gets the Vendor field as specified by ITU-R 1371-3.
    /// </summary>
    public NmeaAisTextFieldParser VendorIdRev3 => new NmeaAisTextFieldParser( _bits, 42, 48 );

    /// <summary>
    /// Gets the Vendor field as specified by ITU-R 1371-4.
    /// </summary>
    public NmeaAisTextFieldParser VendorIdRev4 => new NmeaAisTextFieldParser( _bits, 18, 48 );

    /// <summary>
    /// Gets the Unit Model Code (only on messages conforming to ITU-R 1371-4 or later).
    /// </summary>
    public uint UnitModelCode => _bits.GetUnsignedInteger( 4, 66 );

    /// <summary>
    /// Gets the Serial Number (only on messages conforming to ITU-R 1371-4 or later).
    /// </summary>
    public uint SerialNumber => _bits.GetUnsignedInteger( 20, 70 );

    #endregion

    /// <summary>
    /// Gets the Call Sign field.
    /// </summary>
    public NmeaAisTextFieldParser CallSign => new NmeaAisTextFieldParser( _bits, 42, 90 );

    /// <summary>
    /// Gets the distance in metres from the unit to the bow. (Not present is this is an
    /// auxiliary craft.)
    /// </summary>
    public uint DimensionToBow => _bits.GetUnsignedInteger( 9, 132 );

    /// <summary>
    /// Gets the distance in metres from the unit to the stern. (Not present is this is an
    /// auxiliary craft.)
    /// </summary>
    public uint DimensionToStern => _bits.GetUnsignedInteger( 9, 141 );

    /// <summary>
    /// Gets the distance in metres from the unit to port. (Not present is this is an
    /// auxiliary craft.)
    /// </summary>
    public uint DimensionToPort => _bits.GetUnsignedInteger( 6, 150 );

    /// <summary>
    /// Gets the distance in metres from the unit to starboard. (Not present is this is an
    /// auxiliary craft.)
    /// </summary>
    public uint DimensionToStarboard => _bits.GetUnsignedInteger( 6, 156 );

    /// <summary>
    /// Gets the mothership MMSI. (Only present if <see cref="Mmsi"/> indicates that this is
    /// an auxiliary craft.)
    /// </summary>
    public uint MothershipMmsi => _bits.GetUnsignedInteger( 30, 132 );

    /// <summary>
    /// Gets the position fix type.
    /// </summary>
    public EpfdFixType EpfdFixType => (EpfdFixType)_bits.GetUnsignedInteger( 4, 162 );

    /// <summary>
    /// Gets the value of the 'spare' bits at 162.
    /// </summary>
    public uint Spare162 => _bits.GetUnsignedInteger( 2, 166 );
}
