// <copyright file="SplitMemory.cs" company="Emik">
// Copyright (c) Emik. This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

// ReSharper disable BadPreprocessorIndent CheckNamespace EmptyNamespace InvertIf RedundantExtendsListEntry RedundantUsingDirective StructCanBeMadeReadOnly
namespace Emik.Morsels;
#pragma warning disable 8500, 8618, IDE0044, IDE0250, MA0048, MA0071, MA0102, SA1114, SA1137
using static Span;

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER || ROSLYN
/// <summary>Methods to split memories into multiple memories.</summary>
static partial class SplitMemoryFactory
{
    /// <summary>Determines whether both splits are eventually equal when concatenating all slices.</summary>
    /// <typeparam name="T">The type of <see cref="SplitMemory{T}"/>.</typeparam>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>
    /// The value <paramref langword="true"/> if both sequences are equal, otherwise; <paramref langword="false"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool ConcatEqual<T>(this SplitMemory<T> left, SplitMemory<T> right)
        where T : IEquatable<T>?
    {
        if (left == right)
            return true;

        if (left.GetEnumerator() is var e1 && right.GetEnumerator() is var e2 && !e1.MoveNext())
            return !e2.MoveNext();

        if (!e2.MoveNext())
            return false;

        ReadOnlyMemory<T>
            reader1 = e1.Current,
            reader2 = e2.Current;

        while (true)
            if (Next(ref reader1, ref reader2, ref e1, ref e2, out var ret))
                return ret;
    }

    /// <summary>Determines whether both splits are equal.</summary>
    /// <typeparam name="T">The type of <see cref="SplitMemory{T}"/>.</typeparam>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>
    /// The value <paramref langword="true"/> if both sequences are equal, otherwise; <paramref langword="false"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool SequenceEqual<T>(this SplitMemory<T> left, SplitMemory<T> right)
        where T : IEquatable<T>?
    {
        if (left == right)
            return true;

        var e1 = left.GetEnumerator();
        var e2 = right.GetEnumerator();

        while (e1.MoveNext())
            if (!(e2.MoveNext() && e1.Current.SequenceEqual(e2.Current)))
                return false;

        return !e2.MoveNext();
    }

    /// <summary>Splits a memory by the specified separator.</summary>
    /// <typeparam name="T">The type of element from the memory.</typeparam>
    /// <param name="memory">The memory to split.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>The enumerable object that references the parameter <paramref name="memory"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<T> SplitAny<T>(this ReadOnlyMemory<T> memory, ReadOnlyMemory<T> separator)
        where T : IEquatable<T> =>
        new(memory, separator, true);

    /// <inheritdoc cref="SplitAny{T}(ReadOnlyMemory{T}, ReadOnlyMemory{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<T> SplitAny<T>(this Memory<T> memory, ReadOnlyMemory<T> separator)
        where T : IEquatable<T> =>
        ((ReadOnlyMemory<T>)memory).SplitAny(separator);

    /// <summary>Splits a memory by the specified separator.</summary>
    /// <typeparam name="T">The type of element from the memory.</typeparam>
    /// <param name="memory">The memory to split.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>The enumerable object that references the parameter <paramref name="memory"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<T> SplitAll<T>(this ReadOnlyMemory<T> memory, ReadOnlyMemory<T> separator)
        where T : IEquatable<T> =>
        new(memory, separator, false);

    /// <inheritdoc cref="SplitAll{T}(ReadOnlyMemory{T}, ReadOnlyMemory{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<T> SplitAll<T>(this Memory<T> memory, ReadOnlyMemory<T> separator)
        where T : IEquatable<T> =>
        ((ReadOnlyMemory<T>)memory).SplitAll(separator);
#if NET8_0_OR_GREATER
    /// <inheritdoc cref="SplitAny{T}(ReadOnlyMemory{T}, ReadOnlyMemory{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<T> SplitOn<T>(this ReadOnlyMemory<T> memory, SearchValues<T> separator)
        where T : IEquatable<T> =>
        new(memory, separator);

    /// <inheritdoc cref="SplitAny{T}(ReadOnlyMemory{T}, ReadOnlyMemory{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<T> SplitOn<T>(this Memory<T> memory, SearchValues<T> separator)
        where T : IEquatable<T> =>
        ((ReadOnlyMemory<T>)memory).SplitOn(separator);
#endif

    /// <summary>Copies the values to a new <see cref="List{T}"/>.</summary>
    /// <param name="split">The instance to get the list from.</param>
    /// <returns>The list containing the copied values of this instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static List<string> ToList(this SplitMemory<char> split) => [..split.Select(next => next.ToString())];

    /// <summary>Copies the values to a new <see cref="List{T}"/>.</summary>
    /// <typeparam name="T">The type of element from the memory.</typeparam>
    /// <param name="split">The instance to get the list from.</param>
    /// <returns>The list containing the copied values of this instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static List<T[]> ToList<T>(this SplitMemory<T> split)
        where T : IEquatable<T>? =>
        [..split.Select(next => next.ToArray())];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool Next<T>(
        ref ReadOnlyMemory<T> reader1,
        ref ReadOnlyMemory<T> reader2,
        ref SplitMemory<T>.Enumerator e1,
        ref SplitMemory<T>.Enumerator e2,
        out bool ret
    )
        where T : IEquatable<T>?
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
    static bool SameLength<T>(
        ref ReadOnlyMemory<T> reader1,
        ref ReadOnlyMemory<T> reader2,
        ref SplitMemory<T>.Enumerator e1,
        ref SplitMemory<T>.Enumerator e2,
        ref bool ret
    )
        where T : IEquatable<T>?
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

    /// <inheritdoc cref="SplitAny{T}(ReadOnlyMemory{T}, ReadOnlyMemory{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<char> SplitAny(this string memory, string separator) =>
        memory.AsMemory().SplitAny(separator.AsMemory());

    /// <inheritdoc cref="SplitAny{T}(ReadOnlyMemory{T}, ReadOnlyMemory{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<char> SplitAny(this string memory, ReadOnlyMemory<char> separator) =>
        memory.AsMemory().SplitAny(separator);

    /// <inheritdoc cref="SplitAll{T}(ReadOnlyMemory{T}, ReadOnlyMemory{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<char> SplitAll(this string memory, string separator) =>
        memory.AsMemory().SplitAll(separator.AsMemory());

    /// <inheritdoc cref="SplitAll{T}(ReadOnlyMemory{T}, ReadOnlyMemory{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<char> SplitAll(this string memory, ReadOnlyMemory<char> separator) =>
        memory.AsMemory().SplitAll(separator);

    /// <inheritdoc cref="SplitLines(ReadOnlyMemory{char})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<char> SplitLines(this string memory) => memory.AsMemory().SplitLines();

    /// <summary>Splits a memory by line breaks.</summary>
    /// <remarks><para>Line breaks are considered any character in <see cref="Whitespaces.Breaking"/>.</para></remarks>
    /// <param name="memory">The memory to split.</param>
    /// <returns>The enumerable object that references the parameter <paramref name="memory"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<char> SplitLines(this ReadOnlyMemory<char> memory) =>
#if NET8_0_OR_GREATER
        new(memory, Whitespaces.BreakingSearch);
#else
        new(memory, Whitespaces.Breaking.AsMemory(), true);
#endif

    /// <inheritdoc cref="SplitLines(ReadOnlyMemory{char})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<char> SplitLines(this Memory<char> memory) => ((ReadOnlyMemory<char>)memory).SplitLines();
#if NET8_0_OR_GREATER
    /// <inheritdoc cref="SplitAny{T}(ReadOnlyMemory{T}, ReadOnlyMemory{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<char> SplitOn(this string memory, SearchValues<char> separator) =>
        memory.AsMemory().SplitOn(separator);
#endif

    /// <inheritdoc cref="SplitWhitespace(ReadOnlyMemory{char})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<char> SplitWhitespace(this string memory) => memory.AsMemory().SplitWhitespace();

    /// <summary>Splits a memory by whitespace.</summary>
    /// <remarks><para>Whitespace is considered any character in <see cref="Whitespaces.Unicode"/>.</para></remarks>
    /// <param name="memory">The memory to split.</param>
    /// <returns>The enumerable object that references the parameter <paramref name="memory"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<char> SplitWhitespace(this ReadOnlyMemory<char> memory) =>
#if NET8_0_OR_GREATER
        new(memory, Whitespaces.UnicodeSearch);
#else
        new(memory, Whitespaces.Unicode.AsMemory(), true);
#endif

    /// <inheritdoc cref="SplitWhitespace(ReadOnlyMemory{char})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<char> SplitWhitespace(this Memory<char> memory) =>
        ((ReadOnlyMemory<char>)memory).SplitWhitespace();
}

/// <summary>Represents a split entry.</summary>
/// <typeparam name="T">The type of element from the memory.</typeparam>
[StructLayout(LayoutKind.Auto)]
#if CSHARPREPL
public
#endif
#if !NO_READONLY_STRUCTS
readonly
#endif
    partial struct SplitMemory<T> : IEquatable<SplitMemory<T>>
    where T : IEquatable<T>?
{
    public
#if !NO_READONLY_STRUCTS
        readonly
#endif
        struct Of<TStrategy>(SplitMemory<T> split) : IEquatable<Of<TStrategy>>, IEnumerable<ReadOnlyMemory<T>>
        where TStrategy : SplitSpan<T>.IStrategy
    {
        /// <summary>Gets the line.</summary>
        public ReadOnlyMemory<T> Body
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => split.Body;
        }

        /// <summary>Gets the separator.</summary>
        public ReadOnlyMemory<T> Separator
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => split.Separator;
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
            typeof(T) == typeof(char)
                ? Aggregate(new(), StringBuilderAccumulator()).ToString()
                : this.ToList().Stringify(3, true);

        /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        public Enumerator GetEnumerator() => new(split);

        /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        IEnumerator<ReadOnlyMemory<T>> IEnumerable<ReadOnlyMemory<T>>.GetEnumerator() => GetEnumerator();

        /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>Gets the first element.</summary>
        /// <returns>The first memory from this instance.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        public ReadOnlyMemory<T> First() => GetEnumerator() is var e && e.MoveNext() ? e.Current : default;

        /// <summary>Gets the last element.</summary>
        /// <returns>The last memory from this instance.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        public ReadOnlyMemory<T> Last()
#if NET8_0_OR_GREATER
        =>
            _search.ToAddress() switch
            {
                0 => Body.Span.LastIndexOf(Separator.Span),
                1 => Body.Span.LastIndexOfAny(Separator.Span),
                _ => Body.Span.LastIndexOfAny(_search!),
            } is not -1 and var index
                ? Body[index..]
                : default;
#else
            =>
                (IsAny ? Body.Span.LastIndexOfAny(Separator.Span) : Body.Span.LastIndexOf(Separator.Span))
                is not -1 and var i
                    ? Body[i..]
                    : default;
#endif

        /// <summary>Gets the single element.</summary>
        /// <returns>The single memory from this instance.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        public ReadOnlyMemory<T> Single() =>
            GetEnumerator() is var e && e.MoveNext() && e.Current is var ret && !e.MoveNext() ? ret : default;

        /// <summary>Gets the accumulated result of a set of callbacks where each element is passed in.</summary>
        /// <typeparam name="TAccumulator">The type of the accumulator value.</typeparam>
        /// <param name="seed">The accumulator.</param>
        /// <param name="func">An accumulator function to be invoked on each element.</param>
        /// <returns>The accumulated result of <paramref name="seed"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining), MustUseReturnValue]
        public TAccumulator Aggregate<TAccumulator>(
            TAccumulator seed,
            [InstantHandle, RequireStaticDelegate] Func<TAccumulator, ReadOnlyMemory<T>, TAccumulator> func
        )
        {
            var accumulator = seed;

            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (var next in this)
                accumulator = func(accumulator, next);

            return accumulator;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] // ReSharper disable once RedundantUnsafeContext
        static unsafe Func<StringBuilder, ReadOnlyMemory<T>, StringBuilder> StringBuilderAccumulator() =>
            static (builder, memory) => builder.Append(Unsafe.As<ReadOnlyMemory<T>, ReadOnlyMemory<char>>(ref memory));

        /// <summary>Represents the enumeration object that views <see cref="SplitMemory{T}"/>.</summary>
        [StructLayout(LayoutKind.Auto)]
        public partial struct Enumerator(SplitMemory<T> split) : IEnumerator<ReadOnlyMemory<T>>
        {
            [ValueRange(-1, int.MaxValue)]
            int _end = -1;

            /// <summary>Gets the current index.</summary>
            public readonly int Index
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining), Pure, ValueRange(-1, int.MaxValue)] get => _end;
            }

            /// <inheritdoc cref="IEnumerator{T}.Current"/>
            public ReadOnlyMemory<T> Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get;
                private set;
            }

            /// <summary>Gets the enumerable used to create this instance.</summary>
            public readonly SplitMemory<T> Enumerable
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => split;
            }

            /// <inheritdoc />
            readonly object IEnumerator.Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => Current;
            }

            /// <summary>
            /// Sets the enumerator to its initial position, which is before the first element in the collection.
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Reset() => _end = -1;

            /// <inheritdoc />
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly void IDisposable.Dispose() { }

            /// <summary>Advances the enumerator to the next element of the collection.</summary>
            /// <returns>
            /// <see langword="true"/> if the enumerator was successfully advanced to the next element;
            /// <see langword="false"/> if the enumerator has passed the end of the collection.
            /// </returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext()
            {
                var body = split.Body;
                var separator = split.Separator;

                if (separator.IsEmpty)
                    return !body.IsEmpty && Current.IsEmpty && (Current = body) is var _;

                while (SplitSpan<T>.Enumerator.Step(
                    split.IsAny,
#if NET8_0_OR_GREATER
                search,
#endif
                    body.Span,
                    separator.Span,
                    ref _end,
                    out var start
                ))
                    if (start != _end)
                        return (Current = body[start.._end]) is var _;

                return false;
            }
        }
    }

    /// <summary>Initializes a new instance of the <see cref="SplitMemory{T}"/> struct.</summary>
    /// <param name="body">The line to split.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SplitMemory(ReadOnlyMemory<T> body) => Body = body;

    /// <summary>Initializes a new instance of the <see cref="SplitMemory{T}"/> struct.</summary>
    /// <param name="body">The line to split.</param>
    /// <param name="separator">The characters for separation.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SplitMemory(ReadOnlyMemory<T> body, ReadOnlyMemory<T> separator)
    {
        Body = body;
        Separator = separator;
    }
#if NET8_0_OR_GREATER
    /// <summary>
    /// Initializes a new instance of the <see cref="SplitMemory{T}"/> struct
    /// where <see cref="IsAny"/> is <see langword="true"/>.
    /// </summary>
    /// <remarks><para>This constructor is only available starting from .NET 8.0 or later.</para></remarks>
    /// <param name="body">The line to split.</param>
    /// <param name="separator">The characters for separation.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SplitMemory(ReadOnlyMemory<T> body, SearchValues<T> separator)
    {
        Body = body;
        _search = separator;
    }
#endif

    /// <summary>Gets the specified index.</summary>
    /// <param name="index">The index to get.</param>
    /// <exception cref="ArgumentOutOfRangeException">The parameter <paramref name="index"/> is negative.</exception>
    public ReadOnlyMemory<T> this[[NonNegativeValue] int index]
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

    /// <summary>Gets the empty split memory.</summary>
    public static SplitMemory<T> Empty
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => default;
    }

    /// <summary>
    /// Gets a value indicating whether it should split based on any character in <see cref="Separator"/>,
    /// or if all of them match.
    /// </summary>
#if NET8_0_OR_GREATER
    [MemberNotNullWhen(true, nameof(_search))]
#endif
    public bool IsAny
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        get =>
#if NET8_0_OR_GREATER
            _search is not null
#else
            _isAny
#endif
        ||
            Separator.Length is 1;
    }

    /// <summary>Gets the line.</summary>
    public ReadOnlyMemory<T> Body
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] init;
    }

    /// <summary>Gets the separator.</summary>
    public ReadOnlyMemory<T> Separator
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] init;
    }

    /// <summary>Gets a <see cref="SplitSpan{T}"/> from the memory region.</summary>
    public SplitSpan<T> SplitSpan
    {
        // ReSharper disable once NullableWarningSuppressionIsUsed
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        get =>
#if NET8_0_OR_GREATER
            _search.ToAddress() > 1
                ? new(Body.Span, _search!)
                :
#endif
                new(Body.Span, Separator.Span, IsAny);
    }

    /// <summary>Determines whether both splits are equal.</summary>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>Whether both splits are equal.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool operator ==(SplitMemory<T> left, SplitMemory<T> right) => left.Equals(right);

    /// <summary>Determines whether both splits are not equal.</summary>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>Whether both splits are not equal.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool operator !=(SplitMemory<T> left, SplitMemory<T> right) => !left.Equals(right);

    /// <summary>Separates the head from the tail of this <see cref="SplitMemory{T}"/>.</summary>
    /// <param name="head">The first element of this enumeration.</param>
    /// <param name="tail">The rest of this enumeration.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Deconstruct(out ReadOnlyMemory<T> head, out SplitMemory<T> tail)
    {
        if (GetEnumerator() is var e && !e.MoveNext())
        {
            head = default;
            tail = default;
            return;
        }

        head = e.Current;
        tail = this with { Body = Body[e.Index..] };
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public override bool Equals(object? obj) => false;

    /// <inheritdoc cref="IEquatable{T}.Equals(T)" />
    // ReSharper disable NullableWarningSuppressionIsUsed
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public bool Equals(SplitMemory<T> other) =>
#if NET8_0_OR_GREATER
        _search == other._search &&
#endif // ReSharper disable once ArrangeRedundantParentheses
#pragma warning disable SA1119, RCS1032
        (Body.IsEmpty && other.Body.IsEmpty ||
        Separator.IsEmpty && other.Separator.IsEmpty && Body.SequenceEqual(other.Body) ||
        IsAny == other.IsAny && Separator.SequenceEqual(other.Separator) && Body.SequenceEqual(other.Body));
#pragma warning restore SA1119, RCS1032

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public override int GetHashCode() => unchecked(IsAny.GetHashCode() * 127);
}
#endif
