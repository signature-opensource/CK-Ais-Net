// <copyright file="NmeaAisBitVectorParserSpecsSteps.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using NUnit.Framework;
using System;
using System.Text;
using TechTalk.SpecFlow;

namespace Ais.Net.Specs
{
    [Binding]
    public class NmeaAisBitVectorParserSpecsSteps
    {
        ParserMaker? _makeParser;
        uint _unsignedIntegerResult;
        int _signedIntegerResult;

        delegate NmeaAisBitVectorParser ParserMaker();

        delegate void ParserTest( NmeaAisBitVectorParser parser );

        [Given( "an NMEA AIS payload of '(.*)' and padding (.*)" )]
        public void GivenAnNMEAAISPayloadOfAndPadding( string payload, uint padding )
        {
            Given( () => new NmeaAisBitVectorParser( Encoding.ASCII.GetBytes( payload ), padding ) );
        }

        [When( "I read an unsigned (.*) bit int at offset (.*)" )]
        public void WhenIReadAnUnsignedBitIntAtOffset( uint bitCount, uint offset )
        {
            When( p => _unsignedIntegerResult = p.GetUnsignedInteger( bitCount, offset ) );
        }

        [When( "I read a signed (.*) bit int at offset (.*)" )]
        public void WhenIReadASignedBitIntAtOffset( uint bitCount, uint offset )
        {
            When( p => _signedIntegerResult = p.GetSignedInteger( bitCount, offset ) );
        }

        [Then( "the NmeaAisBitVectorParser returns an unsigned integer with value (.*)" )]
        public void ThenTheNmeaAisBitVectorParserReturnsAnUnsignedIntegerWithValue( int expectedValue )
        {
            Assert.AreEqual( expectedValue, _unsignedIntegerResult );
        }

        [Then( "the NmeaAisBitVectorParser returns an signed integer with value (.*)" )]
        public void ThenTheNmeaAisBitVectorParserReturnsAnSignedIntegerWithValue( int expectedValue )
        {
            Assert.AreEqual( expectedValue, _signedIntegerResult );
        }

        void Given( ParserMaker makeParser )
        {
            _makeParser = makeParser;
        }

        void When( ParserTest test )
        {
            if( _makeParser is null ) throw new InvalidOperationException( $"When step must be called." );
            NmeaAisBitVectorParser parser = _makeParser();
            test( parser );
        }
    }
}
