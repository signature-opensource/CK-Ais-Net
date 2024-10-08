namespace Ais.Net;

/// <summary>
/// Define with which standard the <see cref="NmeaTagBlockParser{TExtraFieldParser}"/> will parse the tag block.
/// </summary>
public enum TagBlockStandard
{
    /// <summary>
    /// The tag block standard is not specified. The <see cref="NmeaTagBlockParser{TExtraFieldParser}"/> shoud defined itself which standard is used.
    /// </summary>
    Unspecified,

    /// <summary>
    /// Standard based on IEC 62320-1.
    /// </summary>
    IEC,

    /// <summary>
    /// Standard based on NMEA 4.10.
    /// </summary>
    Nmea,
}
