using System.Collections.Immutable;
using System.IO;
using System.Linq;
using JetBrains.Annotations;

namespace libsup
{
    /// <summary>
    /// Describes a complete Data Set of a PGS stream with all segments (from the first one to the END segment).
    /// </summary>
    [PublicAPI]
    public sealed class DataSet
    {
        /// <summary>
        /// A list of all contained segments in this Data Set in the order they were read.
        /// </summary>
        public ImmutableList<Segment> Segments { get; }

        /// <summary>
        /// Creates a new Data Set object from a Stream, starting with reading from the current position of the given
        /// Stream.
        /// </summary>
        /// <param name="stream">The stream that should be parsed into a Data Set.</param>
        private DataSet(Stream stream)
        {
            // Create an empty segment list and append all found segments in the stream until an END segment.
            Segments = ImmutableList<Segment>.Empty;
            while (Segments.Count < 1 || Segments.Last().SegmentType != SegmentType.END)
            {
                Segments = Segments.Add(Segment.Read(stream));
            }
        }

        /// <summary>
        /// Parses a stream from its current position until an END segment is encountered, into a Data Set.
        /// </summary>
        /// <param name="stream">The stream which should be parsed into a Data Set.</param>
        /// <returns>The parsed Data Set including all its encountered segments.</returns>
        public static DataSet FromStream(Stream stream)
        {
            return new DataSet(stream);
        }

        /// <summary>
        /// Parses a byte array from its start to the first encountered END segment into a Data Set.
        /// </summary>
        /// <param name="bytes">The byte array which should be parsed into a Data Set.</param>
        /// <returns>The parsed Data Set including all its encountered segments.</returns>
        public static DataSet FromBytes(byte[] bytes)
        {
            using (var stream = new MemoryStream(bytes))
            {
                stream.Seek(0, SeekOrigin.Begin);
                return new DataSet(stream);
            }
        }
    }
}
