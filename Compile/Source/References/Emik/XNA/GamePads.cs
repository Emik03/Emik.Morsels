// SPDX-License-Identifier: MPL-2.0
#if XNA
// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Provides the enumeration over <see cref="GamePad"/> instances.</summary>
[StructLayout(LayoutKind.Auto)]
struct GamePads(PlayerIndex last = PlayerIndex.Four) : IEnumerable<GamePadState>, IEnumerator<GamePadState>
{
    readonly PlayerIndex _length = last + 1;

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
#endif
