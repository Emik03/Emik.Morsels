// SPDX-License-Identifier: MPL-2.0
#if !NET8_0_OR_GREATER
// ReSharper disable CheckNamespace RedundantUnsafeContext
namespace System;
#pragma warning disable 8500
/// <summary>The backport of GetItems methods for <see cref="Random"/>.</summary>
static partial class RandomGetItems
{
    /// <summary>
    /// Fills the elements of a specified span with items chosen at random from the provided set of choices.
    /// </summary>
    /// <remarks><para>
    /// The method uses <see cref="Random.Next(int)"/> to select items randomly from <paramref name="choices"/>
    /// by index and populate <paramref name="destination"/>.
    /// </para></remarks>
    /// <typeparam name="T">The type of span.</typeparam>
    /// <param name="that">The instance of <see cref="Random"/>.</param>
    /// <param name="choices">The items to use to populate the span.</param>
    /// <param name="destination">The span to be filled with items.</param>
    /// <exception cref="InvalidOperationException"><paramref name="choices"/> is empty.</exception>
    public static void GetItems<T>(this Random that, ReadOnlySpan<T> choices, Span<T> destination)
    {
        if (choices.IsEmpty)
            throw CannotBeEmpty;

        for (var i = 0; i < destination.Length; i++)
            destination[i] = choices[that.Next(choices.Length)];
    }

    /// <summary>Creates an array populated with items chosen at random from the provided set of choices.</summary>
    /// <remarks><para>
    /// The method uses <see cref="Random.Next(int)"/> to select items randomly from <paramref name="choices"/>
    /// by index. This is used to populate a newly-created array.
    /// </para></remarks>
    /// <typeparam name="T">The type of array.</typeparam>
    /// <param name="that">The instance of <see cref="Random"/>.</param>
    /// <param name="choices">The items to use to populate the array.</param>
    /// <param name="length">The length of array to return.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="length"/> is not zero or a positive number.
    /// </exception>
    /// <exception cref="InvalidOperationException"><paramref name="choices"/> is empty.</exception>
    /// <returns>An array populated with random items.</returns>
    public static unsafe T[] GetItems<T>(this Random that, T[] choices, [NonNegativeValue] int length) =>
        GetItems(that, new ReadOnlySpan<T>(choices), length);

    /// <summary>Creates an array populated with items chosen at random from the provided set of choices.</summary>
    /// <remarks><para>
    /// The method uses <see cref="Random.Next(int)"/> to select items randomly from <paramref name="choices"/>
    /// by index. This is used to populate a newly-created array.
    /// </para></remarks>
    /// <typeparam name="T">The type of array.</typeparam>
    /// <param name="that">The instance of <see cref="Random"/>.</param>
    /// <param name="choices">The items to use to populate the array.</param>
    /// <param name="length">The length of array to return.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="length"/> is not zero or a positive number.
    /// </exception>
    /// <exception cref="InvalidOperationException"><paramref name="choices"/> is empty.</exception>
    /// <returns>An array populated with random items.</returns>
    public static unsafe T[] GetItems<T>(this Random that, ReadOnlySpan<T> choices, [NonNegativeValue] int length)
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        if (length < 0)
            throw new ArgumentOutOfRangeException(nameof(length), length, "Must not be negative.");

        var items = new T[length];
        GetItems(that, choices, items.AsSpan());
        return items;
    }
}
#endif
