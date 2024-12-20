// <copyright file="NmeaAisTextFieldParser.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System;

namespace Ais.Net;

/// <summary>
/// Parses a text field such as a ship's name stored in an AIS payload of an NMEA message.
/// </summary>
public readonly ref struct NmeaAisTextFieldParser
{
    readonly NmeaAisBitVectorParser _bits;
    readonly uint _bitOffset;
    readonly uint _bitLength;

    /// <summary>
    /// Creates a <see cref="NmeaAisTextFieldParser"/>.
    /// </summary>
    /// <param name="bits">The bit vector (in 6-bit ASCII encoding) containing the name.</param>
    /// <param name="bitLength">The length of the name field in bits.</param>
    /// <param name="bitOffset">The bit offset of the name field within the bit vector.</param>
    public NmeaAisTextFieldParser( NmeaAisBitVectorParser bits, uint bitLength, uint bitOffset )
    {
        _bits = bits;
        _bitOffset = bitOffset;
        _bitLength = bitLength;
    }

    /// <summary>
    /// Gets the number of characters in this field.
    /// </summary>
    public uint CharacterCount => _bitLength / 6;

    /// <summary>
    /// Gets the ASCII value of the character at the specified position.
    /// </summary>
    /// <param name="index">Character position.</param>
    /// <returns>The ASCII value of the character at the specified position.</returns>
    public byte GetAscii( uint index )
    {
        uint bitIndexInField = index * 6;

        if( (bitIndexInField + 6) > _bitLength )
        {
            throw new ArgumentOutOfRangeException(
                nameof( index ),
                "Character index would read off end of name field" );
        }

        byte aisValue = (byte)_bits.GetUnsignedInteger( 6, _bitOffset + bitIndexInField );

        return AisStrings.AisCharacterToAsciiValue( aisValue );
    }

    /// <summary>
    /// Writes the characters into a buffer in ASCII format.
    /// </summary>
    /// <param name="targetBuffer">
    /// The target buffer, which must be at least as long as <see cref="CharacterCount"/>.
    /// </param>
    /// <remarks>
    /// The reason we don't just provide a simple property that returns a string is that it
    /// would cause a heap allocation, defeating the entire point of using <c>ref struct</c>.
    /// </remarks>
    public void WriteAsAscii( in Span<byte> targetBuffer )
    {
        for( int i = 0; i < targetBuffer.Length; ++i )
        {
            targetBuffer[i] = GetAscii( (uint)i );
        }
    }
}
