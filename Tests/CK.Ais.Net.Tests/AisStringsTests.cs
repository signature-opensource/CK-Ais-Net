// <copyright file="AisStringsTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using NUnit.Framework;
using Shouldly;

namespace Ais.Net.Tests;

// AIS text fields use a 6-bit character set: each value from 0 to 63 maps to one ASCII character.
[TestFixture]
public class AisStringsTests
{
    // The whole 6-bit table: 0-31 map to '@' through '_', 32-63 map to ' ' through '?'.
    [TestCase( (byte)0, '@' )]
    [TestCase( (byte)1, 'A' )]
    [TestCase( (byte)2, 'B' )]
    [TestCase( (byte)3, 'C' )]
    [TestCase( (byte)4, 'D' )]
    [TestCase( (byte)5, 'E' )]
    [TestCase( (byte)6, 'F' )]
    [TestCase( (byte)7, 'G' )]
    [TestCase( (byte)8, 'H' )]
    [TestCase( (byte)9, 'I' )]
    [TestCase( (byte)10, 'J' )]
    [TestCase( (byte)11, 'K' )]
    [TestCase( (byte)12, 'L' )]
    [TestCase( (byte)13, 'M' )]
    [TestCase( (byte)14, 'N' )]
    [TestCase( (byte)15, 'O' )]
    [TestCase( (byte)16, 'P' )]
    [TestCase( (byte)17, 'Q' )]
    [TestCase( (byte)18, 'R' )]
    [TestCase( (byte)19, 'S' )]
    [TestCase( (byte)20, 'T' )]
    [TestCase( (byte)21, 'U' )]
    [TestCase( (byte)22, 'V' )]
    [TestCase( (byte)23, 'W' )]
    [TestCase( (byte)24, 'X' )]
    [TestCase( (byte)25, 'Y' )]
    [TestCase( (byte)26, 'Z' )]
    [TestCase( (byte)27, '[' )]
    [TestCase( (byte)28, '\\' )]
    [TestCase( (byte)29, ']' )]
    [TestCase( (byte)30, '^' )]
    [TestCase( (byte)31, '_' )]
    [TestCase( (byte)32, ' ' )]
    [TestCase( (byte)33, '!' )]
    [TestCase( (byte)34, '"' )]
    [TestCase( (byte)35, '#' )]
    [TestCase( (byte)36, '$' )]
    [TestCase( (byte)37, '%' )]
    [TestCase( (byte)38, '&' )]
    [TestCase( (byte)39, '\'' )]
    [TestCase( (byte)40, '(' )]
    [TestCase( (byte)41, ')' )]
    [TestCase( (byte)42, '*' )]
    [TestCase( (byte)43, '+' )]
    [TestCase( (byte)44, ',' )]
    [TestCase( (byte)45, '-' )]
    [TestCase( (byte)46, '.' )]
    [TestCase( (byte)47, '/' )]
    [TestCase( (byte)48, '0' )]
    [TestCase( (byte)49, '1' )]
    [TestCase( (byte)50, '2' )]
    [TestCase( (byte)51, '3' )]
    [TestCase( (byte)52, '4' )]
    [TestCase( (byte)53, '5' )]
    [TestCase( (byte)54, '6' )]
    [TestCase( (byte)55, '7' )]
    [TestCase( (byte)56, '8' )]
    [TestCase( (byte)57, '9' )]
    [TestCase( (byte)58, ':' )]
    [TestCase( (byte)59, ';' )]
    [TestCase( (byte)60, '<' )]
    [TestCase( (byte)61, '=' )]
    [TestCase( (byte)62, '>' )]
    [TestCase( (byte)63, '?' )]
    public void AisCharacterToAsciiValue_converts_every_6_bit_value( byte aisChar, char expected )
    {
        ((char)AisStrings.AisCharacterToAsciiValue( aisChar )).ShouldBe( expected );
    }
}
