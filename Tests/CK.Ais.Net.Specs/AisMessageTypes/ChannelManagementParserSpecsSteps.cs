using NUnit.Framework;
using System;
using System.Text;
using TechTalk.SpecFlow;

namespace Ais.Net.Specs.AisMessageTypes
{
    [Binding]
    public class ChannelManagementParserSpecsSteps
    {
        ParserMaker? _makeParser;

        delegate NmeaAisChannelManagementParser ParserMaker();

        delegate void ParserTest( NmeaAisChannelManagementParser parser );

        [When( "I parse '(.*)' with padding (.*) as a Channel management" )]
        public void WhenIParseWithNmeaAisChannelManagementParser( string payload, uint padding )
        {
            When( () => new NmeaAisChannelManagementParser( Encoding.ASCII.GetBytes( payload ), padding ) );
        }

        [Then( @"NmeaAisChannelManagementParser\.Type is (.*)" )]
        public void ThenNmeaAisChannelManagementParser_TypeIs( MessageType messageType )
        {
            Then( parser => Assert.AreEqual( messageType, parser.MessageType ) );
        }

        [Then( @"NmeaAisChannelManagementParser\.RepeatIndicator is (.*)" )]
        public void ThenNmeaAisChannelManagementParser_RepeatIndicatorIs( uint repeatCount )
        {
            Then( parser => Assert.AreEqual( repeatCount, parser.RepeatIndicator ) );
        }

        [Then( @"NmeaAisChannelManagementParser\.Mmsi is (.*)" )]
        public void ThenNmeaAisChannelManagementParser_MmsiIs( uint mmsi )
        {
            Then( parser => Assert.AreEqual( mmsi, parser.Mmsi ) );
        }

        [Then( @"NmeaAisChannelManagementParser\.SpareBits38 is (.*)" )]
        public void ThenNmeaAisChannelManagementParser_SpareBits38Is( uint value )
        {
            Then( parser => Assert.AreEqual( value, parser.SpareBits38 ) );
        }

        [Then( @"NmeaAisChannelManagementParser\.ChannelA is (.*)" )]
        public void ThenNmeaAisChannelManagementParser_ChannelAIs( uint value )
        {
            Then( parser => Assert.AreEqual( value, parser.ChannelA ) );
        }

        [Then( @"NmeaAisChannelManagementParser\.ChannelB is (.*)" )]
        public void ThenNmeaAisChannelManagementParser_ChannelBIs( uint value )
        {
            Then( parser => Assert.AreEqual( value, parser.ChannelB ) );
        }

        [Then( @"NmeaAisChannelManagementParser\.TxRxMode is (.*)" )]
        public void ThenNmeaAisChannelManagementParser_TxRxModeIs( uint value )
        {
            Then( parser => Assert.AreEqual( value, parser.TxRxMode ) );
        }

        [Then( @"NmeaAisChannelManagementParser\.LowPower is (.*)" )]
        public void ThenNmeaAisChannelManagementParser_LowPowerIs( bool value )
        {
            Then( parser => Assert.AreEqual( value, parser.LowPower ) );
        }

        [Then( @"NmeaAisChannelManagementParser\.Longitude10thMins1 is (.*)" )]
        public void ThenNmeaAisChannelManagementParser_Longitude10thMins1Is( int value )
        {
            Then( parser => Assert.AreEqual( value, parser.Longitude10thMins1 ) );
        }

        [Then( @"NmeaAisChannelManagementParser\.Latitude10thMins1 is (.*)" )]
        public void ThenNmeaAisChannelManagementParser_Latitude10thMins1Is( int value )
        {
            Then( parser => Assert.AreEqual( value, parser.Latitude10thMins1 ) );
        }

        [Then( @"NmeaAisChannelManagementParser\.Longitude10thMins2 is (.*)" )]
        public void ThenNmeaAisChannelManagementParser_Longitude10thMins2Is( int value )
        {
            Then( parser => Assert.AreEqual( value, parser.Longitude10thMins2 ) );
        }

        [Then( @"NmeaAisChannelManagementParser\.Latitude10thMins2 is (.*)" )]
        public void ThenNmeaAisChannelManagementParser_Latitude10thMins2Is( int value )
        {
            Then( parser => Assert.AreEqual( value, parser.Latitude10thMins2 ) );
        }

        [Then( @"NmeaAisChannelManagementParser\.DestinationIndicator is (.*)" )]
        public void ThenNmeaAisChannelManagementParser_DestinationIndicatorIs( DestinationIndicator value )
        {
            Then( parser => Assert.AreEqual( value, parser.DestinationIndicator ) );
        }

        [Then( @"NmeaAisChannelManagementParser\.ChannelABandwidth is (.*)" )]
        public void ThenNmeaAisChannelManagementParser_ChannelABandwidthIs( bool value )
        {
            Then( parser => Assert.AreEqual( value, parser.ChannelABandwidth ) );
        }

        [Then( @"NmeaAisChannelManagementParser\.ChannelBBandwidth is (.*)" )]
        public void ThenNmeaAisChannelManagementParser_ChannelBBandwidthIs( bool value )
        {
            Then( parser => Assert.AreEqual( value, parser.ChannelBBandwidth ) );
        }

        [Then( @"NmeaAisChannelManagementParser\.TransitionalZoneSize is (.*)" )]
        public void ThenNmeaAisChannelManagementParser_TransitionalZoneSizeIs( uint value )
        {
            Then( parser => Assert.AreEqual( value, parser.TransitionalZoneSize ) );
        }

        [Then( @"NmeaAisChannelManagementParser\.SpareBits145 is (.*)" )]
        public void ThenNmeaAisChannelManagementParser_SpareBits145Is( uint value )
        {
            Then( parser => Assert.AreEqual( value, parser.SpareBits145 ) );
        }

        void When( ParserMaker makeParser )
        {
            _makeParser = makeParser;
        }

        void Then( ParserTest test )
        {
            if( _makeParser is null ) throw new InvalidOperationException( $"When step must be called." );
            NmeaAisChannelManagementParser parser = _makeParser();
            test( parser );
        }
    }
}
