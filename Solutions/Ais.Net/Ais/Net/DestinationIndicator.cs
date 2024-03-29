// <copyright file="DestinationIndicator.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Ais.Net
{
    /// <summary>
    /// Indicates which type of destination indicator is used in a Single Slot Binary message.
    /// </summary>
    public enum DestinationIndicator
    {
        /// <summary>
        /// No <see cref="NmeaAisSingleSlotBinaryParser.DestinationMmsi"/> used.
        /// </summary>
        Brocast = 0,

        /// <summary>
        /// <see cref="NmeaAisSingleSlotBinaryParser.DestinationMmsi"/> is used.
        /// </summary>
        Addressed = 1,
    }
}
