# Copyright (c) Endjin Limited. All rights reserved.

Feature: StandardSearchAndRescueAircraftPositionReportParserSpecsSteps
    In order process AIS messages from an nm4 file
    As a developer
    I want the NmeaAisStandardSearchAndRescueAircraftPositionReportParser to be able to parse the payload section of message type 9: Standard Search and Rescue Aircraft Position Report
    
Scenario: Message Type
    When I parse '95M2oQ@41Tr4L4H@eRvQ;2h20000' with padding 0 as a Standard Search and Rescue Aircraft Position Report
    Then NmeaAisStandardSearchAndRescueAircraftPositionReportParser.Type is 9

Scenario: Full message
    When I parse '<payload>' with padding <padding> as a Standard Search and Rescue Aircraft Position Report
    Then NmeaAisStandardSearchAndRescueAircraftPositionReportParser.Type is <type>
    And NmeaAisStandardSearchAndRescueAircraftPositionReportParser.RepeatIndicator is <repeatindicator>
    And NmeaAisStandardSearchAndRescueAircraftPositionReportParser.Mmsi is <mmsi>
    And NmeaAisStandardSearchAndRescueAircraftPositionReportParser.Altitude is <altitude>
    And NmeaAisStandardSearchAndRescueAircraftPositionReportParser.SpeedOverGround is <sog>
    And NmeaAisStandardSearchAndRescueAircraftPositionReportParser.PositionAccuracy is <positionaccuracy>
    And NmeaAisStandardSearchAndRescueAircraftPositionReportParser.Longitude10000thMins is <longitude>
    And NmeaAisStandardSearchAndRescueAircraftPositionReportParser.Latitude10000thMins is <latitude>
    And NmeaAisStandardSearchAndRescueAircraftPositionReportParser.CourseOverGround10thDegrees is <cog>
    And NmeaAisStandardSearchAndRescueAircraftPositionReportParser.TimeStampSecond is <timestamp>
    And NmeaAisStandardSearchAndRescueAircraftPositionReportParser.AltitudeSensor is <altitudesensor>
    And NmeaAisStandardSearchAndRescueAircraftPositionReportParser.SpareBits135 is <spare135>
    And NmeaAisStandardSearchAndRescueAircraftPositionReportParser.DTE is <dte>
    And NmeaAisStandardSearchAndRescueAircraftPositionReportParser.SpareBits143 is <spare143>
    And NmeaAisStandardSearchAndRescueAircraftPositionReportParser.AssignedMode is <assignedmode>
    And NmeaAisStandardSearchAndRescueAircraftPositionReportParser.RaimFlag is <raimflag>
    And NmeaAisStandardSearchAndRescueAircraftPositionReportParser.CommunicationStateSelector is <communicationstateselector>
    And NmeaAisStandardSearchAndRescueAircraftPositionReportParser.CommunicationState is <communicationstate>

    Examples:
    | payload                      | padding | type | repeatindicator | mmsi      | altitude | sog | positionaccuracy | longitude  | latitude | cog | timestamp | altitudesensor | spare135 | dte  | spare143 | assignedmode | raimflag | communicationstateselector | communicationstate |
    | 95M2oQ@41Tr4L4H@eRvQ;2h20000 | 0       | 9    | 0               | 366000005 | 16       | 100 | true             | -49749876  | 17523450 | 300 | 11        | 0              | 0        | true | 0        | false        | false    | 0                          | 0                  |
