// SPDX-License-Identifier: MPL-2.0
namespace Emik.DefaultDocumentation;

/// <inheritdoc />
[CLSCompliant(false)]
public sealed class Morsels : AMarkdownFactory
{
    /// <inheritdoc />
    public override string Name => nameof(Morsels);

    /// <inheritdoc />
    protected override string GetMarkdownFileName(IGeneralContext context, DocItem item) =>
        item is EntityDocItem entity and IParameterizedDocItem { Parameters: var parameters }
            ? MethodName(item, entity, parameters)
            : item.Name;

    static string MethodName(DocItem item, EntityDocItem entity, IEnumerable<ParameterDocItem> parameters) =>
        $"{item.Parent?.Name}.{entity.Entity.Name}({ParameterNames(parameters)})";

    static string ParameterNames(IEnumerable<ParameterDocItem> parameters) =>
        parameters.Select(x => x.Parameter.Type.Name).Conjoin(",");
}
