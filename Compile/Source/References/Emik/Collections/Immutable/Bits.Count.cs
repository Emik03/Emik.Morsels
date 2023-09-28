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
        var ptr = (nuint*)value;
        var sum = 0;

        if (sizeof(T) / (nuint.Size * 16) > 0)
        {
            for (; ptr <= (nuint*)(&value + 1) - 16; ptr += 16)
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
            for (; ptr <= (nuint*)(&value + 1) - 8; ptr += 8)
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
            for (; ptr <= (nuint*)(&value + 1) - 4; ptr += 4)
                sum += BitOperations.PopCount(*ptr) +
                    BitOperations.PopCount(ptr[1]) +
                    BitOperations.PopCount(ptr[2]) +
                    BitOperations.PopCount(ptr[3]);

            if (sizeof(T) % nuint.Size * 4 is 0)
                return sum;
        }

        if (sizeof(T) % (nuint.Size * 4) / (nuint.Size * 2) > 0)
        {
            for (; ptr <= (nuint*)(&value + 1) - 2; ptr += 2)
                sum += BitOperations.PopCount(*ptr) + BitOperations.PopCount(ptr[1]);

            if (sizeof(T) % nuint.Size * 2 is 0)
                return sum;
        }

        if (sizeof(T) % (nuint.Size * 2) / nuint.Size > 0)
        {
            for (; ptr <= (nuint*)(&value + 1); ptr++)
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
}
