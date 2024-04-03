namespace Ais.Net.Specs.AisMessageTypes
{
    using System.Text;
    using NUnit.Framework;
    using TechTalk.SpecFlow;

    [Binding]
    internal class CoordinatedUniversalTimeAndDateResponseParserSpecsSteps
    {
        private ParserMaker makeParser;

        private delegate NmeaAisCoordinatedUniversalTimeAndDateResponseParser ParserMaker();

        private delegate void ParserTest(NmeaAisCoordinatedUniversalTimeAndDateResponseParser parser);

        [When("I parse '(.*)' with padding (.*) as a Coordinated Universal Time and Date Response")]
        public void WhenIParseWithPaddingAsALongRangeAisBroadcast(string payload, uint padding)
        {
            this.When(() => new NmeaAisCoordinatedUniversalTimeAndDateResponseParser(Encoding.ASCII.GetBytes(payload), padding));
        }

        [Then(@"NmeaAisCoordinatedUniversalTimeAndDateResponseParser\.Type is (.*)")]
        public void ThenNmeaAisCoordinatedUniversalTimeAndDateResponseParser_TypeIs(MessageType messageType)
        {
            this.Then(parser => Assert.AreEqual(messageType, parser.MessageType));
        }

        [Then(@"NmeaAisCoordinatedUniversalTimeAndDateResponseParser\.RepeatIndicator is (.*)")]
        public void ThenNmeaAisCoordinatedUniversalTimeAndDateResponseParser_RepeatIndicatorIs(int repeatCount)
        {
            this.Then(parser => Assert.AreEqual(repeatCount, parser.RepeatIndicator));
        }

        [Then(@"NmeaAisCoordinatedUniversalTimeAndDateResponseParser\.Mmsi is (.*)")]
        public void ThenNmeaAisCoordinatedUniversalTimeAndDateResponseParser_MmsiIs(int mmsi)
        {
            this.Then(parser => Assert.AreEqual(mmsi, parser.Mmsi));
        }

        [Then(@"NmeaAisCoordinatedUniversalTimeAndDateResponseParser\.UtcYear is (.*)")]
        public void ThenNmeaAisCoordinatedUniversalTimeAndDateResponseParser_UtcYearIs(int utcYear)
        {
            this.Then(parser => Assert.AreEqual(utcYear, parser.UtcYear));
        }

        [Then(@"NmeaAisCoordinatedUniversalTimeAndDateResponseParser\.UtcMonth is (.*)")]
        public void ThenNmeaAisCoordinatedUniversalTimeAndDateResponseParser_UtcMonthIs(int utcMonth)
        {
            this.Then(parser => Assert.AreEqual(utcMonth, parser.UtcMonth));
        }

        [Then(@"NmeaAisCoordinatedUniversalTimeAndDateResponseParser\.UtcDay is (.*)")]
        public void ThenNmeaAisCoordinatedUniversalTimeAndDateResponseParser_UtcDayIs(int utcDay)
        {
            this.Then(parser => Assert.AreEqual(utcDay, parser.UtcDay));
        }

        [Then(@"NmeaAisCoordinatedUniversalTimeAndDateResponseParser\.UtcHour is (.*)")]
        public void ThenNmeaAisCoordinatedUniversalTimeAndDateResponseParser_UtcHourIs(int utcHour)
        {
            this.Then(parser => Assert.AreEqual(utcHour, parser.UtcHour));
        }

        [Then(@"NmeaAisCoordinatedUniversalTimeAndDateResponseParser\.UtcMinute is (.*)")]
        public void ThenNmeaAisCoordinatedUniversalTimeAndDateResponseParser_UtcMinuteIs(int utcMinute)
        {
            this.Then(parser => Assert.AreEqual(utcMinute, parser.UtcMinute));
        }

        [Then(@"NmeaAisCoordinatedUniversalTimeAndDateResponseParser\.UtcSecond is (.*)")]
        public void ThenNmeaAisCoordinatedUniversalTimeAndDateResponseParser_UtcSecondIs(int utcSecond)
        {
            this.Then(parser => Assert.AreEqual(utcSecond, parser.UtcSecond));
        }

        [Then(@"NmeaAisCoordinatedUniversalTimeAndDateResponseParser\.PositionAccuracy is (.*)")]
        public void ThenNmeaAisCoordinatedUniversalTimeAndDateResponseParser_PositionAccuracyIs(bool positionAccuracy)
        {
            this.Then(parser => Assert.AreEqual(positionAccuracy, parser.PositionAccuracy));
        }

        [Then(@"NmeaAisCoordinatedUniversalTimeAndDateResponseParser\.Longitude10000thMins is (.*)")]
        public void ThenNmeaAisCoordinatedUniversalTimeAndDateResponseParser_Longitude10000thMinsIs(double longitude)
        {
            this.Then(parser => Assert.AreEqual(longitude, parser.Longitude10000thMins / 600_000d));
        }

        [Then(@"NmeaAisCoordinatedUniversalTimeAndDateResponseParser\.Latitude10000thMins is (.*)")]
        public void ThenNmeaAisCoordinatedUniversalTimeAndDateResponseParser_Latitude10000thMinsIs(double latitude)
        {
            this.Then(parser => Assert.AreEqual(latitude, parser.Latitude10000thMins / 600_000d));
        }

        [Then(@"NmeaAisCoordinatedUniversalTimeAndDateResponseParser\.PositionFixType is (.*)")]
        public void ThenNmeaAisCoordinatedUniversalTimeAndDateResponseParser_PositionFixTypeIs(EpfdFixType positionFixType)
        {
            this.Then(parser => Assert.AreEqual(positionFixType, parser.PositionFixType));
        }

        [Then(@"NmeaAisCoordinatedUniversalTimeAndDateResponseParser\.TransmissionControlForLongRangeBroadcastMessage is (.*)")]
        public void ThenNmeaAisCoordinatedUniversalTimeAndDateResponseParser_TransmissionControlForLongRangeBroadcastMessageIs(bool transmissionControlForLongRangeBroadcastMessage)
        {
            this.Then(parser => Assert.AreEqual(transmissionControlForLongRangeBroadcastMessage, parser.TransmissionControlForLongRangeBroadcastMessage));
        }

        [Then(@"NmeaAisCoordinatedUniversalTimeAndDateResponseParser\.SpareBits139 is (.*)")]
        public void ThenNmeaAisCoordinatedUniversalTimeAndDateResponseParser_SpareBits139Is(uint spareBits139)
        {
            this.Then(parser => Assert.AreEqual(spareBits139, parser.SpareBits139));
        }

        [Then(@"NmeaAisCoordinatedUniversalTimeAndDateResponseParser\.RaimFlag is (.*)")]
        public void ThenNmeaAisCoordinatedUniversalTimeAndDateResponseParser_PositionFixTypeIs(bool raimFlag)
        {
            this.Then(parser => Assert.AreEqual(raimFlag, parser.RaimFlag));
        }

        [Then(@"NmeaAisCoordinatedUniversalTimeAndDateResponseParser\.CommunicationState is (.*)")]
        public void ThenNmeaAisCoordinatedUniversalTimeAndDateResponseParser_PositionFixTypeIs(uint communicationState)
        {
            this.Then(parser => Assert.AreEqual(communicationState, parser.CommunicationState));
        }

        private void When(ParserMaker makeParser)
        {
            this.makeParser = makeParser;
        }

        private void Then(ParserTest test)
        {
            NmeaAisCoordinatedUniversalTimeAndDateResponseParser parser = this.makeParser();
            test(parser);
        }
    }
}
