// SPDX-License-Identifier: MPL-2.0

// ReSharper disable CheckNamespace StructCanBeMadeReadOnly
namespace Emik.Morsels;
#pragma warning disable CA1502
/// <inheritdoc cref="Bits{T}"/>
#if CSHARPREPL
public
#endif
#if !NO_READONLY_STRUCTS
readonly
#endif
    partial struct Bits<T>
{
    /// <summary>Computes the Bitwise-AND computation, writing it to the second argument.</summary>
    /// <param name="read">The <typeparamref name="T"/> to read from.</param>
    /// <param name="write">The <typeparamref name="T"/> to write to.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void And(in T read, ref T write)
    {
        fixed (T* r = &read)
        fixed (T* w = &write)
            And(r, w);
    }

    /// <summary>Computes the Bitwise-AND-NOT computation, writing it to the second argument.</summary>
    /// <param name="read">The <typeparamref name="T"/> to read from.</param>
    /// <param name="write">The <typeparamref name="T"/> to write to.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void AndNot(in T read, ref T write)
    {
        fixed (T* r = &read)
        fixed (T* w = &write)
            AndNot(r, w);
    }

    /// <summary>Computes the Bitwise-NOT computation, writing it to the first argument.</summary>
    /// <param name="reference">The <typeparamref name="T"/> to read and write from.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void Not(ref T reference)
    {
        fixed (T* ptr = &reference)
            Not(ptr);
    }

    /// <summary>Computes the Bitwise-OR computation, writing it to the second argument.</summary>
    /// <param name="read">The <typeparamref name="T"/> to read from.</param>
    /// <param name="write">The <typeparamref name="T"/> to write to.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void Or(in T read, ref T write)
    {
        fixed (T* r = &read)
        fixed (T* w = &write)
            Or(r, w);
    }

    /// <summary>Computes the Bitwise-XOR computation, writing it to the second argument.</summary>
    /// <param name="read">The <typeparamref name="T"/> to read from.</param>
    /// <param name="write">The <typeparamref name="T"/> to write to.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void Xor(in T read, ref T write)
    {
        fixed (T* r = &read)
        fixed (T* w = &write)
            Xor(r, w);
    }

    /// <summary>Determines whether both references of <typeparamref name="T"/> contain the same bits.</summary>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>
    /// The value <see langword="true"/> if the parameters <paramref name="left"/> and <paramref name="right"/>
    /// point to values with the same bits as each other; otherwise, <see langword="false"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static unsafe bool Eq(in T left, in T right)
    {
        fixed (T* l = &left)
        fixed (T* r = &right)
            return Eq(l, r);
    }

    /// <summary>Determines whether both references of <typeparamref name="T"/> contain the same bits.</summary>
    /// <param name="reference">The reference to determine if it is zeroed.</param>
    /// <returns>
    /// The value <see langword="true"/> if the parameter <paramref name="reference"/>
    /// points to a value with all zeros; otherwise, <see langword="false"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static unsafe bool EqZero(in T reference)
    {
        fixed (T* ptr = &reference)
            return EqZero(ptr);
    }

    /// <summary>Computes the Bitwise-AND computation, writing it to the second argument.</summary>
    /// <remarks><para>This method assumes the pointers are fixed.</para></remarks>
    /// <param name="read">The <typeparamref name="T"/> to read from.</param>
    /// <param name="write">The <typeparamref name="T"/> to write to.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)] // ReSharper disable once CognitiveComplexity
    public static unsafe void And(T* read, T* write)
    {
        byte* l = (byte*)read, r = (byte*)write, upper = (byte*)(read + 1);
#if NET8_0_OR_GREATER
        if (Vector512.IsHardwareAccelerated && sizeof(T) >= 64)
        {
            for (; l <= upper - 64; l += 64, r += 64)
                Vector512.BitwiseAnd(Vector512.Load(l), Vector512.Load(r)).StoreAligned(r);

            if (sizeof(T) % 64 is 0)
                return;
        }
#endif
#if NETCOREAPP3_0_OR_GREATER
        if (Vector256.IsHardwareAccelerated && sizeof(T) >= 32)
        {
            for (; l <= upper - 32; l += 32, r += 32)
                Vector256.BitwiseAnd(Vector256.Load(l), Vector256.Load(r)).StoreAligned(r);

            if (sizeof(T) % 32 is 0)
                return;
        }

        if (Vector128.IsHardwareAccelerated && sizeof(T) >= 16)
        {
            for (; l <= upper - 16; l += 16, r += 16)
                Vector128.BitwiseAnd(Vector128.Load(l), Vector128.Load(r)).StoreAligned(r);

            if (sizeof(T) % 16 is 0)
                return;
        }

        if (Vector64.IsHardwareAccelerated && sizeof(T) >= 8)
        {
            for (; l <= upper - 8; l += 8, r += 8)
                Vector64.BitwiseAnd(Vector64.Load(l), Vector64.Load(r)).StoreAligned(r);

            if (sizeof(T) % 8 is 0)
                return;
        }
#endif
        for (; l <= upper - sizeof(nuint); l += sizeof(nuint), r += sizeof(nuint))
            *(nuint*)r = *(nuint*)l & *(nuint*)r;

        if (sizeof(T) % sizeof(nuint) is 0)
            return;

        for (; l < upper; l++, r++)
            *r = (byte)(*l & *r);
    }

    /// <summary>Computes the Bitwise-AND-NOT computation, writing it to the second argument.</summary>
    /// <remarks><para>This method assumes the pointers are fixed.</para></remarks>
    /// <param name="read">The <typeparamref name="T"/> to read from.</param>
    /// <param name="write">The <typeparamref name="T"/> to write to.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)] // ReSharper disable once CognitiveComplexity
    public static unsafe void AndNot(T* read, T* write)
    {
        byte* l = (byte*)read, r = (byte*)write, upper = (byte*)(read + 1);
#if NET8_0_OR_GREATER
        if (Vector512.IsHardwareAccelerated && sizeof(T) >= 64)
        {
            for (; l <= upper - 64; l += 64, r += 64)
                Vector512.AndNot(Vector512.Load(l), Vector512.Load(r)).StoreAligned(r);

            if (sizeof(T) % 64 is 0)
                return;
        }
#endif
#if NETCOREAPP3_0_OR_GREATER
        if (Vector256.IsHardwareAccelerated && sizeof(T) >= 32)
        {
            for (; l <= upper - 32; l += 32, r += 32)
                Vector256.AndNot(Vector256.Load(l), Vector256.Load(r)).StoreAligned(r);

            if (sizeof(T) % 32 is 0)
                return;
        }

        if (Vector128.IsHardwareAccelerated && sizeof(T) >= 16)
        {
            for (; l <= upper - 16; l += 16, r += 16)
                Vector128.AndNot(Vector128.Load(l), Vector128.Load(r)).StoreAligned(r);

            if (sizeof(T) % 16 is 0)
                return;
        }

        if (Vector64.IsHardwareAccelerated && sizeof(T) >= 8)
        {
            for (; l <= upper - 8; l += 8, r += 8)
                Vector64.AndNot(Vector64.Load(l), Vector64.Load(r)).StoreAligned(r);

            if (sizeof(T) % 8 is 0)
                return;
        }
#endif
        for (; l <= upper - sizeof(nuint); l += sizeof(nuint), r += sizeof(nuint))
            *(nuint*)r = *(nuint*)l & ~*(nuint*)r;

        if (sizeof(T) % sizeof(nuint) is 0)
            return;

        for (; l < upper; l++, r++)
            *r = (byte)(*l & ~*r);
    }

    /// <summary>Computes the Bitwise-NOT computation, writing it to the second argument.</summary>
    /// <remarks><para>This method assumes the pointers are fixed.</para></remarks>
    /// <param name="ptr">The <typeparamref name="T"/> to read and write from.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)] // ReSharper disable once CognitiveComplexity
    public static unsafe void Not(T* ptr)
    {
        byte* x = (byte*)ptr, upper = (byte*)(ptr + 1);
#if NET8_0_OR_GREATER
        if (Vector512.IsHardwareAccelerated && sizeof(T) >= 64)
        {
            for (; x <= upper - 64; x += 64)
                Vector512.OnesComplement(Vector512.Load(x)).StoreAligned(x);

            if (sizeof(T) % 64 is 0)
                return;
        }
#endif
#if NETCOREAPP3_0_OR_GREATER
        if (Vector256.IsHardwareAccelerated && sizeof(T) >= 32)
        {
            for (; x <= upper - 32; x += 32)
                Vector256.OnesComplement(Vector256.Load(x)).StoreAligned(x);

            if (sizeof(T) % 32 is 0)
                return;
        }

        if (Vector128.IsHardwareAccelerated && sizeof(T) >= 16)
        {
            for (; x <= upper - 16; x += 16)
                Vector128.OnesComplement(Vector128.Load(x)).StoreAligned(x);

            if (sizeof(T) % 16 is 0)
                return;
        }

        if (Vector64.IsHardwareAccelerated && sizeof(T) >= 8)
        {
            for (; x <= upper - 8; x += 8)
                Vector64.OnesComplement(Vector64.Load(x)).StoreAligned(x);

            if (sizeof(T) % 8 is 0)
                return;
        }
#endif
        for (; x <= upper - sizeof(nuint); x += sizeof(nuint))
            *(nuint*)x = ~*(nuint*)x;

        if (sizeof(T) % sizeof(nuint) is 0)
            return;

        for (; x < upper; x++)
            *x = (byte)~*x;
    }

    /// <summary>Computes the Bitwise-OR computation, writing it to the second argument.</summary>
    /// <remarks><para>This method assumes the pointers are fixed.</para></remarks>
    /// <param name="read">The <typeparamref name="T"/> to read from.</param>
    /// <param name="write">The <typeparamref name="T"/> to write to.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)] // ReSharper disable once CognitiveComplexity
    public static unsafe void Or(T* read, T* write)
    {
        byte* l = (byte*)read, r = (byte*)write, upper = (byte*)(read + 1);
#if NET8_0_OR_GREATER
        if (Vector512.IsHardwareAccelerated && sizeof(T) >= 64)
        {
            for (; l <= upper - 64; l += 64, r += 64)
                Vector512.BitwiseOr(Vector512.Load(l), Vector512.Load(r)).StoreAligned(r);

            if (sizeof(T) % 64 is 0)
                return;
        }
#endif
#if NETCOREAPP3_0_OR_GREATER
        if (Vector256.IsHardwareAccelerated && sizeof(T) >= 32)
        {
            for (; l <= upper - 32; l += 32, r += 32)
                Vector256.BitwiseOr(Vector256.Load(l), Vector256.Load(r)).StoreAligned(r);

            if (sizeof(T) % 32 is 0)
                return;
        }

        if (Vector128.IsHardwareAccelerated && sizeof(T) >= 16)
        {
            for (; l <= upper - 16; l += 16, r += 16)
                Vector128.BitwiseOr(Vector128.Load(l), Vector128.Load(r)).StoreAligned(r);

            if (sizeof(T) % 16 is 0)
                return;
        }

        if (Vector64.IsHardwareAccelerated && sizeof(T) >= 8)
        {
            for (; l <= upper - 8; l += 8, r += 8)
                Vector64.BitwiseOr(Vector64.Load(l), Vector64.Load(r)).StoreAligned(r);

            if (sizeof(T) % 8 is 0)
                return;
        }
#endif
        for (; l <= upper - sizeof(nuint); l += sizeof(nuint), r += sizeof(nuint))
            *(nuint*)r = *(nuint*)l | *(nuint*)r;

        if (sizeof(T) % sizeof(nuint) is 0)
            return;

        for (; l < upper; l++, r++)
            *r = (byte)(*l | *r);
    }

    /// <summary>Computes the Bitwise-XOR computation, writing it to the second argument.</summary>
    /// <remarks><para>This method assumes the pointers are fixed.</para></remarks>
    /// <param name="read">The <typeparamref name="T"/> to read from.</param>
    /// <param name="write">The <typeparamref name="T"/> to write to.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)] // ReSharper disable once CognitiveComplexity
    public static unsafe void Xor(T* read, T* write)
    {
        byte* l = (byte*)read, r = (byte*)write, upper = (byte*)(read + 1);
#if NET8_0_OR_GREATER
        if (Vector512.IsHardwareAccelerated && sizeof(T) >= 64)
        {
            for (; l <= upper - 64; l += 64, r += 64)
                Vector512.Xor(Vector512.Load(l), Vector512.Load(r)).StoreAligned(r);

            if (sizeof(T) % 64 is 0)
                return;
        }
#endif
#if NETCOREAPP3_0_OR_GREATER
        if (Vector256.IsHardwareAccelerated && sizeof(T) >= 32)
        {
            for (; l <= upper - 32; l += 32, r += 32)
                Vector256.Xor(Vector256.Load(l), Vector256.Load(r)).StoreAligned(r);

            if (sizeof(T) % 32 is 0)
                return;
        }

        if (Vector128.IsHardwareAccelerated && sizeof(T) >= 16)
        {
            for (; l <= upper - 16; l += 16, r += 16)
                Vector128.Xor(Vector128.Load(l), Vector128.Load(r)).StoreAligned(r);

            if (sizeof(T) % 16 is 0)
                return;
        }

        if (Vector64.IsHardwareAccelerated && sizeof(T) >= 8)
        {
            for (; l <= upper - 8; l += 8, r += 8)
                Vector64.Xor(Vector64.Load(l), Vector64.Load(r)).StoreAligned(r);

            if (sizeof(T) % 8 is 0)
                return;
        }
#endif
        for (; l <= upper - sizeof(nuint); l += sizeof(nuint), r += sizeof(nuint))
            *(nuint*)r = *(nuint*)l ^ *(nuint*)r;

        if (sizeof(T) % sizeof(nuint) is 0)
            return;

        for (; l < upper; l++, r++)
            *r = (byte)(*l ^ *r);
    }

    /// <summary>Determines whether both pointers of <typeparamref name="T"/> contain the same bits.</summary>
    /// <remarks><para>This method assumes the pointers are fixed.</para></remarks>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>
    /// The value <see langword="true"/> if the parameters <paramref name="left"/> and <paramref name="right"/>
    /// point to values with the same bits as each other; otherwise, <see langword="false"/>.
    /// </returns>
    [CLSCompliant(false), MethodImpl(MethodImplOptions.AggressiveInlining), Pure] // ReSharper disable once CognitiveComplexity
    public static unsafe bool Eq(T* left, T* right)
    {
        byte* l = (byte*)left, r = (byte*)right, upper = (byte*)(left + 1);
#if NET8_0_OR_GREATER
        if (Vector512.IsHardwareAccelerated && sizeof(T) >= 64)
        {
            for (; l <= upper - 64; l += 64, r += 64)
                if (!Vector512.EqualsAll(Vector512.Load(l), Vector512.Load(r)))
                    return false;

            if (sizeof(T) % 64 is 0)
                return true;
        }
#endif
#if NETCOREAPP3_0_OR_GREATER
        if (Vector256.IsHardwareAccelerated && sizeof(T) >= 32)
        {
            for (; l <= upper - 32; l += 32, r += 32)
                if (!Vector256.EqualsAll(Vector256.Load(l), Vector256.Load(r)))
                    return false;

            if (sizeof(T) % 32 is 0)
                return true;
        }

        if (Vector128.IsHardwareAccelerated && sizeof(T) >= 16)
        {
            for (; l <= upper - 16; l += 16, r += 16)
                if (!Vector128.EqualsAll(Vector128.Load(l), Vector128.Load(r)))
                    return false;

            if (sizeof(T) % 16 is 0)
                return true;
        }

        if (Vector64.IsHardwareAccelerated && sizeof(T) >= 8)
        {
            for (; l <= upper - 8; l += 8, r += 8)
                if (!Vector64.EqualsAll(Vector64.Load(l), Vector64.Load(r)))
                    return false;

            if (sizeof(T) % 8 is 0)
                return true;
        }
#endif
        for (; l <= upper - sizeof(nuint); l += sizeof(nuint), r += sizeof(nuint))
            if (*(nuint*)l != *(nuint*)r)
                return false;

        if (sizeof(T) % sizeof(nuint) is 0)
            return true;

        for (; l < upper; l++, r++)
            if (*l != *r)
                return false;

        return true;
    }

    /// <summary>Determines whether both pointers of <typeparamref name="T"/> contain the same bits.</summary>
    /// <remarks><para>This method assumes the pointers are fixed.</para></remarks>
    /// <param name="ptr">The pointer to determine if it is zeroed.</param>
    /// <returns>
    /// The value <see langword="true"/> if the parameter <paramref name="ptr"/>
    /// points to a value with all zeros; otherwise, <see langword="false"/>.
    /// </returns>
    [CLSCompliant(false), MethodImpl(MethodImplOptions.AggressiveInlining), Pure] // ReSharper disable once CognitiveComplexity
    public static unsafe bool EqZero(T* ptr)
    {
        byte* x = (byte*)ptr, upper = (byte*)(ptr + 1);
#if NET8_0_OR_GREATER
        if (Vector512.IsHardwareAccelerated && sizeof(T) >= 64)
        {
            for (; x <= upper - 64; x += 64)
                if (!Vector512.EqualsAll(Vector512.Load(x), Vector512<byte>.Zero))
                    return false;

            if (sizeof(T) % 64 is 0)
                return true;
        }
#endif
#if NETCOREAPP3_0_OR_GREATER
        if (Vector256.IsHardwareAccelerated && sizeof(T) >= 32)
        {
            for (; x <= upper - 32; x += 32)
                if (!Vector256.EqualsAll(Vector256.Load(x), Vector256<byte>.Zero))
                    return false;

            if (sizeof(T) % 32 is 0)
                return true;
        }

        if (Vector128.IsHardwareAccelerated && sizeof(T) >= 16)
        {
            for (; x <= upper - 16; x += 16)
                if (!Vector128.EqualsAll(Vector128.Load(x), Vector128<byte>.Zero))
                    return false;

            if (sizeof(T) % 16 is 0)
                return true;
        }

        if (Vector64.IsHardwareAccelerated && sizeof(T) >= 8)
        {
            for (; x <= upper - 8; x += 8)
                if (!Vector64.EqualsAll(Vector64.Load(x), Vector64<byte>.Zero))
                    return false;

            if (sizeof(T) % 8 is 0)
                return true;
        }
#endif
        for (; x <= upper - sizeof(nuint); x += sizeof(nuint))
            if (*(nuint*)x is not 0)
                return false;

        if (sizeof(T) % sizeof(nuint) is 0)
            return true;

        for (; x < upper; x++)
            if (*x is not 0)
                return false;

        return true;
    }
}
