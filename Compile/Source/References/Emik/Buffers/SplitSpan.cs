// SPDX-License-Identifier: MPL-2.0

// ReSharper disable BadPreprocessorIndent CheckNamespace ConvertToAutoPropertyWhenPossible InvertIf RedundantNameQualifier RedundantReadonlyModifier RedundantUsingDirective StructCanBeMadeReadOnly UseSymbolAlias

namespace Emik.Morsels;
#pragma warning disable 8618, 9193, CA1823, IDE0250, MA0071, MA0102, SA1137
using static Span;
using static SplitSpanFactory;

/// <summary>Methods to split spans into multiple spans.</summary>
#pragma warning disable MA0048
static partial class SplitSpanFactory
#pragma warning restore MA0048
{
    /// <summary>The type that indicates to match all elements.</summary>
    public struct All;

    /// <summary>The type that indicates to match any element.</summary>
    public struct Any;

    /// <summary>The type that indicates to match exactly one element.</summary>
    public struct One;

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
        this scoped in SplitSpan<TBody, TLeftSeparator, TLeftStrategy> left,
        scoped in SplitSpan<TBody, TRightSeparator, TRightStrategy> right
    )
#if UNMANAGED_SPAN
        where T : unmanaged, IEquatable<T>?
#else
        where TBody : IEquatable<TBody>?
#endif
    {
        if (left.GetEnumerator() is var e1 && right.GetEnumerator() is var e2 && !e1.MoveNext())
            return !e2.MoveNext();

        if (!e2.MoveNext())
            return false;

        ReadOnlySpan<TBody>
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
        this scoped in SplitSpan<TBody, TLeftSeparator, TLeftStrategy> left,
        scoped in SplitSpan<TBody, TRightSeparator, TRightStrategy> right
    )
#if UNMANAGED_SPAN
        where T : unmanaged, IEquatable<T>?
#else
        where TBody : IEquatable<TBody>?
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
    public static SplitSpan<T, T, Any> SplitAny<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> separator)
#if UNMANAGED_SPAN
        where T : unmanaged, IEquatable<T>
#else
        where T : IEquatable<T>
#endif
        =>
            new(span, separator);

    /// <inheritdoc cref="SplitAny{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<T, T, Any> SplitAny<T>(this Span<T> span, ReadOnlySpan<T> separator)
#if UNMANAGED_SPAN
        where T : unmanaged, IEquatable<T>
#else
        where T : IEquatable<T>
#endif
        =>
            ((ReadOnlySpan<T>)span).SplitAny(separator);

    /// <summary>Splits a span by the specified separator.</summary>
    /// <typeparam name="T">The type of element from the span.</typeparam>
    /// <param name="span">The span to split.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>The enumerable object that references the parameter <paramref name="span"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<T, T, All> SplitAll<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> separator)
#if UNMANAGED_SPAN
        where T : unmanaged, IEquatable<T>
#else
        where T : IEquatable<T>
#endif
        =>
            new(span, separator);

    /// <inheritdoc cref="SplitAll{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<T, T, All> SplitAll<T>(this Span<T> span, ReadOnlySpan<T> separator)
#if UNMANAGED_SPAN
        where T : unmanaged, IEquatable<T>
#else
        where T : IEquatable<T>
#endif
        =>
            ((ReadOnlySpan<T>)span).SplitAll(separator);
#if NET8_0_OR_GREATER
    /// <inheritdoc cref="SplitAny{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<T, SearchValues<T>, Any> SplitOn<T>(this ReadOnlySpan<T> span, in SearchValues<T> separator)
        where T : IEquatable<T> =>
        new(span, In(separator));

    /// <inheritdoc cref="SplitAny{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<T, SearchValues<T>, Any> SplitOn<T>(this Span<T> span, in SearchValues<T> separator)
        where T : IEquatable<T> =>
        ((ReadOnlySpan<T>)span).SplitOn(separator);
#endif
#if NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP
    /// <inheritdoc cref="SplitAny{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<T, T, One> SplitOn<T>(this ReadOnlySpan<T> span, in T separator)
#if UNMANAGED_SPAN
        where T : unmanaged, IEquatable<T>
#else
        where T : IEquatable<T>
#endif
        =>
            new(span, In(separator));

    /// <inheritdoc cref="SplitAny{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<T, T, One> SplitOn<T>(this Span<T> span, in T separator)
#if UNMANAGED_SPAN
        where T : unmanaged, IEquatable<T>
#else
        where T : IEquatable<T>
#endif
        =>
            ((ReadOnlySpan<T>)span).SplitOn(separator);
#endif

    /// <summary>Copies the values to a new <see cref="List{T}"/>.</summary>
    /// <typeparam name="TSeparator">The type of separator.</typeparam>
    /// <typeparam name="TStrategy">The strategy for splitting elements.</typeparam>
    /// <param name="split">The instance to get the list from.</param>
    /// <returns>The list containing the copied values of this instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static List<string> ToList<TSeparator, TStrategy>(this SplitSpan<char, TSeparator, TStrategy> split)
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
    public static List<TBody[]> ToList<TBody, TSeparator, TStrategy>(this SplitSpan<TBody, TSeparator, TStrategy> split)
#if UNMANAGED_SPAN
        where T : unmanaged, IEquatable<T>
#else
        where TBody : IEquatable<TBody>?
#endif
    {
        List<TBody[]> ret = [];

        foreach (var next in split)
            ret.Add(next.ToArray());

        return ret;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool Next<TBody, TLeftSeparator, TLeftStrategy, TRightSeparator, TRightStrategy>(
        ref ReadOnlySpan<TBody> reader1,
        ref ReadOnlySpan<TBody> reader2,
        ref SplitSpan<TBody, TLeftSeparator, TLeftStrategy>.Enumerator e1,
        ref SplitSpan<TBody, TRightSeparator, TRightStrategy>.Enumerator e2,
        out bool ret
    )
#if UNMANAGED_SPAN
        where TBody : unmanaged, IEquatable<TBody>?
#else
        where TBody : IEquatable<TBody>?
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
        ref ReadOnlySpan<TBody> reader1,
        ref ReadOnlySpan<TBody> reader2,
        ref SplitSpan<TBody, TLeftSeparator, TLeftStrategy>.Enumerator e1,
        ref SplitSpan<TBody, TRightSeparator, TRightStrategy>.Enumerator e2,
        ref bool ret
    )
        where TBody : IEquatable<TBody>?
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
    /// <inheritdoc cref="SplitAny{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char, char, Any> SplitSpanAny(this string span, string separator) =>
        span.AsSpan().SplitAny(separator.AsSpan());

    /// <inheritdoc cref="SplitAny{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char, char, Any> SplitAny(this string span, ReadOnlySpan<char> separator) =>
        span.AsSpan().SplitAny(separator);

    /// <inheritdoc cref="SplitAll{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char, char, All> SplitSpanAll(this string span, string separator) =>
        span.AsSpan().SplitAll(separator.AsSpan());

    /// <inheritdoc cref="SplitAll{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char, char, All> SplitAll(this string span, ReadOnlySpan<char> separator) =>
        span.AsSpan().SplitAll(separator);

    /// <inheritdoc cref="SplitLines(ReadOnlySpan{char})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char,
#if NET8_0_OR_GREATER
        SearchValues<char>,
#else
        char,
#endif
        All> SplitSpanLines(this string span) =>
        span.AsSpan().SplitLines();

    /// <summary>Splits a span by line breaks.</summary>
    /// <remarks><para>Line breaks are considered any character in <see cref="Whitespaces.Breaking"/>.</para></remarks>
    /// <param name="span">The span to split.</param>
    /// <returns>The enumerable object that references the parameter <paramref name="span"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char,
#if NET8_0_OR_GREATER
        SearchValues<char>,
#else
        char,
#endif
        All> SplitLines(this ReadOnlySpan<char> span) =>
#if NET8_0_OR_GREATER
        new(span, new(Whitespaces.BreakingSearch));
#else
        new(span, Whitespaces.Breaking.AsSpan(), true);
#endif

    /// <inheritdoc cref="SplitLines(ReadOnlySpan{char})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char,
#if NET8_0_OR_GREATER
        SearchValues<char>,
#else
        char,
#endif
        All> SplitLines(this Span<char> span) =>
        ((ReadOnlySpan<char>)span).SplitLines();

    /// <inheritdoc cref="SplitAny{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char, char, One> SplitOn(this string span, in char separator) =>
        span.AsSpan().SplitOn(separator);

    /// <inheritdoc cref="SplitWhitespace(ReadOnlySpan{char})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char,
#if NET8_0_OR_GREATER
        SearchValues<char>,
#else
        char,
#endif
        All> SplitSpanWhitespace(this string span) =>
        span.AsSpan().SplitWhitespace();

    /// <summary>Splits a span by whitespace.</summary>
    /// <remarks><para>Whitespace is considered any character in <see cref="Whitespaces.Unicode"/>.</para></remarks>
    /// <param name="span">The span to split.</param>
    /// <returns>The enumerable object that references the parameter <paramref name="span"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char,
#if NET8_0_OR_GREATER
        SearchValues<char>,
#else
        char,
#endif
        All> SplitWhitespace(this ReadOnlySpan<char> span) =>
#if NET8_0_OR_GREATER
        new(span, new(Whitespaces.UnicodeSearch));
#else
        new(span, To<SplitSpan<char>.Any>.From(Whitespaces.Unicode.AsSpan()));
#endif

    /// <inheritdoc cref="SplitWhitespace(ReadOnlySpan{char})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char,
#if NET8_0_OR_GREATER
        SearchValues<char>,
#else
        char,
#endif
        All> SplitWhitespace(this Span<char> span) =>
        ((ReadOnlySpan<char>)span).SplitWhitespace();
#endif
#if NET8_0_OR_GREATER
    /// <inheritdoc cref="SplitAny{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char, SearchValues<char>, Any> SplitSpanOn(
        this string span,
        in SearchValues<char> separator
    ) =>
        span.AsSpan().SplitOn(separator);
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
#if !NO_REF_STRUCTS
    ref
#endif
    partial struct SplitSpan<TBody, TSeparator, TStrategy>(ReadOnlySpan<TBody> body, ReadOnlySpan<TSeparator> separator)
#if UNMANAGED_SPAN
    where T : unmanaged, IEquatable<T>?
#else
    where TBody : IEquatable<TBody>?
#endif
{
    /// <summary>Represents the accumulator function for the enumeration of this type.</summary>
    /// <typeparam name="TAccumulator">The type of the accumulator value.</typeparam>
    /// <param name="accumulator">The accumulator.</param>
    /// <param name="next">The next slice from the enumeration.</param>
    /// <returns>The final accumulator value.</returns>
    public delegate TAccumulator Accumulator<TAccumulator>(TAccumulator accumulator, scoped ReadOnlySpan<TBody> next);

    readonly ReadOnlySpan<TBody> _body = body;

    readonly ReadOnlySpan<TSeparator> _separator = separator;

    /// <summary>Initializes a new instance of the <see cref="SplitSpan{T, TSeparator, TStrategy}"/> struct.</summary>
    /// <param name="body">The line to split.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SplitSpan(ReadOnlySpan<TBody> body)
        : this(body, default) { }

    /// <summary>Gets the line.</summary>
    public readonly ReadOnlySpan<TBody> Body
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => _body;
    }

    /// <summary>Gets the line.</summary>
    public readonly ReadOnlySpan<TSeparator> Separator
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => _separator;
    }

    /// <summary>Gets the specified index.</summary>
    /// <param name="index">The index to get.</param>
    /// <exception cref="ArgumentOutOfRangeException">The parameter <paramref name="index"/> is negative.</exception>
    public readonly ReadOnlySpan<TBody> this[[NonNegativeValue] int index]
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
        scoped in SplitSpan<TBody, TSeparator, TStrategy> left,
        scoped in SplitSpan<TBody, TSeparator, TStrategy> right
    ) =>
        left.Equals(right);

    /// <summary>Determines whether both splits are not equal.</summary>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>Whether both splits are not equal.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool operator !=(
        scoped in SplitSpan<TBody, TSeparator, TStrategy> left,
        scoped in SplitSpan<TBody, TSeparator, TStrategy> right
    ) =>
        !left.Equals(right);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public override bool Equals(object? obj) => false;

    /// <inheritdoc cref="IEquatable{T}.Equals(T)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public bool Equals<TOtherStrategy>(scoped in SplitSpan<TBody, TSeparator, TOtherStrategy> other) =>
        typeof(TStrategy) == typeof(TOtherStrategy) &&
        _body.SequenceEqual(other._body) &&
        _separator.SequenceEqual(To<TSeparator>.From(other._separator));

    /// <inheritdoc />
    public override int GetHashCode() => unchecked(typeof(TBody).GetHashCode() * 31);

    /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
    // ReSharper restore NullableWarningSuppressionIsUsed
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public Enumerator GetEnumerator() => new(_body, _separator);

    /// <summary>Gets the first element.</summary>
    /// <returns>The first span from this instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public ReadOnlySpan<TBody> First() => GetEnumerator() is var e && e.MoveNext() ? e.Current : default;

    /// <summary>Gets the last element.</summary>
    /// <returns>The last span from this instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public ReadOnlySpan<TBody> Last()
    {
        var e = GetEnumerator();

        while (e.MoveNext()) { }

        return e.Current;
    }

    /// <summary>Separates the head from the tail of this <see cref="SplitSpan{T, TSeparator, TStrategy}"/>.</summary>
    /// <param name="head">The first element of this enumeration.</param>
    /// <param name="tail">The rest of this enumeration.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Deconstruct(out ReadOnlySpan<TBody> head, out SplitSpan<TBody, TSeparator, TStrategy> tail)
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
    public override string ToString() =>
        typeof(TBody) == typeof(char)
            ? Aggregate(new(), StringBuilderAccumulator()).ToString()
#if NETFRAMEWORK || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
            : this.ToList().Stringify(3, true);
#else
            : throw new NotSupportedException();
#endif

    /// <summary>Gets the single element.</summary>
    /// <returns>The single span from this instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public ReadOnlySpan<TBody> Single() =>
        GetEnumerator() is var e && e.MoveNext() && e.Current is var ret && !e.MoveNext() ? ret : default;

    /// <summary>Gets the accumulated result of a set of callbacks where each element is passed in.</summary>
    /// <typeparam name="TAccumulator">The type of the accumulator value.</typeparam>
    /// <param name="seed">The accumulator.</param>
    /// <param name="func">An accumulator function to be invoked on each element.</param>
    /// <returns>The accumulated result of <paramref name="seed"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), MustUseReturnValue]
    public TAccumulator Aggregate<TAccumulator>(
        TAccumulator seed,
        [InstantHandle, RequireStaticDelegate] Accumulator<TAccumulator> func
    )
    {
        var accumulator = seed;

        foreach (var next in this)
            accumulator = func(accumulator, next);

        return accumulator;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] // ReSharper disable once RedundantUnsafeContext
    static unsafe Accumulator<StringBuilder> StringBuilderAccumulator() =>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        static (builder, span) => builder.Append(
            MemoryMarshal.CreateReadOnlySpan(
                ref Unsafe.As<TBody, char>(ref MemoryMarshal.GetReference(span)),
                span.Length
            )
        );
#else
        static (builder, span) =>
        {
#if NETFRAMEWORK && !NET46_OR_GREATER || NETSTANDARD && !NETSTANDARD1_3_OR_GREATER
            for (var i = 0; i < span.Length; i++)
                builder.Append(((char*)span.Pointer)[i]);

            return builder;
#else
#pragma warning disable 8500
#if !(NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) || NO_SYSTEM_MEMORY
            var ptr = span.Pointer;
#else
            fixed (T* ptr = span)
#endif
#pragma warning restore 8500
                return builder.Append((char*)ptr, span.Length);
#endif
        };
#endif

    /// <summary>Represents the enumeration object that views <see cref="SplitSpan{T, TSeparator, TStrategy}"/>.</summary>
    [StructLayout(LayoutKind.Auto)]
    public
#if !NO_REF_STRUCTS
        ref
#endif
        partial struct Enumerator(ReadOnlySpan<TBody> body, ReadOnlySpan<TSeparator> separator)
    {
        readonly ReadOnlySpan<TSeparator> _separator = separator;

        ReadOnlySpan<TBody> _body = body, _current;

        /// <summary>Initializes a new instance of the <see cref="Enumerator"/> struct.</summary>
        /// <param name="split">The enumerable to enumerate.</param>
        public Enumerator(SplitSpan<TBody, TSeparator, TStrategy> split)
            : this(split._body, split._separator) { }

        /// <inheritdoc cref="SplitSpan{T, TSeparator, TStrategy}.Body"/>
        public readonly ReadOnlySpan<TBody> Body
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => _body;
        }

        /// <inheritdoc cref="IEnumerator{T}.Current"/>
        public readonly ReadOnlySpan<TBody> Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => _current;
        }

        static NotSupportedException Error
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
            get => new($"Unrecognized type: {typeof(TStrategy).Name}");
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
            scoped ref ReadOnlySpan<TBody> body,
            scoped in ReadOnlySpan<TSeparator> separator,
            scoped ref ReadOnlySpan<TBody> current
        ) =>
            typeof(TStrategy) switch
            {
                _ when body.IsEmpty => true,
                _ when separator.IsEmpty => !body.IsEmpty && current.IsEmpty && (current = body) is var _,
                var x when x == typeof(All) => MoveNextAll(ref body, To<TBody>.From(separator), ref current),
#if NET8_0_OR_GREATER
                var x when typeof(TSeparator) == typeof(SearchValues<TBody>) && x == typeof(Any) =>
                    MoveNextAny(ref body, To<SearchValues<TBody>>.From(separator), ref current),
#endif
                var x when x == typeof(Any) => MoveNextAny(ref body, To<TBody>.From(separator), ref current),
                var x when x == typeof(One) => MoveNextOne(ref body, To<TBody>.From(separator), ref current),
                _ => throw Error,
            };

        /// <summary>Advances the enumerator to the next element of the collection.</summary>
        /// <returns>
        /// <see langword="true"/> if the enumerator was successfully advanced to the next element;
        /// <see langword="false"/> if the enumerator has passed the end of the collection.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext() => MoveNext(ref _body, _separator, ref _current);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool MoveNextAll(
            scoped ref ReadOnlySpan<TBody> body,
            scoped ReadOnlySpan<TBody> separator,
            scoped ref ReadOnlySpan<TBody> current
        )
        {
            System.Diagnostics.Debug.Assert(typeof(TStrategy) == typeof(All), "typeof(TStrategy) == typeof(All)");
        Retry:

            if (body.IsEmpty)
                return false;

            switch (body.IndexOf(separator))
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
        static bool MoveNextOne(
            scoped ref ReadOnlySpan<TBody> body,
            scoped ReadOnlySpan<TBody> separator,
            scoped ref ReadOnlySpan<TBody> current
        )
        {
            System.Diagnostics.Debug.Assert(typeof(TStrategy) == typeof(One), "typeof(TStrategy) == typeof(One)");

            ref var single = ref MemoryMarshal.GetReference(separator);
        Retry:

            if (body.IsEmpty)
                return false;

            switch (body.IndexOf(single))
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool MoveNextAny(
            scoped ref ReadOnlySpan<TBody> body,
            scoped ReadOnlySpan<TBody> separator,
            scoped ref ReadOnlySpan<TBody> current
        )
        {
            System.Diagnostics.Debug.Assert(typeof(TStrategy) == typeof(Any), "typeof(TStrategy) == typeof(Any)");
        Retry:

            if (body.IsEmpty)
                return false;

#if NET7_0_OR_GREATER
            switch (body.IndexOfAny(separator))
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
                switch (body.IndexOf(next))
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
            scoped ref ReadOnlySpan<TBody> body,
            scoped ReadOnlySpan<SearchValues<TBody>> separator,
            scoped ref ReadOnlySpan<TBody> current
        )
        {
            System.Diagnostics.Debug.Assert(
                typeof(TStrategy) == typeof(Any),
                "typeof(TStrategy) == typeof(Any.Search)"
            );

            System.Diagnostics.Debug.Assert(!separator.IsEmpty, "!separator.IsEmpty");
            System.Diagnostics.Debug.Assert(!body.IsEmpty, "!body.IsEmpty");

            ref var reference = ref MemoryMarshal.GetReference(separator);

        Retry:

            switch (body.IndexOfAny(reference))
            {
                case -1:
                    current = body;
                    body = default;
                    return true;
                case 0:
                    if (body.Length is 1)
                        return false;

                    body = body[1..];
                    goto Retry;
                case var i:
                    current = body[..i];
                    body = body[(i + 1)..];
                    return true;
            }
        }
#endif
    }
}
