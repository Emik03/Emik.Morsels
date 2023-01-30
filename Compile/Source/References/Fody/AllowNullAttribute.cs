// SPDX-License-Identifier: MPL-2.0
#if WAWA
// ReSharper disable once CheckNamespace
namespace NullGuard;

using static AttributeTargets;

/// <summary>Prevents the injection of null checking (implicit mode only).</summary>
[AttributeUsage(Parameter | ReturnValue | Property)]
sealed class AllowNullAttribute : Attribute { }
#endif
