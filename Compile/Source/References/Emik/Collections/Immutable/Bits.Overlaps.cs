// SPDX-License-Identifier: MPL-2.0
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
#pragma warning disable CS8500
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
    public static bool IsSingle(scoped in T item) =>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
        0 switch
        {
            _ when typeof(T) == typeof(sbyte) => unchecked((uint)(sbyte)(object)item).IsPow2(),
            _ when typeof(T) == typeof(byte) => ((uint)(byte)(object)item).IsPow2(),
            _ when typeof(T) == typeof(short) => unchecked((uint)(short)(object)item).IsPow2(),
            _ when typeof(T) == typeof(ushort) => ((uint)(ushort)(object)item).IsPow2(),
            _ when typeof(T) == typeof(int) => unchecked((uint)(int)(object)item).IsPow2(),
            _ when typeof(T) == typeof(uint) => ((uint)(object)item).IsPow2(),
            _ when typeof(T) == typeof(long) => unchecked((ulong)(long)(object)item).IsPow2(),
            _ when typeof(T) == typeof(ulong) => ((ulong)(object)item).IsPow2(),
            _ when typeof(T) == typeof(nint) => ((nuint)(nint)(object)item).IsPow2(),
            _ when typeof(T) == typeof(nuint) => ((nuint)(object)item).IsPow2(),
            _ when !typeof(T).IsEnum => (Enumerator)item is var e && e.MoveNext() && !e.MoveNext(),
            _ => (typeof(T) == typeof(Enum) ? item.GetType() : typeof(T)).GetEnumUnderlyingType() switch
            {
                var x when x == typeof(sbyte) => unchecked((uint)(sbyte)(object)item).IsPow2(),
                var x when x == typeof(byte) => ((uint)(byte)(object)item).IsPow2(),
                var x when x == typeof(short) => unchecked((uint)(short)(object)item).IsPow2(),
                var x when x == typeof(ushort) => ((uint)(ushort)(object)item).IsPow2(),
                var x when x == typeof(int) => unchecked((uint)(int)(object)item).IsPow2(),
                var x when x == typeof(uint) => ((uint)(object)item).IsPow2(),
                var x when x == typeof(long) => unchecked((ulong)(long)(object)item).IsPow2(),
                var x when x == typeof(ulong) => ((ulong)(object)item).IsPow2(),
                var x when x == typeof(nint) => ((nuint)(nint)(object)item).IsPow2(),
                var x when x == typeof(nuint) => ((nuint)(object)item).IsPow2(),
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

        And(bits, ref item);
        return !Eq0(item);
    }

    /// <inheritdoc cref="ISet{T}.IsProperSubsetOf" />
    [CollectionAccess(CollectionAccessType.Read), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public bool IsProperSubsetOf([InstantHandle] IEnumerable<T> other)
    {
        T t = default;

        var collection = other.ToICollection();

        foreach (var next in this)
            if (collection.Contains(next))
                Or(next, ref t);
            else
                return false;

        // ReSharper disable once LoopCanBeConvertedToQuery
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

        return !Eq(bits, t);
    }

    /// <inheritdoc cref="ISet{T}.IsSubsetOf" />
    [CollectionAccess(CollectionAccessType.Read), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public bool IsSubsetOf([InstantHandle] IEnumerable<T> other)
    {
        var collection = other.ToICollection();
        return this.All(collection.Contains);
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

        return Eq(bits, t);
    }
}
#endif
