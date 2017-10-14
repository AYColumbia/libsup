namespace libsup
{
    /// <summary>
    /// A single color in a palette.
    /// </summary>
    public sealed class PaletteEntry
    {
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
