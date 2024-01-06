// SPDX-License-Identifier: MPL-2.0

// ReSharper disable RedundantExtendsListEntry
// ReSharper disable once CheckNamespace
namespace Emik.Morsels;
#pragma warning disable MA0048, IDISP005
/// <summary>Provides methods to convert <see cref="IEnumerator{T}"/> to <see cref="IEnumerable{T}"/>.</summary>
static partial class EnumeratorToEnumerable
{
    /// <summary>Collects <see cref="IComparer"/> and <see cref="IEqualityComparer"/> instances.</summary>
    sealed class ComparerCollector : IComparer, IEqualityComparer
    {
        /// <summary>The most common usage is with tuples, in which the maximum capacity is 8.</summary>
        const int Capacity = 8;

        public List<object?> List { get; } = new(Capacity);

        /// <inheritdoc />
        bool IEqualityComparer.Equals(object? x, object? y) => Append(x, true);

        /// <inheritdoc />
        int IComparer.Compare(object? x, object? y) => Append(x, 0);

        /// <inheritdoc />
        int IEqualityComparer.GetHashCode(object? obj) => Append(obj, 0);

        T Append<T>(object? obj, T ret)
        {
            List.Add(obj);
            return ret;
        }
    }

    /// <summary>
    /// Wraps an <see cref="IEnumerator{T}"/> and exposes it from an <see cref="IEnumerable{T}"/> context.
    /// </summary>
    /// <param name="enumerator">The <see cref="IEnumerator{T}"/> to encapsulate.</param>
    /// <typeparam name="T">The type of item to enumerate.</typeparam>
    sealed partial class Enumerable<T>([HandlesResourceDisposal, ProvidesContext] IEnumerator<T> enumerator)
        : IDisposable, IEnumerable<T>
    {
        /// <inheritdoc />
        [CollectionAccess(CollectionAccessType.None)]
        public void Dispose() => enumerator.Dispose();

        /// <inheritdoc />
        [CollectionAccess(CollectionAccessType.Read), Pure]
        public IEnumerator<T> GetEnumerator() => enumerator;

        /// <inheritdoc />
        [CollectionAccess(CollectionAccessType.Read), Pure]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    /// <summary>
    /// Wraps an <see cref="IEnumerator{T}"/> and exposes it from an <see cref="IEnumerable{T}"/> context.
    /// </summary>
    /// <param name="enumerator">The enumerator to encapsulate.</param>
    sealed partial class Enumerator([HandlesResourceDisposal, ProvidesContext] IEnumerator enumerator)
        : IEnumerator<object?>
    {
        /// <inheritdoc cref="IEnumerator{T}.Current" />
        [Pure]
        public object? Current => enumerator.Current;

        /// <inheritdoc />
        public void Dispose() => (enumerator as IDisposable)?.Dispose();

        /// <inheritdoc />
        public void Reset() => enumerator.Reset();

        /// <inheritdoc />
        public bool MoveNext() => enumerator.MoveNext();
    }

    /// <summary>Wraps the enumerator inside an <see cref="IEnumerable{T}"/>.</summary>
    /// <param name="enumerator">The enumerator to encapsulate.</param>
    /// <returns>
    /// The <see cref="IEnumerator{T}"/> instance that returns the parameter <paramref name="enumerator"/>.
    /// </returns>
    [MustDisposeResource, Pure]
    public static IEnumerator<object?> AsGeneric([HandlesResourceDisposal] this IEnumerator enumerator) =>
        new Enumerator(enumerator);

    /// <summary>Wraps the enumerator inside an <see cref="IEnumerable{T}"/>.</summary>
    /// <param name="enumerator">The enumerator to encapsulate.</param>
    /// <returns>The <see cref="IEnumerator{T}"/> instance that wraps <paramref name="enumerator"/>.</returns>
    [MustDisposeResource, Pure]
    public static IEnumerable<object?> AsEnumerable([HandlesResourceDisposal] this IEnumerator enumerator) =>
        AsEnumerable(AsGeneric(enumerator));

    /// <summary>Wraps the array inside an <see cref="IEnumerable{T}"/>.</summary>
    /// <param name="array">The array to encapsulate.</param>
    /// <returns>The <see cref="IEnumerator{T}"/> instance that wraps <paramref name="array"/>.</returns>
    [MustDisposeResource, Pure]
    public static IEnumerable<object?> AsGenericEnumerable(this Array array) => AsEnumerable(array.GetEnumerator());

    /// <summary>Wraps the <see cref="IEnumerator{T}"/> inside an <see cref="IEnumerable{T}"/>.</summary>
    /// <typeparam name="T">The type of item to enumerate.</typeparam>
    /// <param name="enumerator">The <see cref="IEnumerator{T}"/> to encapsulate.</param>
    /// <returns>The <see cref="IEnumerator{T}"/> instance that wraps <paramref name="enumerator"/>.</returns>
    [MustDisposeResource, Pure]
    public static IEnumerable<T> AsEnumerable<T>([HandlesResourceDisposal] this IEnumerator<T> enumerator) =>
        new Enumerable<T>(enumerator);

    /// <summary>Converts an <see cref="IStructuralComparable"/> to a <see cref="List{T}"/>.</summary>
    /// <param name="structure">The <see cref="IStructuralComparable"/> to convert.</param>
    /// <returns>The <see cref="List{T}"/> that contains elements from <paramref name="structure"/>.</returns>
    [Pure]
    public static List<object?> ToList(this IStructuralComparable structure)
    {
        ComparerCollector collector = new();
        _ = structure.CompareTo(structure, collector);
        return collector.List;
    }

    /// <summary>Converts an <see cref="IStructuralEquatable"/> to a <see cref="List{T}"/>.</summary>
    /// <param name="structure">The <see cref="IStructuralEquatable"/> to convert.</param>
    /// <returns>The <see cref="List{T}"/> that contains elements from <paramref name="structure"/>.</returns>
    [Pure]
    public static List<object?> ToList(this IStructuralEquatable structure)
    {
        ComparerCollector collector = new();
        _ = structure.Equals(structure, collector);
        return collector.List;
    }
}
