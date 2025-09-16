# Copyright (c) Endjin Limited. All rights reserved.
#
# Contains data under the Norwegian licence for Open Government data (NLOD) distributed by
# the Norwegian Costal Administration - https://ais.kystverket.no/
# The license can be found at https://data.norge.no/nlod/en/2.0
# The lines in this file that contain data from this source are annotated with a comment containing "ais.kystverket.no"
# The NLOD applies only to the data in these annotated lines. The license under which you are using this software
# (either the AGPLv3, or a commercial license) applies to the whole file.

Feature: NmeaStreamParserSpecs
	In order to process AIS messages in NMEA files
	As a developer
	I want to be able to process each of the lines in an NMEA file

Scenario: Empty file
	Given no content
	When I parse the content by line
	Then INmeaLineStreamProcessor.OnComplete should have been called
	And INmeaLineStreamProcessor.OnNext should have been called 0 times

Scenario: Single CRLF blank line only
	Given a CRLF line ''
	When I parse the content by line
	Then INmeaLineStreamProcessor.OnNext should have been called 0 times
	And INmeaLineStreamProcessor.OnComplete should have been called

Scenario: Single LF blank line only
	Given a line ''
	When I parse the content by line
	Then INmeaLineStreamProcessor.OnNext should have been called 0 times
	And INmeaLineStreamProcessor.OnComplete should have been called

Scenario: Single CR blank line only
  Given a CR line ''
  When I parse the content by line
	Then INmeaLineStreamProcessor.OnNext should have been called 0 times
	And INmeaLineStreamProcessor.OnComplete should have been called

Scenario: Multiple CRLF blank lines only
	Given a CRLF line ''
	And a CRLF line ''
	And a CRLF line ''
	When I parse the content by line
	Then INmeaLineStreamProcessor.OnNext should have been called 0 times
	And INmeaLineStreamProcessor.OnComplete should have been called

Scenario: Multiple LF blank lines only
	Given a line ''
	And a line ''
	And a line ''
	When I parse the content by line
	Then INmeaLineStreamProcessor.OnNext should have been called 0 times
	And INmeaLineStreamProcessor.OnComplete should have been called

Scenario: Multiple CR blank lines only
	Given a CR line ''
	And a CR line ''
	And a CR line ''
	When I parse the content by line
	Then INmeaLineStreamProcessor.OnNext should have been called 0 times
	And INmeaLineStreamProcessor.OnComplete should have been called

Scenario: Multiple mixed blank lines only
	Given a CRLF line ''
	And a line ''
	And a CRLF line ''
  And a CR line ''
	When I parse the content by line
	Then INmeaLineStreamProcessor.OnNext should have been called 0 times
	And INmeaLineStreamProcessor.OnComplete should have been called

Scenario: Single line
	# ais.kystverket.no
	Given a line '\s:42,c:1567684904*38\!AIVDM,1,1,,A,B3m:H900AP@b:79ae6:<OwnUoP06,0*78'
	When I parse the content by line
	# ais.kystverket.no
	Then line 0 should have a tag block of 's:42,c:1567684904*38' and a sentence of '!AIVDM,1,1,,A,B3m:H900AP@b:79ae6:<OwnUoP06,0*78'
	Then INmeaLineStreamProcessor.OnNext should have been called 1 time
	And INmeaLineStreamProcessor.OnComplete should have been called

Scenario: Single line without newline only
	# ais.kystverket.no
	Given an unterminated line '\s:42,c:1567684904*38\!AIVDM,1,1,,A,B3m:H900AP@b:79ae6:<OwnUoP06,0*78'
	When I parse the content by line
	# ais.kystverket.no
	Then line 0 should have a tag block of 's:42,c:1567684904*38' and a sentence of '!AIVDM,1,1,,A,B3m:H900AP@b:79ae6:<OwnUoP06,0*78'
	Then INmeaLineStreamProcessor.OnNext should have been called 1 time
	And INmeaLineStreamProcessor.OnComplete should have been called

Scenario: Multiple lines
	# ais.kystverket.no
	Given a line '\s:42,c:1567684904*38\!AIVDM,1,1,,A,B3m:H900AP@b:79ae6:<OwnUoP06,0*78'
	# ais.kystverket.no
	And a line '\s:3,c:1567692251*01\!AIVDM,1,1,,A,13m9WS001d0K==pR=D?HB6WD0pJV,0*54'
	# ais.kystverket.no
	And a line '\s:24,c:1567692878*35\!AIVDM,1,1,,B,13o`9@701j1Ej3vc;o3q@7SJ0D02,0*21'
	# ais.kystverket.no
	And a line '\s:772,c:1567693246*07\!AIVDM,1,1,,,13o7g2001P0Lv<rSdVHf2h3N0000,0*25'
	When I parse the content by line
	Then INmeaLineStreamProcessor.OnNext should have been called 4 times
	# ais.kystverket.no
	And line 0 should have a tag block of 's:42,c:1567684904*38' and a sentence of '!AIVDM,1,1,,A,B3m:H900AP@b:79ae6:<OwnUoP06,0*78'
	# ais.kystverket.no
	And line 1 should have a tag block of 's:3,c:1567692251*01' and a sentence of '!AIVDM,1,1,,A,13m9WS001d0K==pR=D?HB6WD0pJV,0*54'
	# ais.kystverket.no
	And line 2 should have a tag block of 's:24,c:1567692878*35' and a sentence of '!AIVDM,1,1,,B,13o`9@701j1Ej3vc;o3q@7SJ0D02,0*21'
	# ais.kystverket.no
	And line 3 should have a tag block of 's:772,c:1567693246*07' and a sentence of '!AIVDM,1,1,,,13o7g2001P0Lv<rSdVHf2h3N0000,0*25'
	And INmeaLineStreamProcessor.OnComplete should have been called

Scenario: Multiple CR lines
	# ais.kystverket.no
	Given a CR line '\s:42,c:1567684904*38\!AIVDM,1,1,,A,B3m:H900AP@b:79ae6:<OwnUoP06,0*78'
	# ais.kystverket.no
	And a CR line '\s:3,c:1567692251*01\!AIVDM,1,1,,A,13m9WS001d0K==pR=D?HB6WD0pJV,0*54'
	# ais.kystverket.no
	And a CR line '\s:24,c:1567692878*35\!AIVDM,1,1,,B,13o`9@701j1Ej3vc;o3q@7SJ0D02,0*21'
	# ais.kystverket.no
	And a CR line '\s:772,c:1567693246*07\!AIVDM,1,1,,,13o7g2001P0Lv<rSdVHf2h3N0000,0*25'
	When I parse the content by line
	Then INmeaLineStreamProcessor.OnNext should have been called 4 times
	# ais.kystverket.no
	And line 0 should have a tag block of 's:42,c:1567684904*38' and a sentence of '!AIVDM,1,1,,A,B3m:H900AP@b:79ae6:<OwnUoP06,0*78'
	# ais.kystverket.no
	And line 1 should have a tag block of 's:3,c:1567692251*01' and a sentence of '!AIVDM,1,1,,A,13m9WS001d0K==pR=D?HB6WD0pJV,0*54'
	# ais.kystverket.no
	And line 2 should have a tag block of 's:24,c:1567692878*35' and a sentence of '!AIVDM,1,1,,B,13o`9@701j1Ej3vc;o3q@7SJ0D02,0*21'
	# ais.kystverket.no
	And line 3 should have a tag block of 's:772,c:1567693246*07' and a sentence of '!AIVDM,1,1,,,13o7g2001P0Lv<rSdVHf2h3N0000,0*25'
	And INmeaLineStreamProcessor.OnComplete should have been called

Scenario: Multiple lines where final line has no newline
	# ais.kystverket.no
	Given a line '\s:42,c:1567684904*38\!AIVDM,1,1,,A,B3m:H900AP@b:79ae6:<OwnUoP06,0*78'
	# ais.kystverket.no
	And a line '\s:3,c:1567692251*01\!AIVDM,1,1,,A,13m9WS001d0K==pR=D?HB6WD0pJV,0*54'
	# ais.kystverket.no
	And a line '\s:24,c:1567692878*35\!AIVDM,1,1,,B,13o`9@701j1Ej3vc;o3q@7SJ0D02,0*21'
	# ais.kystverket.no
	And an unterminated line '\s:772,c:1567693246*07\!AIVDM,1,1,,,13o7g2001P0Lv<rSdVHf2h3N0000,0*25'
	When I parse the content by line
	Then INmeaLineStreamProcessor.OnNext should have been called 4 times
	# ais.kystverket.no
	And line 0 should have a tag block of 's:42,c:1567684904*38' and a sentence of '!AIVDM,1,1,,A,B3m:H900AP@b:79ae6:<OwnUoP06,0*78'
	# ais.kystverket.no
	And line 1 should have a tag block of 's:3,c:1567692251*01' and a sentence of '!AIVDM,1,1,,A,13m9WS001d0K==pR=D?HB6WD0pJV,0*54'
	# ais.kystverket.no
	And line 2 should have a tag block of 's:24,c:1567692878*35' and a sentence of '!AIVDM,1,1,,B,13o`9@701j1Ej3vc;o3q@7SJ0D02,0*21'
	# ais.kystverket.no
	And line 3 should have a tag block of 's:772,c:1567693246*07' and a sentence of '!AIVDM,1,1,,,13o7g2001P0Lv<rSdVHf2h3N0000,0*25'
	And INmeaLineStreamProcessor.OnComplete should have been called

Scenario: Multiple lines with blanks at start
	Given a line ''
	Given a line ''
	Given a line ''
	# ais.kystverket.no
	And a line '\s:42,c:1567684904*38\!AIVDM,1,1,,A,B3m:H900AP@b:79ae6:<OwnUoP06,0*78'
	# ais.kystverket.no
	And a line '\s:3,c:1567692251*01\!AIVDM,1,1,,A,13m9WS001d0K==pR=D?HB6WD0pJV,0*54'
	# ais.kystverket.no
	And a line '\s:24,c:1567692878*35\!AIVDM,1,1,,B,13o`9@701j1Ej3vc;o3q@7SJ0D02,0*21'
	# ais.kystverket.no
	And a line '\s:772,c:1567693246*07\!AIVDM,1,1,,,13o7g2001P0Lv<rSdVHf2h3N0000,0*25'
	When I parse the content by line
	Then INmeaLineStreamProcessor.OnNext should have been called 4 times
	# ais.kystverket.no
	And line 0 should have a tag block of 's:42,c:1567684904*38' and a sentence of '!AIVDM,1,1,,A,B3m:H900AP@b:79ae6:<OwnUoP06,0*78'
	# ais.kystverket.no
	And line 1 should have a tag block of 's:3,c:1567692251*01' and a sentence of '!AIVDM,1,1,,A,13m9WS001d0K==pR=D?HB6WD0pJV,0*54'
	# ais.kystverket.no
	And line 2 should have a tag block of 's:24,c:1567692878*35' and a sentence of '!AIVDM,1,1,,B,13o`9@701j1Ej3vc;o3q@7SJ0D02,0*21'
	# ais.kystverket.no
	And line 3 should have a tag block of 's:772,c:1567693246*07' and a sentence of '!AIVDM,1,1,,,13o7g2001P0Lv<rSdVHf2h3N0000,0*25'
	And INmeaLineStreamProcessor.OnComplete should have been called

Scenario: Multiple lines with blanks in middle
	# ais.kystverket.no
	Given a line '\s:42,c:1567684904*38\!AIVDM,1,1,,A,B3m:H900AP@b:79ae6:<OwnUoP06,0*78'
	# ais.kystverket.no
	And a line '\s:3,c:1567692251*01\!AIVDM,1,1,,A,13m9WS001d0K==pR=D?HB6WD0pJV,0*54'
	Given a line ''
	Given a line ''
	Given a line ''
	# ais.kystverket.no
	And a line '\s:24,c:1567692878*35\!AIVDM,1,1,,B,13o`9@701j1Ej3vc;o3q@7SJ0D02,0*21'
	# ais.kystverket.no
	And a line '\s:772,c:1567693246*07\!AIVDM,1,1,,,13o7g2001P0Lv<rSdVHf2h3N0000,0*25'
	When I parse the content by line
	Then INmeaLineStreamProcessor.OnNext should have been called 4 times
	# ais.kystverket.no
	And line 0 should have a tag block of 's:42,c:1567684904*38' and a sentence of '!AIVDM,1,1,,A,B3m:H900AP@b:79ae6:<OwnUoP06,0*78'
	# ais.kystverket.no
	And line 1 should have a tag block of 's:3,c:1567692251*01' and a sentence of '!AIVDM,1,1,,A,13m9WS001d0K==pR=D?HB6WD0pJV,0*54'
	# ais.kystverket.no
	And line 2 should have a tag block of 's:24,c:1567692878*35' and a sentence of '!AIVDM,1,1,,B,13o`9@701j1Ej3vc;o3q@7SJ0D02,0*21'
	# ais.kystverket.no
	And line 3 should have a tag block of 's:772,c:1567693246*07' and a sentence of '!AIVDM,1,1,,,13o7g2001P0Lv<rSdVHf2h3N0000,0*25'
	And INmeaLineStreamProcessor.OnComplete should have been called

Scenario: Multiple lines with blanks at end
	# ais.kystverket.no
	Given a line '\s:42,c:1567684904*38\!AIVDM,1,1,,A,B3m:H900AP@b:79ae6:<OwnUoP06,0*78'
	# ais.kystverket.no
	And a line '\s:3,c:1567692251*01\!AIVDM,1,1,,A,13m9WS001d0K==pR=D?HB6WD0pJV,0*54'
	# ais.kystverket.no
	And a line '\s:24,c:1567692878*35\!AIVDM,1,1,,B,13o`9@701j1Ej3vc;o3q@7SJ0D02,0*21'
	# ais.kystverket.no
	And a line '\s:772,c:1567693246*07\!AIVDM,1,1,,,13o7g2001P0Lv<rSdVHf2h3N0000,0*25'
	Given a line ''
	Given a line ''
	Given a line ''
	When I parse the content by line
	Then INmeaLineStreamProcessor.OnNext should have been called 4 times
	# ais.kystverket.no
	And line 0 should have a tag block of 's:42,c:1567684904*38' and a sentence of '!AIVDM,1,1,,A,B3m:H900AP@b:79ae6:<OwnUoP06,0*78'
	# ais.kystverket.no
	And line 1 should have a tag block of 's:3,c:1567692251*01' and a sentence of '!AIVDM,1,1,,A,13m9WS001d0K==pR=D?HB6WD0pJV,0*54'
	# ais.kystverket.no
	And line 2 should have a tag block of 's:24,c:1567692878*35' and a sentence of '!AIVDM,1,1,,B,13o`9@701j1Ej3vc;o3q@7SJ0D02,0*21'
	# ais.kystverket.no
	And line 3 should have a tag block of 's:772,c:1567693246*07' and a sentence of '!AIVDM,1,1,,,13o7g2001P0Lv<rSdVHf2h3N0000,0*25'
	And INmeaLineStreamProcessor.OnComplete should have been called

Scenario: Single unparseable line
	# ais.kystverket.no
	Given a line 'I am not an NMEA message'
	When I parse the content by line
	# ais.kystverket.no
	Then INmeaLineStreamProcessor.OnNext should have been called 0 times
	Then OnError should have been called 1 time
	And the line error report 0 should include the problematic line 'I am not an NMEA message'
	And the line error report 0 should include an exception reporting that the expected exclamation mark is missing
	And the line error report 0 should include the line number 1
	And INmeaLineStreamProcessor.OnComplete should have been called

Scenario: One unparseable line and one good line
	# ais.kystverket.no
	Given a line '\s:42,c:1567684904*38\!AIVDM,1,1,,A,B3m:H900AP@b:79ae6:<OwnUoP06,0*78'
	And a line 'I am not an NMEA message'
	When I parse the content by line
	# ais.kystverket.no
	Then line 0 should have a tag block of 's:42,c:1567684904*38' and a sentence of '!AIVDM,1,1,,A,B3m:H900AP@b:79ae6:<OwnUoP06,0*78'
	Then INmeaLineStreamProcessor.OnNext should have been called 1 time
	Then OnError should have been called 1 time
	And the line error report 0 should include the problematic line 'I am not an NMEA message'
	And the line error report 0 should include an exception reporting that the expected exclamation mark is missing
	And the line error report 0 should include the line number 2
	And INmeaLineStreamProcessor.OnComplete should have been called
    

Scenario: Allow unreconized takler id
	Given I have configured a AllowUnreconizedTalkerId of true
	And a line '\i:<O>NOR</O>,s:2573105,c:1668997143*00\!B2VDM,1,1,9,A,H3mWb5P5HT4pD00000000000000,2*0A'
	When I parse the content by line
	Then INmeaLineStreamProcessor.OnNext should have been called 1 times
	Then OnError should have been called 0 time

Scenario: Disallow unreconized takler id
	Given I have configured a AllowUnreconizedTalkerId of false
	And a line '\i:<O>NOR</O>,s:2573105,c:1668997147*04\!B1VDM,2,1,9,A,53m6J4p00000dp=@E=@dp=@E=@0000000000000000000t00000P00000000,0*20'
	And a line '\i:<O>NOR</O>,s:2573105,c:1668997143*00\!B2VDM,1,1,9,A,H3mWb5P5HT4pD00000000000000,2*0A'
	When I parse the content by line
	Then INmeaLineStreamProcessor.OnNext should have been called 0 times
	Then OnError should have been called 2 time
	And the line error report 0 should include an exception reporting an invalid talker id with invalid char '49'
	And the line error report 1 should include an exception reporting an invalid talker id with invalid char '50'

Scenario: Invalid talker id with AllowUnreconizedTalkerId to false throw an error
	Given I have configured a AllowUnreconizedDataOrigin of false
    And a line '\c:1725231098,t:TER*78\!AIVDI,1,1,,B,37Oftm3Oh:bcIrUkmmv5`Qfp01o0,0*05'
	When I parse the content by line
	Then INmeaLineStreamProcessor.OnNext should have been called 0 time
	Then OnError should have been called 1 time
	And the line error report 0 should include an exception reporting an invalid talker data origin

Scenario: Invalid talker id with AllowUnreconizedTalkerId to true not throw error
	Given I have configured a AllowUnreconizedDataOrigin of true
	And I have configured a AllowUnreconizedTalkerId of true
    And a line '<payload>'
	When I parse the content by line
    Then line 0 should AisTakler of <talkerId>, a DataOrigin of <dataOrigin> and a SentenceFormatter of <sentenceFormatter>
	And INmeaLineStreamProcessor.OnNext should have been called 1 time
	And OnError should have been called 0 time

    Examples:
    | payload                                                                                 | talkerId | dataOrigin | sentenceFormatter |
    | \c:1725231098,t:TER*78\!AIVDI,1,1,,B,37Oftm3Oh:bcIrUkmmv5`Qfp01o0,0*05                  | 2        | 2          | !AIVDI            |
    | \c:1725173550,t:TER*7C\!AIDMV,1,1,,B,15?dMCh01`9ORmnCKTLQ<hnj0<2?,0*4A                  | 2        | 2          | !AIDMV            |
    | \c:1725176210,t:TER*7A\!AIvDM,1,1,,A,402;bK1vR@wTgPbmd4HVqs700l76,0*7A                  | 2        | 2          | !AIvDM            |
    | \c:1725181228,t:TERA*38\!AIVFM,1,1,,A,33f?cT?P00PfpT8I6=:>4?vl2>`<,0*08                 | 2        | 2          | !AIVFM            |
    | \i:<O>NOR</O>,s:2573105,c:1668997143*00\!B2VDM,1,1,9,A,H3mWb5P5HT4pD00000000000000,2*0A | 10       | 0          | !B2VDM            |

Scenario: Missing checksum
    Given I have configured ChecksumOption of 1
    And a line '\s:rMT9999,c:1747907742\!BSVDM,1,1,,A,33mI?D5P00PkU>BU6kw`:gwB2Dmr,0*09'
    And a line '\s:rMT9999,c:1747907742*5D\!BSVDM,1,1,,A,33mI?D5P00PkU>BU6kw`:gwB2Dmr,0'
	When I parse the content by line
	Then OnError should have been called 2 times
	And the line error report 0 should include an exception reporting that the checksum is missing
	And the line error report 1 should include an exception reporting that the checksum is missing

Scenario: Invalid checksum format
    Given I have configured ChecksumOption of 2
    And a line '<payload>'
	When I parse the content by line
	Then OnError should have been called 1 time
	And the line error report 0 should include an invalid checksum format.

    Examples:
    | payload                                                                     |
    | \s:rMT9999,c:1747907742*\!BSVDM,1,1,,A,33mI?D5P00PkU>BU6kw`:gwB2Dmr,0*09    |
    | \s:rMT9999,c:1747907742*5DD\!BSVDM,1,1,,A,33mI?D5P00PkU>BU6kw`:gwB2Dmr,0*09 |
    | \s:rMT9999,c:1747907742*5D\!BSVDM,1,1,,A,33mI?D5P00PkU>BU6kw`:gwB2Dmr,0*    |
    | \s:rMT9999,c:1747907742*5D\!BSVDM,1,1,,A,33mI?D5P00PkU>BU6kw`:gwB2Dmr,0*099 |

Scenario: Invalid strict checksum format
    Given I have configured ChecksumOption of 1
    And a line '<payload>'
	When I parse the content by line
	Then OnError should have been called 1 time
	And the line error report 0 should include an invalid strict checksum format.

    Examples:
    | payload                                                                     |
    | \s:rMT9999,c:1747907742*\!BSVDM,1,1,,A,33mI?D5P00PkU>BU6kw`:gwB2Dmr,0*09    |
    | \s:rMT9999,c:1747907742*5\!BSVDM,1,1,,A,33mI?D5P00PkU>BU6kw`:gwB2Dmr,0*09   |
    | \s:rMT9999,c:1747907742*5DD\!BSVDM,1,1,,A,33mI?D5P00PkU>BU6kw`:gwB2Dmr,0*09 |
    | \s:rMT9999,c:1747907742*5D\!BSVDM,1,1,,A,33mI?D5P00PkU>BU6kw`:gwB2Dmr,0*    |
    | \s:rMT9999,c:1747907742*5D\!BSVDM,1,1,,A,33mI?D5P00PkU>BU6kw`:gwB2Dmr,0*0   |
    | \s:rMT9999,c:1747907742*5D\!BSVDM,1,1,,A,33mI?D5P00PkU>BU6kw`:gwB2Dmr,0*099 |

Scenario: Invalid checksum skipped
    Given I have configured ChecksumOption of 0
    And a line '<payload>'
	When I parse the content by line
	Then OnError should have been called 0 time
    
    Examples:
    | payload                                                                     |
    | \s:rMT9999,c:1747907742*\!BSVDM,1,1,,A,33mI?D5P00PkU>BU6kw`:gwB2Dmr,0*09    |
    | \s:rMT9999,c:1747907742*5\!BSVDM,1,1,,A,33mI?D5P00PkU>BU6kw`:gwB2Dmr,0*09   |
    | \s:rMT9999,c:1747907742*5DD\!BSVDM,1,1,,A,33mI?D5P00PkU>BU6kw`:gwB2Dmr,0*09 |
    | \s:rMT9999,c:1747907742*5D\!BSVDM,1,1,,A,33mI?D5P00PkU>BU6kw`:gwB2Dmr,0*    |
    | \s:rMT9999,c:1747907742*5D\!BSVDM,1,1,,A,33mI?D5P00PkU>BU6kw`:gwB2Dmr,0*0   |
    | \s:rMT9999,c:1747907742*5D\!BSVDM,1,1,,A,33mI?D5P00PkU>BU6kw`:gwB2Dmr,0*099 |

Scenario: Invalid cheksum value
    Given I have configured ChecksumOption of 3
    And a line '<payload>'
	When I parse the content by line
	Then OnError should have been called 1 time
	And the line error report 0 should include an invalid checksum character '<char>'

    Examples:
    | payload                                                                    | char |
    | \s:rMT9999,c:1747907742*0Z\!BSVDM,1,1,,A,33mI?D5P00PkU>BU6kw`:gwB2Dmr,0*09 | Z    |
    | \s:rMT9999,c:1747907742*H5\!BSVDM,1,1,,A,33mI?D5P00PkU>BU6kw`:gwB2Dmr,0*09 | H    |
    | \s:rMT9999,c:1747907742*5D\!BSVDM,1,1,,A,33mI?D5P00PkU>BU6kw`:gwB2Dmr,0*0X | X    |
    | \s:rMT9999,c:1747907742*5D\!BSVDM,1,1,,A,33mI?D5P00PkU>BU6kw`:gwB2Dmr,0*O0 | O    |
    
Scenario: Invalid cheksum result
    Given I have configured ChecksumOption of <co>
    And a line '<payload>'
	When I parse the content by line
	Then OnError should have been called 1 time
	And the line error report 0 should include an invalid checksum result '<result>' but expect '<expect>'

    Examples:
    | payload                                                                    | result | expect | co |
    | \s:rMT9999,c:1747907742*E3\!BSVDM,1,1,,A,33mI?D5P00PkU>BU6kw`:gwB2Dmr,0*09 | 5D     | E3     | 3  |
    | \s:rMT9999,c:1747907742*0B\!BSVDM,1,1,,A,33mI?D5P00PkU>BU6kw`:gwB2Dmr,0*09 | 5D     | 0B     | 3  |
    | \s:rMT9999,c:1747907742*5D\!BSVDM,1,1,,A,33mI?D5P00PkU>BU6kw`:gwB2Dmr,0*03 | 09     | 03     | 3  |
    | \s:rMT9999,c:1747907742*5D\!BSVDM,1,1,,A,33mI?D5P00PkU>BU6kw`:gwB2Dmr,0*AA | 09     | AA     | 3  |
    | \s:rMT9999,c:1747907742*3\!BSVDM,1,1,,A,33mI?D5P00PkU>BU6kw`:gwB2Dmr,0*09  | 5D     | 03     | 2  |
    | \s:rMT9999,c:1747907742*B\!BSVDM,1,1,,A,33mI?D5P00PkU>BU6kw`:gwB2Dmr,0*09  | 5D     | 0B     | 2  |
    | \s:rMT9999,c:1747907742*5D\!BSVDM,1,1,,A,33mI?D5P00PkU>BU6kw`:gwB2Dmr,0*3  | 09     | 03     | 2  |
    | \s:rMT9999,c:1747907742*5D\!BSVDM,1,1,,A,33mI?D5P00PkU>BU6kw`:gwB2Dmr,0*A  | 09     | 0A     | 2  |

Scenario: Validate checksum with one digit
    Given I have configured ChecksumOption of 2
    And a line '<payload>'
	When I parse the content by line
	Then OnError should have been called 0 time

    Examples:
    | payload                                                                    |
    | \s:rMT9999,c:1747907742*5D\!BSVDM,1,1,,A,33mI?D5P00PkU>BU6kw`:gwB2Dmr,0*9  |

Scenario: Validate checksum with two digits
    Given I have configured ChecksumOption of 3
    And a line '<payload>'
	When I parse the content by line
	Then OnError should have been called 0 time

    Examples:
    | payload                                                                                                   |
    | \s:rMT9999,c:1747907742*5D\!BSVDM,1,1,,A,33mI?D5P00PkU>BU6kw`:gwB2Dmr,0*09                                |
    | \s:rMT7304,c:1747704784*5A\!AIVDM,1,1,,A,23:q:j0001WaoVN616Vm?hml082e,0*7F                                |
    | \s:rMT1667,c:1747704784*5C\!AIVDM,1,1,,A,ENkb;0<Hah@@@@@@@@@@@@@@@@@;Wd6F:ewt800003vP003,2*2A             |
    | \s:rMT7800,c:1747704786*57\!AIVDM,1,1,,A,8@2<HW@0BkdhF0dcH50h3j8412VrGwdfwwwwwwwwwwwwwwwwwwwwwwwwwt0,2*3F |
    | \s:rMT5465,c:1747908456*56\!AIVDM,1,1,,A,Dh400QAMe>fp,0*2E                                                |
