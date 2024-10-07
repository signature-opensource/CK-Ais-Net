// <copyright file="EpfdFixType.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Ais.Net;

/// <summary>
/// EPFD fix types.
/// </summary>
public enum EpfdFixType
{
    Undefined = 0,
    Gps = 1,
    Glonass = 2,
    CombinedGpsGlonass = 3,
    LoranC = 4,
    Chayka = 5,
    IntegratedNavigationSystem = 6,
    Surveyed = 7,
    Galileo = 8,
    InternalGnss = 15,
}
