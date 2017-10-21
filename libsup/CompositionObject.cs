using System;
using JetBrains.Annotations;

namespace libsup
{
    /// <summary>
    /// Defines the position on the screen of an image that will be shown.
    /// </summary>
    [PublicAPI]
    public sealed class CompositionObject
    {
        /// <summary>
        /// ID of the ODS segment that defines the image to be shown.
        /// </summary>
        public ushort ObjectId { get; }

        /// <summary>
        /// ID of the WDS segment to which the image is allocated in the PCS.
        /// Up to two images may be assigned to one window.
        /// </summary>
        public byte WindowId { get; }

        /// <summary>
        /// Determines if the sub picture is cropped to show only a portion of it (0x00 = False; 0x40 = True).
        /// </summary>
        public byte ObjectCroppedFlag { get; }

        /// <summary>
        /// X offset from the top left pixel of the image on the screen.
        /// </summary>
        public ushort ObjectHorizontalPosition { get; }

        /// <summary>
        /// Y offset from the top left pixel of the image on the screen.
        /// </summary>
        public ushort ObjectVerticalPosition { get; }

        /// <summary>
        /// X offset from the top left pixel of the cropped object in the screen.
        /// Only used when <see cref="ObjectCroppedFlag"/> = 0x40.
        /// </summary>
        public ushort ObjectCroppingHorizontalPosition { get; }

        /// <summary>
        /// Y offset from the top left pixel of the cropped object in the screen.
        /// Only used when <see cref="ObjectCroppedFlag"/> = 0x40.
        /// </summary>
        public ushort ObjectCroppingVerticalPosition { get; }

        /// <summary>
        /// Width of the cropped object in the screen. Only used when <see cref="ObjectCroppedFlag"/> = 0x40.
        /// </summary>
        public ushort ObjectCroppingWidth { get; }

        /// <summary>
        /// Height of the cropped object in the screen. Only used when <see cref="ObjectCroppedFlag"/> = 0x40.
        /// </summary>
        public ushort ObjectCroppingHeight { get; }

        /// <summary>
        /// Parses a byte array into its <see cref="CompositionObject"/> representation.
        /// </summary>
        /// <param name="bytes">The byte array that should be parsed.</param>
        internal CompositionObject(byte[] bytes)
        {
            // Read values into temporary byte arrays.
            var objid = bytes.Slice(0, 2);
            var winid = bytes.Slice(2, 1);
            var cropped = bytes.Slice(3, 1);
            var horizpos = bytes.Slice(4, 2);
            var vertipos = bytes.Slice(6, 2);

            // Reverse the big-endian encoded arrays if our system is not a big-endian system.
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(objid);
                Array.Reverse(horizpos);
                Array.Reverse(vertipos);
            }

            // Map the byte arrays into their corresponding property.
            ObjectId = BitConverter.ToUInt16(objid, 0);
            WindowId = winid[0];
            ObjectCroppedFlag = cropped[0];
            ObjectHorizontalPosition = BitConverter.ToUInt16(horizpos, 0);
            ObjectVerticalPosition = BitConverter.ToUInt16(vertipos, 0);

            // Stop parsing if not cropped or no more information about cropping.
            if (ObjectCroppedFlag != 0x40 || bytes.Length < 16) return;

            // Read values into temporary byte arrays.
            var crophorizpos = bytes.Slice(8, 2);
            var cropvertipos = bytes.Slice(10, 2);
            var cropwidth = bytes.Slice(12, 2);
            var cropheight = bytes.Slice(14, 2);

            // Reverse the big-endian encoded arrays if our system is not a big-endian system.
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(crophorizpos);
                Array.Reverse(cropvertipos);
                Array.Reverse(cropwidth);
                Array.Reverse(cropheight);
            }

            // Map the byte arrays into their corresponding property.
            ObjectCroppingHorizontalPosition = BitConverter.ToUInt16(crophorizpos, 0);
            ObjectCroppingVerticalPosition = BitConverter.ToUInt16(cropvertipos, 0);
            ObjectCroppingWidth = BitConverter.ToUInt16(cropwidth, 0);
            ObjectCroppingHeight = BitConverter.ToUInt16(cropheight, 0);
        }
    }
}
