// SPDX-License-Identifier: MPL-2.0
// ReSharper disable once CheckNamespace EmptyNamespace
namespace Emik.Morsels;
#if XNA
/// <summary>Contains mouse buttons.</summary>
[Flags]
enum MouseButtons : byte
{
    /// <summary>No mouse button.</summary>
    None,

    /// <summary>Left mouse button.</summary>
    Left,

    /// <summary>Right mouse button.</summary>
    Right,

    /// <summary>Middle mouse button.</summary>
    Middle = 1 << 2,

    /// <summary>X1 mouse button.</summary>
    X1 = 1 << 3,

    /// <summary>X2 mouse button.</summary>
    X2 = 1 << 4,
}
#endif
