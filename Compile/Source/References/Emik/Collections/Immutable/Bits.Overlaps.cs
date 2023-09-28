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
    /// <summary>Determines whether both pointers of <typeparamref name="T"/> contain the same bits.</summary>
    /// <remarks><para>This method assumes the pointers are fixed.</para></remarks>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>
    /// The value <see langword="true"/> if the parameters <paramref name="left"/> and <paramref name="right"/>
    /// point to values with the same bits as each other; otherwise, <see langword="false"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)] // ReSharper disable once CognitiveComplexity
    public static unsafe bool Eq(T* left, T* right)
    {
        byte* l = (byte*)left, r = (byte*)right, upper = (byte*)(left + 1);
#if NET8_0_OR_GREATER
        if (Vector512.IsHardwareAccelerated && sizeof(T) >= 64)
        {
            for (; l <= upper - 64; l += 64, r += 64)
                if (!Vector512.EqualsAll(Vector512.Load(l), Vector512.Load(r)))
                    return false;

            if (sizeof(T) % 64 is 0)
                return true;
        }
#endif
#if NETCOREAPP3_0_OR_GREATER
        if (Vector256.IsHardwareAccelerated && sizeof(T) >= 32)
        {
            for (; l <= upper - 32; l += 32, r += 32)
                if (!Vector256.EqualsAll(Vector256.Load(l), Vector256.Load(r)))
                    return false;

            if (sizeof(T) % 32 is 0)
                return true;
        }

        if (Vector128.IsHardwareAccelerated && sizeof(T) >= 16)
        {
            for (; l <= upper - 16; l += 16, r += 16)
                if (!Vector128.EqualsAll(Vector128.Load(l), Vector128.Load(r)))
                    return false;

            if (sizeof(T) % 16 is 0)
                return true;
        }

        if (Vector64.IsHardwareAccelerated && sizeof(T) >= 8)
        {
            for (; l <= upper - 8; l += 8, r += 8)
                if (!Vector64.EqualsAll(Vector64.Load(l), Vector64.Load(r)))
                    return false;

            if (sizeof(T) % 8 is 0)
                return true;
        }
#endif
        for (; l <= upper - nint.Size; l += nint.Size, r += nint.Size)
            if (*(nuint*)l != *(nuint*)r)
                return false;

        if (sizeof(T) % sizeof(nuint) is 0)
            return true;

        for (; l < upper; l++, r++)
            if (*l != *r)
                return false;

        return true;
    }

    /// <summary>Computes the Bitwise-OR computation, writing it to the second argument.</summary>
    /// <remarks><para>This method assumes the pointers are fixed.</para></remarks>
    /// <param name="read">The <typeparamref name="T"/> to read from.</param>
    /// <param name="write">The <typeparamref name="T"/> to write to.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)] // ReSharper disable once CognitiveComplexity
    public static unsafe void Or(T* read, T* write)
    {
        byte* l = (byte*)read, r = (byte*)write, upper = (byte*)(read + 1);
#if NET8_0_OR_GREATER
        if (Vector512.IsHardwareAccelerated && sizeof(T) >= 64)
        {
            for (; l <= upper - 64; l += 64, r += 64)
                Vector512.BitwiseOr(Vector512.Load(l), Vector512.Load(r)).StoreAligned(r);

            if (sizeof(T) % 64 is 0)
                return;
        }
#endif
#if NETCOREAPP3_0_OR_GREATER
        if (Vector256.IsHardwareAccelerated && sizeof(T) >= 32)
        {
            for (; l <= upper - 32; l += 32, r += 32)
                Vector256.BitwiseOr(Vector256.Load(l), Vector256.Load(r)).StoreAligned(r);

            if (sizeof(T) % 32 is 0)
                return;
        }

        if (Vector128.IsHardwareAccelerated && sizeof(T) >= 16)
        {
            for (; l <= upper - 16; l += 16, r += 16)
                Vector128.BitwiseOr(Vector128.Load(l), Vector128.Load(r)).StoreAligned(r);

            if (sizeof(T) % 16 is 0)
                return;
        }

        if (Vector64.IsHardwareAccelerated && sizeof(T) >= 8)
        {
            for (; l <= upper - 8; l += 8, r += 8)
                Vector64.BitwiseOr(Vector64.Load(l), Vector64.Load(r)).StoreAligned(r);

            if (sizeof(T) % 8 is 0)
                return;
        }
#endif
        for (; l <= upper - nuint.Size; l += nuint.Size, r += nuint.Size)
            *(nuint*)r = *(nuint*)l | *(nuint*)r;

        if (sizeof(T) % nuint.Size is 0)
            return;

        for (; l < upper; l++, r++)
            *r = (byte)(*l | *r);
    }

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
}
