// SPDX-License-Identifier: MPL-2.0
#if ROSLYN
// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Generates the attribute needed to use this analyzer.</summary>
/// <param name="fileName">The file name of the source.</param>
/// <param name="contents">The contents of the source.</param>
public abstract class FixedGenerator(
    [StringSyntax(StringSyntaxAttribute.Uri), UriString] string fileName,
    [StringSyntax("C#")] string contents
) : ISourceGenerator
{
    /// <inheritdoc />
    void ISourceGenerator.Execute(GeneratorExecutionContext context) => context.AddSource($"{fileName}.g.cs", contents);

    /// <inheritdoc />
    void ISourceGenerator.Initialize(GeneratorInitializationContext context) { }
}
#endif
