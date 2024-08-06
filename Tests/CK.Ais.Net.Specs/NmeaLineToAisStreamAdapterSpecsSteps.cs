// <copyright file="NmeaLineToAisStreamAdapterSpecsSteps.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using NUnit.Framework;
using System;
using System.Text;
using TechTalk.SpecFlow;

namespace Ais.Net.Specs
{
    [Binding]
    public class NmeaLineToAisStreamAdapterSpecsSteps : IDisposable
    {
        readonly NmeaAisMessageStreamProcessorBindings _processorBindings;
        readonly NmeaParserOptions _parserOptions = new();
        NmeaLineToAisStreamAdapter<DefaultExtraFieldParser>? _adapter;
        bool _adapterOnCompleteCalled = false;
        Exception? _exceptionProvidedToProcessor;
        int _lineNumber = 1;

        public NmeaLineToAisStreamAdapterSpecsSteps( NmeaAisMessageStreamProcessorBindings processorBindings )
        {
            _processorBindings = processorBindings;
        }

        private NmeaLineToAisStreamAdapter<DefaultExtraFieldParser> Adapter =>
            _adapter ??= new NmeaLineToAisStreamAdapter<DefaultExtraFieldParser>( _processorBindings.Processor, _parserOptions );

        [Then( "the message error report (.*) should include the exception reported by the line stream parser" )]
        public void ThenTheMessageErrorReportShouldIncludeTheExceptionReportedByTheLineStreamParser( int errorCallNumber )
        {
            var call = _processorBindings.OnErrorCalls[errorCallNumber];
            Assert.AreSame( _exceptionProvidedToProcessor, call.Error );
        }

        public void Dispose()
        {
            if( !_adapterOnCompleteCalled )
            {
                Adapter.OnCompleted();
                _adapterOnCompleteCalled = true;
            }

            Adapter.Dispose();
        }

        [Given( "I have configured a MaximumUnmatchedFragmentAge of (.*)" )]
        public void GivenIHaveConfiguredAMaximumUnmatchedFragmentAgeOf( int maximumUnmatchedFragmentAge )
        {
            _parserOptions.MaximumUnmatchedFragmentAge = maximumUnmatchedFragmentAge;
        }

        [Given( "I have configured a empty group tolerance of (.*)" )]
        public void GivenIHaveConfiguredAMaximumUnmatchedFragmentAgeOf( EmptyGroupTolerance tolerance )
        {
            _parserOptions.EmptyGroupTolerance = tolerance;
        }

        [When( "the line to message adapter receives '(.*)'" )]
        public void WhenTheLineToMessageAdapterReceives( string line )
        {
            byte[] ascii = Encoding.ASCII.GetBytes( line );
            var lineParser = new NmeaLineParser<DefaultExtraFieldParser>( ascii, _parserOptions.ThrowWhenTagBlockContainsUnknownFields, _parserOptions.TagBlockStandard, _parserOptions.EmptyGroupTolerance );
            Adapter.OnNext( lineParser, _lineNumber++ );
        }

        [When( "the line to message adapter receives an error report for content '(.*)' with line number (.*)" )]
        public void WhenTheLineToMessageAdapterReceivesAnErrorReportForContentWithLineNumber( string line, int lineNumber )
        {
            byte[] ascii = Encoding.ASCII.GetBytes( line );
            _exceptionProvidedToProcessor = new ArgumentException( "That was never 5 minutes" );
            Adapter.OnError( ascii, _exceptionProvidedToProcessor, lineNumber );
        }

        [When( "the line to message adapter receives an error report for invalid content '(.*)' with line number (.*)" )]
        public void WhenTheLineToMessageAdapterReceivesAnErrorReportForInvalidContentWithLineNumber( string line, int lineNumber )
        {
            byte[] ascii = Encoding.ASCII.GetBytes( line );
            try
            {
                var lineParser = new NmeaLineParser<DefaultExtraFieldParser>( ascii, _parserOptions.ThrowWhenTagBlockContainsUnknownFields, _parserOptions.TagBlockStandard, _parserOptions.EmptyGroupTolerance );
                Assert.Fail( $"No throw when parsing line '{line}'." );
            }
            catch( Exception e )
            {
                _exceptionProvidedToProcessor = e;
                Adapter.OnError( ascii, _exceptionProvidedToProcessor, lineNumber );
            }
        }

        [When( "the line to message adapter receives a progress report of (.*), (.*), (.*), (.*), (.*)" )]
        public void WhenTheLineToMessageAdapterReceivesAProgressReportOfFalse(
            bool done, int totalLines, int totalTicks, int linesSinceLastUpdate, int ticksSinceLastUpdate )
        {
            Adapter.Progress( done, totalLines, totalTicks, linesSinceLastUpdate, ticksSinceLastUpdate );
        }
    }
}
