using System;

namespace Ais.Net;

/// <summary>
/// Enables fields to be extracted from a Group assignment command.
/// It parses the content of messages 23.
/// </summary>
public readonly ref struct NmeaAisGroupAssignmentCommandParser
{
    readonly NmeaAisBitVectorParser _bits;

    /// <summary>
    /// Create an <see cref="NmeaAisGroupAssignmentCommandParser"/>.
    /// </summary>
    /// <param name="ascii">The ASCII-encoded message payload.</param>
    /// <param name="padding">The number of bits of padding in this payload.</param>
    public NmeaAisGroupAssignmentCommandParser( ReadOnlySpan<byte> ascii, uint padding )
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
    /// Gets the value of the bits in this message for which no standard meaning is currently
    /// defined.
    /// </summary>
    public uint SpareBits38 => _bits.GetUnsignedInteger( 2, 38 );

    /// <summary>
    /// Gets the reported longitude, in units of 1/10 arc minutes.
    /// </summary>
    public int Longitude10thMins1 => _bits.GetSignedInteger( 18, 40 );

    /// <summary>
    /// Gets the reported latitude, in units of 1/10 arc minutes.
    /// </summary>
    public int Latitude10thMins1 => _bits.GetSignedInteger( 17, 58 );

    /// <summary>
    /// Gets the reported longitude, in units of 1/10 arc minutes.
    /// </summary>
    public int Longitude10thMins2 => _bits.GetSignedInteger( 18, 75 );

    /// <summary>
    /// Gets the reported latitude, in units of 1/10 arc minutes.
    /// </summary>
    public int Latitude10thMins2 => _bits.GetSignedInteger( 17, 93 );

    /// <summary>
    /// Gets the station type.
    /// </summary>
    public StationType StationType => (StationType)_bits.GetUnsignedInteger( 4, 110 );

    /// <summary>
    /// Gets the type of ship and cargo type.
    /// </summary>
    public uint TypeOfShipAndCargoType => _bits.GetUnsignedInteger( 8, 114 );

    /// <summary>
    /// Gets the value of the bits in this message for which no standard meaning is currently
    /// defined.
    /// </summary>
    public uint SpareBits122 => _bits.GetUnsignedInteger( 22, 122 );

    /// <summary>
    /// Gets the Tx/Rx mode.
    /// </summary>
    public uint TxRxMode => _bits.GetUnsignedInteger( 2, 144 );

    /// <summary>
    /// Gets the parameter that commands the respective stations to the reporting interval.
    /// </summary>
    public ReportingInterval ReportInterval => (ReportingInterval)_bits.GetUnsignedInteger( 4, 146 );

    /// <summary>
    /// Gets the quiet time.
    /// </summary>
    public uint QuietTime => _bits.GetUnsignedInteger( 4, 150 );

    /// <summary>
    /// Gets the value of the bits in this message for which no standard meaning is currently
    /// defined.
    /// </summary>
    public uint SpareBits154 => _bits.GetUnsignedInteger( 22, 154 );
}
