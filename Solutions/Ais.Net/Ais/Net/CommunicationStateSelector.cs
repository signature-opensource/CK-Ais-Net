// <copyright file="CommunicationStateSelector.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Ais.Net
{
    /// <summary>
    /// Indicates which communication state follows is used.
    /// </summary>
    public enum CommunicationStateSelector
    {
#pragma warning disable CS1591, SA1602 // XML comments. The names are from the spec, and they're all the information we have
        Sotdma = 0,
        Itdma = 1,
    }
}
