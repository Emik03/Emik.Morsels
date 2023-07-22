// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Methods that provide functions for enumerations of <see cref="Assert.Result"/> instances.</summary>
static partial class AssertResultEnumerableOperations
{
    /// <summary>Eagerly executes all asserts of the passed in enumerator.</summary>
    /// <param name="enumerator">The <see cref="IEnumerator{T}"/> to execute.</param>
    /// <returns>The collected result of all assertions.</returns>
    [Pure]
    public static IList<Assert.Result> RunAll(this IEnumerator<Assert.Result> enumerator)
    {
        SmallList<Assert.Result> collected = default;

        while (enumerator.MoveNext())
            collected.Add(enumerator.Current.Run());

        return collected;
    }

    /// <summary>Eagerly executes all asserts of the passed in enumerable.</summary>
    /// <param name="enumerable">The <see cref="IEnumerable{T}"/> to execute.</param>
    /// <returns>The collected result of all assertions.</returns>
    [Pure]
    public static IList<Assert.Result> RunAll([InstantHandle] this IEnumerable<Assert.Result> enumerable) =>
        enumerable.Select(x => x.Run()).ToListLazily();
}

/// <inheritdoc cref="Assert"/>
abstract partial class Assert
{
    /// <summary>Represents the result of running an assertion.</summary>
    public readonly partial struct Result
    {
        /// <summary>Initializes a new instance of the <see cref="Result"/> struct.</summary>
        /// <param name="setup">The setup to store.</param>
        public Result(Type? setup = null) => Setup = setup;

        /// <summary>Initializes a new instance of the <see cref="Result"/> struct.</summary>
        /// <param name="assertion">The assertion to store.</param>
        /// <param name="setup">The setup to store.</param>
        public Result(Assert? assertion, Type? setup = null)
        {
            Assertion = assertion;
            Setup = setup ?? assertion?.GetType();
        }

        /// <summary>Initializes a new instance of the <see cref="Result"/> struct.</summary>
        /// <param name="error">The error to store.</param>
        /// <param name="setup">The setup to store.</param>
        public Result(Exception? error, Type? setup)
        {
            Error = error;
            Setup = setup;
        }

        /// <summary>Gets a value indicating whether <see cref="Assertion"/> has failed.</summary>
        [MemberNotNullWhen(false, nameof(Assertion)), Pure]
        public bool Failed => !Succeeded;

        /// <summary>Gets a value indicating whether <see cref="Error"/> is set.</summary>
        [MemberNotNullWhen(true, nameof(Error)), Pure]
        public bool HasError => Error is not null;

        /// <summary>Gets a value indicating whether this <see cref="Result"/> has executed.</summary>
        [Pure]
        public bool HasExecuted => !IsDefault;

        /// <summary>Gets a value indicating whether <see cref="Assertion"/> was successfully instantiated.</summary>
        [MemberNotNullWhen(true, nameof(Assertion)), Pure]
        public bool Instantiated => Assertion is not null;

        /// <summary>Gets a value indicating whether this <see cref="Result"/> is the default instance.</summary>
        [Pure]
        public bool IsDefault => Assertion is null && Error is null && Setup is null;

        /// <summary>Gets a value indicating whether <see cref="Assertion"/> has succeeded.</summary>
        [MemberNotNullWhen(true, nameof(Assertion)), Pure]
        public bool Succeeded => Instantiated && Assertion.Message is null;

        /// <summary>Gets the message of the assertion</summary>
        [Pure]
        public string? Message => Instantiated ? Assertion.Message : null;

        /// <summary>Gets the name of the assertion type.</summary>
        [Pure]
        public string Name => Setup.UnfoldedFullName();

        /// <summary>Gets the assertion that ran.</summary>
        [Pure]
        public Assert? Assertion { get; }

        /// <summary>Gets the error that was thrown while instantiating <see cref="Assertion"/>.</summary>
        [Pure]
        public Exception? Error { get; }

        /// <summary>Gets the default instance.</summary>
        [Pure]
        public Result Default => default;

        /// <summary>Gets the type that was attempted to be instantiated.</summary>
        [Pure]
        public Type? Setup { get; }

        /// <summary>Gets the fail message.</summary>
        /// <returns>The fail message.</returns>
        [Pure]
        string Fail => Setup is null ? "Assertion failed! " : $"Assertion {Setup.Name} failed! ";

        /// <inheritdoc />
        [Pure]
        public override string ToString() =>
            IsDefault ? "N/A" :
            Instantiated ? Succeeded ? "OK" : $"{Fail}{Assertion.Message}" :
            HasError ? $"{Fail}Unexpectedly threw {Error.GetType().UnfoldedFullName()}: {Error}" : "Not determined";

        /// <summary>Executes the assertion and returns the new <see cref="Result"/>.</summary>
        /// <returns>The new instance of <see cref="Result"/> that contains the assertion results.</returns>
        [MustUseReturnValue]
        public Result Run()
        {
            if (Setup is null)
                return default;

            try
            {
                return new(Activator.CreateInstance(Setup, true) as Assert, Setup);
            }
#pragma warning disable CA1031
            catch (Exception ex)
#pragma warning restore CA1031
            {
                return new(ex, Setup);
            }
        }
    }
}
