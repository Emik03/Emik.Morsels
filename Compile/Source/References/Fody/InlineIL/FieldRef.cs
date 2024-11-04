// SPDX-License-Identifier: MPL-2.0
#if !NET452_OR_GREATER && !NETSTANDARD1_1 && !NETCOREAPP
namespace InlineIL;

/// <summary>Represents a field reference.</summary>
sealed class FieldRef
{
    /// <summary>
    /// Constructs a field reference.
    /// </summary>
    /// <param name="type">The field type.</param>
    /// <param name="fieldName">The field name.</param>
    // ReSharper disable UnusedParameter.Local
    public FieldRef(TypeRef type, string fieldName) => IL.Throw();

    /// <summary>
    /// Constructs a field reference.
    /// </summary>
    /// <param name="type">The field type.</param>
    /// <param name="fieldName">The field name.</param>
    public static FieldRef Field(TypeRef type, string fieldName) => throw IL.Throw();
}
#endif
