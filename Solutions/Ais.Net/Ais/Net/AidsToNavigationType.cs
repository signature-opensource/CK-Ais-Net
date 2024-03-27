// <copyright file="AidsToNavigationType.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Ais.Net
{
    /// <summary>
    /// Indicates which type of aids to navigation is used.
    /// </summary>
    public enum AidsToNavigationType
    {
#pragma warning disable CS1591, SA1602 // XML comments. The names are from the spec, and they're all the information we have
        NotSpecified = 0,
        ReferencePoint = 1,
        Racon = 2,

        /// <summary>
        /// Fixed structures off-shore, such as oil platforms, wind farms. (NOTE 1 - This code should identify an obstruction that is fitted with an AtoN AIS station)
        /// </summary>
        FixedStructure = 3,
        EmergencyWreckMarkingBuoy = 4,
        LightWithoutSectors = 5,
        LightWithSectors = 6,
        LeadingLightFront = 7,
        LeadingLightRear = 8,
        BeaconCardinalN = 9,
        BeaconCardinalE = 10,
        BeaconCardinalS = 11,
        BeaconrdinalW = 12,
        BeaconPortHand = 13,
        BeaconStarboardHand = 14,
        BeaconPreferredChannelPortHand = 15,
        BeaconPreferredChannelStarboardHand = 16,
        BeaconIsolatedDanger = 17,
        BeaconSafeWater = 18,
        BeaconSpecialMark = 19,
        CardinalMarkN = 20,
        CardinalMarkE = 21,
        CardinalMarkS = 22,
        CardinalMarkW = 23,
        PortHandMark = 24,
        StarboardHandMark = 25,
        PreferredChannelPortHand = 26,
        PreferredChannelStarboardHand = 27,
        IsolatedDanger = 28,
        SafeWater = 29,
        SpecialMark = 30,

        /// <summary>
        /// Light Vessel/LANBY/Rigs.
        /// </summary>
        Light = 31,
    }
}
