using System;

namespace Ais.Net
{
    /// <summary>
    /// Extracts data from non standard Tag Block fields of an NMEA message.
    /// </summary>
    public interface INmeaTagBlockExtraFieldParser
    {
        /// <summary>
        /// Try to parse a field with an unknown field type.
        /// </summary>
        /// <param name="originalSpan">The original span ending with the *XX (checksum).</param>
        /// <param name="field">The ASCII-encoded tag block field, with the type key.</param>
        /// <param name="fieldOffset">The offset of the <paramref name="field"/> in the <paramref name="originalSpan"/>.</param>
        /// <returns><see langword="true" /> if the field has been successfully parsed.</returns>
        bool TryParseField( ReadOnlySpan<byte> originalSpan, ReadOnlySpan<byte> field, int fieldOffset );
    }
}
