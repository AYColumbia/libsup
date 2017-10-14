namespace libsup
{
    /// <summary>
    /// This segment is used to define the rectangular area on the screen where the sub picture will be shown. This rectangular area is called a Window.
    /// </summary>
    public sealed class WindowDefinitionSegment : SegmentDetail
    {
        public override SegmentType SegmentType => SegmentType.WDS;
        
        /// <summary>
        /// ID of this window.
        /// </summary>
        public byte WindowId { get; private set; }
        
        /// <summary>
        /// X offset from the top left pixel of the window in the screen.
        /// </summary>
        public ushort WindowHorizontalPosition { get; private set; }
        
        /// <summary>
        /// Y offset from the top left pixel of the window in the screen.
        /// </summary>
        public ushort WindowVerticalPosition { get; private set; }
        
        /// <summary>
        /// Width of the window.
        /// </summary>
        public ushort WindowWidth { get; private set; }
        
        /// <summary>
        /// Height of the window.
        /// </summary>
        public ushort WindowHeight { get; private set; }
    }
}
