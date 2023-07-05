// SPDX-License-Identifier: MPL-2.0
#if ROSLYN
// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>
/// <see cref="AnalysisContext.RegisterSyntaxNodeAction{TLanguageKindEnum}(Action{SyntaxNodeAnalysisContext}, TLanguageKindEnum[])"/>
/// with a wrapped callback which filters out ignored contexts.
/// </summary>
static partial class IncludedSyntaxNodeRegistrant
{
    /// <inheritdoc cref="MemberPath.TryGetMemberName(ExpressionSyntax, out string)"/>
    public static string? MemberName(this ExpressionSyntax syntax)
    {
        syntax.TryGetMemberName(out var result);
        return result;
    }

    /// <inheritdoc cref="AttributeArgumentSyntaxExt.TryGetStringValue(AttributeArgumentSyntax, SemanticModel, CancellationToken, out string)"/>
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
    public static IEnumerable<ISymbol> Symbols(this SyntaxNodeAnalysisContext context, ExpressionSyntax syntax) =>
        (syntax.MemberName() ?? $"{syntax}") is var name && syntax is PredefinedTypeSyntax
            ? context.Compilation.GetSymbolsWithName(
                x => x.Contains(name),
                cancellationToken: context.CancellationToken
            )
            : context.SemanticModel.LookupSymbols(syntax.SpanStart, name: name);

    /// <summary>Gets the containing <see cref="INamespaceOrTypeSymbol"/>.</summary>
    /// <param name="syntax">The syntax to lookup.</param>
    /// <returns>The containing type or namespace of the parameter <paramref name="syntax"/>.</returns>
    public static INamespaceOrTypeSymbol ContainingSymbol(this ISymbol syntax) =>
        syntax.ContainingType ?? (INamespaceOrTypeSymbol)syntax.ContainingNamespace;

    /// <inheritdoc cref="GetAllMembers(INamespaceOrTypeSymbol)" />
    public static IEnumerable<INamespaceOrTypeSymbol> GetAllMembers(this IAssemblySymbol symbol) =>
        symbol.GlobalNamespace.GetAllMembers();

    /// <summary>Gets all of the types declared by this symbol.</summary>
    /// <param name="symbol">The symbol to get all of the type symbols of.</param>
    /// <returns>
    /// The <see cref="IEnumerable{T}"/> of all types defined in the parameter <paramref name="symbol"/>.
    /// </returns>
    public static IEnumerable<INamespaceOrTypeSymbol> GetAllMembers(this INamespaceOrTypeSymbol symbol) =>
        symbol.GetMembers().SelectMany(GetAllNamespaceOrTypeSymbolMembers).Prepend(symbol);

    /// <summary>Gets the underlying type symbol of another symbol.</summary>
    /// <param name="symbol">The symbol to get the underlying type from.</param>
    /// <returns>The underlying type symbol from <paramref name="symbol"/>, if applicable.</returns>
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

    static Action<SyntaxNodeAnalysisContext> Filter<TSyntaxNode>(Action<SyntaxNodeAnalysisContext, TSyntaxNode> action)
        where TSyntaxNode : SyntaxNode =>
        context =>
        {
            if (!context.IsExcludedFromAnalysis() && context.Node is TSyntaxNode node)
                action(context, node);
        };

    static IEnumerable<INamespaceOrTypeSymbol> GetAllNamespaceOrTypeSymbolMembers(ISymbol symbol) =>
        symbol is INamespaceOrTypeSymbol x ? x.GetAllMembers() : Enumerable.Empty<INamespaceOrTypeSymbol>();
}
#endif
