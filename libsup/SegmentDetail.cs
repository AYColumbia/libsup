namespace libsup
{
    /// <summary>
    /// Generyl type for detailed segment information depending on the specific segment type. 
    /// </summary>
    public abstract class SegmentDetail
    {
        /// <summary>
        /// The specific segment type represented by this detailed segment information.
        /// </summary>
        public abstract SegmentType SegmentType { get; }
    }
}
