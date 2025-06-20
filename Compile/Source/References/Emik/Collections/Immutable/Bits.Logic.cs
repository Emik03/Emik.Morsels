// SPDX-License-Identifier: MPL-2.0
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
#pragma warning disable CS8500, IDE0004, MA0051
// ReSharper disable BadPreprocessorIndent CheckNamespace CognitiveComplexity RedundantCast StructCanBeMadeReadOnly
namespace Emik.Morsels;

using static Span;

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
    public static void And(scoped in T read, scoped ref T write)
    {
        ref byte l = ref Unsafe.As<T, byte>(ref AsRef(read)),
            r = ref Unsafe.As<T, byte>(ref AsRef(write)),
            upper = ref Unsafe.Add(ref l, Unsafe.SizeOf<T>());
#if NET8_0_OR_GREATER
        if (Vector512.IsHardwareAccelerated && Unsafe.SizeOf<T>() >= 64)
        {
            while (Unsafe.IsAddressLessThan(ref l, ref Unsafe.SubtractByteOffset(ref upper, 63)))
            {
                Vector512.BitwiseAnd(Vector512.LoadUnsafe(ref l), Vector512.LoadUnsafe(ref r)).StoreUnsafe(ref r);
                l = ref Unsafe.Add(ref l, 64);
                r = ref Unsafe.Add(ref r, 64);
            }

            if (Unsafe.SizeOf<T>() % 64 is 0)
                return;
        }
#endif
#if NET7_0_OR_GREATER
        if (Vector256.IsHardwareAccelerated && Unsafe.SizeOf<T>() >= 32)
        {
            while (Unsafe.IsAddressLessThan(ref l, ref Unsafe.SubtractByteOffset(ref upper, 31)))
            {
                Vector256.BitwiseAnd(Vector256.LoadUnsafe(ref l), Vector256.LoadUnsafe(ref r)).StoreUnsafe(ref r);
                l = ref Unsafe.Add(ref l, 32);
                r = ref Unsafe.Add(ref r, 32);
            }

            if (Unsafe.SizeOf<T>() % 32 is 0)
                return;
        }

        if (Vector128.IsHardwareAccelerated && Unsafe.SizeOf<T>() >= 16)
        {
            while (Unsafe.IsAddressLessThan(ref l, ref Unsafe.SubtractByteOffset(ref upper, 15)))
            {
                Vector128.BitwiseAnd(Vector128.LoadUnsafe(ref l), Vector128.LoadUnsafe(ref r)).StoreUnsafe(ref r);
                l = ref Unsafe.Add(ref l, 16);
                r = ref Unsafe.Add(ref r, 16);
            }

            if (Unsafe.SizeOf<T>() % 16 is 0)
                return;
        }

        if (Vector64.IsHardwareAccelerated && Unsafe.SizeOf<T>() >= 8)
        {
            while (Unsafe.IsAddressLessThan(ref l, ref Unsafe.SubtractByteOffset(ref upper, 7)))
            {
                Vector64.BitwiseAnd(Vector64.LoadUnsafe(ref l), Vector64.LoadUnsafe(ref r)).StoreUnsafe(ref r);
                l = ref Unsafe.Add(ref l, 8);
                r = ref Unsafe.Add(ref r, 8);
            }

            if (Unsafe.SizeOf<T>() % 8 is 0)
                return;
        }
#endif
        while (Unsafe.IsAddressLessThan(
            ref l,
            ref Unsafe.SubtractByteOffset(ref upper, (nint)Unsafe.SizeOf<nuint>() - 1)
        ))
        {
            Unsafe.As<byte, nuint>(ref r) &= Unsafe.As<byte, nuint>(ref l);
            l = ref Unsafe.Add(ref l, (nint)Unsafe.SizeOf<nuint>());
            r = ref Unsafe.Add(ref r, (nint)Unsafe.SizeOf<nuint>());
        }

        while (Unsafe.IsAddressLessThan(ref l, ref Unsafe.SubtractByteOffset(ref upper, sizeof(ulong) - 1)))
        {
            Unsafe.As<byte, ulong>(ref r) &= Unsafe.As<byte, ulong>(ref l);
            l = ref Unsafe.Add(ref l, sizeof(ulong));
            r = ref Unsafe.Add(ref r, sizeof(ulong));
        }

        while (Unsafe.IsAddressLessThan(ref l, ref Unsafe.SubtractByteOffset(ref upper, sizeof(uint) - 1)))
        {
            Unsafe.As<byte, uint>(ref r) &= Unsafe.As<byte, uint>(ref l);
            l = ref Unsafe.Add(ref l, sizeof(uint));
            r = ref Unsafe.Add(ref r, sizeof(uint));
        }

        while (Unsafe.IsAddressLessThan(ref l, ref Unsafe.SubtractByteOffset(ref upper, sizeof(ushort) - 1)))
        {
            Unsafe.As<byte, ushort>(ref r) &= Unsafe.As<byte, ushort>(ref l);
            l = ref Unsafe.Add(ref l, sizeof(ushort));
            r = ref Unsafe.Add(ref r, sizeof(ushort));
        }

        while (Unsafe.IsAddressLessThan(ref l, ref upper))
        {
            Unsafe.As<byte, byte>(ref r) &= Unsafe.As<byte, byte>(ref l);
            l = ref Unsafe.Add(ref l, 1);
            r = ref Unsafe.Add(ref r, 1);
        }
    }

    /// <summary>Computes the Bitwise-AND-NOT computation, writing it to the second argument.</summary>
    /// <param name="read">The <typeparamref name="T"/> to read from.</param>
    /// <param name="write">The <typeparamref name="T"/> to write to.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AndNot(scoped in T read, scoped ref T write)
    {
        ref byte l = ref Unsafe.As<T, byte>(ref AsRef(read)),
            r = ref Unsafe.As<T, byte>(ref AsRef(write)),
            upper = ref Unsafe.Add(ref l, Unsafe.SizeOf<T>());
#if NET8_0_OR_GREATER
        if (Vector512.IsHardwareAccelerated && Unsafe.SizeOf<T>() >= 64)
        {
            while (Unsafe.IsAddressLessThan(ref l, ref Unsafe.SubtractByteOffset(ref upper, 63)))
            {
                Vector512.AndNot(Vector512.LoadUnsafe(ref l), Vector512.LoadUnsafe(ref r)).StoreUnsafe(ref r);
                l = ref Unsafe.Add(ref l, 64);
                r = ref Unsafe.Add(ref r, 64);
            }

            if (Unsafe.SizeOf<T>() % 64 is 0)
                return;
        }
#endif
#if NET7_0_OR_GREATER
        if (Vector256.IsHardwareAccelerated && Unsafe.SizeOf<T>() >= 32)
        {
            while (Unsafe.IsAddressLessThan(ref l, ref Unsafe.SubtractByteOffset(ref upper, 31)))
            {
                Vector256.AndNot(Vector256.LoadUnsafe(ref l), Vector256.LoadUnsafe(ref r)).StoreUnsafe(ref r);
                l = ref Unsafe.Add(ref l, 32);
                r = ref Unsafe.Add(ref r, 32);
            }

            if (Unsafe.SizeOf<T>() % 32 is 0)
                return;
        }

        if (Vector128.IsHardwareAccelerated && Unsafe.SizeOf<T>() >= 16)
        {
            while (Unsafe.IsAddressLessThan(ref l, ref Unsafe.SubtractByteOffset(ref upper, 15)))
            {
                Vector128.AndNot(Vector128.LoadUnsafe(ref l), Vector128.LoadUnsafe(ref r)).StoreUnsafe(ref r);
                l = ref Unsafe.Add(ref l, 16);
                r = ref Unsafe.Add(ref r, 16);
            }

            if (Unsafe.SizeOf<T>() % 16 is 0)
                return;
        }

        if (Vector64.IsHardwareAccelerated && Unsafe.SizeOf<T>() >= 8)
        {
            while (Unsafe.IsAddressLessThan(ref l, ref Unsafe.SubtractByteOffset(ref upper, 7)))
            {
                Vector64.AndNot(Vector64.LoadUnsafe(ref l), Vector64.LoadUnsafe(ref r)).StoreUnsafe(ref r);
                l = ref Unsafe.Add(ref l, 8);
                r = ref Unsafe.Add(ref r, 8);
            }

            if (Unsafe.SizeOf<T>() % 8 is 0)
                return;
        }
#endif
        while (Unsafe.IsAddressLessThan(
            ref l,
            ref Unsafe.SubtractByteOffset(ref upper, (nint)Unsafe.SizeOf<nuint>() - 1)
        ))
        {
            Unsafe.As<byte, nuint>(ref r) &= ~Unsafe.As<byte, nuint>(ref l);
            l = ref Unsafe.Add(ref l, (nint)Unsafe.SizeOf<nuint>());
            r = ref Unsafe.Add(ref r, (nint)Unsafe.SizeOf<nuint>());
        }

        while (Unsafe.IsAddressLessThan(ref l, ref Unsafe.SubtractByteOffset(ref upper, sizeof(ulong) - 1)))
        {
            Unsafe.As<byte, ulong>(ref r) &= ~Unsafe.As<byte, ulong>(ref l);
            l = ref Unsafe.Add(ref l, sizeof(ulong));
            r = ref Unsafe.Add(ref r, sizeof(ulong));
        }

        while (Unsafe.IsAddressLessThan(ref l, ref Unsafe.SubtractByteOffset(ref upper, sizeof(uint) - 1)))
        {
            Unsafe.As<byte, uint>(ref r) &= ~Unsafe.As<byte, uint>(ref l);
            l = ref Unsafe.Add(ref l, sizeof(uint));
            r = ref Unsafe.Add(ref r, sizeof(uint));
        }

        while (Unsafe.IsAddressLessThan(ref l, ref Unsafe.SubtractByteOffset(ref upper, sizeof(ushort) - 1)))
        {
            Unsafe.As<byte, ushort>(ref r) &= (ushort)~Unsafe.As<byte, ushort>(ref l);
            l = ref Unsafe.Add(ref l, sizeof(ushort));
            r = ref Unsafe.Add(ref r, sizeof(ushort));
        }

        while (Unsafe.IsAddressLessThan(ref l, ref upper))
        {
            Unsafe.As<byte, byte>(ref r) &= (byte)~Unsafe.As<byte, byte>(ref l);
            l = ref Unsafe.Add(ref l, 1);
            r = ref Unsafe.Add(ref r, 1);
        }
    }

    /// <summary>Computes the Bitwise-NOT computation, writing it to the first argument.</summary>
    /// <param name="reference">The <typeparamref name="T"/> to read and write from.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Not(scoped ref T reference)
    {
        ref byte x = ref Unsafe.As<T, byte>(ref AsRef(reference)), upper = ref Unsafe.Add(ref x, Unsafe.SizeOf<T>());
#if NET8_0_OR_GREATER
        if (Vector512.IsHardwareAccelerated && Unsafe.SizeOf<T>() >= 64)
        {
            while (Unsafe.IsAddressLessThan(ref x, ref Unsafe.SubtractByteOffset(ref upper, 63)))
            {
                Vector512.OnesComplement(Vector512.LoadUnsafe(ref x)).StoreUnsafe(ref x);
                x = ref Unsafe.Add(ref x, 64);
            }

            if (Unsafe.SizeOf<T>() % 64 is 0)
                return;
        }
#endif
#if NET7_0_OR_GREATER
        if (Vector256.IsHardwareAccelerated && Unsafe.SizeOf<T>() >= 32)
        {
            while (Unsafe.IsAddressLessThan(ref x, ref Unsafe.SubtractByteOffset(ref upper, 31)))
            {
                Vector256.OnesComplement(Vector256.LoadUnsafe(ref x)).StoreUnsafe(ref x);
                x = ref Unsafe.Add(ref x, 32);
            }

            if (Unsafe.SizeOf<T>() % 32 is 0)
                return;
        }

        if (Vector128.IsHardwareAccelerated && Unsafe.SizeOf<T>() >= 16)
        {
            while (Unsafe.IsAddressLessThan(ref x, ref Unsafe.SubtractByteOffset(ref upper, 15)))
            {
                Vector128.OnesComplement(Vector128.LoadUnsafe(ref x)).StoreUnsafe(ref x);
                x = ref Unsafe.Add(ref x, 16);
            }

            if (Unsafe.SizeOf<T>() % 16 is 0)
                return;
        }

        if (Vector64.IsHardwareAccelerated && Unsafe.SizeOf<T>() >= 8)
        {
            while (Unsafe.IsAddressLessThan(ref x, ref Unsafe.SubtractByteOffset(ref upper, 7)))
            {
                Vector64.OnesComplement(Vector64.LoadUnsafe(ref x)).StoreUnsafe(ref x);
                x = ref Unsafe.Add(ref x, 8);
            }

            if (Unsafe.SizeOf<T>() % 8 is 0)
                return;
        }
#endif
        while (Unsafe.IsAddressLessThan(
            ref x,
            ref Unsafe.SubtractByteOffset(ref upper, (nint)Unsafe.SizeOf<nuint>() - 1)
        ))
        {
            Unsafe.As<byte, nuint>(ref x) = ~Unsafe.As<byte, nuint>(ref x);
            x = ref Unsafe.Add(ref x, Unsafe.SizeOf<nuint>());
        }

        while (Unsafe.IsAddressLessThan(ref x, ref Unsafe.SubtractByteOffset(ref upper, sizeof(ulong) - 1)))
        {
            Unsafe.As<byte, ulong>(ref x) = ~Unsafe.As<byte, ulong>(ref x);
            x = ref Unsafe.Add(ref x, sizeof(ulong));
        }

        while (Unsafe.IsAddressLessThan(ref x, ref Unsafe.SubtractByteOffset(ref upper, sizeof(uint) - 1)))
        {
            Unsafe.As<byte, uint>(ref x) = ~Unsafe.As<byte, uint>(ref x);
            x = ref Unsafe.Add(ref x, sizeof(uint));
        }

        while (Unsafe.IsAddressLessThan(ref x, ref Unsafe.SubtractByteOffset(ref upper, sizeof(ushort) - 1)))
        {
            Unsafe.As<byte, ushort>(ref x) = (ushort)~Unsafe.As<byte, ushort>(ref x);
            x = ref Unsafe.Add(ref x, sizeof(ushort));
        }

        while (Unsafe.IsAddressLessThan(ref x, ref upper))
        {
            Unsafe.As<byte, byte>(ref x) &= (byte)~Unsafe.As<byte, byte>(ref x);
            x = ref Unsafe.Add(ref x, 1);
        }
    }

    /// <summary>Computes the Bitwise-OR computation, writing it to the second argument.</summary>
    /// <param name="read">The <typeparamref name="T"/> to read from.</param>
    /// <param name="write">The <typeparamref name="T"/> to write to.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Or(scoped in T read, scoped ref T write)
    {
        ref byte l = ref Unsafe.As<T, byte>(ref AsRef(read)),
            r = ref Unsafe.As<T, byte>(ref AsRef(write)),
            upper = ref Unsafe.Add(ref l, Unsafe.SizeOf<T>());
#if NET8_0_OR_GREATER
        if (Vector512.IsHardwareAccelerated && Unsafe.SizeOf<T>() >= 64)
        {
            while (Unsafe.IsAddressLessThan(ref l, ref Unsafe.SubtractByteOffset(ref upper, 63)))
            {
                Vector512.BitwiseOr(Vector512.LoadUnsafe(ref l), Vector512.LoadUnsafe(ref r)).StoreUnsafe(ref r);
                l = ref Unsafe.Add(ref l, 64);
                r = ref Unsafe.Add(ref r, 64);
            }

            if (Unsafe.SizeOf<T>() % 64 is 0)
                return;
        }
#endif
#if NET7_0_OR_GREATER
        if (Vector256.IsHardwareAccelerated && Unsafe.SizeOf<T>() >= 32)
        {
            while (Unsafe.IsAddressLessThan(ref l, ref Unsafe.SubtractByteOffset(ref upper, 31)))
            {
                Vector256.BitwiseOr(Vector256.LoadUnsafe(ref l), Vector256.LoadUnsafe(ref r)).StoreUnsafe(ref r);
                l = ref Unsafe.Add(ref l, 32);
                r = ref Unsafe.Add(ref r, 32);
            }

            if (Unsafe.SizeOf<T>() % 32 is 0)
                return;
        }

        if (Vector128.IsHardwareAccelerated && Unsafe.SizeOf<T>() >= 16)
        {
            while (Unsafe.IsAddressLessThan(ref l, ref Unsafe.SubtractByteOffset(ref upper, 15)))
            {
                Vector128.BitwiseOr(Vector128.LoadUnsafe(ref l), Vector128.LoadUnsafe(ref r)).StoreUnsafe(ref r);
                l = ref Unsafe.Add(ref l, 16);
                r = ref Unsafe.Add(ref r, 16);
            }

            if (Unsafe.SizeOf<T>() % 16 is 0)
                return;
        }

        if (Vector64.IsHardwareAccelerated && Unsafe.SizeOf<T>() >= 8)
        {
            while (Unsafe.IsAddressLessThan(ref l, ref Unsafe.SubtractByteOffset(ref upper, 7)))
            {
                Vector64.BitwiseOr(Vector64.LoadUnsafe(ref l), Vector64.LoadUnsafe(ref r)).StoreUnsafe(ref r);
                l = ref Unsafe.Add(ref l, 8);
                r = ref Unsafe.Add(ref r, 8);
            }

            if (Unsafe.SizeOf<T>() % 8 is 0)
                return;
        }
#endif
        while (Unsafe.IsAddressLessThan(
            ref l,
            ref Unsafe.SubtractByteOffset(ref upper, (nint)Unsafe.SizeOf<nuint>() - 1)
        ))
        {
            Unsafe.As<byte, nuint>(ref r) |= Unsafe.As<byte, nuint>(ref l);
            l = ref Unsafe.Add(ref l, Unsafe.SizeOf<nuint>());
            r = ref Unsafe.Add(ref r, Unsafe.SizeOf<nuint>());
        }

        while (Unsafe.IsAddressLessThan(ref l, ref Unsafe.SubtractByteOffset(ref upper, sizeof(ulong) - 1)))
        {
            Unsafe.As<byte, ulong>(ref r) |= Unsafe.As<byte, ulong>(ref l);
            l = ref Unsafe.Add(ref l, sizeof(ulong));
            r = ref Unsafe.Add(ref r, sizeof(ulong));
        }

        while (Unsafe.IsAddressLessThan(ref l, ref Unsafe.SubtractByteOffset(ref upper, sizeof(uint) - 1)))
        {
            Unsafe.As<byte, uint>(ref r) |= Unsafe.As<byte, uint>(ref l);
            l = ref Unsafe.Add(ref l, sizeof(uint));
            r = ref Unsafe.Add(ref r, sizeof(uint));
        }

        while (Unsafe.IsAddressLessThan(ref l, ref Unsafe.SubtractByteOffset(ref upper, sizeof(ushort) - 1)))
        {
            Unsafe.As<byte, ushort>(ref r) |= Unsafe.As<byte, ushort>(ref l);
            l = ref Unsafe.Add(ref l, sizeof(ushort));
            r = ref Unsafe.Add(ref r, sizeof(ushort));
        }

        while (Unsafe.IsAddressLessThan(ref l, ref upper))
        {
            Unsafe.As<byte, byte>(ref r) |= Unsafe.As<byte, byte>(ref l);
            l = ref Unsafe.Add(ref l, 1);
            r = ref Unsafe.Add(ref r, 1);
        }
    }

    /// <summary>Computes the Bitwise-XOR computation, writing it to the second argument.</summary>
    /// <param name="read">The <typeparamref name="T"/> to read from.</param>
    /// <param name="write">The <typeparamref name="T"/> to write to.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Xor(scoped in T read, scoped ref T write)
    {
        ref byte l = ref Unsafe.As<T, byte>(ref AsRef(read)),
            r = ref Unsafe.As<T, byte>(ref AsRef(write)),
            upper = ref Unsafe.Add(ref l, Unsafe.SizeOf<T>());
#if NET8_0_OR_GREATER
        if (Vector512.IsHardwareAccelerated && Unsafe.SizeOf<T>() >= 64)
        {
            while (Unsafe.IsAddressLessThan(ref l, ref Unsafe.SubtractByteOffset(ref upper, 63)))
            {
                Vector512.Xor(Vector512.LoadUnsafe(ref l), Vector512.LoadUnsafe(ref r)).StoreUnsafe(ref r);
                l = ref Unsafe.Add(ref l, 64);
                r = ref Unsafe.Add(ref r, 64);
            }

            if (Unsafe.SizeOf<T>() % 64 is 0)
                return;
        }
#endif
#if NET7_0_OR_GREATER
        if (Vector256.IsHardwareAccelerated && Unsafe.SizeOf<T>() >= 32)
        {
            while (Unsafe.IsAddressLessThan(ref l, ref Unsafe.SubtractByteOffset(ref upper, 31)))
            {
                Vector256.Xor(Vector256.LoadUnsafe(ref l), Vector256.LoadUnsafe(ref r)).StoreUnsafe(ref r);
                l = ref Unsafe.Add(ref l, 32);
                r = ref Unsafe.Add(ref r, 32);
            }

            if (Unsafe.SizeOf<T>() % 32 is 0)
                return;
        }

        if (Vector128.IsHardwareAccelerated && Unsafe.SizeOf<T>() >= 16)
        {
            while (Unsafe.IsAddressLessThan(ref l, ref Unsafe.SubtractByteOffset(ref upper, 15)))
            {
                Vector128.Xor(Vector128.LoadUnsafe(ref l), Vector128.LoadUnsafe(ref r)).StoreUnsafe(ref r);
                l = ref Unsafe.Add(ref l, 16);
                r = ref Unsafe.Add(ref r, 16);
            }

            if (Unsafe.SizeOf<T>() % 16 is 0)
                return;
        }

        if (Vector64.IsHardwareAccelerated && Unsafe.SizeOf<T>() >= 8)
        {
            while (Unsafe.IsAddressLessThan(ref l, ref Unsafe.SubtractByteOffset(ref upper, 7)))
            {
                Vector64.Xor(Vector64.LoadUnsafe(ref l), Vector64.LoadUnsafe(ref r)).StoreUnsafe(ref r);
                l = ref Unsafe.Add(ref l, 8);
                r = ref Unsafe.Add(ref r, 8);
            }

            if (Unsafe.SizeOf<T>() % 8 is 0)
                return;
        }
#endif
        while (Unsafe.IsAddressLessThan(
            ref l,
            ref Unsafe.SubtractByteOffset(ref upper, (nint)Unsafe.SizeOf<nuint>() - 1)
        ))
        {
            Unsafe.As<byte, nuint>(ref r) ^= Unsafe.As<byte, nuint>(ref l);
            l = ref Unsafe.Add(ref l, Unsafe.SizeOf<nuint>());
            r = ref Unsafe.Add(ref r, Unsafe.SizeOf<nuint>());
        }

        while (Unsafe.IsAddressLessThan(ref l, ref Unsafe.SubtractByteOffset(ref upper, sizeof(ulong) - 1)))
        {
            Unsafe.As<byte, ulong>(ref r) ^= Unsafe.As<byte, ulong>(ref l);
            l = ref Unsafe.Add(ref l, sizeof(ulong));
            r = ref Unsafe.Add(ref r, sizeof(ulong));
        }

        while (Unsafe.IsAddressLessThan(ref l, ref Unsafe.SubtractByteOffset(ref upper, sizeof(uint) - 1)))
        {
            Unsafe.As<byte, uint>(ref r) ^= Unsafe.As<byte, uint>(ref l);
            l = ref Unsafe.Add(ref l, sizeof(uint));
            r = ref Unsafe.Add(ref r, sizeof(uint));
        }

        while (Unsafe.IsAddressLessThan(ref l, ref Unsafe.SubtractByteOffset(ref upper, sizeof(ushort) - 1)))
        {
            Unsafe.As<byte, ushort>(ref r) ^= Unsafe.As<byte, ushort>(ref l);
            l = ref Unsafe.Add(ref l, sizeof(ushort));
            r = ref Unsafe.Add(ref r, sizeof(ushort));
        }

        while (Unsafe.IsAddressLessThan(ref l, ref upper))
        {
            Unsafe.As<byte, byte>(ref r) ^= Unsafe.As<byte, byte>(ref l);
            l = ref Unsafe.Add(ref l, 1);
            r = ref Unsafe.Add(ref r, 1);
        }
    }

    /// <summary>Determines whether both references of <typeparamref name="T"/> contain the same bits.</summary>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>
    /// The value <see langword="true"/> if the parameters <paramref name="left"/> and <paramref name="right"/>
    /// point to values with the same bits as each other; otherwise, <see langword="false"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool Eq(scoped in T left, scoped in T right)
    {
        ref byte l = ref Unsafe.As<T, byte>(ref AsRef(left)),
            r = ref Unsafe.As<T, byte>(ref AsRef(right)),
            upper = ref Unsafe.Add(ref l, Unsafe.SizeOf<T>());
#if NET8_0_OR_GREATER
        if (Vector512.IsHardwareAccelerated && Unsafe.SizeOf<T>() >= 64)
        {
            while (Unsafe.IsAddressLessThan(ref l, ref Unsafe.SubtractByteOffset(ref upper, 63)))
            {
                if (!Vector512.EqualsAll(Vector512.LoadUnsafe(ref l), Vector512.LoadUnsafe(ref r)))
                    return false;

                l = ref Unsafe.Add(ref l, 64);
                r = ref Unsafe.Add(ref r, 64);
            }

            if (Unsafe.SizeOf<T>() % 64 is 0)
                return true;
        }
#endif
#if NET7_0_OR_GREATER
        if (Vector256.IsHardwareAccelerated && Unsafe.SizeOf<T>() >= 32)
        {
            while (Unsafe.IsAddressLessThan(ref l, ref Unsafe.SubtractByteOffset(ref upper, 31)))
            {
                if (!Vector256.EqualsAll(Vector256.LoadUnsafe(ref l), Vector256.LoadUnsafe(ref r)))
                    return false;

                l = ref Unsafe.Add(ref l, 32);
                r = ref Unsafe.Add(ref r, 32);
            }

            if (Unsafe.SizeOf<T>() % 32 is 0)
                return true;
        }

        if (Vector128.IsHardwareAccelerated && Unsafe.SizeOf<T>() >= 16)
        {
            while (Unsafe.IsAddressLessThan(ref l, ref Unsafe.SubtractByteOffset(ref upper, 15)))
            {
                if (!Vector128.EqualsAll(Vector128.LoadUnsafe(ref l), Vector128.LoadUnsafe(ref r)))
                    return false;

                l = ref Unsafe.Add(ref l, 16);
                r = ref Unsafe.Add(ref r, 16);
            }

            if (Unsafe.SizeOf<T>() % 16 is 0)
                return true;
        }

        if (Vector64.IsHardwareAccelerated && Unsafe.SizeOf<T>() >= 8)
        {
            while (Unsafe.IsAddressLessThan(ref l, ref Unsafe.SubtractByteOffset(ref upper, 7)))
            {
                if (!Vector64.EqualsAll(Vector64.LoadUnsafe(ref l), Vector64.LoadUnsafe(ref r)))
                    return false;

                l = ref Unsafe.Add(ref l, 8);
                r = ref Unsafe.Add(ref r, 8);
            }

            if (Unsafe.SizeOf<T>() % 8 is 0)
                return true;
        }
#endif
        while (Unsafe.IsAddressLessThan(
            ref l,
            ref Unsafe.SubtractByteOffset(ref upper, (nint)Unsafe.SizeOf<nuint>() - 1)
        ))
        {
            if (Unsafe.As<byte, nuint>(ref l) != Unsafe.As<byte, nuint>(ref r))
                return false;

            l = ref Unsafe.Add(ref l, Unsafe.SizeOf<nuint>());
            r = ref Unsafe.Add(ref r, Unsafe.SizeOf<nuint>());
        }

        while (Unsafe.IsAddressLessThan(ref l, ref Unsafe.SubtractByteOffset(ref upper, sizeof(ulong) - 1)))
        {
            if (Unsafe.As<byte, ulong>(ref l) != Unsafe.As<byte, ulong>(ref r))
                return false;

            l = ref Unsafe.Add(ref l, sizeof(ulong));
            r = ref Unsafe.Add(ref r, sizeof(ulong));
        }

        while (Unsafe.IsAddressLessThan(ref l, ref Unsafe.SubtractByteOffset(ref upper, sizeof(uint) - 1)))
        {
            if (Unsafe.As<byte, uint>(ref l) != Unsafe.As<byte, uint>(ref r))
                return false;

            l = ref Unsafe.Add(ref l, sizeof(uint));
            r = ref Unsafe.Add(ref r, sizeof(uint));
        }

        while (Unsafe.IsAddressLessThan(ref l, ref Unsafe.SubtractByteOffset(ref upper, sizeof(ushort) - 1)))
        {
            if (Unsafe.As<byte, ushort>(ref l) == Unsafe.As<byte, ushort>(ref r))
                return false;

            l = ref Unsafe.Add(ref l, sizeof(ushort));
            r = ref Unsafe.Add(ref r, sizeof(ushort));
        }

        while (Unsafe.IsAddressLessThan(ref l, ref upper))
        {
            if (l != r)
                return false;

            l = ref Unsafe.Add(ref l, 1);
            r = ref Unsafe.Add(ref r, 1);
        }

        return true;
    }

    /// <summary>Determines whether the reference of <typeparamref name="T"/> contains all zeros.</summary>
    /// <param name="reference">The reference to determine if it is zeroed.</param>
    /// <returns>
    /// The value <see langword="true"/> if the parameter <paramref name="reference"/>
    /// points to a value with all zeros; otherwise, <see langword="false"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool Eq0(scoped in T reference)
    {
        ref byte x = ref Unsafe.As<T, byte>(ref AsRef(reference)), upper = ref Unsafe.Add(ref x, 1);
#if NET8_0_OR_GREATER
        if (Vector512.IsHardwareAccelerated && Unsafe.SizeOf<T>() >= 64)
        {
            while (Unsafe.IsAddressLessThan(ref x, ref Unsafe.SubtractByteOffset(ref upper, 63)))
            {
                if (!Vector512.EqualsAll(Vector512.LoadUnsafe(ref x), Vector512<byte>.Zero))
                    return false;

                x = ref Unsafe.Add(ref x, 64);
            }

            if (Unsafe.SizeOf<T>() % 64 is 0)
                return true;
        }
#endif
#if NET7_0_OR_GREATER
        if (Vector256.IsHardwareAccelerated && Unsafe.SizeOf<T>() >= 32)
        {
            while (Unsafe.IsAddressLessThan(ref x, ref Unsafe.SubtractByteOffset(ref upper, 31)))
            {
                if (!Vector256.EqualsAll(Vector256.LoadUnsafe(ref x), Vector256<byte>.Zero))
                    return false;

                x = ref Unsafe.Add(ref x, 32);
            }

            if (Unsafe.SizeOf<T>() % 32 is 0)
                return true;
        }

        if (Vector128.IsHardwareAccelerated && Unsafe.SizeOf<T>() >= 16)
        {
            while (Unsafe.IsAddressLessThan(ref x, ref Unsafe.SubtractByteOffset(ref upper, 15)))
            {
                if (!Vector128.EqualsAll(Vector128.LoadUnsafe(ref x), Vector128<byte>.Zero))
                    return false;

                x = ref Unsafe.Add(ref x, 16);
            }

            if (Unsafe.SizeOf<T>() % 16 is 0)
                return true;
        }

        if (Vector64.IsHardwareAccelerated && Unsafe.SizeOf<T>() >= 8)
        {
            while (Unsafe.IsAddressLessThan(ref x, ref Unsafe.SubtractByteOffset(ref upper, 7)))
            {
                if (!Vector64.EqualsAll(Vector64.LoadUnsafe(ref x), Vector64<byte>.Zero))
                    return false;

                x = ref Unsafe.Add(ref x, 8);
            }

            if (Unsafe.SizeOf<T>() % 8 is 0)
                return true;
        }
#endif
        while (Unsafe.IsAddressLessThan(
            ref x,
            ref Unsafe.SubtractByteOffset(ref upper, (nint)Unsafe.SizeOf<nuint>() - 1)
        ))
        {
            if (Unsafe.As<byte, nuint>(ref x) is not 0)
                return false;

            x = ref Unsafe.Add(ref x, Unsafe.SizeOf<nuint>());
        }

        while (Unsafe.IsAddressLessThan(ref x, ref Unsafe.SubtractByteOffset(ref upper, sizeof(ulong) - 1)))
        {
            if (Unsafe.As<byte, ulong>(ref x) is not 0)
                return false;

            x = ref Unsafe.Add(ref x, sizeof(ulong));
        }

        while (Unsafe.IsAddressLessThan(ref x, ref Unsafe.SubtractByteOffset(ref upper, sizeof(uint) - 1)))
        {
            if (Unsafe.As<byte, uint>(ref x) is not 0)
                return false;

            x = ref Unsafe.Add(ref x, sizeof(uint));
        }

        while (Unsafe.IsAddressLessThan(ref x, ref Unsafe.SubtractByteOffset(ref upper, sizeof(ushort) - 1)))
        {
            if (Unsafe.As<byte, ushort>(ref x) is not 0)
                return false;

            x = ref Unsafe.Add(ref x, sizeof(ushort));
        }

        while (Unsafe.IsAddressLessThan(ref x, ref upper))
        {
            if (x is not 0)
                return false;

            x = ref Unsafe.Add(ref x, 1);
        }

        return true;
    }

    /// <summary>Clamps a value such that it is not smaller or larger than the defined amount.</summary>
    /// <param name="number">The bits to clamp.</param>
    /// <param name="min">The minimum accepted value.</param>
    /// <param name="max">The maximum accepted value.</param>
    /// <returns>
    /// The parameter <paramref name="number"/> if its bits are greater or equal to the parameter
    /// <paramref name="min"/>, and lesser or equal to the parameter <paramref name="number"/>; otherwise,
    /// <paramref name="min"/> if the parameter <paramref name="number"/> is lesser than
    /// <paramref name="min"/>; otherwise, <paramref name="max"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static ref readonly T Clamp(in T number, in T min, in T max) => ref Max(Min(number, max), min);

    /// <summary>Returns the reference that contains the greater bits.</summary>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>
    /// The parameter <paramref name="left"/> if its bits are greater or equal to the
    /// parameter <paramref name="right"/>; otherwise, <paramref name="right"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static ref readonly T Max(in T left, in T right)
    {
        ref T l = ref AsRef(left), r = ref AsRef(right), upper = ref Unsafe.Add(ref l, 1);

        while (Unsafe.IsAddressLessThan(ref l, ref Unsafe.Subtract(ref upper, (nint)Unsafe.SizeOf<nuint>() - 1)))
        {
            if (Unsafe.As<T, nuint>(ref l) != Unsafe.As<T, nuint>(ref r))
                return ref Unsafe.As<T, nuint>(ref l) > Unsafe.As<T, nuint>(ref r) ? ref left : ref right;

            l = ref Unsafe.AddByteOffset(ref l, (nint)Unsafe.SizeOf<nuint>());
        }

        while (Unsafe.IsAddressLessThan(ref l, ref Unsafe.Subtract(ref upper, sizeof(ulong) - 1)))
        {
            if (Unsafe.As<T, ulong>(ref l) != Unsafe.As<T, ulong>(ref r))
                return ref Unsafe.As<T, ulong>(ref l) > Unsafe.As<T, ulong>(ref r) ? ref left : ref right;

            l = ref Unsafe.AddByteOffset(ref l, (nint)sizeof(ulong));
        }

        while (Unsafe.IsAddressLessThan(ref l, ref Unsafe.Subtract(ref upper, sizeof(uint) - 1)))
        {
            if (Unsafe.As<T, uint>(ref l) != Unsafe.As<T, uint>(ref r))
                return ref Unsafe.As<T, uint>(ref l) > Unsafe.As<T, uint>(ref r) ? ref left : ref right;

            l = ref Unsafe.AddByteOffset(ref l, (nint)sizeof(uint));
        }

        while (Unsafe.IsAddressLessThan(ref l, ref Unsafe.Subtract(ref upper, sizeof(ushort) - 1)))
        {
            if (Unsafe.As<T, ushort>(ref l) != Unsafe.As<T, ushort>(ref r))
                return ref Unsafe.As<T, ushort>(ref l) > Unsafe.As<T, ushort>(ref r) ? ref left : ref right;

            l = ref Unsafe.AddByteOffset(ref l, (nint)sizeof(ushort));
        }

        while (Unsafe.IsAddressLessThan(ref l, ref upper))
        {
            if (Unsafe.As<T, byte>(ref l) != Unsafe.As<T, byte>(ref r))
                return ref Unsafe.As<T, byte>(ref l) > Unsafe.As<T, byte>(ref r) ? ref left : ref right;

            l = ref Unsafe.AddByteOffset(ref l, 1);
        }

        return ref left;
    }

    /// <summary>Returns the reference that contains the lesser bits.</summary>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>
    /// The parameter <paramref name="left"/> if its bits are greater or equal to the
    /// parameter <paramref name="right"/>; otherwise, <paramref name="right"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static ref readonly T Min(in T left, in T right)
    {
        ref T l = ref AsRef(left), r = ref AsRef(right), upper = ref Unsafe.Add(ref l, 1);

        while (Unsafe.IsAddressLessThan(ref l, ref Unsafe.Subtract(ref upper, (nint)Unsafe.SizeOf<nuint>() - 1)))
        {
            if (Unsafe.As<T, nuint>(ref l) != Unsafe.As<T, nuint>(ref r))
                return ref Unsafe.As<T, nuint>(ref l) < Unsafe.As<T, nuint>(ref r) ? ref left : ref right;

            l = ref Unsafe.AddByteOffset(ref l, (nint)Unsafe.SizeOf<nuint>());
        }

        while (Unsafe.IsAddressLessThan(ref l, ref Unsafe.Subtract(ref upper, sizeof(ulong) - 1)))
        {
            if (Unsafe.As<T, ulong>(ref l) != Unsafe.As<T, ulong>(ref r))
                return ref Unsafe.As<T, ulong>(ref l) < Unsafe.As<T, ulong>(ref r) ? ref left : ref right;

            l = ref Unsafe.AddByteOffset(ref l, (nint)sizeof(ulong));
        }

        while (Unsafe.IsAddressLessThan(ref l, ref Unsafe.Subtract(ref upper, sizeof(uint) - 1)))
        {
            if (Unsafe.As<T, uint>(ref l) != Unsafe.As<T, uint>(ref r))
                return ref Unsafe.As<T, uint>(ref l) < Unsafe.As<T, uint>(ref r) ? ref left : ref right;

            l = ref Unsafe.AddByteOffset(ref l, (nint)sizeof(uint));
        }

        while (Unsafe.IsAddressLessThan(ref l, ref Unsafe.Subtract(ref upper, sizeof(ushort) - 1)))
        {
            if (Unsafe.As<T, ushort>(ref l) != Unsafe.As<T, ushort>(ref r))
                return ref Unsafe.As<T, ushort>(ref l) < Unsafe.As<T, ushort>(ref r) ? ref left : ref right;

            l = ref Unsafe.AddByteOffset(ref l, (nint)sizeof(ushort));
        }

        while (Unsafe.IsAddressLessThan(ref l, ref upper))
        {
            if (Unsafe.As<T, byte>(ref l) != Unsafe.As<T, byte>(ref r))
                return ref Unsafe.As<T, byte>(ref l) < Unsafe.As<T, byte>(ref r) ? ref left : ref right;

            l = ref Unsafe.AddByteOffset(ref l, 1);
        }

        return ref left;
    }
}
#endif
