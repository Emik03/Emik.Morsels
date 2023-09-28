// SPDX-License-Identifier: MPL-2.0

// ReSharper disable CheckNamespace StructCanBeMadeReadOnly
namespace Emik.Morsels;

/// <inheritdoc cref="Bits{T}"/>
#if CSHARPREPL
public
#endif
#if !NO_READONLY_STRUCTS
readonly
#endif
    partial struct Bits<T>
{
    /// <inheritdoc cref="IList{T}.this[int]"/>
    [CollectionAccess(CollectionAccessType.Read), Pure]
    public unsafe T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            fixed (T* ptr = &_value)
                return Nth(ptr, index);
        }
    }

    /// <inheritdoc cref="IList{T}.this"/>
    [Pure]
    T IList<T>.this[int index]
    {
        [CollectionAccess(CollectionAccessType.Read), MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this[index];
        [CollectionAccess(CollectionAccessType.None), MethodImpl(MethodImplOptions.AggressiveInlining)] set { }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#pragma warning disable MA0051 // ReSharper disable once CognitiveComplexity
    static unsafe T Nth(T* p, int x)
#pragma warning restore MA0051
    {
        var index = x;
        var ptr = (nuint*)p;

        for (; ptr < p + 1 && index > 0; ptr++)
            if (BitOperations.PopCount(*ptr) is var i && i <= index)
                index -= i;
            else
                break;

        for (; ptr < (byte*)p + sizeof(T) && index > 0; ptr = (nuint*)((byte*)ptr + 1))
            if (BitOperations.PopCount(*(byte*)ptr) is var i && i <= index)
                index -= i;
            else
                break;

        var last = *(byte*)ptr;

        for (var i = 0; i < 8; i++)
            if ((last & 1 << i) is not 0)
                if (index is 0)
                {
                    T t = default;
                    ((byte*)&t)[(byte*)ptr - (byte*)p] = (byte)(1 << i);
                    return t;
                }
                else
                    index--;

        throw new ArgumentOutOfRangeException(nameof(index), x, null);
    }
}
