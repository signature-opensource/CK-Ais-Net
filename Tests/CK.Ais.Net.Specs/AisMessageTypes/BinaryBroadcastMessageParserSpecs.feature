# Copyright (c) Endjin Limited. All rights reserved.

Feature: BinaryBroadcastMessageParserSpecsSteps
    In order process AIS messages from an nm4 file
    As a developer
    I want the NmeaAisBinaryBroadcastMessageParser to be able to parse the payload section of message type 8: Binary Broadcast Message
    
Scenario: Message Type
    When I parse '8:U>M;P0G@:?>G1?6600' with padding 0 as a Binary Broadcast Message
    Then NmeaAisBinaryBroadcastMessageParser.Type is 8

Scenario: Full message
    When I parse '<payload>' with padding <padding> as a Binary Broadcast Message
    Then NmeaAisBinaryBroadcastMessageParser.Type is <type>
    And NmeaAisBinaryBroadcastMessageParser.RepeatIndicator is <repeatindicator>
    And NmeaAisBinaryBroadcastMessageParser.Mmsi is <mmsi>
    And NmeaAisBinaryBroadcastMessageParser.SpareBits38 is <spare38>
    And NmeaAisBinaryBroadcastMessageParser.DAC is <dac>
    And NmeaAisBinaryBroadcastMessageParser.FI is <fi>
    And NmeaAisBinaryBroadcastMessageParser.ApplicationDataPadding is <applicationdatapadding>
    And NmeaAisBinaryBroadcastMessageParser.ApplicationData is <applicationdata>

    Examples:
    | payload                                              | padding | type | repeatindicator | mmsi      | spare38 | dac | fi | applicationdatapadding | applicationdata                             |
    | 88uQwV00G@:?>G1?6600                                 | 0       | 8    | 0               | 601391000 | 0       | 1   | 29 | 2                      | @:?>G1?6600                                 |
    | 85PH6giKf;2=fq`s>8IP:=I4EKA@AUU=KCjl;ndkut;MvLM45ebS | 0       | 8    | 0               | 369493695 | 0       | 366 | 56 | 2                      | ;2=fq`s>8IP:=I4EKA@AUU=KCjl;ndkut;MvLM45ebS |
    | 84eJBS0j2d<<<<<<<0HPOg`50000                         | 0       | 8    | 0               | 316052108 | 0       | 200 | 10 | 2                      | d<<<<<<<0HPOg`50000                         |
