# Copyright (c) Endjin Limited. All rights reserved.

Feature: AddressedBinaryMessageParserSpecsSteps
    In order process AIS messages from an nm4 file
    As a developer
    I want the NmeaAisAddressedBinaryMessageParser to be able to parse the payload section of message type 6: Addressed Binary Message
    
Scenario: Message Type
    When I parse '6>kKL=000000>d`u06?D000' with padding 0 as a Addressed Binary Message
    Then NmeaAisAddressedBinaryMessageParser.Type is 6

Scenario: Full message
    When I parse '<payload>' with padding <padding> as a Addressed Binary Message
    Then NmeaAisAddressedBinaryMessageParser.Type is <type>
    And NmeaAisAddressedBinaryMessageParser.RepeatIndicator is <repeatindicator>
    And NmeaAisAddressedBinaryMessageParser.Mmsi is <mmsi>
    And NmeaAisAddressedBinaryMessageParser.SequenceNumber is <sequencenumber>
    And NmeaAisAddressedBinaryMessageParser.DestinationMmsi is <destinationmmsi>
    And NmeaAisAddressedBinaryMessageParser.Retransmit is <retransmit>
    And NmeaAisAddressedBinaryMessageParser.SpareBit71 is <spare71>
    And NmeaAisAddressedBinaryMessageParser.DAC is <dac>
    And NmeaAisAddressedBinaryMessageParser.FI is <fi>
    And NmeaAisAddressedBinaryMessageParser.ApplicationDataPadding is <applicationdatapadding>

    Examples:
    | payload                                              | padding | type | repeatindicator | mmsi      | sequencenumber | destinationmmsi | retransmit | spare71 | dac | fi | applicationdatapadding |
    | 6>oHhp000000>da30000@00                              | 0       | 6    | 0               | 997601504 | 0              | 0               | false      | false   | 235 | 10 | 4                      |
    | 6>oNma0JOJtL078NuS1`s6mQ0vCv048@0002P1401`0000000000 | 0       | 6    | 0               | 997701028 | 0              | 111111111       | false      | false   | 1   | 50 | 4                      |
    | 6>p?paQREindIt4<                                     | 0       | 6    | 0               | 998504614 | 0              | 412469099       | false      | false   | 415 | 1  | 4                      |
