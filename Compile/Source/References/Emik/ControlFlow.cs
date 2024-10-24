// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Determines control flow for loops.</summary>
enum ControlFlow : byte
{
    /// <summary>The value indicating that the loop should continue.</summary>
    Continue,

    /// <summary>The value indicating that the loop should break.</summary>
    Break,
}
