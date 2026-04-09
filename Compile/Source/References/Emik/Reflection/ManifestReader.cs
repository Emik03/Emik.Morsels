// SPDX-License-Identifier: MPL-2.0
global using static Emik.Morsels.ManifestReader; // ReSharper disable once CheckNamespace
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
#if NETFRAMEWORK && !NET40_OR_GREATER
        List<byte> memory = [];

        while (stream.ReadByte() is not -1 and var b)
            memory.Add((byte)b);

        return memory.ToArray();
#else
        using MemoryStream memory = new();
        stream.CopyTo(memory);
        return memory.ToArray();
#endif
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
    public static Stream? GetManifestResourceStream(
        [PathReference, StringSyntax(StringSyntaxAttribute.Uri), UriString] string? path
    ) =>
        path is null or ""
            ? null
            : typeof(ManifestReader).Assembly.GetManifestResourceStream(
                $"{typeof(ManifestReader).Assembly.GetName().Name}.{path.Replace('/', '.').Replace('\\', '.')}"
            );
}
