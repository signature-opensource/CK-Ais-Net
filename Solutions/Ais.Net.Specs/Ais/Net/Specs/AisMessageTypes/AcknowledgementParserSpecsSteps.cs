namespace Ais.Net.Specs.AisMessageTypes
{
    using System.Text;
    using NUnit.Framework;
    using TechTalk.SpecFlow;

    [Binding]
    public class AcknowledgementParserSpecsSteps
    {
        private ParserMaker makeParser;

        private delegate NmeaAisAcknowledgementParser ParserMaker();

        private delegate void ParserTest(NmeaAisAcknowledgementParser parser);

        [When("I parse '(.*)' with padding (.*) as a Acknowledgement Message")]
        public void WhenIParseWithNmeaAisAcknowledgementParser(string payload, uint padding)
        {
            this.When(() => new NmeaAisAcknowledgementParser(Encoding.ASCII.GetBytes(payload), padding));
        }

        [Then(@"NmeaAisAcknowledgementParser\.Type is (.*)")]
        public void ThenNmeaAisAcknowledgementParser_TypeIs(MessageType messageType)
        {
            this.Then(parser => Assert.AreEqual(messageType, parser.MessageType));
        }

        [Then(@"NmeaAisAcknowledgementParser\.RepeatIndicator is (.*)")]
        public void ThenNmeaAisAcknowledgementParser_RepeatIndicatorIs(uint repeatCount)
        {
            this.Then(parser => Assert.AreEqual(repeatCount, parser.RepeatIndicator));
        }

        [Then(@"NmeaAisAcknowledgementParser\.Mmsi is (.*)")]
        public void ThenNmeaAisAcknowledgementParser_MmsiIs(uint mmsi)
        {
            this.Then(parser => Assert.AreEqual(mmsi, parser.Mmsi));
        }

        [Then(@"NmeaAisAcknowledgementParser\.SpareBits38 is (.*)")]
        public void ThenNmeaAisAcknowledgementParser_Is(uint value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.SpareBits38));
        }

        [Then(@"NmeaAisAcknowledgementParser\.DestinationMmsi1 is (.*)")]
        public void ThenNmeaAisAcknowledgementParser_DestinationMmsi1Is(uint value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.DestinationMmsi1));
        }

        [Then(@"NmeaAisAcknowledgementParser\.SequenceNumberMmsi1 is (.*)")]
        public void ThenNmeaAisAcknowledgementParser_SequenceNumberMmsi1Is(uint value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.SequenceNumberMmsi1));
        }

        [Then(@"NmeaAisAcknowledgementParser\.DestinationMmsi2 is (.*)")]
        public void ThenNmeaAisAcknowledgementParser_DestinationMms21Is(uint? value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.DestinationMmsi2));
        }

        [Then(@"NmeaAisAcknowledgementParser\.SequenceNumberMmsi2 is (.*)")]
        public void ThenNmeaAisAcknowledgementParser_SequenceNumberMmsi2Is(uint? value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.SequenceNumberMmsi2));
        }

        [Then(@"NmeaAisAcknowledgementParser\.DestinationMmsi3 is (.*)")]
        public void ThenNmeaAisAcknowledgementParser_DestinationMmsi3Is(uint? value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.DestinationMmsi3));
        }

        [Then(@"NmeaAisAcknowledgementParser\.SequenceNumberMmsi3 is (.*)")]
        public void ThenNmeaAisAcknowledgementParser_SequenceNumberMmsi3Is(uint? value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.SequenceNumberMmsi3));
        }

        [Then(@"NmeaAisAcknowledgementParser\.DestinationMmsi4 is (.*)")]
        public void ThenNmeaAisAcknowledgementParser_DestinationMmsi4Is(uint? value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.DestinationMmsi4));
        }

        [Then(@"NmeaAisAcknowledgementParser\.SequenceNumberMmsi4 is (.*)")]
        public void ThenNmeaAisAcknowledgementParser_SequenceNumberMmsi4Is(uint? value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.SequenceNumberMmsi4));
        }

        private void When(ParserMaker makeParser)
        {
            this.makeParser = makeParser;
        }

        private void Then(ParserTest test)
        {
            NmeaAisAcknowledgementParser parser = this.makeParser();
            test(parser);
        }
    }
}
