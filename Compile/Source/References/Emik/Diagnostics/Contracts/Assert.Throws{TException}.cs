// SPDX-License-Identifier: MPL-2.0
#if NETFRAMEWORK || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <inheritdoc cref="Emik.Morsels.Assert"/>
abstract partial class Assert
{
    /// <summary>
    /// Defines the base class for an assertion, where the type must throw <typeparamref name="TException"/>.
    /// </summary>
    /// <typeparam name="TException">The type of exception to expect to be thrown.</typeparam>
    public abstract partial class Throws<TException> : Assert
        where TException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Emik.Morsels.Assert.Throws{TException}"/> class.
        /// </summary>
        /// <param name="that">The condition that must throw <typeparamref name="TException"/>.</param>
        /// <param name="message">The message to display when <paramref name="that"/> is false.</param>
        /// <param name="thatEx">The context of where <paramref name="that"/> came from.</param>
        protected Throws(
            [InstantHandle] Action that,
            string? message = null,
            [CallerArgumentExpression(nameof(that))] string thatEx = ""
        )
            : base(Try(that, ref message, thatEx), message, thatEx) { }

        /// <inheritdoc cref="Emik.Morsels.Assert.Throws{TException}(Action, string, string)"/>
        protected Throws(
            [InstantHandle] Func<object> that,
            string? message = null,
            [CallerArgumentExpression(nameof(that))] string thatEx = ""
        )
            : base(Try(() => that(), ref message, thatEx), message, thatEx) { }

        /// <summary>Invokes the callback, expecting <typeparamref name="TException"/> to be thrown.</summary>
        /// <param name="that">The condition that must throw <typeparamref name="TException"/>.</param>
        /// <param name="message">The message to display when <paramref name="that"/> is false.</param>
        /// <param name="thatEx">The context of where <paramref name="that"/> came from.</param>
        /// <returns>
        /// Whether <typeparamref name="TException"/> is thrown by the parameter <paramref name="that"/>.
        /// </returns>
        static bool Try(Action that, [NotNullWhen(false)] ref string? message, string thatEx)
        {
            try
            {
                that();
                return (message ??= Format(thatEx)) is var _ && false;
            }
            catch (TException)
            {
                return true;
            }
#pragma warning disable CA1031
            catch (Exception e)
#pragma warning restore CA1031
            {
                return (message ??= Format(thatEx, e)) is var _ && false;
            }
        }

        /// <summary>Creates the formatted error message.</summary>
        /// <param name="thatEx">The context of where the error came from.</param>
        /// <param name="e">The caught exception, if one exists.</param>
        /// <returns>The formatted error message.</returns>
        static string Format(string thatEx, Exception? e = null) =>
            $"Expected {thatEx.Collapse()} to throw {typeof(TException).UnfoldedName()}, instead received {Format(e)}.";

        /// <summary>Formats the exception.</summary>
        /// <param name="e">The caught exception, if one exists.</param>
        /// <returns>The formatted exception.</returns>
        static string Format(Exception? e) =>
            e is null
                ? "no exception"
#if NET6_0_OR_GREATER
                : $"{e.GetType().UnfoldedName()}: {e.Message}. {e.StackTrace?.ReplaceLineEndings(" ")}";
#else
                : $"{e.GetType().UnfoldedName()}: {e.Message}. {e.StackTrace?.Replace('\n', ' ')}";
#endif
    }
}
#endif
