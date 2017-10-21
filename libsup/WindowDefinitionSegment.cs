using System;
using System.IO;
using JetBrains.Annotations;

namespace libsup
{
    /// <inheritdoc />
    /// <summary>
    /// This segment is used to define the rectangular area on the screen where the sub picture will be shown.
    /// This rectangular area is called a Window.
    /// </summary>
    [PublicAPI]
    public sealed class WindowDefinitionSegment : SegmentDetail
    {
        /// <inheritdoc />
        public override int MinimumLength => 9;

        /// <inheritdoc />
        public override SegmentType SegmentType => SegmentType.WDS;

        /// <summary>
        /// ID of this window.
        /// </summary>
        public byte WindowId { get; }

        /// <summary>
        /// X offset from the top left pixel of the window in the screen.
        /// </summary>
        public ushort WindowHorizontalPosition { get; }

        /// <summary>
        /// Y offset from the top left pixel of the window in the screen.
        /// </summary>
        public ushort WindowVerticalPosition { get; }

        /// <summary>
        /// Width of the window.
        /// </summary>
        public ushort WindowWidth { get; }

        /// <summary>
        /// Height of the window.
        /// </summary>
        public ushort WindowHeight { get; }

        /// <inheritdoc />
        public override object Clone()
        {
            return new WindowDefinitionSegment(this);
        }

        /// <summary>
        /// Create a clone of an existing <see cref="WindowDefinitionSegment"/>.
        /// </summary>
        /// <param name="old">The original <see cref="WindowDefinitionSegment"/>.</param>
        private WindowDefinitionSegment(WindowDefinitionSegment old)
        {
            WindowId = old.WindowId;
            WindowHorizontalPosition = old.WindowHorizontalPosition;
            WindowVerticalPosition = old.WindowVerticalPosition;
            WindowWidth = old.WindowWidth;
            WindowHeight = old.WindowHeight;
        }

        /// <summary>
        /// Parse a byte array into a new <see cref="WindowDefinitionSegment"/>.
        /// </summary>
        /// <param name="bytes">The byte array to parse.</param>
        /// <exception cref="InvalidDataException">If the byte array was not long enough to hold information about an
        /// <see cref="WindowDefinitionSegment"/>.</exception>
        internal WindowDefinitionSegment(byte[] bytes)
        {
            // Check for minimum length of the byte array.
            if (bytes.Length < MinimumLength)
            {
                throw new InvalidDataException(
                    "The given detailed information about this segment could not be parsed because there was not enough data to read!");
            }

            // Read values into temporary byte arrays.
            var winid = bytes.Slice(0, 1);
            var horizpos = bytes.Slice(1, 2);
            var vertipos = bytes.Slice(3, 2);
            var width = bytes.Slice(5, 2);
            var height = bytes.Slice(7, 2);

            // Reverse the big-endian encoded arrays if our system is not a big-endian system.
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(horizpos);
                Array.Reverse(vertipos);
                Array.Reverse(width);
                Array.Reverse(height);
            }

            // Map the byte arrays into their corresponding property.
            WindowId = winid[0];
            WindowHorizontalPosition = BitConverter.ToUInt16(horizpos, 0);
            WindowVerticalPosition = BitConverter.ToUInt16(vertipos, 0);
            WindowWidth = BitConverter.ToUInt16(width, 0);
            WindowHeight = BitConverter.ToUInt16(height, 0);
        }
    }
}
