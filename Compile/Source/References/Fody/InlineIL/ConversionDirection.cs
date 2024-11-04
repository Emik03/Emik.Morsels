// SPDX-License-Identifier: MPL-2.0
#if !NET452_OR_GREATER && !NETSTANDARD1_1 && !NETCOREAPP
namespace InlineIL;

/// <summary>Conversion direction.</summary>
enum ConversionDirection
{
    /// <summary>Convert from the other type.</summary>
    From,

    /// <summary>Convert to the other type.</summary>
    To,
}
#endif
