using NUnit.Framework;
using System;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow;

namespace Ais.Net.Specs.AisMessageTypes
{
    [Binding]
    public class DifferentialCorrectionDataParserSpecsSteps
    {
        ParserMaker? _makeParser;

        delegate NmeaAisDifferentialCorrectionDataParser ParserMaker();

        delegate void ParserTest( NmeaAisDifferentialCorrectionDataParser parser );

        [When( "I parse '(.*)' with padding (.*) as a Differential Correction Data" )]
        public void WhenIParseWithNmeaAisDifferentialCorrectionDataParser( string payload, uint padding )
        {
            When( () =>
            {
                var parser = new NmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser( Encoding.ASCII.GetBytes( payload ), padding );
                return new NmeaAisDifferentialCorrectionDataParser( parser.DifferentialCorrectionData, parser.DifferentialCorrectionDataPaddingBefore, padding );
            } );
        }

        [Then( @"NmeaAisDifferentialCorrectionDataParser\.MessageType is (.*)" )]
        public void ThenNmeaAisDifferentialCorrectionDataParser_TypeIs( MessageType messageType )
        {
            Then( parser => Assert.AreEqual( messageType, parser.MessageType ) );
        }

        [Then( @"NmeaAisDifferentialCorrectionDataParser\.Station is (.*)" )]
        public void ThenNmeaAisDifferentialCorrectionDataParser_StationIs( uint value )
        {
            Then( parser => Assert.AreEqual( value, parser.Station ) );
        }

        [Then( @"NmeaAisDifferentialCorrectionDataParser\.ZCount is (.*)" )]
        public void ThenNmeaAisDifferentialCorrectionDataParser_ZCountIs( uint value )
        {
            Then( parser => Assert.AreEqual( value, parser.ZCount ) );
        }

        [Then( @"NmeaAisDifferentialCorrectionDataParser\.SequenceNumber is (.*)" )]
        public void ThenNmeaAisDifferentialCorrectionDataParser_SequenceNumberIs( uint value )
        {
            Then( parser => Assert.AreEqual( value, parser.SequenceNumber ) );
        }

        [Then( @"NmeaAisDifferentialCorrectionDataParser\.DgnssDataWordCount is (.*)" )]
        public void ThenNmeaAisDifferentialCorrectionDataParser_DgnssDataWordCountIs( uint value )
        {
            Then( parser => Assert.AreEqual( value, parser.DgnssDataWordCount ) );
        }

        [Then( @"NmeaAisDifferentialCorrectionDataParser\.Health is (.*)" )]
        public void ThenNmeaAisDifferentialCorrectionDataParser_HealthIs( uint value )
        {
            Then( parser => Assert.AreEqual( value, parser.Health ) );
        }

        [Then( @"NmeaAisDifferentialCorrectionDataParser\.WriteDgnssDataWord is (.*)" )]
        public void ThenNmeaAisDifferentialCorrectionDataParser_Is( uint[] value )
        {
            Then( parser =>
            {
                uint[] list = new uint[parser.DgnssDataWordCount];
                parser.WriteDgnssDataWord( list );
                Assert.AreEqual( value, list );
            } );
        }

        [StepArgumentTransformation]
        public static uint[] TransformToUIntArray( string commaSeparated )
        {
            return commaSeparated.Split( ',' )
                                 .Select( uint.Parse )
                                 .ToArray();
        }

        void When( ParserMaker makeParser )
        {
            _makeParser = makeParser;
        }

        void Then( ParserTest test )
        {
            if( _makeParser is null ) throw new InvalidOperationException( $"When step must be called not called." );
            NmeaAisDifferentialCorrectionDataParser parser = _makeParser();
            test( parser );
        }
    }
}
