// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <inheritdoc cref="Assert"/>
abstract partial class Assert
{
    /// <summary>Defines the base class for an assertion, where the type must throw.</summary>
    protected abstract partial class Throws : Throws<Exception>
    {
        /// <inheritdoc />
        protected Throws(
            [InstantHandle] Action that,
            string? message = null,
            [CallerArgumentExpression(nameof(that))] string thatEx = ""
        )
            : base(that, message, thatEx) { }

        /// <inheritdoc />
        protected Throws(
            [InstantHandle] Func<object> that,
            string? message = null,
            [CallerArgumentExpression(nameof(that))] string thatEx = ""
        )
            : base(that, message, thatEx) { }
    }
}
