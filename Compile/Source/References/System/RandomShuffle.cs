// SPDX-License-Identifier: MPL-2.0
#if !NET8_0_OR_GREATER && !NO_SYSTEM_MEMORY
// ReSharper disable once CheckNamespace
namespace System;
#pragma warning disable CS8500
/// <summary>The backport of Shuffle methods for <see cref="Random"/>.</summary>
static partial class RandomShuffle
{
    /// <param name="that">The instance of <see cref="Random"/>.</param>
    extension(Random that)
    {
        /// <summary>Performs an in-place shuffle of an array.</summary>
        /// <remarks><para>
        /// This method uses <see cref="Random.Next(int, int)" /> to choose values for shuffling.
        /// This method is an O(n) operation.
        /// </para></remarks>
        /// <typeparam name="T">The type of array.</typeparam>
        /// <param name="values">The array to shuffle.</param>
        // ReSharper disable once RedundantUnsafeContext
        public unsafe void Shuffle<T>(T[] values) => Shuffle(that, values.AsSpan());

        /// <summary>Performs an in-place shuffle of a span.</summary>
        /// <typeparam name="T">The type of span.</typeparam>
        /// <param name="values">The span to shuffle.</param>
        /// <remarks><para>
        /// This method uses <see cref="Random.Next(int, int)" /> to choose values for shuffling.
        /// This method is an O(n) operation.
        /// </para></remarks>
        public void Shuffle<T>(Span<T> values)
        {
            var n = values.Length;

            for (var i = 0; i < n - 1; i++)
            {
                var j = that.Next(i, n);

                if (j == i)
                    continue;

                // ReSharper disable once SwapViaDeconstruction
#pragma warning disable IDE0180
                var temp = values[i];
                values[i] = values[j];
                values[j] = temp;
#pragma warning restore IDE0180
            }
        }
    }
}
#endif
