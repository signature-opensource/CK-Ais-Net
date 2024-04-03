namespace Ais.Net.Specs.AisMessageTypes
{
    using System.Text;
    using NUnit.Framework;
    using TechTalk.SpecFlow;

    [Binding]
    public class GlobalNavigationSatelliteSystemBroadcastBinaryMessageParserSpecsSteps
    {
        private ParserMaker makeParser;

        private delegate NmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser ParserMaker();

        private delegate void ParserTest(NmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser parser);

        [When("I parse '(.*)' with padding (.*) as a Global Navigation-Satellite System Broadcast Binary Message")]
        public void WhenIParseWithNmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser(string payload, uint padding)
        {
            this.When(() => new NmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser(Encoding.ASCII.GetBytes(payload), padding));
        }

        [Then(@"NmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser\.Type is (.*)")]
        public void ThenNmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser_TypeIs(MessageType messageType)
        {
            this.Then(parser => Assert.AreEqual(messageType, parser.MessageType));
        }

        [Then(@"NmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser\.RepeatIndicator is (.*)")]
        public void ThenNmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser_RepeatIndicatorIs(uint repeatCount)
        {
            this.Then(parser => Assert.AreEqual(repeatCount, parser.RepeatIndicator));
        }

        [Then(@"NmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser\.Mmsi is (.*)")]
        public void ThenNmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser_MmsiIs(uint mmsi)
        {
            this.Then(parser => Assert.AreEqual(mmsi, parser.Mmsi));
        }

        [Then(@"NmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser\.SpareBits38 is (.*)")]
        public void ThenNmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser_SpareBits38Is(uint value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.SpareBits38));
        }

        [Then(@"NmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser\.Longitude10thMins is (.*)")]
        public void ThenNmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser_Longitude10thMinsIs(int value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.Longitude10thMins));
        }

        [Then(@"NmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser\.Latitude10thMins is (.*)")]
        public void ThenNmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser_Latitude10thMinsIs(int value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.Latitude10thMins));
        }

        [Then(@"NmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser\.SpareBits75 is (.*)")]
        public void ThenNmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser_SpareBits75Is(uint value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.SpareBits75));
        }

        [Then(@"NmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser\.DifferentialCorrectionDataPadding is (.*)")]
        public void ThenNmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser_DifferentialCorrectionDataPaddingIs(uint value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.DifferentialCorrectionDataPadding));
        }

        [Then(@"NmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser\.DifferentialCorrectionData is (.*)")]
        public void ThenNmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser_DifferentialCorrectionDataIs(string value)
        {
            this.Then(parser => Assert.AreEqual(value, Encoding.ASCII.GetString(parser.DifferentialCorrectionData)));
        }

        private void When(ParserMaker makeParser)
        {
            this.makeParser = makeParser;
        }

        private void Then(ParserTest test)
        {
            NmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser parser = this.makeParser();
            test(parser);
        }
    }
}
