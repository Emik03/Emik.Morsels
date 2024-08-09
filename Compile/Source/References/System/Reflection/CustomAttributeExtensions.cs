// SPDX-License-Identifier: MPL-2.0
#pragma warning disable REFL010

// ReSharper disable once CheckNamespace EmptyNamespace
namespace System.Reflection;
#if NETFRAMEWORK && !NET45_OR_GREATER
/// <summary>Contains static methods for retrieving custom attributes.</summary>
static partial class CustomAttributeExtensions
{
    public static bool IsDefined(this Assembly element, Type attributeType) =>
        Attribute.IsDefined(element, attributeType);

    public static bool IsDefined(this Module element, Type attributeType) =>
        Attribute.IsDefined(element, attributeType);

    public static bool IsDefined(this MemberInfo element, Type attributeType) =>
        Attribute.IsDefined(element, attributeType);

    public static bool IsDefined(this ParameterInfo element, Type attributeType) =>
        Attribute.IsDefined(element, attributeType);

    public static bool IsDefined(this MemberInfo element, Type attributeType, bool inherit) =>
        Attribute.IsDefined(element, attributeType, inherit);

    public static bool IsDefined(this ParameterInfo element, Type attributeType, bool inherit) =>
        Attribute.IsDefined(element, attributeType, inherit);

    public static Attribute? GetCustomAttribute(this Assembly element, Type attributeType) =>
        Attribute.GetCustomAttribute(element, attributeType);

    public static Attribute? GetCustomAttribute(this Module element, Type attributeType) =>
        Attribute.GetCustomAttribute(element, attributeType);

    public static Attribute? GetCustomAttribute(this MemberInfo element, Type attributeType) =>
        Attribute.GetCustomAttribute(element, attributeType);

    public static Attribute? GetCustomAttribute(this ParameterInfo element, Type attributeType) =>
        Attribute.GetCustomAttribute(element, attributeType);

    public static Attribute? GetCustomAttribute(this MemberInfo element, Type attributeType, bool inherit) =>
        Attribute.GetCustomAttribute(element, attributeType, inherit);

    public static Attribute? GetCustomAttribute(this ParameterInfo element, Type attributeType, bool inherit) =>
        Attribute.GetCustomAttribute(element, attributeType, inherit);

    public static IEnumerable<Attribute> GetCustomAttributes(this Assembly element) =>
        Attribute.GetCustomAttributes(element);

    public static IEnumerable<Attribute> GetCustomAttributes(this Module element) =>
        Attribute.GetCustomAttributes(element);

    public static IEnumerable<Attribute> GetCustomAttributes(this MemberInfo element) =>
        Attribute.GetCustomAttributes(element);

    public static IEnumerable<Attribute> GetCustomAttributes(this ParameterInfo element) =>
        Attribute.GetCustomAttributes(element);

    public static IEnumerable<Attribute> GetCustomAttributes(this MemberInfo element, bool inherit) =>
        Attribute.GetCustomAttributes(element, inherit);

    public static IEnumerable<Attribute> GetCustomAttributes(this ParameterInfo element, bool inherit) =>
        Attribute.GetCustomAttributes(element, inherit);

    public static IEnumerable<Attribute> GetCustomAttributes(this Assembly element, Type attributeType) =>
        Attribute.GetCustomAttributes(element, attributeType);

    public static IEnumerable<Attribute> GetCustomAttributes(this Module element, Type attributeType) =>
        Attribute.GetCustomAttributes(element, attributeType);

    public static IEnumerable<Attribute> GetCustomAttributes(this MemberInfo element, Type attributeType) =>
        Attribute.GetCustomAttributes(element, attributeType);

    public static IEnumerable<Attribute> GetCustomAttributes(this ParameterInfo element, Type attributeType) =>
        Attribute.GetCustomAttributes(element, attributeType);

    public static IEnumerable<Attribute> GetCustomAttributes(
        this MemberInfo element,
        Type attributeType,
        bool inherit
    ) =>
        Attribute.GetCustomAttributes(element, attributeType, inherit);

    public static IEnumerable<Attribute> GetCustomAttributes(
        this ParameterInfo element,
        Type attributeType,
        bool inherit
    ) =>
        Attribute.GetCustomAttributes(element, attributeType, inherit);

    public static IEnumerable<T> GetCustomAttributes<T>(this Assembly element)
        where T : Attribute =>
        GetCustomAttributes(element, typeof(T)).Cast<T>();

    public static IEnumerable<T> GetCustomAttributes<T>(this Module element)
        where T : Attribute =>
        GetCustomAttributes(element, typeof(T)).Cast<T>();

    public static IEnumerable<T> GetCustomAttributes<T>(this MemberInfo element)
        where T : Attribute =>
        GetCustomAttributes(element, typeof(T)).Cast<T>();

    public static IEnumerable<T> GetCustomAttributes<T>(this ParameterInfo element)
        where T : Attribute =>
        GetCustomAttributes(element, typeof(T)).Cast<T>();

    public static IEnumerable<T> GetCustomAttributes<T>(this MemberInfo element, bool inherit)
        where T : Attribute =>
        GetCustomAttributes(element, typeof(T), inherit).Cast<T>();

    public static IEnumerable<T> GetCustomAttributes<T>(this ParameterInfo element, bool inherit)
        where T : Attribute =>
        GetCustomAttributes(element, typeof(T), inherit).Cast<T>();

    public static T? GetCustomAttribute<T>(this Assembly element)
        where T : Attribute =>
        (T?)GetCustomAttribute(element, typeof(T));

    public static T? GetCustomAttribute<T>(this Module element)
        where T : Attribute =>
        (T?)GetCustomAttribute(element, typeof(T));

    public static T? GetCustomAttribute<T>(this MemberInfo element)
        where T : Attribute =>
        (T?)GetCustomAttribute(element, typeof(T));

    public static T? GetCustomAttribute<T>(this ParameterInfo element)
        where T : Attribute =>
        (T?)GetCustomAttribute(element, typeof(T));

    public static T? GetCustomAttribute<T>(this MemberInfo element, bool inherit)
        where T : Attribute =>
        (T?)GetCustomAttribute(element, typeof(T), inherit);

    public static T? GetCustomAttribute<T>(this ParameterInfo element, bool inherit)
        where T : Attribute =>
        (T?)GetCustomAttribute(element, typeof(T), inherit);
}
#endif
