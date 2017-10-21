using System;
using System.IO;
using JetBrains.Annotations;

namespace libsup
{
    /// <inheritdoc />
    /// <summary>
    /// This segment defines the graphics object. These are images with rendered text on a transparent background.
    /// </summary>
    [PublicAPI]
    public sealed class ObjectDefinitionSegment : SegmentDetail
    {
        /// <summary>
        /// Internal variable for property <see cref="ObjectData"/>.
        /// </summary>
        private readonly byte[] _objectData;

        /// <inheritdoc />
        public override int MinimumLength => 11;

        /// <inheritdoc />
        public override SegmentType SegmentType => SegmentType.ODS;

        /// <summary>
        /// ID of this object.
        /// </summary>
        public ushort ObjectId { get; }

        /// <summary>
        /// Version of this object.
        /// </summary>
        public byte ObjectVersionNumber { get; }

        /// <summary>
        /// If the image is split into a series of consecutive fragments, the fragments have the according flag set.
        /// </summary>
        public LastInSequenceFlag LastInSequenceFlag { get; }

        /// <summary>
        /// The length of the Run-length Encoding (RLE) data buffer with the compressed image data.
        /// (This is represented as a 3-byte value in the SUP file!)
        /// </summary>
        public uint ObjectDataLength { get; }

        /// <summary>
        /// Width of the image.
        /// </summary>
        public ushort Width { get; }

        /// <summary>
        /// Width of the image.
        /// </summary>
        public ushort Height { get; }

        /// <summary>
        /// This is the image data compressed using Run-length Encoding (RLE). The size of the data is defined in the
        /// Object Data Length field.
        /// </summary>
        public byte[] ObjectData => (byte[]) _objectData.Clone();

        /// <inheritdoc />
        public override object Clone()
        {
            return new ObjectDefinitionSegment(this);
        }

        /// <summary>
        /// Create a clone of an existing <see cref="ObjectDefinitionSegment"/>.
        /// </summary>
        /// <param name="old">The original <see cref="ObjectDefinitionSegment"/>.</param>
        private ObjectDefinitionSegment(ObjectDefinitionSegment old)
        {
            ObjectId = old.ObjectId;
            ObjectVersionNumber = old.ObjectVersionNumber;
            LastInSequenceFlag = old.LastInSequenceFlag;
            ObjectDataLength = old.ObjectDataLength;
            Width = old.Width;
            Height = old.Height;
            _objectData = old.ObjectData;
        }

        /// <summary>
        /// Parse a byte array into a new <see cref="ObjectDefinitionSegment"/>.
        /// </summary>
        /// <param name="bytes">The byte array to parse.</param>
        /// <exception cref="InvalidDataException">If the given byte array was not long enough to hold information about
        /// an <see cref="ObjectDefinitionSegment"/> or about its object data according to the parsed
        /// <see cref="ObjectDataLength"/>.</exception>
        internal ObjectDefinitionSegment(byte[] bytes)
        {
            // Check for minimum length of the byte array.
            if (bytes.Length < MinimumLength)
            {
                throw new InvalidDataException(
                    "The given detailed information about this segment could not be parsed because there was not enough data to read!");
            }

            // Read values into temporary byte arrays.
            var objid = bytes.Slice(0, 2);
            var objversion = bytes.Slice(2, 1);
            var lastinseq = bytes.Slice(3, 1);
            var datalen = bytes.Slice(4, 3);
            var width = bytes.Slice(7, 2);
            var height = bytes.Slice(9, 2);

            // Reverse the big-endian encoded arrays if our system is not a big-endian system.
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(objid);
                Array.Reverse(width);
                Array.Reverse(height);
            }

            // Map the byte arrays into their corresponding property.
            ObjectId = BitConverter.ToUInt16(objid, 0);
            ObjectVersionNumber = objversion[0];
            LastInSequenceFlag = (LastInSequenceFlag) lastinseq[0];
            ObjectDataLength = (uint) datalen[0] << 16 | (uint) datalen[1] << 8 | datalen[2];
            Width = BitConverter.ToUInt16(width, 0);
            Height = BitConverter.ToUInt16(height, 0);

            // Check for right length of the rest of the data according to the parsed ObjectDataLength.
            if (ObjectDataLength != bytes.Length - 7)
            {
                throw new InvalidDataException(
                    "The parsed object data length does not equal the number of given bytes for the object data!");
            }

            // Store the object data (RLE-encoded bitmap data).
            _objectData = bytes.Slice(11, (int) ObjectDataLength - 4);
        }
    }
}
