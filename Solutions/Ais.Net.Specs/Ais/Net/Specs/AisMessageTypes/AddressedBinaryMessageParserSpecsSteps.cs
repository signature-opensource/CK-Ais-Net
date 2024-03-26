namespace Ais.Net.Specs.AisMessageTypes
{
    using System.Text;
    using NUnit.Framework;
    using TechTalk.SpecFlow;

    [Binding]
    public class AddressedBinaryMessageParserSpecsSteps
    {
        private ParserMaker makeParser;

        private delegate NmeaAisAddressedBinaryMessageParser ParserMaker();

        private delegate void ParserTest(NmeaAisAddressedBinaryMessageParser parser);

        [When("I parse '(.*)' with padding (.*) as a Addressed Binary Message")]
        public void WhenIParseWithNmeaAisAddressedBinaryMessageParser(string payload, uint padding)
        {
            this.When(() => new NmeaAisAddressedBinaryMessageParser(Encoding.ASCII.GetBytes(payload), padding));
        }

        [Then(@"NmeaAisAddressedBinaryMessageParser\.Type is (.*)")]
        public void ThenNmeaAisAddressedBinaryMessageParser_TypeIs(uint messageType)
        {
            this.Then(parser => Assert.AreEqual(messageType, parser.MessageType));
        }

        [Then(@"NmeaAisAddressedBinaryMessageParser\.RepeatIndicator is (.*)")]
        public void ThenNmeaAisAddressedBinaryMessageParser_RepeatIndicatorIs(uint repeatCount)
        {
            this.Then(parser => Assert.AreEqual(repeatCount, parser.RepeatIndicator));
        }

        [Then(@"NmeaAisAddressedBinaryMessageParser\.Mmsi is (.*)")]
        public void ThenNmeaAisAddressedBinaryMessageParser_MmsiIs(uint mmsi)
        {
            this.Then(parser => Assert.AreEqual(mmsi, parser.Mmsi));
        }

        [Then(@"NmeaAisAddressedBinaryMessageParser\.SequenceNumber is (.*)")]
        public void ThenNmeaAisAddressedBinaryMessageParser_SequenceNumberIs(uint value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.SequenceNumber));
        }

        [Then(@"NmeaAisAddressedBinaryMessageParser\.DestinationMmsi is (.*)")]
        public void ThenNmeaAisAddressedBinaryMessageParser_DestinationMmsiIs(uint value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.DestinationMmsi));
        }

        [Then(@"NmeaAisAddressedBinaryMessageParser\.Retransmit is (.*)")]
        public void ThenNmeaAisAddressedBinaryMessageParser_Is(bool value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.Retransmit));
        }

        [Then(@"NmeaAisAddressedBinaryMessageParser\.SpareBit71 is (.*)")]
        public void ThenNmeaAisAddressedBinaryMessageParser_SpareBit71Is(bool value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.SpareBit71));
        }

        [Then(@"NmeaAisAddressedBinaryMessageParser\.DAC is (.*)")]
        public void ThenNmeaAisAddressedBinaryMessageParser_DACIs(uint value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.DAC));
        }

        [Then(@"NmeaAisAddressedBinaryMessageParser\.FI is (.*)")]
        public void ThenNmeaAisAddressedBinaryMessageParser_FIIs(uint value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.FI));
        }

        [Then(@"NmeaAisAddressedBinaryMessageParser\.ApplicationDataPadding is (.*)")]
        public void ThenNmeaAisAddressedBinaryMessageParser_ApplicationDataPaddingIs(uint value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.ApplicationDataPadding));
        }

        private void When(ParserMaker makeParser)
        {
            this.makeParser = makeParser;
        }

        private void Then(ParserTest test)
        {
            NmeaAisAddressedBinaryMessageParser parser = this.makeParser();
            test(parser);
        }
    }
}
