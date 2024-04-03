# Copyright (c) Endjin Limited. All rights reserved.

Feature: SafetyRelatedBroadcastParserSpecsSteps
    In order process AIS messages from an nm4 file
    As a developer
    I want the NmeaAisSafetyRelatedBroadcastParser to be able to parse the payload section of message type 14: Addressed Safety Related Message
    
Scenario: Message Type
    When I parse '>>l:N8hp5HTL5@Ttp4j1L58pTpN2b`9Eb37:bb0U>0tHJ10u<U@Ttp0' with padding 0 as a Safety Related Broadcast
    Then NmeaAisSafetyRelatedBroadcastParser.Type is 14

Scenario: Full message
    When I parse '<payload>' with padding <padding> as a Safety Related Broadcast
    Then NmeaAisSafetyRelatedBroadcastParser.Type is <type>
    And NmeaAisSafetyRelatedBroadcastParser.RepeatIndicator is <repeatindicator>
    And NmeaAisSafetyRelatedBroadcastParser.Mmsi is <mmsi>
    And NmeaAisSafetyRelatedBroadcastParser.SpareBit38 is <spare38>
    And NmeaAisSafetyRelatedBroadcastParser.SafetyRelatedText is <safetyrelatedtext>

    Examples:
    | payload                                                | padding | type | repeatindicator | mmsi      | spare38 | safetyrelatedtext                                 |
    | >3@pJu04U>10ib04<f1@qR1<4HF1HuT4LF0                    | 0       | 14   | 0               | 219028212 | 0       | "AIS PLZ ACK TNX SAFE VOYAGE "                    |
    | >>m>cDPp5HTL5@Ttp4j1L58pTpN2ba0EA8tr1<8nbb0U>0H4ThTpL0 | 0       | 14   | 0               | 995339090 | 0       | "NAVIGATIONAL WARNING **PETRON SBM** IS FAILING@" |
    | >8HweV1@E=@m<N0                                        | 0       | 14   | 0               | 563080600 | 0       | "TESTMSG "                                        |