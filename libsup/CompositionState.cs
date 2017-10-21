using JetBrains.Annotations;

namespace libsup
{
    /// <summary>
    /// Type of the composition of a <see cref="PresentationCompositionSegment"/>.
    /// </summary>
    [PublicAPI]
    public enum CompositionState : byte
    {
        /// <summary>
        /// Defines a display update, and contains only functional segments with elements that are different from the
        /// preceding composition.
        /// </summary>
        Normal = 0x00,

        /// <summary>
        /// Defines a display refresh. This is used to compose in the middle of the Epoch.
        /// </summary>
        AcquisitionPoint = 0x40,

        /// <summary>
        /// Defines a new display. The Epoch Start contains all functional segments needed to display a new composition
        /// on the screen.
        /// </summary>
        EpochStart = 0x80
    }
}
