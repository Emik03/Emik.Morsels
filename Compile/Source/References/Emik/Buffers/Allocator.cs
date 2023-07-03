// SPDX-License-Identifier: MPL-2.0

// ReSharper disable CheckNamespace RedundantUsingDirective
namespace Emik.Morsels;
#pragma warning disable 8500
using static Span;

/// <summary>Provides the method to convert spans.</summary>
static partial class Allocator
{
#if !CSHARPREPL
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
    /// <summary>Allocates the buffer on the stack or heap, and gives it to the caller.</summary>
    /// <remarks><para>
    /// This method is aggressively inlined.
    /// </para><para>
    /// See <see cref="StackallocSize"/> for details about stack- and heap-allocation.
    /// </para></remarks>
    /// <typeparam name="T">The type of buffer.</typeparam>
    /// <param name="length">The length of the buffer.</param>
    /// <returns>The allocated buffer.</returns>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> Alloc<T>(this in int length)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            length switch
            {
                <= 0 => default, // No allocation needed
                _ when length <= StackallocSize / Unsafe.SizeOf<T>() => length.Stackalloc<T>(), // Stack-allocated buffer
                _ => new T[length], // Heap-allocated buffer
            };
#endif

    /// <summary>Stack-allocates the buffer, and gives it to the caller.</summary>
    /// <remarks><para>This method is aggressively inlined.</para></remarks>
    /// <typeparam name="T">The type of buffer.</typeparam>
    /// <param name="length">The length of the buffer.</param>
    /// <returns>The stack-allocated buffer.</returns>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe Span<T> Stackalloc<T>(this in int length)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        var pointer = stackalloc byte[unchecked(Unsafe.SizeOf<T>() * length)];
        return MemoryMarshal.CreateSpan(ref Unsafe.AsRef<T>(pointer), length);
    }
#endif
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
    /// <summary>Reinterprets the span as a series of managed types.</summary>
    /// <typeparam name="T">The type of span to convert to.</typeparam>
    /// <param name="span">The span to convert.</param>
    /// <returns>
    /// The span that points to the same region as <paramref name="span"/>
    /// but with each time assumed to be <typeparamref name="T"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> Reinterpret<T>(this Span<nint> span)
        where T : class =>
        MemoryMarshal.CreateSpan(ref Unsafe.As<nint, T>(ref span[0]), span.Length);

    /// <summary>Reinterprets the span as a series of managed types.</summary>
    /// <typeparam name="T">The type of span to convert to.</typeparam>
    /// <param name="span">The span to convert.</param>
    /// <returns>
    /// The span that points to the same region as <paramref name="span"/>
    /// but with each time assumed to be <typeparamref name="T"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> Reinterpret<T>(this Span<nuint> span)
        where T : class =>
        MemoryMarshal.CreateSpan(ref Unsafe.As<nuint, T>(ref span[0]), span.Length);

    /// <summary>Reinterprets the span as a series of managed types.</summary>
    /// <typeparam name="T">The type of span to convert to.</typeparam>
    /// <param name="span">The span to convert.</param>
    /// <returns>
    /// The span that points to the same region as <paramref name="span"/>
    /// but with each time assumed to be <typeparamref name="T"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> Reinterpret<T>(this ReadOnlySpan<nint> span) =>
        MemoryMarshal.CreateSpan(ref Unsafe.As<nint, T>(ref Unsafe.AsRef(in span[0])), span.Length);

    /// <summary>Reinterprets the span as a series of managed types.</summary>
    /// <typeparam name="T">The type of span to convert to.</typeparam>
    /// <param name="span">The span to convert.</param>
    /// <returns>
    /// The span that points to the same region as <paramref name="span"/>
    /// but with each time assumed to be <typeparamref name="T"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> Reinterpret<T>(this ReadOnlySpan<nuint> span) =>
        MemoryMarshal.CreateSpan(ref Unsafe.As<nuint, T>(ref Unsafe.AsRef(in span[0])), span.Length);
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
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe byte[] Raw<T>(scoped ReadOnlySpan<T> value) =>
        MemoryMarshal.CreateReadOnlySpan(ref *(byte*)&value, sizeof(ReadOnlySpan<T>)).ToArray();

    /// <summary>Reads the raw memory of the object.</summary>
    /// <typeparam name="T">The type of value to read.</typeparam>
    /// <param name="value">The value to read.</param>
    /// <returns>The raw memory of the parameter <paramref name="value"/>.</returns>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] Raw<T>(T value) =>
        MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<T, byte>(ref value), Unsafe.SizeOf<T>()).ToArray();
}
