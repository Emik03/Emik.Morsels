// SPDX-License-Identifier: MPL-2.0
#pragma warning disable GlobalUsingsAnalyzer
// ReSharper disable CheckNamespace ClassNeverInstantiated.Global RedundantUsingDirective.Global
#if !NET452_OR_GREATER && !NETSTANDARD1_4_OR_GREATER && !NETCOREAPP
global using MaybeNullTaskResultAttribute = NullGuard.MaybeNullTaskResultAttribute;

namespace NullGuard;

using static AttributeTargets;

/// <summary>Prevents the injection of null checking (implicit mode only).</summary>
[AttributeUsage(ReturnValue)]
sealed partial class MaybeNullTaskResultAttribute : Attribute;
#endif
