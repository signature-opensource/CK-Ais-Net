# Copyright (c) Endjin Limited. All rights reserved.

Feature: InterrogationSpecsSteps
    In order process AIS messages from an nm4 file
    As a developer
    I want the NmeaAisInterrogationParser to be able to parse the payload section of message type 15: Interrogation
    
Scenario: Message Type
    When I parse '?913QK18Uf;0D00' with padding 0 as an Interrogation
    Then NmeaAisInterrogationParser.Type is 15

Scenario: Full message
    When I parse '<payload>' with padding <padding> as an Interrogation
    Then NmeaAisInterrogationParser.Type is <type>
    And NmeaAisInterrogationParser.RepeatIndicator is <repeatindicator>
    And NmeaAisInterrogationParser.Mmsi is <mmsi>
    And NmeaAisInterrogationParser.SpareBits38 is <spare38>
    And NmeaAisInterrogationParser.DestinationMmsi1 is <destinationmmsi1>
    And NmeaAisInterrogationParser.MessageType11 is <messagetype11>
    And NmeaAisInterrogationParser.SlotOffset11 is <slotoffset11>
    And NmeaAisInterrogationParser.SpareBits88 is <spare88>
    And NmeaAisInterrogationParser.MessageType12 is <messagetype12>
    And NmeaAisInterrogationParser.SlotOffset12 is <slotoffset12>
    And NmeaAisInterrogationParser.SpareBits108 is <spare108>
    And NmeaAisInterrogationParser.DestinationMmsi2 is <destinationmmsi2>
    And NmeaAisInterrogationParser.MessageType21 is <mmessagetype21>
    And NmeaAisInterrogationParser.SlotOffset21 is <slotoffset21>
    And NmeaAisInterrogationParser.SpareBits158 is <spare158>

    Examples:
    | payload             | padding | type | repeatindicator | mmsi      | spare38 | destinationmmsi1 | messagetype11 | slotoffset11 | spare88 | messagetype12 | slotoffset12 | spare108 | destinationmmsi2 | mmessagetype21 | slotoffset21 | spare158 |
    | ?913QK18Uf;0D00     | 0       | 15   | 0               | 605086060 | 0       | 304462000        | 5             | 0            | 0       |               |              |          |                  |                |              |          |
    | ?1b60U0kNVOP<005000 | 0       | 15   | 0               | 111247508 | 0       | 215915000        | 3             | 0            | 0       | 5             | 0            | 0        |                  |                |              |          |
    | ?77>7w1iprE@D00     | 0       | 15   | 0               | 477333500 | 0       | 477686100        | 5             | 0            | 0       |               |              |          |                  |                |              |          |
