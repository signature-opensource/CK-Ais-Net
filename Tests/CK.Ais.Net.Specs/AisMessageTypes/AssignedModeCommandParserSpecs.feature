# Copyright (c) Endjin Limited. All rights reserved.

Feature: AssignedModeCommandParserSpecsSteps
    In order process AIS messages from an nm4 file
    As a developer
    I want the NmeaAisAssignedModeCommandParser to be able to parse the payload section of message type 16: Assigned mode command
    
Scenario: Message Type
    When I parse '@02=VgPoD@C43h00' with padding 0 as a Assigned mode command
    Then NmeaAisAssignedModeCommandParser.Type is 16

Scenario: Full message
    When I parse '<payload>' with padding <padding> as a Assigned mode command
    Then NmeaAisAssignedModeCommandParser.Type is <type>
    And NmeaAisAssignedModeCommandParser.RepeatIndicator is <repeatindicator>
    And NmeaAisAssignedModeCommandParser.Mmsi is <mmsi>
    And NmeaAisAssignedModeCommandParser.SpareBits38 is <spare38>
    And NmeaAisAssignedModeCommandParser.DestinationMmsiA is <destinationa>
    And NmeaAisAssignedModeCommandParser.OffsetA is <offseta>
    And NmeaAisAssignedModeCommandParser.IncrementA is <incrementa>
    And NmeaAisAssignedModeCommandParser.DestinationMmsiB is <destinationb>
    And NmeaAisAssignedModeCommandParser.OffsetB is <offsetb>
    And NmeaAisAssignedModeCommandParser.IncrementB is <incrementb>
    And NmeaAisAssignedModeCommandParser.SpareBitsAtEnd is <spareend>

    Examples:
    | payload                  | padding | type | repeatindicator | mmsi      | spare38 | destinationa | offseta | incrementa | destinationb | offsetb | incrementb | spareend |
    | @02=VgPoD@C43h00         | 0       | 16   | 0               | 2320062   | 0       | 232014129    | 60      | 0          |              |         |            | 0        |
    | @02a4KQD2cFP<P0000000500 | 0       | 16   | 0               | 2770030   | 0       | 352497000    | 200     | 0          | 0            | 20      | 0          |          |
