// SPDX-License-Identifier: MPL-2.0
#if NETFRAMEWORK && !NET461_OR_GREATER || NETSTANDARD && !NETSTANDARD2_0_OR_GREATER || NETCOREAPP && !NETCOREAPP2_0_OR_GREATER
// ReSharper disable once CheckNamespace
namespace InlineMethod;

/// <summary>Method to inline.</summary>
[AttributeUsage(AttributeTargets.Method)]
partial class InlineAttribute : Attribute
{
    /// <summary>Initializes a new instance of the <see cref="InlineAttribute"/> class.</summary>
    /// <param name="behavior">Export attribute.</param>
    /// <param name="export">InlineMethod behavior.</param>
    public InlineAttribute(InlineBehavior behavior = InlineBehavior.RemovePrivate, bool export = false)
    {
        Behavior = behavior;
        Export = export;
    }

    /// <summary>Export attribute.</summary>
    public bool Export { get; }

    /// <summary>InlineMethod behavior.</summary>
    public InlineBehavior Behavior { get; }
}
#endif
