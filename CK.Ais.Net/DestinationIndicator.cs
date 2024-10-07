// <copyright file="DestinationIndicator.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Ais.Net;

/// <summary>
/// Indicates which type of destination indicator is used in a Single Slot Binary
/// or Multiple Slot Binary Message With Communications State message.
/// </summary>
public enum DestinationIndicator
{
    /// <summary>
    /// No destination MMSI used.
    /// </summary>
    Brocast = 0,

    /// <summary>
    /// Destination MMSI is used.
    /// </summary>
    Addressed = 1,
}
