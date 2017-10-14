namespace libsup
{
    /// <summary>
    /// Represents additional information for an END Segment (End of Display Set).
    /// </summary>
    public sealed class EndOfDisplaySetSegment : SegmentDetail
    {
        public override SegmentType SegmentType => SegmentType.END;
    }
}
