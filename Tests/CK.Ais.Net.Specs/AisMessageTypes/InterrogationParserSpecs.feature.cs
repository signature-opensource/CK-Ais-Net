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
    [NUnit.Framework.DescriptionAttribute("InterrogationSpecsSteps")]
    public partial class InterrogationSpecsStepsFeature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
        private static string[] featureTags = ((string[])(null));
        
#line 1 "InterrogationParserSpecs.feature"
#line hidden
        
        [NUnit.Framework.OneTimeSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "AisMessageTypes", "InterrogationSpecsSteps", "    In order process AIS messages from an nm4 file\r\n    As a developer\r\n    I wan" +
                    "t the NmeaAisInterrogationParser to be able to parse the payload section of mess" +
                    "age type 15: Interrogation", ProgrammingLanguage.CSharp, featureTags);
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
    testRunner.When("I parse \'?913QK18Uf;0D00\' with padding 0 as an Interrogation", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 10
    testRunner.Then("NmeaAisInterrogationParser.Type is 15", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Full message")]
        [NUnit.Framework.TestCaseAttribute("?913QK18Uf;0D00", "0", "15", "0", "605086060", "0", "304462000", "5", "0", "0", "", "", "", "", "", "", "", null)]
        [NUnit.Framework.TestCaseAttribute("?1b60U0kNVOP<005000", "0", "15", "0", "111247508", "0", "215915000", "3", "0", "0", "5", "0", "0", "", "", "", "", null)]
        [NUnit.Framework.TestCaseAttribute("?77>7w1iprE@D00", "0", "15", "0", "477333500", "0", "477686100", "5", "0", "0", "", "", "", "", "", "", "", null)]
        public void FullMessage(
                    string payload, 
                    string padding, 
                    string type, 
                    string repeatindicator, 
                    string mmsi, 
                    string spare38, 
                    string destinationmmsi1, 
                    string messagetype11, 
                    string slotoffset11, 
                    string spare88, 
                    string messagetype12, 
                    string slotoffset12, 
                    string spare108, 
                    string destinationmmsi2, 
                    string mmessagetype21, 
                    string slotoffset21, 
                    string spare158, 
                    string[] exampleTags)
        {
            string[] tagsOfScenario = exampleTags;
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            argumentsOfScenario.Add("payload", payload);
            argumentsOfScenario.Add("padding", padding);
            argumentsOfScenario.Add("type", type);
            argumentsOfScenario.Add("repeatindicator", repeatindicator);
            argumentsOfScenario.Add("mmsi", mmsi);
            argumentsOfScenario.Add("spare38", spare38);
            argumentsOfScenario.Add("destinationmmsi1", destinationmmsi1);
            argumentsOfScenario.Add("messagetype11", messagetype11);
            argumentsOfScenario.Add("slotoffset11", slotoffset11);
            argumentsOfScenario.Add("spare88", spare88);
            argumentsOfScenario.Add("messagetype12", messagetype12);
            argumentsOfScenario.Add("slotoffset12", slotoffset12);
            argumentsOfScenario.Add("spare108", spare108);
            argumentsOfScenario.Add("destinationmmsi2", destinationmmsi2);
            argumentsOfScenario.Add("mmessagetype21", mmessagetype21);
            argumentsOfScenario.Add("slotoffset21", slotoffset21);
            argumentsOfScenario.Add("spare158", spare158);
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
    testRunner.When(string.Format("I parse \'{0}\' with padding {1} as an Interrogation", payload, padding), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 14
    testRunner.Then(string.Format("NmeaAisInterrogationParser.Type is {0}", type), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 15
    testRunner.And(string.Format("NmeaAisInterrogationParser.RepeatIndicator is {0}", repeatindicator), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 16
    testRunner.And(string.Format("NmeaAisInterrogationParser.Mmsi is {0}", mmsi), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 17
    testRunner.And(string.Format("NmeaAisInterrogationParser.SpareBits38 is {0}", spare38), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 18
    testRunner.And(string.Format("NmeaAisInterrogationParser.DestinationMmsi1 is {0}", destinationmmsi1), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 19
    testRunner.And(string.Format("NmeaAisInterrogationParser.MessageType11 is {0}", messagetype11), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 20
    testRunner.And(string.Format("NmeaAisInterrogationParser.SlotOffset11 is {0}", slotoffset11), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 21
    testRunner.And(string.Format("NmeaAisInterrogationParser.SpareBits88 is {0}", spare88), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 22
    testRunner.And(string.Format("NmeaAisInterrogationParser.MessageType12 is {0}", messagetype12), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 23
    testRunner.And(string.Format("NmeaAisInterrogationParser.SlotOffset12 is {0}", slotoffset12), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 24
    testRunner.And(string.Format("NmeaAisInterrogationParser.SpareBits108 is {0}", spare108), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 25
    testRunner.And(string.Format("NmeaAisInterrogationParser.DestinationMmsi2 is {0}", destinationmmsi2), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 26
    testRunner.And(string.Format("NmeaAisInterrogationParser.MessageType21 is {0}", mmessagetype21), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 27
    testRunner.And(string.Format("NmeaAisInterrogationParser.SlotOffset21 is {0}", slotoffset21), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 28
    testRunner.And(string.Format("NmeaAisInterrogationParser.SpareBits158 is {0}", spare158), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion