// SPDX-License-Identifier: MPL-2.0
namespace Emik.DefaultDocumentation;

/// <inheritdoc />
[CLSCompliant(false)]
public sealed class Morsels : AMarkdownFactory
{
    static readonly Dictionary<string, DocItem> s_seen = new(StringComparer.Ordinal);

    /// <inheritdoc />
    public override string Name => nameof(Morsels);

    /// <inheritdoc />
    protected override string GetMarkdownFileName(IGeneralContext context, DocItem item)
    {
        if (item is AssemblyDocItem)
            return "index";

        var name = $"{Parent(item)}{MemberName(item)}{TypeParameters(item)}{Parameters(item)}";

        if (s_seen.TryGetValue(name, out var seen) && item.Id != seen.Id)
            Throw(item, name, seen);

        s_seen[name] = item;
        return name;
    }

    static void Throw(DocItem item, string name, DocItem seen) =>
        throw new IOException($"Duplicate file name \"{name}\" at {seen.Stringify(true)} and {item.Stringify(true)}.");

    static string Parent(DocItem item) =>
        item.Parent is EntityDocItem { Entity: { SymbolKind: not SymbolKind.Namespace, Name: var name } } parent
            ? $"{name}{TypeParameters(parent)}."
            : "";

    static string Parameters(DocItem item) =>
        (item as IParameterizedDocItem)?.Parameters.ToCollectionLazily() is { } p && p.Any() ? $"({Join(p)})" : "";

    static string TypeParameters(DocItem item) =>
        (item as ITypeParameterizedDocItem)?.TypeParameters.ToCollectionLazily() is { } p && p.Any()
            ? $"{{{Join(p)}}}"
            : "";

    static string TypeParameters(IType item) =>
        ElementType(item).TypeArguments is { } p && p.Any() ? $"{{{Join(p)}}}" : "";

    static string Join(IEnumerable<DocItem> parameters) => parameters.Select(ParameterName).Conjoin(",");

    static string Join(IEnumerable<IType> parameters) => parameters.Select(ParameterName).Conjoin(",");

    static string ParameterName(DocItem x) =>
        x switch
        {
            TypeParameterDocItem { TypeParameter.Name: var p } => p,
            ParameterDocItem { Parameter.Type: var p } => $"{p.Name}{TypeParameters(p)}",
            _ => x.Name,
        };

    static string ParameterName(IType x) => $"{x.Name}{TypeParameters(x)}";

    static string MemberName(DocItem item) =>
        item is not EntityDocItem entity ? item.Name :
        entity.Entity is IMethod { Name: "op_Implicit" or "op_Explicit" } method ? method.ReturnType.Name :
        entity.Entity.Name;

    static IType ElementType(IType item) => (item as ByReferenceType)?.ElementType ?? item;
}
