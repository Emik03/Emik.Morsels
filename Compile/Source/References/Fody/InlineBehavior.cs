// SPDX-License-Identifier: MPL-2.0
#if NETFRAMEWORK && !NET461_OR_GREATER || NETSTANDARD && !NETSTANDARD2_0_OR_GREATER || NETCOREAPP && !NETCOREAPP2_0_OR_GREATER
// ReSharper disable once CheckNamespace
namespace InlineMethod;

/// <summary>InlineMethod behavior.</summary>
enum InlineBehavior
{
    /// <summary>Keep method after inline.</summary>
    Keep,

    /// <summary>Remove method after inline if private.</summary>
    RemovePrivate,

    /// <summary>Remove method after inline.</summary>
    Remove,
}
#endif
