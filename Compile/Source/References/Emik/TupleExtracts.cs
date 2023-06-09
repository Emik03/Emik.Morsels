// SPDX-License-Identifier: MPL-2.0
#pragma warning disable GlobalUsingsAnalyzer

// ReSharper disable once RedundantUsingDirective.Global
global using static Emik.Morsels.TupleExtracts;

// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Methods to get elements of a tuple.</summary>
static partial class TupleExtracts
{
    /// <summary>Gets the first item of the tuple.</summary>
    /// <typeparam name="T1">The first type of the tuple.</typeparam>
    /// <typeparam name="T2">The second type of the tuple.</typeparam>
    /// <param name="tuple">The tuple to get the value from.</param>
    /// <returns>The field <see cref="ValueTuple{T1, T2}.Item1"/> from the parameter <paramref name="tuple"/>.</returns>
    public static T1 First<T1, T2>((T1, T2) tuple) => tuple.Item1;

    /// <summary>Gets the second item of the tuple.</summary>
    /// <typeparam name="T1">The first type of the tuple.</typeparam>
    /// <typeparam name="T2">The second type of the tuple.</typeparam>
    /// <param name="tuple">The tuple to get the value from.</param>
    /// <returns>The field <see cref="ValueTuple{T1, T2}.Item2"/> from the parameter <paramref name="tuple"/>.</returns>
    public static T2 Second<T1, T2>((T1, T2) tuple) => tuple.Item2;
    // Unique in the sense that they either don't have LINQ, or have tuples that don't implement ITuple.
#if !NET20 && !NET30 && !NET47 && !NETSTANDARD2_0
    /// <summary>Gets the enumeration of the tuple.</summary>
    /// <param name="tuple">The tuple to enumerate.</param>
    /// <returns>The enumeration of the parameter <paramref name="tuple"/>.</returns>
    public static IEnumerable<object?> AsEnumerable(this ITuple tuple) => tuple.Length.For(i => tuple[i]);

    /// <summary>Gets the enumeration of the tuple.</summary>
    /// <typeparam name="T">The type of tuple.</typeparam>
    /// <param name="tuple">The tuple to enumerate.</param>
    /// <returns>The enumeration of the parameter <paramref name="tuple"/>.</returns>
    public static IEnumerable<object?> AsEnumerable<T>(this T tuple)
        where T : ITuple =>
        tuple.Length.For(i => tuple[i]);
#endif
}
