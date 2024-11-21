// <copyright file="StationType.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Ais.Net;

/// <summary>
/// Identify the types of stations.
/// </summary>
public enum StationType
{
    /// <summary>
    /// All types of mobiles (default).
    /// </summary>
    AllTypesOfMobiles = 0,

    /// <summary>
    /// Class A mobile stations only.
    /// </summary>
    ClassAMobileStationsOnly = 1,

    /// <summary>
    /// All types of Class B mobile stations.
    /// </summary>
    AllTypesOfClassBMobileStation = 2,

    /// <summary>
    /// SAR airborne mobile station.
    /// </summary>
    SARAirborneMobileStation = 3,

    /// <summary>
    /// Class B "SO" mobile stations only.
    /// </summary>
    ClassBSOMobileStationsOnly = 4,

    /// <summary>
    /// Class B "CS" shipborne mobile station only.
    /// </summary>
    ClassBSOShipborneMobileStationOnly = 5,

    /// <summary>
    /// Inland waterways.
    /// </summary>
    InlandWaterways = 6,

    /// <summary>
    /// Rgional use.
    /// </summary>
    RegionalUse7 = 7,

    /// <summary>
    /// Rgional use.
    /// </summary>
    RegionalUse8 = 8,

    /// <summary>
    /// Rgional use.
    /// </summary>
    Regionaluse9 = 9,

    /// <summary>
    /// Base station coverage area.
    /// </summary>
    BaseStationCoverageArea = 10,
}
