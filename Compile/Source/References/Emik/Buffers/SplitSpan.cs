// SPDX-License-Identifier: MPL-2.0

// ReSharper disable BadPreprocessorIndent CheckNamespace InvertIf StructCanBeMadeReadOnly
namespace Emik.Morsels;
#pragma warning disable 8618, IDE0250, MA0071, MA0102, SA1137
using static Span;

/// <summary>Methods to split spans into multiple spans.</summary>
#pragma warning disable MA0048
static partial class SplitFactory
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
    public static bool ConcatEqual<T>(this SplitSpan<T> left, SplitSpan<T> right)
        where T : unmanaged, IEquatable<T>
    {
        if (left == right)
            return true;

        if (left.GetEnumerator() is var e1 && right.GetEnumerator() is var e2 && !e1.MoveNext())
            return !e2.MoveNext();

        if (!e2.MoveNext())
            return false;

        // SplitSpan<T>.Enumerator must return slices that when combined are at least 1 entry shorter.
        int length1 = left.Body.Length - 1,
            length2 = right.Body.Length - 1;

        Span<T>
            writer1 = e1.Current.Length > length1 ? default :
                length1 <= Stackalloc ? stackalloc T[length1] : new T[length1],
            writer2 = e2.Current.Length > length2 ? default :
                length2 <= Stackalloc ? stackalloc T[length2] : new T[length2];

        ReadOnlySpan<T>
            reader1 = writer1.IsEmpty ? e1.Current : writer1,
            reader2 = writer2.IsEmpty ? e2.Current : writer2;

        if (!writer1.IsEmpty)
            do
            {
                e1.Current.CopyTo(writer1);
                writer1 = writer1[e1.Current.Length..];

                if (reader1.Length - writer1.Length > reader2.Length)
                    return false;
            } while (e1.MoveNext());

        if (!writer2.IsEmpty)
            do
            {
                e2.Current.CopyTo(writer2);
                writer2 = writer2[e2.Current.Length..];

                if (reader2.Length - writer2.Length > reader1.Length)
                    return false;
            } while (e2.MoveNext());

        return reader1[..^writer1.Length].SequenceEqual(reader2[..^writer2.Length]);
    }

    /// <summary>Determines whether both splits are equal.</summary>
    /// <typeparam name="T">The type of <see cref="SplitSpan{T}"/>.</typeparam>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>
    /// The value <paramref langword="true"/> if both sequences are equal, otherwise; <paramref langword="false"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool SequenceEqual<T>(this SplitSpan<T> left, SplitSpan<T> right)
#if UNMANAGED_SPAN
        where T : unmanaged, IEquatable<T>?
#else
        where T : IEquatable<T>?
#endif
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

    /// <summary>Splits a span by the specified separator.</summary>
    /// <typeparam name="T">The type of element from the span.</typeparam>
    /// <param name="span">The span to split.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>The enumerable object that references the parameter <paramref name="span"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<T> SplitAny<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> separator)
#if UNMANAGED_SPAN
        where T : unmanaged, IEquatable<T>
#else
        where T : IEquatable<T>
#endif
        =>
            new(span, separator, true);

    /// <inheritdoc cref="SplitAny{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<T> SplitAny<T>(this Span<T> span, ReadOnlySpan<T> separator)
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
    public static SplitSpan<T> SplitAll<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> separator)
#if UNMANAGED_SPAN
        where T : unmanaged, IEquatable<T>
#else
        where T : IEquatable<T>
#endif
        =>
            new(span, separator, false);

    /// <inheritdoc cref="SplitAll{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<T> SplitAll<T>(this Span<T> span, ReadOnlySpan<T> separator)
#if UNMANAGED_SPAN
        where T : unmanaged, IEquatable<T>
#else
        where T : IEquatable<T>
#endif
        =>
            ((ReadOnlySpan<T>)span).SplitAll(separator);

    /// <summary>Copies the values to a new <see cref="List{T}"/>.</summary>
    /// <param name="split">The instance to get the list from.</param>
    /// <returns>The list containing the copied values of this instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static List<string> ToList(this SplitSpan<char> split)
    {
        List<string> ret = new();

        foreach (var next in split)
            ret.Add(next.ToString());

        return ret;
    }

    /// <summary>Copies the values to a new <see cref="List{T}"/>.</summary>
    /// <typeparam name="T">The type of element from the span.</typeparam>
    /// <param name="split">The instance to get the list from.</param>
    /// <returns>The list containing the copied values of this instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static List<T[]> ToList<T>(this SplitSpan<T> split)
#if UNMANAGED_SPAN
        where T : unmanaged, IEquatable<T>
#else
        where T : IEquatable<T>
#endif
    {
        List<T[]> ret = new();

        foreach (var next in split)
            ret.Add(next.ToArray());

        return ret;
    }
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
    /// <inheritdoc cref="SplitAny{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char> SplitAny(this string span, string separator) =>
        span.AsSpan().SplitAny(separator.AsSpan());

    /// <inheritdoc cref="SplitAny{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char> SplitAny(this string span, ReadOnlySpan<char> separator) =>
        span.AsSpan().SplitAny(separator);

    /// <inheritdoc cref="SplitAll{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char> SplitAll(this string span, string separator) =>
        span.AsSpan().SplitAll(separator.AsSpan());

    /// <inheritdoc cref="SplitAll{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char> SplitAll(this string span, ReadOnlySpan<char> separator) =>
        span.AsSpan().SplitAll(separator);

    /// <inheritdoc cref="SplitLines(ReadOnlySpan{char})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char> SplitLines(this string span) => span.AsSpan().SplitLines();

    /// <summary>Splits a span by line breaks.</summary>
    /// <remarks><para>Line breaks are considered any character in <see cref="Whitespaces.Breaking"/>.</para></remarks>
    /// <param name="span">The span to split.</param>
    /// <returns>The enumerable object that references the parameter <paramref name="span"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char> SplitLines(this ReadOnlySpan<char> span) =>
        new(span, Whitespaces.Breaking.AsSpan(), true);

    /// <inheritdoc cref="SplitLines(ReadOnlySpan{char})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char> SplitLines(this Span<char> span) => ((ReadOnlySpan<char>)span).SplitLines();

    /// <inheritdoc cref="SplitWhitespace(ReadOnlySpan{char})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char> SplitWhitespace(this string span) => span.AsSpan().SplitWhitespace();

    /// <summary>Splits a span by whitespace.</summary>
    /// <remarks><para>Whitespace is considered any character in <see cref="Whitespaces.Unicode"/>.</para></remarks>
    /// <param name="span">The span to split.</param>
    /// <returns>The enumerable object that references the parameter <paramref name="span"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char> SplitWhitespace(this ReadOnlySpan<char> span) =>
        new(span, Whitespaces.Unicode.AsSpan(), true);

    /// <inheritdoc cref="SplitWhitespace(ReadOnlySpan{char})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char> SplitWhitespace(this Span<char> span) => ((ReadOnlySpan<char>)span).SplitWhitespace();
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
    readonly bool _isAny;

    /// <summary>Initializes a new instance of the <see cref="SplitSpan{T}"/> struct.</summary>
    /// <param name="body">The line to split.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SplitSpan(ReadOnlySpan<T> body) => Body = body;

    /// <summary>Initializes a new instance of the <see cref="SplitSpan{T}"/> struct.</summary>
    /// <param name="body">The line to split.</param>
    /// <param name="separator">The characters for separation.</param>
    /// <param name="isAny">When <see langword="true"/>, treat separator as a big pattern match.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SplitSpan(ReadOnlySpan<T> body, ReadOnlySpan<T> separator, bool isAny)
    {
        Body = body;
        Separator = separator;
        _isAny = isAny;
    }

    /// <summary>Gets the empty split span.</summary>
    public static SplitSpan<T> Empty
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)] get => default;
    }

    /// <summary>
    /// Gets a value indicating whether it should split based on any character in <see cref="Separator"/>,
    /// or if all of them match.
    /// </summary>
    public bool IsAny
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)] get => _isAny || Separator.Length is 1;
    }

    /// <summary>Gets the line.</summary>
    public ReadOnlySpan<T> Body { [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get; }

    /// <summary>Gets the separator.</summary>
    public ReadOnlySpan<T> Separator { [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get; }

    /// <summary>Determines whether both splits are equal.</summary>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>Whether both splits are equal.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool operator ==(SplitSpan<T> left, SplitSpan<T> right) => left.Equals(right);

    /// <summary>Determines whether both splits are not equal.</summary>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>Whether both splits are not equal.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool operator !=(SplitSpan<T> left, SplitSpan<T> right) => !left.Equals(right);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public override bool Equals(object? other) => false;

    /// <inheritdoc cref="IEquatable{T}.Equals(T?)" />
    // ReSharper disable NullableWarningSuppressionIsUsed
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public bool Equals(SplitSpan<T> other) =>
        Body.IsEmpty && other.Body.IsEmpty ||
        Separator.IsEmpty && other.Separator.IsEmpty && Body.SequenceEqual(other.Body) ||
        IsAny == other.IsAny && Separator.SequenceEqual(other.Separator) && Body.SequenceEqual(other.Body);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public override int GetHashCode() => unchecked(IsAny.GetHashCode() * 31);

    /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
    // ReSharper restore NullableWarningSuppressionIsUsed
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public Enumerator GetEnumerator() => new(this);

    /// <summary>Represents the enumeration object that views <see cref="SplitSpan{T}"/>.</summary>
    [StructLayout(LayoutKind.Auto)]
    public
#if !NO_REF_STRUCTS
        ref
#endif
        partial struct Enumerator
    {
        readonly SplitSpan<T> _split;

        [ValueRange(-1, int.MaxValue)]
        int _end = -1;

        /// <summary>Initializes a new instance of the <see cref="Enumerator"/> struct.</summary>
        /// <param name="split">Tne entry to enumerate.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Enumerator(SplitSpan<T> split) => _split = split;

        /// <inheritdoc cref="IEnumerator{T}.Current"/>
        public ReadOnlySpan<T> Current { [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get; private set; }

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the collection.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset() => _end = -1;

        /// <summary>Advances the enumerator to the next element of the collection.</summary>
        /// <returns>
        /// <see langword="true"/> if the enumerator was successfully advanced to the next element;
        /// <see langword="false"/> if the enumerator has passed the end of the collection.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            var body = _split.Body;
            var separator = _split.Separator;

            if (separator.IsEmpty)
                return Current.IsEmpty && (Current = body) is var _;

            while (Step(_split.IsAny, body, separator, ref _end, out var start))
                if (start != _end)
                    return (Current = body[start.._end]) is var _;

            return false;
        }

        /// <summary>Attempts to step through to the next slice.</summary>
        /// <param name="isAny">Determines whether to call <see cref="StepAny"/> or <see cref="StepAll"/>.</param>
        /// <param name="body">The reference to its body.</param>
        /// <param name="separator">The reference to its separator.</param>
        /// <param name="end">The ending index of the slice.</param>
        /// <param name="start">The starting index of the slice.</param>
        /// <returns>Whether or not to continue looping.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool Step(
            bool isAny,
            in ReadOnlySpan<T> body,
            in ReadOnlySpan<T> separator,
            ref int end,
            out int start
        ) =>
            isAny ? StepAny(body, separator, ref end, out start) : StepAll(body, separator, ref end, out start);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool StepAll(in ReadOnlySpan<T> body, in ReadOnlySpan<T> separator, ref int end, out int start)
        {
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
            Unsafe.SkipInit(out start);
#else
            start = 0;
#endif

            if (body.Length is var bodyLength && separator.Length is var length && bodyLength == length)
            {
                if (body.SequenceEqual(separator))
                    return false;

                start = 0;
                end = bodyLength;
                return true;
            }

            start = end is -1 ? ++end : end += length;

            while (end <= bodyLength)
                switch (body[end..].IndexOf(separator))
                {
                    case -1:
                        end = bodyLength;
                        return true;
                    case 0:
                        end = start += length;
                        continue;
                    case var i:
                        end += i;
                        return true;
                }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool StepAny(in ReadOnlySpan<T> body, in ReadOnlySpan<T> separator, ref int end, out int start)
        {
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
            Unsafe.SkipInit(out start);
#else
            start = 0;
#endif

            if (body.Length is var bodyLength && ++end >= bodyLength)
                return false;

            start = end;
            goto Begin;

        Increment:
            start++;
            end++;

        Begin:
            var min = int.MaxValue;

            foreach (var next in separator)
                switch (body[end..].IndexOf(next))
                {
                    case -1: continue;
                    case 0: goto Increment;
                    case var i when i < min:
                        min = i;
                        continue;
                }

            end = min is int.MaxValue ? bodyLength : end + min;
            return true;
        }
    }
}
