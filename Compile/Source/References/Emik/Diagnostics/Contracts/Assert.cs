// SPDX-License-Identifier: MPL-2.0
#if NETFRAMEWORK || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Defines the base class for an assertion, where a function is expected to return true.</summary>
/// <param name="that">The condition that must be true.</param>
/// <param name="message">The message to display when <paramref name="that"/> is false.</param>
/// <param name="thatEx">The context of where <paramref name="that"/> came from.</param>
#if CSHARPREPL
public
#endif
abstract partial class Assert(
    bool that,
    string? message = null,
    [CallerArgumentExpression(nameof(that))] string thatEx = ""
)
{
#if !CSHARPREPL
    static readonly IList<Type> s_assertions = typeof(Assert).Assembly.TryGetTypes().Where(IsAssertable).ToIList();
#endif
    /// <summary>Initializes a new instance of the <see cref="Emik.Morsels.Assert"/> class.</summary>
    /// <param name="that">The condition that must be true.</param>
    /// <param name="message">The message to display when <paramref name="that"/> is false.</param>
    /// <param name="thatEx">The context of where <paramref name="that"/> came from.</param>
    protected Assert(
        [InstantHandle] Func<bool> that,
        string? message = null,
        [CallerArgumentExpression(nameof(that))] string thatEx = ""
    )
        : this(Update(that, that, ref message, f => f?[thatEx]), message, thatEx) { }

    /// <summary>Gets the amount of available assertions.</summary>
    [Pure]
    public static int Length =>
#if CSHARPREPL
        Runner.Count();
#else
        s_assertions.Count;
#endif
    /// <summary>
    /// Gets the enumeration responsible for running every <see cref="Emik.Morsels.Assert"/> instance
    /// defined in the current <see cref="Assembly"/>, and returning every instance of a failed assert.
    /// </summary>
    [Pure]
    public static IEnumerable<Result> Runner =>
#if CSHARPREPL
        AppDomain
           .CurrentDomain
           .GetAssemblies()
           .SelectMany(ManyQueries.TryGetTypes)
           .Where(IsAssertable)
#else
        s_assertions
#endif
           .Select(x => new Result(x));

    /// <summary>Gets the message of the assertion if it failed, or null.</summary>
    [Pure]
    public string? Message { get; } = that ? null : message ?? FormatAttribute.Default[thatEx];

    /// <summary>Gets the name of the assertion.</summary>
    [Pure]
    public string Name => GetType().UnfoldedFullName();

    /// <summary>Assertion that the enumerable must contain an item.</summary>
    /// <param name="x">The enumerable that must contain an item.</param>
    /// <returns>Whether the parameter <paramref name="x"/> contains an item.</returns>
    [Format("Expected @x to have any items, received an empty collection."), Pure]
    public static bool Any([InstantHandle] IEnumerable x)
    {
        var e = x.GetEnumerator();

        try
        {
            return e.MoveNext();
        }
        finally
        {
            (e as IDisposable)?.Dispose();
        }
    }

    /// <summary>Assertion that the enumerable must be empty.</summary>
    /// <param name="x">The enumerable that must be empty.</param>
    /// <returns>Whether the parameter <paramref name="x"/> is empty.</returns>
    [Format("Expected @x to be an empty collection, received #x."), Pure]
    public static bool Empty([InstantHandle] IEnumerable x) => !Any(x);

    /// <summary>Assertion that the enumerable must be null or empty.</summary>
    /// <param name="x">The enumerable that must be null or empty.</param>
    /// <returns>Whether the parameter <paramref name="x"/> is null or empty.</returns>
    [Format("Expected @x to be null or empty, received #x."), Pure]
    public static bool EmptyOrNull([InstantHandle, NotNullWhen(false)] IEnumerable? x) => x is null || !Any(x);

    /// <summary>Updates the value of the referenced parameter if the provided assertion fails.</summary>
    /// <param name="exposure">The exposed <see cref="Delegate"/> used to get metadata from.</param>
    /// <param name="that">The condition that must be true.</param>
    /// <param name="message">The message to update.</param>
    /// <param name="formatter">The factory of the message.</param>
    /// <returns>The returned value when calling the parameter <paramref name="that"/>.</returns>
    [MustUseReturnValue]
    public static bool Update(
        [InstantHandle] Delegate exposure,
        [InstantHandle] Func<bool> that,
        ref string? message,
        [InstantHandle] Converter<FormatAttribute?, string?> formatter
    ) =>
        that() || (message ??= formatter(exposure.Method.GetCustomAttribute<FormatAttribute>())) is var _ && false;

    /// <summary>Assertion that both parameters must contain the same items.</summary>
    /// <typeparam name="T">The type of items to compare.</typeparam>
    /// <param name="x">The left-hand side.</param>
    /// <param name="y">The right-hand side.</param>
    /// <returns>Whether the parameters <paramref name="x"/> and <paramref name="y"/> have the same items.</returns>
    [Format("Expected @x to have the same items as @y, received #x and #y."), Pure]
    public static bool SequenceEqualTo<T>([InstantHandle] IEnumerable<T> x, [InstantHandle] IEnumerable<T> y) =>
        x.SequenceEqual(y);

    /// <summary>Assertion that both parameters must be equal.</summary>
    /// <typeparam name="T">The type of values to compare.</typeparam>
    /// <param name="x">The left-hand side.</param>
    /// <param name="y">The right-hand side.</param>
    /// <returns>Whether the parameters <paramref name="x"/> and <paramref name="y"/> are the same.</returns>
    [Format("Expected @x to be equal to @y, received #x and #y."), Pure]
    public static bool EqualTo<T>(T x, T y) => EqualityComparer<T>.Default.Equals(x, y);

    /// <summary>Assertion that the left-hand side must be greater than the right-hand side.</summary>
    /// <typeparam name="T">The type of values to compare.</typeparam>
    /// <param name="x">The left-hand side.</param>
    /// <param name="y">The right-hand side.</param>
    /// <returns>Whether the parameter <paramref name="x"/> is greater than <paramref name="y"/>.</returns>
    [Format("Expected @x to be strictly greater than @y, received #x which is less than or equal to #y."), Pure]
    public static bool GreaterThan<T>(T x, T y) => Compare(x, y) > 0;

    /// <summary>Assertion that the left-hand side must be greater than or equal to the right-hand side.</summary>
    /// <typeparam name="T">The type of values to compare.</typeparam>
    /// <param name="x">The left-hand side.</param>
    /// <param name="y">The right-hand side.</param>
    /// <returns>Whether the parameter <paramref name="x"/> is greater than or equal to <paramref name="y"/>.</returns>
    [Format("Expected @x to be greater than or equal to @y, received #x which is strictly less than #y."), Pure]
    public static bool GreaterThanOrEqualTo<T>(T x, T y) => Compare(x, y) >= 0;

    /// <summary>Assertion that the left-hand side must be less than the right-hand side.</summary>
    /// <typeparam name="T">The type of values to compare.</typeparam>
    /// <param name="x">The left-hand side.</param>
    /// <param name="y">The right-hand side.</param>
    /// <returns>Whether the parameter <paramref name="x"/> is less than <paramref name="y"/>.</returns>
    [Format("Expected @x to be strictly less than @y, received #x which is greater than or equal to #y."), Pure]
    public static bool LessThan<T>(T x, T y) => Compare(x, y) < 0;

    /// <summary>Assertion that the left-hand side must be less than or equal to the right-hand side.</summary>
    /// <typeparam name="T">The type of values to compare.</typeparam>
    /// <param name="x">The left-hand side.</param>
    /// <param name="y">The right-hand side.</param>
    /// <returns>Whether the parameter <paramref name="x"/> is less than or equal to <paramref name="y"/>.</returns>
    [Format("Expected @x to be less than or equal to @y, received #x which is strictly greater than #y."), Pure]
    public static bool LessThanOrEqualTo<T>(T x, T y) => Compare(x, y) <= 0;

    /// <summary>Assertion that the enumerable must not be null.</summary>
    /// <param name="x">The value that must not be null.</param>
    /// <returns>Whether the parameter <paramref name="x"/> is not null.</returns>
    [Format("Expected @x to be not null, received null."), Pure]
    public static bool NotNull([NotNullWhen(true)] object? x) => x is not null;

    /// <summary>Assertion that the enumerable must not be null.</summary>
    /// <typeparam name="T">The type of value to do the null check on.</typeparam>
    /// <param name="x">The value that must not be null.</param>
    /// <returns>Whether the parameter <paramref name="x"/> is not null.</returns>
    [Format("Expected @x to be not null, received null."), Pure]
    public static bool NotNull<T>([NotNullWhen(true)] T x) => x is not null;

    /// <summary>Assertion that the enumerable must be null.</summary>
    /// <param name="x">The value that must be null.</param>
    /// <returns>Whether the parameter <paramref name="x"/> is null.</returns>
    [Format("Expected @x to be null, received #x."), Pure]
    public static bool Null([NotNullWhen(false)] object? x) => x is null;

    /// <summary>Assertion that the enumerable must be null.</summary>
    /// <typeparam name="T">The type of value to do the null check on.</typeparam>
    /// <param name="x">The value that must be null.</param>
    /// <returns>Whether the parameter <paramref name="x"/> is null.</returns>
    [Format("Expected @x to be null, received #x."), Pure]
    public static bool Null<T>([NotNullWhen(false)] T x) => x is null;

    /// <summary>Assertion that both parameters must not be equal.</summary>
    /// <typeparam name="T">The type of values to compare.</typeparam>
    /// <param name="x">The left-hand side.</param>
    /// <param name="y">The right-hand side.</param>
    /// <returns>Whether the parameters <paramref name="x"/> and <paramref name="y"/> are not the same.</returns>
    [Format("Expected @x to not be equal to @y, received #x."), Pure]
    public static bool UnequalTo<T>(T x, T y) => !EqualTo(x, y);

    /// <summary>Compares the two instances. This method is used for any comparing assertion methods.</summary>
    /// <typeparam name="T">The type of values to compare.</typeparam>
    /// <param name="x">The left-hand side.</param>
    /// <param name="y">The right-hand side.</param>
    /// <returns>The resulting value from comparing parameters <paramref name="x"/> and <paramref name="y"/>.</returns>
    [Pure]
    public static int Compare<T>(T x, T y) => Comparer<T>.Default.Compare(x, y);
#if NET7_0_OR_GREATER
    /// <summary>Creates the assertion that two values must be equal to each other within an error of margin.</summary>
    /// <typeparam name="T">The type of value.</typeparam>
    /// <param name="margin">The lossy value to which both instances are considered equal.</param>
    /// <returns>The assertion that determines equality of two values within a margin of error.</returns>
    [Pure]
    public static Func<T, T, bool> RoughlyEqualTo<T>(T margin)
        where T : INumber<T> =>
        [Format("Expected @x to be approximately equal to @y, received #x and #y.")](x, y) =>
            T.Abs(x - y) <= T.Abs(margin);
#else
    /// <summary>Creates the assertion that two items must be equal to each other within an error of margin.</summary>
    /// <param name="margin">The lossy value to which both instances are considered equal.</param>
    /// <returns>The assertion that determines equality of two items within a margin of error.</returns>
    [Pure]
    public static Func<float, float, bool> RoughlyEqualTo(float margin) =>
        [Format("Expected @x to be approximately equal to @y, received #x and #y.")](x, y) =>
            Math.Abs(x - y) <= Math.Abs(margin);

    /// <inheritdoc cref="RoughlyEqualTo(float)"/>
    [Pure]
    public static Func<double, double, bool> RoughlyEqualTo(double margin) =>
        [Format("Expected @x to be approximately equal to @y, received #x and #y.")](x, y) =>
            Math.Abs(x - y) <= Math.Abs(margin);

    /// <inheritdoc cref="RoughlyEqualTo(float)"/>
    [Pure]
    public static Func<decimal, decimal, bool> RoughlyEqualTo(decimal margin) =>
        [Format("Expected @x to be approximately equal to @y, received #x and #y.")](x, y) =>
            Math.Abs(x - y) <= Math.Abs(margin);
#endif
    /// <summary>Executes every assertion and gets all of the assertions that failed.</summary>
    /// <returns>All assertions that failed.</returns>
    [Pure]
    public static IEnumerable<string> AllMessages() => Runner.RunAll().Where(x => x.Failed).Select(x => $"{x}");

    /// <summary>Creates the assertion that the value must be within a certain range.</summary>
    /// <param name="range">The range of values to accept. The range is considered to be inclusive on both ends.</param>
    /// <returns>The assertion that determines whether a value is within the specific range.</returns>
    [Pure]
    public static Predicate<int> InRangeOf(Range range) =>
        [Format("Expected @x to be approximately within the range, received #x.")](x) =>
            x >= range.Start.Value && x <= range.End.Value;
#if NET7_0_OR_GREATER
    /// <summary>Creates the assertion that the value must be within a certain range.</summary>
    /// <typeparam name="T">The type of value.</typeparam>
    /// <param name="range">The range of values to accept. The range is considered to be inclusive on both ends.</param>
    /// <returns>The assertion that determines whether a value is within the specific range.</returns>
    [Pure]
    public static Predicate<T> InRangeOf<T>(Range range)
        where T : INumberBase<T> =>
        [Format("Expected @x to be approximately within the range, received #x.")](x) =>
            int.CreateSaturating(x) is var i && i >= range.Start.Value && i <= range.End.Value;
#endif
    /// <summary>Creates the assertion that the value must be within a certain range.</summary>
    /// <typeparam name="T">The type of value.</typeparam>
    /// <param name="low">The inclusive lower boundary.</param>
    /// <param name="high">The inclusive higher boundary.</param>
    /// <returns>The assertion that determines whether a value is within the specific range.</returns>
    [Pure]
    public static Predicate<T> InRangeOf<T>(T low, T high) =>
        [Format("Expected @x to be approximately within the range, received #x.")](x) =>
            GreaterThanOrEqualTo(x, low) && LessThanOrEqualTo(x, high);

    /// <summary>Creates the assertion that the parameter must contain specific items.</summary>
    /// <typeparam name="T">The type of value.</typeparam>
    /// <param name="items">The items that will be eventually compared to.</param>
    /// <returns>The assertion that determines whether a value contains the pre-determined items.</returns>
    [Pure]
    public static Predicate<IEnumerable<T>> Structured<T>(params T[] items) =>
        [Format("Expected @x to have fixed specific items, received #x.")](x) => SequenceEqualTo(x, items);

    /// <summary>Returns the parameter.</summary>
    /// <typeparam name="T">The type of value.</typeparam>
    /// <param name="items">The items that will be returned directly.</param>
    /// <returns>The parameter <paramref name="items"/>.</returns>
    [Pure]
    public static T[] Params<T>(params T[] items) => items;

    /// <inheritdoc />
    [Pure]
    public override string ToString() => new Result(this, GetType()).ToString();

    /// <summary>
    /// Determines whether the type implements <see cref="Emik.Morsels.Assert"/> and can be instantiated.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns>Whether the type implements <see cref="Emik.Morsels.Assert"/> and can be instantiated.</returns>
    [Pure]
    static bool IsAssertable([NotNullWhen(true)] Type? type) =>
        type is { IsAbstract: false, IsClass: true, IsGenericType: false } &&
        ParameterlessConstructor(type) is not null &&
        type.FindPathToNull(x => x.BaseType).Contains(typeof(Assert));

    /// <summary>Gets the parameterless constructor, ignoring possible exceptions thrown.</summary>
    /// <param name="type">The type to get the parameterless exception from.</param>
    /// <returns>
    /// The <see cref="ConstructorInfo"/> containing no parameters from the parameter <paramref name="type"/>,
    /// if one exists.
    /// </returns>
    [Pure]
    static ConstructorInfo? ParameterlessConstructor(Type type)
    {
        try
        {
            return type.GetConstructor(Type.EmptyTypes);
        }
        catch (FileNotFoundException)
        {
            return null;
        }
    }
}
#endif
