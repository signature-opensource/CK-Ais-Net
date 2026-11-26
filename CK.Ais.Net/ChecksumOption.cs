using System;

namespace Ais.Net;

/// <summary>
/// Defines how the <c>*XX</c> checksum that ends an NMEA sentence or a tag block is checked.
/// The checksum is the XOR of all the characters between the start delimiter and the <c>*</c>,
/// written in hexadecimal.
/// </summary>
[Flags]
public enum ChecksumOption
{
    /// <summary>
    /// The checksum is neither format-checked nor validated: only the presence of the <c>*</c> is required.
    /// </summary>
    AllowInvalidFormat = 0,

    /// <summary>
    /// The checksum must be made of exactly two hexadecimal digits (<c>*XX</c>).
    /// Its value is not validated unless <see cref="CheckValidity"/> is also set.
    /// </summary>
    ValidateStandardFormat = 1,

    /// <summary>
    /// The checksum value must match the content of the section.
    /// Without <see cref="ValidateStandardFormat"/>, a one-digit checksum (<c>*X</c>) is accepted.
    /// </summary>
    CheckValidity = 2
}

static class ChecksumOptionExtensions
{
    internal static void Check( this ChecksumOption checksumOption, in ReadOnlySpan<byte> span )
    {
        var hashIdx = MemoryExtensions.LastIndexOf( span, (byte)'*' );
        if( hashIdx == -1 )
        {
            throw new ArgumentException( "Invalid data. Payload checksum not present - the message may have been corrupted or truncated" );
        }

        if( (checksumOption & ChecksumOption.ValidateStandardFormat) == ChecksumOption.ValidateStandardFormat )
        {
            if( hashIdx != span.Length - 3 )
            {
                throw new ArgumentException( "Line section should end with *XX hexadecimal checksum." );
            }
            if( (checksumOption & ChecksumOption.CheckValidity) == ChecksumOption.CheckValidity )
            {
                var checksum = (byte)((GetHexValue( span[^2] ) << 4) | GetHexValue( span[^1] ));
                InternalCheck( span[..hashIdx], checksum );
            }
        }
        else if( (checksumOption & ChecksumOption.CheckValidity) == ChecksumOption.CheckValidity )
        {
            if( hashIdx != span.Length - 3 && hashIdx != span.Length - 2 )
            {
                throw new ArgumentException( "Line section should end with *X or *XX hexadecimal checksum." );
            }
            var checksum = GetHexValue( span[^1] );
            if( hashIdx == span.Length - 3 )
            {
                checksum |= (byte)(GetHexValue( span[^2] ) << 4);
            }
            InternalCheck( span[..hashIdx], checksum );
        }
    }

    static byte GetHexValue( byte b ) => b switch
    {
        >= (byte)'0' and <= (byte)'9' => (byte)(b - (byte)'0'),
        >= (byte)'A' and <= (byte)'F' => (byte)(b - (byte)'A' + 10),
        _ => throw new ArgumentException( $"Section checksum should contain hexadecimal digits but received '{(char)b}'." )
    };

    static void InternalCheck( ReadOnlySpan<byte> span, byte checksum )
    {
        byte sum = 0;
        for( int i = 0; i < span.Length; i++ )
        {
            sum ^= span[i];
        }
        if( sum != checksum )
        {
            throw new ArgumentException( $"Invalid section checksum: read {checksum:X2} but computed {sum:X2} from the content." );
        }
    }
}
