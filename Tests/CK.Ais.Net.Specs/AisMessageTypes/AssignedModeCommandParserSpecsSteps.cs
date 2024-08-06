using NUnit.Framework;
using System;
using System.Text;
using TechTalk.SpecFlow;

namespace Ais.Net.Specs.AisMessageTypes
{
    [Binding]
    public class AssignedModeCommandParserSpecsSteps
    {
        ParserMaker? _makeParser;

        delegate NmeaAisAssignedModeCommandParser ParserMaker();

        delegate void ParserTest( NmeaAisAssignedModeCommandParser parser );

        [When( "I parse '(.*)' with padding (.*) as a Assigned mode command" )]
        public void WhenIParseWithNmeaAisAssignedModeCommandParser( string payload, uint padding )
        {
            When( () => new NmeaAisAssignedModeCommandParser( Encoding.ASCII.GetBytes( payload ), padding ) );
        }

        [Then( @"NmeaAisAssignedModeCommandParser\.Type is (.*)" )]
        public void ThenNmeaAisAssignedModeCommandParser_TypeIs( MessageType messageType )
        {
            Then( parser => Assert.AreEqual( messageType, parser.MessageType ) );
        }

        [Then( @"NmeaAisAssignedModeCommandParser\.RepeatIndicator is (.*)" )]
        public void ThenNmeaAisAssignedModeCommandParser_RepeatIndicatorIs( uint repeatCount )
        {
            Then( parser => Assert.AreEqual( repeatCount, parser.RepeatIndicator ) );
        }

        [Then( @"NmeaAisAssignedModeCommandParser\.Mmsi is (.*)" )]
        public void ThenNmeaAisAssignedModeCommandParser_MmsiIs( uint mmsi )
        {
            Then( parser => Assert.AreEqual( mmsi, parser.Mmsi ) );
        }

        [Then( @"NmeaAisAssignedModeCommandParser\.SpareBits38 is (.*)" )]
        public void ThenNmeaAisAssignedModeCommandParser_SpareBits38Is( uint value )
        {
            Then( parser => Assert.AreEqual( value, parser.SpareBits38 ) );
        }

        [Then( @"NmeaAisAssignedModeCommandParser\.DestinationMmsiA is (.*)" )]
        public void ThenNmeaAisAssignedModeCommandParser_DestinationMmsiAIs( uint value )
        {
            Then( parser => Assert.AreEqual( value, parser.DestinationMmsiA ) );
        }

        [Then( @"NmeaAisAssignedModeCommandParser\.OffsetA is (.*)" )]
        public void ThenNmeaAisAssignedModeCommandParser_OffsetAIs( uint value )
        {
            Then( parser => Assert.AreEqual( value, parser.OffsetA ) );
        }

        [Then( @"NmeaAisAssignedModeCommandParser\.IncrementA is (.*)" )]
        public void ThenNmeaAisAssignedModeCommandParser_IncrementAIs( uint value )
        {
            Then( parser => Assert.AreEqual( value, parser.IncrementA ) );
        }

        [Then( @"NmeaAisAssignedModeCommandParser\.DestinationMmsiB is (.*)" )]
        public void ThenNmeaAisAssignedModeCommandParser_DestinationMmsiBIs( uint? value )
        {
            Then( parser => Assert.AreEqual( value, parser.DestinationMmsiB ) );
        }

        [Then( @"NmeaAisAssignedModeCommandParser\.OffsetB is (.*)" )]
        public void ThenNmeaAisAssignedModeCommandParser_OffsetBIs( uint? value )
        {
            Then( parser => Assert.AreEqual( value, parser.OffsetB ) );
        }

        [Then( @"NmeaAisAssignedModeCommandParser\.IncrementB is (.*)" )]
        public void ThenNmeaAisAssignedModeCommandParser_IncrementBIs( uint? value )
        {
            Then( parser => Assert.AreEqual( value, parser.IncrementB ) );
        }

        [Then( @"NmeaAisAssignedModeCommandParser\.SpareBitsAtEnd is (.*)" )]
        public void ThenNmeaAisAssignedModeCommandParser_SpareBitsAtEndAIs( uint? value )
        {
            Then( parser => Assert.AreEqual( value, parser.SpareBitsAtEnd ) );
        }

        void When( ParserMaker makeParser )
        {
            _makeParser = makeParser;
        }

        void Then( ParserTest test )
        {
            if( _makeParser is null ) throw new InvalidOperationException( $"When step must be called." );
            NmeaAisAssignedModeCommandParser parser = _makeParser();
            test( parser );
        }
    }
}
