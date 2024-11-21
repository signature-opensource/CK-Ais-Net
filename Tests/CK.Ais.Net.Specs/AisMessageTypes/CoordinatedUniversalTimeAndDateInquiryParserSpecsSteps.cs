using NUnit.Framework;
using System;
using System.Text;
using TechTalk.SpecFlow;

namespace Ais.Net.Specs.AisMessageTypes;

[Binding]
public class CoordinatedUniversalTimeAndDateInquiryParserSpecsSteps
{
    ParserMaker? _makeParser;

    delegate NmeaAisCoordinatedUniversalTimeAndDateInquiryParser ParserMaker();

    delegate void ParserTest( NmeaAisCoordinatedUniversalTimeAndDateInquiryParser parser );

    [When( "I parse '(.*)' with padding (.*) as a Coordinated Universal Time and Date Inquiry" )]
    public void WhenIParseWithPaddingAsALongRangeAisBroadcast( string payload, uint padding )
    {
        When( () => new NmeaAisCoordinatedUniversalTimeAndDateInquiryParser( Encoding.ASCII.GetBytes( payload ), padding ) );
    }

    [Then( @"NmeaAisCoordinatedUniversalTimeAndDateInquiryParser\.Type is (.*)" )]
    public void ThenNmeaAisCoordinatedUniversalTimeAndDateInquiryParser_TypeIs( MessageType messageType )
    {
        Then( parser => Assert.AreEqual( messageType, parser.MessageType ) );
    }

    [Then( @"NmeaAisCoordinatedUniversalTimeAndDateInquiryParser\.RepeatIndicator is (.*)" )]
    public void ThenNmeaAisCoordinatedUniversalTimeAndDateInquiryParser_RepeatIndicatorIs( int repeatCount )
    {
        Then( parser => Assert.AreEqual( repeatCount, parser.RepeatIndicator ) );
    }

    [Then( @"NmeaAisCoordinatedUniversalTimeAndDateInquiryParser\.Mmsi is (.*)" )]
    public void ThenNmeaAisCoordinatedUniversalTimeAndDateInquiryParser_MmsiIs( int mmsi )
    {
        Then( parser => Assert.AreEqual( mmsi, parser.Mmsi ) );
    }

    [Then( @"NmeaAisCoordinatedUniversalTimeAndDateInquiryParser\.SpareBits38 is (.*)" )]
    public void ThenNmeaAisCoordinatedUniversalTimeAndDateInquiryParser_SpareBits38Is( int bits38 )
    {
        Then( parser => Assert.AreEqual( bits38, parser.SpareBits38 ) );
    }

    [Then( @"NmeaAisCoordinatedUniversalTimeAndDateInquiryParser\.DestinationMmsi is (.*)" )]
    public void ThenNmeaAisCoordinatedUniversalTimeAndDateInquiryParser_DestinationMmsiIs( int destinationMmsi )
    {
        Then( parser => Assert.AreEqual( destinationMmsi, parser.DestinationMmsi ) );
    }

    [Then( @"NmeaAisCoordinatedUniversalTimeAndDateInquiryParser\.SpareBits70 is (.*)" )]
    public void ThenNmeaAisCoordinatedUniversalTimeAndDateInquiryParser_SpareBits70Is( int bits70 )
    {
        Then( parser => Assert.AreEqual( bits70, parser.SpareBits70 ) );
    }

    void When( ParserMaker makeParser )
    {
        _makeParser = makeParser;
    }

    void Then( ParserTest test )
    {
        if( _makeParser is null ) throw new InvalidOperationException( $"When step must be called." );
        NmeaAisCoordinatedUniversalTimeAndDateInquiryParser parser = _makeParser();
        test( parser );
    }
}
