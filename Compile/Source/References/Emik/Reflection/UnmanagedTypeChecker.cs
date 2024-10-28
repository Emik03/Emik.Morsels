// SPDX-License-Identifier: MPL-2.0
#if NETFRAMEWORK || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
// ReSharper disable CheckNamespace
namespace Emik.Morsels;

/// <summary>Provides methods to check for unmanaged types.</summary>
static partial class UnmanagedTypeChecker
{
    static readonly Dictionary<Type, bool> s_fullyUnmanaged = [];

    /// <summary>Gets the type name, with its generics extended.</summary>
    /// <param name="type">The <see cref="Type"/> to get the name of.</param>
    /// <returns>The name of the parameter <paramref name="type"/>.</returns>
    [Pure]
    public static bool IsUnmanaged([NotNullWhen(true)] this Type? type) =>
        type is not null &&
        (s_fullyUnmanaged.TryGetValue(type, out var answer) ? answer :
            !type.IsValueType ? s_fullyUnmanaged[type] = false :
            type.IsEnum || type.IsPointer || type.IsPrimitive ? s_fullyUnmanaged[type] = true :
            s_fullyUnmanaged[type] = type.IsGenericTypeDefinition
                ? type
                   .GetCustomAttributes()
                   .Any(x => x?.GetType().FullName is "System.Runtime.CompilerServices.IsUnmanagedAttribute")
                : Array.TrueForAll(
                    type.GetFields(
                        BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                    ),
                    x => IsUnmanaged(x.FieldType)
                ));
}
#endif
