// SPDX-License-Identifier: MPL-2.0
#pragma warning disable GlobalUsingsAnalyzer
global using static Emik.Morsels.Undergo;

#pragma warning restore GlobalUsingsAnalyzer
// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Provides methods to wrap delegates around try-catch blocks.</summary>
static partial class Undergo
{
    /// <summary>Attempts to execute the <paramref name="action"/>.</summary>
    /// <param name="action">The action to execute.</param>
    /// <param name="err">
    /// When this method returns <see langword="true"/>, contains the <see cref="Exception"/> that was thrown.
    /// </param>
    /// <returns>The value indicating whether <paramref name="action"/> threw an <see cref="Exception"/>.</returns>
    public static bool Go([InstantHandle] Action action, [NotNullWhen(true)] out Exception? err)
    {
        try
        {
            action();
            err = null;
            return false;
        }
        catch (Exception ex)
        {
            err = ex;
            return true;
        }
    }

    /// <summary>Attempts to execute the <paramref name="action"/>.</summary>
    /// <typeparam name="T">The type of parameter to pass to <paramref name="action"/>.</typeparam>
    /// <param name="action">The action to execute.</param>
    /// <param name="param">The parameter to pass to <paramref name="action"/>.</param>
    /// <param name="err">
    /// When this method returns <see langword="true"/>, contains the <see cref="Exception"/> that was thrown.
    /// </param>
    /// <returns>The value indicating whether <paramref name="action"/> threw an <see cref="Exception"/>.</returns>
    public static bool Go<T>([InstantHandle] Action<T> action, in T param, [NotNullWhen(true)] out Exception? err)
    {
        try
        {
            action(param);
            err = null;
            return false;
        }
        catch (Exception ex)
        {
            err = ex;
            return true;
        }
    }

    /// <summary>Attempts to execute the <paramref name="action"/>.</summary>
    /// <typeparam name="T1">The first type of parameter to pass to <paramref name="action"/>.</typeparam>
    /// <typeparam name="T2">The second type of parameter to pass to <paramref name="action"/>.</typeparam>
    /// <param name="action">The action to execute.</param>
    /// <param name="first">The first parameter to pass to <paramref name="action"/>.</param>
    /// <param name="second">The second parameter to pass to <paramref name="action"/>.</param>
    /// <param name="err">
    /// When this method returns <see langword="true"/>, contains the <see cref="Exception"/> that was thrown.
    /// </param>
    /// <returns>The value indicating whether <paramref name="action"/> threw an <see cref="Exception"/>.</returns>
    public static bool Go<T1, T2>(
        [InstantHandle] Action<T1, T2> action,
        in T1 first,
        in T2 second,
        [NotNullWhen(true)] out Exception? err
    )
    {
        try
        {
            action(first, second);
            err = null;
            return false;
        }
        catch (Exception ex)
        {
            err = ex;
            return true;
        }
    }

    /// <summary>Attempts to execute the <paramref name="action"/>.</summary>
    /// <typeparam name="T1">The first type of parameter to pass to <paramref name="action"/>.</typeparam>
    /// <typeparam name="T2">The second type of parameter to pass to <paramref name="action"/>.</typeparam>
    /// <typeparam name="T3">The third type of parameter to pass to <paramref name="action"/>.</typeparam>
    /// <param name="action">The action to execute.</param>
    /// <param name="first">The first parameter to pass to <paramref name="action"/>.</param>
    /// <param name="second">The second parameter to pass to <paramref name="action"/>.</param>
    /// <param name="third">The third parameter to pass to <paramref name="action"/>.</param>
    /// <param name="err">
    /// When this method returns <see langword="true"/>, contains the <see cref="Exception"/> that was thrown.
    /// </param>
    /// <returns>The value indicating whether <paramref name="action"/> threw an <see cref="Exception"/>.</returns>
    public static bool Go<T1, T2, T3>(
        [InstantHandle] Action<T1, T2, T3> action,
        in T1 first,
        in T2 second,
        in T3 third,
        [NotNullWhen(true)] out Exception? err
    )
    {
        try
        {
            action(first, second, third);
            err = null;
            return false;
        }
        catch (Exception ex)
        {
            err = ex;
            return true;
        }
    }

    /// <summary>Attempts to execute the <paramref name="action"/>.</summary>
    /// <typeparam name="T1">The first type of parameter to pass to <paramref name="action"/>.</typeparam>
    /// <typeparam name="T2">The second type of parameter to pass to <paramref name="action"/>.</typeparam>
    /// <typeparam name="T3">The third type of parameter to pass to <paramref name="action"/>.</typeparam>
    /// <typeparam name="T4">The fourth type of parameter to pass to <paramref name="action"/>.</typeparam>
    /// <param name="action">The action to execute.</param>
    /// <param name="first">The first parameter to pass to <paramref name="action"/>.</param>
    /// <param name="second">The second parameter to pass to <paramref name="action"/>.</param>
    /// <param name="third">The third parameter to pass to <paramref name="action"/>.</param>
    /// <param name="fourth">The fourth parameter to pass to <paramref name="action"/>.</param>
    /// <param name="err">
    /// When this method returns <see langword="true"/>, contains the <see cref="Exception"/> that was thrown.
    /// </param>
    /// <returns>The value indicating whether <paramref name="action"/> threw an <see cref="Exception"/>.</returns>
    public static bool Go<T1, T2, T3, T4>(
        [InstantHandle] Action<T1, T2, T3, T4> action,
        in T1 first,
        in T2 second,
        in T3 third,
        in T4 fourth,
        [NotNullWhen(true)] out Exception? err
    )
    {
        try
        {
            action(first, second, third, fourth);
            err = null;
            return false;
        }
        catch (Exception ex)
        {
            err = ex;
            return true;
        }
    }

    /// <summary>Attempts to execute the <paramref name="func"/>.</summary>
    /// <typeparam name="T">The return type of <paramref name="func"/>.</typeparam>
    /// <param name="func">The function to execute.</param>
    /// <param name="err">
    /// When this method returns <see langword="true"/>, contains the <see cref="Exception"/> that was thrown.
    /// </param>
    /// <param name="ok">
    /// When this method returns <see langword="false"/>, contains the <typeparamref name="T"/> that was returned.
    /// </param>
    /// <returns>
    /// The value indicating whether <paramref name="func"/> threw an
    /// <see cref="Exception"/> or returned a <typeparamref name="T"/>.
    /// </returns>
    public static bool Go<T>(
        [InstantHandle] Func<T> func,
        [NotNullWhen(true)] out Exception? err,
        [NotNullWhen(false)] out T? ok
    )
        where T : notnull
    {
        try
        {
            ok = func();
            err = null;
            return false;
        }
        catch (Exception ex)
        {
            ok = default;
            err = ex;
            return true;
        }
    }

    /// <summary>Attempts to execute the <paramref name="func"/>.</summary>
    /// <typeparam name="T">The type of parameter to pass to <paramref name="func"/>.</typeparam>
    /// <typeparam name="TResult">The return type of <paramref name="func"/>.</typeparam>
    /// <param name="func">The function to execute.</param>
    /// <param name="param">The parameter to pass to <paramref name="func"/>.</param>
    /// <param name="err">
    /// When this method returns <see langword="true"/>, contains the <see cref="Exception"/> that was thrown.
    /// </param>
    /// <param name="ok">
    /// When this method returns <see langword="false"/>, contains the <typeparamref name="TResult"/> that was returned.
    /// </param>
    /// <returns>
    /// The value indicating whether <paramref name="func"/> threw an
    /// <see cref="Exception"/> or returned a <typeparamref name="TResult"/>.
    /// </returns>
    public static bool Go<T, TResult>(
        [InstantHandle] Func<T, TResult> func,
        in T param,
        [NotNullWhen(true)] out Exception? err,
        [NotNullWhen(false)] out TResult? ok
    )
        where TResult : notnull
    {
        try
        {
            ok = func(param);
            err = null;
            return false;
        }
        catch (Exception ex)
        {
            ok = default;
            err = ex;
            return true;
        }
    }

    /// <summary>Attempts to execute the <paramref name="func"/>.</summary>
    /// <typeparam name="T1">The first type of parameter to pass to <paramref name="func"/>.</typeparam>
    /// <typeparam name="T2">The second type of parameter to pass to <paramref name="func"/>.</typeparam>
    /// <typeparam name="TResult">The return type of <paramref name="func"/>.</typeparam>
    /// <param name="func">The function to execute.</param>
    /// <param name="first">The first parameter to pass to <paramref name="func"/>.</param>
    /// <param name="second">The second parameter to pass to <paramref name="func"/>.</param>
    /// <param name="err">
    /// When this method returns <see langword="true"/>, contains the <see cref="Exception"/> that was thrown.
    /// </param>
    /// <param name="ok">
    /// When this method returns <see langword="false"/>, contains the <typeparamref name="TResult"/> that was returned.
    /// </param>
    /// <returns>
    /// The value indicating whether <paramref name="func"/> threw an
    /// <see cref="Exception"/> or returned a <typeparamref name="TResult"/>.
    /// </returns>
    public static bool Go<T1, T2, TResult>(
        [InstantHandle] Func<T1, T2, TResult> func,
        in T1 first,
        in T2 second,
        [NotNullWhen(true)] out Exception? err,
        [NotNullWhen(false)] out TResult? ok
    )
        where TResult : notnull
    {
        try
        {
            ok = func(first, second);
            err = null;
            return false;
        }
        catch (Exception ex)
        {
            ok = default;
            err = ex;
            return true;
        }
    }

    /// <summary>Attempts to execute the <paramref name="func"/>.</summary>
    /// <typeparam name="T1">The first type of parameter to pass to <paramref name="func"/>.</typeparam>
    /// <typeparam name="T2">The second type of parameter to pass to <paramref name="func"/>.</typeparam>
    /// <typeparam name="T3">The third type of parameter to pass to <paramref name="func"/>.</typeparam>
    /// <typeparam name="TResult">The return type of <paramref name="func"/>.</typeparam>
    /// <param name="func">The function to execute.</param>
    /// <param name="first">The first parameter to pass to <paramref name="func"/>.</param>
    /// <param name="second">The second parameter to pass to <paramref name="func"/>.</param>
    /// <param name="third">The third parameter to pass to <paramref name="func"/>.</param>
    /// <param name="err">
    /// When this method returns <see langword="true"/>, contains the <see cref="Exception"/> that was thrown.
    /// </param>
    /// <param name="ok">
    /// When this method returns <see langword="false"/>, contains the <typeparamref name="TResult"/> that was returned.
    /// </param>
    /// <returns>
    /// The value indicating whether <paramref name="func"/> threw an
    /// <see cref="Exception"/> or returned a <typeparamref name="TResult"/>.
    /// </returns>
    public static bool Go<T1, T2, T3, TResult>(
        [InstantHandle] Func<T1, T2, T3, TResult> func,
        in T1 first,
        in T2 second,
        in T3 third,
        [NotNullWhen(true)] out Exception? err,
        [NotNullWhen(false)] out TResult? ok
    )
        where TResult : notnull
    {
        try
        {
            ok = func(first, second, third);
            err = null;
            return false;
        }
        catch (Exception ex)
        {
            ok = default;
            err = ex;
            return true;
        }
    }

    /// <summary>Attempts to execute the <paramref name="func"/>.</summary>
    /// <typeparam name="T1">The first type of parameter to pass to <paramref name="func"/>.</typeparam>
    /// <typeparam name="T2">The second type of parameter to pass to <paramref name="func"/>.</typeparam>
    /// <typeparam name="T3">The third type of parameter to pass to <paramref name="func"/>.</typeparam>
    /// <typeparam name="T4">The fourth type of parameter to pass to <paramref name="func"/>.</typeparam>
    /// <typeparam name="TResult">The return type of <paramref name="func"/>.</typeparam>
    /// <param name="func">The function to execute.</param>
    /// <param name="first">The first parameter to pass to <paramref name="func"/>.</param>
    /// <param name="second">The second parameter to pass to <paramref name="func"/>.</param>
    /// <param name="third">The third parameter to pass to <paramref name="func"/>.</param>
    /// <param name="fourth">The fourth parameter to pass to <paramref name="func"/>.</param>
    /// <param name="err">
    /// When this method returns <see langword="true"/>, contains the <see cref="Exception"/> that was thrown.
    /// </param>
    /// <param name="ok">
    /// When this method returns <see langword="false"/>, contains the <typeparamref name="TResult"/> that was returned.
    /// </param>
    /// <returns>
    /// The value indicating whether <paramref name="func"/> threw an
    /// <see cref="Exception"/> or returned a <typeparamref name="TResult"/>.
    /// </returns>
    public static bool Go<T1, T2, T3, T4, TResult>(
        [InstantHandle] Func<T1, T2, T3, T4, TResult> func,
        in T1 first,
        in T2 second,
        in T3 third,
        in T4 fourth,
        [NotNullWhen(true)] out Exception? err,
        [NotNullWhen(false)] out TResult? ok
    )
        where TResult : notnull
    {
        try
        {
            ok = func(first, second, third, fourth);
            err = null;
            return false;
        }
        catch (Exception ex)
        {
            ok = default;
            err = ex;
            return true;
        }
    }
}
