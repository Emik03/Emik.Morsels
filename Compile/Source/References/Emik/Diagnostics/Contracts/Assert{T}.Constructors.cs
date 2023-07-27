// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <inheritdoc cref="Assert{T}"/>
abstract partial class Assert<T>
{
    /// <summary>Initializes a new instance of the <see cref="Assert{T}"/> class.</summary>
    /// <param name="it">The context value.</param>
    /// <param name="that">The condition that must be true.</param>
    /// <param name="message">The message to display when <paramref name="that"/> is false.</param>
    /// <param name="itEx">The context of where <paramref name="it"/> came from.</param>
    /// <param name="thatEx">The context of where <paramref name="that"/> came from.</param>
    protected Assert(
        T it,
        [InstantHandle] Predicate<T> that,
        string? message = null,
        [CallerArgumentExpression(nameof(it))] string itEx = "",
        [CallerArgumentExpression(nameof(that))] string thatEx = ""
    )
        : base(Update(that, () => that(it), ref message, f => f?[thatEx, itEx, it]), message, thatEx) { }

    /// <summary>Initializes a new instance of the <see cref="Assert{T}"/> class.</summary>
    /// <param name="x">The first context value.</param>
    /// <param name="y">The second context value.</param>
    /// <param name="that">The condition that must be true.</param>
    /// <param name="message">The message to display when <paramref name="that"/> is false.</param>
    /// <param name="xEx">The context of where <paramref name="x"/> came from.</param>
    /// <param name="yEx">The context of where <paramref name="y"/> came from.</param>
    /// <param name="thatEx">The context of where <paramref name="that"/> came from.</param>
    protected Assert(
        T x,
        T y,
        [InstantHandle] Func<T, T, bool> that,
        string? message = null,
        [CallerArgumentExpression(nameof(x))] string xEx = "",
        [CallerArgumentExpression(nameof(y))] string yEx = "",
        [CallerArgumentExpression(nameof(that))] string thatEx = ""
    )
        : base(Update(that, () => that(x, y), ref message, f => f?[thatEx, xEx, x, yEx, y]), message, thatEx) { }

    /// <inheritdoc cref="Emik.Morsels.Assert{T}(T, Predicate{T}, string, string, string)"/>
    protected Assert(
        [InstantHandle] Predicate<T> that,
        T it,
        string? message = null,
        [CallerArgumentExpression(nameof(it))] string itEx = "",
        [CallerArgumentExpression(nameof(that))] string thatEx = ""
    )
        : this(it, that, message, itEx, thatEx) { }

    /// <inheritdoc cref="Emik.Morsels.Assert{T}(T, Predicate{T}, string, string, string)"/>
    protected Assert(
        [InstantHandle] Func<T> it,
        [InstantHandle] Predicate<T> that,
        string? message = null,
        [CallerArgumentExpression(nameof(it))] string itEx = "",
        [CallerArgumentExpression(nameof(that))] string thatEx = ""
    )
        : this(it(), that, message, itEx, thatEx) { }

    /// <inheritdoc cref="Emik.Morsels.Assert{T}(T, Predicate{T}, string, string, string)"/>
    protected Assert(
        [InstantHandle] Predicate<T> that,
        [InstantHandle] Func<T> it,
        string? message = null,
        [CallerArgumentExpression(nameof(it))] string itEx = "",
        [CallerArgumentExpression(nameof(that))] string thatEx = ""
    )
        : this(it(), that, message, itEx, thatEx) { }

    /// <inheritdoc cref="Emik.Morsels.Assert{T}(T, T, Func{T, T, bool}, string, string, string, string)"/>
    protected Assert(
        T x,
        [InstantHandle] Func<T, T, bool> that,
        T y,
        string? message = null,
        [CallerArgumentExpression(nameof(x))] string xEx = "",
        [CallerArgumentExpression(nameof(y))] string yEx = "",
        [CallerArgumentExpression(nameof(that))] string thatEx = ""
    )
        : this(x, y, that, message, xEx, yEx, thatEx) { }

    /// <inheritdoc cref="Emik.Morsels.Assert{T}(T, T, Func{T, T, bool}, string, string, string, string)"/>
    protected Assert(
        [InstantHandle] Func<T, T, bool> that,
        T x,
        T y,
        string? message = null,
        [CallerArgumentExpression(nameof(x))] string xEx = "",
        [CallerArgumentExpression(nameof(y))] string yEx = "",
        [CallerArgumentExpression(nameof(that))] string thatEx = ""
    )
        : this(x, y, that, message, xEx, yEx, thatEx) { }

    /// <inheritdoc cref="Emik.Morsels.Assert{T}(T, T, Func{T, T, bool}, string, string, string, string)"/>
    protected Assert(
        T x,
        [InstantHandle] Func<T> y,
        [InstantHandle] Func<T, T, bool> that,
        string? message = null,
        [CallerArgumentExpression(nameof(x))] string xEx = "",
        [CallerArgumentExpression(nameof(y))] string yEx = "",
        [CallerArgumentExpression(nameof(that))] string thatEx = ""
    )
        : this(x, y(), that, message, xEx, yEx, thatEx) { }

    /// <inheritdoc cref="Emik.Morsels.Assert{T}(T, T, Func{T, T, bool}, string, string, string, string)"/>
    protected Assert(
        [InstantHandle] Func<T> x,
        T y,
        [InstantHandle] Func<T, T, bool> that,
        string? message = null,
        [CallerArgumentExpression(nameof(x))] string xEx = "",
        [CallerArgumentExpression(nameof(y))] string yEx = "",
        [CallerArgumentExpression(nameof(that))] string thatEx = ""
    )
        : this(x(), y, that, message, xEx, yEx, thatEx) { }

    /// <inheritdoc cref="Emik.Morsels.Assert{T}(T, T, Func{T, T, bool}, string, string, string, string)"/>
    protected Assert(
        [InstantHandle] Func<T> x,
        [InstantHandle] Func<T> y,
        [InstantHandle] Func<T, T, bool> that,
        string? message = null,
        [CallerArgumentExpression(nameof(x))] string xEx = "",
        [CallerArgumentExpression(nameof(y))] string yEx = "",
        [CallerArgumentExpression(nameof(that))] string thatEx = ""
    )
        : this(x(), y(), that, message, xEx, yEx, thatEx) { }

    /// <inheritdoc cref="Emik.Morsels.Assert{T}(T, T, Func{T, T, bool}, string, string, string, string)"/>
    protected Assert(
        [InstantHandle] Func<T> x,
        [InstantHandle] Func<T, T, bool> that,
        [InstantHandle] Func<T> y,
        string? message = null,
        [CallerArgumentExpression(nameof(x))] string xEx = "",
        [CallerArgumentExpression(nameof(y))] string yEx = "",
        [CallerArgumentExpression(nameof(that))] string thatEx = ""
    )
        : this(x(), y(), that, message, xEx, yEx, thatEx) { }

    /// <inheritdoc cref="Emik.Morsels.Assert{T}(T, T, Func{T, T, bool}, string, string, string, string)"/>
    protected Assert(
        [InstantHandle] Func<T> x,
        [InstantHandle] Func<T, T, bool> that,
        T y,
        string? message = null,
        [CallerArgumentExpression(nameof(x))] string xEx = "",
        [CallerArgumentExpression(nameof(y))] string yEx = "",
        [CallerArgumentExpression(nameof(that))] string thatEx = ""
    )
        : this(x(), y, that, message, xEx, yEx, thatEx) { }

    /// <inheritdoc cref="Emik.Morsels.Assert{T}(T, T, Func{T, T, bool}, string, string, string, string)"/>
    protected Assert(
        T x,
        [InstantHandle] Func<T, T, bool> that,
        [InstantHandle] Func<T> y,
        string? message = null,
        [CallerArgumentExpression(nameof(x))] string xEx = "",
        [CallerArgumentExpression(nameof(y))] string yEx = "",
        [CallerArgumentExpression(nameof(that))] string thatEx = ""
    )
        : this(x, y(), that, message, xEx, yEx, thatEx) { }

    /// <inheritdoc cref="Emik.Morsels.Assert{T}(T, T, Func{T, T, bool}, string, string, string, string)"/>
    protected Assert(
        [InstantHandle] Func<T, T, bool> that,
        [InstantHandle] Func<T> x,
        [InstantHandle] Func<T> y,
        string? message = null,
        [CallerArgumentExpression(nameof(x))] string xEx = "",
        [CallerArgumentExpression(nameof(y))] string yEx = "",
        [CallerArgumentExpression(nameof(that))] string thatEx = ""
    )
        : this(x(), y(), that, message, xEx, yEx, thatEx) { }

    /// <inheritdoc cref="Emik.Morsels.Assert{T}(T, T, Func{T, T, bool}, string, string, string, string)"/>
    protected Assert(
        [InstantHandle] Func<T, T, bool> that,
        [InstantHandle] Func<T> x,
        T y,
        string? message = null,
        [CallerArgumentExpression(nameof(x))] string xEx = "",
        [CallerArgumentExpression(nameof(y))] string yEx = "",
        [CallerArgumentExpression(nameof(that))] string thatEx = ""
    )
        : this(x(), y, that, message, xEx, yEx, thatEx) { }

    /// <inheritdoc cref="Emik.Morsels.Assert{T}(T, T, Func{T, T, bool}, string, string, string, string)"/>
    protected Assert(
        [InstantHandle] Func<T, T, bool> that,
        T x,
        [InstantHandle] Func<T> y,
        string? message = null,
        [CallerArgumentExpression(nameof(x))] string xEx = "",
        [CallerArgumentExpression(nameof(y))] string yEx = "",
        [CallerArgumentExpression(nameof(that))] string thatEx = ""
    )
        : this(x, y(), that, message, xEx, yEx, thatEx) { }
}
