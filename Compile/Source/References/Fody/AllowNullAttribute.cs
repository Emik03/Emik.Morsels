// SPDX-License-Identifier: MPL-2.0
#if WAWA
namespace NullGuard;

using static AttributeTargets;

/// <summary>Prevents the injection of null checking (implicit mode only).</summary>
[AttributeUsage(Parameter | ReturnValue | Property)]
sealed class AllowNullAttribute : Attribute { }
#endif
