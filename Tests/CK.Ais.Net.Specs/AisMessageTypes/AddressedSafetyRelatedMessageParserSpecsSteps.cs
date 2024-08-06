using NUnit.Framework;
using System;
using System.Text;
using TechTalk.SpecFlow;

namespace Ais.Net.Specs.AisMessageTypes
{
    [Binding]
    public class AddressedSafetyRelatedMessageParserSpecsSteps
    {
        ParserMaker? _makeParser;

        delegate NmeaAisAddressedSafetyRelatedMessageParser ParserMaker();

        delegate void ParserTest( NmeaAisAddressedSafetyRelatedMessageParser parser );

        [When( "I parse '(.*)' with padding (.*) as a Addressed Safety Related Message" )]
        public void WhenIParseWithPaddingAsALongRangeAisBroadcast( string payload, uint padding )
        {
            When( () => new NmeaAisAddressedSafetyRelatedMessageParser( Encoding.ASCII.GetBytes( payload ), padding ) );
        }

        [Then( @"NmeaAisAddressedSafetyRelatedMessageParser\.Type is (.*)" )]
        public void ThenNmeaAisAddressedSafetyRelatedMessageParser_TypeIs( MessageType messageType )
        {
            Then( parser => Assert.AreEqual( messageType, parser.MessageType ) );
        }

        [Then( @"NmeaAisAddressedSafetyRelatedMessageParser\.RepeatIndicator is (.*)" )]
        public void ThenNmeaAisAddressedSafetyRelatedMessageParser_RepeatIndicatorIs( int repeatCount )
        {
            Then( parser => Assert.AreEqual( repeatCount, parser.RepeatIndicator ) );
        }

        [Then( @"NmeaAisAddressedSafetyRelatedMessageParser\.Mmsi is (.*)" )]
        public void ThenNmeaAisAddressedSafetyRelatedMessageParser_MmsiIs( int mmsi )
        {
            Then( parser => Assert.AreEqual( mmsi, parser.Mmsi ) );
        }

        [Then( @"NmeaAisAddressedSafetyRelatedMessageParser\.SequenceNumber is (.*)" )]
        public void ThenNmeaAisAddressedSafetyRelatedMessageParser_SequenceNumberIs( int sequenceNumber )
        {
            Then( parser => Assert.AreEqual( sequenceNumber, parser.SequenceNumber ) );
        }

        [Then( @"NmeaAisAddressedSafetyRelatedMessageParser\.DestinationMmsi is (.*)" )]
        public void ThenNmeaAisAddressedSafetyRelatedMessageParser_DestinationMmsiIs( int destinationMmsi )
        {
            Then( parser => Assert.AreEqual( destinationMmsi, parser.DestinationMmsi ) );
        }

        [Then( @"NmeaAisAddressedSafetyRelatedMessageParser\.Retransmit is (.*)" )]
        public void ThenNmeaAisAddressedSafetyRelatedMessageParser_RetransmitIs( bool retransmit )
        {
            Then( parser => Assert.AreEqual( retransmit, parser.Retransmit ) );
        }

        [Then( @"NmeaAisAddressedSafetyRelatedMessageParser\.SpareBit71 is (.*)" )]
        public void ThenNmeaAisAddressedSafetyRelatedMessageParser_SpareBit71Is( bool spareBit71 )
        {
            Then( parser => Assert.AreEqual( spareBit71, parser.SpareBit71 ) );
        }

        [Then( @"NmeaAisAddressedSafetyRelatedMessageParser\.SafetyRelatedText is (.*)" )]
        public void ThenNmeaAisAddressedSafetyRelatedMessageParser_SafetyRelatedTextIs( string safetyRelatedText )
        {
            Then( parser =>
            {
                byte[] text = new byte[parser.SafetyRelatedText.CharacterCount];
                parser.SafetyRelatedText.WriteAsAscii( text );
                Assert.AreEqual( safetyRelatedText.Trim( '"' ), text );
            } );
        }

        void When( ParserMaker makeParser )
        {
            _makeParser = makeParser;
        }

        void Then( ParserTest test )
        {
            if( _makeParser is null ) throw new InvalidOperationException( $"When step must be called." );
            NmeaAisAddressedSafetyRelatedMessageParser parser = _makeParser();
            test( parser );
        }
    }
}
