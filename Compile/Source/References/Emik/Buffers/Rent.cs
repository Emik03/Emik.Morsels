// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

// ReSharper disable once RedundantNameQualifier
using static System.Runtime.CompilerServices.RuntimeHelpers;
using static Span;

static partial class Rent
{
    /// <summary>Represents the rented array from <see cref="ArrayPool{T}"/>.</summary>
    /// <typeparam name="T">The type of array to rent.</typeparam>
    public struct Rented<T> : IDisposable
    {
        T[]? _array;

        /// <summary>Initializes a new instance of the <see cref="Rent"/> struct. Rents the array.</summary>
        /// <param name="length">The length of the array to retrieve.</param>
        /// <param name="span">
        /// The resulting <see cref="Span{T}"/>. Note that while <see cref="ArrayPool{T}.Rent"/> may return
        /// </param>
        public Rented(in int length, out Span<T> span) =>
            span = (_array = ArrayPool<T>.Shared.Rent(length)).AsSpan().UnsafelyTake(length);

        /// <inheritdoc />
        void IDisposable.Dispose()
        {
            if (_array is null)
                return;

            ArrayPool<T>.Shared.Return(_array);
            _array = null;
        }
    }

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
}