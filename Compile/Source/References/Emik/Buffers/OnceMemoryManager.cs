// SPDX-License-Identifier: MPL-2.0
#if ROSLYN || NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
// ReSharper disable once CheckNamespace EmptyNamespace
namespace Emik.Morsels;

using static Span;

/// <summary>Encapsulates a single value to be exposed as a <see cref="Memory{T}"/> of size 1.</summary>
/// <typeparam name="T">The type of value.</typeparam>
/// <param name="value">The value to encapsulate.</param>
sealed partial class OnceMemoryManager<T>(in T value) : MemoryManager<T>
{
    GCHandle _handle;

    // ReSharper disable once ReplaceWithPrimaryConstructorParameter
    T _value = value;

    /// <summary>Gets the value.</summary>
    public ref T Value
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => ref _value;
    }

    /// <summary>Wraps the <typeparamref name="T"/> instance into the <see cref="OnceMemoryManager{T}"/>.</summary>
    /// <param name="value">The value to wrap.</param>
    /// <returns>The wrapped value.</returns>
    public static explicit operator OnceMemoryManager<T>(in T value) => new(value);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#pragma warning disable IDISP010, IDISP023
    protected override void Dispose(bool disposing) => Unpin();
#pragma warning restore IDISP010, IDISP023
    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void Unpin()
    {
        if (_handle.IsAllocated)
            _handle.Free();
    }

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
#endif
