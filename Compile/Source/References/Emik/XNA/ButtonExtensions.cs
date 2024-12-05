// SPDX-License-Identifier: MPL-2.0
#if XNA // ReSharper disable once CheckNamespace
namespace Emik.Morsels;

using static Span;

/// <summary>Provides thread-safe access to keyboard input.</summary>
static partial class ButtonExtensions
{
    /// <summary>Converts <see cref="GamePadButtons"/> to <see cref="Buttons"/>.</summary>
    /// <param name="state">The <see cref="GamePadButtons"/> to convert.</param>
    /// <returns>The <see cref="Buttons"/> equivalent.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure,
     UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_buttons")]
    public static extern ref readonly Buttons AsButtons(this in GamePadButtons state);

    /// <summary>Converts <see cref="MouseState"/> to <see cref="MouseButtons"/>.</summary>
    /// <param name="state">The <see cref="MouseState"/> to convert.</param>
    /// <returns>The <see cref="MouseButtons"/> equivalent.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static ref readonly MouseButtons ToMouseButtons(this in MouseState state)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure,
         UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_buttons")]
        static extern ref readonly byte Buttons(in MouseState state);

        return ref Unsafe.As<byte, MouseButtons>(ref AsRef(Buttons(in state)));
    }
}
#endif
