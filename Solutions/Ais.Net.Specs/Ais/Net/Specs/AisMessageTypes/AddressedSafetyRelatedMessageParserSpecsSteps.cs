namespace Ais.Net.Specs.AisMessageTypes
{
    using System.Text;
    using NUnit.Framework;
    using TechTalk.SpecFlow;

    [Binding]
    internal class AddressedSafetyRelatedMessageParserSpecsSteps
    {
        private ParserMaker makeParser;

        private delegate NmeaAisAddressedSafetyRelatedMessageParser ParserMaker();

        private delegate void ParserTest(NmeaAisAddressedSafetyRelatedMessageParser parser);

        [When("I parse '(.*)' with padding (.*) as a Addressed Safety Related Message")]
        public void WhenIParseWithPaddingAsALongRangeAisBroadcast(string payload, uint padding)
        {
            this.When(() => new NmeaAisAddressedSafetyRelatedMessageParser(Encoding.ASCII.GetBytes(payload), padding));
        }

        [Then(@"NmeaAisAddressedSafetyRelatedMessageParser\.Type is (.*)")]
        public void ThenNmeaAisBaseStationReportParser_TypeIs(int messageType)
        {
            this.Then(parser => Assert.AreEqual(messageType, parser.MessageType));
        }

        [Then(@"NmeaAisAddressedSafetyRelatedMessageParser\.RepeatIndicator is (.*)")]
        public void ThenNmeaAisBaseStationReportParser_RepeatIndicatorIs(int repeatCount)
        {
            this.Then(parser => Assert.AreEqual(repeatCount, parser.RepeatIndicator));
        }

        [Then(@"NmeaAisAddressedSafetyRelatedMessageParser\.Mmsi is (.*)")]
        public void ThenNmeaAisBaseStationReportParser_MmsiIs(int mmsi)
        {
            this.Then(parser => Assert.AreEqual(mmsi, parser.Mmsi));
        }

        [Then(@"NmeaAisAddressedSafetyRelatedMessageParser\.SequenceNumber is (.*)")]
        public void ThenNmeaAisBaseStationReportParser_SequenceNumberIs(int sequenceNumber)
        {
            this.Then(parser => Assert.AreEqual(sequenceNumber, parser.SequenceNumber));
        }

        [Then(@"NmeaAisAddressedSafetyRelatedMessageParser\.DestinationMmsi is (.*)")]
        public void ThenNmeaAisBaseStationReportParser_DestinationMmsiIs(int destinationMmsi)
        {
            this.Then(parser => Assert.AreEqual(destinationMmsi, parser.DestinationMmsi));
        }

        [Then(@"NmeaAisAddressedSafetyRelatedMessageParser\.Retransmit is (.*)")]
        public void ThenNmeaAisBaseStationReportParser_RetransmitIs(bool retransmit)
        {
            this.Then(parser => Assert.AreEqual(retransmit, parser.Retransmit));
        }

        [Then(@"NmeaAisAddressedSafetyRelatedMessageParser\.SpareBit71 is (.*)")]
        public void ThenNmeaAisBaseStationReportParser_SpareBit71Is(bool spareBit71)
        {
            this.Then(parser => Assert.AreEqual(spareBit71, parser.SpareBit71));
        }

        [Then(@"NmeaAisAddressedSafetyRelatedMessageParser\.SafetyRelatedText is (.*)")]
        public void ThenNmeaAisBaseStationReportParser_SafetyRelatedTextIs(string safetyRelatedText)
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
            NmeaAisAddressedSafetyRelatedMessageParser parser = this.makeParser();
            test(parser);
        }
    }
}
