// SPDX-License-Identifier: MPL-2.0
namespace Emik.Morsels;
#pragma warning disable MA0048
/// <summary>Provides methods to convert <see cref="IEnumerator{T}"/> to <see cref="IEnumerable{T}"/>.</summary>
static partial class EnumeratorToEnumerable
{
    /// <summary>Wraps the <see cref="IEnumerator"/> inside a <see cref="IEnumerable{T}"/>.</summary>
    /// <param name="enumerator">The <see cref="IEnumerator"/> to encapsulate.</param>
    /// <returns>
    /// The <see cref="IEnumerator{T}"/> instance that returns the parameter <paramref name="enumerator"/>.
    /// </returns>
    [Pure]
    public static IEnumerator<object?> AsGeneric(this IEnumerator enumerator) => new Enumerator(enumerator);

    /// <summary>Wraps the <see cref="IEnumerator"/> inside a <see cref="IEnumerable{T}"/>.</summary>
    /// <param name="enumerator">The <see cref="IEnumerator"/> to encapsulate.</param>
    /// <returns>
    /// The <see cref="IEnumerator{T}"/> instance that returns the parameter <paramref name="enumerator"/>.
    /// </returns>
    [LinqTunnel, Pure]
    public static IEnumerable<object?> AsEnumerable(this IEnumerator enumerator) =>
        enumerator.AsGeneric().AsEnumerable();

    /// <summary>Wraps the <see cref="IEnumerator{T}"/> inside a <see cref="IEnumerable{T}"/>.</summary>
    /// <typeparam name="T">The type of item to enumerate.</typeparam>
    /// <param name="enumerator">The <see cref="IEnumerator{T}"/> to encapsulate.</param>
    /// <returns>
    /// The <see cref="IEnumerator{T}"/> instance that returns the parameter <paramref name="enumerator"/>.
    /// </returns>
    [LinqTunnel, Pure]
    public static IEnumerable<T> AsEnumerable<T>(this IEnumerator<T> enumerator) => new Enumerable<T>(enumerator);

    /// <summary>Wraps an <see cref="IEnumerator{T}"/> and exposes it from an <see cref="IEnumerable{T}"/> context.</summary>
    /// <typeparam name="T">The type of item to enumerate.</typeparam>
    sealed partial class Enumerable<T> : IEnumerable<T>
    {
        [ProvidesContext]
        readonly IEnumerator<T> _enumerator;

        /// <summary>Initializes a new instance of the <see cref="Enumerable{T}"/> class.</summary>
        /// <param name="e">The <see cref="IEnumerator{T}"/> to encapsulate.</param>
        public Enumerable(IEnumerator<T> e) => _enumerator = e;

        /// <inheritdoc />
        [CollectionAccess(CollectionAccessType.Read), Pure]
        public IEnumerator<T> GetEnumerator() => _enumerator;

        /// <inheritdoc />
        [CollectionAccess(CollectionAccessType.Read), Pure]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>Finalizes an instance of the <see cref="Enumerable{T}"/> class.</summary>
#pragma warning disable MA0055
        ~Enumerable() => _enumerator.Dispose();
#pragma warning restore MA0055
    }

    /// <summary>Wraps an <see cref="IEnumerator{T}"/> and exposes it from an <see cref="IEnumerable{T}"/> context.</summary>
    sealed partial class Enumerator : IEnumerator<object?>
    {
        [ProvidesContext]
        readonly IEnumerator _enumerator;

        /// <summary>Initializes a new instance of the <see cref="Enumerator"/> class.</summary>
        /// <param name="e">The <see cref="IEnumerator"/> to encapsulate.</param>
        public Enumerator(IEnumerator e) => _enumerator = e;

        /// <inheritdoc cref="IEnumerator{T}.Current" />
        [Pure]
        public object? Current => _enumerator.Current;

        /// <inheritdoc />
        public void Reset() => _enumerator.Reset();

        /// <inheritdoc />
        public void Dispose() { }

        /// <inheritdoc />
        public bool MoveNext() => _enumerator.MoveNext();
    }
}
