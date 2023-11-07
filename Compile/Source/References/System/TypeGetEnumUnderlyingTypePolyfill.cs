// SPDX-License-Identifier: MPL-2.0
#if NETFRAMEWORK && !NET40_OR_GREATER
namespace System;

using static BindingFlags;

/// <summary>Provides the polyfill to <c>Type.GetEnumUnderlyingType</c>.</summary>
[Obsolete("This class shouldn't be referred to directly, as only the extension method is guaranteed.", true)]
static class TypeGetEnumUnderlyingTypePolyfill
{
    /// <summary>Returns the underlying type of the specified enumeration.</summary>
    /// <remarks><para>
    /// The <see cref="Enum"/> structure enables values to be represented as named constants.
    /// The data type of the enumeration's values is known as its underlying type. For example, the underlying type
    /// of the <see cref="DayOfWeek"/> enumeration, which consists of constants that represent each day of the week
    /// (<see cref="DayOfWeek.Monday"/>, <see cref="DayOfWeek.Tuesday"/>, and so on), is <see cref="int"/>.
    /// </para></remarks>
    /// <param name="enumType">The enumeration whose underlying type will be retrieved.</param>
    /// <exception cref="ArgumentException"><paramref name="enumType"/> is not an <see cref="Enum"/>.</exception>
    /// <returns>The underlying type of <see cref="enumType"/>.</returns>
    public static Type GetEnumUnderlyingType(this Type enumType) =>
        enumType.IsEnum
            ? enumType.GetFields(Public | NonPublic | Instance)[0].FieldType
            : throw new ArgumentException($"{nameof(Type)} provided must be an {nameof(Enum)}.", nameof(enumType));
}
#endif
