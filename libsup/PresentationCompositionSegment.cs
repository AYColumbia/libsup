using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using JetBrains.Annotations;

namespace libsup
{
    /// <inheritdoc />
    /// <summary>
    /// The Presentation Composition Segment is used for composing a sub picture.
    /// </summary>
    [PublicAPI]
    public sealed class PresentationCompositionSegment : SegmentDetail
    {
        /// <inheritdoc />
        public override int MinimumLength => 11;

        /// <inheritdoc />
        public override SegmentType SegmentType => SegmentType.PCS;

        /// <summary>
        /// Video width in pixels.
        /// </summary>
        public ushort Width { get; }

        /// <summary>
        /// Video height in pixels.
        /// </summary>
        public ushort Height { get; }

        /// <summary>
        /// Frame Rate of the sub picture (always 0x10).
        /// </summary>
        public byte FrameRate => 0x10;

        /// <summary>
        /// Number of this specific composition. It is incremented by one every time a graphics update occurs.
        /// </summary>
        public ushort CompositionNumber { get; }

        /// <summary>
        /// The type of this composition.
        /// </summary>
        public CompositionState CompositionState { get; }

        /// <summary>
        /// Indicates if this PCS describes a Palette only Display Update (0x00 = False; 0x80 = True).
        /// </summary>
        public byte PaletteUpdateFlag { get; }

        /// <summary>
        /// ID of the palette to be used in the Palette only Display Update.
        /// </summary>
        public byte PaletteId { get; internal set; }

        /// <summary>
        /// Number of composition objects defined in this segment.
        /// </summary>
        public byte CompositionObjectNumber { get; internal set; }

        /// <summary>
        /// All composition objects in this <see cref="PresentationCompositionSegment"/> object. The number of elements
        /// in this list should be the same as <see cref="CompositionObjectNumber"/>. 
        /// </summary>
        public ImmutableList<CompositionObject> CompositionObjects { get; internal set; }

        /// <inheritdoc />
        public override object Clone()
        {
            return new PresentationCompositionSegment(this);
        }

        /// <summary>
        /// Create a clone of an existing <see cref="PresentationCompositionSegment"/>.
        /// </summary>
        /// <param name="old">The original <see cref="PresentationCompositionSegment"/>.</param>
        private PresentationCompositionSegment(PresentationCompositionSegment old)
        {
            Width = old.Width;
            Height = old.Height;
            CompositionNumber = old.CompositionNumber;
            CompositionState = old.CompositionState;
            PaletteUpdateFlag = old.PaletteUpdateFlag;
            PaletteId = old.PaletteId;
            CompositionObjectNumber = old.CompositionObjectNumber;
            CompositionObjects = ImmutableList<CompositionObject>.Empty.AddRange(old.CompositionObjects);
        }

        /// <summary>
        /// Creates a new empty PresentationCompositionSegment.
        /// </summary>
        internal PresentationCompositionSegment()
        {
            // Do nothing
        }

        /// <summary>
        /// Parse a byte array into a new <see cref="PresentationCompositionSegment"/>.
        /// </summary>
        /// <param name="bytes">The byte array to parse.</param>
        /// <exception cref="InvalidDataException">If the byte array was not long enough to hold information about an
        /// <see cref="PresentationCompositionSegment"/>.</exception>
        internal PresentationCompositionSegment(byte[] bytes)
        {
            // Check for minimum length of the byte array.
            if (bytes.Length < MinimumLength)
            {
                throw new InvalidDataException(
                    "The given detailed information about this segment could not be parsed because there was not enough data to read!");
            }

            // Read values into temporary byte arrays.
            var width = bytes.Slice(0, 2);
            var height = bytes.Slice(2, 2);
            var compnum = bytes.Slice(5, 2);
            var compstate = bytes.Slice(7, 1);
            var palupdate = bytes.Slice(8, 1);
            var palid = bytes.Slice(9, 1);
            var objnum = bytes.Slice(10, 1);

            // Reverse the big-endian encoded arrays if our system is not a big-endian system.
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(width);
                Array.Reverse(height);
                Array.Reverse(compnum);
            }

            // Map the byte arrays into their corresponding property.
            Width = BitConverter.ToUInt16(width, 0);
            Height = BitConverter.ToUInt16(height, 0);
            CompositionNumber = BitConverter.ToUInt16(compnum, 0);
            CompositionState = (CompositionState) compstate[0];
            PaletteUpdateFlag = palupdate[0];
            PaletteId = palid[0];
            CompositionObjectNumber = objnum[0];

            // Read the rest of the bytes as composition objects.
            CompositionObjects = ImmutableList<CompositionObject>.Empty;
            for (var i = 11; i < bytes.Length;)
            {
                // Create a slice of the rest of the bytes that still need to be read and create a new composition
                // from this slice.
                var objbytes = bytes.Slice(i, bytes.Length - i);
                CompositionObjects = CompositionObjects.Add(new CompositionObject(objbytes));

                // Depending on the parsed composition, we had 16 or only 8 bytes read.
                i += CompositionObjects.Last().ObjectCroppedFlag == 0x40 ? 16 : 8;
            }
        }
    }
}
