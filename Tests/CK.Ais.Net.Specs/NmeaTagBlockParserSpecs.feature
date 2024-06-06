# Copyright (c) Endjin Limited. All rights reserved.
#
# Contains data under the Norwegian licence for Open Government data (NLOD) distributed by
# the Norwegian Costal Administration - https://ais.kystverket.no/
# The license can be found at https://data.norge.no/nlod/en/2.0
# The lines in this file that contain data from this source are annotated with a comment containing "ais.kystverket.no"
# The NLOD applies only to the data in these annotated lines. The license under which you are using this software
# (either the AGPLv3, or a commercial license) applies to the whole file.

Feature: NmeaTagBlockParserSpecs
	In order to process AIS messages in NMEA files
	As a developer
	I want the NmeaTagBlockParser to be able to parse a NMEA tag block from an NMEA file

Scenario Outline: Unspecified standard tag block
	When I parse '<payload>' with throwWhenTagBlockContainsUnknownFields of false and tagBlockStandard of 0 as a NMEA tag block parser
	Then the Source is <source>
	And the Timestamp is <timestamp>
	And the SentenceGrouping is null

	Examples:
	| payload               | source | timestamp  |
	| s:ASS,c:1706745485*72 | ASS    | 1706745485 |
	| s:AIS,c:1706800491*63 | AIS    | 1706800491 |

Scenario Outline: Unspecified standard tag block with group
	When I parse 'g:1-2-7764,s:AIS,c:1706800480*13' with throwWhenTagBlockContainsUnknownFields of false and tagBlockStandard of 0 as a NMEA tag block parser
	Then the Source is <source>
	And the Timestamp is <timestamp>
	And the SentenceGrouping is <sentence> <total> <groupid>

	Examples:
	| payload                          | source | timestamp  | sentence | total | groupid |
	| g:1-2-7764,s:AIS,c:1706800480*13 | AIS    | 1706800480 | 1        | 2     | 7764    |
	| 1G2:7764,s:AIS,c:1706800480*33   | AIS    | 1706800480 | 1        | 2     | 7764    |

Scenario: IEC tag block single line
	When I parse '<payload>' with throwWhenTagBlockContainsUnknownFields of false and tagBlockStandard of 1 as a NMEA tag block parser
	Then the Source is <source>
	And the Timestamp is <timestamp>
	And the SentenceGrouping is null
	
	Examples:
	| payload               | source | timestamp  |
	| s:ASS,c:1706745485*72 | ASS    | 1706745485 |
	| s:AIS,c:1706800491*63 | AIS    | 1706800491 |

Scenario: IEC tag block single line with group
	When I parse '1G2:7764,s:AIS,c:1706800480*13' with throwWhenTagBlockContainsUnknownFields of false and tagBlockStandard of 1 as a NMEA tag block parser
	Then the Source is AIS
	And the Timestamp is 1706800480
	And the SentenceGrouping is 1 2 7764

Scenario: IEM tag block but Nmea group
	Given the line '\g:1-2-9628,s:AIS,c:1701650788*13\!AIVDM,2,1,2,B,55Mv3A`00001L=SKOG9@tlmV0F2222222222220l189446lgN5j3mDm3kc56,0*4E'
	When I parse the content by message with throwWhenTagBlockContainsUnknownFields of false and tagBlockStandard of 1
	Then the message error report 0 should include the error message 'Tag block sentence grouping should be <int>G<int>:<int>, but first part was not a decimal integer'

Scenario Outline: Nmea tag block single line
	When I parse '<payload>' with throwWhenTagBlockContainsUnknownFields of false and tagBlockStandard of 2 as a NMEA tag block parser
	Then the Source is <source>
	And the Timestamp is <timestamp>
	And the SentenceGrouping is null

	Examples:
	| payload               | source | timestamp  |
	| s:ASS,c:1706745485*72 | ASS    | 1706745485 |
	| s:AIS,c:1706800491*63 | AIS    | 1706800491 |

Scenario: Nmea tag block single line with group
	When I parse 'g:1-2-7764,s:AIS,c:1706800480*13' with throwWhenTagBlockContainsUnknownFields of false and tagBlockStandard of 2 as a NMEA tag block parser
	Then the Source is AIS
	And the Timestamp is 1706800480
	And the SentenceGrouping is 1 2 7764

Scenario: Nmea tag block but IEC group
	Given the line '\1G2:9628,s:AIS,c:1701650788*13\!AIVDM,2,1,2,B,55Mv3A`00001L=SKOG9@tlmV0F2222222222220l189446lgN5j3mDm3kc56,0*4E'
	When I parse the content by message with throwWhenTagBlockContainsUnknownFields of false and tagBlockStandard of 2
	Then the message error report 0 should include the error message 'Tag block sentence grouping should be <int>-<int>-<int>, but first part was not a decimal integer'

Scenario: IEC tag block TextString
	When I parse '<payload>' with throwWhenTagBlockContainsUnknownFields of false and tagBlockStandard of 1 as a NMEA tag block parser
  Then the TextString is <text>

  Examples:
  | payload                     | text      |
  | c:1673149953,i:<O>ES</O>*1F | <O>ES</O> |
  | c:1673149955,i:<O>GI</O>*01 | <O>GI</O> |

Scenario: Nmea tag block TextString
	When I parse '<payload>' with throwWhenTagBlockContainsUnknownFields of false and tagBlockStandard of 2 as a NMEA tag block parser
  Then the TextString is <text>

  Examples:
  | payload                     | text      |
  | c:1673149951,t:<O>ES</O>*00 | <O>ES</O> |
  | c:1673149954,t:<O>RO</O>*0E | <O>RO</O> |

Scenario: IEC tag block TextString but Nmea TextString
	Given the line '\c:1673149951,t:<O>ES</O>*00\!AIVDM,2,1,2,B,55Mv3A`00001L=SKOG9@tlmV0F2222222222220l189446lgN5j3mDm3kc56,0*4E'
	When I parse the content by message with throwWhenTagBlockContainsUnknownFields of false and tagBlockStandard of 1
	Then the message error report 0 should include the error message 'Unknown field type in IEC tag block: t'

Scenario: Nmea tag block TextString but IEC TextString
	Given the line '\c:1673149953,i:<O>ES</O>*1F\!AIVDM,2,1,2,B,55Mv3A`00001L=SKOG9@tlmV0F2222222222220l189446lgN5j3mDm3kc56,0*4E'
	When I parse the content by message with throwWhenTagBlockContainsUnknownFields of false and tagBlockStandard of 2
	Then the message error report 0 should include the error message 'Unknown field type in Nmea tag block: i'
