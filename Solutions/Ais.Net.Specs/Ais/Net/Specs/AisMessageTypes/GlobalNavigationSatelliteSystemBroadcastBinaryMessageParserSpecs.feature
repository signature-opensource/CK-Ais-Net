# Copyright (c) Endjin Limited. All rights reserved.

Feature: GlobalNavigationSatelliteSystemBroadcastBinaryMessageParserSpecsSteps
    In order process AIS messages from an nm4 file
    As a developer
    I want the NmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser to be able to parse the payload section of message type 17: Global Navigation-Satellite System Broadcast Binary Message
    
Scenario: Message Type
    When I parse 'A028nBCt@hbs02Gvd0H@2gop2ABb' with padding 0 as a Global Navigation-Satellite System Broadcast Binary Message
    Then NmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser.Type is 17

Scenario: Full message
    When I parse '<payload>' with padding <padding> as a Global Navigation-Satellite System Broadcast Binary Message
    Then NmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser.Type is <type>
    And NmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser.RepeatIndicator is <repeatindicator>
    And NmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser.Mmsi is <mmsi>
    And NmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser.SpareBits38 is <spare38>
    And NmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser.Longitude10thMins is <longitude>
    And NmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser.Latitude10thMins is <latitude>
    And NmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser.SpareBits75 is <spare75>
    And NmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser.DifferentialCorrectionDataPadding is <differentialpadding>
    And NmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser.DifferentialCorrectionData is <differential>
    
    Examples:
    | payload                                  | padding | type | repeatindicator | mmsi    | spare38 | longitude | latitude | spare75 | differentialpadding | differential                |
    | A028nBCt@hbs02GvJ:0`5?ku1ET:wdh69@gvkhH? | 0       | 17   | 0               | 2242121 | 0       | -3828     | 21976    | 0       | 2                   | 2GvJ:0`5?ku1ET:wdh69@gvkhH? |
    | A028nBCt@hbs02Gvd0H@2gop2ABb             | 0       | 17   | 0               | 2242121 | 0       | -3828     | 21976    | 0       | 2                   | 2Gvd0H@2gop2ABb             |
    | A028jQ02QLfep2H<UhDP5wpk1CdJw@p42:bb     | 0       | 17   | 0               | 2241156 | 0       | 2583      | 23919    | 0       | 2                   | 2H<UhDP5wpk1CdJw@p42:bb     |
