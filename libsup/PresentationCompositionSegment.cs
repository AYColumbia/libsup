using System.Collections.Immutable;

namespace libsup
{
    /// <summary>
    /// The Presentation Composition Segment is used for composing a sub picture.
    /// </summary>
    public sealed class PresentationCompositionSegment : SegmentDetail
    {
        public override SegmentType SegmentType => SegmentType.PCS;
        
        /// <summary>
        /// Video width in pixels.
        /// </summary>
        public ushort Width { get; private set; }
        
        /// <summary>
        /// Video height in pixels.
        /// </summary>
        public ushort Height { get; private set; }

        /// <summary>
        /// Frame Rate of the sub picture (always 0x10).
        /// </summary>
        public byte FrameRate => 0x10;
        
        /// <summary>
        /// Number of this specific composition. It is incremented by one every time a graphics update occurs.
        /// </summary>
        public ushort CompositionNumber { get; private set; }
        
        /// <summary>
        /// The type of this composition.
        /// </summary>
        public CompositionState CompositionState { get; private set; }
        
        /// <summary>
        /// Indicates if this PCS describes a Palette only Display Update (0x00 = False; 0x80 = True).
        /// </summary>
        public byte PaletteUpdateFlag { get; private set; }
        
        /// <summary>
        /// ID of the palette to be used in the Palette only Display Update.
        /// </summary>
        public byte PaletteId { get; private set; }
        
        /// <summary>
        /// Number of composition objects defined in this segment.
        /// </summary>
        public byte CompositionObjectNumber { get; private set; }
        
        /// <summary>
        /// All composition objects in this <see cref="PresentationCompositionSegment"/> object. The number of elements in this list should be the same as <see cref="CompositionObjectNumber"/>. 
        /// </summary>
        public ImmutableList<CompositionObject> CompositionObjects { get; private set; }
    }
}
