using System.Collections.Immutable;
using System.IO;
using JetBrains.Annotations;

namespace libsup
{
    /// <inheritdoc />
    /// <summary>
    /// This segment is used to define a palette for color conversion (YCbCr + Alpha).
    /// </summary>
    [PublicAPI]
    public sealed class PaletteDefinitionSegment : SegmentDetail
    {
        /// <inheritdoc />
        public override int MinimumLength => 7;

        /// <inheritdoc />
        public override SegmentType SegmentType => SegmentType.PDS;

        /// <summary>
        /// ID of the palette.
        /// </summary>
        public byte PaletteId { get; }

        /// <summary>
        /// Version of this palette within the Epoch.
        /// </summary>
        public byte PaletteVersionNumber { get; }

        /// <summary>
        /// The list of all palette entries in the current palette.
        /// </summary>
        public ImmutableList<PaletteEntry> PaletteEntries { get; }

        /// <inheritdoc />
        public override object Clone()
        {
            return new PaletteDefinitionSegment(this);
        }

        /// <summary>
        /// Create a clone of an existing <see cref="PaletteDefinitionSegment"/>.
        /// </summary>
        /// <param name="old">The original <see cref="PaletteDefinitionSegment"/>.</param>
        private PaletteDefinitionSegment(PaletteDefinitionSegment old)
        {
            PaletteId = old.PaletteId;
            PaletteVersionNumber = old.PaletteVersionNumber;
            PaletteEntries = ImmutableList<PaletteEntry>.Empty.AddRange(old.PaletteEntries);
        }

        /// <summary>
        /// Parse a byte array into a new <see cref="PaletteDefinitionSegment"/>.
        /// </summary>
        /// <param name="bytes">The byte array to parse.</param>
        /// <exception cref="InvalidDataException">If the byte array was not long enough to hold information about an
        /// <see cref="PaletteDefinitionSegment"/>.</exception>
        internal PaletteDefinitionSegment(byte[] bytes)
        {
            // Check for minimum length of the byte array.
            if (bytes.Length < MinimumLength)
            {
                throw new InvalidDataException(
                    "The given detailed information about this segment could not be parsed because there was not enough data to read!");
            }

            // Map the bytes into their corresponding property.
            PaletteId = bytes[0];
            PaletteVersionNumber = bytes[1];

            // Read the rest of the bytes as palette entries.
            PaletteEntries = ImmutableList<PaletteEntry>.Empty;
            for (var i = 2; i < bytes.Length; i += 5)
            {
                PaletteEntries = PaletteEntries.Add(new PaletteEntry(bytes.Slice(i, 5)));
            }
        }
    }
}
