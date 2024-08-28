// SPDX-License-Identifier: MPL-2.0

// ReSharper disable NullableWarningSuppressionIsUsed
// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Extension methods for iterating over a set of elements, or for generating new ones.</summary>
static partial class Each
{
    /// <summary>
    /// The <see langword="for"/> statement executes a statement or a block of statements while a specified
    /// Boolean expression evaluates to <see langword="true"/>.
    /// </summary>
    /// <remarks><para><a href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/statements/iteration-statements#the-for-statement">
    /// See here for more information.
    /// </a></para></remarks>
    /// <param name="upper">The length to reach to in the for loop.</param>
    /// <param name="action">The action for each loop.</param>
    /// <returns>The parameter <paramref name="upper"/>.</returns>
    [NonNegativeValue]
    public static int For([NonNegativeValue] this int upper, [InstantHandle] Action action)
    {
        for (var i = 0; i < upper; i++)
            action();

        return upper;
    }

    /// <summary>
    /// The <see langword="for"/> statement executes a statement or a block of statements while a specified
    /// Boolean expression evaluates to <see langword="true"/>.
    /// </summary>
    /// <remarks><para><a href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/statements/iteration-statements#the-for-statement">
    /// See here for more information.
    /// </a></para></remarks>
    /// <param name="upper">The length to reach to in the for loop.</param>
    /// <param name="action">The action for each loop.</param>
    /// <returns>The parameter <paramref name="upper"/>.</returns>
    [NonNegativeValue]
    public static int For([NonNegativeValue] this int upper, [InstantHandle] Action<int> action)
    {
        for (var i = 0; i < upper; i++)
            action(i);

        return upper;
    }

    /// <summary>
    /// The <see langword="for"/> statement executes a statement or a block of statements while a specified
    /// Boolean expression evaluates to <see langword="true"/>.
    /// </summary>
    /// <remarks><para><a href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/statements/iteration-statements#the-for-statement">
    /// See here for more information.
    /// </a></para></remarks>
    /// <typeparam name="TExternal">The type of external parameter to pass into the callback.</typeparam>
    /// <param name="upper">The length to reach to in the for loop.</param>
    /// <param name="external">Any external parameter to be passed repeatedly into the callback.</param>
    /// <param name="action">The action for each loop.</param>
    /// <returns>The parameter <paramref name="upper"/>.</returns>
    [NonNegativeValue]
    public static int For<TExternal>(
        [NonNegativeValue] this int upper,
        TExternal external,
        [InstantHandle] Action<TExternal> action
    )
    {
        for (var i = 0; i < upper; i++)
            action(external);

        return upper;
    }

    /// <summary>
    /// The <see langword="for"/> statement executes a statement or a block of statements while a specified
    /// Boolean expression evaluates to <see langword="true"/>.
    /// </summary>
    /// <remarks><para><a href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/statements/iteration-statements#the-for-statement">
    /// See here for more information.
    /// </a></para></remarks>
    /// <typeparam name="TExternal">The type of external parameter to pass into the callback.</typeparam>
    /// <param name="upper">The length to reach to in the for loop.</param>
    /// <param name="external">Any external parameter to be passed repeatedly into the callback.</param>
    /// <param name="action">The action for each loop.</param>
    /// <returns>The parameter <paramref name="upper"/>.</returns>
    [NonNegativeValue]
    public static int For<TExternal>(
        [NonNegativeValue] this int upper,
        TExternal external,
        [InstantHandle] Action<int, TExternal> action
    )
    {
        for (var i = 0; i < upper; i++)
            action(i, external);

        return upper;
    }
#if !NET20 && !NET30
    /// <summary>
    /// The <see langword="foreach"/> statement executes a statement or a block of statements for each element in an
    /// instance of the type that implements the <see cref="IEnumerable{T}"/> interface.
    /// </summary>
    /// <remarks><para><a href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/statements/iteration-statements#the-foreach-statement">
    /// See here for more information.
    /// </a></para></remarks>
    /// <typeparam name="T">The type of iterator.</typeparam>
    /// <param name="iterable">The collection of items to go through one-by-one.</param>
    /// <param name="action">The action to do on each item in <paramref name="iterable"/>.</param>
    /// <returns>The parameter <paramref name="iterable"/>.</returns>
    public static ICollection<T> For<T>(
        [InstantHandle] this IEnumerable<T> iterable,
        [InstantHandle] Action<T> action
    )
    {
        var list = iterable.ToICollection();

        foreach (var item in list)
            action(item);

        return list;
    }

    /// <summary>
    /// The <see langword="foreach"/> statement executes a statement or a block of statements for each element in an
    /// instance of the type that implements the <see cref="IEnumerable{T}"/> interface.
    /// </summary>
    /// <remarks><para><a href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/statements/iteration-statements#the-foreach-statement">
    /// See here for more information.
    /// </a></para></remarks>
    /// <typeparam name="T">The type of iterator.</typeparam>
    /// <typeparam name="TExternal">The type of external parameter to pass into the callback.</typeparam>
    /// <param name="iterable">The collection of items to go through one-by-one.</param>
    /// <param name="external">Any external parameter to be passed repeatedly into the callback.</param>
    /// <param name="action">The action to do on each item in <paramref name="iterable"/>.</param>
    /// <returns>The parameter <paramref name="iterable"/>.</returns>
    public static ICollection<T> For<T, TExternal>(
        [InstantHandle] this IEnumerable<T> iterable,
        TExternal external,
        [InstantHandle] Action<T, TExternal> action
    )
    {
        var list = iterable.ToICollection();

        foreach (var item in list)
            action(item, external);

        return list;
    }

    /// <summary>
    /// The <see langword="foreach"/> statement executes a statement or a block of statements for each element in an
    /// instance of the type that implements the <see cref="IEnumerable{T}"/> interface.
    /// </summary>
    /// <remarks><para><a href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/statements/iteration-statements#the-foreach-statement">
    /// See here for more information.
    /// </a></para></remarks>
    /// <typeparam name="T">The type of iterator.</typeparam>
    /// <param name="iterable">The collection of items to go through one-by-one.</param>
    /// <param name="action">The action to do on each item in <paramref name="iterable"/>.</param>
    /// <returns>The parameter <paramref name="iterable"/>.</returns>
    public static ICollection<T> For<T>(
        [InstantHandle] this IEnumerable<T> iterable,
        [InstantHandle] Action<T, int> action
    )
    {
        var list = iterable.ToICollection();
        var i = 0;

        foreach (var item in list)
            action(item, checked(i++));

        return list;
    }

    /// <summary>
    /// The <see langword="foreach"/> statement executes a statement or a block of statements for each element in an
    /// instance of the type that implements the <see cref="IEnumerable{T}"/> interface.
    /// </summary>
    /// <remarks><para><a href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/statements/iteration-statements#the-foreach-statement">
    /// See here for more information.
    /// </a></para></remarks>
    /// <typeparam name="T">The type of iterator.</typeparam>
    /// <typeparam name="TExternal">The type of external parameter to pass into the callback.</typeparam>
    /// <param name="iterable">The collection of items to go through one-by-one.</param>
    /// <param name="external">Any external parameter to be passed repeatedly into the callback.</param>
    /// <param name="action">The action to do on each item in <paramref name="iterable"/>.</param>
    /// <returns>The parameter <paramref name="iterable"/>.</returns>
    public static ICollection<T> For<T, TExternal>(
        [InstantHandle] this IEnumerable<T> iterable,
        TExternal external,
        [InstantHandle] Action<T, int, TExternal> action
    )
    {
        var list = iterable.ToICollection();
        var i = 0;

        foreach (var item in list)
            action(item, checked(i++), external);

        return list;
    }
#endif
    /// <summary>
    /// The <see langword="foreach"/> statement executes a statement or a block of statements for each element in an
    /// instance of the type that implements the <see cref="IEnumerable{T}"/> interface.
    /// </summary>
    /// <remarks><para><a href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/statements/iteration-statements#the-foreach-statement">
    /// See here for more information.
    /// </a></para></remarks>
    /// <typeparam name="TKey">The type of key in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of value in the dictionary.</typeparam>
    /// <param name="dictionary">The collection of items to go through one-by-one.</param>
    /// <param name="action">The action to do on each item in <paramref name="dictionary"/>.</param>
    /// <returns>The parameter <paramref name="dictionary"/>.</returns>
    public static IDictionary<TKey, TValue> For<TKey, TValue>(
        [InstantHandle] this IDictionary<TKey, TValue> dictionary,
        [InstantHandle] Action<TKey, TValue> action
    )
        where TKey : notnull
    {
        foreach (var kvp in dictionary)
            action(kvp.Key, kvp.Value);

        return dictionary;
    }

    /// <summary>
    /// The <see langword="foreach"/> statement executes a statement or a block of statements for each element in an
    /// instance of the type that implements the <see cref="IEnumerable{T}"/> interface.
    /// </summary>
    /// <remarks><para><a href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/statements/iteration-statements#the-foreach-statement">
    /// See here for more information.
    /// </a></para></remarks>
    /// <typeparam name="TKey">The type of key in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of value in the dictionary.</typeparam>
    /// <typeparam name="TExternal">The type of external parameter to pass into the callback.</typeparam>
    /// <param name="dictionary">The collection of items to go through one-by-one.</param>
    /// <param name="external">Any external parameter to be passed repeatedly into the callback.</param>
    /// <param name="action">The action to do on each item in <paramref name="dictionary"/>.</param>
    /// <returns>The parameter <paramref name="dictionary"/>.</returns>
    public static IDictionary<TKey, TValue> For<TKey, TValue, TExternal>(
        [InstantHandle] this IDictionary<TKey, TValue> dictionary,
        TExternal external,
        [InstantHandle] Action<TKey, TValue, TExternal> action
    )
        where TKey : notnull
    {
        foreach (var kvp in dictionary)
            action(kvp.Key, kvp.Value, external);

        return dictionary;
    }

    /// <summary>
    /// The <see langword="foreach"/> statement executes a statement or a block of statements for each element in an
    /// instance of the type that implements the <see cref="IEnumerable{T}"/> interface.
    /// </summary>
    /// <remarks><para><a href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/statements/iteration-statements#the-foreach-statement">
    /// See here for more information.
    /// </a></para></remarks>
    /// <typeparam name="TKey">The type of key in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of value in the dictionary.</typeparam>
    /// <param name="dictionary">The collection of items to go through one-by-one.</param>
    /// <param name="action">The action to do on each item in <paramref name="dictionary"/>.</param>
    /// <returns>The parameter <paramref name="dictionary"/>.</returns>
    public static IDictionary<TKey, TValue> For<TKey, TValue>(
        [InstantHandle] this IDictionary<TKey, TValue> dictionary,
        [InstantHandle] Action<TKey, TValue, int> action
    )
        where TKey : notnull
    {
        var i = 0;

        foreach (var kvp in dictionary)
            action(kvp.Key, kvp.Value, checked(i++));

        return dictionary;
    }

    /// <summary>
    /// The <see langword="foreach"/> statement executes a statement or a block of statements for each element in an
    /// instance of the type that implements the <see cref="IEnumerable{T}"/> interface.
    /// </summary>
    /// <remarks><para><a href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/statements/iteration-statements#the-foreach-statement">
    /// See here for more information.
    /// </a></para></remarks>
    /// <typeparam name="TKey">The type of key in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of value in the dictionary.</typeparam>
    /// <typeparam name="TExternal">The type of external parameter to pass into the callback.</typeparam>
    /// <param name="dictionary">The collection of items to go through one-by-one.</param>
    /// <param name="external">Any external parameter to be passed repeatedly into the callback.</param>
    /// <param name="action">The action to do on each item in <paramref name="dictionary"/>.</param>
    /// <returns>The parameter <paramref name="dictionary"/>.</returns>
    public static IDictionary<TKey, TValue> For<TKey, TValue, TExternal>(
        [InstantHandle] this IDictionary<TKey, TValue> dictionary,
        TExternal external,
        [InstantHandle] Action<TKey, TValue, int, TExternal> action
    )
        where TKey : notnull
    {
        var i = 0;

        foreach (var kvp in dictionary)
            action(kvp.Key, kvp.Value, checked(i++), external);

        return dictionary;
    }
#if !NET20 && !NET30
    /// <summary>
    /// The <see langword="for"/> statement executes a statement or a block of statements while a specified
    /// Boolean expression evaluates to <see langword="true"/>.
    /// </summary>
    /// <remarks><para><a href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/statements/iteration-statements#the-for-statement">
    /// See here for more information.
    /// </a></para></remarks>
    /// <param name="num">The range of numbers to iterate over in the <see langword="for"/> loop.</param>
    /// <returns>An enumeration from a range's start to end.</returns>
    [LinqTunnel, Pure]
    public static IEnumerable<int> For(this int num) =>
        num >= 0 ? Enumerable.Range(0, num) : Enumerable.Repeat(-num, -num).Select((x, i) => x - i - 1);

    /// <summary>Gets an enumeration of a number.</summary>
    /// <param name="num">The index to count up or down to.</param>
    /// <returns>An enumeration from 0 to the index's value, or vice versa.</returns>
    [MustDisposeResource, Pure]
    public static IEnumerator<int> GetEnumerator(this int num) => num.For().GetEnumerator();

    /// <summary>
    /// The <see langword="for"/> statement executes a statement or a block of statements while a specified
    /// Boolean expression evaluates to <see langword="true"/>.
    /// </summary>
    /// <remarks><para><a href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/statements/iteration-statements#the-for-statement">
    /// See here for more information.
    /// </a></para></remarks>
    /// <typeparam name="TExternal">The type of external parameter to pass into the callback.</typeparam>
    /// <param name="upper">The length to reach to in the for loop.</param>
    /// <param name="external">Any external parameter to be passed repeatedly into the callback.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="int"/> from ranges 0 to <paramref name="upper"/> - 1.</returns>
    [LinqTunnel, Pure]
    public static IEnumerable<TExternal> For<TExternal>([NonNegativeValue] this int upper, TExternal external) =>
        Enumerable.Repeat(external, upper);

    /// <summary>
    /// The <see langword="for"/> statement executes a statement or a block of statements while a specified
    /// Boolean expression evaluates to <see langword="true"/>.
    /// </summary>
    /// <remarks><para><a href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/statements/iteration-statements#the-for-statement">
    /// See here for more information.
    /// </a></para></remarks>
    /// <typeparam name="TResult">The type of iterator.</typeparam>
    /// <param name="upper">The length to reach to in the for loop.</param>
    /// <param name="func">The function for each loop.</param>
    /// <returns>All instances that <paramref name="func"/> used in an <see cref="IEnumerable{T}"/>.</returns>
    [LinqTunnel, Pure]
    public static IEnumerable<TResult> For<TResult>(
        [NonNegativeValue] this int upper,
        [InstantHandle] Func<TResult> func
    ) =>
        Enumerable.Repeat(func, upper).Select(x => x());

    /// <summary>
    /// The <see langword="for"/> statement executes a statement or a block of statements while a specified
    /// Boolean expression evaluates to <see langword="true"/>.
    /// </summary>
    /// <remarks><para><a href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/statements/iteration-statements#the-for-statement">
    /// See here for more information.
    /// </a></para></remarks>
    /// <typeparam name="TResult">The type of iterator.</typeparam>
    /// <param name="upper">The length to reach to in the for loop.</param>
    /// <param name="func">The function for each loop.</param>
    /// <returns>All instances that <paramref name="func"/> used in an <see cref="IEnumerable{T}"/>.</returns>
    [LinqTunnel, Pure]
    public static IEnumerable<TResult> For<TResult>(
        [NonNegativeValue] this int upper,
        [InstantHandle] Converter<int, TResult> func
    ) =>
        Enumerable.Repeat(func, upper).Select((x, i) => x(i));
#endif
#if NET7_0_OR_GREATER
    /// <summary>
    /// The <see langword="for"/> statement executes a statement or a block of statements while a specified
    /// Boolean expression evaluates to <see langword="true"/>.
    /// </summary>
    /// <remarks><para><a href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/statements/iteration-statements#the-for-statement">
    /// See here for more information.
    /// </a></para></remarks>
    /// <typeparam name="T">The type of number for the loop.</typeparam>
    /// <param name="upper">The range of numbers to iterate over in the <see langword="for"/> loop.</param>
    /// <returns>An enumeration from a range's start to end.</returns>
    [LinqTunnel, Pure]
    public static IEnumerable<T> For<T>(this T upper)
        where T : IComparisonOperators<T?, T?, bool>,
        ISubtractionOperators<T, T, T>,
        IIncrementOperators<T>
    {
        var isNegative = upper < default(T);
        var abs = isNegative ? default(T)! - upper : upper;

        for (T? i = default; i < abs; i++)
            yield return isNegative ? upper - i : i;
    }

    /// <summary>Gets an enumeration of a number.</summary>
    /// <typeparam name="T">The type of number for the loop.</typeparam>
    /// <param name="num">The index to count up or down to.</param>
    /// <returns>An enumeration from 0 to the index's value, or vice versa.</returns>
    [MustDisposeResource, Pure]
    public static IEnumerator<T> GetEnumerator<T>(this T num)
        where T : IComparisonOperators<T?, T?, bool>,
        ISubtractionOperators<T, T, T>,
        IIncrementOperators<T> =>
        num.For().GetEnumerator();

    /// <summary>
    /// The <see langword="for"/> statement executes a statement or a block of statements while a specified
    /// Boolean expression evaluates to <see langword="true"/>.
    /// </summary>
    /// <remarks><para><a href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/statements/iteration-statements#the-for-statement">
    /// See here for more information.
    /// </a></para></remarks>
    /// <typeparam name="T">The type of number for the loop.</typeparam>
    /// <typeparam name="TExternal">The type of external parameter to pass into the callback.</typeparam>
    /// <param name="upper">The length to reach to in the for loop.</param>
    /// <param name="external">Any external parameter to be passed repeatedly into the callback.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="int"/> from ranges 0 to <paramref name="upper"/> - 1.</returns>
    [LinqTunnel, Pure]
    public static IEnumerable<TExternal> For<T, TExternal>([NonNegativeValue] this T upper, TExternal external)
        where T : IComparisonOperators<T?, T?, bool>, ISubtractionOperators<T, T, T>, IIncrementOperators<T>
    {
        var abs = upper < default(T) ? default(T)! - upper : upper;

        for (T? i = default; i < abs; i++)
            yield return external;
    }

    /// <summary>
    /// The <see langword="for"/> statement executes a statement or a block of statements while a specified
    /// Boolean expression evaluates to <see langword="true"/>.
    /// </summary>
    /// <remarks><para><a href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/statements/iteration-statements#the-for-statement">
    /// See here for more information.
    /// </a></para></remarks>
    /// <typeparam name="T">The type of number for the loop.</typeparam>
    /// <typeparam name="TResult">The type of iterator.</typeparam>
    /// <param name="upper">The length to reach to in the for loop.</param>
    /// <param name="func">The function for each loop.</param>
    /// <returns>All instances that <paramref name="func"/> used in an <see cref="IEnumerable{T}"/>.</returns>
    [LinqTunnel, Pure]
    public static IEnumerable<TResult> For<T, TResult>(
        [NonNegativeValue] this T upper,
        [InstantHandle] Func<TResult> func
    )
        where T : IComparisonOperators<T?, T?, bool>, ISubtractionOperators<T, T, T>, IIncrementOperators<T>
    {
        var abs = upper < default(T) ? default(T)! - upper : upper;

        for (T? i = default; i < abs; i++)
            yield return func();
    }

    /// <summary>
    /// The <see langword="for"/> statement executes a statement or a block of statements while a specified
    /// Boolean expression evaluates to <see langword="true"/>.
    /// </summary>
    /// <remarks><para><a href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/statements/iteration-statements#the-for-statement">
    /// See here for more information.
    /// </a></para></remarks>
    /// <typeparam name="T">The type of number for the loop.</typeparam>
    /// <typeparam name="TResult">The type of iterator.</typeparam>
    /// <param name="upper">The length to reach to in the for loop.</param>
    /// <param name="func">The function for each loop.</param>
    /// <returns>All instances that <paramref name="func"/> used in an <see cref="IEnumerable{T}"/>.</returns>
    [LinqTunnel, Pure]
    public static IEnumerable<TResult> For<T, TResult>(
        [NonNegativeValue] this T upper,
        [InstantHandle] Converter<T, TResult> func
    )
        where T : IComparisonOperators<T?, T?, bool>, ISubtractionOperators<T, T, T>, IIncrementOperators<T>
    {
        var isNegative = upper < default(T);
        var abs = isNegative ? default(T)! - upper : upper;

        for (T? i = default; i < abs; i++)
            yield return func(isNegative ? upper - i : i);
    }

    /// <summary>
    /// The <see langword="for"/> statement executes a statement or a block of statements while a specified
    /// Boolean expression evaluates to <see langword="true"/>.
    /// </summary>
    /// <remarks><para><a href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/statements/iteration-statements#the-for-statement">
    /// See here for more information.
    /// </a></para></remarks>
    /// <typeparam name="T">The type of number for the loop.</typeparam>
    /// <param name="upper">The length to reach to in the for loop.</param>
    /// <param name="action">The action for each loop.</param>
    /// <returns>The parameter <paramref name="upper"/>.</returns>
    [NonNegativeValue]
    public static T For<T>([NonNegativeValue] this T upper, [InstantHandle] Action action)
        where T : IComparisonOperators<T?, T, bool>, IIncrementOperators<T>
    {
        for (T? i = default; i < upper; i++)
            action();

        return upper;
    }

    /// <summary>
    /// The <see langword="for"/> statement executes a statement or a block of statements while a specified
    /// Boolean expression evaluates to <see langword="true"/>.
    /// </summary>
    /// <remarks><para><a href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/statements/iteration-statements#the-for-statement">
    /// See here for more information.
    /// </a></para></remarks>
    /// <typeparam name="T">The type of number for the loop.</typeparam>
    /// <param name="upper">The length to reach to in the for loop.</param>
    /// <param name="action">The action for each loop.</param>
    /// <returns>The parameter <paramref name="upper"/>.</returns>
    [NonNegativeValue]
    public static T For<T>([NonNegativeValue] this T upper, [InstantHandle] Action<T> action)
        where T : IComparisonOperators<T?, T, bool>, IIncrementOperators<T>
    {
        for (T? i = default; i < upper; i++)
            action(i);

        return upper;
    }

    /// <summary>
    /// The <see langword="for"/> statement executes a statement or a block of statements while a specified
    /// Boolean expression evaluates to <see langword="true"/>.
    /// </summary>
    /// <remarks><para><a href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/statements/iteration-statements#the-for-statement">
    /// See here for more information.
    /// </a></para></remarks>
    /// <typeparam name="T">The type of number for the loop.</typeparam>
    /// <typeparam name="TExternal">The type of external parameter to pass into the callback.</typeparam>
    /// <param name="upper">The length to reach to in the for loop.</param>
    /// <param name="external">Any external parameter to be passed repeatedly into the callback.</param>
    /// <param name="action">The action for each loop.</param>
    /// <returns>The parameter <paramref name="upper"/>.</returns>
    [NonNegativeValue]
    public static T For<T, TExternal>(
        [NonNegativeValue] this T upper,
        TExternal external,
        [InstantHandle] Action<TExternal> action
    )
        where T : IComparisonOperators<T?, T, bool>, IIncrementOperators<T>
    {
        for (T? i = default; i < upper; i++)
            action(external);

        return upper;
    }

    /// <summary>
    /// The <see langword="for"/> statement executes a statement or a block of statements while a specified
    /// Boolean expression evaluates to <see langword="true"/>.
    /// </summary>
    /// <remarks><para><a href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/statements/iteration-statements#the-for-statement">
    /// See here for more information.
    /// </a></para></remarks>
    /// <typeparam name="T">The type of number for the loop.</typeparam>
    /// <typeparam name="TExternal">The type of external parameter to pass into the callback.</typeparam>
    /// <param name="upper">The length to reach to in the for loop.</param>
    /// <param name="external">Any external parameter to be passed repeatedly into the callback.</param>
    /// <param name="action">The action for each loop.</param>
    /// <returns>The parameter <paramref name="upper"/>.</returns>
    [NonNegativeValue]
    public static T For<T, TExternal>(
        [NonNegativeValue] this T upper,
        TExternal external,
        [InstantHandle] Action<T, TExternal> action
    )
        where T : IComparisonOperators<T?, T, bool>, IIncrementOperators<T>
    {
        for (T? i = default; i < upper; i++)
            action(i, external);

        return upper;
    }
#endif
    /// <inheritdoc cref="System.Array.FindAll{T}(T[], Predicate{T})"/>
    public static T[] FindAll<T>(this T[] array, [InstantHandle] Predicate<T> match) => Array.FindAll(array, match);

    /// <inheritdoc cref="System.Array.ConvertAll{TInput, TOutput}(TInput[], Converter{TInput, TOutput})"/>
    public static TOutput[] ConvertAll<TInput, TOutput>(
        this TInput[] array,
        [InstantHandle] Converter<TInput, TOutput> converter
    ) =>
        Array.ConvertAll(array, converter);

    /// <inheritdoc cref="System.Array.AsReadOnly{T}(T[])"/>
    public static ReadOnlyCollection<T> AsReadOnly<T>(this T[]? array) => Array.AsReadOnly(array ?? []);
}
