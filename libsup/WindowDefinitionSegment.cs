using JetBrains.Annotations;
using System.Collections.Immutable;
using System.IO;

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
        /// Number of windows defined in this segment.  Added support for multiple
        /// windows based on the PGS spec. available here:
        /// https://blog.thescorpius.com/index.php/2017/07/15/presentation-graphic-stream-sup-files-bluray-subtitle-format/
        /// </summary>
        public byte NumberOfWindows { get; internal set; } = 1;

        /// <summary>
        /// Gets the windows.
        /// </summary>
        /// <value>
        /// The windows.
        /// </value>
        public ImmutableList<WindowDefinition> Windows { get; }
        /// <inheritdoc />
        public override object Clone()
        {
            return new WindowDefinitionSegment(this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowDefinitionSegment"/> class.
        /// </summary>
        /// <param name="old">The old.</param>
        private WindowDefinitionSegment(WindowDefinitionSegment old)
        {
            NumberOfWindows = old.NumberOfWindows;
            Windows = ImmutableList<WindowDefinition>.Empty.AddRange(old.Windows);
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
            var numWindows = bytes.Slice(0, 1);
            Windows = ImmutableList<WindowDefinition>.Empty;
            for (var i = 1; i < bytes.Length; i += MinimumLength)
            {
                Windows = Windows.Add(new WindowDefinition(bytes.Slice(i, 9)));
            }
        }
    }
}
