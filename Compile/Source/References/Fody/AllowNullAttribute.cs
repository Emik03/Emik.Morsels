// SPDX-License-Identifier: MPL-2.0
#if WAWA
#pragma warning disable GlobalUsingsAnalyzer
global using AllowNullAttribute = NullGuard.AllowNullAttribute;

#pragma warning restore GlobalUsingsAnalyzer

// ReSharper disable once CheckNamespace
namespace NullGuard;

using static AttributeTargets;

/// <summary>Prevents the injection of null checking (implicit mode only).</summary>
[AttributeUsage(Parameter | ReturnValue | Property)]
sealed partial class AllowNullAttribute : Attribute { }
#endif
