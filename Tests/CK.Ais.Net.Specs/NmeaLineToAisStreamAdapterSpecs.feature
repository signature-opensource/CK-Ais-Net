# Copyright (c) Endjin Limited. All rights reserved.
#
# Contains data under the Norwegian licence for Open Government data (NLOD) distributed by
# the Norwegian Costal Administration - https://ais.kystverket.no/
# The license can be found at https://data.norge.no/nlod/en/2.0
# The lines in this file that contain data from this source are annotated with a comment containing "ais.kystverket.no"
# The NLOD applies only to the data in these annotated lines. The license under which you are using this software
# (either the AGPLv3, or a commercial license) applies to the whole file.

Feature: NmeaLineToAisStreamAdapterSpecs
	In order to process the AIS messages in NMEA files
	As a developer
	I want to be able to receive complete AIS message payloads even when they are split over multiple NMEA lines

Scenario: Non-fragmented message received
	# ais.kystverket.no
	When the line to message adapter receives '\s:42,c:1567684904*38\!AIVDM,1,1,,B,33m9UtPP@50wwE:VJW6LS67H01<@,0*3C'
	Then INmeaAisMessageStreamProcessor.OnNext should have been called 1 time
    And in ais message 0 the payload should be '33m9UtPP@50wwE:VJW6LS67H01<@' with padding of 0
    And in ais message 0 the source from the first NMEA line should be 42
    And in ais message 0 the timestamp from the first NMEA line should be 1567684904

Scenario: First fragment of two-part message received
	# ais.kystverket.no
	When the line to message adapter receives '\g:1-2-8055,s:99,c:1567685556*4E\!AIVDM,2,1,6,B,53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4l,0*72'
	Then INmeaAisMessageStreamProcessor.OnNext should have been called 0 times

Scenario: First fragment of two-part message without grouping in header received
	# ais.kystverket.no
	When the line to message adapter receives '\s:99,c:1567685556*4E\!AIVDM,2,1,6,B,53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4l,0*72'
	Then INmeaAisMessageStreamProcessor.OnNext should have been called 0 times

Scenario: Two-fragment message fragments received adjacently
	# ais.kystverket.no
	When the line to message adapter receives '\g:1-2-8055,s:99,c:1567685556*4E\!AIVDM,2,1,6,B,53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4l,0*72'
	# ais.kystverket.no
	And the line to message adapter receives '\g:2-2-8055*55\!AIVDM,2,2,6,B,j`888888880,2*2B'
	Then INmeaAisMessageStreamProcessor.OnNext should have been called 1 time
	# ais.kystverket.no
    And in ais message 0 the payload should be '53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4lj`888888880' with padding of 2
    And in ais message 0 the source from the first NMEA line should be 99
    And in ais message 0 the timestamp from the first NMEA line should be 1567685556

Scenario: Two-fragment message fragments without grouping in header received adjacently
	# ais.kystverket.no
	When the line to message adapter receives '\s:99,c:1567685556*4E\!AIVDM,2,1,6,B,53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4l,0*72'
	# ais.kystverket.no
	And the line to message adapter receives '\s:99*55\!AIVDM,2,2,6,B,j`888888880,2*2B'
	Then INmeaAisMessageStreamProcessor.OnNext should have been called 1 time
	# ais.kystverket.no
    And in ais message 0 the payload should be '53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4lj`888888880' with padding of 2
    And in ais message 0 the source from the first NMEA line should be 99
    And in ais message 0 the timestamp from the first NMEA line should be 1567685556

Scenario: Two-fragment message fragments received with single-fragment message in the middle
	# ais.kystverket.no
	When the line to message adapter receives '\g:1-2-8055,s:99,c:1567685556*4E\!AIVDM,2,1,6,B,53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4l,0*72'
	# ais.kystverket.no
	And the line to message adapter receives '\s:42,c:1567684904*38\!AIVDM,1,1,,B,33m9UtPP@50wwE:VJW6LS67H01<@,0*3C'
	# ais.kystverket.no
	And the line to message adapter receives '\g:2-2-8055*55\!AIVDM,2,2,6,B,j`888888880,2*2B'
	Then INmeaAisMessageStreamProcessor.OnNext should have been called 2 times
	# ais.kystverket.no
    And in ais message 0 the payload should be '33m9UtPP@50wwE:VJW6LS67H01<@' with padding of 0
    And in ais message 0 the source from the first NMEA line should be 42
    And in ais message 0 the timestamp from the first NMEA line should be 1567684904
	# ais.kystverket.no
    And in ais message 1 the payload should be '53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4lj`888888880' with padding of 2
    And in ais message 1 the source from the first NMEA line should be 99
    And in ais message 1 the timestamp from the first NMEA line should be 1567685556

Scenario: Two-fragment message fragments without grouping in header received with single-fragment message in the middle
	# ais.kystverket.no
	When the line to message adapter receives '\s:99,c:1567685556*4E\!AIVDM,2,1,6,B,53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4l,0*72'
	# ais.kystverket.no
	And the line to message adapter receives '\s:42,c:1567684904*38\!AIVDM,1,1,,B,33m9UtPP@50wwE:VJW6LS67H01<@,0*3C'
	# ais.kystverket.no
	And the line to message adapter receives '\s:99*55\!AIVDM,2,2,6,B,j`888888880,2*2B'
	Then INmeaAisMessageStreamProcessor.OnNext should have been called 2 times
	# ais.kystverket.no
    And in ais message 0 the payload should be '33m9UtPP@50wwE:VJW6LS67H01<@' with padding of 0
    And in ais message 0 the source from the first NMEA line should be 42
    And in ais message 0 the timestamp from the first NMEA line should be 1567684904
	# ais.kystverket.no
    And in ais message 1 the payload should be '53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4lj`888888880' with padding of 2
    And in ais message 1 the source from the first NMEA line should be 99
    And in ais message 1 the timestamp from the first NMEA line should be 1567685556

Scenario: Three-fragment message fragments received adjacently
	# ais.kystverket.no
	When the line to message adapter receives '\g:1-3-3451,s:27,c:1567686150*40\!AIVDM,3,1,9,A,544MR0827oeaD<u0000lDdP4pTf0duAA,0*17'
	# ais.kystverket.no
	And the line to message adapter receives '\g:2-3-3451*5F\!AIVDM,3,2,9,A,<uH000167pF=2=nG0:0DRj0CQiC4jh00,0*4A'
	# ais.kystverket.no
	And the line to message adapter receives '\g:3-3-3451*5E\!AIVDM,3,3,9,A,0000000,0*2F'
	Then INmeaAisMessageStreamProcessor.OnNext should have been called 1 time
	# ais.kystverket.no
    And in ais message 0 the payload should be '544MR0827oeaD<u0000lDdP4pTf0duAA<uH000167pF=2=nG0:0DRj0CQiC4jh000000000' with padding of 0
    And in ais message 0 the source from the first NMEA line should be 27
    And in ais message 0 the timestamp from the first NMEA line should be 1567686150

Scenario: Three-fragment message fragments without grouping in header received adjacently
	# ais.kystverket.no
	When the line to message adapter receives '\s:27,c:1567686150*40\!AIVDM,3,1,9,A,544MR0827oeaD<u0000lDdP4pTf0duAA,0*17'
	# ais.kystverket.no
	And the line to message adapter receives '\s:27*5F\!AIVDM,3,2,9,A,<uH000167pF=2=nG0:0DRj0CQiC4jh00,0*4A'
	# ais.kystverket.no
	And the line to message adapter receives '\s:27*5E\!AIVDM,3,3,9,A,0000000,0*2F'
	Then INmeaAisMessageStreamProcessor.OnNext should have been called 1 time
	# ais.kystverket.no
    And in ais message 0 the payload should be '544MR0827oeaD<u0000lDdP4pTf0duAA<uH000167pF=2=nG0:0DRj0CQiC4jh000000000' with padding of 0
    And in ais message 0 the source from the first NMEA line should be 27
    And in ais message 0 the timestamp from the first NMEA line should be 1567686150

Scenario: Interleaved multi-fragment messages
	# ais.kystverket.no
	When the line to message adapter receives '\g:1-3-3451,s:27,c:1567686150*40\!AIVDM,3,1,9,A,544MR0827oeaD<u0000lDdP4pTf0duAA,0*17'
	# ais.kystverket.no
	And the line to message adapter receives '\g:1-2-8055,s:99,c:1567685556*4E\!AIVDM,2,1,6,B,53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4l,0*72'
	# ais.kystverket.no
	And the line to message adapter receives '\g:2-3-3451*5F\!AIVDM,3,2,9,A,<uH000167pF=2=nG0:0DRj0CQiC4jh00,0*4A'
	# ais.kystverket.no
	And the line to message adapter receives '\g:2-2-8055*55\!AIVDM,2,2,6,B,j`888888880,2*2B'
	# ais.kystverket.no
	And the line to message adapter receives '\g:3-3-3451*5E\!AIVDM,3,3,9,A,0000000,0*2F'
	Then INmeaAisMessageStreamProcessor.OnNext should have been called 2 times
	# ais.kystverket.no
    And in ais message 0 the payload should be '53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4lj`888888880' with padding of 2
    And in ais message 0 the source from the first NMEA line should be 99
    And in ais message 0 the timestamp from the first NMEA line should be 1567685556
	# ais.kystverket.no
    And in ais message 1 the payload should be '544MR0827oeaD<u0000lDdP4pTf0duAA<uH000167pF=2=nG0:0DRj0CQiC4jh000000000' with padding of 0
    And in ais message 1 the source from the first NMEA line should be 27
    And in ais message 1 the timestamp from the first NMEA line should be 1567686150

Scenario: Interleaved multi-fragment messages without grouping in header
	# ais.kystverket.no
	When the line to message adapter receives '\s:27,c:1567686150*40\!AIVDM,3,1,9,A,544MR0827oeaD<u0000lDdP4pTf0duAA,0*17'
	# ais.kystverket.no
	And the line to message adapter receives '\s:99,c:1567685556*4E\!AIVDM,2,1,6,B,53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4l,0*72'
	# ais.kystverket.no
	And the line to message adapter receives '\s:27,*5F\!AIVDM,3,2,9,A,<uH000167pF=2=nG0:0DRj0CQiC4jh00,0*4A'
	# ais.kystverket.no
	And the line to message adapter receives '\s:99,*55\!AIVDM,2,2,6,B,j`888888880,2*2B'
	# ais.kystverket.no
	And the line to message adapter receives '\s:27*5E\!AIVDM,3,3,9,A,0000000,0*2F'
	Then INmeaAisMessageStreamProcessor.OnNext should have been called 2 times
	# ais.kystverket.no
    And in ais message 0 the payload should be '53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4lj`888888880' with padding of 2
    And in ais message 0 the source from the first NMEA line should be 99
    And in ais message 0 the timestamp from the first NMEA line should be 1567685556
	# ais.kystverket.no
    And in ais message 1 the payload should be '544MR0827oeaD<u0000lDdP4pTf0duAA<uH000167pF=2=nG0:0DRj0CQiC4jh000000000' with padding of 0
    And in ais message 1 the source from the first NMEA line should be 27
    And in ais message 1 the timestamp from the first NMEA line should be 1567686150

Scenario: Interleaved multi-fragment messages distinguished only by grouping in header
	# ais.kystverket.no
	When the line to message adapter receives '\g:1-3-3451,s:27,c:1567686150*40\!AIVDM,3,1,9,A,544MR0827oeaD<u0000lDdP4pTf0duAA,0*17'
	# ais.kystverket.no
	And the line to message adapter receives '\g:1-2-8055,s:99,c:1567685556*4E\!AIVDM,2,1,9,B,53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4l,0*72'
	# ais.kystverket.no
	And the line to message adapter receives '\g:2-3-3451*5F\!AIVDM,3,2,9,A,<uH000167pF=2=nG0:0DRj0CQiC4jh00,0*4A'
	# ais.kystverket.no
	And the line to message adapter receives '\g:2-2-8055*55\!AIVDM,2,2,9,B,j`888888880,2*2B'
	# ais.kystverket.no
	And the line to message adapter receives '\g:3-3-3451*5E\!AIVDM,3,3,9,A,0000000,0*2F'
	Then INmeaAisMessageStreamProcessor.OnNext should have been called 2 times
	# ais.kystverket.no
    And in ais message 0 the payload should be '53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4lj`888888880' with padding of 2
    And in ais message 0 the source from the first NMEA line should be 99
    And in ais message 0 the timestamp from the first NMEA line should be 1567685556
	# ais.kystverket.no
    And in ais message 1 the payload should be '544MR0827oeaD<u0000lDdP4pTf0duAA<uH000167pF=2=nG0:0DRj0CQiC4jh000000000' with padding of 0
    And in ais message 1 the source from the first NMEA line should be 27
    And in ais message 1 the timestamp from the first NMEA line should be 1567686150

Scenario: Interleaved single and multi-fragment messages
	# This test interleaves more extensively than we ever see in reality, so we have to extend our reassembly window
	Given I have configured a MaximumUnmatchedFragmentAge of 10
	# ais.kystverket.no
	When the line to message adapter receives '\s:42,c:1567684904*38\!AIVDM,1,1,,B,33m9UtPP@50wwE:VJW6LS67H01<@,0*3C'
	# ais.kystverket.no
	And the line to message adapter receives '\g:1-3-3451,s:27,c:1567686150*40\!AIVDM,3,1,9,A,544MR0827oeaD<u0000lDdP4pTf0duAA,0*17'
	# ais.kystverket.no
	And the line to message adapter receives '\s:42,c:1567684904*38\!AIVDM,1,1,,A,B3m:H900AP@b:79ae6:<OwnUoP06,0*78'
	# ais.kystverket.no
	And the line to message adapter receives '\s:3,c:1567692251*01\!AIVDM,1,1,,A,13m9WS001d0K==pR=D?HB6WD0pJV,0*54'
	# ais.kystverket.no
	And the line to message adapter receives '\g:1-2-8055,s:99,c:1567685556*4E\!AIVDM,2,1,6,B,53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4l,0*72'
	# ais.kystverket.no
	And the line to message adapter receives '\s:24,c:1567692878*35\!AIVDM,1,1,,B,13o`9@701j1Ej3vc;o3q@7SJ0D02,0*21'
	# ais.kystverket.no
	And the line to message adapter receives '\g:2-3-3451*5F\!AIVDM,3,2,9,A,<uH000167pF=2=nG0:0DRj0CQiC4jh00,0*4A'
	# ais.kystverket.no
	And the line to message adapter receives '\s:67,c:1567693000*34\!AIVDM,1,1,,,3CnWHf50000ga40TCHE0D0@R003B,0*4B'
	# ais.kystverket.no
	And the line to message adapter receives '\s:772,c:1567693246*07\!AIVDM,1,1,,,13o7g2001P0Lv<rSdVHf2h3N0000,0*25'
	# ais.kystverket.no
	And the line to message adapter receives '\g:2-2-8055*55\!AIVDM,2,2,6,B,j`888888880,2*2B'
	# ais.kystverket.no
	And the line to message adapter receives '\s:722,c:1567693372*04\!AIVDM,1,1,,A,13m63A?P00P`@GFTK3s>4?wR20Sf,0*71'
	# ais.kystverket.no
	And the line to message adapter receives '\g:3-3-3451*5E\!AIVDM,3,3,9,A,0000000,0*2F'
	# ais.kystverket.no
	And the line to message adapter receives '\s:808,c:1567693618*0A\!AIVDM,1,1,,B,B3o8B<00F8:0h694gOtbgwqUoP06,0*73'
	Then INmeaAisMessageStreamProcessor.OnNext should have been called 10 times
	# ais.kystverket.no
    And in ais message 0 the payload should be '33m9UtPP@50wwE:VJW6LS67H01<@' with padding of 0
    And in ais message 0 the source from the first NMEA line should be 42
    And in ais message 0 the timestamp from the first NMEA line should be 1567684904
	# ais.kystverket.no
    And in ais message 1 the payload should be 'B3m:H900AP@b:79ae6:<OwnUoP06' with padding of 0
    And in ais message 1 the source from the first NMEA line should be 42
    And in ais message 1 the timestamp from the first NMEA line should be 1567684904
	# ais.kystverket.no
    And in ais message 2 the payload should be '13m9WS001d0K==pR=D?HB6WD0pJV' with padding of 0
    And in ais message 2 the source from the first NMEA line should be 3
    And in ais message 2 the timestamp from the first NMEA line should be 1567692251
	# ais.kystverket.no
    And in ais message 3 the payload should be '13o`9@701j1Ej3vc;o3q@7SJ0D02' with padding of 0
    And in ais message 3 the source from the first NMEA line should be 24
    And in ais message 3 the timestamp from the first NMEA line should be 1567692878
	# ais.kystverket.no
    And in ais message 4 the payload should be '3CnWHf50000ga40TCHE0D0@R003B' with padding of 0
    And in ais message 4 the source from the first NMEA line should be 67
    And in ais message 4 the timestamp from the first NMEA line should be 1567693000
	# ais.kystverket.no
    And in ais message 5 the payload should be '13o7g2001P0Lv<rSdVHf2h3N0000' with padding of 0
    And in ais message 5 the source from the first NMEA line should be 772
    And in ais message 5 the timestamp from the first NMEA line should be 1567693246
	# ais.kystverket.no
    And in ais message 6 the payload should be '53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4lj`888888880' with padding of 2
    And in ais message 6 the source from the first NMEA line should be 99
    And in ais message 6 the timestamp from the first NMEA line should be 1567685556
	# ais.kystverket.no
    And in ais message 7 the payload should be '13m63A?P00P`@GFTK3s>4?wR20Sf' with padding of 0
    And in ais message 7 the source from the first NMEA line should be 722
    And in ais message 7 the timestamp from the first NMEA line should be 1567693372
	# ais.kystverket.no
    And in ais message 8 the payload should be '544MR0827oeaD<u0000lDdP4pTf0duAA<uH000167pF=2=nG0:0DRj0CQiC4jh000000000' with padding of 0
    And in ais message 8 the source from the first NMEA line should be 27
    And in ais message 8 the timestamp from the first NMEA line should be 1567686150
	# ais.kystverket.no
    And in ais message 9 the payload should be 'B3o8B<00F8:0h694gOtbgwqUoP06' with padding of 0
    And in ais message 9 the source from the first NMEA line should be 808
    And in ais message 9 the timestamp from the first NMEA line should be 1567693618

Scenario: Progress reports
	# This test interleaves more extensively than we ever see in reality, so we have to extend our reassembly window
	Given I have configured a MaximumUnmatchedFragmentAge of 10
	# ais.kystverket.no
	When the line to message adapter receives '\s:42,c:1567684904*38\!AIVDM,1,1,,B,33m9UtPP@50wwE:VJW6LS67H01<@,0*3C'
	# ais.kystverket.no
	And the line to message adapter receives '\g:1-3-3451,s:27,c:1567686150*40\!AIVDM,3,1,9,A,544MR0827oeaD<u0000lDdP4pTf0duAA,0*17'
	# ais.kystverket.no
	And the line to message adapter receives '\s:42,c:1567684904*38\!AIVDM,1,1,,A,B3m:H900AP@b:79ae6:<OwnUoP06,0*78'
	# ais.kystverket.no
	And the line to message adapter receives '\s:3,c:1567692251*01\!AIVDM,1,1,,A,13m9WS001d0K==pR=D?HB6WD0pJV,0*54,0*63'
	# ais.kystverket.no
	And the line to message adapter receives '\g:1-2-8055,s:99,c:1567685556*4E\!AIVDM,2,1,6,B,53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4l,0*72'
	# ais.kystverket.no
	And the line to message adapter receives '\s:24,c:1567692878*35\!AIVDM,1,1,,B,13o`9@701j1Ej3vc;o3q@7SJ0D02,0*07'
	# ais.kystverket.no
    And the line to message adapter receives a progress report of false, 6, 1234, 6, 1234
	# ais.kystverket.no
	And the line to message adapter receives '\g:2-3-3451*5F\!AIVDM,3,2,9,A,<uH000167pF=2=nG0:0DRj0CQiC4jh00,0*4A'
	# ais.kystverket.no
	And the line to message adapter receives '\s:67,c:1567693000*34\!AIVDM,1,1,,,3CnWHf50000ga40TCHE0D0@R003B,0*4B'
	# ais.kystverket.no
	And the line to message adapter receives '\s:772,c:1567693246*07\!AIVDM,1,1,,,13o7g2001P0Lv<rSdVHf2h3N0000,0*25'
	# ais.kystverket.no
	And the line to message adapter receives '\g:2-2-8055*55\!AIVDM,2,2,6,B,j`888888880,2*2B'
    And the line to message adapter receives a progress report of false, 10, 2345, 4, 1111
	# ais.kystverket.no
	And the line to message adapter receives '\s:722,c:1567693372*04\!AIVDM,1,1,,A,13m63A?P00P`@GFTK3s>4?wR20Sf,0*71'
	# ais.kystverket.no
	And the line to message adapter receives '\g:3-3-3451*5E\!AIVDM,3,3,9,A,0000000,0*2F'
	# ais.kystverket.no
	And the line to message adapter receives '\s:808,c:1567693618*0A\!AIVDM,1,1,,B,B3o8B<00F8:0h694gOtbgwqUoP06,0*73'
    And the line to message adapter receives a progress report of true, 13, 2445, 3, 100
	Then INmeaAisMessageStreamProcessor.Progress should have been called 3 times
    And progress report 0 was false, 6, 4, 1234, 6, 4, 1234
    And progress report 1 was false, 10, 7, 2345, 4, 3, 1111
    And progress report 2 was true, 13, 10, 2445, 3, 3, 100

Scenario: Line stream parser reports error in non-fragmented message
	When the line to message adapter receives an error report for content 'foobar' with line number 42
	Then INmeaAisMessageStreamProcessor.OnError should have been called 1 time
	And the message error report 0 should include the problematic line 'foobar'
	And the message error report 0 should include the exception reported by the line stream parser
	And the message error report 0 should include the line number 42

Scenario: Line stream parser reports error in first fragment of two-part message
	When the line to message adapter receives an error report for content 'foobar' with line number 42
	# ais.kystverket.no
	And the line to message adapter receives '\g:3-3-3451*5E\!AIVDM,3,3,9,A,0000000,0*2F'
	Then INmeaAisMessageStreamProcessor.OnError should have been called 1 time
	And the message error report 0 should include the problematic line 'foobar'
	And the message error report 0 should include the exception reported by the line stream parser
	And the message error report 0 should include the line number 42

Scenario: Line stream parser reports error in second fragment of two-part message
	When the line to message adapter receives '\g:1-2-8055,s:99,c:1567685556*4E\!AIVDM,2,1,6,B,53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4l,0*72'
	And the line to message adapter receives an error report for content 'foobar' with line number 42
	Then INmeaAisMessageStreamProcessor.OnError should have been called 1 time
	And the message error report 0 should include the problematic line 'foobar'
	And the message error report 0 should include the exception reported by the line stream parser
	And the message error report 0 should include the line number 42

Scenario: Line stream parser passes a message with padding that is the first of a two-part message
	# ais.kystverket.no
	When the line to message adapter receives '\g:1-2-8055,s:99,c:1567685556*4E\!AIVDM,2,1,6,B,53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4l,1*72'
	# ais.kystverket.no
	And the line to message adapter receives '\g:2-2-8055*55\!AIVDM,2,2,6,B,j`888888880,2*2B'
	Then INmeaAisMessageStreamProcessor.OnError should have been called 1 time
	# ais.kystverket.no
	And the message error report 0 should include the problematic line '\g:1-2-8055,s:99,c:1567685556*4E\!AIVDM,2,1,6,B,53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4l,1*72'
	And the message error report 0 should include an exception reporting unexpected padding on a non-terminal message fragment
	And the message error report 0 should include the line number 1

Scenario: Line stream parser passes the same sentence of a two-part message twice
	# ais.kystverket.no
	When the line to message adapter receives '\g:1-2-8055,s:99,c:1567685556*4E\!AIVDM,2,1,6,B,53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4l,0*72'
	# ais.kystverket.no
	And the line to message adapter receives '\g:1-2-8055*55\!AIVDM,2,1,6,B,j`888888880,0*2B'
	Then INmeaAisMessageStreamProcessor.OnError should have been called 1 time
	# ais.kystverket.no
	And the message error report 0 should include the problematic line '\g:1-2-8055*55\!AIVDM,2,1,6,B,j`888888880,0*2B'
	And the message error report 0 should include an exception reporting that it has received two message fragments with the same group id and position
	And the message error report 0 should include the line number 2

Scenario: Two-fragment message fragments received too many sentences in the middle
	Given I have configured a MaximumUnmatchedFragmentAge of 1
	# ais.kystverket.no
	When the line to message adapter receives '\g:1-2-8055,s:99,c:1567685556*4E\!AIVDM,2,1,6,B,53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4l,0*72'
	# ais.kystverket.no
	And the line to message adapter receives '\s:42,c:1567684904*38\!AIVDM,1,1,,B,33m9UtPP@50wwE:VJW6LS67H01<@,0*3C'
	# ais.kystverket.no
	And the line to message adapter receives '\s:3,c:1567692251*01\!AIVDM,1,1,,A,13m9WS001d0K==pR=D?HB6WD0pJV,0*54,0*63'
	# ais.kystverket.no
	And the line to message adapter receives '\g:2-2-8055*55\!AIVDM,2,2,6,B,j`888888880,2*2B'
	Then INmeaAisMessageStreamProcessor.OnNext should have been called 2 times
	Then INmeaAisMessageStreamProcessor.OnError should have been called 1 time
	# ais.kystverket.no
    And in ais message 0 the payload should be '33m9UtPP@50wwE:VJW6LS67H01<@' with padding of 0
    And in ais message 0 the source from the first NMEA line should be 42
    And in ais message 0 the timestamp from the first NMEA line should be 1567684904
	# ais.kystverket.no
    And in ais message 1 the payload should be '13m9WS001d0K==pR=D?HB6WD0pJV' with padding of 0
    And in ais message 1 the source from the first NMEA line should be 3
    And in ais message 1 the timestamp from the first NMEA line should be 1567692251
	# ais.kystverket.no
	And the message error report 0 should include the problematic line '\g:1-2-8055,s:99,c:1567685556*4E\!AIVDM,2,1,6,B,53oGfN42=WRDhlHn221<4i@Dr22222222222220`1@O6640Ht50Skp4mR`4l,0*72'
	And the message error report 0 should include an exception reporting that it received an incomplete set of fragments for a message
	And the message error report 0 should include the line number 1

Scenario: Tree-fragment message with only one sentence and auto fix group
  Given I have configured a empty group tolerance of 2
	When the line to message adapter receives '\g:1-3-1481,s:99,c:1718701847*41\!AIVDM,1,1,,A,13Ef?<3006wt9WlEMhi1S6uJ00Rq,0*41'
	And the line to message adapter receives '\g:2-3-1481*50\'
	And the line to message adapter receives '\g:3-3-1481*51\'
	Then INmeaAisMessageStreamProcessor.OnNext should have been called 1 times
    And in ais message 0 the payload should be '13Ef?<3006wt9WlEMhi1S6uJ00Rq' with padding of 0
    And in ais message 0 the source from the first NMEA line should be 99
    And in ais message 0 the timestamp from the first NMEA line should be 1718701847
    And in ais message 0 the isfixedmessage from the first NMEA line should be true
    And in ais message 0 the sentencesingroup from the first NMEA line should be 0

Scenario: Tree-fragment message with two sentences and auto fix group
  Given I have configured a empty group tolerance of 2
	When the line to message adapter receives '\g:1-3-1481,s:99,c:1718701847*41\!AIVDM,2,1,8,A,13Ef?<3006wt9WlEMhi1S6uJ00Rq,0*7A'
	And the line to message adapter receives '\g:2-3-1481*50\!AIVDM,2,2,8,B,T`0@n888880,2*3D'
	And the line to message adapter receives '\g:3-3-1481*51\'
	Then INmeaAisMessageStreamProcessor.OnNext should have been called 1 times
    And in ais message 0 the payload should be '13Ef?<3006wt9WlEMhi1S6uJ00RqT`0@n888880' with padding of 2
    And in ais message 0 the source from the first NMEA line should be 99
    And in ais message 0 the timestamp from the first NMEA line should be 1718701847
    And in ais message 0 the isfixedmessage from the first NMEA line should be true
    And in ais message 0 the sentencesingroup from the first NMEA line should be 2

Scenario: Tree-fragment message with only one sentence and allow empty sentence
	Given I have configured a empty group tolerance of 1
	When the line to message adapter receives '\g:1-3-1481,s:99,c:1718701847*41\!AIVDM,1,1,,A,13Ef?<3006wt9WlEMhi1S6uJ00Rq,0*41'
	And the line to message adapter receives '\g:2-3-1481*50\'
	And the line to message adapter receives '\g:3-3-1481*51\'
	Then INmeaAisMessageStreamProcessor.OnNext should have been called 1 times
	And in ais message 0 the payload should be '13Ef?<3006wt9WlEMhi1S6uJ00Rq' with padding of 0
	And in ais message 0 the source from the first NMEA line should be 99
	And in ais message 0 the timestamp from the first NMEA line should be 1718701847
	And in ais message 0 the isfixedmessage from the first NMEA line should be false
	And in ais message 0 the sentencesingroup from the first NMEA line should be 3

Scenario: Tree-fragment message with only one sentence and empty sentence is not allowed
	Given I have configured a empty group tolerance of 0
	When the line to message adapter receives '\g:1-3-1481,s:99,c:1718701847*41\!AIVDM,1,1,,A,13Ef?<3006wt9WlEMhi1S6uJ00Rq,0*41'
	And the line to message adapter receives an error report for invalid content '\g:2-3-1481*50\' with line number 2
	And the line to message adapter receives an error report for invalid content '\g:3-3-1481*51\' with line number 3
	# And a line '\g:1-3-1481,s:99,c:1718701847*41\!AIVDM,1,1,,A,13Ef?<3006wt9WlEMhi1S6uJ00Rq,0*41'
	# And a line '\g:2-3-1481*50\'
	# And a line '\g:3-3-1481*51\'
	# When I parse the content by message
	Then INmeaAisMessageStreamProcessor.OnNext should have been called 0 time
	And INmeaAisMessageStreamProcessor.OnError should have been called 2 times
	And the message error report 0 should include the problematic line '\g:2-3-1481*50\'
	And the message error report 0 should include an exception reporting that the message appears to be missing some characters
	And the message error report 1 should include the problematic line '\g:3-3-1481*51\'
	And the message error report 1 should include an exception reporting that the message appears to be missing some characters

Scenario: Multiple lines with non-zero padding in non last fragment disallowed
	Given I have configured a AllowNonZeroPaddingInNonLastFragment of false
	When the line to message adapter receives '\c:1717977721*54\!AIVDM,2,1,2,B,53P7hnD00000LQLF220<P4hhDpLF22200000000N1h4245Ra001RDj2CQp1l,2*15'
	And the line to message adapter receives '\c:1717977721*54\!AIVDM,2,2,2,B,SmCQ4p00000,2*4D'
	Then INmeaAisMessageStreamProcessor.OnNext should have been called 0 times
	And INmeaAisMessageStreamProcessor.OnError should have been called 1 time
	And the message error report 0 should include the problematic line '\c:1717977721*54\!AIVDM,2,1,2,B,53P7hnD00000LQLF220<P4hhDpLF22200000000N1h4245Ra001RDj2CQp1l,2*15'
	And the message error report 0 should include an exception reporting unexpected padding on a non-terminal message fragment

Scenario: Multiple lines with non-zero padding in non last fragment allowed
	Given I have configured a AllowNonZeroPaddingInNonLastFragment of true
	When the line to message adapter receives '\c:1717977721*54\!AIVDM,2,1,2,B,53P7hnD00000LQLF220<P4hhDpLF22200000000N1h4245Ra001RDj2CQp1l,2*15'
	And the line to message adapter receives '\c:1717977721*54\!AIVDM,2,2,2,B,SmCQ4p00000,2*4D'
	Then INmeaAisMessageStreamProcessor.OnNext should have been called 1 times
	And in ais message 0 the payload should be '53P7hnD00000LQLF220<P4hhDpLF22200000000N1h4245Ra001RDj2CQp1lSmCQ4p00000' with padding of 2
	And INmeaAisMessageStreamProcessor.OnError should have been called 0 time

Scenario: Sentence without exclamation mark is disallowed
	Given I have configured a ThrowWhenNoExclamationMark of true
	When the line to message adapter receives an error report for invalid content '\c:1717977721*54\$AIVDM,2,1,2,B,53P7hnD00000LQLF220<P4hhDpLF22200000000N1h4245Ra001RDj2CQp1l,2*15' with line number 1
	Then INmeaAisMessageStreamProcessor.OnNext should have been called 0 time
	And INmeaAisMessageStreamProcessor.OnError should have been called 1 time
	And the message error report 0 should include the problematic line '\c:1717977721*54\$AIVDM,2,1,2,B,53P7hnD00000LQLF220<P4hhDpLF22200000000N1h4245Ra001RDj2CQp1l,2*15'
	And the message error report 0 should include an exception reporting that the expected exclamation mark is missing

Scenario: Sentence without exclamation mark is allowed
	Given I have configured a ThrowWhenNoExclamationMark of false
	When the line to message adapter receives '\s:808,c:1567693618*0A\$AIVDM,1,1,,B,B3o8B<00F8:0h694gOtbgwqUoP06,0*73'
	Then INmeaAisMessageStreamProcessor.OnNext should have been called 1 time
	And INmeaAisMessageStreamProcessor.OnError should have been called 0 time

# TODO:
# Got to end with unclosed fragments (#4067)
# 2nd fragment received without first (#4067)
# 2nd fragment received then first (#4067)
# First fragment has non-zero padding (#4066)
