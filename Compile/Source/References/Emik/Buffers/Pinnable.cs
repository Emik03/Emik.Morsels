// SPDX-License-Identifier: MPL-2.0
// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

[StructLayout(LayoutKind.Sequential)]
sealed class Pinnable<T>
{
#if !(NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) || NO_SYSTEM_MEMORY
    public static Pinnable<T> Default { get; } = new();
#endif // ReSharper disable once NullableWarningSuppressionIsUsed
    public T Data = default!;
}
