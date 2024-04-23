namespace Ais.Net.Specs.AisMessageTypes
{
    using System.Text;
    using NUnit.Framework;
    using TechTalk.SpecFlow;

    [Binding]
    public class InterrogationParserSpecsSteps
    {
        private ParserMaker makeParser;

        private delegate NmeaAisInterrogationParser ParserMaker();

        private delegate void ParserTest(NmeaAisInterrogationParser parser);

        [When("I parse '(.*)' with padding (.*) as an Interrogation")]
        public void WhenIParseWithNmeaAisInterrogationParser(string payload, uint padding)
        {
            this.When(() => new NmeaAisInterrogationParser(Encoding.ASCII.GetBytes(payload), padding));
        }

        [Then(@"NmeaAisInterrogationParser\.Type is (.*)")]
        public void ThenNmeaAisInterrogationParser_TypeIs(MessageType messageType)
        {
            this.Then(parser => Assert.AreEqual(messageType, parser.MessageType));
        }

        [Then(@"NmeaAisInterrogationParser\.RepeatIndicator is (.*)")]
        public void ThenNmeaAisInterrogationParser_RepeatIndicatorIs(int repeatCount)
        {
            this.Then(parser => Assert.AreEqual(repeatCount, parser.RepeatIndicator));
        }

        [Then(@"NmeaAisInterrogationParser\.Mmsi is (.*)")]
        public void ThenNmeaAisInterrogationParser_MmsiIs(int mmsi)
        {
            this.Then(parser => Assert.AreEqual(mmsi, parser.Mmsi));
        }

        [Then(@"NmeaAisInterrogationParser\.SpareBits38 is (.*)")]
        public void ThenNmeaAisInterrogationParser_SpareBits38Is(uint value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.SpareBits38));
        }

        [Then(@"NmeaAisInterrogationParser\.DestinationMmsi1 is (.*)")]
        public void ThenNmeaAisInterrogationParser_DestinationMmsi1Is(uint value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.DestinationMmsi1));
        }

        [Then(@"NmeaAisInterrogationParser\.MessageType11 is (.*)")]
        public void ThenNmeaAisInterrogationParser_MessageType11Is(MessageType value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.MessageType11));
        }

        [Then(@"NmeaAisInterrogationParser\.SlotOffset11 is (.*)")]
        public void ThenNmeaAisInterrogationParser_SlotOffset11Is(uint value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.SlotOffset11));
        }

        [Then(@"NmeaAisInterrogationParser\.SpareBits88 is (.*)")]
        public void ThenNmeaAisInterrogationParser_SpareBits88Is(uint? value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.SpareBits88));
        }

        [Then(@"NmeaAisInterrogationParser\.MessageType12 is (.*)")]
        public void ThenNmeaAisInterrogationParser_MessageType12Is(MessageType? value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.MessageType12));
        }

        [Then(@"NmeaAisInterrogationParser\.SlotOffset12 is (.*)")]
        public void ThenNmeaAisInterrogationParser_SlotOffset12Is(uint? value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.SlotOffset12));
        }

        [Then(@"NmeaAisInterrogationParser\.SpareBits108 is (.*)")]
        public void ThenNmeaAisInterrogationParser_SpareBits108Is(uint? value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.SpareBits108));
        }

        [Then(@"NmeaAisInterrogationParser\.DestinationMmsi2 is (.*)")]
        public void ThenNmeaAisInterrogationParser_DestinationMmsi2Is(uint? value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.DestinationMmsi2));
        }

        [Then(@"NmeaAisInterrogationParser\.MessageType21 is (.*)")]
        public void ThenNmeaAisInterrogationParser_MessageType21Is(uint? value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.MessageType21));
        }

        [Then(@"NmeaAisInterrogationParser\.SlotOffset21 is (.*)")]
        public void ThenNmeaAisInterrogationParser_SlotOffset21Is(uint? value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.SlotOffset21));
        }

        [Then(@"NmeaAisInterrogationParser\.SpareBits158 is (.*)")]
        public void ThenNmeaAisInterrogationParser_SpareBits158Is(uint? value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.SpareBits158));
        }

        private void When(ParserMaker makeParser)
        {
            this.makeParser = makeParser;
        }

        private void Then(ParserTest test)
        {
            NmeaAisInterrogationParser parser = this.makeParser();
            test(parser);
        }
    }
}
