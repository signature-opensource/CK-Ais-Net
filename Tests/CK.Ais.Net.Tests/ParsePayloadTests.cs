// <copyright file="ParsePayloadTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>
//
// Contains data under the Norwegian licence for Open Government data (NLOD) distributed by
// the Norwegian Costal Administration - https://ais.kystverket.no/
// The license can be found at https://data.norge.no/nlod/en/2.0
// The lines in this file that contain data from this source are annotated with a comment containing "ais.kystverket.no"
// The NLOD applies only to the data in these annotated lines. The license under which you are using this software
// (either the AGPLv3, or a commercial license) applies to the whole file.

using NUnit.Framework;
using Shouldly;
using System.Text;

namespace Ais.Net.Tests;

// NmeaPayloadParser.PeekMessageType reads the message type (the first 6 bits of the payload)
// without decoding the whole message: this is what lets a consumer pick the right parser.
[TestFixture]
public class ParsePayloadTests
{
    // For each message type, a synthetic payload (only the type is set) and a real one.
    [TestCase( "1000000000000000000000000000", 0u, 1 )]
    [TestCase( "13nW5<00000IoPlSbE`:P8EH0534", 0u, 1 )]   // ais.kystverket.no
    [TestCase( "2000000000000000000000000000", 0u, 2 )]
    [TestCase( "24c`1`001pPEGSLR:=df3@4620SQ", 0u, 2 )]   // ais.kystverket.no
    [TestCase( "3000000000000000000000000000", 0u, 3 )]
    [TestCase( "33m9UtPP@50wwE:VJW6LS67H01<@", 0u, 3 )]   // ais.kystverket.no
    [TestCase( "50000000000000000000000000000000000000000000000000000000000000000000000", 2u, 5 )]
    [TestCase( "53m8lk`00000hS;;SP0Hu<p61HTdTpL00000000F0H<654@pt0;@0000000000000000000", 2u, 5 )]   // ais.kystverket.no
    [TestCase( "B000000000000000000000000000", 0u, 18 )]
    [TestCase( "B3o8B<00F8:0h694gOtbgwqUoP06", 0u, 18 )]   // ais.kystverket.no
    [TestCase( "H000000000000000000000000000", 0u, 24 )]
    [TestCase( "H3m9b308tL5<d`4E80000000000", 2u, 24 )]   // ais.kystverket.no
    public void PeekMessageType_returns_the_type_without_decoding_the_whole_payload( string payload, uint padding, int expectedType )
    {
        NmeaPayloadParser.PeekMessageType( Encoding.ASCII.GetBytes( payload ), padding ).ShouldBe( expectedType );
    }
}
