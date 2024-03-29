# Copyright (c) Endjin Limited. All rights reserved.

Feature: SingleSlotBinaryParserSpecsSteps
    In order process AIS messages from an nm4 file
    As a developer
    I want the NmeaAisSingleSlotBinaryParser to be able to parse the payload section of message type 25: AIS Single Slot Binary Message
    
Scenario: Message Type
    When I parse 'I>M4ej0' with padding 0 as a Single Slot Binary Message
    Then NmeaAisSingleSlotBinaryParser.Type is 25

Scenario: Full message
    When I parse '<payload>' with padding <padding> as a Single Slot Binary Message
    Then NmeaAisSingleSlotBinaryParser.Type is <type>
    And NmeaAisSingleSlotBinaryParser.RepeatIndicator is <repeatindicator>
    And NmeaAisSingleSlotBinaryParser.Mmsi is <mmsi>
    And NmeaAisSingleSlotBinaryParser.DestinationIndicator is <destindicator>
    And NmeaAisSingleSlotBinaryParser.BinaryDataFlag is <binaryflag>
    And NmeaAisSingleSlotBinaryParser.DestinationMmsi is <destination>
    And NmeaAisSingleSlotBinaryParser.SpareBits70 is <spare70>
    And NmeaAisSingleSlotBinaryParser.DAC is <dac>
    And NmeaAisSingleSlotBinaryParser.FI is <fi>
    And NmeaAisSingleSlotBinaryParser.ApplicationDataPadding is <applicationdatapadding>
    And NmeaAisSingleSlotBinaryParser.ApplicationData is <applicationdata>

    Examples:
    | payload | padding | type | repeatindicator | mmsi      | destindicator | binaryflag | destination | spare70 | dac | fi | applicationdatapadding | applicationdata |
    | I>M4ej0 | 0       | 25   | 0               | 970010056 | 0             | false      |             |         |     |    | 4                      | 0               |
    | I8;@OB0 | 0       | 25   | 0               | 548675400 | 0             | false      |             |         |     |    | 4                      | 0               |
