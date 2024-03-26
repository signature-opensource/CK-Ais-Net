namespace Ais.Net.Specs.AisMessageTypes
{
    using System.Text;
    using NUnit.Framework;
    using TechTalk.SpecFlow;

    [Binding]
    public class BinaryBroadcastMessageParserSpecsSteps
    {
        private ParserMaker makeParser;

        private delegate NmeaAisBinaryBroadcastMessageParser ParserMaker();

        private delegate void ParserTest(NmeaAisBinaryBroadcastMessageParser parser);

        [When("I parse '(.*)' with padding (.*) as a Binary Broadcast Message")]
        public void WhenIParseWithNmeaAisBinaryBroadcastMessageParser(string payload, uint padding)
        {
            this.When(() => new NmeaAisBinaryBroadcastMessageParser(Encoding.ASCII.GetBytes(payload), padding));
        }

        [Then(@"NmeaAisBinaryBroadcastMessageParser\.Type is (.*)")]
        public void ThenNmeaAisBinaryBroadcastMessageParser_TypeIs(uint messageType)
        {
            this.Then(parser => Assert.AreEqual(messageType, parser.MessageType));
        }

        [Then(@"NmeaAisBinaryBroadcastMessageParser\.RepeatIndicator is (.*)")]
        public void ThenNmeaAisBinaryBroadcastMessageParser_RepeatIndicatorIs(uint repeatCount)
        {
            this.Then(parser => Assert.AreEqual(repeatCount, parser.RepeatIndicator));
        }

        [Then(@"NmeaAisBinaryBroadcastMessageParser\.Mmsi is (.*)")]
        public void ThenNmeaAisBinaryBroadcastMessageParser_MmsiIs(uint mmsi)
        {
            this.Then(parser => Assert.AreEqual(mmsi, parser.Mmsi));
        }

        [Then(@"NmeaAisBinaryBroadcastMessageParser\.SpareBits38 is (.*)")]
        public void ThenNmeaAisBinaryBroadcastMessageParser_SpareBits38Is(uint value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.SpareBits38));
        }

        [Then(@"NmeaAisBinaryBroadcastMessageParser\.DAC is (.*)")]
        public void ThenNmeaAisBinaryBroadcastMessageParser_DACIs(uint value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.DAC));
        }

        [Then(@"NmeaAisBinaryBroadcastMessageParser\.FI is (.*)")]
        public void ThenNmeaAisBinaryBroadcastMessageParser_FIIs(uint value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.FI));
        }

        [Then(@"NmeaAisBinaryBroadcastMessageParser\.ApplicationDataPadding is (.*)")]
        public void ThenNmeaAisBinaryBroadcastMessageParser_ApplicationDataPaddingIs(uint value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.ApplicationDataPadding));
        }

        [Then(@"NmeaAisBinaryBroadcastMessageParser\.ApplicationData is (.*)")]
        public void ThenNmeaAisBinaryBroadcastMessageParser_ApplicationDataIs(string value)
        {
            this.Then(parser => Assert.AreEqual(value, Encoding.ASCII.GetString(parser.ApplicationData)));
        }

        private void When(ParserMaker makeParser)
        {
            this.makeParser = makeParser;
        }

        private void Then(ParserTest test)
        {
            NmeaAisBinaryBroadcastMessageParser parser = this.makeParser();
            test(parser);
        }
    }
}
