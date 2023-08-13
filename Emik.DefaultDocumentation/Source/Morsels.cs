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
        throw new IOException($"Duplicate file name \"{name}\" at {seen.Stringify()} and {item.Stringify()}.");

    static string Parent(DocItem item) =>
        item.Parent is EntityDocItem { Entity: { SymbolKind: not SymbolKind.Namespace, Name: var name } } parent
            ? $"{name}{TypeParameters(parent)}."
            : "";

    static string Parameters(DocItem item) =>
        (item as IParameterizedDocItem)?.Parameters.ToListLazily() is [_, ..] p ? $"({Join(p)})" : "";

    static string TypeArguments(IType type) => ElementType(type).TypeArguments is [_, ..] x ? $"{{{Join(x)}}}" : "";

    static string TypeParameters(DocItem item) =>
        (item as ITypeParameterizedDocItem)?.TypeParameters.ToListLazily() is [_, ..] x
            ? $"{{{Join(x)}}}"
            : "";

    static string Join(IEnumerable<DocItem> parameters) => parameters.Select(ParameterName).Conjoin(",");

    static string Join(IEnumerable<IType> parameters) => parameters.Select(ParameterName).Conjoin(",");

    static string ParameterName(DocItem item) =>
        item switch
        {
            TypeParameterDocItem { TypeParameter.Name: var x } => x,
            ParameterDocItem
                {
                    Parameter.Type: { Namespace: nameof(System), Name: nameof(Nullable), TypeArguments: [var x] },
                } => $"{ParameterName(x)}+",
            ParameterDocItem { Parameter.Type: var x } => $"{ToAlias(x)}{TypeArguments(x)}",
            _ => item.Name,
        };

    static string ParameterName(IType type) => $"{ToAlias(type)}{TypeArguments(type)}";

    static string MemberName(DocItem item) =>
        item is not EntityDocItem entity ? item.Name :
        entity.Entity is IMethod { Name: "op_Implicit" or "op_Explicit" } method ? method.ReturnType.Name :
        entity.Entity.Name;

    static string ToAlias(INamedElement name) => name.Namespace is nameof(System) ? ToAlias(name.Name) : name.Name;

    static string ToAlias(string str) =>
        str.Replace(nameof(Boolean), "bool")
           .Replace(nameof(Byte), "byte")
           .Replace(nameof(Char), "char")
           .Replace(nameof(Decimal), "decimal")
           .Replace(nameof(Double), "double")
           .Replace(nameof(Single), "float")
           .Replace(nameof(Int32), "int")
           .Replace(nameof(Int64), "long")
           .Replace(nameof(IntPtr), "nint")
           .Replace(nameof(UIntPtr), "nuint")
           .Replace(nameof(Object), "object")
           .Replace(nameof(SByte), "sbyte")
           .Replace(nameof(Int16), "short")
           .Replace(nameof(String), "string")
           .Replace(nameof(UInt32), "uint")
           .Replace(nameof(UInt64), "ulong")
           .Replace(nameof(UInt16), "ushort");

    static IType ElementType(IType type) => (type as ByReferenceType)?.ElementType ?? type;
}
