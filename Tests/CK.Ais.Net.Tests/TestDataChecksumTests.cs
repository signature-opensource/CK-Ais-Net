using NUnit.Framework;
using Shouldly;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using static CK.Testing.MonitorTestHelper;

namespace Ais.Net.Tests;

// Guard on the test data itself: every NMEA checksum written in the test sources must match its content.
// The default ChecksumOption (ValidateStandardFormat) only checks the checksum format, so wrong checksums went
// unnoticed (about 70 of them were fixed in the test data). A line whose checksum is wrong on purpose must
// end with the "// Intentionally invalid checksum" comment.
// Format strings (with '{n}' placeholders) are ignored: their checksum depends on the formatted values.
[TestFixture]
public class TestDataChecksumTests
{
    const string IntentionalMarker = "// Intentionally invalid checksum";

    // A tag block between '\' delimiters: \s:42,c:1567684904*38\
    static readonly Regex _delimitedTagBlock = new( @"\\([A-Za-z0-9][^\\""*{}\r\n]*)\*([0-9A-Fa-f]{2})(?=\\)" );

    // A sentence: !AIVDM,1,1,,A,...,0*78
    static readonly Regex _sentence = new( @"[!$]([A-Z]{5},[^*""{}\r\n]*)\*([0-9A-Fa-f]{2})" );

    // A tag block alone in a string literal (as given to NmeaTagBlockParser): "s:42,c:1567684904*38"
    static readonly Regex _quotedTagBlock = new( @"""([^""\\*{}!$\r\n]+)\*([0-9A-Fa-f]{2})""" );

    [Test]
    public void NMEA_checksums_in_the_test_sources_match_their_content()
    {
        var errors = new List<string>();
        int checkedCount = 0;
        foreach( var file in Directory.EnumerateFiles( TestHelper.TestProjectFolder, "*.cs", SearchOption.AllDirectories )
                                      .Where( f => !f.Contains( $"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}" )
                                                   && !f.Contains( $"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}" )
                                                   && Path.GetFileName( f ) != $"{nameof( TestDataChecksumTests )}.cs" ) )
        {
            var lines = File.ReadAllLines( file );
            for( int i = 0; i < lines.Length; ++i )
            {
                var line = lines[i];
                if( line.Contains( IntentionalMarker ) || line.TrimStart().StartsWith( "//" ) ) continue;
                foreach( var regex in new[] { _delimitedTagBlock, _sentence, _quotedTagBlock } )
                {
                    foreach( Match m in regex.Matches( line ) )
                    {
                        ++checkedCount;
                        var expected = ComputeChecksum( m.Groups[1].Value );
                        if( expected != m.Groups[2].Value.ToUpperInvariant() )
                        {
                            errors.Add( $"{Path.GetFileName( file )}:{i + 1}: '{m.Value}' should end with *{expected}." );
                        }
                    }
                }
            }
        }
        // Sanity check: the scan really found the test data.
        checkedCount.ShouldBeGreaterThan( 300 );
        errors.ShouldBeEmpty();
    }

    // The NMEA checksum is the XOR of all the characters between the start delimiter and the '*'.
    static string ComputeChecksum( string content )
    {
        int checksum = 0;
        foreach( char c in content ) checksum ^= c;
        return checksum.ToString( "X2" );
    }
}
