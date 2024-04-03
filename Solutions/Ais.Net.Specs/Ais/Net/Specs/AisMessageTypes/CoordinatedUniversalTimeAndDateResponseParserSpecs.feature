# Copyright (c) Endjin Limited. All rights reserved.

Feature: CoordinatedUniversalTimeAndDateResponseParserSpecs
    In order process AIS messages from an nm4 file
    As a developer
    I want the CoordinatedUniversalTimeAndDateResponseParser to be able to parse the payload section of message type 4: Coordinated Universal Time and Date Response 
    
Scenario: Message Type
    When I parse ';028j;iu<JAU80>f7>H0elQ00000' with padding 0 as a Coordinated Universal Time and Date Response 
    Then NmeaAisCoordinatedUniversalTimeAndDateResponseParser.Type is 11

Scenario: Full message
    When I parse '<payload>' with padding <padding> as a Coordinated Universal Time and Date Response 
    Then NmeaAisCoordinatedUniversalTimeAndDateResponseParser.Type is <type>
    And NmeaAisCoordinatedUniversalTimeAndDateResponseParser.RepeatIndicator is <repeatindicator>
    And NmeaAisCoordinatedUniversalTimeAndDateResponseParser.Mmsi is <mmsi>
    And NmeaAisCoordinatedUniversalTimeAndDateResponseParser.UtcYear is <utcyear>
    And NmeaAisCoordinatedUniversalTimeAndDateResponseParser.UtcMonth is <utcmonth>
    And NmeaAisCoordinatedUniversalTimeAndDateResponseParser.UtcDay is <utcday>
    And NmeaAisCoordinatedUniversalTimeAndDateResponseParser.UtcHour is <utchour>
    And NmeaAisCoordinatedUniversalTimeAndDateResponseParser.UtcMinute is <utcminute>
    And NmeaAisCoordinatedUniversalTimeAndDateResponseParser.UtcSecond is <utcsecond>
    And NmeaAisCoordinatedUniversalTimeAndDateResponseParser.PositionAccuracy is <positionaccuracy>
    And NmeaAisCoordinatedUniversalTimeAndDateResponseParser.Longitude10000thMins is <longitude10000thmins>
    And NmeaAisCoordinatedUniversalTimeAndDateResponseParser.Latitude10000thMins is <latitude10000thmins>
    And NmeaAisCoordinatedUniversalTimeAndDateResponseParser.PositionFixType is <positionfixtype>
    And NmeaAisCoordinatedUniversalTimeAndDateResponseParser.TransmissionControlForLongRangeBroadcastMessage is <transmissioncontrolforlongrangebroadcastmessage>
    And NmeaAisCoordinatedUniversalTimeAndDateResponseParser.SpareBits139 is <sparebits139>
    And NmeaAisCoordinatedUniversalTimeAndDateResponseParser.RaimFlag is <raimflag>
    And NmeaAisCoordinatedUniversalTimeAndDateResponseParser.CommunicationState is <communicationstate>
    
    Examples:
    | payload                       | padding | type | repeatindicator | mmsi      | utcyear | utcmonth | utcday | utchour | utcminute | utcsecond | positionaccuracy | longitude10000thmins | latitude10000thmins | positionfixtype | transmissioncontrolforlongrangebroadcastmessage | sparebits139 | raimflag | communicationstate |
    | ;028j;iu<JAU80>f7>H0elQ00000  | 0       | 11   | 0               | 2241071   | 2003    | 1        | 20     | 17      | 37        | 8         | false            | 3.215745             | 41.96259            | 1               | false                                           | 0            | false    | 0                  |
    | ;4`Uu;AvJEF`g14>V0DV@MQ00000  | 0       | 11   | 0               | 311000365 | 2022    | 9        | 10     | 22      | 40        | 47        | false            | 14.90464             | 35.99721            | 1               | false                                           | 0            | false    | 0                  |
