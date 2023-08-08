// SPDX-License-Identifier: MPL-2.0

// ReSharper disable CheckNamespace RedundantUsingDirective
namespace Emik.Morsels;
#pragma warning disable 1574, 8500
using static Span;

/// <summary>Provides the method to convert spans.</summary>
static partial class Allocator
{
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
    /// <inheritdoc cref="Raw{T}(T)" />
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe byte[] Raw<T>(scoped PooledSmallList<T> value) =>
        MemoryMarshal.CreateReadOnlySpan(ref *(byte*)&value, sizeof(PooledSmallList<T>)).ToArray();
#endif

    /// <inheritdoc cref="Raw{T}(T)" />
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe byte[] Raw<T>(scoped Span<T> value) =>
        MemoryMarshal.CreateReadOnlySpan(ref *(byte*)&value, sizeof(Span<T>)).ToArray();

    /// <inheritdoc cref="Raw{T}(T)" />
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe byte[] Raw<T>(scoped SplitSpan<T> value)
#if UNMANAGED_SPAN
        where T : unmanaged, IEquatable<T>?
#else
        where T : IEquatable<T>?
#endif
        =>
            MemoryMarshal.CreateReadOnlySpan(ref *(byte*)&value, sizeof(SplitSpan<T>)).ToArray();

    /// <inheritdoc cref="Raw{T}(T)" />
#pragma warning restore 1574
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe byte[] Raw<T>(scoped ReadOnlySpan<T> value) =>
        MemoryMarshal.CreateReadOnlySpan(ref *(byte*)&value, sizeof(ReadOnlySpan<T>)).ToArray();
#if NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP
    /// <summary>Reads the raw memory of the object.</summary>
    /// <typeparam name="T">The type of value to read.</typeparam>
    /// <param name="value">The value to read.</param>
    /// <returns>The raw memory of the parameter <paramref name="value"/>.</returns>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] Raw<T>(T value) =>
        MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<T, byte>(ref value), Unsafe.SizeOf<T>()).ToArray();
#endif
}
