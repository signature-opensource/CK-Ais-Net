// <copyright file="AivdmExamples.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>
//
// Contains data under the Norwegian licence for Open Government data (NLOD) distributed by
// the Norwegian Costal Administration - https://ais.kystverket.no/
// The license can be found at https://data.norge.no/nlod/en/2.0
// The lines in this file that contain data from this source are annotated with a comment containing "ais.kystverket.no"
// The NLOD applies only to the data in these annotated lines. The license under which you are using this software
// (either the AGPLv3, or a commercial license) applies to the whole file.

namespace Ais.Net.Tests;

/// <summary>
/// Sample NMEA lines and format strings used to build NMEA lines in the tests.
/// </summary>
static class AivdmExamples
{
    /// <summary>
    /// A single-sentence AIVDM line carrying a type 18 message (Class B position report), without tag block.
    /// </summary>
    public const string ClassBPositionReportLine = "!AIVDM,1,1,,A,B3m:H900AP@b:79ae6:<OwnUoP06,0*78"; // ais.kystverket.no

    /// <summary>
    /// A tag block content (source and timestamp) without its '\' delimiters.
    /// </summary>
    public const string SimpleTagBlockWithoutDelimiters = "s:42,c:1567684904*38";

    /// <summary>
    /// <see cref="ClassBPositionReportLine"/> preceded by the <see cref="SimpleTagBlockWithoutDelimiters"/> tag block.
    /// </summary>
    public const string ClassBPositionReportLineWithTagBlock = @"\" + SimpleTagBlockWithoutDelimiters + @"\" + ClassBPositionReportLine;

    /// <summary>
    /// Format string: {0} is the talker and sentence type (e.g. "AIVDM").
    /// </summary>
    public const string MessageWithTalkerTagPlaceholderFormat = @"\s:42,c:1567684904*38\!{0},1,1,,A,B3m:H900AP@b:79ae6:<OwnUoP06,2*0A";   // ais.kystverket.no

    /// <summary>
    /// Format string: {0} total fragments, {1} fragment number, {2} message id, {3} group id in the tag block.
    /// </summary>
    public const string MessageFragmentFormat = @"\g:{0}-{1}-{3},s:42,c:1567684904*4E\!AIVDM,{1},{0},{2},A,53mSc8400000h<pH0008E8qBm=@DTp580000000N1P614t0Ht7P000000000,0*01";  // ais.kystverket.no

    /// <summary>
    /// Same as <see cref="MessageFragmentFormat"/> but without the group in the tag block.
    /// </summary>
    public const string MessageFragmentWithoutGroupInHeaderFormat = @"\s:42,c:1567684904*38\!AIVDM,{1},{0},{2},A,53mSc8400000h<pH0008E8qBm=@DTp580000000N1P614t0Ht7P000000000,0*01";  // ais.kystverket.no

    /// <summary>
    /// A single-sentence message with a tag block.
    /// </summary>
    public const string NonFragmentedMessage = @"\s:42,c:1567684904*38\!AIVDM,1,1,,A,B3m:H900AP@b:79ae6:<OwnUoP06,0*78";    // ais.kystverket.no

    /// <summary>
    /// Format string: {0} is the radio channel.
    /// </summary>
    public const string MessageWithRadioChannelPlaceholderFormat = "!AIVDM,1,1,,{0},B3m:H900AP@b:79ae6:<OwnUoP06,2*0A";     // ais.kystverket.no

    /// <summary>
    /// Format string: {0} is the payload.
    /// </summary>
    public const string MessageWithPayloadPlaceholderFormat = "!AIVDM,1,1,,A,{0},2*0A";

    /// <summary>
    /// Format string: {0} is the padding.
    /// </summary>
    public const string MessageWithPaddingPlaceholderFormat = "!AIVDM,1,1,,A,B3m:H900AP@b:79ae6:<OwnUoP06,{0}*0A";          // ais.kystverket.no
}
