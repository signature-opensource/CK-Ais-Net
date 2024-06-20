// <copyright file="INmeaLineStreamProcessor.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Ais.Net
{
    /// <summary>
    /// Receives the lines parsed from an NMEA file by <see cref="NmeaStreamParser"/>.
    /// </summary>
    /// <remarks>
    /// Processors implementing this interface will receive each separate line, which might contain
    /// only a fragment of an AIS message. If you want to process entire AIS messages, implement
    /// <see cref="INmeaAisMessageStreamProcessor"/> instead.
    /// </remarks>
    public interface INmeaLineStreamProcessor<TExtraFieldParser> : INmeaLineStreamProcessor
        where TExtraFieldParser : struct, INmeaTagBlockExtraFieldParser
    {
        /// <summary>
        /// Called for each non-empty line.
        /// </summary>
        /// <param name="parsedLine">The parsed line.</param>
        /// <param name="lineNumber">The 1-based line number.</param>
        void OnNext( in NmeaLineParser<TExtraFieldParser> parsedLine, int lineNumber );
    }
}
