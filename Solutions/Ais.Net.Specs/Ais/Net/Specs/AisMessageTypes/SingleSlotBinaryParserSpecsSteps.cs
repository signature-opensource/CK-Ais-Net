namespace Ais.Net.Specs.AisMessageTypes
{
    using System.Text;
    using NUnit.Framework;
    using TechTalk.SpecFlow;

    [Binding]
    public class SingleSlotBinaryParserSpecsSteps
    {
        private ParserMaker makeParser;

        private delegate NmeaAisSingleSlotBinaryParser ParserMaker();

        private delegate void ParserTest(NmeaAisSingleSlotBinaryParser parser);

        [When("I parse '(.*)' with padding (.*) as a Single Slot Binary Message")]
        public void WhenIParseWithNmeaAisSingleSlotBinaryParser(string payload, uint padding)
        {
            this.When(() => new NmeaAisSingleSlotBinaryParser(Encoding.ASCII.GetBytes(payload), padding));
        }

        [Then(@"NmeaAisSingleSlotBinaryParser\.Type is (.*)")]
        public void ThenNmeaAisSingleSlotBinaryParser_TypeIs(uint messageType)
        {
            this.Then(parser =>
            {
                Assert.AreEqual(messageType, parser.MessageType);
            });
        }

        [Then(@"NmeaAisSingleSlotBinaryParser\.RepeatIndicator is (.*)")]
        public void ThenNmeaAisSingleSlotBinaryParser_RepeatIndicatorIs(uint repeatCount)
        {
            this.Then(parser => Assert.AreEqual(repeatCount, parser.RepeatIndicator));
        }

        [Then(@"NmeaAisSingleSlotBinaryParser\.Mmsi is (.*)")]
        public void ThenNmeaAisSingleSlotBinaryParser_MmsiIs(uint mmsi)
        {
            this.Then(parser => Assert.AreEqual(mmsi, parser.Mmsi));
        }

        [Then(@"NmeaAisSingleSlotBinaryParser\.DestinationIndicator is (.*)")]
        public void ThenNmeaAisSingleSlotBinaryParser_DestinationIndicatorIs(DestinationIndicator value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.DestinationIndicator));
        }

        [Then(@"NmeaAisSingleSlotBinaryParser\.BinaryDataFlag is (.*)")]
        public void ThenNmeaAisSingleSlotBinaryParser_BinaryDataFlagIs(bool value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.BinaryDataFlag));
        }

        [Then(@"NmeaAisSingleSlotBinaryParser\.DestinationMmsi is (.*)")]
        public void ThenNmeaAisSingleSlotBinaryParser_DestinationMmsiIs(uint? value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.DestinationMmsi));
        }

        [Then(@"NmeaAisSingleSlotBinaryParser\.SpareBits70 is (.*)")]
        public void ThenNmeaAisSingleSlotBinaryParser_SpareBits70Is(uint? value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.SpareBits70));
        }

        [Then(@"NmeaAisSingleSlotBinaryParser\.DAC is (.*)")]
        public void ThenNmeaAisSingleSlotBinaryParser_DACIs(uint? value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.DAC));
        }

        [Then(@"NmeaAisSingleSlotBinaryParser\.FI is (.*)")]
        public void ThenNmeaAisSingleSlotBinaryParser_FIIs(uint? value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.FI));
        }

        [Then(@"NmeaAisSingleSlotBinaryParser\.ApplicationDataPadding is (.*)")]
        public void ThenNmeaAisSingleSlotBinaryParser_ApplicationDataPaddingIs(uint value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.ApplicationDataPadding));
        }

        [Then(@"NmeaAisSingleSlotBinaryParser\.ApplicationData is (.*)")]
        public void ThenNmeaAisSingleSlotBinaryParser_ApplicationDataIs(string value)
        {
            this.Then(parser => Assert.AreEqual(value, Encoding.ASCII.GetString(parser.ApplicationData)));
        }

        private void When(ParserMaker makeParser)
        {
            this.makeParser = makeParser;
        }

        private void Then(ParserTest test)
        {
            NmeaAisSingleSlotBinaryParser parser = this.makeParser();
            test(parser);
        }
    }
}
