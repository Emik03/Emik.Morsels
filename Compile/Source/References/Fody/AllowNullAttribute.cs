// SPDX-License-Identifier: MPL-2.0
// ReSharper disable CheckNamespace ClassNeverInstantiated.Global RedundantUsingDirective.Global
#if !NET452_OR_GREATER && !NETSTANDARD1_4_OR_GREATER && !NETCOREAPP
global using AllowNullAttribute = NullGuard.AllowNullAttribute;

namespace NullGuard;

using static AttributeTargets;

/// <summary>Prevents the injection of null checking (implicit mode only).</summary>
[AttributeUsage(Parameter | ReturnValue | Property)]
sealed partial class AllowNullAttribute : Attribute;
#endif
