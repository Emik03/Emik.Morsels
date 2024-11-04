// SPDX-License-Identifier: MPL-2.0
#if !NET452_OR_GREATER && !NETSTANDARD1_1 && !NETCOREAPP
namespace InlineIL;

/// <summary>Conversion operator.</summary>
enum ConversionOperator
{
    /// <summary><c>op_Implicit</c></summary>
    Implicit,

    /// <summary><c>op_Explicit</c></summary>
    Explicit,
}
#endif
