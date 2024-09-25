using NUnit.Framework;
using System;
using System.Text;
using TechTalk.SpecFlow;

namespace Ais.Net.Specs.AisMessageTypes
{
    [Binding]
    public class BinaryBroadcastMessageParserSpecsSteps
    {
        ParserMaker? _makeParser;

        delegate NmeaAisBinaryBroadcastMessageParser ParserMaker();

        delegate void ParserTest( NmeaAisBinaryBroadcastMessageParser parser );

        [When( "I parse '(.*)' with padding (.*) as a Binary Broadcast Message" )]
        public void WhenIParseWithNmeaAisBinaryBroadcastMessageParser( string payload, uint padding )
        {
            When( () => new NmeaAisBinaryBroadcastMessageParser( Encoding.ASCII.GetBytes( payload ), padding ) );
        }

        [Then( @"NmeaAisBinaryBroadcastMessageParser\.Type is (.*)" )]
        public void ThenNmeaAisBinaryBroadcastMessageParser_TypeIs( MessageType messageType )
        {
            Then( parser => Assert.AreEqual( messageType, parser.MessageType ) );
        }

        [Then( @"NmeaAisBinaryBroadcastMessageParser\.RepeatIndicator is (.*)" )]
        public void ThenNmeaAisBinaryBroadcastMessageParser_RepeatIndicatorIs( uint repeatCount )
        {
            Then( parser => Assert.AreEqual( repeatCount, parser.RepeatIndicator ) );
        }

        [Then( @"NmeaAisBinaryBroadcastMessageParser\.Mmsi is (.*)" )]
        public void ThenNmeaAisBinaryBroadcastMessageParser_MmsiIs( uint mmsi )
        {
            Then( parser => Assert.AreEqual( mmsi, parser.Mmsi ) );
        }

        [Then( @"NmeaAisBinaryBroadcastMessageParser\.SpareBits38 is (.*)" )]
        public void ThenNmeaAisBinaryBroadcastMessageParser_SpareBits38Is( uint value )
        {
            Then( parser => Assert.AreEqual( value, parser.SpareBits38 ) );
        }

        [Then( @"NmeaAisBinaryBroadcastMessageParser\.DAC is (.*)" )]
        public void ThenNmeaAisBinaryBroadcastMessageParser_DACIs( uint value )
        {
            Then( parser => Assert.AreEqual( value, parser.DAC ) );
        }

        [Then( @"NmeaAisBinaryBroadcastMessageParser\.FI is (.*)" )]
        public void ThenNmeaAisBinaryBroadcastMessageParser_FIIs( uint value )
        {
            Then( parser => Assert.AreEqual( value, parser.FI ) );
        }

        [Then( @"NmeaAisBinaryBroadcastMessageParser\.ApplicationDataPadding is (.*)" )]
        public void ThenNmeaAisBinaryBroadcastMessageParser_ApplicationDataPaddingIs( uint value )
        {
            Then( parser => Assert.AreEqual( value, parser.ApplicationDataPaddingBefore ) );
        }

        [Then( @"NmeaAisBinaryBroadcastMessageParser\.ApplicationData is (.*)" )]
        public void ThenNmeaAisBinaryBroadcastMessageParser_ApplicationDataIs( string value )
        {
            Then( parser => Assert.AreEqual( value, Encoding.ASCII.GetString( parser.ApplicationData ) ) );
        }

        void When( ParserMaker makeParser )
        {
            _makeParser = makeParser;
        }

        void Then( ParserTest test )
        {
            if( _makeParser is null ) throw new InvalidOperationException( $"When step must be called." );
            NmeaAisBinaryBroadcastMessageParser parser = _makeParser();
            test( parser );
        }
    }
}
