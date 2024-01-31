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
    /// <summary>Determines whether the item has only a single bit.</summary>
    /// <param name="item">The element to test.</param>
    /// <returns>
    /// The value <see langword="true"/> if the parameter <paramref name="item"/>
    /// has a single bit set; otherwise, <see langword="false"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool IsSingle(in T item) =>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
    	0 switch
    	{
    	    _ when typeof(T) == typeof(byte) => BitOperations.IsPow2((byte)(object)item),
    	    _ when typeof(T) == typeof(sbyte) => BitOperations.IsPow2(unchecked((byte)(sbyte)(object)item)),
    	    _ when typeof(T) == typeof(short) => BitOperations.IsPow2(unchecked((ushort)(short)(object)item)),
    	    _ when typeof(T) == typeof(ushort) => BitOperations.IsPow2((ushort)(object)item),
    	    _ when typeof(T) == typeof(int) => BitOperations.IsPow2(unchecked((uint)(int)(object)item)),
    	    _ when typeof(T) == typeof(uint) => BitOperations.IsPow2((uint)(object)item),
    	    _ when typeof(T) == typeof(long) => BitOperations.IsPow2(unchecked((ulong)(long)(object)item)),
    	    _ when typeof(T) == typeof(ulong) => BitOperations.IsPow2((ulong)(object)item),
    	    _ when typeof(T) == typeof(nint) => BitOperations.IsPow2(unchecked((nuint)(nint)(object)item)),
    	    _ when typeof(T) == typeof(nuint) => BitOperations.IsPow2((nuint)(object)item),
    	    _ when !typeof(T).IsEnum => (Enumerator)item is var e && e.MoveNext() && !e.MoveNext(),
            _ => (typeof(T) == typeof(Enum) ? item.GetType() : typeof(T)).GetEnumUnderlyingType() switch
            {
    	        var x when x == typeof(byte) => BitOperations.IsPow2((byte)(object)item),
    	        var x when x == typeof(sbyte) => BitOperations.IsPow2(unchecked((byte)(sbyte)(object)item)),
    	        var x when x == typeof(short) => BitOperations.IsPow2(unchecked((ushort)(short)(object)item)),
    	        var x when x == typeof(ushort) => BitOperations.IsPow2((ushort)(object)item),
    	        var x when x == typeof(int) => BitOperations.IsPow2(unchecked((uint)(int)(object)item)),
    	        var x when x == typeof(uint) => BitOperations.IsPow2((uint)(object)item),
    	        var x when x == typeof(long) => BitOperations.IsPow2(unchecked((ulong)(long)(object)item)),
    	        var x when x == typeof(ulong) => BitOperations.IsPow2((ulong)(object)item),
    	        var x when x == typeof(nint) => BitOperations.IsPow2(unchecked((nuint)(nint)(object)item)),
    	        var x when x == typeof(nuint) => BitOperations.IsPow2((nuint)(object)item),
    	        _ => (Enumerator)item is var e && e.MoveNext() && !e.MoveNext(),
            },
    	};
#else
        (Enumerator)item is var e && e.MoveNext() && !e.MoveNext();
#endif

    /// <inheritdoc cref="ICollection{T}.Contains"/>
    [CollectionAccess(CollectionAccessType.Read), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public bool Contains(T item)
    {
    	if (!IsSingle(item))
            return false;

        And(_value, ref item);
        return !EqZero(item);
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
            if (IsSingle(next))
                Or(next, ref t);
            else
                return false;

        return Eq(_value, t);
    }
}
