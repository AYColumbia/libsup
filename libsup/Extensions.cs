using System;

namespace libsup
{
    /// <summary>
    /// Provides various extension methods used in this library.
    /// </summary>
    internal static class Extensions
    {
        /// <summary>
        /// Creates a slice of the array.
        /// </summary>
        /// <param name="arr">The array from which a slice will be created.</param>
        /// <param name="indexFrom">The first element from arr that should be in the slice.</param>
        /// <param name="length">The number of elements that should be in the slice.</param>
        /// <returns>length elements as array starting with the element at index indexFrom in arr.</returns>
        /// <exception cref="ArgumentOutOfRangeException">indexFrom cannot be smaller than 0 and must be smaller than
        /// the length of arr. length cannot be smaller than 1 and cannot be higher than the number of elements in arr
        /// starting from index indexFrom.</exception>
        internal static T[] Slice<T>(this T[] arr, int indexFrom, int length)
        {
            // Check for valid start index.
            if (indexFrom >= arr.Length || indexFrom < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(indexFrom));
            }

            // Check for valid length.
            if (length < 1 || indexFrom + length > arr.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            // Create a slice of the source array.
            var result = new T[length];
            Array.Copy(arr, indexFrom, result, 0, length);

            // Return the array copy.
            return result;
        }
    }
}
