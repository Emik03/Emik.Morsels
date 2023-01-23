// SPDX-License-Identifier: MPL-2.0
#pragma warning disable GlobalUsingsAnalyzer

global using static Emik.Morsels.TupleExtracts;

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
}
