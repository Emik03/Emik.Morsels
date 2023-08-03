// SPDX-License-Identifier: MPL-2.0
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
// ReSharper disable RedundantExtendsListEntry RedundantUnsafeContext
// ReSharper disable once CheckNamespace
namespace Emik.Morsels;
#pragma warning disable RCS1242 // Normally causes defensive copies; Parameter is unused though.
using static Span;

/// <summary>Factory methods for creating inlined <see cref="SmallList{T}"/> instances.</summary>
static partial class SmallList
{
    /// <summary>Allocates an inlined list of the specified size.</summary>
    /// <remarks><para>
    /// The returned <see cref="PooledSmallList{T}"/> will point to uninitialized memory.
    /// Be sure to call <see cref="Span{T}.Fill"/> or otherwise written to first before enumeration or reading.
    /// </para></remarks>
    /// <typeparam name="T">The type of <see cref="Span{T}"/>.</typeparam>
    /// <param name="_">The discard, which is used to let the compiler track lifetimes.</param>
    /// <returns>The <see cref="Span{T}"/> of the specified size.</returns>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining)]
#if DEBUG || CSHARPREPL // ReSharper disable once NullableWarningSuppressionIsUsed
    public static PooledSmallList<T> New1<T>(in T _ = default!) =>
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        Ref(ref Unsafe.AsRef(_));
#else
    public static unsafe PooledSmallList<T> New1<T>(in bool _ = false)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        Unsafe.SkipInit(out T one);
#pragma warning disable 9091 // InlineAttribute makes this okay.
        return Ref(ref Unsafe.AsRef(one));
#pragma warning restore 9091
    }
#endif

    /// <inheritdoc cref="New1{T}"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining)]
#if DEBUG || CSHARPREPL
    public static PooledSmallList<T> New2<T>(in Two<T> _ = default)
#else
    public static PooledSmallList<T> New2<T>(in bool _ = false)
#endif
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            From<T, Two<T>>(_);

    /// <inheritdoc cref="New1{T}"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining)]
#if DEBUG || CSHARPREPL
    public static PooledSmallList<T> New4<T>(in Two<Two<T>> _ = default)
#else
    public static PooledSmallList<T> New4<T>(in bool _ = false)
#endif
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            From<T, Two<Two<T>>>(_);

    /// <inheritdoc cref="New1{T}"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining)]
#if DEBUG || CSHARPREPL
    public static PooledSmallList<T> New8<T>(in Two<Two<Two<T>>> _ = default)
#else
    public static PooledSmallList<T> New8<T>(in bool _ = false)
#endif
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            From<T, Two<Two<Two<T>>>>(_);

    /// <inheritdoc cref="New1{T}"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining)]
#if DEBUG || CSHARPREPL
    public static PooledSmallList<T> New16<T>(in Two<Two<Two<Two<T>>>> _ = default)
#else
    public static PooledSmallList<T> New16<T>(in bool _ = false)
#endif
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            From<T, Two<Two<Two<Two<T>>>>>(_);

    /// <inheritdoc cref="New1{T}"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining)]
#if DEBUG || CSHARPREPL
    public static PooledSmallList<T> New32<T>(in Two<Two<Two<Two<Two<T>>>>> _ = default)
#else
    public static PooledSmallList<T> New32<T>(in bool _ = false)
#endif
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            From<T, Two<Two<Two<Two<Two<T>>>>>>(_);

    /// <inheritdoc cref="New1{T}"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining)]
#if DEBUG || CSHARPREPL
    public static PooledSmallList<T> New64<T>(in Two<Two<Two<Two<Two<Two<T>>>>>> _ = default)
#else
    public static PooledSmallList<T> New64<T>(in bool _ = false)
#endif
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            From<T, Two<Two<Two<Two<Two<Two<T>>>>>>>(_);

    /// <inheritdoc cref="New1{T}"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining)]
#if DEBUG || CSHARPREPL
    public static PooledSmallList<T> New128<T>(in Two<Two<Two<Two<Two<Two<Two<T>>>>>>> _ = default)
#else
    public static PooledSmallList<T> New128<T>(in bool _ = false)
#endif
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            From<T, Two<Two<Two<Two<Two<Two<Two<T>>>>>>>>(_);

    /// <inheritdoc cref="New1{T}"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining)]
#if DEBUG || CSHARPREPL
    public static PooledSmallList<T> New256<T>(in Two<Two<Two<Two<Two<Two<Two<Two<T>>>>>>>> _ = default)
#else
    public static PooledSmallList<T> New256<T>(in bool _ = false)
#endif
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            From<T, Two<Two<Two<Two<Two<Two<Two<Two<T>>>>>>>>>(_);

    /// <inheritdoc cref="New1{T}"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining)]
#if DEBUG || CSHARPREPL
    public static PooledSmallList<T> New512<T>(in Two<Two<Two<Two<Two<Two<Two<Two<Two<T>>>>>>>>> _ = default)
#else
    public static PooledSmallList<T> New512<T>(in bool _ = false)
#endif
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            From<T, Two<Two<Two<Two<Two<Two<Two<Two<Two<T>>>>>>>>>>(_);

    /// <inheritdoc cref="New1{T}"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining)]
#if DEBUG || CSHARPREPL
    public static PooledSmallList<T> New1024<T>(in Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<T>>>>>>>>>> _ = default)
#else
    public static PooledSmallList<T> New1024<T>(in bool _ = false)
#endif
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            From<T, Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<T>>>>>>>>>>>(_);

    /// <inheritdoc cref="New1{T}"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining)]
#if DEBUG || CSHARPREPL
    public static PooledSmallList<T> From<T, TRef>(in TRef _ = default)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        where TRef : struct =>
        PooledSmallList<T>.From(ref Unsafe.AsRef(_));
#else
    public static unsafe PooledSmallList<T> From<T, TRef>(in bool _ = false)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        where TRef : struct
    {
        Unsafe.SkipInit(out TRef two);
#pragma warning disable 9091 // InlineAttribute makes this okay.
        return PooledSmallList<T>.From(ref Unsafe.AsRef(two));
#pragma warning restore 9091
    }
#endif
}
#endif
