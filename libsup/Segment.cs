namespace libsup
{
    /// <summary>
    /// Represents a single segment in the SUP file.
    /// </summary>
    public sealed class Segment
    {
        /// <summary>
        /// Magic number - Every segment needs to start with this value.
        /// </summary>
        public const string MAGIC_NUMBER = "PG";

        /// <summary>
        /// Presentation Timestamp - The time when this segment is presented in the video.
        /// </summary>
        public uint Pts { get; private set; }

        /// <summary>
        /// Decoding Timestamp - The time when this segment should be decoded (usually 0 in practice).
        /// </summary>
        public uint Dts { get; private set; }

        /// <summary>
        /// The type of this segment.
        /// </summary>
        public SegmentType SegmentType { get; private set; }

        /// <summary>
        /// The size of this segment's data.
        /// </summary>
        public ushort SegmentSize { get; private set; }

        /// <summary>
        /// Detailed information for this specific segment depending on its type.
        /// </summary>
        public SegmentDetail SegmentDetail { get; private set; }

        /// <summary>
        /// Converts the Presentation Timestamp to its milliseconds value.
        /// </summary>
        /// <returns>The point of time in the video in milliseconds when this segment should be shown.</returns>
        public uint GetPtsInMilliseconds()
        {
            return Pts / 90;
        }

        /// <summary>
        /// Converts the Decoding Timestamp to its milliseconds value.
        /// </summary>
        /// <returns>The point of time in the video in milliseconds when this segment should be decoded.</returns>
        public uint GetDtsInMilliseconds()
        {
            return Dts / 90;
        }
    }
}
