# Copyright (c) Endjin Limited. All rights reserved.

Feature: DataLinkManagementMessageParserSpecsSteps
    In order process AIS messages from an nm4 file
    As a developer
    I want the NmeaAisDataLinkManagementMessageParser to be able to parse the payload section of message type 20: Data link management message
    
Scenario: Message Type
    When I parse 'D02;bMhRl@fq6DA6DB0i6D0' with padding 0 as a Data link management message
    Then NmeaAisDataLinkManagementMessageParser.Type is 20

Scenario: Full message
    When I parse '<payload>' with padding <padding> as a Data link management message
    Then NmeaAisDataLinkManagementMessageParser.Type is <type>
    And NmeaAisDataLinkManagementMessageParser.RepeatIndicator is <repeatindicator>
    And NmeaAisDataLinkManagementMessageParser.Mmsi is <mmsi>
    And NmeaAisDataLinkManagementMessageParser.SpareBits38 is <spare38>
    And NmeaAisDataLinkManagementMessageParser.Offset1 is <offset1>
    And NmeaAisDataLinkManagementMessageParser.SlotNumber1 is <slot1>
    And NmeaAisDataLinkManagementMessageParser.Timeout1 is <timeout1>
    And NmeaAisDataLinkManagementMessageParser.Increment1 is <increment1>
    And NmeaAisDataLinkManagementMessageParser.Offset2 is <offset2>
    And NmeaAisDataLinkManagementMessageParser.SlotNumber2 is <slot2>
    And NmeaAisDataLinkManagementMessageParser.Timeout2 is <timeout2>
    And NmeaAisDataLinkManagementMessageParser.Increment2 is <increment2>
    And NmeaAisDataLinkManagementMessageParser.Offset3 is <offset3>
    And NmeaAisDataLinkManagementMessageParser.SlotNumber3 is <slot3>
    And NmeaAisDataLinkManagementMessageParser.Timeout3 is <timeout3>
    And NmeaAisDataLinkManagementMessageParser.Increment3 is <increment3>
    And NmeaAisDataLinkManagementMessageParser.Offset4 is <offset4>
    And NmeaAisDataLinkManagementMessageParser.SlotNumber4 is <slot4>
    And NmeaAisDataLinkManagementMessageParser.Timeout4 is <timeout4>
    And NmeaAisDataLinkManagementMessageParser.Increment4 is <increment4>
    And NmeaAisDataLinkManagementMessageParser.SpareBitsAtEnd is <spareend>

    Examples:
    | payload                 | padding | type | repeatindicator | mmsi      | spare38 | offset1 | slot1 | timeout1 | increment1 | offset2 | slot2 | timeout2 | increment2 | offset3 | slot3 | timeout3 | increment3 | offset4 | slot4 | timeout4 | increment4 | spareend |
    | D02;bMhRl@fq6DA6DB0i6D0 | 0       | 20   | 0               | 2288247   | 0       | 557     | 1     | 0        | 750        | 1125    | 1     | 0        | 1125       | 288     | 3     | 0        | 1125       |         |       |          |            |          |
    | D02;bMR0tLfp00M6EpDu6D0 | 0       | 20   | 0               | 2288246   | 0       | 2063    | 1     | 6        | 750        | 0       | 1     | 6        | 1125       | 1925    | 3     | 6        | 1125       |         |       |          |            |          |
