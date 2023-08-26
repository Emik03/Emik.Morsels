// SPDX-License-Identifier: MPL-2.0
#if WAWA
#pragma warning disable GlobalUsingsAnalyzer
global using MaybeNullTaskResultAttribute = NullGuard.MaybeNullTaskResultAttribute;

#pragma warning restore GlobalUsingsAnalyzer

// ReSharper disable once CheckNamespace
namespace NullGuard;

using static AttributeTargets;

/// <summary>Prevents the injection of null checking (implicit mode only).</summary>
[AttributeUsage(ReturnValue)]
sealed partial class MaybeNullTaskResultAttribute : Attribute;
#endif
