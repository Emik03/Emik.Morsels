// SPDX-License-Identifier: MPL-2.0

// ReSharper disable CheckNamespace
namespace Emik.Morsels;

using static Span;

/// <summary>Extension methods to attempt to grab the span from enumerables.</summary>
static partial class TryAccessSpan
{
    /// <summary>Tries to extract a span from the source.</summary>
    /// <typeparam name="T">The type of element in the <see cref="IEnumerable{T}"/>.</typeparam>
    /// <param name="source">The source to extract the span from.</param>
    /// <param name="span">The resulting span.</param>
    /// <returns>Whether the span can be extracted from the parameter <paramref name="source"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGetSpan<T>(
        [NoEnumeration, NotNullWhen(true)] this IEnumerable<T>? source,
        out ReadOnlySpan<T> span
    ) =>
        source switch
        {
            T[] provider => (span = provider) is var _,
#if !NETFRAMEWORK || NET45_OR_GREATER
            ArraySegment<T> provider => (span = provider.AsSpan()) is var _,
#endif
#if NETCOREAPP || ROSLYN
            ImmutableArray<T> provider => (span = provider.AsSpan()) is var _,
#endif
            List<T> provider => (span = CollectionsMarshal.AsSpan(provider)) is var _,
            string provider => (span = To<T>.From(provider.AsSpan())) is var _,
            _ => !((span = default) is var _),
        };
}
