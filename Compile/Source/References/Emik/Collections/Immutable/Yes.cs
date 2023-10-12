// SPDX-License-Identifier: MPL-2.0

// ReSharper disable BadPreprocessorIndent CheckNamespace RedundantExtendsListEntry StructCanBeMadeReadOnly

namespace Emik.Morsels;
#pragma warning disable IDE0250, IDE0251, MA0102, SA1137
using static CollectionAccessType;
using static YesFactory;

/// <summary>Extension methods that act as factories for <see cref="Yes{T}"/>.</summary>
#pragma warning disable MA0048
static partial class YesFactory
#pragma warning restore MA0048
{
    /// <summary>Gets the fallback for when an enumeration returns <see langword="null"/>.</summary>
    public static object Fallback { get; } = new();

    /// <summary>Creates a <see cref="Yes{T}"/> from an item.</summary>
    /// <typeparam name="T">The type of item.</typeparam>
    /// <param name="source">The item.</param>
    /// <returns>The <see cref="Yes{T}"/> instance that can be yielded forever.</returns>
    [Pure]
    public static Yes<T> Forever<T>(this T source) => source;
}

/// <summary>A factory for creating iterator types that yield the same item forever.</summary>
/// <typeparam name="T">The type of the item to yield.</typeparam>
/// <param name="value">The item to use.</param>
[StructLayout(LayoutKind.Auto)]
#if CSHARPREPL
public
#endif
#if !NO_READONLY_STRUCTS
readonly
#endif
    partial struct Yes<T>([ProvidesContext] T value) : IEnumerable<T>, IEnumerator<T>
{
    /// <inheritdoc />
    [CollectionAccess(Read), ProvidesContext, Pure]
    public T Current => value;

    /// <inheritdoc />
    [CollectionAccess(Read), Pure]
    object IEnumerator.Current => value ?? Fallback;

    /// <summary>Implicitly calls the constructor.</summary>
    /// <param name="value">The value to pass into the constructor.</param>
    /// <returns>A new instance of <see cref="Yes{T}"/> with <paramref name="value"/> passed in.</returns>
    [CollectionAccess(Read), Pure]
    public static implicit operator Yes<T>([ProvidesContext] T value) => new(value);

    /// <summary>Implicitly calls <see cref="Current"/>.</summary>
    /// <param name="value">The value to call <see cref="Current"/>.</param>
    /// <returns>The value that was passed in to this instance.</returns>
    [CollectionAccess(Read), Pure]
    public static implicit operator T(Yes<T> value) => value.Current;

    /// <summary>Returns itself.</summary>
    /// <remarks><para>Used to allow <see langword="foreach"/> to be used on <see cref="Yes{T}"/>.</para></remarks>
    /// <returns>Itself.</returns>
    [CollectionAccess(None), Pure]
    public Yes<T> GetEnumerator() => this;

    /// <inheritdoc />
    [CollectionAccess(None)]
    void IDisposable.Dispose() { }

    /// <inheritdoc />
    [CollectionAccess(None)]
    void IEnumerator.Reset() { }

    /// <inheritdoc />
    [CollectionAccess(None), Pure]
    bool IEnumerator.MoveNext() => true;

    /// <inheritdoc />
    [CollectionAccess(None), Pure]
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    [CollectionAccess(None), Pure]
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
}
