// <copyright file="MessageType.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Ais.Net;

/// <summary>
/// Identify the AIS messages types.
/// </summary>
public enum MessageType
{
    /// <summary>
    /// Scheduled position report.
    /// Class A shipborne mobile equipment.
    /// </summary>
    PositionReport1 = 1,

    /// <summary>
    /// Assigned scheduled position report.
    /// Class A shipborne mobile equipment.
    /// </summary>
    PositionReport2 = 2,

    /// <summary>
    /// Special position report, response to interrogation.
    /// Class A shipborne mobile equipment.
    /// </summary>
    PositionReport3 = 3,

    /// <summary>
    /// Position, UTC, date and current slot number of base station.
    /// </summary>
    BaseStationReport = 4,

    /// <summary>
    /// /Scheduled static and voyage related vessel data report, Class A shipborne mobile
    /// equipment.
    /// </summary>
    StaticAndVoyageRelatedData = 5,

    /// <summary>
    /// Binary data for addressed communication.
    /// </summary>
    BinaryAddressedMessage = 6,

    /// <summary>
    /// Acknowledgement of received addressed binary data.
    /// </summary>
    BinaryAcknoledgement = 7,

    /// <summary>
    /// Binary data for broadcast communication.
    /// </summary>
    BinaryBroadcastMessage = 8,

    /// <summary>
    /// Position report for airborne stations involved in SAR operations only.
    /// </summary>
    StandardSARAircraftPositionReport = 9,

    /// <summary>
    /// Request UTC and date.
    /// </summary>
    UTCDateInquiry = 10,

    /// <summary>
    /// Current UTC and date if available.
    /// </summary>
    UTCDateResponse = 11,

    /// <summary>
    /// Safety related data for addressed communication.
    /// </summary>
    AddressedSafetyRelatedMessage = 12,

    /// <summary>
    /// Acknowledgement of received addressed safety related message.
    /// </summary>
    SafetyRelatedAcknoledgement = 13,

    /// <summary>
    /// Safety related data for broadcast communication.
    /// </summary>
    SafetyRelatedBroadcastMessage = 14,

    /// <summary>
    /// Request for a specific message type can result in multiple responses from one
    /// or several stations.
    /// </summary>
    Interrigation = 15,

    /// <summary>
    /// Assignment of a specific report behaviour by competent authority using a Base station.
    /// </summary>
    AssignmentModeCommand = 16,

    /// <summary>
    /// DGNSS corrections provided by a base station.
    /// </summary>
    DGNSSBroadcastBinaryMessage = 17,

    /// <summary>
    /// Standard position report for Class B shipborne mobile equipment to be used instead of
    /// Messages 1, 2, 3.
    /// </summary>
    StandardClassBEquipmentPositionReport = 18,

    /// <summary>
    /// No longer required. Extended position report for Class B shipborne mobile equipment;
    /// contains additional static information.
    /// </summary>
    ExtendedClassBEquipmentPositionReport = 19,

    /// <summary>
    /// Reserve slots for Base station(s).
    /// </summary>
    DataLinkManagementMessage = 20,

    /// <summary>
    /// Position and status report for aids-to-navigation.
    /// </summary>
    AidsToNavigationReport = 21,

    /// <summary>
    /// Management of channels and transceiver modes by a Base station.
    /// </summary>
    ChannelManagement = 22,

    /// <summary>
    /// Assignment of a specific report behaviour by competent authority using a Base station
    /// to a specific group of mobiles.
    /// </summary>
    GroupAssignmentCommand = 23,

    /// <summary>
    /// Additional data assigned to an MMSI Part A: Name Part B: Static Data.
    /// </summary>
    StaticDataReport = 24,

    /// <summary>
    /// Short unscheduled binary data transmission Broadcast or addressed.
    /// </summary>
    SingleSlotBinaryMessage = 25,

    /// <summary>
    /// Scheduled binary data transmission Broadcast or addressed.
    /// </summary>
    MultipleSlotBinaryMessageSithCommunicationState = 26,

    /// <summary>
    /// Class A and Class B "SO" shipborne mobile equipment outside base station coverage.
    /// </summary>
    PositionReportForLongRangeApplication = 27,
}
