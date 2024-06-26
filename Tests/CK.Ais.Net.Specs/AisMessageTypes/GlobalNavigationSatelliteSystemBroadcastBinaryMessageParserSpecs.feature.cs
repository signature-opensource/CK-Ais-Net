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
    [NUnit.Framework.DescriptionAttribute("GlobalNavigationSatelliteSystemBroadcastBinaryMessageParserSpecsSteps")]
    public partial class GlobalNavigationSatelliteSystemBroadcastBinaryMessageParserSpecsStepsFeature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
        private static string[] featureTags = ((string[])(null));
        
#line 1 "GlobalNavigationSatelliteSystemBroadcastBinaryMessageParserSpecs.feature"
#line hidden
        
        [NUnit.Framework.OneTimeSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "AisMessageTypes", "GlobalNavigationSatelliteSystemBroadcastBinaryMessageParserSpecsSteps", @"    In order process AIS messages from an nm4 file
    As a developer
    I want the NmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser to be able to parse the payload section of message type 17: Global Navigation-Satellite System Broadcast Binary Message", ProgrammingLanguage.CSharp, featureTags);
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
    testRunner.When("I parse \'A028nBCt@hbs02Gvd0H@2gop2ABb\' with padding 0 as a Global Navigation-Sate" +
                        "llite System Broadcast Binary Message", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 10
    testRunner.Then("NmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser.Type is 17", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Full message")]
        [NUnit.Framework.TestCaseAttribute("A028nBCt@hbs02GvJ:0`5?ku1ET:wdh69@gvkhH?", "0", "17", "0", "2242121", "0", "-3828", "21976", "0", "2", "2GvJ:0`5?ku1ET:wdh69@gvkhH?", null)]
        [NUnit.Framework.TestCaseAttribute("A028nBCt@hbs02Gvd0H@2gop2ABb", "0", "17", "0", "2242121", "0", "-3828", "21976", "0", "2", "2Gvd0H@2gop2ABb", null)]
        [NUnit.Framework.TestCaseAttribute("A028jQ02QLfep2H<UhDP5wpk1CdJw@p42:bb", "0", "17", "0", "2241156", "0", "2583", "23919", "0", "2", "2H<UhDP5wpk1CdJw@p42:bb", null)]
        public void FullMessage(string payload, string padding, string type, string repeatindicator, string mmsi, string spare38, string longitude, string latitude, string spare75, string differentialpadding, string differential, string[] exampleTags)
        {
            string[] tagsOfScenario = exampleTags;
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            argumentsOfScenario.Add("payload", payload);
            argumentsOfScenario.Add("padding", padding);
            argumentsOfScenario.Add("type", type);
            argumentsOfScenario.Add("repeatindicator", repeatindicator);
            argumentsOfScenario.Add("mmsi", mmsi);
            argumentsOfScenario.Add("spare38", spare38);
            argumentsOfScenario.Add("longitude", longitude);
            argumentsOfScenario.Add("latitude", latitude);
            argumentsOfScenario.Add("spare75", spare75);
            argumentsOfScenario.Add("differentialpadding", differentialpadding);
            argumentsOfScenario.Add("differential", differential);
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
    testRunner.When(string.Format("I parse \'{0}\' with padding {1} as a Global Navigation-Satellite System Broadcast " +
                            "Binary Message", payload, padding), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 14
    testRunner.Then(string.Format("NmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser.Type is {0}", type), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 15
    testRunner.And(string.Format("NmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser.RepeatIndicato" +
                            "r is {0}", repeatindicator), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 16
    testRunner.And(string.Format("NmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser.Mmsi is {0}", mmsi), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 17
    testRunner.And(string.Format("NmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser.SpareBits38 is" +
                            " {0}", spare38), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 18
    testRunner.And(string.Format("NmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser.Longitude10thM" +
                            "ins is {0}", longitude), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 19
    testRunner.And(string.Format("NmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser.Latitude10thMi" +
                            "ns is {0}", latitude), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 20
    testRunner.And(string.Format("NmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser.SpareBits75 is" +
                            " {0}", spare75), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 21
    testRunner.And(string.Format("NmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser.DifferentialCo" +
                            "rrectionDataPadding is {0}", differentialpadding), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 22
    testRunner.And(string.Format("NmeaAisGlobalNavigationSatelliteSystemBroadcastBinaryMessageParser.DifferentialCo" +
                            "rrectionData is {0}", differential), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
