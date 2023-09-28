// SPDX-License-Identifier: MPL-2.0

// ReSharper disable CheckNamespace StructCanBeMadeReadOnly
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
    public unsafe bool Contains(T item)
    {
        using var that = GetEnumerator();

        while (that.MoveNext())
        {
            var current = that.Current;

            if (Eq(&current, &item))
                return true;
        }

        return false;
    }

    /// <inheritdoc cref="ISet{T}.IsProperSubsetOf" />
    [CollectionAccess(CollectionAccessType.Read), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public unsafe bool IsProperSubsetOf([InstantHandle] IEnumerable<T> other)
    {
        T t = default;
        var collection = other.ToCollectionLazily();

        // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
        foreach (var next in this)
            if (collection.Contains(next))
                Or(&next, &t);
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
    public unsafe bool IsProperSupersetOf([InstantHandle] IEnumerable<T> other)
    {
        T t = default;

        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach (var next in other)
            if (Contains(next))
                Or(&next, &t);
            else
                return false;

        fixed (T* ptr = &_value)
            return !Eq(ptr, &t);
    }

    /// <inheritdoc cref="ISet{T}.IsSubsetOf" />
    [CollectionAccess(CollectionAccessType.Read), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public bool IsSubsetOf([InstantHandle] IEnumerable<T> other)
    {
        var collection = other.ToCollectionLazily();

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
    public unsafe bool SetEquals([InstantHandle] IEnumerable<T> other)
    {
        T t = default;

        foreach (var next in other)
            if (new Enumerator(next) is var e && !e.MoveNext() || e.MoveNext())
                return false;
            else
                Or(&next, &t);

        fixed (T* ptr = &_value)
            return Eq(ptr, &t);
    }

    // ReSharper disable once CognitiveComplexity
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static unsafe void Or(T* left, T* right)
    {
        var i = 0;
        var l = (byte*)left;
        var r = (byte*)right;
#if NET8_0_OR_GREATER
        if (Vector512.IsHardwareAccelerated && sizeof(Vector512<nuint>) <= sizeof(T))
            for (; i <= sizeof(T) - sizeof(Vector512<nuint>); i += sizeof(Vector512<nuint>))
                Vector512.BitwiseOr(Vector512.Load(l + i), Vector512.Load(r + i)).StoreAligned(r + i);
#endif
#if NETCOREAPP3_0_OR_GREATER
        if (Vector256.IsHardwareAccelerated && sizeof(Vector256<nuint>) <= sizeof(T))
            for (; i <= sizeof(T) - sizeof(Vector256<nuint>); i += sizeof(Vector256<nuint>))
                Vector256.BitwiseOr(Vector256.Load(l + i), Vector256.Load(r + i)).StoreAligned(r + i);

        if (Vector128.IsHardwareAccelerated && sizeof(Vector128<nuint>) <= sizeof(T))
            for (; i <= sizeof(T) - sizeof(Vector128<nuint>); i += sizeof(Vector128<nuint>))
                Vector128.BitwiseOr(Vector128.Load(l + i), Vector128.Load(r + i)).StoreAligned(r + i);

        if (Vector64.IsHardwareAccelerated && sizeof(Vector64<nuint>) <= sizeof(T))
            for (; i <= sizeof(T) - sizeof(Vector64<nuint>); i += sizeof(Vector64<nuint>))
                Vector64.BitwiseOr(Vector64.Load(l + i), Vector64.Load(r + i)).StoreAligned(r + i);
#endif
        for (; i <= sizeof(T) - sizeof(nuint); i++)
            *(nuint*)r = *(nuint*)(l + i) | *(nuint*)(r + i);

        if (sizeof(T) % sizeof(nuint) is 0)
            return;

        for (; i < sizeof(T); i++)
            r[i] = (byte)(l[i] | r[i]);
    }

    // ReSharper disable once CognitiveComplexity
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static unsafe bool Eq(T* left, T* right)
    {
        var i = 0;
        var l = (byte*)left;
        var r = (byte*)right;
#if NET8_0_OR_GREATER
        if (Vector512.IsHardwareAccelerated && sizeof(Vector512<nuint>) <= sizeof(T))
            for (; i <= sizeof(T) - sizeof(Vector512<nuint>); i += sizeof(Vector512<nuint>))
                if (!Vector512.EqualsAll(Vector512.Load(l + i), Vector512.Load(r + i)))
                    return false;
#endif
#if NETCOREAPP3_0_OR_GREATER
        if (Vector256.IsHardwareAccelerated && sizeof(Vector256<nuint>) <= sizeof(T))
            for (; i <= sizeof(T) - sizeof(Vector256<nuint>); i += sizeof(Vector256<nuint>))
                if (!Vector256.EqualsAll(Vector256.Load(l + i), Vector256.Load(r + i)))
                    return false;

        if (Vector128.IsHardwareAccelerated && sizeof(Vector128<nuint>) <= sizeof(T))
            for (; i <= sizeof(T) - sizeof(Vector128<nuint>); i += sizeof(Vector128<nuint>))
                if (!Vector128.EqualsAll(Vector128.Load(l + i), Vector128.Load(r + i)))
                    return false;

        if (Vector64.IsHardwareAccelerated && sizeof(Vector64<nuint>) <= sizeof(T))
            for (; i <= sizeof(T) - sizeof(Vector64<nuint>); i += sizeof(Vector64<nuint>))
                if (!Vector64.EqualsAll(Vector64.Load(l + i), Vector64.Load(r + i)))
                    return false;
#endif
        for (; i <= sizeof(T) - sizeof(nuint); i += sizeof(nuint))
            if (*((nuint*)l + i) != *((nuint*)r + i))
                return false;

        if (sizeof(T) % sizeof(nuint) is 0)
            return true;

        for (; i < sizeof(T); i++)
            if (l[i] != r[i])
                return false;

        return true;
    }
}
