// <copyright file="NmeaAisBitVectorParserTests.cs" company="Endjin Limited">
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

// NmeaAisBitVectorParser extracts arbitrary bit ranges from a payload encoded with the
// 'AIVDM/AIVDO Payload Armoring' (6 bits per ASCII character): unsigned integers aligned or not
// on a character boundary, signed (two's complement) integers and single bits, including at the
// very end of the data and with padding.
[TestFixture]
public class NmeaAisBitVectorParserTests
{
    // A 6-bit unsigned integer that starts on a character boundary is the value of that character.
    // Message type field at start of message
    [TestCase( "1000000000000000000000000000", 0u, 0u, 1u )]
    [TestCase( "13nW5<00000IoPlSbE`:P8EH0534", 0u, 0u, 1u )]   // ais.kystverket.no
    [TestCase( "2000000000000000000000000000", 0u, 0u, 2u )]
    [TestCase( "24c`1`001kPEGSLR98IP00462D0s", 0u, 0u, 2u )]   // ais.kystverket.no
    [TestCase( "3000000000000000000000000000", 0u, 0u, 3u )]
    [TestCase( "33n24L00000p3bHUiw<v46`60?Kk", 0u, 0u, 3u )]   // ais.kystverket.no
    [TestCase( "50000000000000000000000000000000000000000000000000000000", 0u, 0u, 5u )]
    [TestCase( "53m89J81SR44hdhL0004eH5A84q<00000000001@4P8825LD08j0DTm0A000", 0u, 0u, 5u )]   // ais.kystverket.no
    [TestCase( "B000000000000000000000000000", 0u, 0u, 18u )]
    [TestCase( "B3q35T005h<0h@`Dd:i;gwRUoP06", 0u, 0u, 18u )]   // ais.kystverket.no
    [TestCase( "H00000000000000000000000000", 2u, 0u, 24u )]
    [TestCase( "H3mhO30PDT@V04pU@4000000000", 2u, 0u, 24u )]   // ais.kystverket.no
    // Minutes and seconds in base station report
    [TestCase( "40000000000M0000000000000000", 0u, 66u, 29u )]
    [TestCase( "400000000000c000000000000000", 0u, 72u, 43u )]
    [TestCase( "40000000000O0000000000000000", 0u, 66u, 31u )]
    [TestCase( "400000000000T000000000000000", 0u, 72u, 36u )]
    // End of data
    [TestCase( "ABC0", 0u, 18u, 0u )]
    [TestCase( "ABC1", 0u, 18u, 1u )]
    [TestCase( "ABC:", 0u, 18u, 10u )]
    [TestCase( "ABCA", 0u, 18u, 17u )]
    [TestCase( "ABC`", 0u, 18u, 40u )]
    [TestCase( "ABCa", 0u, 18u, 41u )]
    [TestCase( "ABCw", 0u, 18u, 63u )]
    public void unsigned_aligned_6_bit_integer( string payload, uint padding, uint offset, uint expected )
    {
        var parser = new NmeaAisBitVectorParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.GetUnsignedInteger( 6, offset ).ShouldBe( expected );
    }

    // A 6-bit unsigned integer that straddles two characters is reassembled from both.
    // Time Stamp in Common Navigation Block
    [TestCase( "1000000000000000000000020000", 0u, 137u, 1u )]
    [TestCase( "1000000000000000000000040000", 0u, 137u, 2u )]
    [TestCase( "1000000000000000000000060000", 0u, 137u, 3u )]
    [TestCase( "1000000000000000000000100000", 0u, 137u, 32u )]
    [TestCase( "1000000000000000000000120000", 0u, 137u, 33u )]
    [TestCase( "10000000000000000000001d0000", 0u, 137u, 54u )]
    [TestCase( "10000000000000000000001f0000", 0u, 137u, 55u )]
    [TestCase( "13nW5<00000IoPlSbE`:P8EH0534", 0u, 137u, 44u )]   // ais.kystverket.no
    [TestCase( "13oO7800000FH>>S6rpur`P200S<", 0u, 137u, 1u )]   // ais.kystverket.no
    // Functional ID in Binary Addressed Message
    [TestCase( "00000000000000000000000000000000000000000000000", 2u, 82u, 0u )]
    [TestCase( "00000000000000400000000000000000000000000000000", 2u, 82u, 1u )]
    [TestCase( "00000000000000800000000000000000000000000000000", 2u, 82u, 2u )]
    [TestCase( "00000000000001000000000000000000000000000000000", 2u, 82u, 16u )]
    [TestCase( "00000000000001400000000000000000000000000000000", 2u, 82u, 17u )]
    [TestCase( "00000000000002000000000000000000000000000000000", 2u, 82u, 32u )]
    [TestCase( "00000000000002H00000000000000000000000000000000", 2u, 82u, 38u )]
    [TestCase( "00000000000003t00000000000000000000000000000000", 2u, 82u, 63u )]
    //    e      o      g      f
    // 101101 110111 101111 101110
    //  01101 1  - 27
    //   1101 11  - 55
    //    101 110  - 46
    //     01 1101  - 29
    //      1 11011  - 59
    //         10111 1  - 47
    //          0111 10  - 30
    //           111 101  - 61
    //            11 1011  - 59
    //             1 10111  - 55
    //                01111 1  - 31
    //                 1111 10  - 62
    //                  111 101  - 61
    //                   11 1011  - 59
    //                    1 10111  - 55
    // Synthetic examples testing at various bit positions
    [TestCase( "eogf", 1u, 1u, 27u )]
    [TestCase( "eogf", 1u, 2u, 55u )]
    [TestCase( "eogf", 1u, 3u, 46u )]
    [TestCase( "eogf", 1u, 4u, 29u )]
    [TestCase( "eogf", 1u, 5u, 59u )]
    [TestCase( "eogf", 1u, 7u, 47u )]
    [TestCase( "eogf", 1u, 8u, 30u )]
    [TestCase( "eogf", 1u, 9u, 61u )]
    [TestCase( "eogf", 1u, 10u, 59u )]
    [TestCase( "eogf", 1u, 11u, 55u )]
    [TestCase( "eogf", 1u, 13u, 31u )]
    [TestCase( "eogf", 1u, 14u, 62u )]
    [TestCase( "eogf", 1u, 15u, 61u )]
    [TestCase( "eogf", 1u, 16u, 59u )]
    [TestCase( "eogf", 1u, 17u, 55u )]
    public void unsigned_misaligned_6_bit_integer( string payload, uint padding, uint offset, uint expected )
    {
        var parser = new NmeaAisBitVectorParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.GetUnsignedInteger( 6, offset ).ShouldBe( expected );
    }

    // An 8-bit signed integer is read as two's complement, whatever its bit position.
    // Rate of Turn in Common Navigation Block
    [TestCase( "0000000000000000000000000000", 0u, 42u, 0 )]
    [TestCase( "00000000@0000000000000000000", 0u, 42u, 1 )]
    [TestCase( "00000000h0000000000000000000", 0u, 42u, 3 )]
    [TestCase( "0000000100000000000000000000", 0u, 42u, 4 )]
    [TestCase( "00000001@0000000000000000000", 0u, 42u, 5 )]
    [TestCase( "0000000Oh0000000000000000000", 0u, 42u, 127 )]
    [TestCase( "0000000P00000000000000000000", 0u, 42u, -128 )]
    [TestCase( "13oO7800000FH>>S6rpur`P200S<", 0u, 42u, 0 )]   // ais.kystverket.no
    [TestCase( "13oHtV7OhN0=B9bQch;WqnCp0W3h", 0u, 42u, 127 )]   // ais.kystverket.no
    [TestCase( "13mCIp0P00PFnJBSHS1>4?wH2@JB", 0u, 42u, -128 )]   // ais.kystverket.no
    //    e      o      g      f
    // 101101 110111 101111 101110
    // 101101 11  -73
    //  01101 110  110
    //   1101 1101  -35
    //    101 11011  -69
    //     01 110111  119
    //      1 110111 1  -17
    //        110111 10  -34
    // Synthetic examples testing at various bit positions
    [TestCase( "eogf", 1u, 0u, -73 )]
    [TestCase( "eogf", 1u, 1u, 110 )]
    [TestCase( "eogf", 1u, 2u, -35 )]
    [TestCase( "eogf", 1u, 3u, -69 )]
    [TestCase( "eogf", 1u, 4u, 119 )]
    [TestCase( "eogf", 1u, 5u, -17 )]
    [TestCase( "eogf", 1u, 6u, -34 )]
    public void signed_8_bit_integer( string payload, uint padding, uint offset, int expected )
    {
        var parser = new NmeaAisBitVectorParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.GetSignedInteger( 8, offset ).ShouldBe( expected );
    }

    // A single bit is read at any position.
    // Position Accuracy in Common Navigation Block
    [TestCase( "0000", 0u, 0u, 0u )]
    [TestCase( "1000", 0u, 0u, 0u )]
    [TestCase( "1000", 0u, 1u, 0u )]
    [TestCase( "1000", 0u, 2u, 0u )]
    [TestCase( "1000", 0u, 3u, 0u )]
    [TestCase( "1000", 0u, 4u, 0u )]
    [TestCase( "1000", 0u, 5u, 1u )]
    [TestCase( "2000", 0u, 3u, 0u )]
    [TestCase( "2000", 0u, 4u, 1u )]
    [TestCase( "2000", 0u, 5u, 0u )]
    [TestCase( "0000000000P", 0u, 59u, 0u )]
    [TestCase( "0000000000P", 0u, 60u, 1u )]
    [TestCase( "0000000000P", 0u, 61u, 0u )]
    [TestCase( "0000000000000000000000000000", 0u, 60u, 0u )]
    [TestCase( "0000000000P00000000000000000", 0u, 60u, 1u )]
    [TestCase( "13mCIp0P00PFnJBSHS1>4?wH2@JB", 0u, 60u, 1u )]   // ais.kystverket.no
    [TestCase( "13nW5<00000IoPlSbE`:P8EH0534", 0u, 60u, 0u )]   // ais.kystverket.no
    public void single_bit( string payload, uint padding, uint offset, uint expected )
    {
        var parser = new NmeaAisBitVectorParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.GetUnsignedInteger( 1, offset ).ShouldBe( expected );
    }
}
