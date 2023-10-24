// SPDX-License-Identifier: MPL-2.0
#if XNA
// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Provides thread-safe access to keyboard input.</summary>
public static class ConcurrentKeyboard // ReSharper disable InconsistentNaming
{
    [ThreadStatic]
    static int t_last;

    [ThreadStatic]
    static Keys[]? t_copy;

    [ThreadStatic]
    static List<Keys>? t_orig; // ReSharper restore InconsistentNaming

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
        if (t_copy is null)
        {
            t_copy = new Keys[Length];
            t_orig = typeof(Keyboard).GetField(Name, Flags)?.GetValue(null) as List<Keys>;
        }

        // Must be its own variable, as the backing array can change at any moment.
        var original = CollectionsMarshal.AsSpan(t_orig);
        original.CopyTo(t_copy);

        // Clear only what hasn't been overriden by the previous copy.
        if (original.Length < t_last)
            t_copy.AsSpan(original.Length..t_last).Clear();

        t_last = original.Length;
        return new(t_copy);
    }
}
#endif
