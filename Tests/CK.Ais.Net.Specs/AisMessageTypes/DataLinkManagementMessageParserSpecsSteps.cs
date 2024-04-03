namespace Ais.Net.Specs.AisMessageTypes
{
    using System.Text;
    using NUnit.Framework;
    using TechTalk.SpecFlow;

    [Binding]
    public class DataLinkManagementMessageParserSpecsSteps
    {
        private ParserMaker makeParser;

        private delegate NmeaAisDataLinkManagementMessageParser ParserMaker();

        private delegate void ParserTest(NmeaAisDataLinkManagementMessageParser parser);

        [When("I parse '(.*)' with padding (.*) as a Data link management message")]
        public void WhenIParseWithNmeaAisDataLinkManagementMessageParser(string payload, uint padding)
        {
            this.When(() => new NmeaAisDataLinkManagementMessageParser(Encoding.ASCII.GetBytes(payload), padding));
        }

        [Then(@"NmeaAisDataLinkManagementMessageParser\.Type is (.*)")]
        public void ThenNmeaAisDataLinkManagementMessageParser_TypeIs(MessageType messageType)
        {
            this.Then(parser => Assert.AreEqual(messageType, parser.MessageType));
        }

        [Then(@"NmeaAisDataLinkManagementMessageParser\.RepeatIndicator is (.*)")]
        public void ThenNmeaAisDataLinkManagementMessageParser_RepeatIndicatorIs(uint repeatCount)
        {
            this.Then(parser => Assert.AreEqual(repeatCount, parser.RepeatIndicator));
        }

        [Then(@"NmeaAisDataLinkManagementMessageParser\.Mmsi is (.*)")]
        public void ThenNmeaAisDataLinkManagementMessageParser_MmsiIs(uint mmsi)
        {
            this.Then(parser => Assert.AreEqual(mmsi, parser.Mmsi));
        }

        [Then(@"NmeaAisDataLinkManagementMessageParser\.SpareBits38 is (.*)")]
        public void ThenNmeaAisDataLinkManagementMessageParser_SpareBits38Is(uint value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.SpareBits38));
        }

        [Then(@"NmeaAisDataLinkManagementMessageParser\.Offset1 is (.*)")]
        public void ThenNmeaAisDataLinkManagementMessageParser_Offset1Is(uint value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.Offset1));
        }

        [Then(@"NmeaAisDataLinkManagementMessageParser\.SlotNumber1 is (.*)")]
        public void ThenNmeaAisDataLinkManagementMessageParser_SlotNumber1Is(uint value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.SlotNumber1));
        }

        [Then(@"NmeaAisDataLinkManagementMessageParser\.Timeout1 is (.*)")]
        public void ThenNmeaAisDataLinkManagementMessageParser_Timeout1Is(uint value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.Timeout1));
        }

        [Then(@"NmeaAisDataLinkManagementMessageParser\.Increment1 is (.*)")]
        public void ThenNmeaAisDataLinkManagementMessageParser_Increment1Is(uint value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.Increment1));
        }

        [Then(@"NmeaAisDataLinkManagementMessageParser\.Offset2 is (.*)")]
        public void ThenNmeaAisDataLinkManagementMessageParser_Offset2Is(uint? value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.Offset2));
        }

        [Then(@"NmeaAisDataLinkManagementMessageParser\.SlotNumber2 is (.*)")]
        public void ThenNmeaAisDataLinkManagementMessageParser_SlotNumber2Is(uint? value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.SlotNumber2));
        }

        [Then(@"NmeaAisDataLinkManagementMessageParser\.Timeout2 is (.*)")]
        public void ThenNmeaAisDataLinkManagementMessageParser_Timeout2Is(uint? value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.Timeout2));
        }

        [Then(@"NmeaAisDataLinkManagementMessageParser\.Increment2 is (.*)")]
        public void ThenNmeaAisDataLinkManagementMessageParser_Increment2Is(uint? value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.Increment2));
        }

        [Then(@"NmeaAisDataLinkManagementMessageParser\.Offset3 is (.*)")]
        public void ThenNmeaAisDataLinkManagementMessageParser_Offset3Is(uint? value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.Offset3));
        }

        [Then(@"NmeaAisDataLinkManagementMessageParser\.SlotNumber3 is (.*)")]
        public void ThenNmeaAisDataLinkManagementMessageParser_SlotNumber3Is(uint? value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.SlotNumber3));
        }

        [Then(@"NmeaAisDataLinkManagementMessageParser\.Timeout3 is (.*)")]
        public void ThenNmeaAisDataLinkManagementMessageParser_Timeout3Is(uint? value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.Timeout3));
        }

        [Then(@"NmeaAisDataLinkManagementMessageParser\.Increment3 is (.*)")]
        public void ThenNmeaAisDataLinkManagementMessageParser_Increment3Is(uint? value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.Increment3));
        }

        [Then(@"NmeaAisDataLinkManagementMessageParser\.Offset4 is (.*)")]
        public void ThenNmeaAisDataLinkManagementMessageParser_Offset4Is(uint? value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.Offset4));
        }

        [Then(@"NmeaAisDataLinkManagementMessageParser\.SlotNumber4 is (.*)")]
        public void ThenNmeaAisDataLinkManagementMessageParser_SlotNumber4Is(uint? value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.SlotNumber4));
        }

        [Then(@"NmeaAisDataLinkManagementMessageParser\.Timeout4 is (.*)")]
        public void ThenNmeaAisDataLinkManagementMessageParser_Timeout4Is(uint? value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.Timeout4));
        }

        [Then(@"NmeaAisDataLinkManagementMessageParser\.Increment4 is (.*)")]
        public void ThenNmeaAisDataLinkManagementMessageParser_Increment4Is(uint? value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.Increment4));
        }

        [Then(@"NmeaAisDataLinkManagementMessageParser\.SpareBitsAtEnd is (.*)")]
        public void ThenNmeaAisDataLinkManagementMessageParser_SpareBitsAtEndAIs(uint? value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.SpareBitsAtEnd));
        }

        private void When(ParserMaker makeParser)
        {
            this.makeParser = makeParser;
        }

        private void Then(ParserTest test)
        {
            NmeaAisDataLinkManagementMessageParser parser = this.makeParser();
            test(parser);
        }
    }
}
