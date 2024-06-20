// <copyright file="NmeaTagBlockParser.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Ais.Net
{
    using System;

    public readonly struct DefaultExtraFieldParser : INmeaTagBlockExtraFieldParser
    {
        public bool TryParseField( ReadOnlySpan<byte> originalSpan, ReadOnlySpan<byte> field, int fieldOffset ) => false;
    }
}
