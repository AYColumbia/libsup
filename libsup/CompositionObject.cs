namespace libsup
{
    /// <summary>
    /// Defines the position on the screen of an image that will be shown.
    /// </summary>
    public sealed class CompositionObject
    {
        /// <summary>
        /// ID of the ODS segment that defines the image to be shown.
        /// </summary>
        public ushort ObjectId { get; private set; }
        
        /// <summary>
        /// ID of the WDS segment to which the image is allocated in the PCS. Up to two images may be assigned to one window.
        /// </summary>
        public byte WindowId { get; private set; }
        
        /// <summary>
        /// Determines if the sub picture is cropped to show only a portion of it (0x00 = False; 0x40 = True).
        /// </summary>
        public byte ObjectCroppedFlag { get; private set; }
        
        /// <summary>
        /// X offset from the top left pixel of the image on the screen.
        /// </summary>
        public ushort ObjectHorizontalPosition { get; private set; }
        
        /// <summary>
        /// Y offset from the top left pixel of the image on the screen.
        /// </summary>
        public ushort ObjectVerticalPosition { get; private set; }
        
        /// <summary>
        /// X offset from the top left pixel of the cropped object in the screen. Only used when <see cref="ObjectCroppedFlag"/> = 0x40.
        /// </summary>
        public ushort ObjectCroppingHorizontalPosition { get; private set; }
        
        /// <summary>
        /// Y offset from the top left pixel of the cropped object in the screen. Only used when <see cref="ObjectCroppedFlag"/> = 0x40.
        /// </summary>
        public ushort ObjectCroppingVerticalPosition { get; private set; }
        
        /// <summary>
        /// Width of the cropped object in the screen. Only used when <see cref="ObjectCroppedFlag"/> = 0x40.
        /// </summary>
        public ushort ObjectCroppingWidth { get; private set; }
        
        /// <summary>
        /// Height of the cropped object in the screen. Only used when <see cref="ObjectCroppedFlag"/> = 0x40.
        /// </summary>
        public ushort ObjectCroppingHeight { get; private set; }
    }
}
