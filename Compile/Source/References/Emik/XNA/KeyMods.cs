// SPDX-License-Identifier: MPL-2.0
#if XNA // ReSharper disable once CheckNamespace
namespace Emik.Morsels;
#pragma warning disable CS1591, SA1602
/// <summary>Contains the set of all key modifiers.</summary>
[CLSCompliant(false), Flags]
enum KeyMods : ushort
{
    None,
    LeftShift,
    RightShift,
    Shift = RightShift | LeftShift,
    LeftCtrl = 1 << 6,
    RightCtrl = 1 << 7,
    Ctrl = RightCtrl | LeftCtrl,
    LeftAlt = 1 << 8,
    RightAlt = 1 << 9,
    Alt = RightAlt | LeftAlt,
    LeftGui = 1 << 10,
    RightGui = 1 << 11,
    Gui = RightGui | LeftGui,
    NumLock = 1 << 12,
    CapsLock = 1 << 13,
    AltGr = 1 << 14,
    Reserved = 1 << 15,
}
#endif
