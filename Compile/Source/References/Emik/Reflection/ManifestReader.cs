// SPDX-License-Identifier: MPL-2.0
#if !NETSTANDARD || NETSTANDARD2_0_OR_GREATER
#pragma warning disable GlobalUsingsAnalyzer // ReSharper disable once RedundantUsingDirective.Global
global using static Emik.Morsels.ManifestReader;

// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Methods to read this assembly's manifest streams into common data structures.</summary>
// ReSharper disable RedundantNameQualifier
static partial class ManifestReader
{
    /// <summary>Reads the manifest resource as a sequence of bytes.</summary>
    /// <param name="path">The path of the manifest resource.</param>
    /// <returns>The sequence of bytes of the resource <paramref name="path"/> contained in this assembly.</returns>
    [MustUseReturnValue]
    public static byte[]? GetManifestResourceBytes(
        [PathReference, StringSyntax(StringSyntaxAttribute.Uri), UriString] string? path
    )
    {
        using var stream = GetManifestResourceStream(path);

        if (stream is null)
            return null;

        using MemoryStream memory = new();
        stream.CopyTo(memory);
        return memory.ToArray();
    }

    /// <summary>Reads the manifest resource as a <see cref="string"/>.</summary>
    /// <param name="path">The path of the manifest resource.</param>
    /// <returns>The <see cref="string"/> of the resource <paramref name="path"/> contained in this assembly.</returns>
    [MustUseReturnValue]
    public static string? GetManifestResourceString(
        [PathReference, StringSyntax(StringSyntaxAttribute.Uri), UriString] string? path
    )
    {
        using var stream = GetManifestResourceStream(path);

        if (stream is null)
            return null;

        using StreamReader sr = new(stream);
        return sr.ReadToEnd();
    }

    /// <summary>Reads the manifest.</summary>
    /// <param name="path">The path of the manifest resource.</param>
    /// <returns>The <see cref="Stream"/> of the resource <paramref name="path"/> contained in this assembly.</returns>
    [MustDisposeResource]
    static Stream? GetManifestResourceStream(
        [PathReference, StringSyntax(StringSyntaxAttribute.Uri), UriString] string? path
    ) =>
        path is null or ""
            ? null
            : typeof(Split<>).Assembly.GetManifestResourceStream(
                $"{typeof(Split<>).Assembly.GetName().Name}.{path.Replace('/', '.').Replace('\\', '.')}"
            );
}
#endif
