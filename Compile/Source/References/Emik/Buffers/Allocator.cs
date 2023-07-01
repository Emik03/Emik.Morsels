// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace
namespace Emik.Morsels;
#pragma warning disable 8500
/// <summary>Provides the method to convert spans.</summary>
static partial class Allocator
{
    /// <summary>Reinterprets the span as a series of managed types.</summary>
    /// <typeparam name="T">The type of span to convert to.</typeparam>
    /// <param name="span">The span to convert.</param>
    /// <returns>
    /// The span that points to the same region as <paramref name="span"/>
    /// but with each time assumed to be <typeparamref name="T"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe Span<T> Reinterpret<T>(this scoped in Span<nint> span)
        where T : class =>
        MemoryMarshal.CreateSpan(ref *(T*)span[0], span.Length);

    /// <summary>Reinterprets the span as a series of managed types.</summary>
    /// <typeparam name="T">The type of span to convert to.</typeparam>
    /// <param name="span">The span to convert.</param>
    /// <returns>
    /// The span that points to the same region as <paramref name="span"/>
    /// but with each time assumed to be <typeparamref name="T"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe Span<T> Reinterpret<T>(this scoped in Span<nuint> span)
        where T : class =>
        MemoryMarshal.CreateSpan(ref *(T*)span[0], span.Length);

    /// <summary>Reinterprets the span as a series of managed types.</summary>
    /// <typeparam name="T">The type of span to convert to.</typeparam>
    /// <param name="span">The span to convert.</param>
    /// <returns>
    /// The span that points to the same region as <paramref name="span"/>
    /// but with each time assumed to be <typeparamref name="T"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe ReadOnlySpan<T> Reinterpret<T>(this scoped in ReadOnlySpan<nint> span) =>
        MemoryMarshal.CreateReadOnlySpan(ref *(T*)span[0], span.Length);

    /// <summary>Reinterprets the span as a series of managed types.</summary>
    /// <typeparam name="T">The type of span to convert to.</typeparam>
    /// <param name="span">The span to convert.</param>
    /// <returns>
    /// The span that points to the same region as <paramref name="span"/>
    /// but with each time assumed to be <typeparamref name="T"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe ReadOnlySpan<T> Reinterpret<T>(this scoped in ReadOnlySpan<nuint> span) =>
        MemoryMarshal.CreateReadOnlySpan(ref *(T*)span[0], span.Length);

    /// <inheritdoc cref="Raw{T}(T)" />
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe byte[] Raw<T>(Span<T> value) =>
        MemoryMarshal.CreateReadOnlySpan(ref *(byte*)&value, sizeof(Span<T>)).ToArray();

    /// <inheritdoc cref="Raw{T}(T)" />
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe byte[] Raw<T>(SplitSpan<T> value)
#if UNMANAGED_SPAN
        where T : unmanaged, IEquatable<T>?
#else
        where T : IEquatable<T>?
#endif
        =>
            MemoryMarshal.CreateReadOnlySpan(ref *(byte*)&value, sizeof(SplitSpan<T>)).ToArray();

    /// <inheritdoc cref="Raw{T}(T)" />
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe byte[] Raw<T>(ReadOnlySpan<T> value) =>
        MemoryMarshal.CreateReadOnlySpan(ref *(byte*)&value, sizeof(ReadOnlySpan<T>)).ToArray();

    /// <summary>Reads the raw memory of the object.</summary>
    /// <typeparam name="T">The type of value to read.</typeparam>
    /// <param name="value">The value to read.</param>
    /// <returns>The raw memory of the parameter <paramref name="value"/>.</returns>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe byte[] Raw<T>(T value) =>
        MemoryMarshal.CreateReadOnlySpan(ref *(byte*)&value, sizeof(T)).ToArray();
}
