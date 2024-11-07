// SPDX-License-Identifier: MPL-2.0

// ReSharper disable CheckNamespace RedundantUsingDirective
namespace Emik.Morsels;
#if NETCOREAPP_3_0_OR_GREATER
using static Span;
#endif

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
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
            ArraySegment<T> provider => (span = provider.AsSpan()) is var _,
#endif
#if NETCOREAPP || ROSLYN
            ImmutableArray<T> provider => (span = provider.AsSpan()) is var _,
#endif
            List<T> provider => (span = CollectionsMarshal.AsSpan(provider)) is var _,
            string provider => (span = Span.To<T>.From(provider.AsSpan())) is var _,
            _ => !((span = default) is var _),
        };
}
