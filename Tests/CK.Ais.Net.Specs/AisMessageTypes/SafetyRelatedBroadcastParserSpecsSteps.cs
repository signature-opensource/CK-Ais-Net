using NUnit.Framework;
using System;
using System.Text;
using TechTalk.SpecFlow;

namespace Ais.Net.Specs.AisMessageTypes
{
    [Binding]
    public class SafetyRelatedBroadcastParserSpecsSteps
    {
        ParserMaker? _makeParser;

        delegate NmeaAisSafetyRelatedBroadcastParser ParserMaker();

        delegate void ParserTest( NmeaAisSafetyRelatedBroadcastParser parser );

        [When( "I parse '(.*)' with padding (.*) as a Safety Related Broadcast" )]
        public void WhenIParseWithPaddingAsALongRangeAisBroadcast( string payload, uint padding )
        {
            When( () => new NmeaAisSafetyRelatedBroadcastParser( Encoding.ASCII.GetBytes( payload ), padding ) );
        }

        [Then( @"NmeaAisSafetyRelatedBroadcastParser\.Type is (.*)" )]
        public void ThenNmeaAisSafetyRelatedBroadcastParser_TypeIs( MessageType messageType )
        {
            Then( parser => Assert.AreEqual( messageType, parser.MessageType ) );
        }

        [Then( @"NmeaAisSafetyRelatedBroadcastParser\.RepeatIndicator is (.*)" )]
        public void ThenNmeaAisSafetyRelatedBroadcastParser_RepeatIndicatorIs( int repeatCount )
        {
            Then( parser => Assert.AreEqual( repeatCount, parser.RepeatIndicator ) );
        }

        [Then( @"NmeaAisSafetyRelatedBroadcastParser\.Mmsi is (.*)" )]
        public void ThenNmeaAisSafetyRelatedBroadcastParser_MmsiIs( int mmsi )
        {
            Then( parser => Assert.AreEqual( mmsi, parser.Mmsi ) );
        }

        [Then( @"NmeaAisSafetyRelatedBroadcastParser\.SpareBit38 is (.*)" )]
        public void ThenNmeaAisSafetyRelatedBroadcastParser_SpareBit38Is( uint spareBit38 )
        {
            Then( parser => Assert.AreEqual( spareBit38, parser.SpareBits38 ) );
        }

        [Then( @"NmeaAisSafetyRelatedBroadcastParser\.SafetyRelatedText is (.*)" )]
        public void ThenNmeaAisSafetyRelatedBroadcastParser_SafetyRelatedTextIs( string safetyRelatedText )
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
            NmeaAisSafetyRelatedBroadcastParser parser = _makeParser();
            test( parser );
        }
    }
}
