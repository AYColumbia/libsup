using JetBrains.Annotations;
using System;
using System.IO;

namespace libsup
{
    /// <summary>
    /// A window
    /// </summary>
    [PublicAPI]
    public sealed class WindowDefinition
    {
        /// <summary>
        /// Gets the window identifier.
        /// </summary>
        /// <value>
        /// The window identifier.
        /// </value>
        public byte WindowId { get; }
        /// <summary>
        /// Gets the window horizontal position.
        /// </summary>
        /// <value>
        /// The window horizontal position.
        /// </value>
        public ushort WindowHorizontalPosition { get; }
        /// <summary>
        /// Gets the window vertical position.
        /// </summary>
        /// <value>
        /// The window vertical position.
        /// </value>
        public ushort WindowVerticalPosition { get; }
        /// <summary>
        /// Gets the width of the window.
        /// </summary>
        /// <value>
        /// The width of the window.
        /// </value>
        public ushort WindowWidth { get; }
        /// <summary>
        /// Gets the height of the window.
        /// </summary>
        /// <value>
        /// The height of the window.
        /// </value>
        public ushort WindowHeight { get; }
        /// <summary>
        /// Initializes a new instance of the <see cref="WindowDefinition"/> class.
        /// </summary>
        internal WindowDefinition()
        {
            // do nothing
        }
        internal WindowDefinition(byte[] bytes)
        {
            // Check for invalid amount of bytes in the given byte collection.
            if (bytes.Length != 9)
            {
                throw new InvalidDataException(
                    "The given byte array has an invalid amount of data and cannot be parsed as a window item!");
            }

            // Map the byte values in the given collection to their corresponding properties.
            WindowId = bytes[0];

            var horizpos = bytes.Slice(1, 2);
            var vertipos = bytes.Slice(3, 2);
            var width = bytes.Slice(5, 2);
            var height = bytes.Slice(7, 2);

            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(horizpos);
                Array.Reverse(vertipos);
                Array.Reverse(width);
                Array.Reverse(height);
            }
            WindowHorizontalPosition = BitConverter.ToUInt16(horizpos, 0);
            WindowVerticalPosition = BitConverter.ToUInt16(vertipos, 0);
            WindowWidth = BitConverter.ToUInt16(width, 0);
            WindowHeight = BitConverter.ToUInt16(height, 0);
        }
    }
}
