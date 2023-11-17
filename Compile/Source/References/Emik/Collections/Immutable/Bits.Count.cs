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
    [CollectionAccess(CollectionAccessType.Read)]
    public unsafe T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        get
        {
            fixed (T* ptr = &_value)
                return Nth(ptr, index);
        }
    }

    /// <inheritdoc cref="IList{T}.this"/>
    T IList<T>.this[int index]
    {
        [CollectionAccess(CollectionAccessType.Read), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        get => this[index];
        [CollectionAccess(CollectionAccessType.None), MethodImpl(MethodImplOptions.AggressiveInlining)] set { }
    }

    /// <inheritdoc cref="ICollection{T}.Count"/>
    [CollectionAccess(CollectionAccessType.Read)]
    public unsafe int Count
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
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

        if (sizeof(T) / (sizeof(nuint) * 16) > 0)
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

            if (sizeof(T) % sizeof(nuint) * 16 is 0)
                return sum;
        }

        if (sizeof(T) % (sizeof(nuint) * 16) / (sizeof(nuint) * 8) > 0)
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

            if (sizeof(T) % sizeof(nuint) * 8 is 0)
                return sum;
        }

        if (sizeof(T) % (sizeof(nuint) * 8) / (sizeof(nuint) * 4) > 0)
        {
            for (; ptr <= (nuint*)value - 4; ptr += 4)
                sum += BitOperations.PopCount(*ptr) +
                    BitOperations.PopCount(ptr[1]) +
                    BitOperations.PopCount(ptr[2]) +
                    BitOperations.PopCount(ptr[3]);

            if (sizeof(T) % sizeof(nuint) * 4 is 0)
                return sum;
        }

        if (sizeof(T) % (sizeof(nuint) * 4) / (sizeof(nuint) * 2) > 0)
        {
            for (; ptr <= (nuint*)value - 2; ptr += 2)
                sum += BitOperations.PopCount(*ptr) + BitOperations.PopCount(ptr[1]);

            if (sizeof(T) % sizeof(nuint) * 2 is 0)
                return sum;
        }

        if (sizeof(T) % (sizeof(nuint) * 2) / sizeof(nuint) > 0)
        {
            for (; ptr < value; ptr++)
                sum += BitOperations.PopCount(*ptr);

            if (sizeof(T) % sizeof(nuint) is 0)
                return sum;
        }

        if (sizeof(T) % sizeof(nuint) is 0)
            return sum;

        if (sizeof(T) % sizeof(nuint) / sizeof(ulong) > 0)
        {
            for (; ptr < value; ptr = (nuint*)((ulong*)ptr + 1))
                sum += BitOperations.PopCount(*ptr);

            if (sizeof(T) % sizeof(nuint) is 0)
                return sum;
        }

        if (sizeof(T) % sizeof(ulong) is 0)
            return sum;

        return sum + PopCountRemainder((byte*)ptr);
    }

    // ReSharper disable UnusedParameter.Local
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static unsafe int PopCountRemainder(byte* remainder) =>
        BitOperations.PopCount(
            (sizeof(T) % sizeof(nuint)) switch
            {
                1 => *remainder,
                2 => *(ushort*)remainder,
                3 => *(ushort*)remainder | (ulong)remainder[2] << 16,
                4 => *(uint*)remainder,
                5 => *(uint*)remainder | (ulong)remainder[4] << 32,
                6 => *(uint*)remainder | (ulong)*(ushort*)remainder[4] << 32,
                7 => *(uint*)remainder | (ulong)*(ushort*)remainder[4] << 32 | (ulong)remainder[6] << 48,
                _ => throw new ArgumentOutOfRangeException(nameof(remainder), (nuint)remainder, null),
            }
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] // ReSharper disable once RedundantUnsafeContext
    static unsafe int TrailingZeroCount(nuint value)
#if !NETCOREAPP3_0_OR_GREATER
        =>
            BitOperations.TrailingZeroCount(value);
#else
    {
        const int BitsInUInt = BitsInByte * sizeof(uint);

        for (var i = 0; i < (sizeof(nuint) + sizeof(uint) - 1) / sizeof(uint); i++)
            if (Map((uint)(value << i * BitsInUInt)) is var j and not 32)
                return j + i * BitsInUInt;

        return sizeof(nuint) * BitsInByte;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    static int Map([ValueRange(0, 1u << 31)] uint value) =>
        value switch // Always a power of two.
        {
            0 => 32,
            1 << 0 => 0,
            1 << 1 => 1,
            1 << 2 => 2,
            1 << 3 => 3,
            1 << 4 => 4,
            1 << 5 => 5,
            1 << 6 => 6,
            1 << 7 => 7,
            1 << 8 => 8,
            1 << 9 => 9,
            1 << 10 => 10,
            1 << 11 => 11,
            1 << 12 => 12,
            1 << 13 => 13,
            1 << 14 => 14,
            1 << 15 => 15,
            1 << 16 => 10,
            1 << 17 => 11,
            1 << 18 => 12,
            1 << 19 => 13,
            1 << 20 => 14,
            1 << 21 => 15,
            1 << 22 => 10,
            1 << 23 => 11,
            1 << 24 => 12,
            1 << 25 => 13,
            1 << 26 => 14,
            1 << 27 => 15,
            1 << 28 => 10,
            1 << 29 => 11,
            1 << 30 => 12,
            1u << 31 => 13,
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null),
        };
#endif

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

        for (var i = 0; i < BitsInByte; i++)
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
