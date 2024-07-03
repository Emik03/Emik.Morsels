// SPDX-License-Identifier: MPL-2.0
#if XNA
// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Provides thread-safe access to keyboard input.</summary>
static partial class ButtonExtensions
{
    /// <summary>Converts <see cref="GamePadButtons"/> to <see cref="Buttons"/>.</summary>
    /// <param name="state">The <see cref="GamePadButtons"/> to convert.</param>
    /// <returns>The <see cref="Buttons"/> equivalent.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] // Not `in` because `GamePadButtons` is 4 bytes large.
    public static Buttons AsButtons(this GamePadButtons state) => Unsafe.As<GamePadButtons, Buttons>(ref state);

    /// <summary>Converts <see cref="MouseState"/> to <see cref="MouseButtons"/>.</summary>
    /// <param name="state">The <see cref="MouseState"/> to convert.</param>
    /// <returns>The <see cref="MouseButtons"/> equivalent.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static MouseButtons ToMouseButtons(this in MouseState state) =>
        (state.LeftButton is ButtonState.Pressed ? MouseButtons.Left : MouseButtons.None) |
        (state.MiddleButton is ButtonState.Pressed ? MouseButtons.Middle : MouseButtons.None) |
        (state.RightButton is ButtonState.Pressed ? MouseButtons.Right : MouseButtons.None) |
        (state.XButton1 is ButtonState.Pressed ? MouseButtons.X1 : MouseButtons.None) |
        (state.XButton2 is ButtonState.Pressed ? MouseButtons.X2 : MouseButtons.None);
}
#endif
