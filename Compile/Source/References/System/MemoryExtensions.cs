// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace EmptyNamespace
namespace System;
#if !!(NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) || NO_SYSTEM_MEMORY
/// <summary>
/// Extension methods for Span{T}, Memory{T}, and friends.
/// </summary>
public static partial class MemoryExtensions
{
    /// <summary>
    /// Determines whether two read-only sequences are equal by comparing
    /// the elements using <see cref="IEquatable{T}.Equals(T?)"/>.
    /// </summary>
    /// <param name="span">The first sequence to compare.</param>
    /// <param name="other">The second sequence to compare.</param>
    /// <typeparam name="T">The type of elements in the sequence.</typeparam>
    /// <returns><see langword="true"/> if the two sequences are equal; otherwise, <see langword="false"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool SequenceEqual<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> other)
        where T : IEquatable<T>
    {
        if (span.Length != other.Length)
            return false;

        var e1 = span.GetEnumerator();
        var e2 = other.GetEnumerator();

        while (e1.MoveNext())
            if (!(e2.MoveNext() && e1.Current.Equals(e2.Current)))
                return false;

        return !e2.MoveNext();
    }

    /// <summary>
    /// Determines whether a span and a read-only span are equal by comparing
    /// the elements using <see cref="IEquatable{T}.Equals(T?)"/>.
    /// </summary>
    /// <typeparam name="T">The type of elements in the sequence.</typeparam>
    /// <param name="span">The first sequence to compare.</param>
    /// <param name="other">The second sequence to compare.</param>
    /// <returns><see langword="true"/> if the two sequences are equal; otherwise, <see langword="false"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool SequenceEqual<T>(this Span<T> span, ReadOnlySpan<T> other)
        where T : IEquatable<T> =>
        ((ReadOnlySpan<T>)span).SequenceEqual(other);
}
#endif
