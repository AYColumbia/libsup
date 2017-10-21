namespace libsup
{
    /// <inheritdoc />
    /// <summary>
    /// Represents additional information for an END Segment (End of Display Set).
    /// </summary>
    public sealed class EndOfDisplaySetSegment : SegmentDetail
    {
        /// <inheritdoc />
        public override int MinimumLength => 0;

        /// <inheritdoc />
        public override SegmentType SegmentType => SegmentType.END;

        /// <summary>
        /// Creates a new EndOfDisplaySegment object.
        /// </summary>
        internal EndOfDisplaySetSegment()
        {
            // Do nothing
        }

        /// <inheritdoc />
        public override object Clone()
        {
            return new EndOfDisplaySetSegment();
        }
    }
}
