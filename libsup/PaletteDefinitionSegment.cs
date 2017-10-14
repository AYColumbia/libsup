using System.Collections.Immutable;

namespace libsup
{
    /// <summary>
    /// This segment is used to define a palette for color conversion (YCbCr + Alpha).
    /// </summary>
    public sealed class PaletteDefinitionSegment : SegmentDetail
    {
        public override SegmentType SegmentType => SegmentType.PDS;
        
        /// <summary>
        /// ID of the palette.
        /// </summary>
        public byte PaletteId { get; private set; }
        
        /// <summary>
        /// Version of this palette within the Epoch.
        /// </summary>
        public byte PaletteVersionNumber { get; private set; }
        
        /// <summary>
        /// The list of all palette entries in the current palette.
        /// </summary>
        public ImmutableList<PaletteEntry> PaletteEntries { get; private set; }
    }
}
