// SPDX-License-Identifier: MPL-2.0

// ReSharper disable MissingIndent UsePositionalDeconstructionPattern
// ReSharper disable once CheckNamespace
namespace Emik.Morsels;
#pragma warning disable CS1574, CS1580
/// <summary>Methods that deconstructs <see cref="Version"/> instances.</summary>
static partial class VersionDeconstructors
{
    /// <summary>Deconstructs this instance into the major and minor versions.</summary>
    /// <remarks><para>
    /// If the passed in value is <see langword="null"/>, all out parameters are zero-initialized.
    /// </para></remarks>
    /// <param name="version">The <see cref="Version"/> to deconstruct.</param>
    /// <param name="major">The resulting major version.</param>
    /// <param name="minor">The resulting minor version.</param>
    public static void Deconstruct(
        this Version? version,
        [NonNegativeValue] out int major,
        [NonNegativeValue] out int minor
    )
    {
        if (version is { Major: var maj, Minor: var min })
        {
            major = maj;
            minor = min;
            return;
        }

        major = default;
        minor = default;
    }

    /// <summary>Deconstructs this instance into the major, minor, and build versions.</summary>
    /// <remarks><para>
    /// If the passed in value is <see langword="null"/>, all out parameters are zero-initialized.
    /// </para></remarks>
    /// <param name="version">The <see cref="Version"/> to deconstruct.</param>
    /// <param name="major">The resulting major version.</param>
    /// <param name="minor">The resulting minor version.</param>
    /// <param name="build">The resulting build version.</param>
    public static void Deconstruct(
        this Version? version,
        [NonNegativeValue] out int major,
        [NonNegativeValue] out int minor,
        [NonNegativeValue] out int build
    )
    {
        if (version is { Major: var maj, Minor: var min, Build: var bui })
        {
            major = maj;
            minor = min;
            build = bui;
            return;
        }

        major = default;
        minor = default;
        build = default;
    }

    /// <summary>Deconstructs this instance into the major, minor, build, and revision versions.</summary>
    /// <remarks><para>
    /// If the passed in value is <see langword="null"/>, all out parameters are zero-initialized.
    /// </para></remarks>
    /// <param name="version">The <see cref="Version"/> to deconstruct.</param>
    /// <param name="major">The resulting major version.</param>
    /// <param name="minor">The resulting minor version.</param>
    /// <param name="build">The resulting build version.</param>
    /// <param name="revision">The resulting revision version.</param>
    public static void Deconstruct(
        this Version? version,
        [NonNegativeValue] out int major,
        [NonNegativeValue] out int minor,
        [NonNegativeValue] out int build,
        [NonNegativeValue] out int revision
    )
    {
        if (version is { Major: var maj, Minor: var min, Build: var bui, Revision: var rev })
        {
            major = maj;
            minor = min;
            build = bui;
            revision = rev;
            return;
        }

        major = default;
        minor = default;
        build = default;
        revision = default;
    }

    /// <summary>
    /// Deconstructs this instance into the major, minor, build, major revision, and minor revision versions.
    /// </summary>
    /// <remarks><para>
    /// If the passed in value is <see langword="null"/>, all out parameters are zero-initialized.
    /// </para></remarks>
    /// <param name="version">The <see cref="Version"/> to deconstruct.</param>
    /// <param name="major">The resulting major version.</param>
    /// <param name="minor">The resulting minor version.</param>
    /// <param name="build">The resulting build version.</param>
    /// <param name="majorRevision">The resulting major revision version.</param>
    /// <param name="minorRevision">The resulting minor revision version.</param>
    public static void Deconstruct(
        this Version? version,
        [NonNegativeValue] out int major,
        [NonNegativeValue] out int minor,
        [NonNegativeValue] out int build,
        [NonNegativeValue] out int majorRevision,
        [NonNegativeValue] out int minorRevision
    )
    {
        if (version is
        {
            Major: var maj, Minor: var min, Build: var bui, MajorRevision: var majRev, MinorRevision: var minRev,
        })
        {
            major = maj;
            minor = min;
            build = bui;
            majorRevision = majRev;
            minorRevision = minRev;
            return;
        }

        major = default;
        minor = default;
        build = default;
        majorRevision = default;
        minorRevision = default;
    }
}
