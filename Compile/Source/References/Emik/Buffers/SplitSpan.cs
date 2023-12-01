// SPDX-License-Identifier: MPL-2.0

// ReSharper disable BadPreprocessorIndent CheckNamespace ConvertToAutoPropertyWhenPossible InvertIf RedundantNameQualifier RedundantUsingDirective StructCanBeMadeReadOnly UseSymbolAlias
namespace Emik.Morsels;
#pragma warning disable 8618, CA1823, IDE0250, MA0071, MA0102, SA1137
using static Span;

/// <summary>Methods to split spans into multiple spans.</summary>
#pragma warning disable MA0048
static partial class SplitSpanFactory
#pragma warning restore MA0048
{
    /// <summary>Determines whether both splits are eventually equal when concatenating all slices.</summary>
    /// <typeparam name="T">The type of <see cref="SplitSpan{T}"/>.</typeparam>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>
    /// The value <paramref langword="true"/> if both sequences are equal, otherwise; <paramref langword="false"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool ConcatEqual<T, TFirstStrategy, TSecondStrategy>(
        this SplitSpan<T>.Of<TFirstStrategy> left,
        SplitSpan<T>.Of<TSecondStrategy> right
    )
#if UNMANAGED_SPAN
        where T : unmanaged, IEquatable<T>?
#else
        where T : IEquatable<T>?
#endif
        where TFirstStrategy : IEquatable<TFirstStrategy>, SplitSpan<T>.IStrategy
        where TSecondStrategy : IEquatable<TSecondStrategy>, SplitSpan<T>.IStrategy
    {
        if (left.GetEnumerator() is var e1 && right.GetEnumerator() is var e2 && !e1.MoveNext())
            return !e2.MoveNext();

        if (!e2.MoveNext())
            return false;

        ReadOnlySpan<T>
            reader1 = e1.Current,
            reader2 = e2.Current;

        while (true)
            if (Next(ref reader1, ref reader2, ref e1, ref e2, out var ret))
                return ret;
    }

    /// <summary>Determines whether both splits are equal.</summary>
    /// <typeparam name="T">The type of <see cref="SplitSpan{T}"/>.</typeparam>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>
    /// The value <paramref langword="true"/> if both sequences are equal, otherwise; <paramref langword="false"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool SequenceEqual<T, TFirstStrategy, TSecondStrategy>(
        this SplitSpan<T>.Of<TFirstStrategy> left,
        SplitSpan<T>.Of<TSecondStrategy> right
    )
#if UNMANAGED_SPAN
        where T : unmanaged, IEquatable<T>?
#else
        where T : IEquatable<T>?
#endif
        where TFirstStrategy : IEquatable<TFirstStrategy>, SplitSpan<T>.IStrategy
        where TSecondStrategy : IEquatable<TSecondStrategy>, SplitSpan<T>.IStrategy
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
    public static SplitSpan<T>.Of<SplitSpan<T>.Any> SplitAny<T>(this ReadOnlySpan<T> span, in ReadOnlySpan<T> separator)
#if UNMANAGED_SPAN
        where T : unmanaged, IEquatable<T>
#else
        where T : IEquatable<T>
#endif
        =>
            new(span, To<SplitSpan<T>.Any>.From(separator));

    /// <inheritdoc cref="SplitAny{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<T>.Of<SplitSpan<T>.Any> SplitAny<T>(this Span<T> span, in ReadOnlySpan<T> separator)
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
    public static SplitSpan<T>.Of<SplitSpan<T>.All> SplitAll<T>(this ReadOnlySpan<T> span, in ReadOnlySpan<T> separator)
#if UNMANAGED_SPAN
        where T : unmanaged, IEquatable<T>
#else
        where T : IEquatable<T>
#endif
        =>
            new(span, To<SplitSpan<T>.All>.From(separator));

    /// <inheritdoc cref="SplitAll{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<T>.Of<SplitSpan<T>.All> SplitAll<T>(this Span<T> span, in ReadOnlySpan<T> separator)
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
    public static SplitSpan<T>.Of<SplitSpan<T>.Any.Search> SplitOn<T>(
        this ReadOnlySpan<T> span,
        SearchValues<T> separator
    )
        where T : IEquatable<T> =>
        new(span, To<SplitSpan<T>.Any.Search>.From(In(separator)));

    /// <inheritdoc cref="SplitAny{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<T>.Of<SplitSpan<T>.Any.Search> SplitOn<T>(this Span<T> span, SearchValues<T> separator)
        where T : IEquatable<T> =>
        ((ReadOnlySpan<T>)span).SplitOn(separator);
#endif
#if NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP
    /// <inheritdoc cref="SplitAny{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<T>.Of<SplitSpan<T>.All.One> SplitOn<T>(this ReadOnlySpan<T> span, in T separator)
#if UNMANAGED_SPAN
        where T : unmanaged, IEquatable<T>
#else
        where T : IEquatable<T>
#endif
        =>
            new(span, To<SplitSpan<T>.All.One>.From(In(separator)));

    /// <inheritdoc cref="SplitAny{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<T>.Of<SplitSpan<T>.All.One> SplitOn<T>(this Span<T> span, in T separator)
#if UNMANAGED_SPAN
        where T : unmanaged, IEquatable<T>
#else
        where T : IEquatable<T>
#endif
        =>
            ((ReadOnlySpan<T>)span).SplitOn(separator);
#endif

    /// <summary>Copies the values to a new <see cref="List{T}"/>.</summary>
    /// <param name="split">The instance to get the list from.</param>
    /// <returns>The list containing the copied values of this instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static List<string> ToList<TStrategy>(this SplitSpan<char>.Of<TStrategy> split)
        where TStrategy : IEquatable<TStrategy>, SplitSpan<char>.IStrategy
    {
        List<string> ret = [];

        foreach (var next in split)
            ret.Add(next.ToString());

        return ret;
    }

    /// <summary>Copies the values to a new <see cref="List{T}"/>.</summary>
    /// <typeparam name="T">The type of element from the span.</typeparam>
    /// <param name="split">The instance to get the list from.</param>
    /// <returns>The list containing the copied values of this instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static List<T[]> ToList<T, TStrategy>(this SplitSpan<T>.Of<TStrategy> split)
#if UNMANAGED_SPAN
        where T : unmanaged, IEquatable<T>
#else
        where T : IEquatable<T>?
#endif
        where TStrategy : IEquatable<TStrategy>, SplitSpan<T>.IStrategy
    {
        List<T[]> ret = [];

        foreach (var next in split)
            ret.Add(next.ToArray());

        return ret;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool Next<T, TFirstStrategy, TSecondStrategy>(
        ref ReadOnlySpan<T> reader1,
        ref ReadOnlySpan<T> reader2,
        ref SplitSpan<T>.Of<TFirstStrategy>.Enumerator e1,
        ref SplitSpan<T>.Of<TSecondStrategy>.Enumerator e2,
        out bool ret
    )
#if UNMANAGED_SPAN
        where T : unmanaged, IEquatable<T>?
#else
        where T : IEquatable<T>?
#endif
        where TFirstStrategy : IEquatable<TFirstStrategy>, SplitSpan<T>.IStrategy
        where TSecondStrategy : IEquatable<TSecondStrategy>, SplitSpan<T>.IStrategy
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
    static bool SameLength<T, TFirstStrategy, TSecondStrategy>(
        ref ReadOnlySpan<T> reader1,
        ref ReadOnlySpan<T> reader2,
        ref SplitSpan<T>.Of<TFirstStrategy>.Enumerator e1,
        ref SplitSpan<T>.Of<TSecondStrategy>.Enumerator e2,
        ref bool ret
    )
        where T : IEquatable<T>?
        where TFirstStrategy : IEquatable<TFirstStrategy>, SplitSpan<T>.IStrategy
        where TSecondStrategy : IEquatable<TSecondStrategy>, SplitSpan<T>.IStrategy
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
    public static SplitSpan<char>.Of<SplitSpan<char>.Any> SplitSpanAny(this string span, string separator) =>
        span.AsSpan().SplitAny(separator.AsSpan());

    /// <inheritdoc cref="SplitAny{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char>.Of<SplitSpan<char>.Any> SplitAny(this string span, in ReadOnlySpan<char> separator) =>
        span.AsSpan().SplitAny(separator);

    /// <inheritdoc cref="SplitAll{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char>.Of<SplitSpan<char>.All> SplitSpanAll(this string span, string separator) =>
        span.AsSpan().SplitAll(separator.AsSpan());

    /// <inheritdoc cref="SplitAll{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char>.Of<SplitSpan<char>.All> SplitAll(this string span, in ReadOnlySpan<char> separator) =>
        span.AsSpan().SplitAll(separator);

    /// <inheritdoc cref="SplitLines(ReadOnlySpan{char})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char>.Of<SplitSpan<char>.Any.Search> SplitSpanLines(this string span) =>
        span.AsSpan().SplitLines();

    /// <summary>Splits a span by line breaks.</summary>
    /// <remarks><para>Line breaks are considered any character in <see cref="Whitespaces.Breaking"/>.</para></remarks>
    /// <param name="span">The span to split.</param>
    /// <returns>The enumerable object that references the parameter <paramref name="span"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char>.Of<SplitSpan<char>.Any.Search> SplitLines(this ReadOnlySpan<char> span) =>
#if NET8_0_OR_GREATER
        new(span, To<SplitSpan<char>.Any.Search>.From(In(Whitespaces.BreakingSearch)));
#else
        new(span, Whitespaces.Breaking.AsSpan(), true);
#endif

    /// <inheritdoc cref="SplitLines(ReadOnlySpan{char})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char>.Of<SplitSpan<char>.Any.Search> SplitLines(this Span<char> span) =>
        ((ReadOnlySpan<char>)span).SplitLines();

    /// <inheritdoc cref="SplitAny{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char>.Of<SplitSpan<char>.All.One> SplitOn(this string span, in char separator) =>
        span.AsSpan().SplitOn(separator);

    /// <inheritdoc cref="SplitWhitespace(ReadOnlySpan{char})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char>.Of<SplitSpan<char>.Any.Search> SplitSpanWhitespace(this string span) =>
        span.AsSpan().SplitWhitespace();

    /// <summary>Splits a span by whitespace.</summary>
    /// <remarks><para>Whitespace is considered any character in <see cref="Whitespaces.Unicode"/>.</para></remarks>
    /// <param name="span">The span to split.</param>
    /// <returns>The enumerable object that references the parameter <paramref name="span"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char>.Of<SplitSpan<char>.Any.Search> SplitWhitespace(this ReadOnlySpan<char> span) =>
#if NET8_0_OR_GREATER
        new(span, To<SplitSpan<char>.Any.Search>.From(In(Whitespaces.UnicodeSearch)));
#else
        new(span, To<SplitSpan<char>.Any>.From(Whitespaces.Unicode.AsSpan()));
#endif

    /// <inheritdoc cref="SplitWhitespace(ReadOnlySpan{char})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char>.Of<SplitSpan<char>.Any.Search> SplitWhitespace(this Span<char> span) =>
        ((ReadOnlySpan<char>)span).SplitWhitespace();
#endif
#if NET8_0_OR_GREATER
    /// <inheritdoc cref="SplitAny{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char>.Of<SplitSpan<char>.Any.Search> SplitSpanOn(
        this string span,
        SearchValues<char> separator
    ) =>
        span.AsSpan().SplitOn(separator);
#endif
}

/// <summary>Represents a split entry.</summary>
/// <typeparam name="T">The type of element from the span.</typeparam>
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
    partial struct SplitSpan<T>
#if UNMANAGED_SPAN
    where T : unmanaged, IEquatable<T>?
#else
    where T : IEquatable<T>?
#endif
{
    /// <summary>Represents the accumulator function for the enumeration of this type.</summary>
    /// <typeparam name="TAccumulator">The type of the accumulator value.</typeparam>
    /// <param name="accumulator">The accumulator.</param>
    /// <param name="next">The next slice from the enumeration.</param>
    /// <returns>The final accumulator value.</returns>
    public delegate TAccumulator Accumulator<TAccumulator>(TAccumulator accumulator, scoped ReadOnlySpan<T> next);

    [StructLayout(LayoutKind.Auto)]
    public
#if !NO_READONLY_STRUCTS
        readonly
#endif
#if !NO_REF_STRUCTS
        ref
#endif
        partial struct Of<TStrategy>(SplitSpan<T> body, ReadOnlySpan<TStrategy> separator)
        where TStrategy : IEquatable<TStrategy>, IStrategy
    {
        readonly ReadOnlySpan<T> _body = body._body;

        readonly ReadOnlySpan<TStrategy> _separator = separator;

        /// <inheritdoc cref="SplitSpan{T}.Body"/>
#pragma warning disable SA1600, RCS1085
        public ReadOnlySpan<T> Body
#pragma warning restore SA1600, RCS1085
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => _body;
        }

        static NotSupportedException Error
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
            get => new($"Unrecognized type: {typeof(TStrategy).Name}");
        }

        /// <summary>Gets the specified index.</summary>
        /// <param name="index">The index to get.</param>
        /// <exception cref="ArgumentOutOfRangeException">The parameter <paramref name="index"/> is negative.</exception>
        public ReadOnlySpan<T> this[[NonNegativeValue] int index]
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

        /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
        // ReSharper restore NullableWarningSuppressionIsUsed
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        public Enumerator GetEnumerator() => new(Body, _separator);

        /// <summary>Gets the first element.</summary>
        /// <returns>The first span from this instance.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        public ReadOnlySpan<T> First() => GetEnumerator() is var e && e.MoveNext() ? e.Current : default;

        /// <summary>Gets the last element.</summary>
        /// <returns>The last span from this instance.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        public ReadOnlySpan<T> Last()
        {
            var e = GetEnumerator();

            while (e.MoveNext()) { }

            return e.Current;
        }

        /// <summary>Separates the head from the tail of this <see cref="SplitSpan{T}"/>.</summary>
        /// <param name="head">The first element of this enumeration.</param>
        /// <param name="tail">The rest of this enumeration.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Deconstruct(out ReadOnlySpan<T> head, out Of<TStrategy> tail)
        {
            if (GetEnumerator() is var e && !e.MoveNext())
            {
                head = default;
                tail = default;
                return;
            }

            head = e.Current;
            tail = new(new(Body[head.Length..]), _separator);
        }

        public bool Equals<TOtherStrategy>(Of<TOtherStrategy> other)
            where TOtherStrategy : IEquatable<TOtherStrategy>, IStrategy =>
            To<TStrategy>.Is<TOtherStrategy>.Supported &&
            To<TStrategy>.From(other._separator) is var reinterpret &&
            _separator.SequenceEqual(reinterpret) &&
            _body.SequenceEqual(other._body);

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
            typeof(T) == typeof(char)
                ? Aggregate(new(), StringBuilderAccumulator()).ToString()
#if NETFRAMEWORK || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
                : this.ToList().Stringify(3, true);
#else
                : throw new NotSupportedException();
#endif

        /// <summary>Gets the single element.</summary>
        /// <returns>The single span from this instance.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        public ReadOnlySpan<T> Single() =>
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
                    ref Unsafe.As<T, char>(ref MemoryMarshal.GetReference(span)),
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

        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        static ReadOnlySpan<T> UnsafelyAsT(scoped in ReadOnlySpan<TStrategy> separator)
        {
            System.Diagnostics.Debug.Assert(
                Unsafe.SizeOf<TStrategy>() == Unsafe.SizeOf<T>(),
                "Unsafe.SizeOf<TStrategy>() == Unsafe.SizeOf<T>()"
            );

            return MemoryMarshal.CreateReadOnlySpan(
                ref Unsafe.As<TStrategy, T>(ref MemoryMarshal.GetReference(separator)),
                separator.Length
            );
        }
#if NET8_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        static ref SearchValues<T> UnsafelyAsSearchValuesT(scoped in ReadOnlySpan<TStrategy> separator)
        {
            return ref Unsafe.As<TStrategy, SearchValues<T>>(ref MemoryMarshal.GetReference(separator));
        }
#endif

        /// <summary>Represents the enumeration object that views <see cref="SplitSpan{T}"/>.</summary>
        [StructLayout(LayoutKind.Auto)]
        public
#if !NO_REF_STRUCTS
            ref
#endif
            partial struct Enumerator(ReadOnlySpan<T> body, ReadOnlySpan<TStrategy> separator)
        {
            readonly ReadOnlySpan<TStrategy> _separator = separator;

            ReadOnlySpan<T> _body = body, _current;

            /// <inheritdoc cref="SplitSpan{T}.Body"/>
            public readonly ReadOnlySpan<T> Body
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => _body;
            }

            /// <inheritdoc cref="IEnumerator{T}.Current"/>
            public readonly ReadOnlySpan<T> Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => _current;
            }

            public static bool MoveNext(
                scoped ref ReadOnlySpan<T> body,
                in ReadOnlySpan<TStrategy> separator,
                scoped ref ReadOnlySpan<T> current
            ) =>
                typeof(TStrategy) switch
                {
                    _ when body.IsEmpty => true,
                    _ when separator.IsEmpty => !body.IsEmpty && current.IsEmpty && (current = body) is var _,
                    var x when x == typeof(All) => MoveNextAll(ref body, To<T>.From(separator), ref current),
                    var x when x == typeof(All.One) => MoveNextAllOne(ref body, To<T>.From(separator), ref current),
                    var x when x == typeof(Any) => MoveNextAny(ref body, To<T>.From(separator), ref current),
#if NET8_0_OR_GREATER
                    var x when x == typeof(Any.Search) =>
                        MoveNextAnySearch(ref body, To<SearchValues<T>>.From(separator), ref current),
#endif
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
                scoped ref ReadOnlySpan<T> body,
                scoped ReadOnlySpan<T> separator,
                scoped ref ReadOnlySpan<T> current
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
            static bool MoveNextAllOne(
                scoped ref ReadOnlySpan<T> body,
                scoped ReadOnlySpan<T> separator,
                scoped ref ReadOnlySpan<T> current
            )
            {
                System.Diagnostics.Debug.Assert(
                    typeof(TStrategy) == typeof(All.One),
                    "typeof(TStrategy) == typeof(All.One)"
                );

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
                scoped ref ReadOnlySpan<T> body,
                scoped ReadOnlySpan<T> separator,
                scoped ref ReadOnlySpan<T> current
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
            static bool MoveNextAnySearch(
                scoped ref ReadOnlySpan<T> body,
                scoped ReadOnlySpan<SearchValues<T>> separator,
                scoped ref ReadOnlySpan<T> current
            )
            {
                System.Diagnostics.Debug.Assert(
                    typeof(TStrategy) == typeof(Any.Search),
                    "typeof(TStrategy) == typeof(Any.Search)"
                );

                ref var reference = ref MemoryMarshal.GetReference(separator);

            Retry:

                if (body.IsEmpty)
                    return false;

                switch (body.IndexOfAny(reference))
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
        }
    }

    public
#if !NO_READONLY_STRUCTS
        readonly
#endif
        struct Any : IEquatable<Any>, IStrategy
    {
        public
#if !NO_READONLY_STRUCTS
            readonly
#endif
            struct Search : IEquatable<Search>, IStrategy
        {
            [UsedImplicitly]
#if NET8_0_OR_GREATER
            readonly SearchValues<T> _item;
#else
            readonly T _item;
#endif

            /// <inheritdoc />
            // ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            bool IEquatable<Search>.Equals(Search other) =>
                _item is null ? other._item is null : other._item is not null && _item.Equals(other._item);
            // ReSharper restore ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        }

        [UsedImplicitly]
        readonly T _item;

        /// <inheritdoc />
        bool IEquatable<Any>.Equals(Any other) =>
            _item is null ? other._item is null : other._item is not null && _item.Equals(other._item);
    }

    public
#if !NO_READONLY_STRUCTS
        readonly
#endif
        struct All : IStrategy, IEquatable<All>
    {
        [UsedImplicitly]
        readonly T _item;

        public
#if !NO_READONLY_STRUCTS
            readonly
#endif
            struct One : IStrategy, IEquatable<One>
        {
            [UsedImplicitly]
            readonly T _item;

            /// <inheritdoc />
            bool IEquatable<One>.Equals(One other) =>
                _item is null ? other._item is null : other._item is not null && _item.Equals(other._item);
        }

        /// <inheritdoc />
        bool IEquatable<All>.Equals(All other) =>
            _item is null ? other._item is null : other._item is not null && _item.Equals(other._item);
    }

    public interface IStrategy;

    readonly ReadOnlySpan<T> _body;

    /// <summary>Initializes a new instance of the <see cref="SplitSpan{T}"/> struct.</summary>
    /// <param name="body">The line to split.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SplitSpan(ReadOnlySpan<T> body) => _body = body;

    /// <summary>Gets the empty split span.</summary>
    public static SplitSpan<T> Empty
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => default;
    }

    /// <summary>Gets the line.</summary>
    public readonly ReadOnlySpan<T> Body
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => _body;
    }

    /// <summary>Determines whether both splits are equal.</summary>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>Whether both splits are equal.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool operator ==(scoped SplitSpan<T> left, scoped SplitSpan<T> right) => left.Equals(right);

    /// <summary>Determines whether both splits are not equal.</summary>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>Whether both splits are not equal.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool operator !=(scoped SplitSpan<T> left, scoped SplitSpan<T> right) => !left.Equals(right);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public override bool Equals(object? obj) => false;

    /// <inheritdoc cref="IEquatable{T}.Equals(T)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public bool Equals(scoped SplitSpan<T> other) => Body.SequenceEqual(other.Body);
}
