// SPDX-License-Identifier: MPL-2.0
#if NETFRAMEWORK || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <inheritdoc cref="Emik.Morsels.Assert"/>
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
#endif
