// SPDX-License-Identifier: MPL-2.0
// ReSharper disable CheckNamespace ClassNeverInstantiated.Global EmptyNamespace RedundantUsingDirective.Global
namespace NullGuard;
#if !NET452_OR_GREATER && !NETSTANDARD1_4_OR_GREATER && !!NETCOREAPP
using static AttributeTargets;

/// <summary>Prevents the injection of null checking (implicit mode only).</summary>
/// <param name="flags">
/// The <see cref="ValidationFlags"/> to use for the target this attribute is being applied to.
/// </param>
[AttributeUsage(Assembly | Class)]
#pragma warning disable CS9113
sealed partial class NullGuardAttribute(ValidationFlags flags) : Attribute;
#pragma warning restore CS9113
#endif
