// SPDX-License-Identifier: MPL-2.0

// ReSharper disable CheckNamespace ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator LoopCanBeConvertedToQuery MergeIntoPattern NullableWarningSuppressionIsUsed RedundantUsingDirective SuggestBaseTypeForParameter
namespace Emik.Morsels;
#pragma warning disable 1574, 8500, MA0051
using static Span;

/// <summary>Provides the method to convert spans.</summary>
static partial class Allocator
{
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
    /// <inheritdoc cref="Raw{T}(T)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static unsafe byte[] Raw<T>(scoped PooledSmallList<T> value) =>
        [.. MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef<byte>(&value), sizeof(PooledSmallList<T>))];
#endif

    /// <inheritdoc cref="Raw{T}(T)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static unsafe byte[] Raw<T>(scoped Span<T> value) =>
        [.. MemoryMarshal.CreateReadOnlySpan(ref *(byte*)&value, sizeof(Span<T>))];

    /// <inheritdoc cref="Raw{T}(T)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static unsafe byte[] Raw<TBody, TSeparator, TStrategy>(scoped SplitSpan<TBody, TSeparator, TStrategy> value)
#if UNMANAGED_SPAN
        where TBody : unmanaged, IEquatable<TBody>?
#else
        where TBody : IEquatable<TBody>?
#endif
#if !NET7_0_OR_GREATER
        where TSeparator : IEquatable<TSeparator>?
#endif
        =>
            [.. MemoryMarshal.CreateReadOnlySpan(ref *(byte*)&value, sizeof(SplitSpan<TBody, TSeparator, TStrategy>))];

    /// <inheritdoc cref="Raw{T}(T)" />
#pragma warning restore 1574
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static unsafe byte[] Raw<T>(scoped ReadOnlySpan<T> value) =>
       [.. MemoryMarshal.CreateReadOnlySpan(ref *(byte*)&value, sizeof(ReadOnlySpan<T>))];
#if NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP
    /// <summary>Reads the raw memory of the object.</summary>
    /// <typeparam name="T">The type of value to read.</typeparam>
    /// <param name="value">The value to read.</param>
    /// <returns>The raw memory of the parameter <paramref name="value"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static byte[] Raw<T>(T value) =>
        [.. MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<T, byte>(ref value), Unsafe.SizeOf<T>())];
#endif
}
