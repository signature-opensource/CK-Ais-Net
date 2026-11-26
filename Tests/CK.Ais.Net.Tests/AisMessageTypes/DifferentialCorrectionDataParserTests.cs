// <copyright file="DifferentialCorrectionDataParserTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using NUnit.Framework;
using Shouldly;
using System.Text;

namespace Ais.Net.Tests.AisMessageTypes;

// NmeaAisDifferentialCorrectionDataParser decodes the DGNSS correction data embedded in a message type 17
// (GNSS Broadcast Binary Message): the 2-word header (message type, station, Z-count, sequence, word count,
// health) and the data words. It is built from the DifferentialCorrectionData slice of the type 17 parser.
[TestFixture]
public class DifferentialCorrectionDataParserTests
{
    // Every field of real corrections. The message type here is the type of the correction message (9),
    // not an AIS message type: MessageType has no dedicated member, hence the cast.
    [TestCase( "A028nBCt@hbs02GvJ:0`5?ku1ET:wdh69@gvkhH?", 0u, (MessageType)9, 510u, 3348u, 0u, 5u, 0u, new uint[] { 1375485, 350474, 16698374, 2427902, 13567503 } )]
    [TestCase( "A028nBCt@hbs02Gvd0H@2gop2ABb", 0u, (MessageType)9, 510u, 5632u, 6u, 2u, 0u, new uint[] { 720376, 595114 } )]
    [TestCase( "A028jQ02QLfep2H<UhDP5wpk1CdJw@p42:bb", 0u, (MessageType)9, 524u, 4832u, 5u, 4u, 0u, new uint[] { 1572403, 342810, 16584196, 567978 } )]
    public void Full_message_is_decoded( string payload,
                                         uint padding,
                                         MessageType type,
                                         uint station,
                                         uint zCount,
                                         uint sequence,
                                         uint wordCount,
                                         uint health,
                                         uint[] dataWords )
    {
        var message = new NmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser( Encoding.ASCII.GetBytes( payload ), padding );
        var parser = new NmeaAisDifferentialCorrectionDataParser( message.DifferentialCorrectionData, message.DifferentialCorrectionDataPaddingBefore, padding );
        parser.MessageType.ShouldBe( type );
        parser.Station.ShouldBe( station );
        parser.ZCount.ShouldBe( zCount );
        parser.SequenceNumber.ShouldBe( sequence );
        parser.DgnssDataWordCount.ShouldBe( wordCount );
        parser.Health.ShouldBe( health );
        var words = new uint[parser.DgnssDataWordCount];
        parser.WriteDgnssDataWord( words );
        words.ShouldBe( dataWords );
    }
}
