using System;

namespace Ais.Net;

/// <summary>
/// Enables fields to be extracted from an Acknowledge.
/// It parses the content of messages 7 and 13.
/// </summary>
public readonly ref struct NmeaAisAcknowledgementParser
{
    readonly NmeaAisBitVectorParser _bits;

    /// <summary>
    /// Create an <see cref="NmeaAisAcknowledgementParser"/>.
    /// </summary>
    /// <param name="ascii">The ASCII-encoded message payload.</param>
    /// <param name="padding">The number of bits of padding in this payload.</param>
    public NmeaAisAcknowledgementParser( ReadOnlySpan<byte> ascii, uint padding )
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
    /// Gets the MMSI number of first destination of this ACK.
    /// </summary>
    public uint DestinationMmsi1 => _bits.GetUnsignedInteger( 30, 40 );

    /// <summary>
    /// Gets the sequence number of message to be acknowledged.
    /// </summary>
    public uint SequenceNumberMmsi1 => _bits.GetUnsignedInteger( 2, 70 );

    /// <summary>
    /// Gets the MMSI number of second destination of this ACK.
    /// </summary>
    public uint? DestinationMmsi2 => _bits.BitCount >= 102
        ? _bits.GetUnsignedInteger( 30, 72 )
        : null;

    /// <summary>
    /// Gets the sequence number of message to be acknowledged.
    /// </summary>
    public uint? SequenceNumberMmsi2 => _bits.BitCount >= 104
        ? _bits.GetUnsignedInteger( 2, 102 )
        : null;

    /// <summary>
    /// Gets the MMSI number of third destination of this ACK.
    /// </summary>
    public uint? DestinationMmsi3 => _bits.BitCount >= 134
        ? _bits.GetUnsignedInteger( 30, 104 )
        : null;

    /// <summary>
    /// Gets the sequence number of message to be acknowledged.
    /// </summary>
    public uint? SequenceNumberMmsi3 => _bits.BitCount >= 136
        ? _bits.GetUnsignedInteger( 2, 134 )
        : null;

    /// <summary>
    /// Gets the MMSI number of fourth destination of this ACK.
    /// </summary>
    public uint? DestinationMmsi4 => _bits.BitCount >= 166
        ? _bits.GetUnsignedInteger( 30, 136 )
        : null;

    /// <summary>
    /// Gets the sequence number of message to be acknowledged.
    /// </summary>
    public uint? SequenceNumberMmsi4 => _bits.BitCount >= 168
        ? _bits.GetUnsignedInteger( 2, 166 )
        : null;
}
