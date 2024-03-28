namespace Ais.Net.Specs.AisMessageTypes
{
    using System.Linq;
    using System.Text;
    using NUnit.Framework;
    using TechTalk.SpecFlow;

    [Binding]
    public class DifferentialCorrectionDataParserSpecsSteps
    {
        private ParserMaker makeParser;

        private delegate NmeaAisDifferentialCorrectionDataParser ParserMaker();

        private delegate void ParserTest(NmeaAisDifferentialCorrectionDataParser parser);

        [When("I parse '(.*)' with padding (.*) as a Differential Correction Data")]
        public void WhenIParseWithNmeaAisDifferentialCorrectionDataParser(string payload, uint padding)
        {
            this.When(() =>
            {
                var parser = new NmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser(Encoding.ASCII.GetBytes(payload), padding);
                return new NmeaAisDifferentialCorrectionDataParser(parser.DifferentialCorrectionData, parser.DifferentialCorrectionDataPadding, padding);
            });
        }

        [Then(@"NmeaAisDifferentialCorrectionDataParser\.MessageType is (.*)")]
        public void ThenNmeaAisDifferentialCorrectionDataParser_TypeIs(uint messageType)
        {
            this.Then(parser => Assert.AreEqual(messageType, parser.MessageType));
        }

        [Then(@"NmeaAisDifferentialCorrectionDataParser\.Station is (.*)")]
        public void ThenNmeaAisDifferentialCorrectionDataParser_StationIs(uint value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.Station));
        }

        [Then(@"NmeaAisDifferentialCorrectionDataParser\.ZCount is (.*)")]
        public void ThenNmeaAisDifferentialCorrectionDataParser_ZCountIs(uint value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.ZCount));
        }

        [Then(@"NmeaAisDifferentialCorrectionDataParser\.SequenceNumber is (.*)")]
        public void ThenNmeaAisDifferentialCorrectionDataParser_SequenceNumberIs(uint value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.SequenceNumber));
        }

        [Then(@"NmeaAisDifferentialCorrectionDataParser\.DgnssDataWordCount is (.*)")]
        public void ThenNmeaAisDifferentialCorrectionDataParser_DgnssDataWordCountIs(uint value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.DgnssDataWordCount));
        }

        [Then(@"NmeaAisDifferentialCorrectionDataParser\.Health is (.*)")]
        public void ThenNmeaAisDifferentialCorrectionDataParser_HealthIs(uint value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.Health));
        }

        [Then(@"NmeaAisDifferentialCorrectionDataParser\.WriteDgnssDataWord is (.*)")]
        public void ThenNmeaAisDifferentialCorrectionDataParser_Is(uint[] value)
        {
            this.Then(parser =>
            {
                uint[] list = new uint[parser.DgnssDataWordCount];
                parser.WriteDgnssDataWord(list);
                Assert.AreEqual(value, list);
            });
        }

        [StepArgumentTransformation]
        public uint[] TransformToUIntArray(string commaSeparated)
        {
            return commaSeparated.Split(',')
                                 .Select(uint.Parse)
                                 .ToArray();
        }

        private void When(ParserMaker makeParser)
        {
            this.makeParser = makeParser;
        }

        private void Then(ParserTest test)
        {
            NmeaAisDifferentialCorrectionDataParser parser = this.makeParser();
            test(parser);
        }
    }
}
