# Copyright (c) Endjin Limited. All rights reserved.

Feature: AddressedSafetyRelatedMessageParserSpecsSteps
    In order process AIS messages from an nm4 file
    As a developer
    I want the AddressedSafetyRelatedMessageParser to be able to parse the payload section of message type 12: Addressed Safety Related Message
    
Scenario: Message Type
    When I parse '<37ood0ne>80P@<CP3853;P=IP19CP41D1P1>4PB5@<I' with padding 0 as a Addressed Safety Related Message
    Then NmeaAisAddressedSafetyRelatedMessageParser.Type is 12

Scenario: Full message
    When I parse '<payload>' with padding <padding> as a Addressed Safety Related Message
    Then NmeaAisAddressedSafetyRelatedMessageParser.Type is <type>
    And NmeaAisAddressedSafetyRelatedMessageParser.RepeatIndicator is <repeatindicator>
    And NmeaAisAddressedSafetyRelatedMessageParser.Mmsi is <mmsi>
    And NmeaAisAddressedSafetyRelatedMessageParser.SequenceNumber is <sequencenumber>
    And NmeaAisAddressedSafetyRelatedMessageParser.DestinationMmsi is <destinationmmsi>
    And NmeaAisAddressedSafetyRelatedMessageParser.Retransmit is <retransmitflag>
    And NmeaAisAddressedSafetyRelatedMessageParser.SpareBit71 is <spare71>
    And NmeaAisAddressedSafetyRelatedMessageParser.SafetyRelatedText is <safetyrelatedtext>

    Examples:
    | payload                                              | padding | type | repeatindicator | mmsi      | sequencenumber | destinationmmsi | retransmitflag | spare71 | safetyrelatedtext                          |
    | <37ood0ne>80P@<CP3853;P=IP19CP41D1P1>4PB5@<I         | 0       | 12   | 0               | 209582000 | 0              | 229456000       | false          | false   | " PLS CHECK MY AIS DATA AND REPLY"         |
    | <815N7n0AABp19CPD5CDP@<CP13;QQ0                      | 0       | 12   | 0               | 538009119 | 1              | 538002734       | false          | false   | "AIS TEST PLS ACK!!@"                      |
    | <9N`1nBD@=8L7??4P41IQP5D1PD?PBFPilrhh<DP`ED3Pckrhha0 | 0       | 12   | 0               | 636092889 | 0              | 621819015       | false          | false   | "GOOD DAY! ETA TO RV 14:00LT (UTC +3:00)@" |