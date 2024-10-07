using System;

namespace Ais.Net;

/// <summary>
/// Enables fields to be extracted from a Data link management message.
/// It parses the content of messages 20.
/// </summary>
public readonly ref struct NmeaAisDataLinkManagementMessageParser
{
    readonly NmeaAisBitVectorParser _bits;

    /// <summary>
    /// Create an <see cref="NmeaAisDataLinkManagementMessageParser"/>.
    /// </summary>
    /// <param name="ascii">The ASCII-encoded message payload.</param>
    /// <param name="padding">The number of bits of padding in this payload.</param>
    public NmeaAisDataLinkManagementMessageParser( ReadOnlySpan<byte> ascii, uint padding )
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
    /// Gets the reserved offset number.
    /// </summary>
    public uint Offset1 => _bits.GetUnsignedInteger( 12, 40 );

    /// <summary>
    /// Gets the number of reserved consecutive slots.
    /// </summary>
    public uint SlotNumber1 => _bits.GetUnsignedInteger( 4, 52 );

    /// <summary>
    /// Gets the time-out value in minutes.
    /// </summary>
    public uint Timeout1 => _bits.GetUnsignedInteger( 3, 56 );

    /// <summary>
    /// Gets the increment to repeat reservation block 1.
    /// </summary>
    public uint Increment1 => _bits.GetUnsignedInteger( 11, 59 );

    /// <summary>
    /// Gets the reserved offset number.
    /// </summary>
    public uint? Offset2 => _bits.BitCount >= 82
        ? _bits.GetUnsignedInteger( 12, 70 )
        : null;

    /// <summary>
    /// Gets the number of reserved consecutive slots.
    /// </summary>
    public uint? SlotNumber2 => _bits.BitCount >= 86
        ? _bits.GetUnsignedInteger( 4, 82 )
        : null;

    /// <summary>
    /// Gets the time-out value in minutes.
    /// </summary>
    public uint? Timeout2 => _bits.BitCount >= 89
        ? _bits.GetUnsignedInteger( 3, 86 )
        : null;

    /// <summary>
    /// Gets the increment to repeat reservation block 2.
    /// </summary>
    public uint? Increment2 => _bits.BitCount >= 100
        ? _bits.GetUnsignedInteger( 11, 89 )
        : null;

    /// <summary>
    /// Gets the reserved offset number.
    /// </summary>
    public uint? Offset3 => _bits.BitCount >= 112
        ? _bits.GetUnsignedInteger( 12, 100 )
        : null;

    /// <summary>
    /// Gets the number of reserved consecutive slots.
    /// </summary>
    public uint? SlotNumber3 => _bits.BitCount >= 116
        ? _bits.GetUnsignedInteger( 4, 112 )
        : null;

    /// <summary>
    /// Gets the time-out value in minutes.
    /// </summary>
    public uint? Timeout3 => _bits.BitCount >= 119
        ? _bits.GetUnsignedInteger( 3, 116 )
        : null;

    /// <summary>
    /// Gets the increment to repeat reservation block 3.
    /// </summary>
    public uint? Increment3 => _bits.BitCount >= 130
        ? _bits.GetUnsignedInteger( 11, 119 )
        : null;

    /// <summary>
    /// Gets the reserved offset number.
    /// </summary>
    public uint? Offset4 => _bits.BitCount >= 142
        ? _bits.GetUnsignedInteger( 12, 130 )
        : null;

    /// <summary>
    /// Gets the number of reserved consecutive slots.
    /// </summary>
    public uint? SlotNumber4 => _bits.BitCount >= 146
        ? _bits.GetUnsignedInteger( 4, 142 )
        : null;

    /// <summary>
    /// Gets the time-out value in minutes.
    /// </summary>
    public uint? Timeout4 => _bits.BitCount >= 149
        ? _bits.GetUnsignedInteger( 3, 146 )
        : null;

    /// <summary>
    /// Gets the increment to repeat reservation block 4.
    /// </summary>
    public uint? Increment4 => _bits.BitCount >= 160
        ? _bits.GetUnsignedInteger( 11, 149 )
        : null;

    /// <summary>
    /// Gets the value of the bits in this message for which no standard meaning is currently
    /// defined.
    /// </summary>
    public uint? SpareBitsAtEnd => _bits.BitCount switch
    {
        72 => _bits.GetUnsignedInteger( 2, 70 ),
        102 => _bits.GetUnsignedInteger( 2, 100 ),
        132 => _bits.GetUnsignedInteger( 2, 130 ),
        _ => null
    };
}
