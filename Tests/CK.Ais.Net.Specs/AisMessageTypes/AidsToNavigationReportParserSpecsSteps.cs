using NUnit.Framework;
using System;
using System.Text;
using TechTalk.SpecFlow;

namespace Ais.Net.Specs.AisMessageTypes;

[Binding]
public class AidsToNavigationReportParserSpecsSteps
{
    ParserMaker? _makeParser;

    delegate NmeaAisAidsToNavigationReportParser ParserMaker();

    delegate void ParserTest( NmeaAisAidsToNavigationReportParser parser );

    [When( "I parse '(.*)' with padding (.*) as a Aids to Navigation Report" )]
    public void WhenIParseWithNmeaAisAidsToNavigationReportParser( string payload, uint padding )
    {
        When( () => new NmeaAisAidsToNavigationReportParser( Encoding.ASCII.GetBytes( payload ), padding ) );
    }

    [Then( @"NmeaAisAidsToNavigationReportParser\.Type is (.*)" )]
    public void ThenNmeaAisAidsToNavigationReportParser_TypeIs( MessageType messageType )
    {
        Then( parser => Assert.AreEqual( messageType, parser.MessageType ) );
    }

    [Then( @"NmeaAisAidsToNavigationReportParser\.RepeatIndicator is (.*)" )]
    public void ThenNmeaAisAidsToNavigationReportParser_RepeatIndicatorIs( uint repeatCount )
    {
        Then( parser => Assert.AreEqual( repeatCount, parser.RepeatIndicator ) );
    }

    [Then( @"NmeaAisAidsToNavigationReportParser\.Mmsi is (.*)" )]
    public void ThenNmeaAisAidsToNavigationReportParser_MmsiIs( uint mmsi )
    {
        Then( parser => Assert.AreEqual( mmsi, parser.Mmsi ) );
    }

    [Then( @"NmeaAisAidsToNavigationReportParser\.AidsToNavigationType is (.*)" )]
    public void ThenNmeaAisAidsToNavigationReportParser_AidsToNavigationTypeIs( AidsToNavigationType value )
    {
        Then( parser => Assert.AreEqual( value, parser.AidsToNavigationType ) );
    }

    [Then( @"NmeaAisAidsToNavigationReportParser\.NameOfAidsToNavigation is (.*)" )]
    public void ThenNmeaAisAidsToNavigationReportParser_NameOfAidsToNavigationIs( string value )
    {
        Then( parser =>
        {
            byte[] bytes = new byte[parser.NameOfAidsToNavigation.CharacterCount];
            parser.NameOfAidsToNavigation.WriteAsAscii( bytes );
            Assert.AreEqual( value, Encoding.ASCII.GetString( bytes ) );
        } );
    }

    [Then( @"NmeaAisAidsToNavigationReportParser\.PositionAccuracy is (.*)" )]
    public void ThenNmeaAisAidsToNavigationReportParser_PositionAccuracyIs( bool value )
    {
        Then( parser => Assert.AreEqual( value, parser.PositionAccuracy ) );
    }

    [Then( @"NmeaAisAidsToNavigationReportParser\.Longitude10000thMins is (.*)" )]
    public void ThenNmeaAisAidsToNavigationReportParser_Longitude10000thMinsIs( int value )
    {
        Then( parser => Assert.AreEqual( value, parser.Longitude10000thMins ) );
    }

    [Then( @"NmeaAisAidsToNavigationReportParser\.Latitude10000thMins is (.*)" )]
    public void ThenNmeaAisAidsToNavigationReportParser_Latitude10000thMinsIs( int value )
    {
        Then( parser => Assert.AreEqual( value, parser.Latitude10000thMins ) );
    }

    [Then( @"NmeaAisAidsToNavigationReportParser\.ReferenceForPositionA is (.*)" )]
    public void ThenNmeaAisAidsToNavigationReportParser_ReferenceForPositionAIs( uint value )
    {
        Then( parser => Assert.AreEqual( value, parser.ReferenceForPositionA ) );
    }

    [Then( @"NmeaAisAidsToNavigationReportParser\.ReferenceForPositionB is (.*)" )]
    public void ThenNmeaAisAidsToNavigationReportParser_ReferenceForPositionBIs( uint value )
    {
        Then( parser => Assert.AreEqual( value, parser.ReferenceForPositionB ) );
    }

    [Then( @"NmeaAisAidsToNavigationReportParser\.ReferenceForPositionC is (.*)" )]
    public void ThenNmeaAisAidsToNavigationReportParser_ReferenceForPositionCIs( uint value )
    {
        Then( parser => Assert.AreEqual( value, parser.ReferenceForPositionC ) );
    }

    [Then( @"NmeaAisAidsToNavigationReportParser\.ReferenceForPositionD is (.*)" )]
    public void ThenNmeaAisAidsToNavigationReportParser_ReferenceForPositionDIs( uint value )
    {
        Then( parser => Assert.AreEqual( value, parser.ReferenceForPositionD ) );
    }

    [Then( @"NmeaAisAidsToNavigationReportParser\.EpfdFixType is (.*)" )]
    public void ThenNmeaAisAidsToNavigationReportParser_EpfdFixTypeIs( EpfdFixType value )
    {
        Then( parser => Assert.AreEqual( value, parser.EpfdFixType ) );
    }

    [Then( @"NmeaAisAidsToNavigationReportParser\.TimeStampSecond is (.*)" )]
    public void ThenNmeaAisAidsToNavigationReportParser_TimeStampSecondIs( uint value )
    {
        Then( parser => Assert.AreEqual( value, parser.TimeStampSecond ) );
    }

    [Then( @"NmeaAisAidsToNavigationReportParser\.OffPositionIndicator is (.*)" )]
    public void ThenNmeaAisAidsToNavigationReportParser_OffPositionIndicatorIs( bool value )
    {
        Then( parser => Assert.AreEqual( value, parser.OffPositionIndicator ) );
    }

    [Then( @"NmeaAisAidsToNavigationReportParser\.AtoNStatus is (.*)" )]
    public void ThenNmeaAisAidsToNavigationReportParser_AtoNStatusIs( uint value )
    {
        Then( parser => Assert.AreEqual( value, parser.AtoNStatus ) );
    }

    [Then( @"NmeaAisAidsToNavigationReportParser\.RaimFlag is (.*)" )]
    public void ThenNmeaAisAidsToNavigationReportParser_RaimFlagIs( bool value )
    {
        Then( parser => Assert.AreEqual( value, parser.RaimFlag ) );
    }

    [Then( @"NmeaAisAidsToNavigationReportParser\.VirtualAtoN is (.*)" )]
    public void ThenNmeaAisAidsToNavigationReportParser_VirtualAtoNIs( bool value )
    {
        Then( parser => Assert.AreEqual( value, parser.VirtualAtoN ) );
    }

    [Then( @"NmeaAisAidsToNavigationReportParser\.AssignedMode is (.*)" )]
    public void ThenNmeaAisAidsToNavigationReportParser_AssignedModeIs( bool value )
    {
        Then( parser => Assert.AreEqual( value, parser.AssignedMode ) );
    }

    [Then( @"NmeaAisAidsToNavigationReportParser\.SpareBit241 is (.*)" )]
    public void ThenNmeaAisAidsToNavigationReportParser_SpareBit241Is( bool value )
    {
        Then( parser => Assert.AreEqual( value, parser.SpareBit241 ) );
    }

    [Then( @"NmeaAisAidsToNavigationReportParser\.NameOfAidToNavigationExtension is (.*)" )]
    public void ThenNmeaAisAidsToNavigationReportParser_NameOfAidToNavigationExtensionIs( string value )
    {
        Then( parser =>
        {
            byte[] bytes = new byte[parser.NameOfAidToNavigationExtension.CharacterCount];
            parser.NameOfAidToNavigationExtension.WriteAsAscii( bytes );
            Assert.AreEqual( value, Encoding.ASCII.GetString( bytes ) );
        } );
    }

    [Then( @"NmeaAisAidsToNavigationReportParser\.SpareBitsAtEnd is (.*)" )]
    public void ThenNmeaAisAidsToNavigationReportParser_SpareBitsAtEndIs( uint value )
    {
        Then( parser => Assert.AreEqual( value, parser.SpareBitsAtEnd ) );
    }

    void When( ParserMaker makeParser )
    {
        _makeParser = makeParser;
    }

    void Then( ParserTest test )
    {
        if( _makeParser is null ) throw new InvalidOperationException( $"When step must be called." );
        NmeaAisAidsToNavigationReportParser parser = _makeParser();
        test( parser );
    }
}
