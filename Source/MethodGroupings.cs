#region Emik.MPL

// <copyright file="MethodGroupings.cs" company="Emik">
// Copyright (c) Emik. This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

#endregion

#pragma warning disable GlobalUsingsAnalyzer

#region

global using static Emik.Morsels.MethodGroupings;

#endregion

namespace Emik.Morsels;

/// <summary>Methods to create methods.</summary>
static partial class MethodGroupings
{
    /// <summary>Invokes a method.</summary>
    /// <param name="del">The method to invoke.</param>
    internal static void Invoke(Action del) => del();

    /// <summary>Create a delegate.</summary>
    /// <param name="del">The method group.</param>
    /// <returns>An invokable method.</returns>
    [Pure]
    internal static Action Action(Action del) => del;

    /// <inheritdoc cref="MethodGroupings.Action"/>
    [Pure]
    internal static Action<T> Action1<T>(Action<T> del) => del;

    /// <inheritdoc cref="MethodGroupings.Action"/>
    [Pure]
    internal static Action<T1, T2> Action2<T1, T2>(Action<T1, T2> del) => del;

    /// <inheritdoc cref="MethodGroupings.Action"/>
    [Pure]
    internal static Action<T1, T2, T3> Action3<T1, T2, T3>(Action<T1, T2, T3> del) => del;

    /// <inheritdoc cref="MethodGroupings.Action"/>
    [Pure]
    internal static Action<T1, T2, T3, T4> Action4<T1, T2, T3, T4>(Action<T1, T2, T3, T4> del) => del;

    /// <inheritdoc cref="MethodGroupings.Action"/>
    [Pure]
    internal static Func<T> Func<T>(Func<T> del) => del;

    /// <inheritdoc cref="MethodGroupings.Action"/>
    [Pure]
    internal static Func<T, TResult> Func1<T, TResult>(Func<T, TResult> del) => del;

    /// <inheritdoc cref="MethodGroupings.Action"/>
    [Pure]
    internal static Func<T1, T2, TResult> Func2<T1, T2, TResult>(Func<T1, T2, TResult> del) => del;

    /// <inheritdoc cref="MethodGroupings.Action"/>
    [Pure]
    internal static Func<T1, T2, T3, TResult> Func3<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> del) => del;

    /// <inheritdoc cref="MethodGroupings.Action"/>
    [Pure]
    internal static Func<T1, T2, T3, T4, TResult> Func4<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> del) =>
        del;

    /// <summary>Negates a predicate.</summary>
    /// <typeparam name="T">The type of item for the predicate.</typeparam>
    /// <param name="predicate">The predicate to negate.</param>
    /// <returns>The argument <paramref name="predicate"/> wrapped in another that negates its result.</returns>
    [Pure]
    internal static Func<T, bool> Not1<T>(Func<T, bool> predicate) => t => !predicate(t);

    /// <inheritdoc cref="MethodGroupings.Not{T}(Predicate{T})"/>
    [Pure]
    internal static Func<T, int, bool> Not2<T>(Func<T, int, bool> predicate) => (t, i) => !predicate(t, i);

    /// <inheritdoc cref="MethodGroupings.Not{T}(Predicate{T})"/>
    [Pure]
    internal static Predicate<T> Not<T>(Predicate<T> predicate) => t => !predicate(t);

    /// <inheritdoc cref="Invoke"/>
    internal static TResult Invoke<TResult>([InstantHandle] Func<TResult> del) => del();
}
