// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <inheritdoc cref="Assert{T}"/>
abstract partial class Assert<T>
{
    /// <inheritdoc cref="Assert.Throws{TException}"/>
    public new abstract partial class Throws<TException> : Assert.Throws<TException>
        where TException : Exception
    {
        /// <inheritdoc />
        protected Throws(
            [InstantHandle] Action<T> that,
            T x,
            string? message = null,
            [CallerArgumentExpression(nameof(that))] string thatEx = ""
        )
            : base(() => that(x), message, thatEx) { }

        /// <inheritdoc />
        protected Throws(
            [InstantHandle] Converter<T, object> that,
            T x,
            string? message = null,
            [CallerArgumentExpression(nameof(that))] string thatEx = ""
        )
            : base(() => that(x), message, thatEx) { }

        /// <inheritdoc />
        protected Throws(
            T x,
            [InstantHandle] Action<T> that,
            string? message = null,
            [CallerArgumentExpression(nameof(that))] string thatEx = ""
        )
            : base(() => that(x), message, thatEx) { }

        /// <inheritdoc />
        protected Throws(
            T x,
            [InstantHandle] Converter<T, object> that,
            string? message = null,
            [CallerArgumentExpression(nameof(that))] string thatEx = ""
        )
            : base(() => that(x), message, thatEx) { }

        /// <inheritdoc />
        protected Throws(
            [InstantHandle] Action<T> that,
            [InstantHandle] Func<T> x,
            string? message = null,
            [CallerArgumentExpression(nameof(that))] string thatEx = ""
        )
            : base(() => that(x()), message, thatEx) { }

        /// <inheritdoc />
        protected Throws(
            [InstantHandle] Converter<T, object> that,
            [InstantHandle] Func<T> x,
            string? message = null,
            [CallerArgumentExpression(nameof(that))] string thatEx = ""
        )
            : base(() => that(x()), message, thatEx) { }

        /// <inheritdoc />
        protected Throws(
            [InstantHandle] Func<T> x,
            [InstantHandle] Action<T> that,
            string? message = null,
            [CallerArgumentExpression(nameof(that))] string thatEx = ""
        )
            : base(() => that(x()), message, thatEx) { }

        /// <inheritdoc />
        protected Throws(
            [InstantHandle] Func<T> x,
            [InstantHandle] Converter<T, object> that,
            string? message = null,
            [CallerArgumentExpression(nameof(that))] string thatEx = ""
        )
            : base(() => that(x()), message, thatEx) { }
    }
}
