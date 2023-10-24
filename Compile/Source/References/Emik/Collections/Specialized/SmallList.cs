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
    /// <summary>Number of items to keep inline for <see cref="SmallList{T}"/>.</summary>
    /// <remarks><para>
    /// And Saint Attila raised the <see cref="SmallList{T}"/> up on high, saying, "O Lord, bless this Thy
    /// <see cref="SmallList{T}"/> that, with it, Thou mayest blow Thine allocation costs to tiny bits in Thy mercy.".
    /// </para><para>
    /// And the Lord did grin, and the people did feast upon the lambs and sloths and carp and anchovies and orangutans
    /// and breakfast cereals and fruit bats and large chu...
    /// </para><para>
    /// And the Lord spake, saying, "First shalt thou recreate the <c>smallvec</c> (https://crates.io/crates/smallvec)
    /// crate. Then, shalt thou keep three inline. No more. No less. Three shalt be the number thou shalt keep inline,
    /// and the number to keep inline shalt be three. Four shalt thou not keep inline, nor either keep inline thou two,
    /// excepting that thou then proceed to three. Five is right out. Once the number three,  being the third number,
    /// be reached, then, lobbest thou thy <see cref="SmallList{T}"/> towards thy heap, who, being slow and
    /// cache-naughty in My sight, shall snuff it.".
    /// </para><para>
    /// (Source: https://github.com/rhaiscript/rhai/blob/ca18cdd7f47f8ae8bd6e2b7a950ad4815d62f026/src/lib.rs#L373).
    /// </para></remarks>
#pragma warning disable RCS1158
    public const int InlinedLength = 3;
#pragma warning restore RCS1158

    /// <summary>Initializes a new instance of the <see cref="SmallList{T}"/> struct with no elements.</summary>
    /// <typeparam name="T">The type of element in the <see cref="SmallList{T}"/>.</typeparam>
    /// <returns>The created <see cref="SmallList{T}"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SmallList<T> Create<T>() => default;

    /// <summary>Initializes a new instance of the <see cref="SmallList{T}"/> struct with 1 element.</summary>
    /// <typeparam name="T">The type of element in the <see cref="SmallList{T}"/>.</typeparam>
    /// <param name="first">The first element.</param>
    /// <returns>The created <see cref="SmallList{T}"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SmallList<T> Create<T>(T first) => first;

    /// <summary>Initializes a new instance of the <see cref="SmallList{T}"/> struct with 2 elements.</summary>
    /// <typeparam name="T">The type of element in the <see cref="SmallList{T}"/>.</typeparam>
    /// <param name="first">The first element.</param>
    /// <param name="second">The second element.</param>
    /// <returns>The created <see cref="SmallList{T}"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SmallList<T> Create<T>(T first, T second) => new(first, second);

    /// <summary>Initializes a new instance of the <see cref="SmallList{T}"/> struct with 3 elements.</summary>
    /// <typeparam name="T">The type of element in the <see cref="SmallList{T}"/>.</typeparam>
    /// <param name="first">The first element.</param>
    /// <param name="second">The second element.</param>
    /// <param name="third">The third element.</param>
    /// <returns>The created <see cref="SmallList{T}"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SmallList<T> Create<T>(T first, T second, T third) => new(first, second, third);

    /// <summary>Creates a new instance of the <see cref="SmallList{T}"/> struct with arbitrary elements.</summary>
    /// <typeparam name="T">The type of element in the <see cref="SmallList{T}"/>.</typeparam>
    /// <param name="first">The first element.</param>
    /// <param name="second">The second element.</param>
    /// <param name="third">The third element.</param>
    /// <param name="rest">The rest of the elements.</param>
    /// <returns>The created <see cref="SmallList{T}"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SmallList<T> Create<T>(T first, T second, T third, params T[] rest) =>
        new(first, second, third, rest);

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
        return Ref(ref one);
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
        return PooledSmallList<T>.From(ref two);
#pragma warning restore 9091
    }
#endif
}
#endif
