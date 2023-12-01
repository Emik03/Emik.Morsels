// SPDX-License-Identifier: MPL-2.0

// ReSharper disable BadPreprocessorIndent CheckNamespace ConvertToAutoPropertyWhenPossible ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator InvertIf RedundantExtendsListEntry RedundantNameQualifier RedundantReadonlyModifier RedundantUsingDirective StructCanBeMadeReadOnly UseSymbolAlias

namespace Emik.Morsels;
#pragma warning disable 8618, 9193, CA1823, IDE0250, MA0071, MA0102, SA1137
using static Span;
using static SplitMemoryFactory;
using static SplitSpanFactory;

/// <summary>Methods to split spans into multiple spans.</summary>
#pragma warning disable MA0048
static partial class SplitMemoryFactory
#pragma warning restore MA0048
{
    /// <summary>
    /// Defines the values for <see cref="SplitMemory{TBody, TSeparator, TStrategy}"/> without a compile-time strategy.
    /// </summary>
    /// <typeparam name="TBody">The type of element from the span.</typeparam>
    /// <typeparam name="TSeparator">The type of separator.</typeparam>
    public interface ISplitMemory<TBody, TSeparator> : IEnumerable<ReadOnlyMemory<TBody>>
    {
        /// <summary>Gets the body.</summary>
        public ReadOnlyMemory<TBody> Body { [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get; }

        /// <summary>Gets the separator.</summary>
        public ReadOnlyMemory<TSeparator> Separator { [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get; }
    }

    /// <summary>Determines whether both splits are eventually equal when concatenating all slices.</summary>
    /// <typeparam name="TBody">The type of element from the span.</typeparam>
    /// <typeparam name="TLeftSeparator">The type of separator for the left-hand side.</typeparam>
    /// <typeparam name="TLeftStrategy">The strategy for splitting elements for the left-hand side.</typeparam>
    /// <typeparam name="TRightSeparator">The type of separator for the right-hand side.</typeparam>
    /// <typeparam name="TRightStrategy">The strategy for splitting elements for the right-hand side.</typeparam>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>
    /// The value <paramref langword="true"/> if both sequences are equal, otherwise; <paramref langword="false"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool ConcatEqual<TBody, TLeftSeparator, TLeftStrategy, TRightSeparator, TRightStrategy>(
        this scoped in SplitMemory<TBody, TLeftSeparator, TLeftStrategy> left,
        scoped in SplitMemory<TBody, TRightSeparator, TRightStrategy> right
    )
#if UNMANAGED_SPAN
        where TBody : unmanaged, IEquatable<TBody>
#else
        where TBody : IEquatable<TBody>?
#endif
#if !NET7_0_OR_GREATER
        where TLeftSeparator : IEquatable<TLeftSeparator>?
        where TRightSeparator : IEquatable<TRightSeparator>?
#endif
    {
        if (left.GetEnumerator() is var e1 && right.GetEnumerator() is var e2 && !e1.MoveNext())
            return !e2.MoveNext();

        if (!e2.MoveNext())
            return false;

        ReadOnlyMemory<TBody>
            reader1 = e1.Current,
            reader2 = e2.Current;

        while (true)
            if (Next(ref reader1, ref reader2, ref e1, ref e2, out var ret))
                return ret;
    }

    /// <summary>Determines whether both splits are equal.</summary>
    /// <typeparam name="TBody">The type of element from the span.</typeparam>
    /// <typeparam name="TLeftSeparator">The type of separator for the left-hand side.</typeparam>
    /// <typeparam name="TLeftStrategy">The strategy for splitting elements for the left-hand side.</typeparam>
    /// <typeparam name="TRightSeparator">The type of separator for the right-hand side.</typeparam>
    /// <typeparam name="TRightStrategy">The strategy for splitting elements for the right-hand side.</typeparam>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>
    /// The value <paramref langword="true"/> if both sequences are equal, otherwise; <paramref langword="false"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool SequenceEqual<TBody, TLeftSeparator, TLeftStrategy, TRightSeparator, TRightStrategy>(
        this scoped in SplitMemory<TBody, TLeftSeparator, TLeftStrategy> left,
        scoped in SplitMemory<TBody, TRightSeparator, TRightStrategy> right
    )
#if UNMANAGED_SPAN
        where TBody : unmanaged, IEquatable<TBody>
#else
        where TBody : IEquatable<TBody>?
#endif
#if !NET7_0_OR_GREATER
        where TLeftSeparator : IEquatable<TLeftSeparator>?
        where TRightSeparator : IEquatable<TRightSeparator>?
#endif
    {
        var e1 = left.GetEnumerator();
        var e2 = right.GetEnumerator();

        while (e1.MoveNext())
            if (!(e2.MoveNext() && e1.Current.SequenceEqual(e2.Current)))
                return false;

        return !e2.MoveNext();
    }

    /// <summary>Splits a span by the specified separator.</summary>
    /// <typeparam name="T">The type of element from the span.</typeparam>
    /// <param name="span">The span to split.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>The enumerable object that references the parameter <paramref name="span"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<T, T, MatchAny> SplitAny<T>(this ReadOnlyMemory<T> span, ReadOnlyMemory<T> separator)
#if UNMANAGED_SPAN
        where T : unmanaged, IEquatable<T>
#else
        where T : IEquatable<T>
#endif
        =>
            new(span, separator);

    /// <inheritdoc cref="SplitAny{T}(ReadOnlyMemory{T}, ReadOnlyMemory{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<T, T, MatchAny> SplitAny<T>(this Memory<T> span, ReadOnlyMemory<T> separator)
#if UNMANAGED_SPAN
        where T : unmanaged, IEquatable<T>
#else
        where T : IEquatable<T>
#endif
        =>
            ((ReadOnlyMemory<T>)span).SplitAny(separator);

    /// <summary>Splits a span by the specified separator.</summary>
    /// <typeparam name="T">The type of element from the span.</typeparam>
    /// <param name="span">The span to split.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>The enumerable object that references the parameter <paramref name="span"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<T, T, MatchAll> SplitAll<T>(this ReadOnlyMemory<T> span, ReadOnlyMemory<T> separator)
#if UNMANAGED_SPAN
        where T : unmanaged, IEquatable<T>
#else
        where T : IEquatable<T>
#endif
        =>
            new(span, separator);

    /// <inheritdoc cref="SplitAll{T}(ReadOnlyMemory{T}, ReadOnlyMemory{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<T, T, MatchAll> SplitAll<T>(this Memory<T> span, ReadOnlyMemory<T> separator)
#if UNMANAGED_SPAN
        where T : unmanaged, IEquatable<T>
#else
        where T : IEquatable<T>
#endif
        =>
            ((ReadOnlyMemory<T>)span).SplitAll(separator);
#if NET8_0_OR_GREATER
    /// <inheritdoc cref="SplitAny{T}(ReadOnlyMemory{T}, ReadOnlyMemory{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<T, SearchValues<T>, MatchAny> SplitOn<T>(
        this ReadOnlyMemory<T> span,
        in OnceMemoryManager<SearchValues<T>> separator
    )
        where T : IEquatable<T> =>
        new(span, separator.Memory);

    /// <inheritdoc cref="SplitAny{T}(ReadOnlyMemory{T}, ReadOnlyMemory{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<T, SearchValues<T>, MatchAny> SplitOn<T>(
        this Memory<T> span,
        OnceMemoryManager<SearchValues<T>> separator
    )
        where T : IEquatable<T> =>
        ((ReadOnlyMemory<T>)span).SplitOn(separator);
#endif
#if NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP
    /// <inheritdoc cref="SplitAny{T}(ReadOnlyMemory{T}, ReadOnlyMemory{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<T, T, MatchOne> SplitOn<T>(this ReadOnlyMemory<T> span, in OnceMemoryManager<T> separator)
#if UNMANAGED_SPAN
        where T : unmanaged, IEquatable<T>
#else
        where T : IEquatable<T>
#endif
        =>
            new(span, separator.Memory);

    /// <inheritdoc cref="SplitAny{T}(ReadOnlyMemory{T}, ReadOnlyMemory{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<T, T, MatchOne> SplitOn<T>(this Memory<T> span, in OnceMemoryManager<T> separator)
#if UNMANAGED_SPAN
        where T : unmanaged, IEquatable<T>
#else
        where T : IEquatable<T>
#endif
        =>
            ((ReadOnlyMemory<T>)span).SplitOn(separator);
#endif

    /// <summary>Copies the values to a new <see cref="List{T}"/>.</summary>
    /// <typeparam name="TSeparator">The type of separator.</typeparam>
    /// <typeparam name="TStrategy">The strategy for splitting elements.</typeparam>
    /// <param name="split">The instance to get the list from.</param>
    /// <returns>The list containing the copied values of this instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static List<string> ToList<TSeparator, TStrategy>(
        this scoped in SplitMemory<char, TSeparator, TStrategy> split
    )
#if !NET7_0_OR_GREATER
        where TSeparator : IEquatable<TSeparator>?
#endif
    {
        List<string> ret = [];

        foreach (var next in split)
            ret.Add(next.ToString());

        return ret;
    }

    /// <summary>Copies the values to a new <see cref="List{T}"/>.</summary>
    /// <typeparam name="TBody">The type of element from the span.</typeparam>
    /// <typeparam name="TSeparator">The type of separator.</typeparam>
    /// <typeparam name="TStrategy">The strategy for splitting elements.</typeparam>
    /// <param name="split">The instance to get the list from.</param>
    /// <returns>The list containing the copied values of this instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static List<TBody[]> ToList<TBody, TSeparator, TStrategy>(
        this scoped in SplitMemory<TBody, TSeparator, TStrategy> split
    )
#if UNMANAGED_SPAN
        where TBody : unmanaged, IEquatable<TBody>
#else
        where TBody : IEquatable<TBody>?
#endif
#if !NET7_0_OR_GREATER
        where TSeparator : IEquatable<TSeparator>?
#endif
    {
        List<TBody[]> ret = [];

        foreach (var next in split)
            ret.Add(next.ToArray());

        return ret;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool Next<TBody, TLeftSeparator, TLeftStrategy, TRightSeparator, TRightStrategy>(
        ref ReadOnlyMemory<TBody> reader1,
        ref ReadOnlyMemory<TBody> reader2,
        ref SplitMemory<TBody, TLeftSeparator, TLeftStrategy>.Enumerator e1,
        ref SplitMemory<TBody, TRightSeparator, TRightStrategy>.Enumerator e2,
        out bool ret
    )
#if UNMANAGED_SPAN
        where TBody : unmanaged, IEquatable<TBody>?
#else
        where TBody : IEquatable<TBody>?
#endif
#if !NET7_0_OR_GREATER
        where TLeftSeparator : IEquatable<TLeftSeparator>?
        where TRightSeparator : IEquatable<TRightSeparator>?
#endif
    {
        Unsafe.SkipInit(out ret);

        if (reader1.Length is var length1 && reader2.Length is var length2 && length1 == length2)
            return SameLength(ref reader1, ref reader2, ref e1, ref e2, ref ret);

        if (length1 < length2)
        {
            if (!reader1.SequenceEqual(reader2[..length1]) || !e1.MoveNext())
            {
                ret = false;
                return true;
            }

            reader1 = e1.Current;
            reader2 = reader2[length1..];
            return false;
        }

        if (!reader1[..length2].SequenceEqual(reader2) || !e2.MoveNext())
        {
            ret = false;
            return true;
        }

        reader1 = reader1[length2..];
        reader2 = e2.Current;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool SameLength<TBody, TLeftSeparator, TLeftStrategy, TRightSeparator, TRightStrategy>(
        ref ReadOnlyMemory<TBody> reader1,
        ref ReadOnlyMemory<TBody> reader2,
        ref SplitMemory<TBody, TLeftSeparator, TLeftStrategy>.Enumerator e1,
        ref SplitMemory<TBody, TRightSeparator, TRightStrategy>.Enumerator e2,
        ref bool ret
    )
        where TBody : IEquatable<TBody>?
#if !NET7_0_OR_GREATER
        where TLeftSeparator : IEquatable<TLeftSeparator>?
        where TRightSeparator : IEquatable<TRightSeparator>?
#endif
    {
        if (!reader1.SequenceEqual(reader2))
        {
            ret = false;
            return true;
        }

        if (!e1.MoveNext())
        {
            ret = !e2.MoveNext();
            return true;
        }

        if (!e2.MoveNext())
        {
            ret = false;
            return true;
        }

        reader1 = e1.Current;
        reader2 = e2.Current;
        return false;
    }
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
    /// <inheritdoc cref="SplitAny{T}(ReadOnlyMemory{T}, ReadOnlyMemory{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<char, char, MatchAny> SplitAny(this string span, string separator) =>
        span.AsMemory().SplitAny(separator.AsMemory());

    /// <inheritdoc cref="SplitAny{T}(ReadOnlyMemory{T}, ReadOnlyMemory{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<char, char, MatchAny> SplitAny(this string span, ReadOnlyMemory<char> separator) =>
        span.AsMemory().SplitAny(separator);

    /// <inheritdoc cref="SplitAll{T}(ReadOnlyMemory{T}, ReadOnlyMemory{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<char, char, MatchAll> SplitAll(this string span, string separator) =>
        span.AsMemory().SplitAll(separator.AsMemory());

    /// <inheritdoc cref="SplitAll{T}(ReadOnlyMemory{T}, ReadOnlyMemory{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<char, char, MatchAll> SplitAll(this string span, ReadOnlyMemory<char> separator) =>
        span.AsMemory().SplitAll(separator);

    /// <inheritdoc cref="SplitLines(ReadOnlyMemory{char})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<char,
#if NET8_0_OR_GREATER
        SearchValues<char>,
#else
        char,
#endif
        MatchAny> SplitLines(this string span) =>
        span.AsMemory().SplitLines();

    /// <summary>Splits a span by line breaks.</summary>
    /// <remarks><para>Line breaks are considered any character in <see cref="Whitespaces.Breaking"/>.</para></remarks>
    /// <param name="span">The span to split.</param>
    /// <returns>The enumerable object that references the parameter <paramref name="span"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<char,
#if NET8_0_OR_GREATER
        SearchValues<char>,
#else
        char,
#endif
        MatchAny> SplitLines(this ReadOnlyMemory<char> span) =>
#if NET8_0_OR_GREATER
        new(span, Whitespaces.BreakingSearchMemory);
#else
        new(span, Whitespaces.Breaking.AsMemory());
#endif

    /// <inheritdoc cref="SplitLines(ReadOnlyMemory{char})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<char,
#if NET8_0_OR_GREATER
        SearchValues<char>,
#else
        char,
#endif
        MatchAny> SplitLines(this Memory<char> span) =>
        ((ReadOnlyMemory<char>)span).SplitLines();

    // /// <inheritdoc cref="SplitAny{T}(ReadOnlyMemory{T}, ReadOnlyMemory{T})"/>
    // [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    // public static SplitMemory<char, char, One> SplitOn(this string span, in char separator) =>
    //     span.AsMemory().SplitOn(separator);

    /// <inheritdoc cref="SplitWhitespace(ReadOnlyMemory{char})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<char,
#if NET8_0_OR_GREATER
        SearchValues<char>,
#else
        char,
#endif
        MatchAny> SplitWhitespace(this string span) =>
        span.AsMemory().SplitWhitespace();

    /// <summary>Splits a span by whitespace.</summary>
    /// <remarks><para>Whitespace is considered any character in <see cref="Whitespaces.Unicode"/>.</para></remarks>
    /// <param name="span">The span to split.</param>
    /// <returns>The enumerable object that references the parameter <paramref name="span"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<char,
#if NET8_0_OR_GREATER
        SearchValues<char>,
#else
        char,
#endif
        MatchAny> SplitWhitespace(this ReadOnlyMemory<char> span) =>
#if NET8_0_OR_GREATER
        new(span, Whitespaces.UnicodeSearchMemory);
#else
        new(span, Whitespaces.Unicode.AsMemory());
#endif

    /// <inheritdoc cref="SplitWhitespace(ReadOnlyMemory{char})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<char,
#if NET8_0_OR_GREATER
        SearchValues<char>,
#else
        char,
#endif
        MatchAny> SplitWhitespace(this Memory<char> span) =>
        ((ReadOnlyMemory<char>)span).SplitWhitespace();
#endif
#if NET8_0_OR_GREATER
    /// <inheritdoc cref="SplitAny{T}(ReadOnlyMemory{T}, ReadOnlyMemory{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<char, SearchValues<char>, MatchAny> SplitOn(
        this string span,
        in OnceMemoryManager<SearchValues<char>> separator
    ) =>
        span.AsMemory().SplitOn(separator);
#endif
}

/// <summary>Represents a split entry.</summary>
/// <typeparam name="TBody">The type of element from the span.</typeparam>
/// <typeparam name="TSeparator">The type of separator.</typeparam>
/// <typeparam name="TStrategy">The strategy for splitting elements.</typeparam>
[StructLayout(LayoutKind.Auto)]
#if CSHARPREPL
public
#endif
#if !NO_READONLY_STRUCTS
readonly
#endif
    partial struct SplitMemory<TBody, TSeparator, TStrategy>(
        ReadOnlyMemory<TBody> body,
        ReadOnlyMemory<TSeparator> separator
    ) : IEquatable<object>,
    IEquatable<ISplitMemory<TBody, TSeparator>>,
    IEquatable<SplitMemory<TBody, TSeparator, TStrategy>>,
    ISplitMemory<TBody, TSeparator>
#if UNMANAGED_SPAN
    where T : unmanaged, IEquatable<T>?
#else
    where TBody : IEquatable<TBody>?
#endif
#if !NET7_0_OR_GREATER
    where TSeparator : IEquatable<TSeparator>?
#endif
{
    readonly ReadOnlyMemory<TBody> _body = body;

    readonly ReadOnlyMemory<TSeparator> _separator = separator;

    /// <summary>
    /// Initializes a new instance of the <see cref="SplitMemory{TBody, TSeparator, TStrategy}"/> struct.
    /// </summary>
    /// <param name="body">The line to split.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SplitMemory(ReadOnlyMemory<TBody> body)
        : this(body, default) { }

    /// <inheritdoc />
    public readonly ReadOnlyMemory<TBody> Body
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => _body;
    }

    /// <inheritdoc />
    public readonly ReadOnlyMemory<TSeparator> Separator
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => _separator;
    }

    /// <summary>Gets itself as <see cref="SplitSpan{TBody, TSeparator, TStrategy}"/>.</summary>
    public readonly SplitSpan<TBody, TSeparator, TStrategy> SplitSpan
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => new(_body.Span, _separator.Span);
    }

    /// <summary>Gets the specified index.</summary>
    /// <param name="index">The index to get.</param>
    /// <exception cref="ArgumentOutOfRangeException">The parameter <paramref name="index"/> is negative.</exception>
    public readonly ReadOnlyMemory<TBody> this[[NonNegativeValue] int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        get
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), index, "must be positive");

            var e = GetEnumerator();

            for (var i = 0; i <= index; i++)
                if (!e.MoveNext())
                    return default;

            return e.Current;
        }
    }

    /// <summary>Determines whether both splits are equal.</summary>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>Whether both splits are equal.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool operator ==(
        SplitMemory<TBody, TSeparator, TStrategy> left,
        SplitMemory<TBody, TSeparator, TStrategy> right
    ) =>
        left.Equals(right);

    /// <summary>Determines whether both splits are not equal.</summary>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>Whether both splits are not equal.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool operator !=(
        SplitMemory<TBody, TSeparator, TStrategy> left,
        SplitMemory<TBody, TSeparator, TStrategy> right
    ) =>
        !left.Equals(right);

    /// <summary>Separates the head from the tail of this <see cref="SplitMemory{T, TSeparator, TStrategy}"/>.</summary>
    /// <param name="head">The first element of this enumeration.</param>
    /// <param name="tail">The rest of this enumeration.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Deconstruct(out ReadOnlyMemory<TBody> head, out SplitMemory<TBody, TSeparator, TStrategy> tail)
    {
        if (GetEnumerator() is var e && !e.MoveNext())
        {
            head = default;
            tail = default;
            return;
        }

        head = e.Current;
        tail = new(e.Body, _separator);
    }

    /// <inheritdoc cref="object.Equals(object)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public override bool Equals(object? obj) => Equals(obj as ISplitMemory<TBody, TSeparator>);

    /// <inheritdoc cref="IEquatable{T}.Equals(T)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public bool Equals(ISplitMemory<TBody, TSeparator>? other) =>
        other?.GetType() == typeof(SplitMemory<TBody, TSeparator, TStrategy>) &&
        _body.Span.SequenceEqual(other.Body.Span) &&
        _separator.Span.SequenceEqual(To<TSeparator>.From(other.Separator.Span));

    /// <inheritdoc cref="IEquatable{T}.Equals(T)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public bool Equals(SplitMemory<TBody, TSeparator, TStrategy> other) =>
        _body.Span.SequenceEqual(other._body.Span) &&
        _separator.Span.SequenceEqual(To<TSeparator>.From(other._separator.Span));

    /// <inheritdoc cref="IEquatable{T}.Equals(T)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public bool Equals<TOtherStrategy>(scoped in SplitMemory<TBody, TSeparator, TOtherStrategy> other) =>
        typeof(TStrategy) == typeof(TOtherStrategy) &&
        _body.Span.SequenceEqual(other._body.Span) &&
        _separator.Span.SequenceEqual(To<TSeparator>.From(other._separator.Span));

    /// <summary>Computes the length.</summary>
    /// <returns>The length.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public int Count()
    {
        var e = GetEnumerator();
        var count = 0;

        while (e.MoveNext())
            count++;

        return count;
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public override int GetHashCode() => unchecked(typeof(SplitMemory<TBody, TSeparator, TStrategy>).GetHashCode() * 7);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public override string ToString() => SplitSpan.ToString();

    /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
    // ReSharper restore NullableWarningSuppressionIsUsed
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public Enumerator GetEnumerator() => new(this);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    IEnumerator<ReadOnlyMemory<TBody>> IEnumerable<ReadOnlyMemory<TBody>>.GetEnumerator() => GetEnumerator();

    /// <summary>Gets the first element.</summary>
    /// <returns>The first span from this instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public ReadOnlyMemory<TBody> First() => GetEnumerator() is var e && e.MoveNext() ? e.Current : default;

    /// <summary>Gets the last element.</summary>
    /// <returns>The last span from this instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public ReadOnlyMemory<TBody> Last()
    {
        var e = GetEnumerator();

        while (e.MoveNext()) { }

        return e.Current;
    }

    /// <summary>Gets the single element.</summary>
    /// <returns>The single span from this instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public ReadOnlyMemory<TBody> Single() =>
        GetEnumerator() is var e && e.MoveNext() && e.Current is var ret && !e.MoveNext() ? ret : default;

    /// <summary>Gets the accumulated result of a set of callbacks where each element is passed in.</summary>
    /// <typeparam name="TAccumulator">The type of the accumulator value.</typeparam>
    /// <param name="seed">The accumulator.</param>
    /// <param name="func">An accumulator function to be invoked on each element.</param>
    /// <returns>The accumulated result of <paramref name="seed"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), MustUseReturnValue]
    public TAccumulator Aggregate<TAccumulator>(
        TAccumulator seed,
        [InstantHandle, RequireStaticDelegate] Func<TAccumulator, ReadOnlyMemory<TBody>, TAccumulator> func
    )
    {
        var accumulator = seed;

        foreach (var next in this)
            accumulator = func(accumulator, next);

        return accumulator;
    }

    /// <summary>
    /// Represents the enumeration object that views <see cref="SplitMemory{T, TSeparator, TStrategy}"/>.
    /// </summary>
    [StructLayout(LayoutKind.Auto)]
    public partial struct Enumerator(ReadOnlyMemory<TBody> body, ReadOnlyMemory<TSeparator> separator)
        : IEnumerator<ReadOnlyMemory<TBody>>
    {
        readonly ReadOnlyMemory<TSeparator> _separator = separator;

        ReadOnlyMemory<TBody> _body = body, _current;

        /// <summary>Initializes a new instance of the <see cref="Enumerator"/> struct.</summary>
        /// <param name="body">The body.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Enumerator(ReadOnlyMemory<TBody> body)
            : this(body, default) { }

        /// <summary>Initializes a new instance of the <see cref="Enumerator"/> struct.</summary>
        /// <param name="split">The enumerable to enumerate.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Enumerator(SplitMemory<TBody, TSeparator, TStrategy> split)
            : this(split._body, split._separator) { }

        /// <inheritdoc cref="SplitMemory{T, TSeparator, TStrategy}.Body"/>
        public readonly ReadOnlyMemory<TBody> Body
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => _body;
        }

        /// <inheritdoc />
        readonly object IEnumerator.Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => Current;
        }

        /// <inheritdoc cref="IEnumerator{T}.Current"/>
        public readonly ReadOnlyMemory<TBody> Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => _current;
        }

        /// <summary>Performs one step of an enumeration over the provided spans.</summary>
        /// <param name="body">The span that contains the current state of the enumeration.</param>
        /// <param name="separator">The separator span.</param>
        /// <param name="current">The current span.</param>
        /// <returns>
        /// <see langword="true"/> if a step was able to be performed successfully;
        /// <see langword="false"/> if the end of the collection is reached.
        /// </returns>
        public static bool MoveNext(
            ref ReadOnlyMemory<TBody> body,
            scoped in ReadOnlySpan<TSeparator> separator,
            ref ReadOnlyMemory<TBody> current
        ) =>
            typeof(TStrategy) switch
            {
                _ when separator.IsEmpty => !body.IsEmpty && current.IsEmpty && (current = body) is var _,
                var x when x == typeof(MatchAll) => MoveNextAll(ref body, To<TBody>.From(separator), ref current),
#if NET8_0_OR_GREATER
                var x when typeof(TSeparator) == typeof(SearchValues<TBody>) && x == typeof(MatchAny) =>
                    MoveNextAny(ref body, To<SearchValues<TBody>>.From(separator), ref current),
#endif
                var x when x == typeof(MatchAny) => MoveNextAny(ref body, To<TBody>.From(separator), ref current),
                var x when x == typeof(MatchOne) => MoveNextOne(ref body, To<TBody>.From(separator), ref current),
                _ => throw SplitSpan<TBody, TSeparator, TStrategy>.Enumerator.Error,
            };

        /// <summary>Advances the enumerator to the next element of the collection.</summary>
        /// <returns>
        /// <see langword="true"/> if the enumerator was successfully advanced to the next element;
        /// <see langword="false"/> if the enumerator has passed the end of the collection.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext() => MoveNext(ref _body, _separator.Span, ref _current);

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly void IDisposable.Dispose() { }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly void IEnumerator.Reset() { }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool MoveNextAll(
            ref ReadOnlyMemory<TBody> body,
            ReadOnlySpan<TBody> separator,
            ref ReadOnlyMemory<TBody> current
        )
        {
            System.Diagnostics.Debug.Assert(typeof(TStrategy) == typeof(MatchAll), "TStrategy is MatchAll");
            System.Diagnostics.Debug.Assert(!separator.IsEmpty, "separator is non-empty");
        Retry:

            if (body.IsEmpty)
                return false;

            switch (body.Span.IndexOf(separator))
            {
                case -1:
                    current = body;
                    body = default;
                    return true;
                case 0:
                    body = body[separator.Length..];
                    goto Retry;
                case var i:
                    current = body[..i];
                    body = body[(i + separator.Length)..];
                    return true;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool MoveNextAny(
            ref ReadOnlyMemory<TBody> body,
            ReadOnlySpan<TBody> separator,
            ref ReadOnlyMemory<TBody> current
        )
        {
            System.Diagnostics.Debug.Assert(typeof(TStrategy) == typeof(MatchAny), "TStrategy is MatchAny");
            System.Diagnostics.Debug.Assert(!separator.IsEmpty, "separator is non-empty");
        Retry:

            if (body.IsEmpty)
                return false;

#if NET7_0_OR_GREATER
            switch (body.Span.IndexOfAny(separator))
            {
                case -1:
                    current = body;
                    body = default;
                    return true;
                case 0:
                    body = body[1..];
                    goto Retry;
                case var i:
                    current = body[..i++];
                    body = body[i..];
                    return true;
            }
#else
            foreach (var next in separator)
                switch (body.Span.IndexOf(next))
                {
                    case -1: continue;
                    case 0:
                        body = body[1..];
                        goto Retry;
                    case var i:
                        current = body[..i++];
                        body = body[i..];
                        return true;
                }

            current = body;
            body = default;
            return true;
#endif
        }

#if NET8_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool MoveNextAny(
            ref ReadOnlyMemory<TBody> body,
            ReadOnlySpan<SearchValues<TBody>> separator,
            ref ReadOnlyMemory<TBody> current
        )
        {
            System.Diagnostics.Debug.Assert(typeof(TStrategy) == typeof(MatchAny), "TStrategy is MatchAny");
            System.Diagnostics.Debug.Assert(!separator.IsEmpty, "separator is non-empty");
            ref var single = ref MemoryMarshal.GetReference(separator);

        Retry:

            if (body.IsEmpty)
                return false;

            switch (body.Span.IndexOfAny(single))
            {
                case -1:
                    current = body;
                    body = default;
                    return true;
                case 0:
                    body = body[1..];
                    goto Retry;
                case var i:
                    current = body[..i];
                    body = body[(i + 1)..];
                    return true;
            }
        }
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool MoveNextOne(
            ref ReadOnlyMemory<TBody> body,
            ReadOnlySpan<TBody> separator,
            ref ReadOnlyMemory<TBody> current
        )
        {
            System.Diagnostics.Debug.Assert(typeof(TStrategy) == typeof(MatchOne), "TStrategy is MatchOne");
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
            ref var single = ref MemoryMarshal.GetReference(separator);
#else
            var single = separator[0];
#endif
        Retry:

            if (body.IsEmpty)
                return false;

            switch (body.Span.IndexOf(single))
            {
                case -1:
                    current = body;
                    body = default;
                    return true;
                case 0:
                    body = body[1..];
                    goto Retry;
                case var i:
                    current = body[..i++];
                    body = body[i..];
                    return true;
            }
        }
    }
}
