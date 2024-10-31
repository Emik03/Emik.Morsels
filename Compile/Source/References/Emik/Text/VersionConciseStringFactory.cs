// SPDX-License-Identifier: MPL-2.0
// ReSharper disable CheckNamespace RedundantNameQualifier UseSymbolAlias
namespace Emik.Morsels;

/// <summary>Provides methods to turn <see cref="Version"/> into a <see cref="string"/>.</summary>
static partial class VersionConciseStringFactory
{
    /// <summary>Gets the short display form of the version.</summary>
    /// <param name="version">The <see cref="Version"/> to convert.</param>
    /// <param name="prefix">The prefix to use.</param>
    /// <returns>The full name of the parameter <paramref name="version"/>.</returns>
    [Pure]
    public static string ToShortString(this Version? version, string? prefix = "v")
    {
        var (major, minor, build, revision) = version;

        var length = (prefix?.Length ?? 0) +
            major.DigitCount() +
            (revision > 0 ? minor.DigitCount() + build.DigitCount() + revision.DigitCount() + 3 :
                build > 0 ? minor.DigitCount() + build.DigitCount() + 2 :
                minor > 0 ? minor.DigitCount() + 1 : 0);

        Span<char> span = stackalloc char[length];
        Format(span, version, prefix);
        return span.ToString();
    }

    static unsafe void Format(scoped Span<char> span, Version? version, string? prefix)
    {
        static void PushLast([NonNegativeValue] int next, scoped ref Span<char> span)
        {
            if (!next.TryFormat(span, out var charsWritten))
                System.Diagnostics.Debug.Fail("TryFormat");

            span = span.UnsafelySkip(charsWritten);
        }

        static void Push([NonNegativeValue] int next, scoped ref Span<char> span)
        {
            PushLast(next, ref span);
            span[0] = '.';
            span = span.UnsafelySkip(1);
        }

        var length = prefix?.Length ?? 0;
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
        prefix.AsSpan().CopyTo(span);
#else
        fixed (char* ptr = prefix)
            new ReadOnlySpan<char>(ptr, length).CopyTo(span);
#endif
        span = span.UnsafelySkip(length);

        switch (version)
        {
            case (var major, var minor, var build, > 0 and var revision):
                Push(major, ref span);
                Push(minor, ref span);
                Push(build, ref span);
                PushLast(revision, ref span);
                break;
            case (var major, var minor, > 0 and var build):
                Push(major, ref span);
                Push(minor, ref span);
                PushLast(build, ref span);
                break;
            case (var major, > 0 and var minor):
                Push(major, ref span);
                PushLast(minor, ref span);
                break;
            default:
                PushLast(version?.Major ?? 0, ref span);
                break;
        }

        System.Diagnostics.Debug.Assert(span.IsEmpty, $"span is drained and not {span.Length} characters long");
    }
}
