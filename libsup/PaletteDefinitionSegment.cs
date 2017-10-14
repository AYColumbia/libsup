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
        /// Entry number of the palette.
        /// </summary>
        public byte PaletteEntryId { get; private set; }
        
        /// <summary>
        /// Luminance (Y value).
        /// </summary>
        public byte Luminance { get; private set; }
        
        /// <summary>
        /// Color Difference Red (Cr value).
        /// </summary>
        public byte ColorDifferenceRed { get; private set; }
        
        /// <summary>
        /// Color Difference Blue (Cb value).
        /// </summary>
        public byte ColorDifferenceBlue { get; private set; }
        
        /// <summary>
        /// Transparency (Alpha value).
        /// </summary>
        public byte Transparency { get; private set; }
    }
}
