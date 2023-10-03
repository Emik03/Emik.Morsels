// SPDX-License-Identifier: MPL-2.0
#if ROSLYN
// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Generates the attribute needed to use this analyzer.</summary>
/// <param name="hintName">The file name of the source.</param>
/// <param name="contents">The contents of the source.</param>
public abstract class FixedGenerator(
    [StringSyntax(StringSyntaxAttribute.Uri), UriString] string hintName,
    [StringSyntax("C#")] string contents
) : IIncrementalGenerator
{
    /// <inheritdoc />
    void IIncrementalGenerator.Initialize(IncrementalGeneratorInitializationContext context) =>
        context.RegisterPostInitializationOutput(x => x.AddSource(hintName, contents));
}
#endif
