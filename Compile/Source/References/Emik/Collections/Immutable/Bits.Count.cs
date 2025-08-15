// SPDX-License-Identifier: MPL-2.0
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
#pragma warning disable CS8500, IDE0004
// ReSharper disable BadPreprocessorIndent CheckNamespace RedundantUnsafeContext RedundantCast StructCanBeMadeReadOnly
namespace Emik.Morsels;

using Unsafe = System.Runtime.CompilerServices.Unsafe;

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
    public T this[[NonNegativeValue] int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        get
        {
            foreach (var ret in this)
                if (index is 0)
                    return ret;
                else
                    index--;

            throw new ArgumentOutOfRangeException(nameof(index), index, null);
        }
    }

    /// <inheritdoc cref="IList{T}.this"/>
    T IList<T>.this[[NonNegativeValue] int index]
    {
        [CollectionAccess(CollectionAccessType.Read), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        get => this[index];
        [CollectionAccess(CollectionAccessType.None), MethodImpl(MethodImplOptions.AggressiveInlining)] set { }
    }

    /// <inheritdoc cref="ICollection{T}.Count"/>
    [CollectionAccess(CollectionAccessType.Read)]
    public int Count
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        get
        {
            ref var f = ref Unsafe.AsRef(bits);
            ref var l = ref Unsafe.Add(ref f, 1);
            var sum = 0;

            if (Unsafe.SizeOf<T>() >= Unsafe.SizeOf<nint>())
            {
                while (Unsafe.IsAddressLessThan(
                    ref f,
                    ref Unsafe.SubtractByteOffset(ref l, (nint)Unsafe.SizeOf<nint>() - 1)
                ))
                {
                    sum += BitOperations.PopCount(Unsafe.As<T, nuint>(ref f));
                    f = ref Unsafe.As<nint, T>(ref Unsafe.Add(ref Unsafe.As<T, nint>(ref f), 1));
                }

                if (Unsafe.SizeOf<T>() % Unsafe.SizeOf<nint>() is 0)
                    return sum;
            }

            while (Unsafe.IsAddressLessThan(
                ref f,
                ref Unsafe.SubtractByteOffset(ref l, (nint)Unsafe.SizeOf<ulong>() - 1)
            ))
            {
                sum += BitOperations.PopCount(Unsafe.As<T, ulong>(ref f));
                f = ref Unsafe.As<ulong, T>(ref Unsafe.Add(ref Unsafe.As<T, ulong>(ref f), 1));
            }

            if (Unsafe.SizeOf<T>() % Unsafe.SizeOf<ulong>() is 0)
                return sum;

            while (Unsafe.IsAddressLessThan(
                ref f,
                ref Unsafe.SubtractByteOffset(ref l, (nint)Unsafe.SizeOf<uint>() - 1)
            ))
            {
                sum += BitOperations.PopCount(Unsafe.As<T, uint>(ref f));
                f = ref Unsafe.As<uint, T>(ref Unsafe.Add(ref Unsafe.As<T, uint>(ref f), 1));
            }

            if (Unsafe.SizeOf<T>() % sizeof(uint) is not 0)
                sum += BitOperations.PopCount(
                    (Unsafe.SizeOf<T>() % sizeof(uint)) switch
                    {
                        1 => Unsafe.As<T, byte>(ref f),
                        2 => Unsafe.As<T, ushort>(ref f),
                        3 => Unsafe.As<T, ushort>(ref f) | (uint)Unsafe.Add(ref Unsafe.As<T, byte>(ref f), 2) << 16,
                        _ => throw new InvalidOperationException("Unsafe.SizeOf<T>() is assumed to be within [1, 3]."),
                    }
                );

            return sum;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] // ReSharper disable once RedundantUnsafeContext
    static int TrailingZeroCount(nuint value)
#if NET7_0_OR_GREATER
        =>
            BitOperations.TrailingZeroCount(value);
#else
    {
        const int BitsInUInt = BitsInByte * sizeof(uint);

        for (var i = 0; i < (Unsafe.SizeOf<nint>() + sizeof(uint) - 1) / sizeof(uint); i++)
            if (Map((uint)(value << i * BitsInUInt)) is var j and not 32)
                return j + i * BitsInUInt;

        return Unsafe.SizeOf<nint>() * BitsInByte;
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
}
#endif
