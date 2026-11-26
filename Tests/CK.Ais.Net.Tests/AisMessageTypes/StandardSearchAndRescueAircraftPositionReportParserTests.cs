// <copyright file="StandardSearchAndRescueAircraftPositionReportParserTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using NUnit.Framework;
using Shouldly;
using System.Text;

namespace Ais.Net.Tests.AisMessageTypes;

// NmeaAisStandardSearchAndRescueAircraftPositionReportParser decodes the payload of message type 9
// (Standard SAR Aircraft Position Report): altitude, speed, position, course, time stamp and communication state.
[TestFixture]
public class StandardSearchAndRescueAircraftPositionReportParserTests
{
    // The first 6 bits carry the message type (9).
    [Test]
    public void MessageType_is_decoded()
    {
        var parser = new NmeaAisStandardSearchAndRescueAircraftPositionReportParser( Encoding.ASCII.GetBytes( "95M2oQ@41Tr4L4H@eRvQ;2h20000" ), 0 );
        parser.MessageType.ShouldBe( MessageType.StandardSARAircraftPositionReport );
    }

    // Every field of a real message.
    [TestCase( "95M2oQ@41Tr4L4H@eRvQ;2h20000", 0u, MessageType.StandardSARAircraftPositionReport, 0u, 366000005u, 16u, 100u, true, -49749876, 17523450, 300u, 11u, AltitudeSensor.Gnss, 0u, true, 0u, false, false, CommunicationStateSelector.Sotdma, 0u )]
    public void Full_message_is_decoded( string payload,
                                         uint padding,
                                         MessageType type,
                                         uint repeatIndicator,
                                         uint mmsi,
                                         uint altitude,
                                         uint sog,
                                         bool positionAccuracy,
                                         int longitude,
                                         int latitude,
                                         uint cog,
                                         uint timestamp,
                                         AltitudeSensor altitudeSensor,
                                         uint spare135,
                                         bool dte,
                                         uint spare143,
                                         bool assignedMode,
                                         bool raimFlag,
                                         CommunicationStateSelector communicationStateSelector,
                                         uint communicationState )
    {
        var parser = new NmeaAisStandardSearchAndRescueAircraftPositionReportParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.MessageType.ShouldBe( type );
        parser.RepeatIndicator.ShouldBe( repeatIndicator );
        parser.Mmsi.ShouldBe( mmsi );
        parser.Altitude.ShouldBe( altitude );
        parser.SpeedOverGround.ShouldBe( sog );
        parser.PositionAccuracy.ShouldBe( positionAccuracy );
        parser.Longitude10000thMins.ShouldBe( longitude );
        parser.Latitude10000thMins.ShouldBe( latitude );
        parser.CourseOverGround10thDegrees.ShouldBe( cog );
        parser.TimeStampSecond.ShouldBe( timestamp );
        parser.AltitudeSensor.ShouldBe( altitudeSensor );
        parser.SpareBits135.ShouldBe( spare135 );
        parser.DTE.ShouldBe( dte );
        parser.SpareBits143.ShouldBe( spare143 );
        parser.AssignedMode.ShouldBe( assignedMode );
        parser.RaimFlag.ShouldBe( raimFlag );
        parser.CommunicationStateSelector.ShouldBe( communicationStateSelector );
        parser.CommunicationState.ShouldBe( communicationState );
    }
}
