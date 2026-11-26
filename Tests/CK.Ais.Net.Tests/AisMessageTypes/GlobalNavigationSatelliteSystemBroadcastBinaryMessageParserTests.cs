// <copyright file="GlobalNavigationSatelliteSystemBroadcastBinaryMessageParserTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using NUnit.Framework;
using Shouldly;
using System.Text;

namespace Ais.Net.Tests.AisMessageTypes;

// NmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser decodes the payload of message type 17
// (GNSS Broadcast Binary Message): header, reference position and the raw (still 6-bit ASCII armored)
// differential correction data.
[TestFixture]
public class GlobalNavigationSatelliteSystemBroadcastBinaryMessageParserTests
{
    // The first 6 bits carry the message type (17).
    [Test]
    public void MessageType_is_decoded()
    {
        var parser = new NmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser( Encoding.ASCII.GetBytes( "A028nBCt@hbs02Gvd0H@2gop2ABb" ), 0 );
        parser.MessageType.ShouldBe( MessageType.DGNSSBroadcastBinaryMessage );
    }

    // Every field of real messages; the correction data is the payload slice that follows bit 80
    // (it starts with 2 bits belonging to the header).
    [TestCase( "A028nBCt@hbs02GvJ:0`5?ku1ET:wdh69@gvkhH?", 0u, MessageType.DGNSSBroadcastBinaryMessage, 0u, 2242121u, 0u, -3828, 21976, 0u, 2u, "2GvJ:0`5?ku1ET:wdh69@gvkhH?" )]
    [TestCase( "A028nBCt@hbs02Gvd0H@2gop2ABb", 0u, MessageType.DGNSSBroadcastBinaryMessage, 0u, 2242121u, 0u, -3828, 21976, 0u, 2u, "2Gvd0H@2gop2ABb" )]
    [TestCase( "A028jQ02QLfep2H<UhDP5wpk1CdJw@p42:bb", 0u, MessageType.DGNSSBroadcastBinaryMessage, 0u, 2241156u, 0u, 2583, 23919, 0u, 2u, "2H<UhDP5wpk1CdJw@p42:bb" )]
    public void Full_message_is_decoded( string payload,
                                         uint padding,
                                         MessageType type,
                                         uint repeatIndicator,
                                         uint mmsi,
                                         uint spare38,
                                         int longitude,
                                         int latitude,
                                         uint spare75,
                                         uint differentialPadding,
                                         string differential )
    {
        var parser = new NmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.MessageType.ShouldBe( type );
        parser.RepeatIndicator.ShouldBe( repeatIndicator );
        parser.Mmsi.ShouldBe( mmsi );
        parser.SpareBits38.ShouldBe( spare38 );
        parser.Longitude10thMins.ShouldBe( longitude );
        parser.Latitude10thMins.ShouldBe( latitude );
        parser.SpareBits75.ShouldBe( spare75 );
        parser.DifferentialCorrectionDataPaddingBefore.ShouldBe( differentialPadding );
        Encoding.ASCII.GetString( parser.DifferentialCorrectionData ).ShouldBe( differential );
    }
}
