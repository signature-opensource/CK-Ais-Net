namespace Ais.Net.Specs.AisMessageTypes
{
    using System.Linq;
    using System.Text;
    using NUnit.Framework;
    using TechTalk.SpecFlow;

    [Binding]
    internal class SafetyRelatedBroadcastParserSpecsSteps
    {
        private ParserMaker makeParser;

        private delegate NmeaAisSafetyRelatedBroadcastParser ParserMaker();

        private delegate void ParserTest(NmeaAisSafetyRelatedBroadcastParser parser);

        [When("I parse '(.*)' with padding (.*) as a Safety Related Broadcast")]
        public void WhenIParseWithPaddingAsALongRangeAisBroadcast(string payload, uint padding)
        {
            this.When(() => new NmeaAisSafetyRelatedBroadcastParser(Encoding.ASCII.GetBytes(payload), padding));
        }

        [Then(@"NmeaAisSafetyRelatedBroadcastParser\.Type is (.*)")]
        public void ThenNmeaAisSafetyRelatedBroadcastParser_TypeIs(int messageType)
        {
            this.Then(parser => Assert.AreEqual(messageType, parser.MessageType));
        }

        [Then(@"NmeaAisSafetyRelatedBroadcastParser\.RepeatIndicator is (.*)")]
        public void ThenNmeaAisSafetyRelatedBroadcastParser_RepeatIndicatorIs(int repeatCount)
        {
            this.Then(parser => Assert.AreEqual(repeatCount, parser.RepeatIndicator));
        }

        [Then(@"NmeaAisSafetyRelatedBroadcastParser\.Mmsi is (.*)")]
        public void ThenNmeaAisSafetyRelatedBroadcastParser_MmsiIs(int mmsi)
        {
            this.Then(parser => Assert.AreEqual(mmsi, parser.Mmsi));
        }

        [Then(@"NmeaAisSafetyRelatedBroadcastParser\.SpareBit38 is (.*)")]
        public void ThenNmeaAisSafetyRelatedBroadcastParser_SpareBit38Is(uint spareBit38)
        {
            this.Then(parser => Assert.AreEqual(spareBit38, parser.SpareBits38));
        }

        [Then(@"NmeaAisSafetyRelatedBroadcastParser\.SafetyRelatedText is (.*)")]
        public void ThenNmeaAisSafetyRelatedBroadcastParser_SafetyRelatedTextIs(string safetyRelatedText)
        {
            this.Then(parser =>
            {
                byte[] text = new byte[parser.SafetyRelatedText.CharacterCount];
                parser.SafetyRelatedText.WriteAsAscii(text);
                Assert.AreEqual(safetyRelatedText.Trim('"'), text);
            });
        }

        private void When(ParserMaker makeParser)
        {
            this.makeParser = makeParser;
        }

        private void Then(ParserTest test)
        {
            NmeaAisSafetyRelatedBroadcastParser parser = this.makeParser();
            test(parser);
        }
    }
}
