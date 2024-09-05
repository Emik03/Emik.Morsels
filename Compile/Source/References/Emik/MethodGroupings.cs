// SPDX-License-Identifier: MPL-2.0
#pragma warning disable GlobalUsingsAnalyzer

// ReSharper disable once RedundantUsingDirective.Global
global using static Emik.Morsels.MethodGroupings;

// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Methods to create methods.</summary>
static partial class MethodGroupings
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

    /// <summary>Gets a pseudo-random prime number.</summary>
    /// <param name="line">Automatically filled by compilers; the line number where this method was called.</param>
    /// <returns>The pseudo-random prime number.</returns>
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
}
