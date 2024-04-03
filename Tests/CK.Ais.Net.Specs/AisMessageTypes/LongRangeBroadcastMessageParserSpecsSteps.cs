namespace Ais.Net.Specs.AisMessageTypes
{
    using System.Text;
    using NUnit.Framework;
    using TechTalk.SpecFlow;

    [Binding]
    public class LongRangeBroadcastMessageParserSpecsSteps
    {
        private ParserMaker makeParser;

        private delegate NmeaAisLongRangeBroadcastMessageParser ParserMaker();

        private delegate void ParserTest(NmeaAisLongRangeBroadcastMessageParser parser);

        [When("I parse '(.*)' with padding (.*) as a Long-range Automatic Identifcation System Broadcast Message")]
        public void WhenIParseWithNmeaAisLongRangeBroadcastMessageParser(string payload, uint padding)
        {
            this.When(() => new NmeaAisLongRangeBroadcastMessageParser(Encoding.ASCII.GetBytes(payload), padding));
        }

        [Then(@"NmeaAisLongRangeBroadcastMessageParser\.Type is (.*)")]
        public void ThenNmeaAisLongRangeBroadcastMessageParser_TypeIs(MessageType messageType)
        {
            this.Then(parser => Assert.AreEqual(messageType, parser.MessageType));
        }

        [Then(@"NmeaAisLongRangeBroadcastMessageParser\.RepeatIndicator is (.*)")]
        public void ThenNmeaAisLongRangeBroadcastMessageParser_RepeatIndicatorIs(uint repeatCount)
        {
            this.Then(parser => Assert.AreEqual(repeatCount, parser.RepeatIndicator));
        }

        [Then(@"NmeaAisLongRangeBroadcastMessageParser\.Mmsi is (.*)")]
        public void ThenNmeaAisLongRangeBroadcastMessageParser_MmsiIs(uint mmsi)
        {
            this.Then(parser => Assert.AreEqual(mmsi, parser.Mmsi));
        }

        [Then(@"NmeaAisLongRangeBroadcastMessageParser\.PositionAccuracy is (.*)")]
        public void ThenNmeaAisLongRangeBroadcastMessageParser_PositionAccuracyIs(bool value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.PositionAccuracy));
        }

        [Then(@"NmeaAisLongRangeBroadcastMessageParser\.RaimFlag is (.*)")]
        public void ThenNmeaAisLongRangeBroadcastMessageParser_RaimFlagIs(bool value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.RaimFlag));
        }

        [Then(@"NmeaAisLongRangeBroadcastMessageParser\.NavigationStatus is (.*)")]
        public void ThenNmeaAisLongRangeBroadcastMessageParser_NavigationStatusIs(NavigationStatus value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.NavigationStatus));
        }

        [Then(@"NmeaAisLongRangeBroadcastMessageParser\.Longitude10thMins is (.*)")]
        public void ThenNmeaAisLongRangeBroadcastMessageParser_Longitude10thMinsIs(int value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.Longitude10thMins));
        }

        [Then(@"NmeaAisLongRangeBroadcastMessageParser\.Latitude10thMins is (.*)")]
        public void ThenNmeaAisLongRangeBroadcastMessageParser_Latitude10thMinsIs(int value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.Latitude10thMins));
        }

        [Then(@"NmeaAisLongRangeBroadcastMessageParser\.SpeedOverGround is (.*)")]
        public void ThenNmeaAisLongRangeBroadcastMessageParser_SpeedOverGroundIs(uint value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.SpeedOverGround));
        }

        [Then(@"NmeaAisLongRangeBroadcastMessageParser\.CourseOverGround is (.*)")]
        public void ThenNmeaAisLongRangeBroadcastMessageParser_CourseOverGroundIs(uint value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.CourseOverGround));
        }

        [Then(@"NmeaAisLongRangeBroadcastMessageParser\.PositionLatency is (.*)")]
        public void ThenNmeaAisLongRangeBroadcastMessageParser_PositionLatencyIs(bool value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.PositionLatency));
        }

        [Then(@"NmeaAisLongRangeBroadcastMessageParser\.SpareBit94 is (.*)")]
        public void ThenNmeaAisLongRangeBroadcastMessageParser_SpareBit94Is(bool value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.SpareBit94));
        }

        private void When(ParserMaker makeParser)
        {
            this.makeParser = makeParser;
        }

        private void Then(ParserTest test)
        {
            NmeaAisLongRangeBroadcastMessageParser parser = this.makeParser();
            test(parser);
        }
    }
}
