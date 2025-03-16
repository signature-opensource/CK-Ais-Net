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
	Then the Source is '<source>'
	And the Timestamp is '<timestamp>'
	And the SentenceGrouping is null

	Examples:
	| payload               | source | timestamp  |
	| s:ASS,c:1706745485*72 | ASS    | 1706745485 |
	| s:AIS,c:1706800491*63 | AIS    | 1706800491 |

Scenario Outline: Unspecified standard tag block with group
	When I parse 'g:1-2-7764,s:AIS,c:1706800480*13' with throwWhenTagBlockContainsUnknownFields of false and tagBlockStandard of 0 as a NMEA tag block parser
	Then the Source is '<source>'
	And the Timestamp is '<timestamp>'
	And the SentenceGrouping is <sentence> <total> <groupid>

	Examples:
	| payload                          | source | timestamp  | sentence | total | groupid |
	| g:1-2-7764,s:AIS,c:1706800480*13 | AIS    | 1706800480 | 1        | 2     | 7764    |
	| 1G2:7764,s:AIS,c:1706800480*33   | AIS    | 1706800480 | 1        | 2     | 7764    |

Scenario: IEC tag block single line
	When I parse '<payload>' with throwWhenTagBlockContainsUnknownFields of false and tagBlockStandard of 1 as a NMEA tag block parser
	Then the Source is '<source>'
	And the Timestamp is '<timestamp>'
	And the SentenceGrouping is null
	
	Examples:
	| payload               | source | timestamp  |
	| s:ASS,c:1706745485*72 | ASS    | 1706745485 |
	| s:AIS,c:1706800491*63 | AIS    | 1706800491 |

Scenario: IEC tag block single line with group
	When I parse '1G2:7764,s:AIS,c:1706800480*13' with throwWhenTagBlockContainsUnknownFields of false and tagBlockStandard of 1 as a NMEA tag block parser
	Then the Source is 'AIS'
	And the Timestamp is '1706800480'
	And the SentenceGrouping is 1 2 7764

Scenario: IEM tag block but Nmea group
	Given the line '\g:1-2-9628,s:AIS,c:1701650788*13\!AIVDM,2,1,2,B,55Mv3A`00001L=SKOG9@tlmV0F2222222222220l189446lgN5j3mDm3kc56,0*4E'
	When I parse the content by message with throwWhenTagBlockContainsUnknownFields of false and tagBlockStandard of 1
	Then the message error report 0 should include the error message 'Tag block sentence grouping should be <int>G<int>:<int>, but first part was not a decimal integer'

Scenario Outline: Nmea tag block single line
	When I parse '<payload>' with throwWhenTagBlockContainsUnknownFields of false and tagBlockStandard of 2 as a NMEA tag block parser
	Then the Source is '<source>'
	And the Timestamp is '<timestamp>'
	And the SentenceGrouping is null

	Examples:
	| payload               | source | timestamp  |
	| s:ASS,c:1706745485*72 | ASS    | 1706745485 |
	| s:AIS,c:1706800491*63 | AIS    | 1706800491 |

Scenario: Nmea tag block single line with group
	When I parse 'g:1-2-7764,s:AIS,c:1706800480*13' with throwWhenTagBlockContainsUnknownFields of false and tagBlockStandard of 2 as a NMEA tag block parser
	Then the Source is 'AIS'
	And the Timestamp is '1706800480'
	And the SentenceGrouping is 1 2 7764

Scenario: Nmea tag block but IEC group
	Given the line '\1G2:9628,s:AIS,c:1701650788*13\!AIVDM,2,1,2,B,55Mv3A`00001L=SKOG9@tlmV0F2222222222220l189446lgN5j3mDm3kc56,0*4E'
	When I parse the content by message with throwWhenTagBlockContainsUnknownFields of false and tagBlockStandard of 2
	Then the message error report 0 should include the error message 'Tag block sentence grouping should be <int>-<int>-<int>, but first part was not a decimal integer'

Scenario: IEC tag block TextString
	When I parse '<payload>' with throwWhenTagBlockContainsUnknownFields of false and tagBlockStandard of 1 as a NMEA tag block parser
  Then the TextString is '<text>'

  Examples:
  | payload                     | text      |
  | c:1673149953,i:<O>ES</O>*1F | <O>ES</O> |
  | c:1673149955,i:<O>GI</O>*01 | <O>GI</O> |

Scenario: Nmea tag block TextString
	When I parse '<payload>' with throwWhenTagBlockContainsUnknownFields of false and tagBlockStandard of 2 as a NMEA tag block parser
  Then the TextString is '<text>'

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

Scenario: Nmea tag block Extra Fields but no parser
  Given the line '\s:KIN1B,c:1716810431,q:mt-pt-ct-st-kt*78\!AIVDM,1,1,,B,100BkthL2fmlG@@iC=w2CQfF0Gh8,0*38'
	When I parse the content by message with throwWhenTagBlockContainsUnknownFields of true and tagBlockStandard of 0
	Then the message error report 0 should include the error message 'Unknown field type: q'

Scenario: Nmea tag block Extra Fields with extra parser
	When I parse '<payload>' with throwWhenTagBlockContainsUnknownFields of true, tagBlockStandard of 0 and extra parser
  Then the extra field parser q value is '<qvalue>'
  And the extra field parser v value is '<vvalue>'

  Examples:
  | payload                                        | qvalue         | vvalue |
  | s:KIN1B,c:1716810431,q:mt-pt-ct-st-kt*78       | mt-pt-ct-st-kt |        |
  | s:KIN1B,c:1716810431,q:mt-pt-ct-st-kt,v:123*28 | mt-pt-ct-st-kt | 123    |

Scenario: Allow tag block empty fields
    When I parse '<payload>' with allowTagBlockEmptyFields of true and throwWhenTagBlockContainsUnknownFields of false
    Then there are no error
    And the Source is empty
    And the Timestamp is null
    And the TextString is empty
    And the SentenceGrouping is null

    Examples:
    | payload                    |
    | c:,i:,t:,d:,n:,r:,x:,s:*21 |
    | s:,i:,t:,d:,n:,r:,x:,c:*21 |
    | s:,c:,t:,d:,n:,r:,x:,i:*21 |
    | s:,c:,i:,d:,n:,r:,x:,t:*21 |
    | s:,c:,i:,t:,n:,r:,x:,d:*21 |
    | s:,c:,i:,t:,d:,r:,x:,n:*21 |
    | s:,c:,i:,t:,d:,n:,x:,r:*21 |
    | s:,c:,i:,t:,d:,n:,r:,x:*21 |

Scenario: Allow tag block empty fields but invlid field
    When I parse '<payload>' with allowTagBlockEmptyFields of true and throwWhenTagBlockContainsUnknownFields of false
    Then the parser throw an error message 'Tag block entries should start with a type character followed by a colon, and there was no colon'

    Examples:
    | payload                   |
    | c:,i:,t:,d:,n:,r:,x:,s*21 |
    | s:,i:,t:,d:,n:,r:,x:,c*21 |
    | s:,c:,t:,d:,n:,r:,x:,i*21 |
    | s:,c:,i:,d:,n:,r:,x:,t*21 |
    | s:,c:,i:,t:,n:,r:,x:,d*21 |
    | s:,c:,i:,t:,d:,r:,x:,n*21 |
    | s:,c:,i:,t:,d:,n:,x:,r*21 |
    | s:,c:,i:,t:,d:,n:,r:,x*21 |
    | s,c:,i:,t:,d:,n:,r:,x:*21 |
    | c,s:,i:,t:,d:,n:,r:,x:*21 |
    | i,s:,c:,t:,d:,n:,r:,x:*21 |
    | t,s:,c:,i:,d:,n:,r:,x:*21 |
    | d,s:,c:,i:,t:,n:,r:,x:*21 |
    | n,s:,c:,i:,t:,d:,r:,x:*21 |
    | r,s:,c:,i:,t:,d:,n:,x:*21 |
    | x,s:,c:,i:,t:,d:,n:,r:*21 |

Scenario: Disallow tag block empty fields
    When I parse '<payload>' with allowTagBlockEmptyFields of false and throwWhenTagBlockContainsUnknownFields of false
    Then the parser throw an error message 'Tag block entries should start with a type character followed by a colon, and there was no colon'

    Examples:
    | payload |
    | s:*49   |
    | c:*59   |
    | i:*53   |
    | t:*4E   |
    | d:*5E   |
    | n:*54   |
    | r:*48   |
    | x:*42   |
    | s*49    |
    | c*59    |
    | i*53    |
    | t*4E    |
    | d*5E    |
    | n*54    |
    | r*48    |
    | x*42    |

Scenario: Group field is the last of the tag block
    When I parse 'i:<O>IRL</O>,c:1738367940,g:2-2-1470*2F' with allowTagBlockEmptyFields of true and throwWhenTagBlockContainsUnknownFields of false
    Then the TextString is '<O>IRL</O>'
	And the Timestamp is '1738367940'
	And the SentenceGrouping is 2 2 1470
    And no error message reported

Scenario: Group field is the last of the tag block throw an error when allowTagBlockEmptyFields is false
    When I parse 'i:<O>IRL</O>,c:1738367940,g:2-2-1470*2F' with allowTagBlockEmptyFields of false and throwWhenTagBlockContainsUnknownFields of false
    Then the TextString is '<O>IRL</O>'
	And the Timestamp is '1738367940'
	And the SentenceGrouping is 2 2 1470
    And no error message reported

#    When I parse 'i:<O>IRL</O>,c:1738367940,g:2-2-1470*2F' with allowTagBlockEmptyFields of false and throwWhenTagBlockContainsUnknownFields of false
#    Then the parser throw an error message 'Tag block entries should start with a type character followed by a colon, and there was no colon'
