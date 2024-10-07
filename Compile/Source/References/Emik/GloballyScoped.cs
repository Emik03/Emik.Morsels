// SPDX-License-Identifier: MPL-2.0
#pragma warning disable GlobalUsingsAnalyzer // ReSharper disable once RedundantUsingDirective.Global
global using static Emik.Morsels.GloballyScoped; // ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Methods and properties that are globally scoped.</summary>
static partial class GloballyScoped
{
    sealed class Compared<T>(Comparison<T?> comparer) : IComparer<T>
    {
        /// <inheritdoc />
        public int Compare(T? x, T? y) => comparer(x, y);
    }

    sealed class Compared<T, TResult>(Converter<T?, TResult> converter, IComparer<TResult> comparer) : IComparer<T>
    {
        /// <inheritdoc />
        public int Compare(T? x, T? y) => comparer.Compare(converter(x), converter(y));
    }

    sealed class Equated<T>(Func<T?, T?, bool> comparer, Func<T, int> hashCode) : IEqualityComparer<T>
    {
        /// <summary>Initializes a new instance of the <see cref="Equated{T}"/> class.</summary>
        /// <param name="comparer">The comparer to convert.</param>
        public Equated(IComparer<T> comparer)
            : this(FromIComparer(comparer), Default) { }

        /// <summary>Returns 0.</summary>
        /// <param name="_">The discard.</param>
        /// <returns>The value 0.</returns>
        public static int Default(T? _) => 0;

        /// <inheritdoc />
        public bool Equals(T? x, T? y) => comparer(x, y);

        /// <inheritdoc />
        public int GetHashCode(T obj) => hashCode(obj);

        /// <summary>Returns the equality function based on the <see cref="IComparer{T}"/>.</summary>
        /// <param name="comparer">The comparer to evaluate equality.</param>
        /// <returns>The equality function that wraps <paramref name="comparer"/>.</returns>
        // ReSharper disable NullableWarningSuppressionIsUsed
        static Func<T?, T?, bool> FromIComparer(IComparer<T> comparer) => (x, y) => comparer.Compare(x!, y!) is 0;
    }

    sealed class Equated<T, TResult>(Converter<T?, TResult> converter, IEqualityComparer<TResult> equalityComparer)
        : IEqualityComparer<T>
    {
        /// <inheritdoc />
        public bool Equals(T? x, T? y) => equalityComparer.Equals(converter(x), converter(y));

        /// <inheritdoc />
        // ReSharper disable once NullableWarningSuppressionIsUsed
        public int GetHashCode(T obj) => equalityComparer.GetHashCode(converter(obj)!);
    }

    /// <summary>The number of bits in a byte.</summary>
    public const int BitsInByte = 8;

    /// <summary>Gets the <see cref="Exception"/> that a collection cannot be empty.</summary>
    public static InvalidOperationException CannotBeEmpty { get; } = new("Buffer is empty.");

    /// <summary>Gets the <see cref="Exception"/> that represents an unreachable state.</summary>
    public static UnreachableException Unreachable { get; } = new();

    /// <summary>Disposes of the <paramref name="disposable"/> and sets it to <see langword="default"/>.</summary>
    /// <typeparam name="T">The type of <paramref name="disposable"/>.</typeparam>
    /// <param name="disposable">The disposable to dispose.</param>
    public static void DisposeOf<T>(ref T? disposable)
#if NET9_0_OR_GREATER
        where T : IDisposable, allows ref struct
#else
        where T : IDisposable
#endif
    {
        disposable?.Dispose();
        disposable = default;
    }

    /// <summary>Invokes a method.</summary>
    /// <param name="del">The method to invoke.</param>
    public static void Invoke([InstantHandle] Action del) => del();

    /// <summary>Performs nothing.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Noop()
    {
        // .maxstack 8
        // #if DEBUG
        // IL_0000: nop
        // IL_0001: ret
        // #elif RELEASE
        // IL_0000: ret
        // #endif
    }

    /// <summary>Performs nothing.</summary>
    /// <typeparam name="T">The type of discard.</typeparam>
    /// <param name="_">The discard.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Noop<T>(T _)
    {
        // .maxstack 8
        // #if DEBUG
        // IL_0000: nop
        // IL_0001: ret
        // #elif RELEASE
        // IL_0000: ret
        // #endif
    }

    /// <summary>Performs nothing.</summary>
    /// <typeparam name="T1">The first type of discard.</typeparam>
    /// <typeparam name="T2">The second type of discard.</typeparam>
    /// <param name="_">The first discard.</param>
    /// <param name="__">The second discard.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Noop<T1, T2>(T1 _, T2 __)
    {
        // .maxstack 8
        // #if DEBUG
        // IL_0000: nop
        // IL_0001: ret
        // #elif RELEASE
        // IL_0000: ret
        // #endif
    }

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
        [MaybeNullWhen(true)] out T ok
    )
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
        [MaybeNullWhen(true)] out TResult ok
    )
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
        [MaybeNullWhen(true)] out TResult ok
    )
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
        [MaybeNullWhen(true)] out TResult ok
    )
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
        [MaybeNullWhen(true)] out TResult ok
    )
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

    /// <summary>Gets a consistent prime number based on the line number this was called from.</summary>
    /// <param name="line">Automatically filled by compilers; the line number where this method was called.</param>
    /// <returns>The consistent pseudo-random prime number.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure, ValueRange(Primes.Min, Primes.MaxInt16)]
    public static short Prime([CallerLineNumber] int line = 0) => Primes.Index(line);

    /// <summary>Creates the <see cref="IComparer{T}"/> from the mapping.</summary>
    /// <typeparam name="T">The type to compare.</typeparam>
    /// <param name="comparison">The <see cref="Comparison{T}"/> to use.</param>
    /// <returns>The <see cref="IComparer{T}"/> that wraps the parameter <paramref name="comparison"/>.</returns>
    public static IComparer<T> Comparing<T>(Comparison<T?> comparison) => new Compared<T>(comparison);

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
        new Compared<T, TResult>(converter, comparer ?? Comparer<TResult>.Default);
#if !NET20 && !NET30 && !NET47 && !NETSTANDARD2_0 // Unique in the sense that they either don't have LINQ, or have tuples that don't implement ITuple.
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
    /// <summary>Creates the <see cref="IComparer{T}"/> from the mapping.</summary>
    /// <typeparam name="T">The type to compare.</typeparam>
    /// <param name="comparison">The <see cref="Comparison{T}"/> to use.</param>
    /// <returns>The <see cref="IComparer{T}"/> that wraps the parameter <paramref name="comparison"/>.</returns>
    public static IEqualityComparer<T> AsEquality<T>(this IComparer<T> comparison) => new Equated<T>(comparison);

    /// <summary>Creates the <see cref="IEqualityComparer{T}"/> from the mapping.</summary>
    /// <typeparam name="T">The type to compare.</typeparam>
    /// <typeparam name="TResult">The resulting value from the mapping used for comparison.</typeparam>
    /// <param name="converter">The converter to use.</param>
    /// <param name="comparer">If specified, the way the result of the delegate should be sorted.</param>
    /// <returns>The <see cref="IComparer{T}"/> that wraps the parameter <paramref name="converter"/>.</returns>
    public static IEqualityComparer<T> Equating<T, TResult>(
        Converter<T?, TResult> converter,
        IEqualityComparer<TResult>? comparer = null
    ) =>
        new Equated<T, TResult>(converter, comparer ?? EqualityComparer<TResult>.Default);

    /// <summary>Creates the <see cref="IEqualityComparer{T}"/> from the mapping.</summary>
    /// <typeparam name="T">The type to compare.</typeparam>
    /// <param name="comparer">The comparer to use.</param>
    /// <param name="hashCode">If specified, the hash code algorithm.</param>
    /// <returns>The <see cref="IComparer{T}"/> that wraps the parameter <paramref name="comparer"/>.</returns>
    public static IEqualityComparer<T> Equating<T>(Func<T?, T?, bool> comparer, Func<T, int>? hashCode = null) =>
        new Equated<T>(comparer, hashCode ?? Equated<T>.Default);

    /// <inheritdoc cref="Invoke"/>
    public static TResult Invoke<TResult>([InstantHandle] Func<TResult> del) => del();

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
