// SPDX-License-Identifier: MPL-2.0
#if XNA
// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Provides thread-safe access to keyboard input.</summary>
public static class ConcurrentKeyboard
{
    [ThreadStatic]
    static int s_last;

    [ThreadStatic]
    static Keys[]? s_copy;

    [ThreadStatic]
    static List<Keys>? s_original;

    /// <summary>Thread-safe version of <see cref="Keyboard.GetState()"/>.</summary>
    /// <remarks><para>
    /// Due to limitations, values returned from this method have <see cref="KeyboardState.CapsLock"/>
    /// and <see cref="KeyboardState.NumLock"/> always set to <see langword="false"/>.
    /// </para><para>
    /// Use <see cref="Keyboard.GetState()"/> instead if this is needed.
    /// </para></remarks>
    /// <returns>The current <see cref="KeyboardState"/>.</returns>
    public static KeyboardState GetState()
    {
        const int Length = 256;
        const string Name = "_keys";
        const BindingFlags Flags = BindingFlags.NonPublic | BindingFlags.Static;

        // This initialization runs only once per thread.
        if (s_copy is null)
        {
            s_copy = new Keys[Length];
            s_original = typeof(Keyboard).GetField(Name, Flags)?.GetValue(null) as List<Keys>;
        }

        // Must be its own variable, as the backing array can change at any moment.
        var original = CollectionsMarshal.AsSpan(s_original);
        original.CopyTo(s_copy);

        // Clear only what hasn't been overriden by the previous copy.
        if (original.Length < s_last)
            s_copy.AsSpan(original.Length..s_last).Clear();

        s_last = original.Length;
        return new(s_copy);
    }
}
#endif
