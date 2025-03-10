// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

using static Span;

/// <summary>Encapsulates a single value to be exposed as a <see cref="Memory{T}"/> of size 1.</summary>
/// <typeparam name="T">The type of value.</typeparam>
/// <param name="value">The value to encapsulate.</param>
sealed partial class OnceMemoryManager<T>(T value) : MemoryManager<T>
{
    GCHandle _handle;

    T _value = value;

    /// <summary>Wraps the <typeparamref name="T"/> instance into the <see cref="OnceMemoryManager{T}"/>.</summary>
    /// <param name="value">The value to wrap.</param>
    /// <returns>The wrapped value.</returns>
    public static explicit operator OnceMemoryManager<T>(T value) => new(value);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#pragma warning disable IDISP010 // You cannot call abstract base methods.
    protected override void Dispose(bool disposing)
#pragma warning restore IDISP010
    {
        if (_handle.IsAllocated)
            _handle.Free();
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void Unpin() => Dispose(true);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining), MustUseReturnValue]
    public override unsafe MemoryHandle Pin(int elementIndex = 0) =>
        typeof(T).IsValueType
            ? default
            : new(
                (void*)(_handle.IsAllocated ? _handle : _handle = GCHandle.Alloc(_value, GCHandleType.Pinned))
               .AddrOfPinnedObject()
            );

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public override Span<T> GetSpan() => Ref(ref _value);
}
