// SPDX-License-Identifier: MPL-2.0
namespace Emik.DefaultDocumentation;

/// <inheritdoc />
[CLSCompliant(false)]
public sealed class Morsels : AMarkdownFactory
{
    readonly Dictionary<string, DocItem> _seen = new(StringComparer.Ordinal);

    /// <inheritdoc />
    public override string Name => nameof(Morsels);

    /// <inheritdoc />
    protected override string GetMarkdownFileName(IGeneralContext context, DocItem item) =>
        item is not AssemblyDocItem &&
        $"{Parent(item)}{Member(item)}{TypeParameters(item)}{Parameters(item)}" is var name &&
        (_seen[name] = _seen.TryGetValue(name, out var seen) && seen.Id == item.Id
            ? throw new IOException($"Duplicate file name \"{name}\" at {seen.Stringify()} and {item.Stringify()}.")
            : item) is var _
            ? name
            : "index";

    static string Alias(INamedElement name) =>
        name.Namespace is nameof(System)
            ? name.Name
               .ToBuilder()
               .Replace(nameof(Boolean), "bool")
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
               .Replace(nameof(UInt16), "ushort")
               .ToString()
            : name.Name;

    static string Member(DocItem item) =>
        item is not EntityDocItem entity ? item.Name :
        entity.Entity is IMethod { Name: "op_Implicit" or "op_Explicit" } method ? method.ReturnType.Name :
        entity.Entity.Name;

    static string Join(IEnumerable<DocItem> parameters) => parameters.Select(Parameter).Conjoin(',');

    static string Join(IEnumerable<IType> parameters) => parameters.Select(Parameter).Conjoin(',');

    static string Parameter(DocItem item) =>
        item switch
        {
            TypeParameterDocItem { TypeParameter.Name: var x } => x,
            ParameterDocItem { Parameter.Type: { FullName: "System.Nullable", TypeArguments: [var x] } }
                => $"{Parameter(x)}+",
            ParameterDocItem { Parameter.Type: var x } => $"{Alias(x)}{TypeArguments(x)}",
            _ => item.Name,
        };

    static string Parameter(IType type) => $"{Alias(type)}{TypeArguments(type)}";

    static string Parameters(DocItem item) =>
        (item as IParameterizedDocItem)?.Parameters.ToIList() is [_, ..] p ? $"({Join(p)})" : "";

    static string Parent(DocItem item) =>
        item.Parent is EntityDocItem { Entity: { SymbolKind: not SymbolKind.Namespace, Name: var name } } parent
            ? $"{name}{TypeParameters(parent)}."
            : "";

    static string TypeArguments(IType type) =>
        ((type as ByReferenceType)?.ElementType ?? type).TypeArguments is [_, ..] x ? $"{{{Join(x)}}}" : "";

    static string TypeParameters(DocItem item) =>
        (item as ITypeParameterizedDocItem)?.TypeParameters.ToIList() is [_, ..] x ? $"{{{Join(x)}}}" : "";
}
