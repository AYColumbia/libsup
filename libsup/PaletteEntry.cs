using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using JetBrains.Annotations;

namespace libsup
{
    /// <summary>
    /// A single color in a palette.
    /// </summary>
    [PublicAPI]
    public sealed class PaletteEntry
    {
        /// <summary>
        /// Entry number of the palette.
        /// </summary>
        public byte PaletteEntryId { get; }

        /// <summary>
        /// Luminance (Y value).
        /// </summary>
        public byte Luminance { get; }

        /// <summary>
        /// Color Difference Red (Cr value).
        /// </summary>
        public byte ColorDifferenceRed { get; }

        /// <summary>
        /// Color Difference Blue (Cb value).
        /// </summary>
        public byte ColorDifferenceBlue { get; }

        /// <summary>
        /// Transparency (Alpha value).
        /// </summary>
        public byte Transparency { get; }

        /// <summary>
        /// Converts the color format of the palette entry into a <see cref="System.Drawing.Color"/> structure.
        /// </summary>
        /// <returns>The <see cref="System.Drawing.Color"/> structure representing the color of this palette
        /// entry.</returns>
        public Color GetColor()
        {
            // Convert HDTV YCbCr to RGB.
            var y = Luminance - 16;
            var cr = ColorDifferenceRed - 128;
            var cb = ColorDifferenceBlue - 128;
            var r = (byte) Math.Min(Math.Max(Math.Round(1.1644 * y + 1.596 * cr), 0), 255);
            var g = (byte) Math.Min(Math.Max(Math.Round(1.1644 * y - 0.813 * cr - 0.391 * cb), 0), 255);
            var b = (byte) Math.Min(Math.Max(Math.Round(1.1644 * y + 2.018 * cb), 0), 255);

            return Color.FromArgb(Transparency, r, g, b);
        }

        /// <summary>
        /// Create a new palette entry from an array of bytes.
        /// </summary>
        /// <param name="bytes">The byte collection that should be parsed.</param>
        /// <exception cref="InvalidDataException">The given byte array has an invalid amount of data.</exception>
        internal PaletteEntry(IReadOnlyList<byte> bytes)
        {
            // Check for invalid amount of bytes in the given byte collection.
            if (bytes.Count != 5)
            {
                throw new InvalidDataException(
                    "The given byte array has an invalid amount of data and cannot be parsed as a palette entry!");
            }

            // Map the byte values in the given collection to their corresponding properties.
            PaletteEntryId = bytes[0];
            Luminance = bytes[1];
            ColorDifferenceRed = bytes[2];
            ColorDifferenceBlue = bytes[3];
            Transparency = bytes[4];
        }
    }
}
