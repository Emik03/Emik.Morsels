// SPDX-License-Identifier: MPL-2.0
#pragma warning disable GlobalUsingsAnalyzer

// ReSharper disable once RedundantUsingDirective.Global
global using static Emik.Morsels.MethodGroupings;

// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Methods to create methods.</summary>
static partial class MethodGroupings
{
    sealed class Comparer<T, TResult>(Converter<T?, TResult> converter, IComparer<TResult> comparer) : IComparer<T>
    {
        /// <inheritdoc />
        public int Compare(T? x, T? y) => comparer.Compare(converter(x), converter(y));
    }

    /// <summary>Invokes a method.</summary>
    /// <param name="del">The method to invoke.</param>
    public static void Invoke([InstantHandle] Action del) => del();

    /// <summary>Performs nothing.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Noop() { }

    /// <summary>Performs nothing.</summary>
    /// <typeparam name="T">The type of discard.</typeparam>
    /// <param name="_">The discard.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Noop<T>(T _) { }

    /// <summary>Performs nothing.</summary>
    /// <typeparam name="T1">The first type of discard.</typeparam>
    /// <typeparam name="T2">The second type of discard.</typeparam>
    /// <param name="_">The first discard.</param>
    /// <param name="__">The second discard.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Noop<T1, T2>(T1 _, T2 __) { }

    /// <summary>Create a delegate.</summary>
    /// <param name="del">The method group.</param>
    /// <returns>An invokable method.</returns>
    [Pure]
    public static Action Action(Action del) => del;

    /// <inheritdoc cref="MethodGroupings.Action"/>
    [Pure]
    public static Action<T> Action1<T>(Action<T> del) => del;

    /// <inheritdoc cref="MethodGroupings.Action"/>
    [Pure]
    public static Action<T1, T2> Action2<T1, T2>(Action<T1, T2> del) => del;

    /// <inheritdoc cref="MethodGroupings.Action"/>
    [Pure]
    public static Action<T1, T2, T3> Action3<T1, T2, T3>(Action<T1, T2, T3> del) => del;

    /// <inheritdoc cref="MethodGroupings.Action"/>
    [Pure]
    public static Action<T1, T2, T3, T4> Action4<T1, T2, T3, T4>(Action<T1, T2, T3, T4> del) => del;

    /// <inheritdoc cref="MethodGroupings.Action"/>
    [Pure]
    public static Func<T> Func<T>(Func<T> del) => del;

    /// <inheritdoc cref="MethodGroupings.Action"/>
    [Pure]
    public static Func<T, TResult> Func1<T, TResult>(Func<T, TResult> del) => del;

    /// <inheritdoc cref="MethodGroupings.Action"/>
    [Pure]
    public static Func<T1, T2, TResult> Func2<T1, T2, TResult>(Func<T1, T2, TResult> del) => del;

    /// <inheritdoc cref="MethodGroupings.Action"/>
    [Pure]
    public static Func<T1, T2, T3, TResult> Func3<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> del) => del;

    /// <inheritdoc cref="MethodGroupings.Action"/>
    [Pure]
    public static Func<T1, T2, T3, T4, TResult> Func4<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> del) =>
        del;

    /// <summary>Negates a predicate.</summary>
    /// <typeparam name="T">The type of item for the predicate.</typeparam>
    /// <param name="predicate">The predicate to negate.</param>
    /// <returns>The argument <paramref name="predicate"/> wrapped in another that negates its result.</returns>
    [Pure]
    public static Func<T, bool> Not1<T>(Func<T, bool> predicate) => t => !predicate(t);

    /// <inheritdoc cref="MethodGroupings.Not{T}(Predicate{T})"/>
    [Pure]
    public static Func<T, int, bool> Not2<T>(Func<T, int, bool> predicate) => (t, i) => !predicate(t, i);

    /// <summary>Creates the <see cref="IComparer{T}"/> from the mapping.</summary>
    /// <typeparam name="T">The type to compare.</typeparam>
    /// <typeparam name="TResult">The resulting value from the mapping used for comparison.</typeparam>
    /// <param name="converter">The converter to use.</param>
    /// <param name="comparer">If specified, the way the result of the delegate should be sorted.</param>
    /// <returns>The <see cref="IComparer{T}"/> that wraps the parameter <paramref name="converter"/>.</returns>
    public static IComparer<T> Comparing<T, TResult>(
        Converter<T?, TResult> converter,
        IComparer<TResult>? comparer = null
    ) =>
        new Comparer<T, TResult>(converter, comparer ?? Comparer<TResult>.Default);

    /// <inheritdoc cref="MethodGroupings.Not{T}(Predicate{T})"/>
    [Pure]
    public static Predicate<T> Not<T>(Predicate<T> predicate) => t => !predicate(t);

    /// <inheritdoc cref="Invoke"/>
    public static TResult Invoke<TResult>([InstantHandle] Func<TResult> del) => del();
}
