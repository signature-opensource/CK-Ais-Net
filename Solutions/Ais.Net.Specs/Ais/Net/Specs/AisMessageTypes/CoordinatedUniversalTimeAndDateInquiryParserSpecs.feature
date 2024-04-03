# Copyright (c) Endjin Limited. All rights reserved.

Feature: CoordinatedUniversalTimeAndDateInquiryParserSpecs
    In order process AIS messages from an nm4 file
    As a developer
    I want the CoordinatedUniversalTimeAndDateInquiryParser to be able to parse the payload section of message type 10: Coordinated Universal Time and Date Inquiry
    
Scenario: Message Type
    When I parse ':815J6hrsEKP' with padding 0 as a Coordinated Universal Time and Date Inquiry
    Then NmeaAisCoordinatedUniversalTimeAndDateInquiryParser.Type is 10
    
Scenario Outline: Repeat Indicator
    When I parse '<payload>' with padding <padding> as a Coordinated Universal Time and Date Inquiry
    Then NmeaAisCoordinatedUniversalTimeAndDateInquiryParser.RepeatIndicator is <repeatCount>

    Examples:
    | payload      | padding | repeatCount |
    | :00000000000 | 0       | 0           |
    | :@0000000000 | 0       | 1           |
    | :P0000000000 | 0       | 2           |
    | :h0000000000 | 0       | 3           |

Scenario Outline: MMSI
    When I parse '<payload>' with padding <padding> as a Coordinated Universal Time and Date Inquiry
    Then NmeaAisCoordinatedUniversalTimeAndDateInquiryParser.Mmsi is <mmsi>

    Examples:
    | payload      | padding | mmsi      |
    | K00000000000 | 0       | 0         |
    | K00000@00000 | 0       | 1         |
    | K00000P00000 | 0       | 2         |

Scenario: Full message
    When I parse '<payload>' with padding <padding> as a Coordinated Universal Time and Date Inquiry
    Then NmeaAisCoordinatedUniversalTimeAndDateInquiryParser.Type is <type>
    And NmeaAisCoordinatedUniversalTimeAndDateInquiryParser.RepeatIndicator is <repeatindicator>
    And NmeaAisCoordinatedUniversalTimeAndDateInquiryParser.Mmsi is <mmsi>
    And NmeaAisCoordinatedUniversalTimeAndDateInquiryParser.SpareBits38 is <spare38>
    And NmeaAisCoordinatedUniversalTimeAndDateInquiryParser.DestinationMmsi is <destinationmmsi>
    And NmeaAisCoordinatedUniversalTimeAndDateInquiryParser.SpareBits70 is <spare70>

    Examples:
    | payload      | padding | type | repeatindicator | mmsi      | spare38 | destinationmmsi | spare70 |
    | :815J6hrsEKP | 0       | 10   | 0               | 538008091 | 0       | 247158200       | 0       |
    | :9NWsc1sgBDL | 0       | 10   | 0               | 636091308 | 0       | 518998343       | 0       |
    | :5Tjij10WiuP | 0       | 10   | 0               | 374125000 | 0       | 271042520       | 0       |
