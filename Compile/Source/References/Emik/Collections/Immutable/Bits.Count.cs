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
        [CollectionAccess(CollectionAccessType.Read), MethodImpl(MethodImplOptions.AggressiveInlining)] get => this[index];
        [CollectionAccess(CollectionAccessType.None), MethodImpl(MethodImplOptions.AggressiveInlining)] set { }
    }

    /// <inheritdoc cref="ICollection{T}.Count"/>
    [CollectionAccess(CollectionAccessType.Read), Pure]
    public unsafe int Count
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            fixed (T* ptr = &_value)
                return PopCount(ptr);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#pragma warning disable MA0051 // ReSharper disable once CognitiveComplexity
    static unsafe int PopCount(T* value)
#pragma warning restore MA0051
    {
        var ptr = (nuint*)value++;
        var sum = 0;

        if (sizeof(T) / (nuint.Size * 16) > 0)
        {
            for (; ptr <= (nuint*)value - 16; ptr += 16)
                sum += BitOperations.PopCount(*ptr) +
                    BitOperations.PopCount(ptr[1]) +
                    BitOperations.PopCount(ptr[2]) +
                    BitOperations.PopCount(ptr[3]) +
                    BitOperations.PopCount(ptr[4]) +
                    BitOperations.PopCount(ptr[5]) +
                    BitOperations.PopCount(ptr[6]) +
                    BitOperations.PopCount(ptr[7]) +
                    BitOperations.PopCount(ptr[8]) +
                    BitOperations.PopCount(ptr[9]) +
                    BitOperations.PopCount(ptr[10]) +
                    BitOperations.PopCount(ptr[11]) +
                    BitOperations.PopCount(ptr[12]) +
                    BitOperations.PopCount(ptr[13]) +
                    BitOperations.PopCount(ptr[14]) +
                    BitOperations.PopCount(ptr[15]);

            if (sizeof(T) % nuint.Size * 16 is 0)
                return sum;
        }

        if (sizeof(T) % (nuint.Size * 16) / (nuint.Size * 8) > 0)
        {
            for (; ptr <= (nuint*)value - 8; ptr += 8)
                sum += BitOperations.PopCount(*ptr) +
                    BitOperations.PopCount(ptr[1]) +
                    BitOperations.PopCount(ptr[2]) +
                    BitOperations.PopCount(ptr[3]) +
                    BitOperations.PopCount(ptr[4]) +
                    BitOperations.PopCount(ptr[5]) +
                    BitOperations.PopCount(ptr[6]) +
                    BitOperations.PopCount(ptr[7]);

            if (sizeof(T) % nuint.Size * 8 is 0)
                return sum;
        }

        if (sizeof(T) % (nuint.Size * 8) / (nuint.Size * 4) > 0)
        {
            for (; ptr <= (nuint*)value - 4; ptr += 4)
                sum += BitOperations.PopCount(*ptr) +
                    BitOperations.PopCount(ptr[1]) +
                    BitOperations.PopCount(ptr[2]) +
                    BitOperations.PopCount(ptr[3]);

            if (sizeof(T) % nuint.Size * 4 is 0)
                return sum;
        }

        if (sizeof(T) % (nuint.Size * 4) / (nuint.Size * 2) > 0)
        {
            for (; ptr <= (nuint*)value - 2; ptr += 2)
                sum += BitOperations.PopCount(*ptr) + BitOperations.PopCount(ptr[1]);

            if (sizeof(T) % nuint.Size * 2 is 0)
                return sum;
        }

        if (sizeof(T) % (nuint.Size * 2) / nuint.Size > 0)
        {
            for (; ptr <= (nuint*)value; ptr++)
                sum += BitOperations.PopCount(*ptr);

            if (sizeof(T) % nuint.Size is 0)
                return sum;
        }

        if (sizeof(T) % nuint.Size is 0)
            return sum;

        return sum + PopCountRemainder((byte*)ptr);
    }

    // ReSharper disable UnusedParameter.Local
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static unsafe int PopCountRemainder(byte* remainder) =>
        BitOperations.PopCount(
            (sizeof(T) % nuint.Size) switch
            {
                1 => *remainder,
                2 => *(ushort*)remainder,
                3 => *(ushort*)remainder | (ulong)remainder[2] << 16,
                4 => *(uint*)remainder,
                5 => *(uint*)remainder | (ulong)remainder[4] << 32,
                6 => *(uint*)remainder | (ulong)*(ushort*)remainder[4] << 32,
                7 => *(uint*)remainder | (ulong)*(ushort*)remainder[4] << 32 | (ulong)remainder[6] << 48,
                _ => throw Unreachable,
            }
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#pragma warning disable MA0051 // ReSharper disable once CognitiveComplexity
    static unsafe T Nth(T* p, int index)
#pragma warning restore MA0051
    {
        var x = index;
        var ptr = (nuint*)p;

        for (; ptr < p + 1 && x > 0; ptr++)
            if (BitOperations.PopCount(*ptr) is var i && i <= x)
                x -= i;
            else
                break;

        for (; ptr < (byte*)p + sizeof(T) && x > 0; ptr = (nuint*)((byte*)ptr + 1))
            if (BitOperations.PopCount(*(byte*)ptr) is var i && i <= x)
                x -= i;
            else
                break;

        var last = *(byte*)ptr;

        for (var i = 0; i < 8; i++)
            if ((last & 1 << i) is not 0)
                if (x is 0)
                {
                    T t = default;
                    ((byte*)&t)[(byte*)ptr - (byte*)p] = (byte)(1 << i);
                    return t;
                }
                else
                    x--;

        throw new ArgumentOutOfRangeException(nameof(index), index, null);
    }
}