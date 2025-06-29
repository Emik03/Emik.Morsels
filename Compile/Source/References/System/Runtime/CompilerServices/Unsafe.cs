﻿// SPDX-License-Identifier: MPL-2.0
#pragma warning disable GlobalUsingsAnalyzer
#if NETFRAMEWORK && !NET45_OR_GREATER || NETSTANDARD1_0 // ReSharper disable once CheckNamespace
namespace System.Runtime.CompilerServices;

static class Unsafe
{
#pragma warning disable CS8500
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static unsafe void* AsPointer<T>(ref T value)
    {
        var tr = __makeref(value);
        return (void*)*(nint*)&tr;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] // ReSharper disable once NullableWarningSuppressionIsUsed
    public static T As<T>(object? o) => (T)o!;

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TTo As<TFrom, TTo>(ref readonly TFrom o)
    {
        // ReSharper disable RedundantNameQualifier
        // ReSharper disable once UseSymbolAlias
        System.Diagnostics.Debug.Assert(SizeOf<TFrom>() >= SizeOf<TTo>(), "No out-of-bounds access.");
#if NET452_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP
        // We have to resort to inline IL because Unsafe.As<T> has a constraint for classes,
        // and Unsafe.As<TFrom, TTo> introduces a miniscule amount of overhead.
        // Doing it like this reduces the IL size from 9 to 2 bytes, and the JIT assembly from 9 to 3 bytes.
        InlineIL.IL.Emit.Ldarg_0();
        return InlineIL.IL.Return<TTo>();
#else
        unsafe
        {
            return *(TTo*)&o;
        }
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)] // ReSharper disable once NullableWarningSuppressionIsUsed
    public static void SkipInit<T>(out T value) => value = default!;

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static unsafe int SizeOf<T>() => sizeof(T);
#pragma warning restore CS8500
}
#elif !(NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) || NO_SYSTEM_MEMORY
using static InlineIL.IL;
using static InlineIL.IL.Emit;

// Taken from: https://raw.githubusercontent.com/ltrzesniewski/InlineIL.Fody/master/src/InlineIL.Examples/Unsafe.cs
// ReSharper disable CheckNamespace EntityNameCapturedOnly.Global
namespace System.Runtime.CompilerServices;

static unsafe class Unsafe
{
    // This is the InlineIL equivalent of System.Runtime.CompilerServices.Unsafe
    // https://github.com/dotnet/runtime/blob/release/6.0/src/libraries/System.Runtime.CompilerServices.Unsafe/src/System.Runtime.CompilerServices.Unsafe.il
    // Last update: 98ace7d4837fcd81c1f040b1f67e63e9e1973e13 - these methods became intrinsics starting from .NET 7

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T Read<T>(void* source)
    {
        Ldarg(nameof(source));
        Ldobj(typeof(T));
        return Return<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T ReadUnaligned<T>(void* source)
    {
        Ldarg(nameof(source));
        Unaligned(1);
        Ldobj(typeof(T));
        return Return<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T ReadUnaligned<T>(ref byte source)
    {
        Ldarg(nameof(source));
        Unaligned(1);
        Ldobj(typeof(T));
        return Return<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write<T>(void* destination, T value)
    {
        Ldarg(nameof(destination));
        Ldarg(nameof(value));
        Stobj(typeof(T));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteUnaligned<T>(void* destination, T value)
    {
        Ldarg(nameof(destination));
        Ldarg(nameof(value));
        Unaligned(1);
        Stobj(typeof(T));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteUnaligned<T>(ref byte destination, T value)
    {
        Ldarg(nameof(destination));
        Ldarg(nameof(value));
        Unaligned(1);
        Stobj(typeof(T));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Copy<T>(void* destination, ref T source)
    {
        Ldarg(nameof(destination));
        Ldarg(nameof(source));
        Ldobj(typeof(T));
        Stobj(typeof(T));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Copy<T>(ref T destination, void* source)
    {
        Ldarg(nameof(destination));
        Ldarg(nameof(source));
        Ldobj(typeof(T));
        Stobj(typeof(T));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static void* AsPointer<T>(ref T value)
    {
        Ldarg(nameof(value));
        Conv_U();
        return ReturnPointer();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SkipInit<T>(out T value)
    {
        Ret();
        throw Unreachable();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static int SizeOf<T>()
    {
        Sizeof(typeof(T));
        return Return<int>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyBlock(void* destination, void* source, uint byteCount)
    {
        Ldarg(nameof(destination));
        Ldarg(nameof(source));
        Ldarg(nameof(byteCount));
        Cpblk();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyBlock(ref byte destination, ref byte source, uint byteCount)
    {
        Ldarg(nameof(destination));
        Ldarg(nameof(source));
        Ldarg(nameof(byteCount));
        Cpblk();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyBlockUnaligned(void* destination, void* source, uint byteCount)
    {
        Ldarg(nameof(destination));
        Ldarg(nameof(source));
        Ldarg(nameof(byteCount));
        Unaligned(1);
        Cpblk();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyBlockUnaligned(ref byte destination, ref byte source, uint byteCount)
    {
        Ldarg(nameof(destination));
        Ldarg(nameof(source));
        Ldarg(nameof(byteCount));
        Unaligned(1);
        Cpblk();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void InitBlock(void* startAddress, byte value, uint byteCount)
    {
        Ldarg(nameof(startAddress));
        Ldarg(nameof(value));
        Ldarg(nameof(byteCount));
        Initblk();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void InitBlock(ref byte startAddress, byte value, uint byteCount)
    {
        Ldarg(nameof(startAddress));
        Ldarg(nameof(value));
        Ldarg(nameof(byteCount));
        Initblk();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void InitBlockUnaligned(void* startAddress, byte value, uint byteCount)
    {
        Ldarg(nameof(startAddress));
        Ldarg(nameof(value));
        Ldarg(nameof(byteCount));
        Unaligned(1);
        Initblk();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void InitBlockUnaligned(ref byte startAddress, byte value, uint byteCount)
    {
        Ldarg(nameof(startAddress));
        Ldarg(nameof(value));
        Ldarg(nameof(byteCount));
        Unaligned(1);
        Initblk();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T As<T>(object? o)
        where T : class
    {
        Ldarg(nameof(o));
        return Return<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static ref T AsRef<T>(void* source)
    {
#if NETCOREAPP
        Push(source);
        return ref ReturnRef<T>();
#else
        DeclareLocals(
            false, // ReSharper disable once RedundantNameQualifier
            new InlineIL.LocalVar("local", typeof(int).MakeByRefType())
        );

        Push(source);
        Stloc("local");
        Ldloc("local");
        return ref ReturnRef<T>();
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static ref T AsRef<T>(in T source)
    {
        Ldarg(nameof(source));
        return ref ReturnRef<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static ref TTo As<TFrom, TTo>(ref TFrom source)
    {
        Ldarg(nameof(source));
        return ref ReturnRef<TTo>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static ref T Unbox<T>(object box)
        where T : struct
    {
        Push(box);
        Emit.Unbox(typeof(T));
        return ref ReturnRef<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static ref T Add<T>(ref T source, int elementOffset)
    {
        Ldarg(nameof(source));
        Ldarg(nameof(elementOffset));
        Sizeof(typeof(T));
        Conv_I();
        Mul();
        Emit.Add();
        return ref ReturnRef<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static void* Add<T>(void* source, int elementOffset)
    {
        Ldarg(nameof(source));
        Ldarg(nameof(elementOffset));
        Sizeof(typeof(T));
        Conv_I();
        Mul();
        Emit.Add();
        return ReturnPointer();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static ref T Add<T>(ref T source, nint elementOffset)
    {
        Ldarg(nameof(source));
        Ldarg(nameof(elementOffset));
        Sizeof(typeof(T));
        Mul();
        Emit.Add();
        return ref ReturnRef<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static ref T Add<T>(ref T source, nuint elementOffset)
    {
        Ldarg(nameof(source));
        Ldarg(nameof(elementOffset));
        Sizeof(typeof(T));
        Mul();
        Emit.Add();
        return ref ReturnRef<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static ref T AddByteOffset<T>(ref T source, nint byteOffset)
    {
        Ldarg(nameof(source));
        Ldarg(nameof(byteOffset));
        Emit.Add();
        return ref ReturnRef<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static ref T AddByteOffset<T>(ref T source, nuint byteOffset)
    {
        Ldarg(nameof(source));
        Ldarg(nameof(byteOffset));
        Emit.Add();
        return ref ReturnRef<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static ref T Subtract<T>(ref T source, int elementOffset)
    {
        Ldarg(nameof(source));
        Ldarg(nameof(elementOffset));
        Sizeof(typeof(T));
        Conv_I();
        Mul();
        Sub();
        return ref ReturnRef<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static void* Subtract<T>(void* source, int elementOffset)
    {
        Ldarg(nameof(source));
        Ldarg(nameof(elementOffset));
        Sizeof(typeof(T));
        Conv_I();
        Mul();
        Sub();
        return ReturnPointer();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static ref T Subtract<T>(ref T source, nint elementOffset)
    {
        Ldarg(nameof(source));
        Ldarg(nameof(elementOffset));
        Sizeof(typeof(T));
        Mul();
        Sub();
        return ref ReturnRef<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static ref T Subtract<T>(ref T source, nuint elementOffset)
    {
        Ldarg(nameof(source));
        Ldarg(nameof(elementOffset));
        Sizeof(typeof(T));
        Mul();
        Sub();
        return ref ReturnRef<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static ref T SubtractByteOffset<T>(ref T source, nint byteOffset)
    {
        Ldarg(nameof(source));
        Ldarg(nameof(byteOffset));
        Sub();
        return ref ReturnRef<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static ref T SubtractByteOffset<T>(ref T source, nuint byteOffset)
    {
        Ldarg(nameof(source));
        Ldarg(nameof(byteOffset));
        Sub();
        return ref ReturnRef<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static nint ByteOffset<T>(ref T origin, ref T target)
    {
        Ldarg(nameof(target));
        Ldarg(nameof(origin));
        Sub();
        return Return<nint>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool AreSame<T>(ref T left, ref T right)
    {
        Ldarg(nameof(left));
        Ldarg(nameof(right));
        Ceq();
        return Return<bool>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool IsAddressGreaterThan<T>(ref T left, ref T right)
    {
        Ldarg(nameof(left));
        Ldarg(nameof(right));
        Cgt_Un();
        return Return<bool>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool IsAddressLessThan<T>(ref T left, ref T right)
    {
        Ldarg(nameof(left));
        Ldarg(nameof(right));
        Clt_Un();
        return Return<bool>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool IsNullRef<T>(ref T source)
    {
        Ldarg(nameof(source));
        Ldc_I4_0();
        Conv_U();
        Ceq();
        return Return<bool>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static ref T NullRef<T>()
    {
        Ldc_I4_0();
        Conv_U();
        return ref ReturnRef<T>();
    }
}
#endif
