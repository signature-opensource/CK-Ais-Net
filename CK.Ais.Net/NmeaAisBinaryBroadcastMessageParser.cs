using System;

namespace Ais.Net;

/// <summary>
/// Enables fields to be extracted from an AIS Binary Broadcast Message.
/// It parses the content of messages 8.
/// </summary>
public readonly ref struct NmeaAisBinaryBroadcastMessageParser
{
    readonly NmeaAisBitVectorParser _bits;

    /// <summary>
    /// Create an <see cref="NmeaAisBinaryBroadcastMessageParser"/>.
    /// </summary>
    /// <param name="ascii">The ASCII-encoded message payload.</param>
    /// <param name="padding">The number of bits of padding in this payload.</param>
    public NmeaAisBinaryBroadcastMessageParser( ReadOnlySpan<byte> ascii, uint padding )
    {
        _bits = new NmeaAisBitVectorParser( ascii, padding );
        ApplicationData = ascii.Slice( 9 );
        ApplicationDataPaddingAfter = padding;
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
    /// Gets the Designated area code (DAC).
    /// </summary>
    public uint DAC => _bits.GetUnsignedInteger( 10, 40 );

    /// <summary>
    /// Gets the Function identifier (FI).
    /// </summary>
    public uint FI => _bits.GetUnsignedInteger( 6, 50 );

    /// <summary>
    /// Gets the padding before the data in the <see cref="ApplicationData"/>.
    /// </summary>
#pragma warning disable CA1822 // Mark members as static
    public uint ApplicationDataPaddingBefore => 2;
#pragma warning restore CA1822 // Mark members as static

    /// <summary>
    /// Gets the padding after the data in the <see cref="ApplicationData"/>.
    /// </summary>
    public readonly uint ApplicationDataPaddingAfter;

    /// <summary>
    /// Gets the application specific data.
    /// </summary>
    public readonly ReadOnlySpan<byte> ApplicationData;
}
