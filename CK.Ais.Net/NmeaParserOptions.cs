// <copyright file="NmeaParserOptions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Ais.Net;

/// <summary>
/// Enables control over configurable aspects of NMEA parsing.
/// </summary>
public class NmeaParserOptions
{
    /// <summary>
    /// Gets or sets a value indicating which standard is used by the <seealso cref="NmeaTagBlockParser"/>
    /// to parse the tag block.
    /// </summary>
    public TagBlockStandard TagBlockStandard { get; set; } = TagBlockStandard.Unspecified;

    /// <summary>
    /// Gets or sets a value indicating whether the presence of non-standard or unsupported
    /// fields in an NMEA tag block should be treated as a non-recoverable error.
    /// </summary>
    public bool ThrowWhenTagBlockContainsUnknownFields { get; set; } = true;

    /// <summary>
    /// Gets or sets a value determining how long the <see cref="NmeaLineToAisStreamAdapter"/>
    /// will wait for further related fragments after receiving a message fragment.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Some AIS messages are typically split across multiple NMEA message. E.g., type 5
    /// messages are on the large side and are normally split. Message-oriented methods such as
    /// <see cref="NmeaStreamParser.ParseFileAsync(string, INmeaAisMessageStreamProcessor, NmeaParserOptions)"/>
    /// deal with this for you, using the <see cref="NmeaLineToAisStreamAdapter"/> to combine
    /// fragments, enabling processors to see the whole reassembled payload. But what should
    /// happen if we receive an incomplete set of fragments? E.g., we might receive the first
    /// fragment but the 2nd is lost due to transmission errors.
    /// </para>
    /// <para>
    /// We must not hang onto message fragments indefinitely because the grouping identifiers
    /// used to reunite them are recycled. When relying on the fragment number at the NMEA
    /// sentence layer, it appears that the group is identified with a single non-zero decimal
    /// digit, meaning that there can be at most 9 fragmented messages in progress at any time.
    /// Many sources provide grouping information at the tag block level, where it is common
    /// to use more digits. (4-digit group ids seem common.) But since applications may be
    /// processing millions of messages per hour, even the tag-level group identifiers get
    /// reused pretty frequently.
    /// </para>
    /// <para>
    /// It is therefore necessary to limited how long we wait for all of the fragments of
    /// a message to arrive. Otherwise, any time a fragment goes missing, we would
    /// misidentify a later fragment: when the group identifier is reused, we would
    /// incorrectly attempt to pair this new fragment with the older one. At best, this
    /// causes us to report an error—if we get two "part 1" fragments for the same group,
    /// we report this as an error. But if we are unlucky, we could lose the 2nd fragment
    /// of a message and then later lose the 1st fragment of an unrelated message that
    /// happens to have recycled the group id, and we will erroneously combine the
    /// payloads of two unrelated messages and report them as a correctly-received
    /// single message.
    /// </para>
    /// <para>
    /// To avoid this, we limit the lifetime of unmatched message fragments. We measure
    /// this lifetime in received messages, not elapsed time, because recycling of group
    /// ids is driven by the number of messages that arrive.
    /// </para>
    /// <para>
    /// In practice, we very rarely see message fragments interleaved with other messages
    /// at all. Most of the time, the 2nd fragment appears immediately after the 1st if
    /// it appears at all. So the default age of 8 is more than enough for the data
    /// sources we've seen to date, and is small enough to work with single-digit group ids.
    /// </para>
    /// </remarks>
    public int MaximumUnmatchedFragmentAge { get; set; } = 8;

    /// <summary>
    /// Define the tolerance of the parser to the ill-formed groups.
    /// <para>
    /// This option was added to prevent CLS ill-formed data 2024/06/18.
    /// </para>
    /// </summary>
    public EmptyGroupTolerance EmptyGroupTolerance { get; set; } = EmptyGroupTolerance.None;

    /// <summary>
    /// When enabled, this option allows non-zero padding in fragments that are not the last one in a multi-fragment AIS message.
    /// <para>
    /// If set to false and non-zero padding is encountered in a non-last fragment, the decoder will throw an error.
    /// </para>
    /// </summary>
    public bool AllowNonZeroPaddingInNonLastFragment { get; set; } = false;

    /// <summary>
    /// If <c>true</c> and the talker id does not follow the specs (see <see href="https://gpsd.gitlab.io/gpsd/AIVDM.html#_talker_ids"/>),
    /// <see cref="NmeaLineParser{TExtraFieldParser}.AisTalker"/> will be set to <see cref="TalkerId.Unreconized"/>.
    /// Otherwise, <c>false</c> an error will be thrown.
    /// </summary>
    public bool AllowUnreconizedTalkerId { get; set; } = false;

    /// <summary>
    /// If <c>true</c> and the data origin is not VDM or VDO,
    /// <see cref="NmeaLineParser{TExtraFieldParser}.DataOrigin"/> will be set to <see cref="VesselDataOrigin.Unreconized"/>.
    /// Otherwise, <c>false</c> an error will be thrown.
    /// </summary>
    public bool AllowUnreconizedDataOrigin { get; set; } = false;

    /// <summary>
    /// Determines whether empty fields within an AIS tag block are permitted. If <c>true</c>, empty fields will be allowed; otherwise, they will be considered invalid.
    /// </summary>
    public bool AllowTagBlockEmptyFields { get; set; } = false;
}
