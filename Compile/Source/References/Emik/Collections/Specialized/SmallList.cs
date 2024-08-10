// SPDX-License-Identifier: MPL-2.0

// ReSharper disable RedundantUnsafeContext RedundantUsingDirective
// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

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
    /// And the Lord spake, saying, "First shalt thou recreate the
    /// <a href="https://crates.io/crates/smallvec"><c>smallvec</c></a> crate. Then, shalt thou keep three inline. No
    /// more. No less. Three shalt be the number thou shalt keep inline, and the number to keep inline shalt be three.
    /// Four shalt thou not keep inline, nor either keep inline thou two, excepting that thou then proceed to three.
    /// Five is right out. Once the number three,  being the third number, be reached, then, lobbest thou thy
    /// <see cref="SmallList{T}"/> towards thy heap, who, being slow and cache-naughty in My sight, shall snuff it.".
    /// </para><para>
    /// <a href="https://github.com/rhaiscript/rhai/blob/ca18cdd7f47f8ae8bd6e2b7a950ad4815d62f026/src/lib.rs#L373">
    /// (Adapted from Rhai)
    /// </a></para></remarks>
    public const int InlinedLength = 3;

    /// <summary>Initializes a new instance of the <see cref="SmallList{T}"/> struct with no elements.</summary>
    /// <typeparam name="T">The type of element in the <see cref="SmallList{T}"/>.</typeparam>
    /// <returns>The created <see cref="SmallList{T}"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SmallList<T> Create<T>() => [];

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
}
