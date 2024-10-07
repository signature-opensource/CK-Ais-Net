namespace Ais.Net;

/// <summary>
/// Defines the tolerance level for groups of NMEA frames containing fragments with an empty sentence.
/// See <see cref="NmeaParserOptions.EmptyGroupTolerance"/>.
/// <para>
/// Example:
/// <code>
/// \g:1-3-1481,s:AIS,c:1718701847*1A\!AIVDM,1,1,,A,13Ef?=3006wt9WlEMhi1S6uJ00Rq,0*40
/// \g:2-3-1481*50\
/// \g:3-3-1481*51\
/// </code>
/// </para>
/// </summary>
public enum EmptyGroupTolerance
{
    /// <summary>
    /// Throw when a group fragment contain an empty sentence.
    /// </summary>
    None,
    /// <summary>
    /// Allow group fragments with empty sentence.
    /// </summary>
    Allow,
    /// <summary>
    /// If only one group fragment contain a sentence, then the group is transformed into a simple message.
    /// </summary>
    AutoFix
}
