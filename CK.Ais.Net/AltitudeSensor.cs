// <copyright file="AltitudeSensor.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Ais.Net;

/// <summary>
/// Indicates which type of altitude sensor is present in a Standard Search and Rescue Aircraft Position Report message.
/// </summary>
public enum AltitudeSensor
{
    Gnss = 0,
    BarometricSource = 1,
}
