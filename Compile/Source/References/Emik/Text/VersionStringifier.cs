// SPDX-License-Identifier: MPL-2.0
// ReSharper disable CheckNamespace RedundantNameQualifier UseSymbolAlias
namespace Emik.Morsels;

/// <summary>Provides methods to turn <see cref="Version"/> into a <see cref="string"/>.</summary>
static partial class VersionConciseStringFactory
{
    /// <summary>Gets the short display form of the version.</summary>
    /// <param name="version">The <see cref="Version"/> to convert.</param>
    /// <returns>The full name of the parameter <paramref name="version"/>.</returns>
    [Pure]
    public static string ToShortString(this Version? version)
    {
        if (version is not var (major, minor, build, revision) ||
            major <= 0 && minor <= 0 && build <= 0 && revision <= 0)
            return "v0";

        var length = (major.DigitCount() + 1 is var l && revision > 0 ?
                minor.DigitCount() + build.DigitCount() + revision.DigitCount() + 3 :
                build > 0 ? minor.DigitCount() + build.DigitCount() + 2 :
                    minor > 0 ? minor.DigitCount() + 1 : 0) +
            l;

        Span<char> span = stackalloc char[length];
        Format(span, version);
        return span.ToString();
    }

    static void Format(scoped Span<char> span, Version version)
    {
        Push('v', ref span);

        switch (version)
        {
            case (var major, var minor, var build, > 0 and var revision):
                Push(major, '.', ref span);
                Push(minor, '.', ref span);
                Push(build, '.', ref span);
                Push(revision, ref span);
                break;
            case (var major, var minor, > 0 and var build):
                Push(major, '.', ref span);
                Push(minor, '.', ref span);
                Push(build, ref span);
                break;
            case (var major, > 0 and var minor):
                Push(major, '.', ref span);
                Push(minor, ref span);
                break;
            default:
                Push(version.Major, ref span);
                break;
        }

        System.Diagnostics.Debug.Assert(span.IsEmpty, "span is drained");
    }

    static void Push(char c, scoped ref Span<char> span)
    {
        span[0] = c;
        span = span.UnsafelySkip(1);
    }

    static void Push([NonNegativeValue] int next, scoped ref Span<char> span)
    {
        if (!next.TryFormat(span, out var slice))
            System.Diagnostics.Debug.Fail("TryFormat");

        span = span.UnsafelySkip(slice);
    }

    // ReSharper disable RedundantAssignment
    static void Push([NonNegativeValue] int next, char c, scoped ref Span<char> span)
    {
        Push(next, ref span);
        Push(c, ref span);
    }
}
