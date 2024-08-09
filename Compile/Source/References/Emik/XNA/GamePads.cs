// SPDX-License-Identifier: MPL-2.0
#if XNA
#pragma warning disable GlobalUsingsAnalyzer
// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Provides the enumeration over <see cref="GamePad"/> instances.</summary>
[StructLayout(LayoutKind.Auto)]
struct GamePads(PlayerIndex last = PlayerIndex.Four) : IEnumerable<GamePadState>, IEnumerator<GamePadState>
{
    readonly PlayerIndex _length = last + 1;

    PlayerIndex _index;

    /// <summary>Gets the first four <see cref="GamePadState"/> instances.</summary>
    public static (GamePadState First, GamePadState Second, GamePadState Third, GamePadState Fourth) Four =>
    (
        GamePad.GetState(PlayerIndex.One),
        GamePad.GetState(PlayerIndex.Two),
        GamePad.GetState(PlayerIndex.Three),
        GamePad.GetState(PlayerIndex.Four)
    );

    /// <inheritdoc />
    public GamePadState Current { get; private set; }

    /// <inheritdoc />
    readonly object IEnumerator.Current => Current;

    /// <inheritdoc />
    readonly void IDisposable.Dispose() { }

    /// <inheritdoc />
    public void Reset() => _index = PlayerIndex.One;

    /// <inheritdoc />
    public bool MoveNext() =>
#pragma warning disable MA0099
        _index < (_length is 0 ? PlayerIndex.Four + 1 : _length) && (Current = GamePad.GetState(_index++)) is var _;
#pragma warning restore MA0099
    /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
    public readonly GamePads GetEnumerator() => this;

    /// <inheritdoc />
    readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    readonly IEnumerator<GamePadState> IEnumerable<GamePadState>.GetEnumerator() => GetEnumerator();
}

/// <summary>Extensions for <see cref="GamePadState"/>.</summary>
#pragma warning disable MA0048
static class GamePadStateExtensions
#pragma warning restore MA0048
{
    /// <inheritdoc cref="GamePadState.IsConnected"/>
    public static bool IsConnected(this in (GamePadState, GamePadState, GamePadState, GamePadState) state) =>
        state.Item1.IsConnected || state.Item2.IsConnected || state.Item3.IsConnected || state.Item4.IsConnected;

    /// <inheritdoc cref="GamePadState.IsButtonDown"/>
    public static bool IsButtonDown(
        this in (GamePadState, GamePadState, GamePadState, GamePadState) state,
        Buttons buttons
    ) =>
        Unsafe.AsRef(state.Item1).IsButtonDown(buttons) ||
        Unsafe.AsRef(state.Item2).IsButtonDown(buttons) ||
        Unsafe.AsRef(state.Item3).IsButtonDown(buttons) ||
        Unsafe.AsRef(state.Item4).IsButtonDown(buttons);

    /// <inheritdoc cref="GamePadState.IsButtonUp"/>
    public static bool IsButtonUp(
        this in (GamePadState, GamePadState, GamePadState, GamePadState) state,
        Buttons buttons
    ) =>
        Unsafe.AsRef(state.Item1).IsButtonUp(buttons) ||
        Unsafe.AsRef(state.Item2).IsButtonUp(buttons) ||
        Unsafe.AsRef(state.Item3).IsButtonUp(buttons) ||
        Unsafe.AsRef(state.Item4).IsButtonUp(buttons);
}
#endif
