using NUnit.Framework;
using System.Text;

namespace Ais.Net.Tests.AisMessageTypes;

// NmeaAisGroupAssignmentCommandParser decodes the payload of message type 23 (Group Assignment Command):
// a base station commands the mobile stations of a rectangular area (given by two corners, in 1/10 arc minutes),
// optionally filtered by station type and ship type, to use a Tx/Rx mode, a reporting interval and a quiet time.
// The message is 160 bits long: 27 payload characters with 2 bits of padding.
[TestFixture]
public class GroupAssignmentCommandParserTests
{
    // The first 6 bits carry the message type (23).
    [Test]
    public void MessageType_is_decoded()
    {
        var parser = new NmeaAisGroupAssignmentCommandParser( Encoding.ASCII.GetBytes( "G@475:1B`Tc02TeIEJ@00000800" ), 2 );
        parser.MessageType.ShouldBe( MessageType.GroupAssignmentCommand );
    }

    // Every field of real messages: Japanese base stations (MMSI 00431xxxx) received by satellite on 2026-01-01.
    // They command a 5 seconds reporting interval to all the mobiles of an area around 141°E 36°N
    // (e.g. 84617 / 600 = 141.03°E and 22016 / 600 = 36.69°N). The first two messages come from two
    // stations that share the same area.
    // The last two rows are synthetic, to give a non-zero value to the fields that the real messages leave at 0:
    // - limits: every unsigned field at its maximum, the "not available" longitude (181°) and latitude (91°),
    //   and their negative counterparts for the first corner;
    // - distinct values in every field (south-west hemisphere corners, InlandWaterways, ship type 70,
    //   NextShorterReportingInterval...) so that a wrong offset or width of any field is detected.
    [TestCase( "G@475:1B`Tc02TeIEJ@00000800", 2u, 1u, 4310312u, 0u, 84617, 22016, 84331, 21865, StationType.AllTypesOfMobiles, 0u, 0u, 0u, ReportingInterval.FiveSeconds, 0u, 0u )]
    [TestCase( "G@474hAB`Tc02TeIEJ@00000800", 2u, 1u, 4310209u, 0u, 84617, 22016, 84331, 21865, StationType.AllTypesOfMobiles, 0u, 0u, 0u, ReportingInterval.FiveSeconds, 0u, 0u )]
    [TestCase( "G@475Pi@>h`D:OEa>u000000800", 2u, 1u, 4310403u, 0u, 82156, 20641, 81581, 20212, StationType.AllTypesOfMobiles, 0u, 0u, 0u, ReportingInterval.FiveSeconds, 0u, 0u )]
    [TestCase( "Gvqc9wvEtREG3D73EB:wwwwwswt", 2u, 3u, 999999999u, 3u, -108600, -54600, 108600, 54600, StationType.BaseStationCoverageArea, 255u, 4194303u, 3u, ReportingInterval.TwoSeconds, 15u, 63u )]
    [TestCase( "GQmg=5FolSH@uiK6jG6AP001aL4", 2u, 2u, 123456789u, 1u, -73912, -20345, -73000, -19876, StationType.InlandWaterways, 70u, 1u, 2u, ReportingInterval.NextShorterReportingInterval, 7u, 1u )]
    public void Full_message_is_decoded( string payload,
                                         uint padding,
                                         uint repeatIndicator,
                                         uint mmsi,
                                         uint spare38,
                                         int longitude1,
                                         int latitude1,
                                         int longitude2,
                                         int latitude2,
                                         StationType stationType,
                                         uint shipAndCargoType,
                                         uint spare122,
                                         uint txRxMode,
                                         ReportingInterval reportInterval,
                                         uint quietTime,
                                         uint spare154 )
    {
        var parser = new NmeaAisGroupAssignmentCommandParser( Encoding.ASCII.GetBytes( payload ), padding );
        parser.MessageType.ShouldBe( MessageType.GroupAssignmentCommand );
        parser.RepeatIndicator.ShouldBe( repeatIndicator );
        parser.Mmsi.ShouldBe( mmsi );
        parser.SpareBits38.ShouldBe( spare38 );
        parser.Longitude10thMins1.ShouldBe( longitude1 );
        parser.Latitude10thMins1.ShouldBe( latitude1 );
        parser.Longitude10thMins2.ShouldBe( longitude2 );
        parser.Latitude10thMins2.ShouldBe( latitude2 );
        parser.StationType.ShouldBe( stationType );
        parser.TypeOfShipAndCargoType.ShouldBe( shipAndCargoType );
        parser.SpareBits122.ShouldBe( spare122 );
        parser.TxRxMode.ShouldBe( txRxMode );
        parser.ReportInterval.ShouldBe( reportInterval );
        parser.QuietTime.ShouldBe( quietTime );
        parser.SpareBits154.ShouldBe( spare154 );
    }
}
