// SPDX-License-Identifier: MPL-2.0
#pragma warning disable IDE0250
// ReSharper disable BadPreprocessorIndent CheckNamespace StructCanBeMadeReadOnly
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
    /// <inheritdoc cref="ICollection{T}.Contains"/>
    [CollectionAccess(CollectionAccessType.Read), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public bool Contains(T item)
    {
        using var that = GetEnumerator();

        while (that.MoveNext())
        {
            var current = that.Current;

            if (Eq(current, item))
                return true;
        }

        return false;
    }

    /// <inheritdoc cref="ISet{T}.IsProperSubsetOf" />
    [CollectionAccess(CollectionAccessType.Read), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public bool IsProperSubsetOf([InstantHandle] IEnumerable<T> other)
    {
        T t = default;

        var collection = other
#if WAWA
           .ToList();
#else
           .ToCollectionLazily();
#endif

        // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
        foreach (var next in this)
            if (collection.Contains(next))
                Or(next, ref t);
            else
                return false;

        // ReSharper disable once LoopCanBeConvertedToQuery ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
        foreach (var next in collection)
            if (!Contains(next))
                return true;

        return false;
    }

    /// <inheritdoc cref="ISet{T}.IsProperSupersetOf" />
    [CollectionAccess(CollectionAccessType.Read), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public bool IsProperSupersetOf([InstantHandle] IEnumerable<T> other)
    {
        T t = default;

        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach (var next in other)
            if (Contains(next))
                Or(next, ref t);
            else
                return false;

        return !Eq(_value, t);
    }

    /// <inheritdoc cref="ISet{T}.IsSubsetOf" />
    [CollectionAccess(CollectionAccessType.Read), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public bool IsSubsetOf([InstantHandle] IEnumerable<T> other)
    {
        var collection = other
#if WAWA
           .ToList();
#else
           .ToCollectionLazily();
#endif

        // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
        foreach (var next in this)
            if (!collection.Contains(next))
                return false;

        return true;
    }

    /// <inheritdoc cref="ISet{T}.IsSupersetOf" />
    [CollectionAccess(CollectionAccessType.Read), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public bool IsSupersetOf([InstantHandle] IEnumerable<T> other) => other.All(Contains);

    /// <inheritdoc cref="ISet{T}.Overlaps" />
    [CollectionAccess(CollectionAccessType.Read), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public bool Overlaps([InstantHandle] IEnumerable<T> other) => other.Any(Contains);

    /// <inheritdoc cref="ISet{T}.SetEquals" />
    [CollectionAccess(CollectionAccessType.Read), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public bool SetEquals([InstantHandle] IEnumerable<T> other)
    {
        T t = default;

        foreach (var next in other)
            if ((Enumerator)next is var e && !e.MoveNext() || e.MoveNext())
                return false;
            else
                Or(next, ref t);

        return Eq(_value, t);
    }
}
