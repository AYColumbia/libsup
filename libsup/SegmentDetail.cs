using System;
using JetBrains.Annotations;

namespace libsup
{
    /// <summary>
    /// Generyl type for detailed segment information depending on the specific segment type. 
    /// </summary>
    [PublicAPI]
    public abstract class SegmentDetail : ICloneable
    {
        /// <summary>
        /// Describes the minimum length in bytes for this type of segment.
        /// </summary>
        public abstract int MinimumLength { get; }

        /// <summary>
        /// The specific segment type represented by this detailed segment information.
        /// </summary>
        public abstract SegmentType SegmentType { get; }

        /// <summary>
        /// Creates a deep copy of the current <see cref="SegmentDetail"/> object.
        /// </summary>
        /// <returns>The deep copy of the current <see cref="SegmentDetail"/>.</returns>
        public abstract object Clone();
    }
}
