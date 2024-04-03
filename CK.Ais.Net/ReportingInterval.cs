// <copyright file="ReportingInterval.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Ais.Net
{
    /// <summary>
    /// Identify the reporting interval settings for use with message 23.
    /// </summary>
    public enum ReportingInterval
    {
#pragma warning disable CS1591, SA1602 // XML comments. The names are from the spec, and they're all the information we have
        AsGivenByTheAutonomousMode = 0,
        TenMinutes = 1,
        SixMinutes = 2,
        ThreeMinutes = 3,
        OneMinute = 4,
        ThirtySeconds = 5,
        FifteenSeconds = 6,
        TenSeconds = 7,
        FiveSeconds = 8,

        /// <summary>
        /// Only applicable if in autonomous mode.
        /// </summary>
        NextShorterReportingInterval = 9,

        /// <summary>
        /// Only applicable if in autonomous mode.
        /// </summary>
        NextLongerReportingInterval = 10,

        /// <summary>
        /// Not applicable to the Class B "CS" and Class B "SO".
        /// </summary>
        TwoSeconds = 11,
    }
}
