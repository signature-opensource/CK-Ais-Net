# Copyright (c) Endjin Limited. All rights reserved.

Feature: BaseStationReportParserSpecs
    In order process AIS messages from an nm4 file
    As a developer
    I want the BaseStationReportParserSpecs to be able to parse the payload section of message type 4: Base Station Report
    
Scenario: Message Type
    When I parse '4028j;iu<JAU80>f7>H0elQ00000' with padding 0 as a Base Station Report
    Then NmeaAisBaseStationReportParser.Type is 4

Scenario: Full message
    When I parse '<payload>' with padding <padding> as a Base Station Report
    Then NmeaAisBaseStationReportParser.Type is <type>
    And NmeaAisBaseStationReportParser.RepeatIndicator is <repeatindicator>
    And NmeaAisBaseStationReportParser.Mmsi is <mmsi>
    And NmeaAisBaseStationReportParser.UtcYear is <utcyear>
    And NmeaAisBaseStationReportParser.UtcMonth is <utcmonth>
    And NmeaAisBaseStationReportParser.UtcDay is <utcday>
    And NmeaAisBaseStationReportParser.UtcHour is <utchour>
    And NmeaAisBaseStationReportParser.UtcMinute is <utcminute>
    And NmeaAisBaseStationReportParser.UtcSecond is <utcsecond>
    And NmeaAisBaseStationReportParser.PositionAccuracy is <positionaccuracy>
    And NmeaAisBaseStationReportParser.Longitude10000thMins is <longitude10000thmins>
    And NmeaAisBaseStationReportParser.Latitude10000thMins is <latitude10000thmins>
    And NmeaAisBaseStationReportParser.PositionFixType is <positionfixtype>
    And NmeaAisBaseStationReportParser.TransmissionControlForLongRangeBroadcastMessage is <transmissioncontrolforlongrangebroadcastmessage>
    And NmeaAisBaseStationReportParser.SpareBits139 is <sparebits139>
    And NmeaAisBaseStationReportParser.RaimFlag is <raimflag>
    And NmeaAisBaseStationReportParser.CommunicationState is <communicationstate>
    
    Examples:
    | payload                       | padding | type | repeatindicator | mmsi      | utcyear | utcmonth | utcday | utchour | utcminute | utcsecond | positionaccuracy | longitude10000thmins | latitude10000thmins | positionfixtype | transmissioncontrolforlongrangebroadcastmessage | sparebits139 | raimflag | communicationstate |
    | 4028j;iu<JAU80>f7>H0elQ00000  | 0       | 4    | 0               | 2241071   | 2003    | 1        | 20     | 17      | 37      | 8           | false            | 3.215745             | 41.96259            | 1               | false                                           | 0            | false    | 0                  |
    | 44`Uu;AvJEF`g14>V0DV@MQ00000  | 0       | 4    | 0               | 311000365 | 2022    | 9        | 10     | 22      | 40      | 47          | false            | 14.90464             | 35.99721            | 1               | false                                           | 0            | false    | 0                  |
