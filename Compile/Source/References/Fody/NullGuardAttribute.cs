// SPDX-License-Identifier: MPL-2.0
#if WAWA
#pragma warning disable GlobalUsingsAnalyzer
global using NullGuardAttribute = NullGuard.NullGuardAttribute;

#pragma warning restore GlobalUsingsAnalyzer

// ReSharper disable once CheckNamespace
namespace NullGuard;

using static AttributeTargets;

/// <summary>Prevents the injection of null checking (implicit mode only).</summary>
/// <param name="flags">
/// The <see cref="ValidationFlags"/> to use for the target this attribute is being applied to.
/// </param>
[AttributeUsage(ReturnValue)]
#pragma warning disable 9113
sealed partial class NullGuardAttribute(ValidationFlags flags) : Attribute;
#pragma warning restore 9113
/// <summary>Used by <see cref="NullGuardAttribute"/> to target specific categories of members.</summary>
[Flags]
#pragma warning disable MA0048
public enum ValidationFlags
#pragma warning restore MA0048
#pragma warning disable 1591, SA1602
{
    None = 0,
    Properties = 1,
    Arguments = 2,
    OutValues = 4,
    ReturnValues = 8,
    NonPublic = 16,
    Methods = Arguments | OutValues | ReturnValues,
    AllPublicArguments = Properties | Arguments,
    AllPublic = Properties | Methods,
    All = AllPublic | NonPublic,
#pragma warning restore 1591, SA1602
}
#endif
