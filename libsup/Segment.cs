using System;
using System.IO;
using System.Text;
using JetBrains.Annotations;

namespace libsup
{
    /// <summary>
    /// Represents a single segment in the SUP file.
    /// </summary>
    [PublicAPI]
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

        /// <summary>
        /// Parse the SegmentDetail property according to the SegmentType of this segment.
        /// </summary>
        /// <param name="bytes">The additional bytes to parse.</param>
        /// <exception cref="InvalidDataException">Unknown segment type.</exception>
        private void ParseDetail(byte[] bytes)
        {
            switch (SegmentType)
            {
                case SegmentType.PCS:
                    SegmentDetail = new PresentationCompositionSegment(bytes);
                    break;
                case SegmentType.ODS:
                    SegmentDetail = new ObjectDefinitionSegment(bytes);
                    break;
                case SegmentType.PDS:
                    SegmentDetail = new PaletteDefinitionSegment(bytes);
                    break;
                case SegmentType.WDS:
                    SegmentDetail = new WindowDefinitionSegment(bytes);
                    break;
                case SegmentType.END:
                    SegmentDetail = new EndOfDisplaySetSegment();
                    break;
                default:
                    throw new InvalidDataException(
                        "The given data could not be parsed because the segment type is unknown!");
            }
        }

        /// <summary>
        /// Creates a new segment from the current position in a stream.
        /// </summary>
        /// <param name="stream">The stream from which the segment should be parsed and created.</param>
        /// <returns>The segment parsed from the given stream.</returns>
        /// <exception cref="InvalidDataException">The given data does not start with the magic value.</exception>
        internal static Segment Read(Stream stream)
        {
            // Create a new segment and read the magic number from the stream.
            var newseg = new Segment();
            var magicnum = new byte[2];
            stream.Read(magicnum, 0, magicnum.Length);

            // Check if magic number is correct.
            if (!Encoding.ASCII.GetString(magicnum).Equals(MAGIC_NUMBER))
                throw new InvalidDataException(
                    "The given data that should be parsed into a segment does not start with the magic value!");

            // Create temporary arrays and read their contents from the stream.
            var pts = new byte[4];
            var dts = new byte[4];
            var segmentType = new byte[1];
            var segmentSize = new byte[2];
            stream.Read(pts, 0, pts.Length);
            stream.Read(dts, 0, dts.Length);
            stream.Read(segmentType, 0, segmentType.Length);
            stream.Read(segmentSize, 0, segmentSize.Length);

            // Reverse the big-endian encoded arrays if our system is not a big-endian system.
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(pts);
                Array.Reverse(dts);
                Array.Reverse(segmentSize);
            }

            // Map the byte arrays into their corresponding property.
            newseg.Pts = BitConverter.ToUInt32(pts, 0);
            newseg.Dts = BitConverter.ToUInt32(dts, 0);
            newseg.SegmentType = (SegmentType) segmentType[0];
            newseg.SegmentSize = BitConverter.ToUInt16(segmentSize, 0);

            // Now read the needed bytes from the stream according to the parsed segment size.
            var detail = new byte[newseg.SegmentSize];
            stream.Read(detail, 0, detail.Length);

            // Parse the read bytes from the segment data into the SegmentDetail property.
            newseg.ParseDetail(detail);

            // Return the created segment.
            return newseg;
        }
    }
}
