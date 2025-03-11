// SPDX-License-Identifier: MPL-2.0
#if ROSLYN // ReSharper disable OutParameterValueIsAlwaysDiscarded.Global
// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Contains syntactic operations and registrations.</summary>
static partial class AgainstAttributeWithMetadataNameProvider
{
    /// <summary>Gets the fully qualified name for a given symbol.</summary>
    /// <param name="symbol">The input <see cref="ISymbol"/> instance.</param>
    /// <returns>The fully qualified name for <paramref name="symbol"/>.</returns>
    public static string GetFullyQualifiedName(this ISymbol symbol) =>
        symbol.ToDisplayString(
            SymbolDisplayFormat.FullyQualifiedFormat.AddMiscellaneousOptions(
                SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers
            )
        );

    /// <summary>Gets the fully qualified name for a given symbol, including nullability annotations.</summary>
    /// <param name="symbol">The input <see cref="ISymbol"/> instance.</param>
    /// <returns>The fully qualified name for <paramref name="symbol"/>.</returns>
    public static string GetFullyQualifiedNameWithNullabilityAnnotations(this ISymbol symbol) =>
        symbol.ToDisplayString(
            SymbolDisplayFormat.FullyQualifiedFormat.AddMiscellaneousOptions(
                SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier |
                SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers
            )
        );

    /// <summary>Gets the minimally qualified name for a given symbol.</summary>
    /// <param name="symbol">The input <see cref="ISymbol"/> instance.</param>
    /// <returns>The minimally qualified name for <paramref name="symbol"/>.</returns>
    public static string GetMinimallyQualifiedName(this ISymbol symbol) =>
        symbol.ToDisplayString(
            SymbolDisplayFormat.MinimallyQualifiedFormat.AddMiscellaneousOptions(
                SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers
            )
        );

    /// <summary>Checks whether or not a given type symbol has a specified full name.</summary>
    /// <param name="symbol">The input <see cref="ISymbol"/> instance to check.</param>
    /// <param name="name">The full name to check.</param>
    /// <returns>Whether <paramref name="symbol"/> has a full name equals to <paramref name="name"/>.</returns>
    public static bool HasFullyQualifiedName(this ISymbol symbol, string name) =>
        symbol.GetFullyQualifiedName() == name;

    /// <summary>
    /// Checks whether or not a given symbol has an attribute with the specified fully qualified metadata name.
    /// </summary>
    /// <param name="symbol">The input <see cref="ISymbol"/> instance to check.</param>
    /// <param name="name">The attribute name to look for.</param>
    /// <returns>Whether or not <paramref name="symbol"/> has an attribute with the specified name.</returns>
    public static bool HasAttributeWithFullyQualifiedMetadataName(this ISymbol symbol, string name)
    {
        foreach (var attribute in symbol.GetAttributes())
            if (attribute.AttributeClass is { } named && named.HasFullyQualifiedMetadataName(name))
                return true;

        return false;
    }

    /// <summary>Checks whether or not a given symbol has an attribute with the specified type.</summary>
    /// <param name="symbol">The input <see cref="ISymbol"/> instance to check.</param>
    /// <param name="typeSymbol">The <see cref="ITypeSymbol"/> instance for the attribute type to look for.</param>
    /// <returns>Whether or not <paramref name="symbol"/> has an attribute with the specified type.</returns>
    public static bool HasAttributeWithType(this ISymbol symbol, ITypeSymbol typeSymbol) =>
        TryGetAttributeWithType(symbol, typeSymbol, out _);

    /// <summary>Tries to get an attribute with the specified type.</summary>
    /// <param name="symbol">The input <see cref="ISymbol"/> instance to check.</param>
    /// <param name="typeSymbol">The <see cref="ITypeSymbol"/> instance for the attribute type to look for.</param>
    /// <param name="attributeData">The resulting attribute, if it was found.</param>
    /// <returns>Whether or not <paramref name="symbol"/> has an attribute with the specified type.</returns>
    [Pure]
    public static bool TryGetAttributeWithType(
        this ISymbol symbol,
        ITypeSymbol typeSymbol,
        [NotNullWhen(true)] out AttributeData? attributeData
    )
    {
        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (var attribute in symbol.GetAttributes())
            if (SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, typeSymbol))
            {
                attributeData = attribute;
                return true;
            }

        attributeData = null;
        return false;
    }

    /// <summary>Checks whether or not a given type symbol has a specified fully qualified metadata name.</summary>
    /// <param name="symbol">The input <see cref="ITypeSymbol"/> instance to check.</param>
    /// <param name="name">The full name to check.</param>
    /// <returns>Whether <paramref name="symbol"/> has a full name equals to <paramref name="name"/>.</returns>
    public static bool HasFullyQualifiedMetadataName(this ITypeSymbol symbol, string name)
    {
        var builder = ImmutableArrayBuilder<char>.Rent();

        try
        {
            symbol.AppendFullyQualifiedMetadataName(builder);
            return builder.WrittenSpan.SequenceEqual(name.AsSpan());
        }
        finally
        {
            builder.Dispose();
        }
    }

    /// <summary>Gets the fully qualified metadata name for a given <see cref="ITypeSymbol"/> instance.</summary>
    /// <param name="symbol">The input <see cref="ITypeSymbol"/> instance.</param>
    /// <returns>The fully qualified metadata name for <paramref name="symbol"/>.</returns>
    [Pure]
    public static string GetFullyQualifiedMetadataName(this ITypeSymbol symbol)
    {
        var builder = ImmutableArrayBuilder<char>.Rent();

        try
        {
            symbol.AppendFullyQualifiedMetadataName(builder);
            return builder.ToString();
        }
        finally
        {
            builder.Dispose();
        }
    }

    /// <summary>Tries to get an attribute with the specified fully qualified metadata name.</summary>
    /// <param name="symbol">The input <see cref="ISymbol"/> instance to check.</param>
    /// <param name="name">The attribute name to look for.</param>
    /// <param name="attributeData">The resulting attribute, if it was found.</param>
    /// <returns>Whether or not <paramref name="symbol"/> has an attribute with the specified name.</returns>
    [Pure]
    public static bool TryGetAttributeWithFullyQualifiedMetadataName(
        this ISymbol symbol,
        string name,
        [NotNullWhen(true)] out AttributeData? attributeData
    )
    {
        foreach (var attribute in symbol.GetAttributes())
            if (attribute.AttributeClass is { } named && named.HasFullyQualifiedMetadataName(name))
            {
                attributeData = attribute;
                return true;
            }

        attributeData = null;
        return false;
    }

    /// <summary>Calculates the effective accessibility for a given symbol.</summary>
    /// <param name="symbol">The <see cref="ISymbol"/> instance to check.</param>
    /// <returns>The effective accessibility for <paramref name="symbol"/>.</returns>
    [Pure]
    public static Accessibility GetEffectiveAccessibility(this ISymbol symbol)
    {
        // Start by assuming it's visible
        var visibility = Accessibility.Public;

        // Handle special cases
        // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
        switch (symbol.Kind)
        {
            case SymbolKind.Alias: return Accessibility.Private; // ReSharper disable once TailRecursiveCall
            case SymbolKind.Parameter: return GetEffectiveAccessibility(symbol.ContainingSymbol);
            case SymbolKind.TypeParameter: return Accessibility.Private;
        }

        // Traverse the symbol hierarchy to determine the effective accessibility
        while (symbol is not null && symbol.Kind != SymbolKind.Namespace)
        {
            switch (symbol.DeclaredAccessibility)
            {
                case Accessibility.NotApplicable or Accessibility.Private: return Accessibility.Private;
                case Accessibility.Internal or Accessibility.ProtectedAndInternal:
                    visibility = Accessibility.Internal;
                    break;
            }

            symbol = symbol.ContainingSymbol;
        }

        return visibility;
    }

    /// <summary>Checks whether or not a given symbol can be accessed from a specified assembly.</summary>
    /// <param name="symbol">The input <see cref="ISymbol"/> instance to check.</param>
    /// <param name="assembly">The assembly to check the accessibility of <paramref name="symbol"/> for.</param>
    /// <returns>Whether <paramref name="assembly"/> can access <paramref name="symbol"/>.</returns>
    [Pure]
    public static bool CanBeAccessedFrom(this ISymbol symbol, IAssemblySymbol assembly) =>
        symbol.GetEffectiveAccessibility() is var accessibility &&
        accessibility == Accessibility.Public ||
        accessibility == Accessibility.Internal && symbol.ContainingAssembly.GivesAccessTo(assembly);

    /// <summary>Negated <see cref="SyntaxValueProvider.ForAttributeWithMetadataName"/>.</summary>
    /// <inheritdoc cref="SyntaxValueProvider.ForAttributeWithMetadataName"/>
    [Pure]
    public static IncrementalValuesProvider<T> AgainstAttributeWithMetadataName<T>(
        this SyntaxValueProvider syntaxValueProvider,
        string fullyQualifiedMetadataName,
        [InstantHandle] Func<SyntaxNode, CancellationToken, bool> predicate,
        [InstantHandle] Func<SyntaxNode, ISymbol, SemanticModel, CancellationToken, T> transform
    )
    {
        (bool HasValue, T Value) Extract(GeneratorSyntaxContext context, CancellationToken token) =>
            context.SemanticModel.GetDeclaredSymbol(context.Node, token) is { } symbol &&
            !symbol.TryGetAttributeWithFullyQualifiedMetadataName(fullyQualifiedMetadataName, out _)
                ? (true, transform(context.Node, symbol, context.SemanticModel, token))
                : default;

        return syntaxValueProvider
           .CreateSyntaxProvider(predicate, Extract)
           .Where(static x => x.HasValue)
           .Select(static (item, _) => item.Value);
    }

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

    /// <summary>Filters an <see cref="IncrementalValuesProvider{T}"/> to the specified destination type.</summary>
    /// <typeparam name="TFrom">The initial type.</typeparam>
    /// <typeparam name="TTo">The target type.</typeparam>
    /// <param name="provider">The <see cref="IncrementalValuesProvider{T}"/> to filter.</param>
    /// <returns>A filtered <see cref="IncrementalValuesProvider{T}"/> with <typeparamref name="TTo"/> values.</returns>
    [Pure]
    public static IncrementalValuesProvider<TTo> OfType<TFrom, TTo>(this IncrementalValuesProvider<TFrom?> provider)
        where TTo : TFrom => // ReSharper disable once NullableWarningSuppressionIsUsed
        provider.Where(static x => x is TTo).Select(static (x, _) => (TTo)x!);

    /// <summary>Appends the fully qualified metadata name for a given symbol to a target builder.</summary>
    /// <param name="symbol">The input <see cref="ITypeSymbol"/> instance.</param>
    /// <param name="builder">The target <see cref="ImmutableArrayBuilder{T}"/> instance.</param>
    static void AppendFullyQualifiedMetadataName(this ISymbol symbol, in ImmutableArrayBuilder<char> builder)
    {
        static void BuildFrom(ISymbol? symbol, in ImmutableArrayBuilder<char> builder)
        {
            switch (symbol)
            {
                // Namespaces that are nested also append a leading '.'
                case INamespaceSymbol { ContainingNamespace.IsGlobalNamespace: false }:
                    BuildFrom(symbol.ContainingNamespace, builder);
                    builder.Add('.');
                    builder.AddRange(symbol.MetadataName.AsSpan());
                    break;

                // Other namespaces (ie. the one right before global) skip the leading '.'
                case INamespaceSymbol { IsGlobalNamespace: false }:
                // Types with no namespace just have their metadata name directly written
                case ITypeSymbol { ContainingSymbol: INamespaceSymbol { IsGlobalNamespace: true } }:
                    builder.AddRange(symbol.MetadataName.AsSpan());
                    break;

                // Types with a containing non-global namespace also append a leading '.'
                case ITypeSymbol { ContainingSymbol: INamespaceSymbol namespaceSymbol }:
                    BuildFrom(namespaceSymbol, builder);
                    builder.Add('.');
                    builder.AddRange(symbol.MetadataName.AsSpan());
                    break;

                // Nested types append a leading '+'
                case ITypeSymbol { ContainingSymbol: ITypeSymbol typeSymbol }:
                    BuildFrom(typeSymbol, builder);
                    builder.Add('+');
                    builder.AddRange(symbol.MetadataName.AsSpan());
                    break;
            }
        }

        BuildFrom(symbol, builder);
    }
}
#endif
