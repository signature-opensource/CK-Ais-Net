# Copyright (c) Endjin Limited. All rights reserved.

Feature: LongRangeBroadcastMessageParserSpecsSteps
    In order process AIS messages from an nm4 file
    As a developer
    I want the NmeaAisLongRangeBroadcastMessageParser to be able to parse the payload section of message type 27: Long-range Automatic Identifcation System Broadcast Message
    
Scenario: Message Type
    When I parse 'KC5E2b@U19PFdLbMuc5=ROv62<7m' with padding 0 as a Long-range Automatic Identifcation System Broadcast Message
    Then NmeaAisLongRangeBroadcastMessageParser.Type is 27

Scenario: Full message
    When I parse '<payload>' with padding <padding> as a Long-range Automatic Identifcation System Broadcast Message
    Then NmeaAisLongRangeBroadcastMessageParser.Type is <type>
    And NmeaAisLongRangeBroadcastMessageParser.RepeatIndicator is <repeatindicator>
    And NmeaAisLongRangeBroadcastMessageParser.Mmsi is <mmsi>
    And NmeaAisLongRangeBroadcastMessageParser.PositionAccuracy is <accuracy>
    And NmeaAisLongRangeBroadcastMessageParser.RaimFlag is <raimflag>
    And NmeaAisLongRangeBroadcastMessageParser.NavigationStatus is <navigationstatus>
    And NmeaAisLongRangeBroadcastMessageParser.Longitude10thMins is <longitude>
    And NmeaAisLongRangeBroadcastMessageParser.Latitude10thMins is <latitude>
    And NmeaAisLongRangeBroadcastMessageParser.SpeedOverGround is <sog>
    And NmeaAisLongRangeBroadcastMessageParser.CourseOverGround is <cog>
    And NmeaAisLongRangeBroadcastMessageParser.PositionLatency is <latency>
    And NmeaAisLongRangeBroadcastMessageParser.SpareBit94 is <spare94>

    Examples:
    | payload                      | padding | type | repeatindicator | mmsi      | accuracy | raimflag | navigationstatus | longitude | latitude | sog | cog | latency | spare94 |
    | KC5E2b@U19PFdLbMuc5=ROv62<7m | 0       | 27   | 1               | 206914217 | false    | false    | 2                | 82214     | 2904     | 57  | 167 | false   | true    |
    | Kp15Li@1D=MeT5T@             | 0       | 27   | 3               | 538008773 | false    | false    | 0                | 21557     | -18744   | 11  | 68  | false   | false   |
    | Kk=>UB03fbh:R`7d             | 0       | 27   | 3               | 215197000 | false    | false    | 0                | 61099     | 1349     | 16  | 123 | false   | false   |
    