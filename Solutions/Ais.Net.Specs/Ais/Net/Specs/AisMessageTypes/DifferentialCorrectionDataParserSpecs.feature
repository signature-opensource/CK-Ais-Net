# Copyright (c) Endjin Limited. All rights reserved.

Feature: DifferentialCorrectionDataParserSpecsSteps
    In order process AIS messages from an nm4 file
    As a developer
    I want the NmeaAisDifferentialCorrectionDataParser to be able to parse the payload section of message type 17 differential correction data: Differential Correction Data

Scenario: Full message
    When I parse '<payload>' with padding <padding> as a Differential Correction Data
    Then NmeaAisDifferentialCorrectionDataParser.MessageType is <type>
    And NmeaAisDifferentialCorrectionDataParser.Station is <station>
    And NmeaAisDifferentialCorrectionDataParser.ZCount is <zcount>
    And NmeaAisDifferentialCorrectionDataParser.SequenceNumber is <sequence>
    And NmeaAisDifferentialCorrectionDataParser.DgnssDataWordCount is <n>
    And NmeaAisDifferentialCorrectionDataParser.Health is <health>
    And NmeaAisDifferentialCorrectionDataParser.WriteDgnssDataWord is <data>
    
    Examples:
    | payload                                  | padding | type | station | zcount | sequence | n | health | data                                     |
    | A028nBCt@hbs02GvJ:0`5?ku1ET:wdh69@gvkhH? | 0       | 9    | 510     | 3348   | 0        | 5 | 0      | 1375485,350474,16698374,2427902,13567503 |
    | A028nBCt@hbs02Gvd0H@2gop2ABb             | 0       | 9    | 510     | 5632   | 6        | 2 | 0      | 720376,595114                            |
    | A028jQ02QLfep2H<UhDP5wpk1CdJw@p42:bb     | 0       | 9    | 524     | 4832   | 5        | 4 | 0      | 1572403,342810,16584196,567978           |
