// SPDX-License-Identifier: MPL-2.0
#pragma warning disable CS9113, GlobalUsingsAnalyzer
// ReSharper disable ClassNeverInstantiated.Global RedundantUsingDirective.Global
#if !NET452_OR_GREATER && !NETSTANDARD1_4_OR_GREATER && !NETCOREAPP
global using NullGuardAttribute = NullGuard.NullGuardAttribute;

// ReSharper disable once CheckNamespace
namespace NullGuard;

using static AttributeTargets;

/// <summary>Prevents the injection of null checking (implicit mode only).</summary>
/// <param name="flags">
/// The <see cref="ValidationFlags"/> to use for the target this attribute is being applied to.
/// </param>
[AttributeUsage(Assembly | Class)]
sealed partial class NullGuardAttribute(ValidationFlags flags) : Attribute;
#endif
