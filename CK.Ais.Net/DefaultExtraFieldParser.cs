using System;

namespace Ais.Net;

public readonly struct DefaultExtraFieldParser : INmeaTagBlockExtraFieldParser
{
    public bool TryParseField( ReadOnlySpan<byte> originalSpan, ReadOnlySpan<byte> field, int fieldOffset ) => false;
}
