// SPDX-License-Identifier: MPL-2.0
#if NET452
// ReSharper disable once CheckNamespace
namespace RemoveReference;

/// <summary>Ensures that a reference be removed from the compiled binary.</summary>
/// <remarks><para>Declared so as to prevent a hard dependency to RemoveReference.Fody.</para></remarks>
[AttributeUsage(AttributeTargets.Assembly)]
sealed partial class RemoveReferenceAttribute : Attribute
{
    /// <summary>Initializes a new instance of the <see cref="RemoveReferenceAttribute"/> class.</summary>
    /// <param name="fullName">The full name of an assembly to remove its reference.</param>
    public RemoveReferenceAttribute(string fullName) => FullName = fullName;

    /// <summary>Gets an assembly's full name which has been stripped from this compiled binary.</summary>
    public string FullName { get; }
}
#endif
