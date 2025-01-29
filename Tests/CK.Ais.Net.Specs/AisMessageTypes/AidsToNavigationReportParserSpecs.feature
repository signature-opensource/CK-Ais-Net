# Copyright (c) Endjin Limited. All rights reserved.

Feature: AidsToNavigationReportParserSpecsSteps
    In order process AIS messages from an nm4 file
    As a developer
    I want the NmeaAisAidsToNavigationReportParser to be able to parse the payload section of message type 21: Aids to Navigation Report
    
Scenario: Message Type
    When I parse 'Evlt<Cf50QUaWW@97QUP0000000D8U=0r5W0P00003jP10' with padding 0 as a Aids to Navigation Report
    Then NmeaAisAidsToNavigationReportParser.Type is 21

Scenario: Full message
    When I parse '<payload>' with padding <padding> as a Aids to Navigation Report
    Then NmeaAisAidsToNavigationReportParser.Type is <type>
    And NmeaAisAidsToNavigationReportParser.RepeatIndicator is <repeatindicator>
    And NmeaAisAidsToNavigationReportParser.Mmsi is <mmsi>
    And NmeaAisAidsToNavigationReportParser.AidsToNavigationType is <atontype>
    And NmeaAisAidsToNavigationReportParser.NameOfAidsToNavigation is <nameaton>
    And NmeaAisAidsToNavigationReportParser.PositionAccuracy is <accuracy>
    And NmeaAisAidsToNavigationReportParser.Longitude10000thMins is <longitude>
    And NmeaAisAidsToNavigationReportParser.Latitude10000thMins is <latitude>
    And NmeaAisAidsToNavigationReportParser.ReferenceForPositionA is <positiona>
    And NmeaAisAidsToNavigationReportParser.ReferenceForPositionB is <positionb>
    And NmeaAisAidsToNavigationReportParser.ReferenceForPositionC is <positionc>
    And NmeaAisAidsToNavigationReportParser.ReferenceForPositionD is <positiond>
    And NmeaAisAidsToNavigationReportParser.EpfdFixType is <epfdfixtype>
    And NmeaAisAidsToNavigationReportParser.TimeStampSecond is <timestamp>
    And NmeaAisAidsToNavigationReportParser.OffPositionIndicator is <offposition>
    And NmeaAisAidsToNavigationReportParser.AtoNStatus is <atonstatus>
    And NmeaAisAidsToNavigationReportParser.RaimFlag is <raimflag>
    And NmeaAisAidsToNavigationReportParser.VirtualAtoN is <virtualaton>
    And NmeaAisAidsToNavigationReportParser.AssignedMode is <assignedmode>
    And NmeaAisAidsToNavigationReportParser.SpareBit241 is <spare241>
    And NmeaAisAidsToNavigationReportParser.NameOfAidToNavigationExtension is <nameatonext>
    And NmeaAisAidsToNavigationReportParser.SpareBitsAtEnd is <spareend>

    Examples:
    | payload                                                     | padding | type | repeatindicator | mmsi      | atontype | nameaton             | accuracy | longitude | latitude  | positiona | positionb | positionc | positiond | epfdfixtype | timestamp | offposition | atonstatus | raimflag | virtualaton | assignedmode | spare241 | nameatonext   | spareend |
    | Evlt<Cf50QUaWW@97QUP0000000D8U=0r5W0P00003jP10              | 0       | 21   | 3               | 995036238 | 28       | JACKSON ROCK@@@@@@@@ | true     | 69358400  | -12399100 | 0         | 0         | 0         | 0         | 7           | 37        | false       | 0          | false    | true        | false        | false    |               | 0        |
    | E>nRFnO77h0W1T7a9hFh84`2V4W@3AEb1fHgh00003aP11H0DQ@H>@      | 0       | 21   | 0               | 996710105 | 30       | NO ANCHORS - PIPELIN | true     | 857450    | 3617150   | 0         | 0         | 0         | 0         | 7           | 19        | false       | 0          | false    | true        | false        | false    | E AREA 9      | 0        |
    | E>nlfV1`:Rab7h;4Sh<h1WW:@:9MbW`ghDbC@10888gh20A`0UAC`4m@iDh | 0       | 21   | 0               | 997011096 | 3        | PUESTO VIG Y CONT TR | true     | -39159249 | -32877414 | 1         | 1         | 1         | 1         | 1           | 31        | true        | 0          | true     | false       | false        | false    | AF BUEN SUCES | 0        |

Scenario: Invalid out of range
    When I parse 'ENjV3u0;4a::PV@0b7WDHlP0000@IH6:@u?S800000I00' with padding 6 as a Aids to Navigation Report
    Then throw an overflow error
