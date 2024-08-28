// SPDX-License-Identifier: MPL-2.0

// ReSharper disable NullableWarningSuppressionIsUsed RedundantUnsafeContext
// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

// ReSharper disable once RedundantNameQualifier
using static System.Runtime.CompilerServices.RuntimeHelpers;
using static Span;

/// <summary>Extension methods that act as factories for <see cref="SmallList{T}"/>.</summary>
static partial class SmallFactory
{
#if NETCOREAPP3_1_OR_GREATER
    /// <inheritdoc cref="System.MemoryExtensions.Contains"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool Contains<T>(this scoped PooledSmallList<T> span, T item)
        where T : IEquatable<T>? =>
        span.View.Contains(item);
#endif
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
    /// <summary>Removes the first occurence of a specific object from the <see cref="PooledSmallList{T}"/>.</summary>
    /// <typeparam name="T">The type of item.</typeparam>
    /// <param name="span">The <see cref="PooledSmallList{T}"/> to remove an element from.</param>
    /// <param name="item">The item to remove from the <see cref="PooledSmallList{T}"/>.</param>
    /// <returns>
    /// Whether or not it found the parameter <paramref name="item"/> within the bounds of the
    /// parameter <paramref name="span"/>, and substantially removed it from the collection.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Remove<T>(this scoped PooledSmallList<T> span, T item)
        where T : IEquatable<T>?
    {
        var i = span.IndexOf(item);

        if (i is -1)
            return false;

        span.RemoveAt(i);
        return true;
    }

    /// <inheritdoc cref="System.MemoryExtensions.IndexOf"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static int IndexOf<T>(this scoped PooledSmallList<T> span, T item)
        where T : IEquatable<T>? =>
        span.View.IndexOf(item);

    /// <summary>Allocates the buffer on the stack or heap, and gives it to the caller.</summary>
    /// <remarks><para>See <see cref="Span.MaxStackalloc"/> for details about stack- and heap-allocation.</para></remarks>
    /// <typeparam name="T">The type of buffer.</typeparam>
    /// <param name="it">The length of the buffer.</param>
    /// <returns>The allocated buffer.</returns>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining), MustDisposeResource, Pure]
    public static PooledSmallList<T> Alloc<T>(this in int it)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        return it switch
        {
            <= 0 => default, // No allocation
#if !CSHARPREPL // This only works with InlineMethod.Fody. Without it, the span points to deallocated stack memory.
            _ when !IsReferenceOrContainsReferences<T>() && IsStack<T>(it) => Stackalloc<T>(it), // Stack allocation
#endif
            _ => it, // Heap allocation
        };
    }

    /// <summary>Creates a new instance of the <see cref="PooledSmallList{T}"/> struct.</summary>
    /// <typeparam name="T">The type of the collection.</typeparam>
    /// <param name="capacity">
    /// The initial allocation, which puts it on the heap immediately but can save future resizing.
    /// </param>
    /// <returns>The created instance of <see cref="PooledSmallList{T}"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static PooledSmallList<T> ToPooledSmallList<T>(this int capacity)
#if UNMANAGED_SPAN
    where T : unmanaged
#endif
        =>
            new(capacity);

    /// <inheritdoc cref="Emik.Morsels.PooledSmallList{T}(Span{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), MustDisposeResource, Pure]
    public static PooledSmallList<T> ToPooledSmallList<T>(this Span<T> span)
#if UNMANAGED_SPAN
    where T : unmanaged
#endif
        =>
            new(span);

    /// <inheritdoc cref="Emik.Morsels.PooledSmallList{T}(Span{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), MustDisposeResource, Pure]
    public static PooledSmallList<T> ToPooledSmallList<T>(this IEnumerable<T>? enumerable)
#if UNMANAGED_SPAN
    where T : unmanaged
#endif
        =>
            (enumerable?.TryCount() is { } count ? count.ToPooledSmallList<T>() : default).Append(enumerable);
#endif
    /// <summary>Collects the enumerable; allocating the heaped list lazily.</summary>
    /// <typeparam name="T">The type of the <paramref name="iterable"/> and the <see langword="return"/>.</typeparam>
    /// <param name="iterable">The collection to turn into a <see cref="SmallList{T}"/>.</param>
    /// <returns>A <see cref="SmallList{T}"/> of <paramref name="iterable"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SmallList<T> ToSmallList<T>([InstantHandle] this IEnumerable<T>? iterable) =>
        iterable is null ? default : [..iterable];

    /// <summary>Mutates the enumerator; allocating the heaped list lazily.</summary>
    /// <typeparam name="T">The type of the <paramref name="iterator"/> and the <see langword="return"/>.</typeparam>
    /// <param name="iterator">The collection to turn into a <see cref="SmallList{T}"/>.</param>
    /// <returns>A <see cref="SmallList{T}"/> of <paramref name="iterator"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SmallList<T> ToSmallList<T>(this IEnumerator<T>? iterator) => new(iterator);
}
