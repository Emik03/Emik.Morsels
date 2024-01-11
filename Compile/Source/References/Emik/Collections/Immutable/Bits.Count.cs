// SPDX-License-Identifier: MPL-2.0
#pragma warning disable IDE0250
// ReSharper disable BadPreprocessorIndent CheckNamespace RedundantUnsafeContext StructCanBeMadeReadOnly
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
    /// <inheritdoc cref="IList{T}.Item(int)"/>
    [CollectionAccess(CollectionAccessType.Read)]
    public unsafe T this[[NonNegativeValue] int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        get
        {
            fixed (T* ptr = &_value)
                return Nth(ptr, index);
        }
    }

    /// <inheritdoc cref="IList{T}.Item"/>
    T IList<T>.this[[NonNegativeValue] int index]
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

#pragma warning disable MA0051 // ReSharper disable CognitiveComplexity
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static unsafe void MovePopCount(ref byte* ptr, in byte* upper, ref int x)
    {
        for (; sizeof(T) >= sizeof(nuint) && ptr <= upper - sizeof(nuint); ptr += sizeof(nuint))
            if (BitOperations.PopCount(*(nuint*)ptr) is var i && i <= x)
                x -= i;
            else
                break;

        for (; sizeof(T) % sizeof(nuint) >= sizeof(ulong) && ptr <= upper - sizeof(ulong); ptr += sizeof(ulong))
            if (BitOperations.PopCount(*(ulong*)ptr) is var i && i <= x)
                x -= i;
            else
                break;

        for (; sizeof(T) % sizeof(ulong) >= sizeof(uint) && ptr <= upper - sizeof(uint); ptr += sizeof(uint))
            if (BitOperations.PopCount(*(uint*)ptr) is var i && i <= x)
                x -= i;
            else
                break;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    static unsafe byte Find(ref byte* ptr, in byte* upper, int x)
    {
        for (; ptr < upper; ptr++)
        {
            if ((*ptr & 1) is not 0 && x-- is 0)
                return 1;

            if ((*ptr & 1 << 1) is not 0 && x-- is 0)
                return 1 << 1;

            if ((*ptr & 1 << 2) is not 0 && x-- is 0)
                return 1 << 2;

            if ((*ptr & 1 << 3) is not 0 && x-- is 0)
                return 1 << 3;

            if ((*ptr & 1 << 4) is not 0 && x-- is 0)
                return 1 << 4;

            if ((*ptr & 1 << 5) is not 0 && x-- is 0)
                return 1 << 5;

            if ((*ptr & 1 << 6) is not 0 && x-- is 0)
                return 1 << 6;

            if ((*ptr & 1 << 7) is not 0 && x-- is 0)
                return 1 << 7;
        }

        return 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    static unsafe int PopCount(T* value)
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

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] // ReSharper disable once RedundantUnsafeContext
    static unsafe int TrailingZeroCount(nuint value)
#if NET7_0_OR_GREATER
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
    static unsafe int PopCountRemainder(byte* remainder) =>
        BitOperations.PopCount(
            (sizeof(T) % sizeof(ulong)) switch
            {
                1 => *remainder,
                2 => *(ushort*)remainder,
                3 => *(ushort*)remainder | (ulong)remainder[2] << 16,
                4 => *(uint*)remainder,
                5 => *(uint*)remainder | (ulong)remainder[4] << 32,
                6 => *(uint*)remainder | (ulong)*(ushort*)remainder[4] << 32,
                7 => *(uint*)remainder | (ulong)*(ushort*)remainder[4] << 32 | (ulong)remainder[6] << 48,
                _ => throw new InvalidOperationException("sizeof(T) is assumed to be within [1, 7]."),
            }
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    static unsafe T Create(T* p, byte* ptr, byte i)
    {
        T t = default;
        ((byte*)&t)[ptr - (byte*)p] = i;
        return t;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    static unsafe T Nth(T* p, [NonNegativeValue] int index)
    {
        var x = index;
        var ptr = (byte*)p;
        var upper = (byte*)(p + 1);

        if (sizeof(T) >= sizeof(uint))
            MovePopCount(ref ptr, upper, ref x);

        return Find(ref ptr, upper, x) is not 0 and var i
            ? Create(p, ptr, i)
            : throw new ArgumentOutOfRangeException(nameof(index), index, null);
    }
}
