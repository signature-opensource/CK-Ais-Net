namespace Ais.Net.Specs
{
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TechTalk.SpecFlow;

    [Binding]
    public class NmeaTagBlockParserSpecsSteps
    {
        private readonly StringBuilder content = new StringBuilder();
        private readonly NmeaAisMessageStreamProcessorBindings messageProcessor;

        private ParserMaker makeParser;

        public NmeaTagBlockParserSpecsSteps(NmeaAisMessageStreamProcessorBindings messageProcessor)
        {
            this.messageProcessor = messageProcessor;
        }

        private delegate NmeaTagBlockParser ParserMaker();

        private delegate void ParserTest(NmeaTagBlockParser parser);

        [Given("the line '(.*)'")]
        public void GivenALine(string line)
        {
            this.content.Append(line);
            this.content.Append('\n');
        }

        [When("I parse '(.*)' with throwWhenTagBlockContainsUnknownFields of (.*) and tagBlockStandard of (.*) as a NMEA tag block parser")]
        public void WhenIParseWithThrowWhenTagBlockContainsUnknownFieldsOfAndTagBlockStandardOfAsANMEATagBlockParser(string messageLine, bool throwWhenTagBlockContainsUnknownFields, TagBlockStandard tagBlockStandard)
        {
            this.When(messageLine, throwWhenTagBlockContainsUnknownFields, tagBlockStandard);
        }

        [When("I parse the content by message with throwWhenTagBlockContainsUnknownFields of (.*) and tagBlockStandard of (.*)")]
        public async Task WhenIParseTheContentByMessageAsync(bool throwWhenTagBlockContainsUnknownFields, TagBlockStandard tagBlockStandard)
        {
            await NmeaStreamParser.ParseStreamAsync(
                new MemoryStream(Encoding.ASCII.GetBytes(this.content.ToString())),
                new NmeaLineToAisStreamAdapter(this.messageProcessor.Processor),
                new NmeaParserOptions { ThrowWhenTagBlockContainsUnknownFields = throwWhenTagBlockContainsUnknownFields, TagBlockStandard = tagBlockStandard }).ConfigureAwait(false);
        }

        [Then("the Source is (.*)")]
        public void ThenSourceIs(string source)
        {
            this.Then(parser => Assert.AreEqual(source, Encoding.ASCII.GetString(parser.Source)));
        }

        [Then("the Timestamp is (.*)")]
        public void ThenTimestampIs(double source)
        {
            this.Then(parser =>
            {
                Assert.IsTrue(parser.UnixTimestamp.HasValue);
                Assert.AreEqual(source, parser.UnixTimestamp.Value);
            });
        }

        [Then("the SentenceGrouping is (.*) (.*) (.*)")]
        public void ThenSentenceGroupingIs(int sentenceNumber, int sentencesInGroup, int groupId)
        {
            this.Then(parser =>
            {
                Assert.IsTrue(parser.SentenceGrouping.HasValue);
                Assert.AreEqual(sentenceNumber, parser.SentenceGrouping.Value.SentenceNumber);
                Assert.AreEqual(sentencesInGroup, parser.SentenceGrouping.Value.SentencesInGroup);
                Assert.AreEqual(groupId, parser.SentenceGrouping.Value.GroupId);
            });
        }

        [Then("the SentenceGrouping is null")]
        public void ThenSentenceGroupingIsNull()
        {
            this.Then(parser => Assert.IsFalse(parser.SentenceGrouping.HasValue));
        }

        [Then("the message error report (.*) should include the error message '(.*)'")]
        public void ThenTheLineErrorReportShouldIncludeTheProblematicLine(int errorCallNumber, string errorMessage)
        {
            NmeaAisMessageStreamProcessorBindings.ErrorReport call = this.messageProcessor.OnErrorCalls[errorCallNumber];
            Assert.AreEqual(errorMessage, call.Error.Message);
        }

        [Then("the TextString is (.*)")]
        public void TheTheTextStringIs(string text)
        {
            this.Then(parser => Assert.AreEqual(text, Encoding.ASCII.GetString(parser.TextString)));
        }

        private void When(string messageLine, bool throwWhenTagBlockContainsUnknownFields, TagBlockStandard tagBlockStandard)
        {
            this.When(() => new NmeaTagBlockParser(Encoding.ASCII.GetBytes(messageLine), throwWhenTagBlockContainsUnknownFields, tagBlockStandard));
        }

        private void When(ParserMaker makeParser)
        {
            this.makeParser = makeParser;
        }

        private void Then(ParserTest test)
        {
            NmeaTagBlockParser parser = this.makeParser();
            test(parser);
        }
    }
}
