// SPDX-License-Identifier: MPL-2.0
#if NETFRAMEWORK && !NET45_OR_GREATER
// ReSharper disable once CheckNamespace
namespace Virtuosity;

using static AttributeTargets;

/// <summary>Used to exclude a class form virtualization.</summary>
[AttributeUsage(Class)]
sealed partial class DoNotVirtualizeAttribute : Attribute;

#endif
