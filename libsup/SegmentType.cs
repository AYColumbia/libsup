namespace libsup
{
    /// <summary>
    /// Possible segment types for the PGS segments including their byte value.
    /// </summary>
    public enum SegmentType : byte
    {
        /// <summary>
        /// Palette Definition Segment
        /// </summary>
        PDS = 0x14,

        /// <summary>
        /// Object Definition Segment
        /// </summary>
        ODS = 0x15,

        /// <summary>
        /// Presentation Composition Segment
        /// </summary>
        PCS = 0x16,

        /// <summary>
        /// Window Definition Segment
        /// </summary>
        WDS = 0x17,

        /// <summary>
        /// End of Display Set Segment
        /// </summary>
        END = 0x80
    }
}
