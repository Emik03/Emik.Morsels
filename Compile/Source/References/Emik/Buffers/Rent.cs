// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace
namespace Emik.Morsels;
#pragma warning disable 8500
// ReSharper disable once RedundantNameQualifier
using static System.Runtime.CompilerServices.RuntimeHelpers;
using static Span;

/// <summary>Extension methods to allocate temporary buffers.</summary>
static partial class Rent
{
    /// <summary>Allocates the buffer on the stack or heap, and gives it to the caller.</summary>
    /// <remarks><para>See <see cref="Span.MaxStackalloc"/> for details about stack- and heap-allocation.</para></remarks>
    /// <typeparam name="T">The type of buffer.</typeparam>
    /// <param name="it">The length of the buffer.</param>
    /// <param name="span">The temporary allocation.</param>
    /// <returns>The allocated buffer.</returns>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining), MustDisposeResource, Pure]
    public static Rented<T> Alloc<T>(this in int it, out Span<T> span)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            it switch
            {
                <= 0 when (span = default) is var _ => default, // No allocation
#if !CSHARPREPL // This only works with InlineMethod.Fody. Without it, the span points to deallocated stack memory.
                _ when !IsReferenceOrContainsReferences<T>() &&
                    IsStack<T>(it) &&
                    (span = Stackalloc<T>(it)) is var _ => default, // Stack allocation
#endif
                _ => new(it, out span), // Heap allocation
            };

    /// <summary>Allocates the buffer on the stack or heap, and gives it to the caller.</summary>
    /// <remarks><para>See <see cref="Span.MaxStackalloc"/> for details about stack- and heap-allocation.</para></remarks>
    /// <typeparam name="T">The type of buffer.</typeparam>
    /// <param name="it">The length of the buffer.</param>
    /// <param name="ptr">The temporary allocation.</param>
    /// <returns>The allocated buffer.</returns>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining), MustDisposeResource, Pure]
    public static unsafe Rented<T>.Pinned Alloc<T>(this in int it, out T* ptr) =>
        it switch
        {
            <= 0 when (ptr = default) is var _ => default, // No allocation
#if !CSHARPREPL // This only works with InlineMethod.Fody. Without it, the span points to deallocated stack memory.
            _ when !IsReferenceOrContainsReferences<T>() &&
                IsStack<T>(it) &&
                (ptr = StackallocPtr<T>(it)) is var _ => default, // Stack allocation
#endif
            _ => new(it, out ptr), // Heap allocation
        };
}

/// <summary>Represents the rented array.</summary>
/// <typeparam name="T">The type of array to rent.</typeparam>
[StructLayout(LayoutKind.Auto)]
partial struct Rented<T> : IDisposable
{
    /// <summary>Represents the pinned array.</summary>
    [StructLayout(LayoutKind.Auto)]
    public partial struct Pinned : IDisposable
    {
        GCHandle _handle;

        Rented<T> _rented;

        /// <summary>Initializes a new instance of the <see cref="Pinned"/> struct.</summary>
        /// <param name="rented">The rented array.</param>
        /// <param name="ptr">The pointer to the allocated memory.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe Pinned(Rented<T> rented, out T* ptr)
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
        {
            _rented = rented;
            _handle = GCHandle.Alloc(rented._array, GCHandleType.Pinned);
            ptr = (T*)_handle.AddrOfPinnedObject();
        }
#else
            =>
                ptr = (T*)(_rented = rented)._ptr;
#endif
        /// <summary>Initializes a new instance of the <see cref="Pinned"/> struct.</summary>
        /// <param name="length">The length of the array to retrieve.</param>
        /// <param name="ptr">The pointer to the allocated memory.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe Pinned(int length, out T* ptr)
            : this(new Rented<T>(length), out ptr) { }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            if (!_handle.IsAllocated)
                return;

            _handle.Free();
            _handle = default;
            _rented.Dispose();
        }
    }
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
    T[]? _array;
#else
    nint _ptr;

    static Rented()
    {
        if (IsReferenceOrContainsReferences<T>())
            throw new NotSupportedException($"Type {typeof(T)} cannot be rented in this framework.");
    }
#endif
    /// <summary>Initializes a new instance of the <see cref="Rent"/> struct. Rents the array.</summary>
    /// <param name="length">The length of the array to retrieve.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)] // ReSharper disable once RedundantUnsafeContext
    public unsafe Rented(int length) =>
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
        _array = ArrayPool<T>.Shared.Rent(length);
#else
        _ptr = Marshal.AllocHGlobal(length * sizeof(T));
#endif
    /// <summary>Initializes a new instance of the <see cref="Rent"/> struct. Rents the array.</summary>
    /// <param name="length">The length of the array to retrieve.</param>
    /// <param name="span">The resulting <see cref="Span{T}"/>.</param>
    // ReSharper disable once RedundantUnsafeContext
    [MethodImpl(MethodImplOptions.AggressiveInlining)] // ReSharper disable once RedundantUnsafeContext
    public unsafe Rented(int length, out Span<T> span) =>
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
        span = (_array = ArrayPool<T>.Shared.Rent(length)).AsSpan().UnsafelyTake(length);
#else
        span = new((void*)(_ptr = Marshal.AllocHGlobal(length * sizeof(T))), length);
#endif
    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
        if (_array is null)
            return;

        ArrayPool<T>.Shared.Return(_array);
        _array = null;
#else
        if (_ptr is 0)
            return;

        Marshal.FreeHGlobal(_ptr);
        _ptr = 0;
#endif
    }
}
