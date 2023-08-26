// SPDX-License-Identifier: MPL-2.0
#pragma warning disable 1591, 9113, GlobalUsingsAnalyzer, MA0048, SA1602
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
[AttributeUsage(ReturnValue)]
sealed partial class NullGuardAttribute(ValidationFlags flags) : Attribute;

/// <summary>Used by <see cref="NullGuardAttribute"/> to target specific categories of members.</summary>
[Flags]
public enum ValidationFlags
{
    None = 0,
    Properties = 1,
    Arguments = 2,
    AllPublicArguments = Properties | Arguments,
    OutValues = 4,
    ReturnValues = 8,
    Methods = Arguments | OutValues | ReturnValues,
    AllPublic = Properties | Methods,
    NonPublic = 16,
    All = AllPublic | NonPublic,
}
#endif
