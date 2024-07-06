// SPDX-License-Identifier: MPL-2.0
#if XNA
// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Provides the enumeration over <see cref="GamePad"/> instances.</summary>
[StructLayout(LayoutKind.Auto)]
struct GamePads(PlayerIndex? last = PlayerIndex.Four) : IEnumerable<GamePadState>, IEnumerator<GamePadState>
{
    readonly PlayerIndex _last = last ?? PlayerIndex.Four;

    PlayerIndex _index;

    /// <inheritdoc />
    public GamePadState Current { get; private set; }

    /// <inheritdoc />
    readonly object IEnumerator.Current => Current;

    /// <inheritdoc />
    readonly void IDisposable.Dispose() { }

    /// <inheritdoc />
    public void Reset() => _index = PlayerIndex.One;

    /// <inheritdoc />
    public bool MoveNext() => _index <= _last && (Current = GamePad.GetState(_index++)) is var _;

    /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
    public readonly GamePads GetEnumerator() => this;

    /// <inheritdoc />
    readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    readonly IEnumerator<GamePadState> IEnumerable<GamePadState>.GetEnumerator() => GetEnumerator();
}
#endif
