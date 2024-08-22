// SPDX-License-Identifier: MPL-2.0
#if !NETSTANDARD || NETSTANDARD2_0_OR_GREATER
// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Allows you to get an attribute on an enum field value.</summary>
static partial class AttributeFromEnumFieldValue
{
    /// <summary>Gets the <typeparamref name="T"/> applied to the field of the enum.</summary>
    /// <typeparam name="T">The type of <see cref="Attribute"/> to get.</typeparam>
    /// <param name="value">The enum containing the <typeparamref name="T"/> instance in metadata.</param>
    /// <returns>
    /// The <typeparamref name="T"/> instance attached to the parameter <paramref name="value"/>'s field metadata.
    /// </returns>
    public static T? GetCustomAttribute<T>(this Enum value)
        where T : Attribute =>
        value.GetType().GetMember($"{value}", BindingFlags.Static | BindingFlags.Public)[0].GetCustomAttribute<T>();
}
#endif
