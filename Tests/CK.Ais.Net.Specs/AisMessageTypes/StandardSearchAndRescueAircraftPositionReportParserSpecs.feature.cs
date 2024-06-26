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
    [NUnit.Framework.DescriptionAttribute("StandardSearchAndRescueAircraftPositionReportParserSpecsSteps")]
    public partial class StandardSearchAndRescueAircraftPositionReportParserSpecsStepsFeature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
        private static string[] featureTags = ((string[])(null));
        
#line 1 "StandardSearchAndRescueAircraftPositionReportParserSpecs.feature"
#line hidden
        
        [NUnit.Framework.OneTimeSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "AisMessageTypes", "StandardSearchAndRescueAircraftPositionReportParserSpecsSteps", @"    In order process AIS messages from an nm4 file
    As a developer
    I want the NmeaAisStandardSearchAndRescueAircraftPositionReportParser to be able to parse the payload section of message type 9: Standard Search and Rescue Aircraft Position Report", ProgrammingLanguage.CSharp, featureTags);
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
    testRunner.When("I parse \'95M2oQ@41Tr4L4H@eRvQ;2h20000\' with padding 0 as a Standard Search and Re" +
                        "scue Aircraft Position Report", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 10
    testRunner.Then("NmeaAisStandardSearchAndRescueAircraftPositionReportParser.Type is 9", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Full message")]
        [NUnit.Framework.TestCaseAttribute("95M2oQ@41Tr4L4H@eRvQ;2h20000", "0", "9", "0", "366000005", "16", "100", "true", "-49749876", "17523450", "300", "11", "0", "0", "true", "0", "false", "false", "0", "0", null)]
        public void FullMessage(
                    string payload, 
                    string padding, 
                    string type, 
                    string repeatindicator, 
                    string mmsi, 
                    string altitude, 
                    string sog, 
                    string positionaccuracy, 
                    string longitude, 
                    string latitude, 
                    string cog, 
                    string timestamp, 
                    string altitudesensor, 
                    string spare135, 
                    string dte, 
                    string spare143, 
                    string assignedmode, 
                    string raimflag, 
                    string communicationstateselector, 
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
            argumentsOfScenario.Add("altitude", altitude);
            argumentsOfScenario.Add("sog", sog);
            argumentsOfScenario.Add("positionaccuracy", positionaccuracy);
            argumentsOfScenario.Add("longitude", longitude);
            argumentsOfScenario.Add("latitude", latitude);
            argumentsOfScenario.Add("cog", cog);
            argumentsOfScenario.Add("timestamp", timestamp);
            argumentsOfScenario.Add("altitudesensor", altitudesensor);
            argumentsOfScenario.Add("spare135", spare135);
            argumentsOfScenario.Add("dte", dte);
            argumentsOfScenario.Add("spare143", spare143);
            argumentsOfScenario.Add("assignedmode", assignedmode);
            argumentsOfScenario.Add("raimflag", raimflag);
            argumentsOfScenario.Add("communicationstateselector", communicationstateselector);
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
    testRunner.When(string.Format("I parse \'{0}\' with padding {1} as a Standard Search and Rescue Aircraft Position " +
                            "Report", payload, padding), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 14
    testRunner.Then(string.Format("NmeaAisStandardSearchAndRescueAircraftPositionReportParser.Type is {0}", type), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 15
    testRunner.And(string.Format("NmeaAisStandardSearchAndRescueAircraftPositionReportParser.RepeatIndicator is {0}" +
                            "", repeatindicator), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 16
    testRunner.And(string.Format("NmeaAisStandardSearchAndRescueAircraftPositionReportParser.Mmsi is {0}", mmsi), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 17
    testRunner.And(string.Format("NmeaAisStandardSearchAndRescueAircraftPositionReportParser.Altitude is {0}", altitude), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 18
    testRunner.And(string.Format("NmeaAisStandardSearchAndRescueAircraftPositionReportParser.SpeedOverGround is {0}" +
                            "", sog), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 19
    testRunner.And(string.Format("NmeaAisStandardSearchAndRescueAircraftPositionReportParser.PositionAccuracy is {0" +
                            "}", positionaccuracy), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 20
    testRunner.And(string.Format("NmeaAisStandardSearchAndRescueAircraftPositionReportParser.Longitude10000thMins i" +
                            "s {0}", longitude), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 21
    testRunner.And(string.Format("NmeaAisStandardSearchAndRescueAircraftPositionReportParser.Latitude10000thMins is" +
                            " {0}", latitude), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 22
    testRunner.And(string.Format("NmeaAisStandardSearchAndRescueAircraftPositionReportParser.CourseOverGround10thDe" +
                            "grees is {0}", cog), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 23
    testRunner.And(string.Format("NmeaAisStandardSearchAndRescueAircraftPositionReportParser.TimeStampSecond is {0}" +
                            "", timestamp), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 24
    testRunner.And(string.Format("NmeaAisStandardSearchAndRescueAircraftPositionReportParser.AltitudeSensor is {0}", altitudesensor), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 25
    testRunner.And(string.Format("NmeaAisStandardSearchAndRescueAircraftPositionReportParser.SpareBits135 is {0}", spare135), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 26
    testRunner.And(string.Format("NmeaAisStandardSearchAndRescueAircraftPositionReportParser.DTE is {0}", dte), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 27
    testRunner.And(string.Format("NmeaAisStandardSearchAndRescueAircraftPositionReportParser.SpareBits143 is {0}", spare143), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 28
    testRunner.And(string.Format("NmeaAisStandardSearchAndRescueAircraftPositionReportParser.AssignedMode is {0}", assignedmode), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 29
    testRunner.And(string.Format("NmeaAisStandardSearchAndRescueAircraftPositionReportParser.RaimFlag is {0}", raimflag), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 30
    testRunner.And(string.Format("NmeaAisStandardSearchAndRescueAircraftPositionReportParser.CommunicationStateSele" +
                            "ctor is {0}", communicationstateselector), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 31
    testRunner.And(string.Format("NmeaAisStandardSearchAndRescueAircraftPositionReportParser.CommunicationState is " +
                            "{0}", communicationstate), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
