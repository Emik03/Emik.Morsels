// SPDX-License-Identifier: MPL-2.0
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
                diagnostic.Descriptor.CustomTags.ToArray()
            ),
            diagnostic.Location,
            diagnostic.Severity,
            diagnostic.AdditionalLocations,
            diagnostic.Properties
        );

    /// <inheritdoc cref="AttributeArgumentSyntaxExt.TryGetStringValue(AttributeArgumentSyntax, SemanticModel, CancellationToken, out string)"/>
    public static IEnumerable<ISymbol> Symbols(this SyntaxNodeAnalysisContext context, ExpressionSyntax syntax) =>
        (syntax.MemberName() ?? $"{syntax}") is var name && syntax is PredefinedTypeSyntax
            ? context.Compilation.GetSymbolsWithName(
                x => x.Contains(name),
                cancellationToken: context.CancellationToken
            )
            : context.SemanticModel.LookupSymbols(syntax.SpanStart, name: name);

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
            _ => null,
        };

    static Action<SyntaxNodeAnalysisContext> Filter<TSyntaxNode>(Action<SyntaxNodeAnalysisContext, TSyntaxNode> action)
        where TSyntaxNode : SyntaxNode =>
        context =>
        {
            if (!context.IsExcludedFromAnalysis() && context.Node is TSyntaxNode node)
                action(context, node);
        };
}
