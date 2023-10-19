// SPDX-License-Identifier: MPL-2.0
#if ROSLYN
#pragma warning disable GlobalUsingsAnalyzer
// ReSharper disable once RedundantUsingDirective.Global
global using static Emik.Morsels.IncludedSyntaxNodeRegistrant;

#pragma warning restore GlobalUsingsAnalyzer
// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

using static SpecialType;

/// <summary>Contains syntactic operations and registrations.</summary>
static partial class IncludedSyntaxNodeRegistrant
{
    /// <summary>Adds the deconstruction of the tuples onto the <see cref="SourceProductionContext"/>.</summary>
    /// <param name="context">The context to use for source generation.</param>
    /// <param name="tuple">The tuple containing the hint name and source.</param>
    public static void AddSource(SourceProductionContext context, (string HintName, string Source) tuple) =>
        context.AddSource(tuple.HintName, tuple.Source);

    /// <summary>Returns whether the provided <see cref="SyntaxNode"/> is of type <typeparamref name="T"/>.</summary>
    /// <typeparam name="T">The type of <see cref="SyntaxNode"/> to test the instance for.</typeparam>
    /// <param name="node">The passed in node to test.</param>
    /// <param name="_">The discarded token, existing purely for convenience.</param>
    /// <returns>
    /// The value <see langword="true"/> if the parameter <paramref name="node"/> is
    /// an instance of <typeparamref name="T"/>, otherwise; <see langword="false"/>.
    /// </returns>
    [Pure]
    public static bool AnnotatedAndIs<T>([NotNullWhen(true)] SyntaxNode? node, CancellationToken _ = default)
        where T : MemberDeclarationSyntax =>
        node is T { AttributeLists.Count: >= 1 };

    /// <summary>Determines whether the symbol can be passed in as a generic.</summary>
    /// <param name="symbol">The symbol to check.</param>
    /// <returns>
    /// The value <see langword="true"/> if the parameter <paramref name="symbol"/>
    /// can be placed as a generic parameter, otherwise; <see langword="false"/>.
    /// </returns>
    [Pure]
    public static bool CanBeGeneric([NotNullWhen(true)] this ITypeSymbol? symbol) =>
        symbol is
            not null and
            not IDynamicTypeSymbol and
            not IPointerTypeSymbol and
            not { IsRefLikeType: true } and
            not { SpecialType: System_Void };

    /// <summary>Determines whether the symbol is declared with the attribute of the specific name.</summary>
    /// <param name="symbol">The symbol to check.</param>
    /// <param name="name">The name to get.</param>
    /// <returns>
    /// The value <see langword="true"/> if the parameter <paramref name="symbol"/>
    /// has the attribute <paramref name="name"/>, otherwise; <see langword="false"/>.
    /// </returns>
    [Pure]
    public static bool HasAttribute([NotNullWhen(true)] this ISymbol? symbol, string? name)
    {
        [Pure]
        static ReadOnlySpan<char> WithoutAttributeSuffix(string name) =>
            name.AsSpan() is var span && span is [.. var x, 'A', 't', 't', 'r', 'i', 'b', 'u', 't', 'e'] ? x : span;

        if (symbol is null)
            return false;

        if (name is null)
            return symbol.GetAttributes() is not [];

        var against = WithoutAttributeSuffix(name);

        foreach (var attribute in symbol.GetAttributes())
            if (attribute.AttributeClass?.Name is { } match && WithoutAttributeSuffix(match).SequenceEqual(against))
                return true;

        return false;
    }

    /// <summary>Returns whether the provided <see cref="SyntaxNode"/> is of type <typeparamref name="T"/>.</summary>
    /// <typeparam name="T">The type of <see cref="SyntaxNode"/> to test the instance for.</typeparam>
    /// <param name="node">The passed in node to test.</param>
    /// <param name="_">The discarded token, existing purely for convenience.</param>
    /// <returns>
    /// The value <see langword="true"/> if the parameter <paramref name="node"/> is
    /// an instance of <typeparamref name="T"/>, otherwise; <see langword="false"/>.
    /// </returns>
    [Pure]
    public static bool Is<T>([NotNullWhen(true)] SyntaxNode? node, CancellationToken _ = default)
        where T : SyntaxNode =>
        node is T;

    /// <summary>Determines whether the symbol is accessible from an external assembly.</summary>
    /// <param name="accessibility">The symbol to check.</param>
    /// <returns>
    /// The value <see langword="true"/> if the parameter <paramref name="accessibility"/> is accessible externally.
    /// </returns>
    [Pure]
    public static bool IsAccessible(this Accessibility accessibility) =>
        accessibility is Accessibility.Protected or Accessibility.ProtectedOrInternal or Accessibility.Public;

    /// <summary>Determines whether the symbol is accessible from an external assembly.</summary>
    /// <param name="symbol">The symbol to check.</param>
    /// <returns>
    /// The value <see langword="true"/> if the parameter <paramref name="symbol"/> is accessible externally.
    /// </returns>
    [Pure]
    public static bool IsAccessible([NotNullWhen(true)] this ISymbol? symbol) =>
        symbol?.DeclaredAccessibility.IsAccessible() is true;

    /// <summary>
    /// Determines whether the symbol and all subsequent parent types
    /// are declared with the <see langword="partial"/> keyword.
    /// </summary>
    /// <param name="symbol">The symbol to check.</param>
    /// <returns>
    /// The value <see langword="true"/> if the parameter <paramref name="symbol"/> and all its subsequent
    /// parent types are <see langword="partial"/>, otherwise; <see langword="false"/>.
    /// </returns>
    [Pure]
    public static bool IsCompletelyPartial([NotNullWhen(true)] this ISymbol? symbol) =>
        symbol?.FindPathToNull(x => x.ContainingType).All(IsPartial) is true;

    /// <summary>Returns whether the provided <see cref="SyntaxNode"/> is the first declaration.</summary>
    /// <param name="node">The passed in node to test.</param>
    /// <param name="symbol">The symbol to retrieve the declaring syntax references from.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>
    /// The value <see langword="true"/> if the parameter <paramref name="node"/> is the first
    /// to declare the parameter <paramref name="symbol"/>, otherwise; <see langword="false"/>.
    /// </returns>
    [Pure]
    public static bool IsFirst(
        this SyntaxNode? node,
        [NotNullWhen(true)] ISymbol? symbol,
        CancellationToken token = default
    ) =>
        symbol is { DeclaringSyntaxReferences: var x } && (x is not [var first, ..] || first.GetSyntax(token) == node);

    /// <summary>Determines whether the symbol is an <see langword="interface"/>.</summary>
    /// <param name="symbol">The symbol to check.</param>
    /// <returns>
    /// The value <see langword="true"/> if the parameter <paramref name="symbol"/>
    /// is an <see langword="interface"/>, otherwise; <see langword="false"/>.
    /// </returns>
    [Pure]
    public static bool IsInterface([NotNullWhen(true)] this ITypeSymbol? symbol) =>
        symbol is { BaseType: null, SpecialType: not System_Object };

    /// <summary>Returns whether the provided <see cref="ISymbol"/> is an interface implementation.</summary>
    /// <param name="symbol">The passed in symbol to test.</param>
    /// <returns>
    /// The value <see langword="true"/> if the parameter <paramref name="symbol"/>
    /// is an explicit interface implementation, otherwise; <see langword="false"/>.
    /// </returns>
    [Pure]
    public static bool IsInterfaceDeclaration([NotNullWhen(true)] this ISymbol? symbol) =>
        symbol?.Name.Contains('.') ?? false;

    /// <summary>Determines whether the symbol is declared with the <see cref="ObsoleteAttribute"/> attribute.</summary>
    /// <param name="symbol">The symbol to check.</param>
    /// <returns>
    /// The value <see langword="true"/> if the parameter <paramref name="symbol"/>
    /// is obsolete, otherwise; <see langword="false"/>.
    /// </returns>
    [Pure]
    public static bool IsObsolete([NotNullWhen(true)] this ISymbol? symbol) =>
        symbol.HasAttribute(nameof(ObsoleteAttribute));

    /// <summary>Determines whether the symbol is declared with the <see langword="partial"/> keyword.</summary>
    /// <param name="symbol">The symbol to check.</param>
    /// <returns>
    /// The value <see langword="true"/> if the parameter <paramref name="symbol"/>
    /// is <see langword="partial"/>, otherwise; <see langword="false"/>.
    /// </returns>
    [Pure]
    public static bool IsPartial([NotNullWhen(true)] this ISymbol? symbol) =>
        symbol
          ?.DeclaringSyntaxReferences
           .Select(x => x.GetSyntax())
           .OfType<TypeDeclarationSyntax>()
           .Any(x => x.Modifiers.Any(x => x.ValueText is "partial")) is true;

    /// <summary>Determines whether the symbol is an <see langword="unmanaged"/> primitive type.</summary>
    /// <param name="symbol">The symbol to check.</param>
    /// <returns>
    /// The value <see langword="true"/> if the parameter <paramref name="symbol"/> is
    /// an <see langword="unmanaged"/> primitive, otherwise; <see langword="false"/>.
    /// </returns>
    [Pure]
    public static bool IsUnmanagedPrimitive([NotNullWhen(true)] this ITypeSymbol? symbol) =>
        symbol is // ReSharper disable once MissingIndent
        {
            SpecialType: System_Char or
            System_SByte or
            System_Byte or
            System_Int16 or
            System_UInt16 or
            System_Int32 or
            System_UInt32 or
            System_Int64 or
            System_UInt64 or
            System_Decimal or
            System_Single or
            System_Double or
            System_IntPtr or
            System_UIntPtr,
        };

    /// <summary>Determines whether the symbol has a default implementation.</summary>
    /// <param name="symbol">The symbol to check.</param>
    /// <returns>The value <see langword="true"/> if the symbol has a default implementation.</returns>
    [Pure]
    public static bool HasDefaultImplementation([NotNullWhen(true)] this ISymbol? symbol) =>
        symbol is IMethodSymbol { IsAbstract: false, IsVirtual: true };

    /// <summary>Determines whether the symbol has a parameterless constructor.</summary>
    /// <param name="symbol">The symbol to check.</param>
    /// <returns>
    /// The value <see langword="true"/> if the parameter <paramref name="symbol"/>
    /// has a parameterless constructor, otherwise; <see langword="false"/>.
    /// </returns>
    [Pure]
    public static bool HasParameterlessConstructor([NotNullWhen(true)] this ITypeSymbol? symbol) =>
        symbol is INamedTypeSymbol { InstanceConstructors: var x } && x.Any(x => x.Parameters is []);

    /// <summary>Gets the hint name of the <see cref="INamedTypeSymbol"/>.</summary>
    /// <param name="symbol">The symbol to use.</param>
    /// <param name="prefix">If specified, the prefix to contain within the hint name.</param>
    /// <returns>The hint name of the parameter <paramref name="symbol"/>.</returns>
    [Pure]
    [return: NotNullIfNotNull(nameof(symbol))]
    public static string? HintName(this INamedTypeSymbol? symbol, string? prefix = nameof(Emik))
    {
        if (symbol is null)
            return null;

        StringBuilder sb = new(symbol.Name);
        ISymbol? containing = symbol;

        while ((containing = containing.ContainingWithoutGlobal()) is not null)
            sb.Insert(0, '.').Insert(0, containing.Name);

        if (prefix is not null)
            sb.Insert(0, '.').Insert(0, prefix);

        if (symbol.TypeParameters.Length is not 0 and var i)
            sb.Append('`').Append(i);

        return sb.Append(".g.cs").ToString();
    }

    /// <summary>Gets the keyword associated with the declaration of the <see cref="ITypeSymbol"/>.</summary>
    /// <param name="symbol">The symbol to get its keyword.</param>
    /// <returns>The keyword used to declare the parameter <paramref name="symbol"/>.</returns>
    [Pure]
    public static string Keyword(this ITypeSymbol symbol) =>
        symbol switch
        {
            { IsValueType: true, IsRecord: true } => "record struct",
            { IsRecord: true } => "record",
            { IsValueType: true } => "struct",
            { IsReferenceType: true } => "class",
            _ => throw Unreachable,
        };

    /// <summary>Gets the keyword associated with the declaration of the <see cref="RefKind"/>.</summary>
    /// <param name="kind">The symbol to get its keyword.</param>
    /// <returns>The keyword used to declare the parameter <paramref name="kind"/>.</returns>
    [Pure]
    public static string Keyword(this RefKind kind) =>
        kind switch
        {
            RefKind.In => "in ",
            RefKind.Out => "out ",
            RefKind.Ref => "ref ",
            _ => "",
        };

    /// <summary>Gets the keyword associated with the declaration of the <see cref="ISymbol"/>.</summary>
    /// <param name="symbol">The symbol to get its keyword.</param>
    /// <param name="alwaysReadOnly">Determines whether to always treat <see cref="IFieldSymbol"/> immutably.</param>
    /// <returns>The keyword used to declare the parameter <paramref name="symbol"/>.</returns>
    [Pure]
    public static string KeywordByRefReturn(this ISymbol symbol, bool alwaysReadOnly = false) =>
        symbol switch
        {
            IPropertySymbol { ReturnsByRef: true } or IMethodSymbol { ReturnsByRef: true } => "ref ",
            IPropertySymbol { ReturnsByRefReadonly: true } or IMethodSymbol { ReturnsByRefReadonly: true } =>
                "ref readonly ",
            IFieldSymbol { ContainingType.IsReadOnly: var containing, IsReadOnly: var self, RefKind: not RefKind.None }
                => containing || self || alwaysReadOnly ? "ref readonly " : "ref ",
            _ => "",
        };

    /// <inheritdoc cref="MemberPath.TryGetMemberName(ExpressionSyntax, out string)"/>
    [Pure]
    public static string? MemberName(this ExpressionSyntax syntax)
    {
        syntax.TryGetMemberName(out var result);
        return result;
    }

    /// <inheritdoc cref="AttributeArgumentSyntaxExt.TryGetStringValue(AttributeArgumentSyntax, SemanticModel, CancellationToken, out string)"/>
    [Pure]
    public static string? StringValue(this SyntaxNodeAnalysisContext context, AttributeArgumentSyntax syntax)
    {
        syntax.TryGetStringValue(context.SemanticModel, context.CancellationToken, out var result);
        return result;
    }

    /// <inheritdoc cref="AnalysisContext.RegisterSyntaxNodeAction{TLanguageKindEnum}(Action{SyntaxNodeAnalysisContext}, TLanguageKindEnum[])"/>
    public static AnalysisContext RegisterSyntaxNodeAction<TSyntaxNode>(
        this AnalysisContext context,
        Action<SyntaxNodeAnalysisContext, TSyntaxNode> action,
        params SyntaxKind[] syntaxKinds
    )
        where TSyntaxNode : SyntaxNode =>
        context.RegisterSyntaxNodeAction(action, ImmutableArray.Create(syntaxKinds));

    /// <inheritdoc cref="AnalysisContext.RegisterSyntaxNodeAction{TLanguageKindEnum}(Action{SyntaxNodeAnalysisContext}, ImmutableArray{TLanguageKindEnum})"/>
    public static AnalysisContext RegisterSyntaxNodeAction<TSyntaxNode>(
        this AnalysisContext context,
        Action<SyntaxNodeAnalysisContext, TSyntaxNode> action,
        ImmutableArray<SyntaxKind> syntaxKinds
    )
        where TSyntaxNode : SyntaxNode
    {
        context.RegisterSyntaxNodeAction(Filter(action), syntaxKinds);
        return context;
    }

    /// <summary>Adds information to a diagnostic.</summary>
    /// <typeparam name="T">The type of <paramref name="message"/>.</typeparam>
    /// <param name="diagnostic">The diagnostic to append.</param>
    /// <param name="message">The string to append.</param>
    /// <returns>The diagnostic with added information.</returns>
    [MustUseReturnValue]
    public static Diagnostic And<T>(this Diagnostic diagnostic, T message) =>
        Diagnostic.Create(
            new(
                diagnostic.Descriptor.Id,
                diagnostic.Descriptor.Title,
                $"{diagnostic.Descriptor.MessageFormat} {message.Stringify()}",
                diagnostic.Descriptor.Category,
                diagnostic.Descriptor.DefaultSeverity,
                diagnostic.Descriptor.IsEnabledByDefault,
                $"{diagnostic.Descriptor.Description} {message.Stringify()}",
                diagnostic.Descriptor.HelpLinkUri,
                diagnostic.Descriptor.CustomTags.ToArrayLazily()
            ),
            diagnostic.Location,
            diagnostic.Severity,
            diagnostic.AdditionalLocations,
            diagnostic.Properties
        );

    /// <summary>Gets all the members, including its interfaces and base type members.</summary>
    /// <param name="symbol">The symbol to get all of the members of.</param>
    /// <returns>
    /// All of the symbols of the parameter <paramref name="symbol"/>, including the members that come from its
    /// interfaces and base types, and any subsequent interfaces and base types from those.
    /// </returns>
    [Pure]
    public static IEnumerable<ISymbol> GetAllMembers(this INamedTypeSymbol symbol) =>
        symbol
           .BaseType
           .FindPathToNull(x => x.BaseType)
           .SelectMany(GetAllMembers)
           .Concat(symbol.GetMembers());

    /// <summary>Gets the symbol from a lookup.</summary>
    /// <param name="context">The context to use.</param>
    /// <param name="syntax">The syntax to lookup.</param>
    /// <returns>The symbols that likely define it.</returns>
    [Pure]
    public static IEnumerable<ISymbol> Symbols(this in SyntaxNodeAnalysisContext context, ExpressionSyntax syntax) =>
        (syntax.MemberName() ?? $"{syntax}") is var name && syntax is PredefinedTypeSyntax
            ? context.Compilation.GetSymbolsWithName(
                x => x.Contains(name),
                cancellationToken: context.CancellationToken
            )
            : context.SemanticModel.LookupSymbols(syntax.SpanStart, name: name);

    /// <summary>Gets the containing <see cref="INamespaceOrTypeSymbol"/>.</summary>
    /// <param name="syntax">The syntax to lookup.</param>
    /// <returns>The containing type or namespace of the parameter <paramref name="syntax"/>.</returns>
    [Pure]
    public static INamespaceOrTypeSymbol ContainingSymbol(this ISymbol syntax) =>
        syntax.ContainingType ?? (INamespaceOrTypeSymbol)syntax.ContainingNamespace;

    /// <summary>Gets the containing symbol so long as it isn't the global namespace.</summary>
    /// <param name="symbol">The symbol to use.</param>
    /// <returns>The containing symbol, or <see langword="null"/> if it is the global namespace.</returns>
    [Pure]
    public static ISymbol? ContainingWithoutGlobal(this ISymbol? symbol) =>
        symbol?.ContainingSymbol is var x && x is INamespaceSymbol { IsGlobalNamespace: true } ? null : x;

    /// <inheritdoc cref="GetAllMembers(INamespaceSymbol)" />
    [Pure]
    public static IEnumerable<INamespaceOrTypeSymbol> GetAllMembers(this Compilation symbol) =>
        symbol.GlobalNamespace.GetAllMembers();

    /// <inheritdoc cref="GetAllMembers(INamespaceSymbol)" />
    [Pure]
    public static IEnumerable<INamespaceOrTypeSymbol> GetAllMembers(this IAssemblySymbol symbol) =>
        symbol.GlobalNamespace.GetAllMembers();

    /// <summary>Gets all of the types declared by this symbol.</summary>
    /// <param name="symbol">The symbol to get all of the type symbols of.</param>
    /// <returns>
    /// The <see cref="IEnumerable{T}"/> of all types defined in the parameter <paramref name="symbol"/>.
    /// </returns>
    [Pure]
    public static IEnumerable<INamespaceOrTypeSymbol> GetAllMembers(this INamespaceSymbol symbol) =>
        symbol.GetMembers().SelectMany(GetAllNamespaceOrTypeSymbolMembers).Prepend(symbol);

    /// <summary>Gets the underlying type symbol of another symbol.</summary>
    /// <param name="symbol">The symbol to get the underlying type from.</param>
    /// <returns>The underlying type symbol from <paramref name="symbol"/>, if applicable.</returns>
    [Pure]
    public static ITypeSymbol? ToUnderlying(this ISymbol? symbol) =>
        symbol switch
        {
            IEventSymbol x => x.Type,
            IFieldSymbol x => x.Type,
            ILocalSymbol x => x.Type,
            IDiscardSymbol x => x.Type,
            IPropertySymbol x => x.Type,
            IParameterSymbol x => x.Type,
            IMethodSymbol x => x.ReturnType,
            IArrayTypeSymbol x => x.ElementType,
            IPointerTypeSymbol x => x.PointedAtType,
            IFunctionPointerTypeSymbol x => x.Signature.ReturnType,
            _ => null,
        };

    /// <summary>Gets the underlying symbol if the provided parameter is the nullable type.</summary>
    /// <param name="symbol">The symbol to get the underlying type from.</param>
    /// <returns>The underlying type of <paramref name="symbol"/>, if it exists.</returns>
    [Pure]
    public static ITypeSymbol? UnderlyingNullable(this ISymbol? symbol) =>
        symbol is INamedTypeSymbol
        {
            ContainingNamespace: { ContainingNamespace.IsGlobalNamespace: true, Name: nameof(System) },
            Name: nameof(Nullable),
            IsValueType: true,
            TypeArguments:
            [
                { } underlying and not { Name: nameof(Nullable) },
            ],
        }
            ? underlying
            : null;

    /// <summary>Gets the specified symbol.</summary>
    /// <typeparam name="T">The type of symbol to get.</typeparam>
    /// <param name="context">The context.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>The context node as <typeparamref name="T"/>.</returns>
    public static T? Get<T>(this in GeneratorSyntaxContext context, CancellationToken token = default)
        where T : ISymbol =>
        context.SemanticModel.GetDeclaredSymbol(context.Node, token) is T symbol ? symbol : default;

    [Pure]
    static Action<SyntaxNodeAnalysisContext> Filter<TSyntaxNode>(Action<SyntaxNodeAnalysisContext, TSyntaxNode> action)
        where TSyntaxNode : SyntaxNode =>
        context =>
        {
            if (!context.IsExcludedFromAnalysis() && context.Node is TSyntaxNode node)
                action(context, node);
        };

    [Pure]
    static IEnumerable<INamespaceOrTypeSymbol> GetAllNamespaceOrTypeSymbolMembers(INamespaceOrTypeSymbol x) =>
        ((x as INamespaceSymbol)?.GetAllMembers() ?? Enumerable.Empty<INamespaceOrTypeSymbol>()).Prepend(x);
}
#endif
