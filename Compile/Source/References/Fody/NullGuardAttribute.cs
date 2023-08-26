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
[AttributeUsage(Assembly | Class)]
sealed partial class NullGuardAttribute(ValidationFlags flags) : Attribute;

/// <summary>Used by <see cref="NullGuardAttribute"/> to target specific categories of members.</summary>
[Flags]
enum ValidationFlags
{
    /// <summary>Does nothing.</summary>
    None = 0,

    /// <summary>
    /// Adds null guard checks to properties getter (cannot return null) and setter (cannot be set to null).
    /// </summary>
    Properties = 1,

    /// <summary>
    /// Method arguments are checked to make sure they are not null. This only
    /// applies to normal arguments, and the incoming value of a ref argument.
    /// </summary>
    Arguments = 2,

    /// <summary>Processes all methods (arguments and return values) and properties.</summary>
    AllPublicArguments = Properties | Arguments,

    /// <summary>Out and ref arguments of a method are checked for null just before the method returns.</summary>
    OutValues = 4,

    /// <summary>Checks the return value of a method for null.</summary>
    ReturnValues = 8,

    /// <summary>Processes all arguments (normal, out and ref) and return values of methods.</summary>
    Methods = Arguments | OutValues | ReturnValues,

    /// <summary>Checks everything (properties, all method args and return values).</summary>
    AllPublic = Properties | Methods,

    /// <summary>Applies the other flags to all non-public members as well.</summary>
    NonPublic = 16,

    /// <summary>Wildcard.</summary>
    All = AllPublic | NonPublic,
}
#endif
