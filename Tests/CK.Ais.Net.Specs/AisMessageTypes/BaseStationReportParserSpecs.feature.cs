﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (https://www.specflow.org/).
//      SpecFlow Version:3.9.0.0
//      SpecFlow Generator Version:3.9.0.0
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace CK.Ais.Net.Specs.AisMessageTypes
{
    using TechTalk.SpecFlow;
    using System;
    using System.Linq;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.9.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("BaseStationReportParserSpecs")]
    public partial class BaseStationReportParserSpecsFeature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
        private static string[] featureTags = ((string[])(null));
        
#line 1 "BaseStationReportParserSpecs.feature"
#line hidden
        
        [NUnit.Framework.OneTimeSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "AisMessageTypes", "BaseStationReportParserSpecs", "    In order process AIS messages from an nm4 file\r\n    As a developer\r\n    I wan" +
                    "t the BaseStationReportParserSpecs to be able to parse the payload section of me" +
                    "ssage type 4: Base Station Report", ProgrammingLanguage.CSharp, featureTags);
            testRunner.OnFeatureStart(featureInfo);
        }
        
        [NUnit.Framework.OneTimeTearDownAttribute()]
        public virtual void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        [NUnit.Framework.SetUpAttribute()]
        public void TestInitialize()
        {
        }
        
        [NUnit.Framework.TearDownAttribute()]
        public void TestTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public void ScenarioInitialize(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioInitialize(scenarioInfo);
            testRunner.ScenarioContext.ScenarioContainer.RegisterInstanceAs<NUnit.Framework.TestContext>(NUnit.Framework.TestContext.CurrentContext);
        }
        
        public void ScenarioStart()
        {
            testRunner.OnScenarioStart();
        }
        
        public void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Message Type")]
        public void MessageType()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Message Type", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 8
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 9
    testRunner.When("I parse \'4028j;iu<JAU80>f7>H0elQ00000\' with padding 0 as a Base Station Report", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 10
    testRunner.Then("NmeaAisBaseStationReportParser.Type is 4", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Full message")]
        [NUnit.Framework.TestCaseAttribute("4028j;iu<JAU80>f7>H0elQ00000", "0", "4", "0", "2241071", "2003", "1", "20", "17", "37", "8", "false", "3.215745", "41.96259", "1", "false", "0", "false", "0", null)]
        [NUnit.Framework.TestCaseAttribute("44`Uu;AvJEF`g14>V0DV@MQ00000", "0", "4", "0", "311000365", "2022", "9", "10", "22", "40", "47", "false", "14.90464", "35.99721", "1", "false", "0", "false", "0", null)]
        public void FullMessage(
                    string payload, 
                    string padding, 
                    string type, 
                    string repeatindicator, 
                    string mmsi, 
                    string utcyear, 
                    string utcmonth, 
                    string utcday, 
                    string utchour, 
                    string utcminute, 
                    string utcsecond, 
                    string positionaccuracy, 
                    string longitude10000Thmins, 
                    string latitude10000Thmins, 
                    string positionfixtype, 
                    string transmissioncontrolforlongrangebroadcastmessage, 
                    string sparebits139, 
                    string raimflag, 
                    string communicationstate, 
                    string[] exampleTags)
        {
            string[] tagsOfScenario = exampleTags;
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            argumentsOfScenario.Add("payload", payload);
            argumentsOfScenario.Add("padding", padding);
            argumentsOfScenario.Add("type", type);
            argumentsOfScenario.Add("repeatindicator", repeatindicator);
            argumentsOfScenario.Add("mmsi", mmsi);
            argumentsOfScenario.Add("utcyear", utcyear);
            argumentsOfScenario.Add("utcmonth", utcmonth);
            argumentsOfScenario.Add("utcday", utcday);
            argumentsOfScenario.Add("utchour", utchour);
            argumentsOfScenario.Add("utcminute", utcminute);
            argumentsOfScenario.Add("utcsecond", utcsecond);
            argumentsOfScenario.Add("positionaccuracy", positionaccuracy);
            argumentsOfScenario.Add("longitude10000thmins", longitude10000Thmins);
            argumentsOfScenario.Add("latitude10000thmins", latitude10000Thmins);
            argumentsOfScenario.Add("positionfixtype", positionfixtype);
            argumentsOfScenario.Add("transmissioncontrolforlongrangebroadcastmessage", transmissioncontrolforlongrangebroadcastmessage);
            argumentsOfScenario.Add("sparebits139", sparebits139);
            argumentsOfScenario.Add("raimflag", raimflag);
            argumentsOfScenario.Add("communicationstate", communicationstate);
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Full message", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 12
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 13
    testRunner.When(string.Format("I parse \'{0}\' with padding {1} as a Base Station Report", payload, padding), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 14
    testRunner.Then(string.Format("NmeaAisBaseStationReportParser.Type is {0}", type), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 15
    testRunner.And(string.Format("NmeaAisBaseStationReportParser.RepeatIndicator is {0}", repeatindicator), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 16
    testRunner.And(string.Format("NmeaAisBaseStationReportParser.Mmsi is {0}", mmsi), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 17
    testRunner.And(string.Format("NmeaAisBaseStationReportParser.UtcYear is {0}", utcyear), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 18
    testRunner.And(string.Format("NmeaAisBaseStationReportParser.UtcMonth is {0}", utcmonth), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 19
    testRunner.And(string.Format("NmeaAisBaseStationReportParser.UtcDay is {0}", utcday), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 20
    testRunner.And(string.Format("NmeaAisBaseStationReportParser.UtcHour is {0}", utchour), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 21
    testRunner.And(string.Format("NmeaAisBaseStationReportParser.UtcMinute is {0}", utcminute), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 22
    testRunner.And(string.Format("NmeaAisBaseStationReportParser.UtcSecond is {0}", utcsecond), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 23
    testRunner.And(string.Format("NmeaAisBaseStationReportParser.PositionAccuracy is {0}", positionaccuracy), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 24
    testRunner.And(string.Format("NmeaAisBaseStationReportParser.Longitude10000thMins is {0}", longitude10000Thmins), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 25
    testRunner.And(string.Format("NmeaAisBaseStationReportParser.Latitude10000thMins is {0}", latitude10000Thmins), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 26
    testRunner.And(string.Format("NmeaAisBaseStationReportParser.PositionFixType is {0}", positionfixtype), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 27
    testRunner.And(string.Format("NmeaAisBaseStationReportParser.TransmissionControlForLongRangeBroadcastMessage is" +
                            " {0}", transmissioncontrolforlongrangebroadcastmessage), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 28
    testRunner.And(string.Format("NmeaAisBaseStationReportParser.SpareBits139 is {0}", sparebits139), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 29
    testRunner.And(string.Format("NmeaAisBaseStationReportParser.RaimFlag is {0}", raimflag), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 30
    testRunner.And(string.Format("NmeaAisBaseStationReportParser.CommunicationState is {0}", communicationstate), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
