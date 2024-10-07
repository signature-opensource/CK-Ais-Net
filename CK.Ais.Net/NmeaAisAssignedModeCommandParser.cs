using System;

namespace Ais.Net;

/// <summary>
/// Enables fields to be extracted from an Assigned mode command.
/// It parses the content of messages 16.
/// </summary>
public readonly ref struct NmeaAisAssignedModeCommandParser
{
    readonly NmeaAisBitVectorParser _bits;

    /// <summary>
    /// Create an <see cref="NmeaAisAssignedModeCommandParser"/>.
    /// </summary>
    /// <param name="ascii">The ASCII-encoded message payload.</param>
    /// <param name="padding">The number of bits of padding in this payload.</param>
    public NmeaAisAssignedModeCommandParser( ReadOnlySpan<byte> ascii, uint padding )
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
    /// Gets the MMSI number of A destination.
    /// </summary>
    public uint DestinationMmsiA => _bits.GetUnsignedInteger( 30, 40 );

    /// <summary>
    /// Gets the offset from current slot to first assigned slot.
    /// </summary>
    public uint OffsetA => _bits.GetUnsignedInteger( 12, 70 );

    /// <summary>
    /// Gets the increment to next assigned slot.
    /// </summary>
    public uint IncrementA => _bits.GetUnsignedInteger( 10, 82 );

    /// <summary>
    /// Gets the MMSI number of B destination.
    /// </summary>
    public uint? DestinationMmsiB => _bits.BitCount >= 122
        ? _bits.GetUnsignedInteger( 30, 92 )
        : null;

    /// <summary>
    /// Gets the offset from current slot to first assigned slot.
    /// </summary>
    public uint? OffsetB => _bits.BitCount >= 134
        ? _bits.GetUnsignedInteger( 12, 122 )
        : null;

    /// <summary>
    /// Gets the increment to next assigned slot.
    /// </summary>
    public uint? IncrementB => _bits.BitCount >= 144
        ? _bits.GetUnsignedInteger( 10, 134 )
        : null;

    /// <summary>
    /// Gets the value of the bits in this message for which no standard meaning is currently
    /// defined.
    /// </summary>
    public uint? SpareBitsAtEnd => _bits.BitCount == 96
        ? _bits.GetUnsignedInteger( 4, 92 )
        : null;
}
