# Copyright (c) Endjin Limited. All rights reserved.

Feature: AcknowledgementParserSpecsSteps
    In order process AIS messages from an nm4 file
    As a developer
    I want the NmeaAisAcknowledgementParser to be able to parse the payload section of message type 25: Acknowledgement Message
    
Scenario: Message Type
    When I parse '=8156b@iuus2' with padding 0 as a Acknowledgement Message
    Then NmeaAisAcknowledgementParser.Type is 13

Scenario: Full message
    When I parse '<payload>' with padding <padding> as a Acknowledgement Message
    Then NmeaAisAcknowledgementParser.Type is <type>
    And NmeaAisAcknowledgementParser.RepeatIndicator is <repeatindicator>
    And NmeaAisAcknowledgementParser.Mmsi is <mmsi>
    And NmeaAisAcknowledgementParser.SpareBits38 is <spare38>
    And NmeaAisAcknowledgementParser.DestinationMmsi1 is <destination1>
    And NmeaAisAcknowledgementParser.SequenceNumberMmsi1 is <sequence1>
    And NmeaAisAcknowledgementParser.DestinationMmsi2 is <destination2>
    And NmeaAisAcknowledgementParser.SequenceNumberMmsi2 is <sequence2>
    And NmeaAisAcknowledgementParser.DestinationMmsi3 is <destination3>
    And NmeaAisAcknowledgementParser.SequenceNumberMmsi3 is <sequence3>
    And NmeaAisAcknowledgementParser.DestinationMmsi4 is <destination4>
    And NmeaAisAcknowledgementParser.SequenceNumberMmsi4 is <sequence4>

    Examples:
    | payload                                                                 | padding | type | repeatindicator | mmsi      | spare38 | destination1 | sequence1 | destination2 | sequence2 | destination3 | sequence3 | destination4 | sequence4 |
    | =8156b@iuus2                                                            | 0       | 13   | 0               | 538003113 | 0       | 209582000    | 2         |              |           |              |           |              |           |
    | =5@jof1HMrCPF7NTph                                                      | 0       | 13   | 0               | 353155000 | 0       | 371059000    | 0         | 371059000    | 3         |              |           |              |           |
    | =6D2aSc?SAghBHVwTP0<0;<4:AOwhgs17wpEF:NuHP8oBsti@Rep80klilPCQ830BiH8888 | 0       | 13   | 0               | 423668110 | 2       | 870532860    | 0         | 308441060    | 2         | 196652       | 3         | 17450495     | 3         |
