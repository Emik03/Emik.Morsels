// SPDX-License-Identifier: MPL-2.0
#if NETFRAMEWORK && !NET461_OR_GREATER || NETSTANDARD && !NETSTANDARD2_0_OR_GREATER || NETCOREAPP && !NETCOREAPP2_0_OR_GREATER
// ReSharper disable once CheckNamespace
namespace InlineMethod;

/// <summary>Resolve delegate parameter.</summary>
[AttributeUsage(AttributeTargets.Parameter)]
partial class ResolveDelegateAttribute : Attribute
{
    /// <summary>Initializes a new instance of the <see cref="ResolveDelegateAttribute"/> class.</summary>
    /// <param name="inline">Inline after resolve.</param>
    public ResolveDelegateAttribute(bool inline = true) => Inline = inline;

    /// <summary>Inline after resolve.</summary>
    public bool Inline { get; }
}
#endif
