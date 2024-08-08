// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

// ReSharper disable RedundantReadonlyModifier StructCanBeMadeReadOnly
using static CollectionAccessType;

/// <summary>Provides methods to calculate various binomial coefficients.</summary>
static partial class Choices
{
#if NET8_0_OR_GREATER
    /// <summary>Calculates the binomial coefficient (nCk) (N items, choose k).</summary>
    /// <remarks><para>
    /// Implementation based on <a href="https://stackoverflow.com/a/19125294/18052726">Moop's solution</a>.
    /// </para></remarks>
    /// <typeparam name="T">The type of the number.</typeparam>
    /// <param name="n">The number of items.</param>
    /// <param name="k">The number to choose.</param>
    /// <returns>
    /// <math><mrow><mo>(</mo><mfrac linethickness="0"><mi>n</mi><mi>k</mi></mfrac><mo>)</mo></mrow></math>
    /// </returns>
    [NonNegativeValue, Pure]
    public static T Choose<T>(this T n, T k)
        where T : INumberBase<T>
    {
        if (T.IsNegative(n)) // ReSharper disable once TailRecursiveCall
            return checked(T.IsEvenInteger(k) ? Choose(-n + T.One, k) : -Choose(-n + T.One + T.One, k));

        if (T.IsZero(k) || T.IsNegative(k) || T.IsNegative(n - k))
            return T.Zero;

        if (n == k)
            return T.One;

        var c = T.One;

        for (var i = T.One; T.IsPositive(k - i); i++)
            c = checked(c * n-- / i);

        return c;
    }
#else
    /// <summary>Calculates the binomial coefficient (nCk) (N items, choose k).</summary>
    /// <remarks><para>
    /// Implementation based on <a href="https://stackoverflow.com/a/19125294/18052726">Moop's solution</a>.
    /// </para></remarks>
    /// <param name="n">The number of items.</param>
    /// <param name="k">The number to choose.</param>
    /// <returns>
    /// <math><mrow><mo>(</mo><mfrac linethickness="0"><mi>n</mi><mi>k</mi></mfrac><mo>)</mo></mrow></math>
    /// </returns>
    [NonNegativeValue, Pure]
    public static int Choose(this int n, int k)
    {
        if (n < 0) // ReSharper disable once TailRecursiveCall
            return checked((k & 1) is 0 ? Choose(-n + 1, k) : -Choose(-n + 2, k));

        if (n < k || k <= 0)
            return 0;

        if (n == k)
            return 1;

        var c = 1;

        for (var i = 1; i <= k; i++)
            c = checked(c * n-- / i);

        return c;
    }

    /// <inheritdoc cref="Choose(int, int)"/>
    [NonNegativeValue, Pure]
    public static long Choose(this long n, long k)
    {
        if (n < 0) // ReSharper disable once TailRecursiveCall
            return checked((k & 1) is 0 ? Choose(-n + 1, k) : -Choose(-n + 2, k));

        if (n < k || k <= 0)
            return 0;

        if (n == k)
            return 1;

        long c = 1;

        for (long i = 1; i <= k; i++)
            c = checked(c * n-- / i);

        return c;
    }
#endif

    /// <summary>Calculates the binomial coefficient (nCk) (N items, choose k).</summary>
    /// <typeparam name="T">The type of items to choose from.</typeparam>
    /// <param name="n">The items to choose from.</param>
    /// <param name="k">The amount of items to choose.</param>
    /// <returns>
    /// The <see cref="ICollection{T}"/> of <see cref="IList{T}"/> containing the binomial coefficients.
    /// </returns>
    [Pure]
    public static Choices<T> Choose<T>(this IEnumerable<T>? n, int k) => new(n.ToIList(), k);
}

/// <summary>Provides methods to calculate various binomial coefficients.</summary>
/// <typeparam name="T">The type of element.</typeparam>
/// <param name="n">The collection to choose from.</param>
/// <param name="k">The number to choose.</param>
[StructLayout(LayoutKind.Auto)]
#if CSHARPREPL
public
#endif
#if !NO_READONLY_STRUCTS
readonly
#endif
#pragma warning disable CA1710, IDE0250, SA1137 // ReSharper disable once BadPreprocessorIndent
    struct Choices<T>(IList<T>? n, int k) : ICollection<IList<T>>, IEquatable<Choices<T>>
#pragma warning restore CA1710, IDE0250, SA1137
{
    /// <summary>Provides the enumerator for the <see cref="Choices{T}"/> struct.</summary>
    /// <param name="n">The collection to choose from.</param>
    /// <param name="k">The number to choose.</param>
    [StructLayout(LayoutKind.Auto)]
    public struct Enumerator(IList<T>? n, int k) : IEnumerator<IList<T>>
    {
        bool
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
            _hasDisposed,
#endif
            _hasMoved;

        int[] _values = Rent(n, k);

        /// <inheritdoc cref="Choices{T}.K"/>
        [NonNegativeValue, Pure]
        public int K { get; } = Math.Max(k, 0);

        /// <inheritdoc/>
        [Pure]
        public IList<T> Current { get; private set; } = [];

        /// <inheritdoc cref="Choices{T}.N"/>
        [Pure]
        public readonly IList<T> N { get; } = n ?? [];

        /// <inheritdoc />
        [Pure]
        readonly object IEnumerator.Current => Current;

        /// <summary>Resets the provided array to the initial state.</summary>
        /// <param name="values">The array to fill.</param>
        /// <param name="k">
        /// The length of the area to fill, assumed to be at least
        /// the length of the parameter <paramref name="values"/>.
        /// </param>
        /// <returns>The parameter <paramref name="values"/>.</returns>
        public static int[] Reset(int[] values, int k)
        {
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
            values.AsSpan()[..k].Range();
#else
            for (var i = 0; i < k; i++)
                values[i] = i;
#endif
            return values;
        }

        /// <inheritdoc />
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
        public void Dispose()
        {
            if (_hasDisposed || _values is null or [])
                return;

            ArrayPool<int>.Shared.Return(_values);
            _hasDisposed = true;
            _values = [];
        }
#else
        public readonly void Dispose() { }
#endif

        /// <inheritdoc />
        public void Reset()
        {
            _hasMoved = false;
            Reset(_values ??= new int[K], K);
        }

        /// <inheritdoc />
        public bool MoveNext()
        {
            if (EarlyReturn() is { } next)
                return next;

            for (var i = K - 1; i >= 0; i--)
                if (Found(i))
                    return true;

            return false;
        }

        [Pure]
        static int[] Rent(IList<T>? n, int k) =>
            n is not { Count: not 0 and var count } || count <= k
                ? []
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
                : Reset(ArrayPool<int>.Shared.Rent(k), k);
#else
                : Reset(new int[k], k);
#endif
        void Copy()
        {
            Current = new T[K];

            for (var i = 0; i < K; i++)
                Current[i] = N[_values[i]];
        }

        [MustUseReturnValue]
        bool Found(int found)
        {
            if (_values[found] + 1 is var next && next >= N.Count - (K - found - 1))
                return false;
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
            _values.AsSpan()[found..].Range(next);
#else
            for (var i = found; i < K; i++)
                _values[i] = next + i - found;
#endif
            Copy();
            return true;
        }

        [MustUseReturnValue]
        bool? EarlyReturn()
        {
            if (K is 0 || N is not { Count: not 0 and var count } || count < K)
                return false;

            if (K == count)
                return (Current = N) is var _ && !_hasMoved && (_hasMoved = true);

            if (_hasMoved)
                return null;

            Copy();
            _hasMoved = true;
            return true;
        }
    }

    /// <inheritdoc />
    [CollectionAccess(None), Pure]
    readonly bool ICollection<IList<T>>.IsReadOnly => true;

    /// <inheritdoc cref="ICollection{T}.Count"/>
    [CollectionAccess(None), NonNegativeValue, Pure]
    public readonly int Count => N.Count.Choose(K);

    /// <summary>Gets the number of choices.</summary>
    [CollectionAccess(None), NonNegativeValue, Pure]
    public int K { get; } = Math.Max(k, 0);

    /// <summary>Gets the list of choices.</summary>
    [CollectionAccess(Read), Pure]
    public IList<T> N { get; } = n ?? [];

    /// <summary>Gets the first <see cref="K"/> choices.</summary>
    [CollectionAccess(Read), Pure]
    public readonly IEnumerable<T> First =>
        N.Count is var count && count < K ? [] :
        count == K ? N : N.Take(K);

    /// <summary>Gets the last <see cref="K"/> choices.</summary>
    [CollectionAccess(Read), Pure]
    public readonly IEnumerable<T> Last =>
        N.Count is var count && count < K ? [] :
        count == K ? N : N.Skip(N.Count - K);

    /// <summary>Determines whether the specified objects are equal.</summary>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>Whether the specified objects are equal.</returns>
    public static bool operator ==(Choices<T> left, Choices<T> right) => left.Equals(right);

    /// <summary>Determines whether the specified objects are unequal.</summary>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>Whether the specified objects are unequal.</returns>
    public static bool operator !=(Choices<T> left, Choices<T> right) => !(left == right);

    /// <inheritdoc />
    [CollectionAccess(None)]
    readonly void ICollection<IList<T>>.Add(IList<T> item) { }

    /// <inheritdoc />
    [CollectionAccess(None)]
    readonly void ICollection<IList<T>>.Clear() { }

    /// <inheritdoc />
    [CollectionAccess(Read)]
    public readonly void CopyTo(IList<T>[] array, int arrayIndex)
    {
        foreach (var next in this)
            array[arrayIndex++] = next;
    }

    /// <inheritdoc />
    [CollectionAccess(Read), Pure]
    public readonly bool Contains(IList<T> item) => IndexOf(item) is not -1;

    /// <inheritdoc />
    public readonly override bool Equals(object? obj) => obj is Choices<T> other && Equals(other);

    /// <inheritdoc />
    public readonly bool Equals(Choices<T> other) => K == other.K && N.Equals(other.N);

    /// <inheritdoc />
    [CollectionAccess(None), Pure]
    readonly bool ICollection<IList<T>>.Remove(IList<T> item) => false;

    /// <inheritdoc cref="IList{T}.IndexOf"/>
    [CollectionAccess(Read), Pure]
    public readonly int IndexOf(IList<T> item)
    {
        if (N.Count == K)
            return N.Equals(item) ? 0 : -1;

        var i = 0;

        foreach (var next in this)
            if (next.Equals(item))
                return i;
            else
                i++;

        return -1;
    }

    /// <inheritdoc />
    public readonly override int GetHashCode() => unchecked(K * 397 ^ N.GetHashCode());

    /// <inheritdoc />
    public readonly override string ToString()
    {
#if NET6_0_OR_GREATER
        var count = Count;
        DefaultInterpolatedStringHandler str = new(count is 0 ? 2 : count * 4, count);
        str.AppendLiteral("[");
#else
        StringBuilder str = new("[");
#endif
        using var e = GetEnumerator();

        if (!e.MoveNext())
            goto Done;
#if NET6_0_OR_GREATER
        str.AppendLiteral("[");
#else
        str.Append('[');
#endif
        str.AppendMany(e.Current);
#if NET6_0_OR_GREATER
        str.AppendLiteral("]");
#else
        str.Append(']');
#endif
        while (e.MoveNext())
        {
#if NET6_0_OR_GREATER
            str.AppendLiteral(", [");
#else
            str.Append(", [");
#endif
            str.AppendMany(e.Current);
#if NET6_0_OR_GREATER
            str.AppendLiteral("]");
#else
            str.Append(']');
#endif
        }

    Done:
#if NET6_0_OR_GREATER
        str.AppendLiteral("]");
        return str.ToStringAndClear();
#else
        return $"{str.Append(']')}";
#endif
    }

    /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
    [CollectionAccess(Read), Pure]
    public readonly Enumerator GetEnumerator() => new(N, K);

    /// <inheritdoc />
    [CollectionAccess(Read), Pure]
    readonly IEnumerator<IList<T>> IEnumerable<IList<T>>.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    [CollectionAccess(Read), Pure]
    readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
