namespace libsup
{
    /// <summary>
    /// This segment defines the graphics object. These are images with rendered text on a transparent background.
    /// </summary>
    public sealed class ObjectDefinitionSegment : SegmentDetail
    {
        private byte[] _objectData;
        public override SegmentType SegmentType => SegmentType.ODS;
        
        /// <summary>
        /// ID of this object.
        /// </summary>
        public ushort ObjectId { get; private set; }
        
        /// <summary>
        /// Version of this object.
        /// </summary>
        public byte ObjectVersionNumber { get; private set; }
        
        /// <summary>
        /// If the image is split into a series of consecutive fragments, the fragments have the according flag set.
        /// </summary>
        public LastInSequenceFlag LastInSequenceFlag { get; private set; }
        
        /// <summary>
        /// The length of the Run-length Encoding (RLE) data buffer with the compressed image data. (This is represented as a 3-byte value in the SUP file!)
        /// </summary>
        public uint ObjectDataLength { get; private set; }
        
        /// <summary>
        /// Width of the image.
        /// </summary>
        public ushort Width { get; private set; }
        
        /// <summary>
        /// Width of the image.
        /// </summary>
        public ushort Height { get; private set; }

        /// <summary>
        /// This is the image data compressed using Run-length Encoding (RLE). The size of the data is defined in the Object Data Length field.
        /// </summary>
        public byte[] ObjectData => (byte[])_objectData.Clone();
    }
}
