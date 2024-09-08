// SPDX-License-Identifier: MPL-2.0
#if XNA
// ReSharper disable BitwiseOperatorOnEnumWithoutFlags InconsistentNaming NullableWarningSuppressionIsUsed
// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Provides thread-safe access to keyboard input.</summary>
static partial class ConcurrentKeyboard
{
#pragma warning disable CA1810
    static ConcurrentKeyboard()
#pragma warning restore CA1810
    {
        Trace.Assert(Unsafe.SizeOf<Keys>() is sizeof(int), $"sizeof({nameof(Keys)}) is 4");
        Trace.Assert(Unsafe.SizeOf<KeyMods>() is sizeof(ushort), $"sizeof({nameof(KeyMods)}) is 2");
        Trace.Assert(Unsafe.SizeOf<KeyboardState>() >= sizeof(uint) * 8 + sizeof(byte), "Memory layout is known.");

        Trace.Assert(TryGetType(out var type), $"{nameof(type)} is not null");
        Trace.Assert(TryGetField(type, out var delegateField), $"{nameof(delegateField)} is not null");
        Trace.Assert(TryGetValue(delegateField, out Delegate? del), $"{nameof(del)} is not null");
        s_modState = CompileModState(del);

        Trace.Assert(TryGetField(out var keyField), $"{nameof(keyField)} is not null");
        Trace.Assert(TryGetValue(keyField, out List<Keys>? keys), $"{nameof(keys)} is not null");
        s_keys = keys;

        Trace.Assert(TryFindInvalidState(out var invalid), $"{nameof(ToState)} breaks on {invalid}");
    }

    static readonly Func<KeyMods> s_modState;

    static readonly List<Keys> s_keys;

    /// <summary>Thread-safe version of <see cref="Keyboard.GetState()"/>.</summary>
    /// <returns>The current <see cref="KeyboardState"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static KeyboardState GetState() => s_keys.AsSpan().ToState(GetModState());

    /// <summary>
    /// Converts the <see cref="ReadOnlySpan{T}"/> of <see cref="Keys"/> into the summed <see cref="KeyboardState"/>.
    /// </summary>
    /// <remarks><para>
    /// This operation treats the provided <see cref="ReadOnlySpan{T}"/> of <see cref="Keys"/> as a set for computation,
    /// meaning that repeated <see cref="Keys"/> of the same value have the same effect as if it appeared once.
    /// </para></remarks>
    /// <param name="keys">The <see cref="ReadOnlySpan{T}"/> of <see cref="Keys"/> to process.</param>
    /// <param name="mods">The <see cref="KeyMods"/> for modifiers.</param>
    /// <returns>
    /// The <see cref="KeyboardState"/> that comes from both parameters
    /// <paramref name="keys"/> and <paramref name="mods"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static KeyboardState ToState(this scoped in ReadOnlySpan<Keys> keys, KeyMods mods = KeyMods.None)
    {
        var bits = (ushort)mods;
        KeyboardState output = default;
        var reader = MemoryMarshal.Cast<Keys, int>(keys);
        ref var start = ref MemoryMarshal.GetReference(reader);
        ref var end = ref Unsafe.Add(ref start, reader.Length);
        ref var writer = ref Unsafe.As<KeyboardState, uint>(ref output);

        while (Unsafe.IsAddressLessThan(ref start, ref end))
            Unsafe.Add(ref writer, start >> 5 & 7) |= 1u << (start & 31);

        Unsafe.As<uint, byte>(ref Unsafe.Add(ref writer, 8)) = (byte)((bits & 4096) >> 11 | (bits & 8192) >> 13);
        return output;
    }

    /// <summary>
    /// Converts the <see cref="Span{T}"/> of <see cref="Keys"/> into the summed <see cref="KeyboardState"/>.
    /// </summary>
    /// <remarks><para>
    /// This operation treats the provided <see cref="Span{T}"/> of <see cref="Keys"/> as a set for computation,
    /// meaning that repeated <see cref="Keys"/> of the same value have the same effect as if it appeared once.
    /// </para></remarks>
    /// <param name="keys">The <see cref="ReadOnlySpan{T}"/> of <see cref="Keys"/> to process.</param>
    /// <param name="mods">The <see cref="KeyMods"/> for modifiers.</param>
    /// <returns>
    /// The <see cref="KeyboardState"/> that comes from both parameters
    /// <paramref name="keys"/> and <paramref name="mods"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static KeyboardState ToState(this scoped in Span<Keys> keys, KeyMods mods = KeyMods.None) =>
        keys.ReadOnly().ToState(mods);

    /// <summary>Gets the current set of key modifiers that are active.</summary>
    /// <returns>The <see cref="KeyMods"/> representing the current modifiers active.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static KeyMods GetModState() => s_modState();

    [Pure]
    static bool TryFindInvalidState([NotNullWhen(false)] out Enum? invalid)
    {
        static bool IsModifierCausingInvalidState(KeyMods mod) =>
            ((ReadOnlySpan<Keys>)[]).ToState(mod) is { CapsLock: var capsLock, NumLock: var numLock } state &&
            capsLock != mod is KeyMods.CapsLock ||
            numLock != mod is KeyMods.NumLock ||
            state.GetPressedKeyCount() is not 0;

        static bool IsKeyCausingInvalidState(Keys key) =>
            Span.In(key).ToState() is not { CapsLock: false, NumLock: false } state ||
            state.IsKeyUp(key) ||
            state.GetPressedKeyCount() is not 1;

        var keyModTests = EnumMath.GetValues<KeyMods>().Where(IsModifierCausingInvalidState).Cast<Enum>();
        var keyTests = EnumMath.GetValues<Keys>().Where(IsKeyCausingInvalidState).Cast<Enum>();
        invalid = keyModTests.Concat(keyTests).Filter().FirstOrDefault();
        return invalid is null;
    }

    [MustUseReturnValue]
    static bool TryGetField([NotNullWhen(true)] out FieldInfo? field) =>
        (field = typeof(Keyboard).GetField("_keys", BindingFlags.NonPublic | BindingFlags.Static)) is not null;

    [MustUseReturnValue]
    static bool TryGetField(in Type type, [NotNullWhen(true)] out FieldInfo? field) =>
        (field = type.GetField(nameof(GetModState), BindingFlags.Public | BindingFlags.Static)) is not null;

    [MustUseReturnValue]
    static bool TryGetType([NotNullWhen(true)] out Type? type) =>
#pragma warning disable REFL037
        (type = typeof(Keyboard).Assembly.GetType("Sdl+Keyboard")) is not null;
#pragma warning restore REFL037
    [MustUseReturnValue]
    static bool TryGetValue(in FieldInfo delegateField, [NotNullWhen(true)] out Delegate? del) =>
        (del = delegateField.GetValue(null) as Delegate) is not null;

    [MustUseReturnValue]
    static bool TryGetValue(in FieldInfo field, [NotNullWhen(true)] out List<Keys>? keys) =>
        (keys = field.GetValue(null) as List<Keys>) is not null;

    [Pure]
    static Func<KeyMods> CompileModState(in Delegate del)
    {
        var constant = Expression.Constant(del);
        var invoke = Expression.Invoke(constant);
        var mods = Expression.Convert(invoke, typeof(KeyMods));
        return Expression.Lambda<Func<KeyMods>>(mods).Compile();
    }
}
#endif
