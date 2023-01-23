// SPDX-License-Identifier: MPL-2.0
#if NETFRAMEWORK && !NET48 || NETSTANDARD && !NETSTANDARD2_0_OR_GREATER
namespace InlineMethod;

using static AttributeTargets;

/// <summary>Method to inline.</summary>
[AttributeUsage(Method)]
sealed partial class InlineAttribute : Attribute
{
    /// <summary>Initializes a new instance of the <see cref="InlineAttribute"/> class.</summary>
    /// <param name="remove">The value to set.</param>
    public InlineAttribute(bool remove = true) => Remove = remove;

    /// <summary>Gets a value indicating whether to remove the method after inlining, if private.</summary>
    public bool Remove { get; }
}
#endif
