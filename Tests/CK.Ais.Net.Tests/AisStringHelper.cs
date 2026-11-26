// <copyright file="AisStringHelper.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Shouldly;

namespace Ais.Net.Tests;

static class AisStringHelper
{
    /// <summary>
    /// Checks that a fixed-size AIS text field decodes to <paramref name="expected"/>.
    /// </summary>
    /// <remarks>
    /// Text fields are supposed to be padded with '@', but real transponders often pad with spaces.
    /// The expected value is therefore padded with spaces up to <paramref name="fieldSizeInChars"/>:
    /// a test that expects '@' padding writes the '@' explicitly.
    /// </remarks>
    public static void ShouldBeAisString( this in NmeaAisTextFieldParser parser, string expected, int fieldSizeInChars )
    {
        expected = expected.PadRight( fieldSizeInChars, ' ' );
        var actual = new char[expected.Length];
        for( int i = 0; i < actual.Length; ++i )
        {
            actual[i] = (char)parser.GetAscii( (uint)i );
        }
        new string( actual ).ShouldBe( expected );
    }
}
