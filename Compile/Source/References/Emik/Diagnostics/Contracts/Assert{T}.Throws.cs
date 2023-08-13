// SPDX-License-Identifier: MPL-2.0
#if NETFRAMEWORK || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <inheritdoc cref="Assert{T}"/>
abstract partial class Assert<T>
{
    /// <summary>Defines the base class for an assertion, where the type must throw.</summary>
    protected new abstract partial class Throws : Throws<Exception>
    {
        /// <inheritdoc />
        protected Throws(
            [InstantHandle] Action<T> that,
            T x,
            string? message = null,
            [CallerArgumentExpression(nameof(that))] string thatEx = ""
        )
            : base(that, x, message, thatEx) { }

        /// <inheritdoc />
        protected Throws(
            [InstantHandle] Converter<T, object> that,
            T x,
            string? message = null,
            [CallerArgumentExpression(nameof(that))] string thatEx = ""
        )
            : base(that, x, message, thatEx) { }

        /// <inheritdoc />
        protected Throws(
            T x,
            [InstantHandle] Action<T> that,
            string? message = null,
            [CallerArgumentExpression(nameof(that))] string thatEx = ""
        )
            : base(x, that, message, thatEx) { }

        /// <inheritdoc />
        protected Throws(
            T x,
            [InstantHandle] Converter<T, object> that,
            string? message = null,
            [CallerArgumentExpression(nameof(that))] string thatEx = ""
        )
            : base(x, that, message, thatEx) { }

        /// <inheritdoc />
        protected Throws(
            [InstantHandle] Action<T> that,
            [InstantHandle] Func<T> x,
            string? message = null,
            [CallerArgumentExpression(nameof(that))] string thatEx = ""
        )
            : base(that, x, message, thatEx) { }

        /// <inheritdoc />
        protected Throws(
            [InstantHandle] Converter<T, object> that,
            [InstantHandle] Func<T> x,
            string? message = null,
            [CallerArgumentExpression(nameof(that))] string thatEx = ""
        )
            : base(that, x, message, thatEx) { }

        /// <inheritdoc />
        protected Throws(
            [InstantHandle] Func<T> x,
            [InstantHandle] Action<T> that,
            string? message = null,
            [CallerArgumentExpression(nameof(that))] string thatEx = ""
        )
            : base(x, that, message, thatEx) { }

        /// <inheritdoc />
        protected Throws(
            [InstantHandle] Func<T> x,
            [InstantHandle] Converter<T, object> that,
            string? message = null,
            [CallerArgumentExpression(nameof(that))] string thatEx = ""
        )
            : base(x, that, message, thatEx) { }
    }
}
#endif
