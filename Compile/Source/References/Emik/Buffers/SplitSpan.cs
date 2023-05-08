// SPDX-License-Identifier: MPL-2.0

// ReSharper disable BadPreprocessorIndent CheckNamespace StructCanBeMadeReadOnly
namespace Emik.Morsels;
#pragma warning disable IDE0250, MA0102, SA1137
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
/// <summary>Methods to split spans into multiple spans.</summary>
#pragma warning disable MA0048
static partial class SplitFactory
#pragma warning restore MA0048
{
    /// <inheritdoc cref="SplitLines(ReadOnlySpan{char})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char> SplitLines(this string s) => s.AsSpan().SplitLines();

    /// <summary>Splits a span by line breaks.</summary>
    /// <remarks><para>Line breaks are considered any character in <see cref="Whitespaces.Breaking"/>.</para></remarks>
    /// <param name="s">The span to split.</param>
    /// <returns>The enumerable object that references the parameter <paramref name="s"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char> SplitLines(this ReadOnlySpan<char> s) => new(s, Whitespaces.Breaking.AsSpan());

    /// <inheritdoc cref="SplitLines(ReadOnlySpan{char})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char> SplitLines(this Span<char> s) => ((ReadOnlySpan<char>)s).SplitLines();

    /// <inheritdoc cref="SplitTerminated{T}(ReadOnlySpan{T}, ReadOnlySpan{T}, IEqualityComparer{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char> SplitTerminated(
        this string s,
        string sep,
        IEqualityComparer<char>? comparer = null
    ) =>
        s.AsSpan().SplitTerminated(sep.AsSpan(), comparer);

    /// <inheritdoc cref="SplitWhitespace(ReadOnlySpan{char})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char> SplitWhitespace(this string s) => s.AsSpan().SplitWhitespace();

    /// <summary>Splits a span by whitespace.</summary>
    /// <remarks><para>Whitespace is considered any character in <see cref="Whitespaces.Unicode"/>.</para></remarks>
    /// <param name="s">The span to split.</param>
    /// <returns>The enumerable object that references the parameter <paramref name="s"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char> SplitWhitespace(this ReadOnlySpan<char> s) => new(s, Whitespaces.Unicode.AsSpan());

    /// <inheritdoc cref="SplitWhitespace(ReadOnlySpan{char})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char> SplitWhitespace(this Span<char> s) => ((ReadOnlySpan<char>)s).SplitWhitespace();

    /// <summary>Splits a span by the specified separator.</summary>
    /// <typeparam name="T">The type of element from the span.</typeparam>
    /// <param name="s">The span to split.</param>
    /// <param name="sep">The separator.</param>
    /// <param name="comparer">The comparer to use.</param>
    /// <returns>The enumerable object that references the parameter <paramref name="s"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<T> SplitTerminated<T>(
        this ReadOnlySpan<T> s,
        ReadOnlySpan<T> sep,
        Func<T, T, bool>? comparer = null
    ) =>
        new(s, sep, comparer);

    /// <inheritdoc cref="SplitTerminated{T}(ReadOnlySpan{T}, ReadOnlySpan{T}, IEqualityComparer{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<T> SplitTerminated<T>(
        this ReadOnlySpan<T> s,
        ReadOnlySpan<T> sep,
        IEqualityComparer<T>? comparer
    ) =>
        s.SplitTerminated(sep, comparer is null ? null : comparer.Equals);

    /// <inheritdoc cref="SplitTerminated{T}(ReadOnlySpan{T}, ReadOnlySpan{T}, IEqualityComparer{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<T> SplitTerminated<T>(
        this Span<T> s,
        ReadOnlySpan<T> sep,
        Func<T, T, bool>? comparer = null
    ) =>
        ((ReadOnlySpan<T>)s).SplitTerminated(sep, comparer);

    /// <inheritdoc cref="SplitTerminated{T}(ReadOnlySpan{T}, ReadOnlySpan{T}, IEqualityComparer{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<T> SplitTerminated<T>(
        this Span<T> s,
        ReadOnlySpan<T> sep,
        IEqualityComparer<T>? comparer
    ) =>
        ((ReadOnlySpan<T>)s).SplitTerminated(sep, comparer is null ? null : comparer.Equals);
}
#endif

/// <summary>Represents a split entry.</summary>
/// <typeparam name="T">The type of element from the span.</typeparam>
[StructLayout(LayoutKind.Auto)]
#if !NO_READONLY_STRUCTS
readonly
#endif
#if !NO_REF_STRUCTS
    ref
#endif
    partial struct SplitSpan<T>
#if UNMANAGED_SPAN
    where T : unmanaged
#endif
{
    /// <summary>Initializes a new instance of the <see cref="SplitSpan{T}"/> struct.</summary>
    /// <param name="span">The line to split.</param>
    /// <param name="separator">The characters for separation.</param>
    /// <param name="comparer">The comparison to determine when to split.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SplitSpan(
        ReadOnlySpan<T> span,
        ReadOnlySpan<T> separator,
        Func<T, T, bool>? comparer = null
    )
    {
        Comparer = comparer ?? EqualityComparer<T>.Default.Equals;
        Span = span;
        Separator = separator;
    }

    /// <summary>Gets the comparer that determines when to split.</summary>
    public Func<T, T, bool> Comparer { [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get; }

    /// <summary>Gets the line.</summary>
    public ReadOnlySpan<T> Span { [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get; }

    /// <summary>Gets the separator.</summary>
    public ReadOnlySpan<T> Separator { [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get; }

    /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
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
            while (true)
            {
                var start = ++_end;
                Terminate();

                if (_end > _split.Span.Length)
                    return false;

                Current = _split.Span[start.._end];

                if (Current.IsEmpty)
                    continue;

                return true;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Terminate()
        {
            for (; _end < _split.Span.Length; _end++)
                foreach (var t in _split.Separator)
                    if (_split.Comparer(_split.Span[_end], t))
                        return;
        }
    }
}
