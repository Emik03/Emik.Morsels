// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Similar to <see cref="Each"/>, but with control flow, using <see cref="ControlFlow"/>.</summary>
// ReSharper disable LoopCanBePartlyConvertedToQuery
static class EachWithControlFlow
{
#if !NET7_0_OR_GREATER
    /// <summary>
    /// The <see langword="for"/> statement executes a statement or a block of statements while a specified
    /// Boolean expression evaluates to <see langword="true"/>.
    /// </summary>
    /// <remarks><para>https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/statements/iteration-statements#the-for-statement.</para></remarks>
    /// <param name="upper">The length to reach to in the for loop.</param>
    /// <param name="func">The action for each loop.</param>
    /// <returns>The parameter <paramref name="upper"/>.</returns>
    [NonNegativeValue]
    internal static int For([NonNegativeValue] this int upper, [InstantHandle] Func<ControlFlow> func)
    {
        for (var i = 0; i < upper; i++)
            if (func() is ControlFlow.Break)
                break;

        return upper;
    }

    /// <summary>
    /// The <see langword="for"/> statement executes a statement or a block of statements while a specified
    /// Boolean expression evaluates to <see langword="true"/>.
    /// </summary>
    /// <remarks><para>https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/statements/iteration-statements#the-for-statement.</para></remarks>
    /// <param name="upper">The length to reach to in the for loop.</param>
    /// <param name="func">The action for each loop.</param>
    /// <returns>The parameter <paramref name="upper"/>.</returns>
    [NonNegativeValue]
    internal static int For([NonNegativeValue] this int upper, [InstantHandle] Func<int, ControlFlow> func)
    {
        for (var i = 0; i < upper; i++)
            if (func(i) is ControlFlow.Break)
                break;

        return upper;
    }

    /// <summary>
    /// The <see langword="for"/> statement executes a statement or a block of statements while a specified
    /// Boolean expression evaluates to <see langword="true"/>.
    /// </summary>
    /// <remarks><para>https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/statements/iteration-statements#the-for-statement.</para></remarks>
    /// <typeparam name="TExternal">The type of external parameter to pass into the callback.</typeparam>
    /// <param name="upper">The length to reach to in the for loop.</param>
    /// <param name="external">Any external parameter to be passed repeatedly into the callback.</param>
    /// <param name="func">The action for each loop.</param>
    /// <returns>The parameter <paramref name="upper"/>.</returns>
    [NonNegativeValue]
    internal static int For<TExternal>(
        [NonNegativeValue] this int upper,
        TExternal external,
        [InstantHandle] Func<TExternal, ControlFlow> func
    )
    {
        for (var i = 0; i < upper; i++)
            if (func(external) is ControlFlow.Break)
                break;

        return upper;
    }

    /// <summary>
    /// The <see langword="for"/> statement executes a statement or a block of statements while a specified
    /// Boolean expression evaluates to <see langword="true"/>.
    /// </summary>
    /// <remarks><para>https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/statements/iteration-statements#the-for-statement.</para></remarks>
    /// <typeparam name="TExternal">The type of external parameter to pass into the callback.</typeparam>
    /// <param name="upper">The length to reach to in the for loop.</param>
    /// <param name="external">Any external parameter to be passed repeatedly into the callback.</param>
    /// <param name="func">The action for each loop.</param>
    /// <returns>The parameter <paramref name="upper"/>.</returns>
    [NonNegativeValue]
    internal static int For<TExternal>(
        [NonNegativeValue] this int upper,
        TExternal external,
        [InstantHandle] Func<int, TExternal, ControlFlow> func
    )
    {
        for (var i = 0; i < upper; i++)
            if (func(i, external) is ControlFlow.Break)
                break;

        return upper;
    }
#endif
#if !NET20 && !NET30
    /// <summary>
    /// The <see langword="foreach"/> statement executes a statement or a block of statements for each element in an
    /// instance of the type that implements the <see cref="IEnumerable"/> or <see cref="IEnumerable{T}"/> interface.
    /// </summary>
    /// <remarks><para>https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/statements/iteration-statements#the-foreach-statement.</para></remarks>
    /// <typeparam name="T">The type of iterator.</typeparam>
    /// <param name="iterable">The collection of items to go through one-by-one.</param>
    /// <param name="func">The action to do on each item in <paramref name="iterable"/>.</param>
    /// <returns>The parameter <paramref name="iterable"/>.</returns>
    internal static ICollection<T> For<T>(
        [InstantHandle] this IEnumerable<T> iterable,
        [InstantHandle] Func<T, ControlFlow> func
    )
    {
        var list = iterable.ToCollectionLazily();

        foreach (var item in list)
            if (func(item) is ControlFlow.Break)
                break;

        return list;
    }

    /// <summary>
    /// The <see langword="foreach"/> statement executes a statement or a block of statements for each element in an
    /// instance of the type that implements the <see cref="IEnumerable"/> or <see cref="IEnumerable{T}"/> interface.
    /// </summary>
    /// <remarks><para>https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/statements/iteration-statements#the-foreach-statement.</para></remarks>
    /// <typeparam name="T">The type of iterator.</typeparam>
    /// <typeparam name="TExternal">The type of external parameter to pass into the callback.</typeparam>
    /// <param name="iterable">The collection of items to go through one-by-one.</param>
    /// <param name="external">Any external parameter to be passed repeatedly into the callback.</param>
    /// <param name="func">The action to do on each item in <paramref name="iterable"/>.</param>
    /// <returns>The parameter <paramref name="iterable"/>.</returns>
    internal static ICollection<T> For<T, TExternal>(
        [InstantHandle] this IEnumerable<T> iterable,
        TExternal external,
        [InstantHandle] Func<T, TExternal, ControlFlow> func
    )
    {
        var list = iterable.ToCollectionLazily();

        foreach (var item in list)
            if (func(item, external) is ControlFlow.Break)
                break;

        return list;
    }

    /// <summary>
    /// The <see langword="foreach"/> statement executes a statement or a block of statements for each element in an
    /// instance of the type that implements the <see cref="IEnumerable"/> or <see cref="IEnumerable{T}"/> interface.
    /// </summary>
    /// <remarks><para>https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/statements/iteration-statements#the-foreach-statement.</para></remarks>
    /// <typeparam name="T">The type of iterator.</typeparam>
    /// <param name="iterable">The collection of items to go through one-by-one.</param>
    /// <param name="func">The action to do on each item in <paramref name="iterable"/>.</param>
    /// <returns>The parameter <paramref name="iterable"/>.</returns>
    internal static ICollection<T> For<T>(
        [InstantHandle] this IEnumerable<T> iterable,
        [InstantHandle] Func<T, int, ControlFlow> func
    )
    {
        var list = iterable.ToCollectionLazily();
        var i = 0;

        foreach (var item in list)
            if (func(item, checked(i++)) is ControlFlow.Break)
                break;

        return list;
    }

    /// <summary>
    /// The <see langword="foreach"/> statement executes a statement or a block of statements for each element in an
    /// instance of the type that implements the <see cref="IEnumerable"/> or <see cref="IEnumerable{T}"/> interface.
    /// </summary>
    /// <remarks><para>https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/statements/iteration-statements#the-foreach-statement.</para></remarks>
    /// <typeparam name="T">The type of iterator.</typeparam>
    /// <typeparam name="TExternal">The type of external parameter to pass into the callback.</typeparam>
    /// <param name="iterable">The collection of items to go through one-by-one.</param>
    /// <param name="external">Any external parameter to be passed repeatedly into the callback.</param>
    /// <param name="func">The action to do on each item in <paramref name="iterable"/>.</param>
    /// <returns>The parameter <paramref name="iterable"/>.</returns>
    internal static ICollection<T> For<T, TExternal>(
        [InstantHandle] this IEnumerable<T> iterable,
        TExternal external,
        [InstantHandle] Func<T, int, TExternal, ControlFlow> func
    )
    {
        var list = iterable.ToCollectionLazily();
        var i = 0;

        foreach (var item in list)
            if (func(item, checked(i++), external) is ControlFlow.Break)
                break;

        return list;
    }

    /// <summary>
    /// The <see langword="foreach"/> statement executes a statement or a block of statements for each element in an
    /// instance of the type that implements the <see cref="IEnumerable"/> or <see cref="IEnumerable{T}"/> interface.
    /// </summary>
    /// <remarks><para>https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/statements/iteration-statements#the-foreach-statement.</para></remarks>
    /// <typeparam name="TKey">The type of key in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of value in the dictionary.</typeparam>
    /// <param name="dictionary">The collection of items to go through one-by-one.</param>
    /// <param name="func">The action to do on each item in <paramref name="dictionary"/>.</param>
    /// <returns>The parameter <paramref name="dictionary"/>.</returns>
    internal static IDictionary<TKey, TValue> For<TKey, TValue>(
        [InstantHandle] this IDictionary<TKey, TValue> dictionary,
        [InstantHandle] Func<TKey, TValue, ControlFlow> func
    )
    {
        foreach (var kvp in dictionary)
            if (func(kvp.Key, kvp.Value) is ControlFlow.Break)
                break;

        return dictionary;
    }

    /// <summary>
    /// The <see langword="foreach"/> statement executes a statement or a block of statements for each element in an
    /// instance of the type that implements the <see cref="IEnumerable"/> or <see cref="IEnumerable{T}"/> interface.
    /// </summary>
    /// <remarks><para>https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/statements/iteration-statements#the-foreach-statement.</para></remarks>
    /// <typeparam name="TKey">The type of key in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of value in the dictionary.</typeparam>
    /// <typeparam name="TExternal">The type of external parameter to pass into the callback.</typeparam>
    /// <param name="dictionary">The collection of items to go through one-by-one.</param>
    /// <param name="external">Any external parameter to be passed repeatedly into the callback.</param>
    /// <param name="func">The action to do on each item in <paramref name="dictionary"/>.</param>
    /// <returns>The parameter <paramref name="dictionary"/>.</returns>
    internal static IDictionary<TKey, TValue> For<TKey, TValue, TExternal>(
        [InstantHandle] this IDictionary<TKey, TValue> dictionary,
        TExternal external,
        [InstantHandle] Func<TKey, TValue, TExternal, ControlFlow> func
    )
    {
        foreach (var kvp in dictionary)
            if (func(kvp.Key, kvp.Value, external) is ControlFlow.Break)
                break;

        return dictionary;
    }

    /// <summary>
    /// The <see langword="foreach"/> statement executes a statement or a block of statements for each element in an
    /// instance of the type that implements the <see cref="IEnumerable"/> or <see cref="IEnumerable{T}"/> interface.
    /// </summary>
    /// <remarks><para>https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/statements/iteration-statements#the-foreach-statement.</para></remarks>
    /// <typeparam name="TKey">The type of key in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of value in the dictionary.</typeparam>
    /// <param name="dictionary">The collection of items to go through one-by-one.</param>
    /// <param name="func">The action to do on each item in <paramref name="dictionary"/>.</param>
    /// <returns>The parameter <paramref name="dictionary"/>.</returns>
    internal static IDictionary<TKey, TValue> For<TKey, TValue>(
        [InstantHandle] this IDictionary<TKey, TValue> dictionary,
        [InstantHandle] Func<TKey, TValue, int, ControlFlow> func
    )
    {
        var i = 0;

        foreach (var kvp in dictionary)
            if (func(kvp.Key, kvp.Value, checked(i++)) is ControlFlow.Break)
                break;

        return dictionary;
    }

    /// <summary>
    /// The <see langword="foreach"/> statement executes a statement or a block of statements for each element in an
    /// instance of the type that implements the <see cref="IEnumerable"/> or <see cref="IEnumerable{T}"/> interface.
    /// </summary>
    /// <remarks><para>https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/statements/iteration-statements#the-foreach-statement.</para></remarks>
    /// <typeparam name="TKey">The type of key in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of value in the dictionary.</typeparam>
    /// <typeparam name="TExternal">The type of external parameter to pass into the callback.</typeparam>
    /// <param name="dictionary">The collection of items to go through one-by-one.</param>
    /// <param name="external">Any external parameter to be passed repeatedly into the callback.</param>
    /// <param name="func">The action to do on each item in <paramref name="dictionary"/>.</param>
    /// <returns>The parameter <paramref name="dictionary"/>.</returns>
    internal static IDictionary<TKey, TValue> For<TKey, TValue, TExternal>(
        [InstantHandle] this IDictionary<TKey, TValue> dictionary,
        TExternal external,
        [InstantHandle] Func<TKey, TValue, int, TExternal, ControlFlow> func
    )
    {
        var i = 0;

        foreach (var kvp in dictionary)
            if (func(kvp.Key, kvp.Value, checked(i++), external) is ControlFlow.Break)
                break;

        return dictionary;
    }
#endif
#if NET7_0_OR_GREATER
    /// <summary>
    /// The <see langword="for"/> statement executes a statement or a block of statements while a specified
    /// Boolean expression evaluates to <see langword="true"/>.
    /// </summary>
    /// <remarks><para>https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/statements/iteration-statements#the-for-statement.</para></remarks>
    /// <typeparam name="T">The type of number for the loop.</typeparam>
    /// <param name="upper">The length to reach to in the for loop.</param>
    /// <param name="func">The action for each loop.</param>
    /// <returns>The parameter <paramref name="upper"/>.</returns>
    [NonNegativeValue]
    internal static T For<T>([NonNegativeValue] this T upper, [InstantHandle] Func<ControlFlow> func)
        where T : IComparisonOperators<T, T, bool>, INumberBase<T>
    {
        for (var i = T.Zero; i < upper; i++)
            if (func() is ControlFlow.Break)
                break;

        return upper;
    }

    /// <summary>
    /// The <see langword="for"/> statement executes a statement or a block of statements while a specified
    /// Boolean expression evaluates to <see langword="true"/>.
    /// </summary>
    /// <remarks><para>https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/statements/iteration-statements#the-for-statement.</para></remarks>
    /// <typeparam name="T">The type of number for the loop.</typeparam>
    /// <param name="upper">The length to reach to in the for loop.</param>
    /// <param name="func">The action for each loop.</param>
    /// <returns>The parameter <paramref name="upper"/>.</returns>
    [NonNegativeValue]
    internal static T For<T>([NonNegativeValue] this T upper, [InstantHandle] Func<T, ControlFlow> func)
        where T : IComparisonOperators<T, T, bool>, INumberBase<T>
    {
        for (var i = T.Zero; i < upper; i++)
            if (func(i) is ControlFlow.Break)
                break;

        return upper;
    }

    /// <summary>
    /// The <see langword="for"/> statement executes a statement or a block of statements while a specified
    /// Boolean expression evaluates to <see langword="true"/>.
    /// </summary>
    /// <remarks><para>https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/statements/iteration-statements#the-for-statement.</para></remarks>
    /// <typeparam name="T">The type of number for the loop.</typeparam>
    /// <typeparam name="TExternal">The type of external parameter to pass into the callback.</typeparam>
    /// <param name="upper">The length to reach to in the for loop.</param>
    /// <param name="external">Any external parameter to be passed repeatedly into the callback.</param>
    /// <param name="func">The action for each loop.</param>
    /// <returns>The parameter <paramref name="upper"/>.</returns>
    [NonNegativeValue]
    internal static T For<T, TExternal>(
        [NonNegativeValue] this T upper,
        TExternal external,
        [InstantHandle] Func<TExternal, ControlFlow> func
    )
        where T : IComparisonOperators<T, T, bool>, INumberBase<T>
    {
        for (var i = T.Zero; i < upper; i++)
            if (func(external) is ControlFlow.Break)
                break;

        return upper;
    }

    /// <summary>
    /// The <see langword="for"/> statement executes a statement or a block of statements while a specified
    /// Boolean expression evaluates to <see langword="true"/>.
    /// </summary>
    /// <remarks><para>https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/statements/iteration-statements#the-for-statement.</para></remarks>
    /// <typeparam name="T">The type of number for the loop.</typeparam>
    /// <typeparam name="TExternal">The type of external parameter to pass into the callback.</typeparam>
    /// <param name="upper">The length to reach to in the for loop.</param>
    /// <param name="external">Any external parameter to be passed repeatedly into the callback.</param>
    /// <param name="func">The action for each loop.</param>
    /// <returns>The parameter <paramref name="upper"/>.</returns>
    [NonNegativeValue]
    internal static T For<T, TExternal>(
        [NonNegativeValue] this T upper,
        TExternal external,
        [InstantHandle] Func<T, TExternal, ControlFlow> func
    )
        where T : IComparisonOperators<T, T, bool>, INumberBase<T>
    {
        for (var i = T.Zero; i < upper; i++)
            if (func(i, external) is ControlFlow.Break)
                break;

        return upper;
    }
#endif
}

/// <summary>Determines control flow for loops in <see cref="Each"/>.</summary>
#pragma warning disable MA0048
enum ControlFlow
#pragma warning restore MA0048
{
    /// <summary>The value indicating that the loop should continue.</summary>
    Continue,

    /// <summary>The value indicating that the loop should break.</summary>
    Break,
}
