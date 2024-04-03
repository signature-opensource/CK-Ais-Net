namespace Ais.Net.Specs.AisMessageTypes
{
    using System.Text;
    using NUnit.Framework;
    using TechTalk.SpecFlow;

    [Binding]
    public class CoordinatedUniversalTimeAndDateInquiryParserSpecsSteps
    {
        private ParserMaker makeParser;

        private delegate NmeaAisCoordinatedUniversalTimeAndDateInquiryParser ParserMaker();

        private delegate void ParserTest(NmeaAisCoordinatedUniversalTimeAndDateInquiryParser parser);

        [When("I parse '(.*)' with padding (.*) as a Coordinated Universal Time and Date Inquiry")]
        public void WhenIParseWithPaddingAsALongRangeAisBroadcast(string payload, uint padding)
        {
            this.When(() => new NmeaAisCoordinatedUniversalTimeAndDateInquiryParser(Encoding.ASCII.GetBytes(payload), padding));
        }

        [Then(@"NmeaAisCoordinatedUniversalTimeAndDateInquiryParser\.Type is (.*)")]
        public void ThenNmeaAisCoordinatedUniversalTimeAndDateInquiryParser_TypeIs(MessageType messageType)
        {
            this.Then(parser => Assert.AreEqual(messageType, parser.MessageType));
        }

        [Then(@"NmeaAisCoordinatedUniversalTimeAndDateInquiryParser\.RepeatIndicator is (.*)")]
        public void ThenNmeaAisCoordinatedUniversalTimeAndDateInquiryParser_RepeatIndicatorIs(int repeatCount)
        {
            this.Then(parser => Assert.AreEqual(repeatCount, parser.RepeatIndicator));
        }

        [Then(@"NmeaAisCoordinatedUniversalTimeAndDateInquiryParser\.Mmsi is (.*)")]
        public void ThenNmeaAisCoordinatedUniversalTimeAndDateInquiryParser_MmsiIs(int mmsi)
        {
            this.Then(parser => Assert.AreEqual(mmsi, parser.Mmsi));
        }

        [Then(@"NmeaAisCoordinatedUniversalTimeAndDateInquiryParser\.SpareBits38 is (.*)")]
        public void ThenNmeaAisCoordinatedUniversalTimeAndDateInquiryParser_SpareBits38Is(int bits38)
        {
            this.Then(parser => Assert.AreEqual(bits38, parser.SpareBits38));
        }

        [Then(@"NmeaAisCoordinatedUniversalTimeAndDateInquiryParser\.DestinationMmsi is (.*)")]
        public void ThenNmeaAisCoordinatedUniversalTimeAndDateInquiryParser_DestinationMmsiIs(int destinationMmsi)
        {
            this.Then(parser => Assert.AreEqual(destinationMmsi, parser.DestinationMmsi));
        }

        [Then(@"NmeaAisCoordinatedUniversalTimeAndDateInquiryParser\.SpareBits70 is (.*)")]
        public void ThenNmeaAisCoordinatedUniversalTimeAndDateInquiryParser_SpareBits70Is(int bits70)
        {
            this.Then(parser => Assert.AreEqual(bits70, parser.SpareBits70));
        }

        private void When(ParserMaker makeParser)
        {
            this.makeParser = makeParser;
        }

        private void Then(ParserTest test)
        {
            NmeaAisCoordinatedUniversalTimeAndDateInquiryParser parser = this.makeParser();
            test(parser);
        }
    }
}
