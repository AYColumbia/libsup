using System;

namespace libsup
{
    /// <summary>
    /// Information about objects if the image is split into a series of consecutive fragments.
    /// </summary>
    [Flags]
    public enum LastInSequenceFlag : byte
    {
        /// <summary>
        /// Unset flag.
        /// </summary>
        None = 0x00,
        
        /// <summary>
        /// Last in sequence.
        /// </summary>
        Last = 0x40,
        
        /// <summary>
        /// First in sequence.
        /// </summary>
        First = 0x80
    }
}
