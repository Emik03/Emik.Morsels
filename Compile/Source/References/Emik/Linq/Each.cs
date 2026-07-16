// SPDX-License-Identifier: MPL-2.0
// ReSharper disable CheckNamespace
namespace Emik.Morsels;

/// <summary>Extension methods for iterating over a set of elements, or for generating new ones.</summary>
static partial class Each
{
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
        where T : IComparisonOperators<T?, T?, bool>, INumberBase<T> =>
        upper >= T.Zero
            ? Enumerable.Repeat(T.Zero, int.CreateChecked(upper)).Select((x, i) => x + T.CreateChecked(i))
            : Enumerable.Repeat(-upper, int.CreateChecked(-upper)).Select((x, i) => x - T.CreateChecked(i) - T.One);

    /// <summary>Gets an enumeration of a number.</summary>
    /// <typeparam name="T">The type of number for the loop.</typeparam>
    /// <param name="num">The index to count up or down to.</param>
    /// <returns>An enumeration from 0 to the index's value, or vice versa.</returns>
    [MustDisposeResource, Pure]
    public static IEnumerator<T> GetEnumerator<T>(this T num)
        where T : IComparisonOperators<T?, T?, bool>, INumberBase<T> =>
        num.For().GetEnumerator();
#else
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
#endif
}
