// SPDX-License-Identifier: MPL-2.0

// ReSharper disable BadPreprocessorIndent CheckNamespace ConvertToAutoPropertyWhenPossible InvertIf RedundantNameQualifier RedundantReadonlyModifier RedundantUsingDirective StructCanBeMadeReadOnly UseSymbolAlias

namespace Emik.Morsels;
#pragma warning disable IDE0032
using static Span;
using static SplitSpanFactory;

/// <inheritdoc cref="SplitSpan{TBody, TSeparator, TStrategy}"/>
partial struct SplitSpan<TBody, TSeparator, TStrategy>
{
    /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly ReversedEnumerator GetReversedEnumerator() => new(this);

    /// <summary>
    /// Returns itself but with the number of elements from the end specified skipped. This is evaluated eagerly.
    /// </summary>
    /// <param name="count">The number of elements to skip from the end.</param>
    /// <returns>Itself but skipping from the end the parameter <paramref name="count"/> number of elements.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly SplitSpan<TBody, TSeparator, TStrategy> SkippedLast([NonNegativeValue] int count)
    {
        Enumerator e = this;

        for (; count > 0 && e.MoveNext(); count--) { }

        return e.SplitSpan;
    }

    /// <summary>
    /// Represents the backwards enumeration object that views <see cref="SplitSpan{T, TSeparator, TStrategy}"/>.
    /// </summary>
    [StructLayout(LayoutKind.Auto)]
    [method: MethodImpl(MethodImplOptions.AggressiveInlining)]
    public
#if !NO_REF_STRUCTS
        ref
#endif
        partial struct ReversedEnumerator(ReadOnlySpan<TBody> body, ReadOnlySpan<TSeparator> separator)
    {
        readonly ReadOnlySpan<TSeparator> _separator = separator;

        ReadOnlySpan<TBody> _body = body, _current;

        /// <summary>Initializes a new instance of the <see cref="ReversedEnumerator"/> struct.</summary>
        /// <param name="body">The body.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReversedEnumerator(ReadOnlySpan<TBody> body)
            : this(body, default) { }

        /// <summary>Initializes a new instance of the <see cref="ReversedEnumerator"/> struct.</summary>
        /// <param name="split">The enumerable to enumerate.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReversedEnumerator(SplitSpan<TBody, TSeparator, TStrategy> split)
            : this(split._body, split._separator) { }

        /// <inheritdoc cref="SplitSpan{T, TSeparator, TStrategy}.Body"/>
        public readonly ReadOnlySpan<TBody> Body
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => _body;
            [MethodImpl(MethodImplOptions.AggressiveInlining)] init => _body = value;
        }

        /// <inheritdoc cref="IEnumerator{T}.Current"/>
        public readonly ReadOnlySpan<TBody> Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => _current;
        }

        /// <inheritdoc cref="SplitSpan{T, TSeparator, TStrategy}.Separator"/>
        public readonly ReadOnlySpan<TSeparator> Separator
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => _separator;
            [MethodImpl(MethodImplOptions.AggressiveInlining)] init => _separator = value;
        }

        /// <summary>
        /// Reconstructs the <see cref="SplitSpan{TBody, TSeparator, TStrategy}"/> based on the current state.
        /// </summary>
        public readonly SplitSpan<TBody, TSeparator, TStrategy> SplitSpan
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
            get => new(_body, _separator);
        }

        /// <summary>
        /// Explicitly converts the parameter by creating the new instance
        /// of <see cref="ReversedEnumerator"/> by using the constructor
        /// <see cref="SplitSpan{TBody, TSeparator, TStrategy}.ReversedEnumerator(ReadOnlySpan{TBody})"/>.
        /// </summary>
        /// <param name="body">The parameter to pass onto the constructor.</param>
        /// <returns>
        /// The new instance of <see cref="ReversedEnumerator"/> by passing
        /// the parameter <paramref name="body"/> to the constructor
        /// <see cref="SplitSpan{TBody, TSeparator, TStrategy}.ReversedEnumerator(ReadOnlySpan{TBody})"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        public static explicit operator SplitSpan<TBody, TSeparator, TStrategy>.ReversedEnumerator(
            ReadOnlySpan<TBody> body
        ) =>
            new(body);

        /// <summary>
        /// Implicitly converts the parameter by creating the new instance
        /// of <see cref="ReversedEnumerator"/> by using the constructor
        /// <see cref="SplitSpan{TBody, TSeparator, TStrategy}.ReversedEnumerator(SplitSpan{TBody, TSeparator, TStrategy})"/>.
        /// </summary>
        /// <param name="split">The parameter to pass onto the constructor.</param>
        /// <returns>
        /// The new instance of <see cref="ReversedEnumerator"/> by passing
        /// the parameter <paramref name="split"/> to the constructor
        /// <see cref="SplitSpan{TBody, TSeparator, TStrategy}.ReversedEnumerator(SplitSpan{TBody, TSeparator, TStrategy})"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        public static implicit operator SplitSpan<TBody, TSeparator, TStrategy>.ReversedEnumerator(
            SplitSpan<TBody, TSeparator, TStrategy> split
        ) =>
            new(split);

        /// <summary>Performs one step of an enumeration over the provided spans.</summary>
        /// <param name="sep">The separator span.</param>
        /// <param name="body">The span that contains the current state of the enumeration.</param>
        /// <param name="current">The current span.</param>
        /// <returns>
        /// <see langword="true"/> if a step was performed successfully;
        /// <see langword="false"/> if the end of the collection is reached.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Move(
            scoped ReadOnlySpan<TSeparator> sep,
            scoped ref ReadOnlySpan<TBody> body,
            out ReadOnlySpan<TBody> current
        ) =>
            0 switch
            {
                _ when body.IsEmpty && (current = default) is var _ => false,
                _ when sep.IsEmpty => (current = body) is var _ && (body = default) is var _,
                _ when typeof(TStrategy) == typeof(MatchAll) => MoveNextAll(To<TBody>.From(sep), ref body, out current),
#if NET8_0_OR_GREATER
                _ when typeof(TStrategy) == typeof(MatchAny) && typeof(TSeparator) == typeof(SearchValues<TBody>) =>
                    MoveNextAny(To<SearchValues<TBody>>.From(sep), ref body, out current),
#endif
                _ when typeof(TStrategy) == typeof(MatchAny) => MoveNextAny(To<TBody>.From(sep), ref body, out current),
                _ when typeof(TStrategy) == typeof(MatchOne) => MoveNextOne(To<TBody>.From(sep), ref body, out current),
                _ => throw Error,
            };

        /// <inheritdoc cref="System.Collections.IEnumerator.MoveNext"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext() => Move(_separator, ref _body, out _current);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool MoveNextAll(
            scoped ReadOnlySpan<TBody> sep,
            scoped ref ReadOnlySpan<TBody> body,
            out ReadOnlySpan<TBody> current
        )
        {
            System.Diagnostics.Debug.Assert(typeof(TStrategy) == typeof(MatchAll), "TStrategy is MatchAll");
            System.Diagnostics.Debug.Assert(!sep.IsEmpty, "separator is non-empty");

            if (sep.Length < body.Length)
            {
                current = default;
                return false;
            }

        Retry:

            switch (body.LastIndexOf(sep))
            {
                case -1:
                    current = body;
                    body = default;
                    return true;
                case var i when i == body.Length - sep.Length:
                    if (body.Length != sep.Length)
                    {
                        body = body.UnsafelyTake(i);
                        goto Retry;
                    }

                    current = default;
                    return false;
                case var i:
                    current = body.UnsafelySkip(i + sep.Length);
                    body = body.UnsafelyTake(i);
                    return true;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool MoveNextAny(
            scoped ReadOnlySpan<TBody> sep,
            scoped ref ReadOnlySpan<TBody> body,
            out ReadOnlySpan<TBody> current
        )
        {
            System.Diagnostics.Debug.Assert(typeof(TStrategy) == typeof(MatchAny), "TStrategy is MatchAny");
            System.Diagnostics.Debug.Assert(!sep.IsEmpty, "separator is non-empty");
#if NET7_0_OR_GREATER
            switch (body.LastIndexOfAnyExcept(sep))
            {
                case -1:
                    current = default;
                    return false;
                case var i when i == body.Length - 1: break;
                case var i:
                    body = body.UnsafelyTake(i + 1);
                    break;
            }

            if (body.LastIndexOfAny(sep) is not -1 and var length)
            {
                current = body.UnsafelySkip(length + 1);
                body = body.UnsafelyTake(length);
                return true;
            }

            current = body;
            body = default;
#else
        Retry:

            var max = -1;

            foreach (var next in sep)
                switch (body.LastIndexOf(next))
                {
                    case -1: continue;
                    case var i when i == body.Length - 1:
                        if (body.Length is not 1)
                        {
                            body = body.UnsafelyTake(body.Length - 1);
                            goto Retry;
                        }

                        current = default;
                        return false;
                    case var i when i > max:
                        max = i;
                        continue;
                }

            if (max is not -1)
            {
                current = body.UnsafelySkip(max + 1);
                body = body.UnsafelyTake(max);
                return true;
            }

            current = body;
            body = default;
#endif
            return true;
        }
#if NET8_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool MoveNextAny(
            scoped ReadOnlySpan<SearchValues<TBody>> sep,
            scoped ref ReadOnlySpan<TBody> body,
            out ReadOnlySpan<TBody> current
        )
        {
            System.Diagnostics.Debug.Assert(typeof(TStrategy) == typeof(MatchAny), "TStrategy is MatchAny");
            System.Diagnostics.Debug.Assert(!sep.IsEmpty, "separator is non-empty");
            ref var single = ref MemoryMarshal.GetReference(sep);

            switch (body.LastIndexOfAnyExcept(single))
            {
                case -1:
                    current = default;
                    return false;
                case var i when i == body.Length - 1: break;
                case var i:
                    body = body.UnsafelyTake(i + 1);
                    break;
            }

            if (body.LastIndexOfAny(single) is not -1 and var length)
            {
                current = body.UnsafelySkip(length + 1);
                body = body.UnsafelyTake(length);
                return true;
            }

            current = body;
            body = default;
            return true;
        }
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool MoveNextOne(
            scoped ReadOnlySpan<TBody> sep,
            scoped ref ReadOnlySpan<TBody> body,
            out ReadOnlySpan<TBody> current
        )
        {
            System.Diagnostics.Debug.Assert(typeof(TStrategy) == typeof(MatchOne), "TStrategy is MatchOne");
            System.Diagnostics.Debug.Assert(!sep.IsEmpty, "separator is non-empty");
            var single = sep.UnsafelyIndex(0);
#if NET7_0_OR_GREATER
            switch (body.LastIndexOfAnyExcept(single))
            {
                case -1:
                    current = default;
                    return false;
                case var i when i == body.Length - 1: break;
                case var offset:
                    body = body.UnsafelyTake(offset + 1);
                    break;
            }

            if (body.IndexOf(single) is not -1 and var length)
            {
                current = body.UnsafelySkip(length + 1);
                body = body.UnsafelyTake(length);
                return true;
            }

            current = body;
            body = default;
            return true;
#else
        Retry:

            switch (body.LastIndexOf(single))
            {
                case -1:
                    current = body;
                    body = default;
                    return true;
                case var i when i == body.Length - 1:
                    if (body.Length is not 1)
                    {
                        body = body.UnsafelyTake(1);
                        goto Retry;
                    }

                    current = default;
                    return false;
                case var i:
                    current = body.UnsafelySkip(i);
                    body = body.UnsafelyTake(i - 1);
                    return true;
            }
#endif
        }
    }
}
