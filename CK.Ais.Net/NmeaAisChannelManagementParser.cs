using System;

namespace Ais.Net;

/// <summary>
/// Enables fields to be extracted from a Channel management.
/// It parses the content of messages 22.
/// </summary>
public readonly ref struct NmeaAisChannelManagementParser
{
    readonly NmeaAisBitVectorParser _bits;

    /// <summary>
    /// Create an <see cref="NmeaAisChannelManagementParser"/>.
    /// </summary>
    /// <param name="ascii">The ASCII-encoded message payload.</param>
    /// <param name="padding">The number of bits of padding in this payload.</param>
    public NmeaAisChannelManagementParser( ReadOnlySpan<byte> ascii, uint padding )
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
    /// Gets the channel number of 25 kHz simplex or simplex use of 25 kHz duplex in accordance
    /// with Recommendation ITU-R M.1084.
    /// </summary>
    public uint ChannelA => _bits.GetUnsignedInteger( 12, 40 );

    /// <summary>
    /// Gets the channel number of 25 kHz simplex or simplex use of 25 kHz duplex in accordance
    /// with Recommendation ITU-R M.1084.
    /// </summary>
    public uint ChannelB => _bits.GetUnsignedInteger( 12, 52 );

    /// <summary>
    /// Gets the Tx/Rx mode.
    /// </summary>
    public uint TxRxMode => _bits.GetUnsignedInteger( 4, 64 );

    /// <summary>
    /// Gets a value indicating whether the power is low or high.
    /// </summary>
    public bool LowPower => _bits.GetBit( 68 );

    /// <summary>
    /// Gets the reported longitude, in units of 1/10 arc minutes.
    /// </summary>
    public int Longitude10thMins1 => _bits.GetSignedInteger( 18, 69 );

    /// <summary>
    /// Gets the reported latitude, in units of 1/10 arc minutes.
    /// </summary>
    public int Latitude10thMins1 => _bits.GetSignedInteger( 17, 87 );

    /// <summary>
    /// Gets the reported longitude, in units of 1/10 arc minutes.
    /// </summary>
    public int Longitude10thMins2 => _bits.GetSignedInteger( 18, 104 );

    /// <summary>
    /// Gets the reported latitude, in units of 1/10 arc minutes.
    /// </summary>
    public int Latitude10thMins2 => _bits.GetSignedInteger( 17, 122 );

    /// <summary>
    /// Gets a value indicating whether the message is used.
    /// </summary>
        public DestinationIndicator DestinationIndicator => (DestinationIndicator)_bits.GetUnsignedInteger( 1, 139 );

    /// <summary>
    /// Gets a value indicating whether badwith is used.
    /// </summary>
    public bool ChannelABandwidth => _bits.GetBit( 140 );

    /// <summary>
    /// Gets a value indicating whether badwith is used.
    /// </summary>
    public bool ChannelBBandwidth => _bits.GetBit( 141 );

    /// <summary>
    /// Gets the transitional zone size in nautical miles.
    /// </summary>
    public uint TransitionalZoneSize => _bits.GetUnsignedInteger( 3, 142 );

    /// <summary>
    /// Gets the value of the bits in this message for which no standard meaning is currently
    /// defined.
    /// </summary>
    public uint SpareBits145 => _bits.GetUnsignedInteger( 23, 145 );
}
