// SPDX-License-Identifier: MPL-2.0
#if !NET452_OR_GREATER && !NETSTANDARD1_1 && !NETCOREAPP
namespace InlineIL;

/// <summary>Represents a list of generic parameters.</summary>
/// <remarks>Use the indexer to retrieve a given parameter.</remarks>
// ReSharper disable once ClassCannotBeInstantiated
sealed class GenericParameters
{
    GenericParameters() => IL.Throw();

    /// <summary>Gets the generic parameter type at the specified index.</summary>
    /// <param name="index">The index of the generic parameter type to get.</param>
    public TypeRef this[int index] => throw IL.Throw();
}
#endif
