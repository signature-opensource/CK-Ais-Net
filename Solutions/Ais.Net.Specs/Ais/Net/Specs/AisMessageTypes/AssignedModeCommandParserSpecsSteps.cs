namespace Ais.Net.Specs.AisMessageTypes
{
    using System.Text;
    using NUnit.Framework;
    using TechTalk.SpecFlow;

    [Binding]
    public class AssignedModeCommandParserSpecsSteps
    {
        private ParserMaker makeParser;

        private delegate NmeaAisAssignedModeCommandParser ParserMaker();

        private delegate void ParserTest(NmeaAisAssignedModeCommandParser parser);

        [When("I parse '(.*)' with padding (.*) as a Assigned mode command")]
        public void WhenIParseWithNmeaAisAssignedModeCommandParser(string payload, uint padding)
        {
            this.When(() => new NmeaAisAssignedModeCommandParser(Encoding.ASCII.GetBytes(payload), padding));
        }

        [Then(@"NmeaAisAssignedModeCommandParser\.Type is (.*)")]
        public void ThenNmeaAisAssignedModeCommandParser_TypeIs(MessageType messageType)
        {
            this.Then(parser => Assert.AreEqual(messageType, parser.MessageType));
        }

        [Then(@"NmeaAisAssignedModeCommandParser\.RepeatIndicator is (.*)")]
        public void ThenNmeaAisAssignedModeCommandParser_RepeatIndicatorIs(uint repeatCount)
        {
            this.Then(parser => Assert.AreEqual(repeatCount, parser.RepeatIndicator));
        }

        [Then(@"NmeaAisAssignedModeCommandParser\.Mmsi is (.*)")]
        public void ThenNmeaAisAssignedModeCommandParser_MmsiIs(uint mmsi)
        {
            this.Then(parser => Assert.AreEqual(mmsi, parser.Mmsi));
        }

        [Then(@"NmeaAisAssignedModeCommandParser\.SpareBits38 is (.*)")]
        public void ThenNmeaAisAssignedModeCommandParser_Is(uint value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.SpareBits38));
        }

        [Then(@"NmeaAisAssignedModeCommandParser\.DestinationMmsiA is (.*)")]
        public void ThenNmeaAisAssignedModeCommandParser_DestinationMmsiAIs(uint value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.DestinationMmsiA));
        }

        [Then(@"NmeaAisAssignedModeCommandParser\.OffsetA is (.*)")]
        public void ThenNmeaAisAssignedModeCommandParser_OffsetAIs(uint value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.OffsetA));
        }

        [Then(@"NmeaAisAssignedModeCommandParser\.IncrementA is (.*)")]
        public void ThenNmeaAisAssignedModeCommandParser_IncrementAIs(uint value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.IncrementA));
        }

        [Then(@"NmeaAisAssignedModeCommandParser\.DestinationMmsiB is (.*)")]
        public void ThenNmeaAisAssignedModeCommandParser_DestinationMmsiBIs(uint? value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.DestinationMmsiB));
        }

        [Then(@"NmeaAisAssignedModeCommandParser\.OffsetB is (.*)")]
        public void ThenNmeaAisAssignedModeCommandParser_OffsetBIs(uint? value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.OffsetB));
        }

        [Then(@"NmeaAisAssignedModeCommandParser\.IncrementB is (.*)")]
        public void ThenNmeaAisAssignedModeCommandParser_IncrementBIs(uint? value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.IncrementB));
        }

        [Then(@"NmeaAisAssignedModeCommandParser\.SpareBitsAtEnd is (.*)")]
        public void ThenNmeaAisAssignedModeCommandParser_SpareBitsAtEndAIs(uint? value)
        {
            this.Then(parser => Assert.AreEqual(value, parser.SpareBitsAtEnd));
        }

        private void When(ParserMaker makeParser)
        {
            this.makeParser = makeParser;
        }

        private void Then(ParserTest test)
        {
            NmeaAisAssignedModeCommandParser parser = this.makeParser();
            test(parser);
        }
    }
}
