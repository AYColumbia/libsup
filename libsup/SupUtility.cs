using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using JetBrains.Annotations;

namespace libsup
{
    /// <summary>
    /// Provides various utility methods around SUP files.
    /// </summary>
    [PublicAPI]
    public static class SupUtility
    {
        /// <summary>
        /// Reads all <see cref="DataSet"/> objects from a SUP file and returns a list of them.
        /// </summary>
        /// <param name="filename">The path to the SUP file.</param>
        /// <returns>A list with all <see cref="DataSet"/> objects found in the SUP file.</returns>
        public static IEnumerable<DataSet> GetAllDataSets(string filename)
        {
            // Open a stream from the given file and read all data sets from it.
            using (var fileStream = File.OpenRead(filename))
            {
                foreach (var set in GetAllDataSets(fileStream))
                    yield return set;
            }
        }

        /// <summary>
        /// Reads all <see cref="DataSet"/> objects from a SUP filestram and returns a list of them.
        /// </summary>
        /// <param name="file">The SUP filestream.</param>
        /// <returns>A list with all <see cref="DataSet"/> objects found in the SUP filestream.</returns>
        public static IEnumerable<DataSet> GetAllDataSets(Stream file)
        {
            // Read from stream while not at the end and return each DataSet.
            while (file.Position < file.Length)
                yield return DataSet.FromStream(file);
        }

        /// <summary>
        /// Reads all subtitle compositions from a SUP file and returns a list of them.
        /// </summary>
        /// <param name="filename">The path to the SUP file.</param>
        /// <returns>A list with all subtitle images found in the SUP file.</returns>
        public static IEnumerable<Image> GetAllImages(string filename)
        {
            // Open a stream from the given file and read all images from it.
            using (var fileStream = File.OpenRead(filename))
            {
                foreach (var image in GetAllImages(fileStream))
                    yield return image;
            }
        }

        /// <summary>
        /// Reads all subtitle compositions from a SUP filestream and returns a list of them.
        /// </summary>
        /// <param name="file">The SUP filestream.</param>
        /// <returns>A list with all subtitle images found in the SUP filestream.</returns>
        public static IEnumerable<Image> GetAllImages(Stream file)
        {
            // Create local variables for the current composition, the currently used palettes
            // and the currently known objects.
            var currentComposition = new PresentationCompositionSegment();
            var currentPalettes = new Dictionary<byte, PaletteDefinitionSegment>();
            var currentObjects = new Dictionary<ushort, List<ObjectDefinitionSegment>>();

            // Go through each data set (each set represents an unique image).
            foreach (var set in GetAllDataSets(file))
            {
                // If any of the segments in the current data set is a PresentationCompositionSegment which's
                // state is EpochStart or AcquisitionPoint, we need to reset the palettes, objects and the current
                // composition (so we need to build a new image from completely new information).
                var needsReset = set.Segments.Any(segment =>
                {
                    if (segment.SegmentType != SegmentType.PCS)
                        return false;

                    var detail = segment.SegmentDetail as PresentationCompositionSegment;
                    Debug.Assert(detail != null, nameof(detail) + " != null");
                    return detail.CompositionState == CompositionState.EpochStart ||
                           detail.CompositionState == CompositionState.AcquisitionPoint;
                });
                if (needsReset)
                {
                    currentPalettes = new Dictionary<byte, PaletteDefinitionSegment>();
                    currentObjects = new Dictionary<ushort, List<ObjectDefinitionSegment>>();
                    currentComposition = new PresentationCompositionSegment();
                }

                // After we reset the current state (if needed), we go through each segment, to change/build the
                // information we need for the next image.
                foreach (var segment in set.Segments)
                {
                    switch (segment.SegmentType)
                    {
                        case SegmentType.WDS:
                            // Unneeded, so we just skip it.
                            break;
                        case SegmentType.PDS:
                            // Clone the palette definition and add it to our current palettes. If we already have
                            // a palette with the same PaletteId, we check if the currently encountered one is newer
                            // than the one we have on record (compare versions) and replace it if that's the case.
                            var palette = segment.SegmentDetail.Clone() as PaletteDefinitionSegment;
                            Debug.Assert(palette != null, nameof(palette) + " != null");
                            if (currentPalettes.ContainsKey(palette.PaletteId))
                            {
                                if (palette.PaletteVersionNumber >
                                    currentPalettes[palette.PaletteId].PaletteVersionNumber)
                                    currentPalettes[palette.PaletteId] = palette;
                            }
                            else
                            {
                                currentPalettes.Add(palette.PaletteId, palette);
                            }
                            break;
                        case SegmentType.ODS:
                            // Clone the object definition and check if we already have an object with the same id.
                            // If we do, we check if they have the same version. Again, if that's the case, we assume
                            // the currently encountered object has to be directly related to the object(s) with that id
                            // we have on record and thus we add it to the end of those object list. If the currently
                            // encountered object has a higher version number, we assume we have to replace our object
                            // list with the newer version completely. If we don't have objects with the encountered id
                            // on record, we just add it to a newly created object list.
                            var obj = segment.SegmentDetail.Clone() as ObjectDefinitionSegment;
                            Debug.Assert(obj != null, nameof(obj) + " != null");
                            if (currentObjects.ContainsKey(obj.ObjectId))
                            {
                                if (currentObjects[obj.ObjectId].First().ObjectVersionNumber == obj.ObjectVersionNumber)
                                {
                                    currentObjects[obj.ObjectId].Add(obj);
                                }
                                else if (currentObjects[obj.ObjectId].First().ObjectVersionNumber <
                                         obj.ObjectVersionNumber)
                                {
                                    currentObjects[obj.ObjectId].Clear();
                                    currentObjects[obj.ObjectId].Add(obj);
                                }
                            }
                            else
                            {
                                currentObjects[obj.ObjectId] = new List<ObjectDefinitionSegment> {obj};
                            }
                            break;
                        case SegmentType.PCS:
                            // Clone the presentation composition segment and check if it is an EpochStart or an
                            // AcquisitionPoint. If it is, it overrides our current composition completely. If it isn't,
                            // we need to handle it as an update only. If its flag says it only wants to update the
                            // palette, we do that. If it doesn't, we also replace all composition objects with those
                            // included in the currently encountered presentation composition segment.
                            var pcs = segment.SegmentDetail.Clone() as PresentationCompositionSegment;
                            Debug.Assert(pcs != null, nameof(pcs) + " != null");
                            if (pcs.CompositionState == CompositionState.EpochStart ||
                                pcs.CompositionState == CompositionState.AcquisitionPoint)
                            {
                                currentComposition = pcs;
                            }
                            else
                            {
                                currentComposition.PaletteId = pcs.PaletteId;

                                if (pcs.PaletteUpdateFlag == 0x80) break;

                                currentComposition.CompositionObjectNumber = pcs.CompositionObjectNumber;
                                currentComposition.CompositionObjects =
                                    ImmutableList<CompositionObject>.Empty.AddRange(pcs.CompositionObjects);
                            }
                            break;
                        case SegmentType.END:
                            // An end segment simply ends the set; nothing to do.
                            break;
                        default:
                            // If we encounter that error, something went HORRIBLY wrong!
                            throw new ArgumentOutOfRangeException();
                    }
                }

                // If we don't have any composition objects in our current composition,
                // we just skip to the next Data Set.
                if (currentComposition.CompositionObjectNumber <= 0) continue;

                // Go through every composition object in the current composition.
                foreach (var composition in currentComposition.CompositionObjects)
                {
                    // TODO Cropped compositions are currently not handled differently from normal compositions.

                    // Get the object data (multiple ObjectDefinitionSegments) for the composition object.
                    var objects = currentObjects[composition.ObjectId];

                    // Extract the colors of the palette used by the current composition.
                    // This extraction is important because it will be about ten times faster than direct access later!
                    var palette = currentPalettes[currentComposition.PaletteId].PaletteEntries
                        .ToDictionary(entry => entry.PaletteEntryId, entry => entry.GetColor());

                    // Create a new image with the right width and height and get Graphics object to draw in the image.
                    var image = new Bitmap(objects.First().Width, objects.First().Height, PixelFormat.Format32bppArgb);
                    var g = Graphics.FromImage(image);

                    // Aggregate the data from all object definition segments into one byte array for decoding.
                    var data = objects
                        .Aggregate(ImmutableList<byte>.Empty, (list, obj) => list.AddRange(obj.ObjectData)).ToArray();

                    // Set the current row and column (pixel coordinates in the final image output) to top-left corner.
                    var row = 0;
                    var col = 0;

                    // Go through every byte in the complete data array and decode it (RLE-encoded).
                    for (var i = 0; i < data.Length; i++)
                    {
                        // If the first byte in a tuple is not zero, we write its according color value directly to
                        // the image output, wander to the next column in the output and continue to decode.
                        if (data[i] != 0)
                        {
                            image.SetPixel(col, row,
                                palette.TryGetValue(data[i], out var color) ? color : Color.Transparent);
                            col += 1;
                            continue;
                        }

                        // If the first byte was zero, we go to the next byte and extract its first and second bit.
                        i += 1;
                        var flagA = (data[i] & (1 << 7)) != 0;
                        var flagB = (data[i] & (1 << 6)) != 0;

                        // If the whole second byte was also zero, we go one row down in the output and restart at the
                        // first column again.
                        if (data[i] == 0)
                        {
                            col = 0;
                            row += 1;
                        }
                        // If the second byte is not zero, but has the first and second bit set to zero, we extract the
                        // pixel count to write to the output from the last 6 bits (3 to 63 pixels possible) and draw
                        // them as a straight line in the color with the id 0.
                        else if (!flagA && !flagB)
                        {
                            var count = data[i] & 0x3F;
                            g.DrawLine(new Pen(palette.TryGetValue(0, out var color) ? color : Color.Transparent), col,
                                row, col + count - 1, row);
                            col += count;
                        }
                        // If the second byte is not zero, but has the first bit set to zero, the second to one, we
                        // extract the pixel count from the last 6 bits and the next byte (64 to 16383) and draw them
                        // as a straigt line in the color with the id 0.
                        else if (!flagA && flagB)
                        {
                            var countLow = data[i] & 0x3F;
                            i += 1;
                            var countHigh = data[i];
                            var count = (ushort) countLow << 8 | countHigh;
                            g.DrawLine(new Pen(palette.TryGetValue(0, out var color) ? color : Color.Transparent), col,
                                row, col + count - 1, row);
                            col += count;
                        }
                        // If the second byte is not zero, but has the first bit set to one, the second to zero, we
                        // extract the pixel count from the last 6 bits (3 to 63 pixels possible) and draw them as a
                        // straight line in the color with the id of the byte value of the byte thereafter.
                        else if (flagA && !flagB)
                        {
                            var count = data[i] & 0x3F;
                            i += 1;
                            g.DrawLine(new Pen(palette.TryGetValue(data[i], out var color) ? color : Color.Transparent),
                                col, row, col + count - 1, row);
                            col += count;
                        }
                        // If the second byte is not zero and has the first and second bit set to one, we extract the
                        // pixel count from the last 6 bits and the next byte (64 to 16383) and draw them as a straight
                        // line in the color with the id of the byte value of the byte thereafter.
                        else if (flagA && flagB)
                        {
                            var countLow = data[i] & 0x3F;
                            i += 1;
                            var countHigh = data[i];
                            i += 1;
                            var count = (ushort) countLow << 8 | countHigh;
                            g.DrawLine(new Pen(palette.TryGetValue(data[i], out var color) ? color : Color.Transparent),
                                col, row, col + count - 1, row);
                            col += count;
                        }
                    }

                    // Since we are done and have decoded the data and put the pixels in the output image, we can return
                    // the current one before we continue with the next composition.
                    yield return image;
                }
            }
        }
    }
}
