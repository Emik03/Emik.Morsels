// SPDX-License-Identifier: MPL-2.0

// ReSharper disable RedundantExtendsListEntry RedundantUnsafeContext
// ReSharper disable once CheckNamespace
namespace Emik.Morsels;
#pragma warning disable RCS1242 // Normally causes defensive copies; Parameter is unused though.
/// <summary>Factory methods for creating inlined <see cref="SmallList{T}"/> instances.</summary>
static partial class SmallList
{
    /// <summary>Allocates an inlined list of the specified size.</summary>
    /// <remarks><para>
    /// The returned <see cref="SmallList{T, TRef}"/> will point to uninitialized memory.
    /// Be sure to call <see cref="Span{T}.Fill"/> or otherwise written to first before enumeration or reading.
    /// </para></remarks>
    /// <typeparam name="T">The type of <see cref="Span{T}"/>.</typeparam>
    /// <param name="_">The discard, which is used to let the compiler track lifetimes.</param>
    /// <returns>The <see cref="Span{T}"/> of the specified size.</returns>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining)]
#if DEBUG || CSHARPREPL // ReSharper disable once NullableWarningSuppressionIsUsed
    public static SmallList<T, T> New1<T>(in T _ = default!)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            new(ref Unsafe.AsRef(_));
#else
    public static SmallList<T, T> New1<T>(in bool _ = false)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        Unsafe.SkipInit(out T two);
        return new(ref Unsafe.AsRef(two));
    }
#endif

    /// <inheritdoc cref="New1{T}"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining)]
#if DEBUG || CSHARPREPL
    public static SmallList<T, Two<T>> New2<T>(in Two<T> _ = default)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            new(ref Unsafe.AsRef(_));
#else
    public static SmallList<T, Two<T>> New2<T>(in bool _ = false)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        Unsafe.SkipInit(out Two<T> two);
        return new(ref Unsafe.AsRef(two));
    }
#endif

    /// <inheritdoc cref="New1{T}"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining)]
#if DEBUG || CSHARPREPL
    public static SmallList<T, Two<Two<T>>> New4<T>(in Two<Two<T>> _ = default)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            new(ref Unsafe.AsRef(_));
#else
    public static SmallList<T, Two<Two<T>>> New4<T>(in bool _ = false)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        Unsafe.SkipInit(out Two<Two<T>> two);
        return new(ref Unsafe.AsRef(two));
    }
#endif

    /// <inheritdoc cref="New1{T}"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining)]
#if DEBUG || CSHARPREPL
    public static SmallList<T, Two<Two<Two<T>>>> New8<T>(in Two<Two<Two<T>>> _ = default)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            new(ref Unsafe.AsRef(_));
#else
    public static SmallList<T, Two<Two<Two<T>>>> New8<T>(in bool _ = false)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        Unsafe.SkipInit(out Two<Two<Two<T>>> two);
        return new(ref Unsafe.AsRef(two));
    }
#endif

    /// <inheritdoc cref="New1{T}"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining)]
#if DEBUG || CSHARPREPL
    public static SmallList<T, Two<Two<Two<Two<T>>>>> New16<T>(in Two<Two<Two<Two<T>>>> _ = default)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            new(ref Unsafe.AsRef(_));
#else
    public static SmallList<T, Two<Two<Two<Two<T>>>>> New16<T>(in bool _ = false)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        Unsafe.SkipInit(out Two<Two<Two<Two<T>>>> two);
        return new(ref Unsafe.AsRef(two));
    }
#endif

    /// <inheritdoc cref="New1{T}"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining)]
#if DEBUG || CSHARPREPL
    public static SmallList<T, Two<Two<Two<Two<Two<T>>>>>> New32<T>(in Two<Two<Two<Two<Two<T>>>>> _ = default)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            new(ref Unsafe.AsRef(_));
#else
    public static SmallList<T, Two<Two<Two<Two<Two<T>>>>>> New32<T>(in bool _ = false)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        Unsafe.SkipInit(out Two<Two<Two<Two<Two<T>>>>> two);
        return new(ref Unsafe.AsRef(two));
    }
#endif

    /// <inheritdoc cref="New1{T}"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining)]
#if DEBUG || CSHARPREPL
    public static SmallList<T, Two<Two<Two<Two<Two<Two<T>>>>>>> New64<T>(
        in Two<Two<Two<Two<Two<Two<T>>>>>> _ = default
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            new(ref Unsafe.AsRef(_));
#else
    public static SmallList<T, Two<Two<Two<Two<Two<Two<T>>>>>>> New64<T>(in bool _ = false)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        Unsafe.SkipInit(out Two<Two<Two<Two<Two<Two<T>>>>>> two);
        return new(ref Unsafe.AsRef(two));
    }
#endif

    /// <inheritdoc cref="New1{T}"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining)]
#if DEBUG || CSHARPREPL
    public static SmallList<T, Two<Two<Two<Two<Two<Two<Two<T>>>>>>>> New128<T>(
        in Two<Two<Two<Two<Two<Two<Two<T>>>>>>> _ = default
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            new(ref Unsafe.AsRef(_));
#else
    public static SmallList<T, Two<Two<Two<Two<Two<Two<Two<T>>>>>>>> New128<T>(in bool _ = false)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        Unsafe.SkipInit(out Two<Two<Two<Two<Two<Two<Two<T>>>>>>> two);
        return new(ref Unsafe.AsRef(two));
    }
#endif

    /// <inheritdoc cref="New1{T}"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining)]
#if DEBUG || CSHARPREPL
    public static SmallList<T, Two<Two<Two<Two<Two<Two<Two<Two<T>>>>>>>>> New256<T>(
        in Two<Two<Two<Two<Two<Two<Two<Two<T>>>>>>>> _ = default
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            new(ref Unsafe.AsRef(_));
#else
    public static SmallList<T, Two<Two<Two<Two<Two<Two<Two<Two<T>>>>>>>>> New256<T>(in bool _ = false)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        Unsafe.SkipInit(out Two<Two<Two<Two<Two<Two<Two<Two<T>>>>>>>> two);
        return new(ref Unsafe.AsRef(two));
    }
#endif

    /// <inheritdoc cref="New1{T}"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining)]
#if DEBUG || CSHARPREPL
    public static SmallList<T, Two<Two<Two<Two<Two<Two<Two<Two<Two<T>>>>>>>>>> New512<T>(
        in Two<Two<Two<Two<Two<Two<Two<Two<Two<T>>>>>>>>> _ = default
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            new(ref Unsafe.AsRef(_));
#else
    public static SmallList<T, Two<Two<Two<Two<Two<Two<Two<Two<Two<T>>>>>>>>>> New512<T>(in bool _ = false)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        Unsafe.SkipInit(out Two<Two<Two<Two<Two<Two<Two<Two<Two<T>>>>>>>>> two);
        return new(ref Unsafe.AsRef(two));
    }
#endif

    /// <inheritdoc cref="New1{T}"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining)]
#if DEBUG || CSHARPREPL
    public static SmallList<T, Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<T>>>>>>>>>>> New1024<T>(
        in Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<T>>>>>>>>>> _ = default
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            new(ref Unsafe.AsRef(_));
#else
    public static SmallList<T, Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<T>>>>>>>>>>> New1024<T>(
        in bool _ = false
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        Unsafe.SkipInit(out Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<T>>>>>>>>>> two);
        return new(ref Unsafe.AsRef(two));
    }
#endif

    /// <inheritdoc cref="New1{T}"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining)]
#if DEBUG || CSHARPREPL
    public static SmallList<T, TRef> New<T>(in TRef _ = default)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            new(ref Unsafe.AsRef(_));
#else
    public static SmallList<T, TRef> New<T, TRef>(
        in bool _ = false
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        Unsafe.SkipInit(out TRef two);
        return new(ref Unsafe.AsRef(two));
    }
#endif
}
