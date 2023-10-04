// SPDX-License-Identifier: MPL-2.0
#if ROSLYN
#pragma warning disable GlobalUsingsAnalyzer
global using static Emik.Morsels.IncludedSyntaxNodeRegistrant;

#pragma warning restore GlobalUsingsAnalyzer
// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>
/// <see cref="AnalysisContext.RegisterSyntaxNodeAction{TLanguageKindEnum}(Action{SyntaxNodeAnalysisContext}, TLanguageKindEnum[])"/>
/// with a wrapped callback which filters out ignored contexts.
/// </summary>
static partial class IncludedSyntaxNodeRegistrant
{
    /// <summary>Filters an <see cref="IncrementalValuesProvider{T}"/> to only non-null values.</summary>
    /// <typeparam name="T">The type of value to filter.</typeparam>
    /// <param name="provider">The <see cref="IncrementalValuesProvider{T}"/> to filter.</param>
    /// <returns>A filtered <see cref="IncrementalValuesProvider{T}"/> with strictly non-null values.</returns>
    [Pure]
    public static IncrementalValuesProvider<T> Filter<T>(this IncrementalValuesProvider<T?> provider) =>
#pragma warning disable 8619
        provider.Where(x => x is not null);
#pragma warning restore 8619

    /// <summary>Filters an <see cref="IncrementalValuesProvider{T}"/> to only non-null values.</summary>
    /// <typeparam name="T">The type of value to filter.</typeparam>
    /// <param name="provider">The <see cref="IncrementalValuesProvider{T}"/> to filter.</param>
    /// <returns>A filtered <see cref="IncrementalValuesProvider{T}"/> with strictly non-null values.</returns>
    [Pure]
    public static IncrementalValuesProvider<T> Filter<T>(this IncrementalValuesProvider<T?> provider)
        where T : struct =>
#pragma warning disable 8629
        provider.Where(x => x.HasValue).Select((x, _) => x.Value);
#pragma warning restore 8629

    /// <summary>Determines whether the symbol is declared with the attribute of the specific name.</summary>
    /// <param name="symbol">The symbol to check.</param>
    /// <param name="name">The name to get.</param>
    /// <returns>
    /// The value <see langword="true"/> if the parameter <paramref name="symbol"/>
    /// has the attribute <paramref name="name"/>, otherwise; <see langword="false"/>.
    /// </returns>
    [Pure]
    public static bool HasAttribute([NotNullWhen(true)] this ISymbol? symbol, string? name) =>
        symbol is not null &&
        (name is null
            ? !symbol.GetAttributes().IsEmpty
            : (name.EndsWith(nameof(Attribute)) ? name : $"{name}{nameof(Attribute)}") is var first &&
            (name.EndsWith(nameof(Attribute)) ? name[..^nameof(Attribute).Length] : name) is var second &&
            symbol.GetAttributes().Any(x => x.AttributeClass?.Name is { } name && (name == first || name == second)));

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

    /// <summary>Determines whether the symbol is an <see langword="interface"/>.</summary>
    /// <param name="symbol">The symbol to check.</param>
    /// <returns>
    /// The value <see langword="true"/> if the parameter <paramref name="symbol"/>
    /// is an <see langword="interface"/>, otherwise; <see langword="false"/>.
    /// </returns>
    [Pure]
    public static bool IsInterface([NotNullWhen(true)] this ITypeSymbol? symbol) =>
        symbol is { BaseType: null, SpecialType: not SpecialType.System_Object };

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
            not { SpecialType: SpecialType.System_Void };

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
        symbol is INamedTypeSymbol { InstanceConstructors: var x } && x.Any(x => x.Parameters.IsEmpty);

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
