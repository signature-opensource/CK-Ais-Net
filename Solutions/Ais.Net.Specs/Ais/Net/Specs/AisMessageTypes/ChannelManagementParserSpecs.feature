# Copyright (c) Endjin Limited. All rights reserved.

Feature: ChannelManagementParserSpecsSteps
    In order process AIS messages from an nm4 file
    As a developer
    I want the NmeaAisChannelManagementParser to be able to parse the payload section of message type 22: Channel management
    
Scenario: Message Type
    When I parse 'F028n@R2N2P3D73EB6`>6bT20000' with padding 0 as a Channel management
    Then NmeaAisChannelManagementParser.Type is 22

Scenario: Full message
    When I parse '<payload>' with padding <padding> as a Channel management
    Then NmeaAisChannelManagementParser.Type is <type>
    And NmeaAisChannelManagementParser.RepeatIndicator is <repeatindicator>
    And NmeaAisChannelManagementParser.Mmsi is <mmsi>
    And NmeaAisChannelManagementParser.SpareBits38 is <spare38>
    And NmeaAisChannelManagementParser.ChannelA is <channela>
    And NmeaAisChannelManagementParser.ChannelB is <channelb>
    And NmeaAisChannelManagementParser.TxRxMode is <txrx>
    And NmeaAisChannelManagementParser.LowPower is <power>
    And NmeaAisChannelManagementParser.Longitude10thMins1 is <longitude1>
    And NmeaAisChannelManagementParser.Latitude10thMins1 is <latitude1>
    And NmeaAisChannelManagementParser.Longitude10thMins2 is <longitude2>
    And NmeaAisChannelManagementParser.Latitude10thMins2 is <latitude2>
    And NmeaAisChannelManagementParser.MessageIndicator is <indicator>
    And NmeaAisChannelManagementParser.ChannelABandwidth is <bandwitha>
    And NmeaAisChannelManagementParser.ChannelBBandwidth is <bandwithb>
    And NmeaAisChannelManagementParser.TransitionalZoneSize is <zonesize>
    And NmeaAisChannelManagementParser.SpareBits145 is <spare145>

    Examples:
    | payload                      | padding | type | repeatindicator | mmsi      | spare38 | channela | channelb | txrx | power | longitude1 | latitude1 | longitude2 | latitude2 | indicator | bandwitha | bandwithb | zonesize | spare145 |
    | F028n@R2N2P3D73EB6`>6bT20000 | 0       | 22   | 0               | 2242114   | 0       | 2087     | 2088     | 0    | false | 108600     | 54600     | 108600     | 54600     | 0         | false     | false     | 4        | 0        |
    | FM5293Ppsrh1S?SmKP0>0BOkJ0t0 | 0       | 22   | 1               | 877693198 | 0       | 910      | 4012     | 0    | false | 50812      | 62830     | 56         | 2367      | 1         | false     | false     | 6        | 6819584  |
    