// <copyright file="ParsePayloadSpecsSteps.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using NUnit.Framework;
using System.Text;
using TechTalk.SpecFlow;

namespace Ais.Net.Specs;

[Binding]
public class ParsePayloadSpecsSteps
{
    int _peekedType;

    [When( "I peek at the payload '(.*)' with padding of (.*)" )]
    public void WhenIPeekAtThePayloadWithPaddingOf( string payload, uint padding )
    {
        _peekedType = NmeaPayloadParser.PeekMessageType( Encoding.ASCII.GetBytes( payload ), padding );
    }

    [Then( "the message type returned by peek should be (.*)" )]
    public void ThenTheMessageTypeReturnedByPeekShouldBe( int type )
    {
        Assert.AreEqual( type, _peekedType );
    }
}
