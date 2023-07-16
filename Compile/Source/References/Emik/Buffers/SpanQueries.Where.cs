// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace EmptyNamespace
namespace Emik.Morsels;

/// <summary>Efficient LINQ-like methods for <see cref="ReadOnlySpan{T}"/> and siblings.</summary>
// ReSharper disable NullableWarningSuppressionIsUsed
#pragma warning disable MA0048
static partial class SpanQueries
#pragma warning restore MA0048
{
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
    /// <inheritdoc cref="Enumerable.Where{T}(IEnumerable{T}, Func{T, bool})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Memory<T> Where<T>(
        this IMemoryOwner<T> source,
        [InstantHandle, RequireStaticDelegate] Predicate<T> predicate
    ) =>
        source.Memory[..^Filter(source.Memory.Span, predicate)];

    /// <inheritdoc cref="Enumerable.Where{T}(IEnumerable{T}, Func{T, bool})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Memory<T> Where<T>(
        this Memory<T> source,
        [InstantHandle, RequireStaticDelegate] Predicate<T> predicate
    ) =>
        source[..^Filter(source.Span, predicate)];
#endif

    /// <inheritdoc cref="Enumerable.Where{T}(IEnumerable{T}, Func{T, bool})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> Where<T>(
        this Span<T> source,
        [InstantHandle, RequireStaticDelegate] Predicate<T> predicate
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            source[..^Filter(source, predicate)];

    // Surprisingly, direct indexing is more efficient than .CopyTo despite latter guaranteeing SIMD.
    // Benchmarked with various sizes and on function "static x => BitOperations.PopCount(x) % 2 is 0".
    // This function was chosen as the baseline due to it being cheap to compute, evenly distributed,
    // and ensuring all patterns of [[false, false], [false, true], [true, false], [true, true]] would appear.
    // ReSharper disable CommentTypo
    // https://sharplab.io/#v2:D4AQTAjAsAULIQGwAICWA7ALsg6gCwFMAnAgSXXWIB4AVAPgApZkXkBlABwEN1a7kAzgHsArkQDGBADTNWABRIATVOK6YCfZByUq1BWAEpYAb1ksAZkKLIGANy7XUyALzIADAG40yKoNESCADoAGQJ0AHNMPC9UAGpYoxhWZFMk5NZUcxttAmVVdQZhMUkAbVQAXQNE9JrkcSEsDBECD1gzdPtrAUwHbFdUVrg0msUhdtq0LIZ4pzpXIoCQsMi8aonakAB2QR6iTEGJgHc8VAAbAhsAQhy8vUL/UoqqwfHWTuRlAQ5TrkkAWzCfW8AFodr0DrUFo9AoFyoEAMJCDgATxoQnuxQIJW6vRhlQhNShF3mDyxMIAep9vr8CACsOUCeknMDXFSfv9AYyAL5tYbILZ+TFLCJRQZcoA
    // https://cdn.discordapp.com/attachments/445375602648940544/1129045669148098610/image.png
    // ReSharper restore CommentTypo

    [NonNegativeValue, MethodImpl(MethodImplOptions.AggressiveInlining)]
    static int Filter<T>(Span<T> source, [InstantHandle, RequireStaticDelegate] Predicate<T> predicate)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        var end = 0;

        for (var i = 0; i < source.Length; i++)
        {
            if (predicate(source[i]))
            {
                if (end > 0)
                    source[i - end] = source[i];

                continue;
            }

            var start = i;

            do
                if (++i >= source.Length)
                    return end + i - start;
            while (!predicate(source[i]));

            end += i - start;
            source[i - end] = source[i];
        }

        return end;
    }
}
