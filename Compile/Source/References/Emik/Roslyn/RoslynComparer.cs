// SPDX-License-Identifier: MPL-2.0
#if ROSLYN
// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Strict value-based equality for symbol comparison.</summary>
public sealed class RoslynComparer
    : IEqualityComparer<CustomModifier?>, IEqualityComparer<ISymbol?>, IEqualityComparer<SyntaxReference?>
{
    /// <summary>Provides the signature for value-based equality in comparisons.</summary>
    /// <typeparam name="TOutput">The type of comparison to get.</typeparam>
    /// <param name="that">The recursive function.</param>
    /// <returns>The comparison function.</returns>
    public delegate Func<TOutput, TOutput, bool> Equal<in TOutput>(RoslynComparer that);

    /// <summary>Provides the signature for recursive hashing.</summary>
    /// <typeparam name="T">The type to hash.</typeparam>
    /// <param name="that">The recursive function.</param>
    /// <returns>The hashing function.</returns>
    public delegate Converter<T, int> Hash<in T>(RoslynComparer that);

    readonly Func<ISymbol, ISymbol, bool> _onSymbol;

    readonly Func<IAliasSymbol, IAliasSymbol, bool> _onAliasSymbol;

    readonly Func<IAssemblySymbol, IAssemblySymbol, bool> _onAssemblySymbol;

    readonly Func<IDiscardSymbol, IDiscardSymbol, bool> _onDiscardSymbol;

    readonly Func<IEventSymbol, IEventSymbol, bool> _onEventSymbol;

    readonly Func<IFieldSymbol, IFieldSymbol, bool> _onFieldSymbol;

    readonly Func<ILabelSymbol, ILabelSymbol, bool> _onLabelSymbol;

    readonly Func<ILocalSymbol, ILocalSymbol, bool> _onLocalSymbol;

    readonly Func<IMethodSymbol, IMethodSymbol, bool> _onMethodSymbol;

    readonly Func<IModuleSymbol, IModuleSymbol, bool> _onModuleSymbol;

    readonly Func<INamespaceOrTypeSymbol, INamespaceOrTypeSymbol, bool> _onNamespaceOrTypeSymbol;

    readonly Func<IParameterSymbol, IParameterSymbol, bool> _onParameterSymbol;

    readonly Func<IPreprocessingSymbol, IPreprocessingSymbol, bool> _onPreprocessingSymbol;

    readonly Func<IPropertySymbol, IPropertySymbol, bool> _onPropertySymbol;

    readonly Func<IRangeVariableSymbol, IRangeVariableSymbol, bool> _onRangeVariableSymbol;

    readonly Func<ISourceAssemblySymbol, ISourceAssemblySymbol, bool> _onSourceAssemblySymbol;

    readonly Func<INamespaceSymbol, INamespaceSymbol, bool> _onNamespaceSymbol;

    readonly Func<ITypeSymbol, ITypeSymbol, bool> _onTypeSymbol;

    readonly Func<IArrayTypeSymbol, IArrayTypeSymbol, bool> _onArrayTypeSymbol;

    readonly Func<IDynamicTypeSymbol, IDynamicTypeSymbol, bool> _onDynamicTypeSymbol;

    readonly Func<IFunctionPointerTypeSymbol, IFunctionPointerTypeSymbol, bool> _onFunctionPointerTypeSymbol;

    readonly Func<INamedTypeSymbol, INamedTypeSymbol, bool> _onNamedTypeSymbol;

    readonly Func<IPointerTypeSymbol, IPointerTypeSymbol, bool> _onPointerTypeSymbol;

    readonly Func<ITypeParameterSymbol, ITypeParameterSymbol, bool> _onTypeParameterSymbol;

    readonly Func<IErrorTypeSymbol, IErrorTypeSymbol, bool> _onErrorTypeSymbol;

    readonly Func<CustomModifier, CustomModifier, bool> _onCustomModifier;

    readonly Func<SyntaxReference, SyntaxReference, bool> _onSyntaxReference;

    readonly Converter<ISymbol, int> _onSymbolHash;

    readonly Converter<CustomModifier, int> _onCustomModifierHash;

    readonly Converter<SyntaxReference, int> _onSyntaxReferenceHash;

    /// <summary>Strict value-based equality for symbol comparison.</summary>
    public RoslynComparer(
        Equal<ISymbol>? onSymbol = null,
        Equal<IAliasSymbol>? onAliasSymbol = null,
        Equal<IAssemblySymbol>? onAssemblySymbol = null,
        Equal<IDiscardSymbol>? onDiscardSymbol = null,
        Equal<IEventSymbol>? onEventSymbol = null,
        Equal<IFieldSymbol>? onFieldSymbol = null,
        Equal<ILabelSymbol>? onLabelSymbol = null,
        Equal<ILocalSymbol>? onLocalSymbol = null,
        Equal<IMethodSymbol>? onMethodSymbol = null,
        Equal<IModuleSymbol>? onModuleSymbol = null,
        Equal<INamespaceOrTypeSymbol>? onNamespaceOrTypeSymbol = null,
        Equal<IParameterSymbol>? onParameterSymbol = null,
        Equal<IPreprocessingSymbol>? onPreprocessingSymbol = null,
        Equal<IPropertySymbol>? onPropertySymbol = null,
        Equal<IRangeVariableSymbol>? onRangeVariableSymbol = null,
        Equal<ISourceAssemblySymbol>? onSourceAssemblySymbol = null,
        Equal<INamespaceSymbol>? onNamespaceSymbol = null,
        Equal<ITypeSymbol>? onTypeSymbol = null,
        Equal<IArrayTypeSymbol>? onArrayTypeSymbol = null,
        Equal<IDynamicTypeSymbol>? onDynamicTypeSymbol = null,
        Equal<IFunctionPointerTypeSymbol>? onFunctionPointerTypeSymbol = null,
        Equal<INamedTypeSymbol>? onNamedTypeSymbol = null,
        Equal<IPointerTypeSymbol>? onPointerTypeSymbol = null,
        Equal<ITypeParameterSymbol>? onTypeParameterSymbol = null,
        Equal<IErrorTypeSymbol>? onErrorTypeSymbol = null,
        Equal<CustomModifier>? onCustomModifier = null,
        Equal<SyntaxReference>? onSyntaxReference = null,
        Hash<ISymbol>? onSymbolHash = null,
        Hash<CustomModifier>? onCustomModifierHash = null,
        Hash<SyntaxReference>? onSyntaxReferenceHash = null
    )
    {
        _onSymbol = onSymbol?.Invoke(this) ?? True;
        _onAliasSymbol = onAliasSymbol?.Invoke(this) ?? True;
        _onAssemblySymbol = onAssemblySymbol?.Invoke(this) ?? True;
        _onDiscardSymbol = onDiscardSymbol?.Invoke(this) ?? True;
        _onEventSymbol = onEventSymbol?.Invoke(this) ?? True;
        _onFieldSymbol = onFieldSymbol?.Invoke(this) ?? True;
        _onLabelSymbol = onLabelSymbol?.Invoke(this) ?? True;
        _onLocalSymbol = onLocalSymbol?.Invoke(this) ?? True;
        _onMethodSymbol = onMethodSymbol?.Invoke(this) ?? True;
        _onModuleSymbol = onModuleSymbol?.Invoke(this) ?? True;
        _onNamespaceOrTypeSymbol = onNamespaceOrTypeSymbol?.Invoke(this) ?? True;
        _onParameterSymbol = onParameterSymbol?.Invoke(this) ?? True;
        _onPreprocessingSymbol = onPreprocessingSymbol?.Invoke(this) ?? True;
        _onPropertySymbol = onPropertySymbol?.Invoke(this) ?? True;
        _onRangeVariableSymbol = onRangeVariableSymbol?.Invoke(this) ?? True;
        _onSourceAssemblySymbol = onSourceAssemblySymbol?.Invoke(this) ?? True;
        _onNamespaceSymbol = onNamespaceSymbol?.Invoke(this) ?? True;
        _onTypeSymbol = onTypeSymbol?.Invoke(this) ?? True;
        _onArrayTypeSymbol = onArrayTypeSymbol?.Invoke(this) ?? True;
        _onDynamicTypeSymbol = onDynamicTypeSymbol?.Invoke(this) ?? True;
        _onFunctionPointerTypeSymbol = onFunctionPointerTypeSymbol?.Invoke(this) ?? True;
        _onNamedTypeSymbol = onNamedTypeSymbol?.Invoke(this) ?? True;
        _onPointerTypeSymbol = onPointerTypeSymbol?.Invoke(this) ?? True;
        _onTypeParameterSymbol = onTypeParameterSymbol?.Invoke(this) ?? True;
        _onErrorTypeSymbol = onErrorTypeSymbol?.Invoke(this) ?? True;
        _onCustomModifier = onCustomModifier?.Invoke(this) ?? True;
        _onSyntaxReference = onSyntaxReference?.Invoke(this) ?? True;
        _onSymbolHash = onSymbolHash?.Invoke(this) ?? Zero;
        _onCustomModifierHash = onCustomModifierHash?.Invoke(this) ?? Zero;
        _onSyntaxReferenceHash = onSyntaxReferenceHash?.Invoke(this) ?? Zero;
    }

    /// <summary>Gets the instance for a comparison behaving identically to <see cref="SymbolComparer"/>.</summary>
    [Pure]
    public static RoslynComparer Gu { get; } = new(
        _ => (x, y) => x.Kind == y.Kind,
        onAssemblySymbol: _ => (x, y) => x.Identity == y.Identity,
        onEventSymbol: r => (x, y) => x.MetadataName == y.MetadataName &&
            r.Equals(x.ContainingType, y.ContainingType) &&
            r.Equals(x.Type, y.Type),
        onFieldSymbol: r => (x, y) => x.MetadataName == y.MetadataName && r.Equals(x.ContainingType, y.ContainingType),
        onLocalSymbol: r => (x, y) =>
            x.MetadataName == y.MetadataName && r.Equals(x.ContainingSymbol, y.ContainingSymbol),
        onMethodSymbol: r => (x, y) => x.MetadataName == y.MetadataName &&
            r.Equals(x.ContainingType, y.ContainingType) &&
            x.Parameters.SequenceEqual(y.Parameters, r),
        onParameterSymbol: _ => (x, y) => x.MetadataName == y.MetadataName,
        onPropertySymbol: r => (x, y) => x.MetadataName == y.MetadataName &&
            r.Equals(x.ContainingType, y.ContainingType) &&
            r.Equals(x.Type, y.Type),
        onNamespaceSymbol: r => (x, y) =>
            x.MetadataName == y.MetadataName && r.Equals(x.ContainingNamespace, y.ContainingNamespace),
        onTypeSymbol: r => (x, y) => x.MetadataName == y.MetadataName &&
            r.Equals(x.ContainingNamespace, y.ContainingNamespace) &&
            (!x.IsReferenceType || x.MatchesNullableAnnotation(y)),
        onNamedTypeSymbol: r => (x, y) => x.TypeArguments.GuardedSequenceEqual(y.TypeArguments, r)
    );

    /// <summary>Gets the instance for comparing signatures.</summary>
    [Pure]
    public static RoslynComparer Signature { get; } = new(
        r => (x, y) => x.Kind == y.Kind &&
            x.MetadataName == y.MetadataName &&
            r.Equals(x.ContainingType, y.ContainingType) &&
            r.Equals(x.ContainingNamespace, y.ContainingNamespace) &&
            r.Equals(x.ToUnderlying(), y.ToUnderlying()),
        onMethodSymbol: r => (x, y) => x.Parameters.SequenceEqual(y.Parameters, r) &&
            x.TypeArguments.SequenceEqual(y.TypeArguments, r) &&
            x.TypeParameters.SequenceEqual(y.TypeParameters, r),
        onPropertySymbol: r => (x, y) => x.Parameters.SequenceEqual(y.Parameters, r),
        onTypeSymbol: _ => IncludedSyntaxNodeRegistrant.MatchesNullableAnnotation,
        onNamedTypeSymbol: r => (x, y) =>
            x.TypeArguments.SequenceEqual(y.TypeArguments, r) && x.TypeParameters.SequenceEqual(y.TypeParameters, r)
    );

    /// <summary>Gets the instance for comparing as strict as value-based comparisons go.</summary>
    [Pure]
    public static RoslynComparer Strict { get; } = new(
        r => (x, y) => x.Kind == y.Kind &&
            x.Name == y.Name &&
            x.IsExtern == y.IsExtern &&
            x.IsSealed == y.IsSealed &&
            x.IsStatic == y.IsStatic &&
            x.Language == y.Language &&
            x.IsVirtual == y.IsVirtual &&
            x.IsAbstract == y.IsAbstract &&
            x.IsOverride == y.IsOverride &&
            x.MetadataName == y.MetadataName &&
            x.IsDefinition == y.IsDefinition &&
            x.MetadataToken == y.MetadataToken &&
            x.IsImplicitlyDeclared == y.IsImplicitlyDeclared &&
            x.CanBeReferencedByName == y.CanBeReferencedByName &&
            x.DeclaredAccessibility == y.DeclaredAccessibility &&
            x.HasUnsupportedMetadata == y.HasUnsupportedMetadata &&
            r.Equals(x.ContainingType, y.ContainingType) &&
            r.Equals(x.ContainingModule, y.ContainingModule) &&
            r.Equals(x.ContainingAssembly, y.ContainingAssembly) &&
            r.Equals(x.ContainingNamespace, y.ContainingNamespace) &&
            x.Locations.GuardedSequenceEqual(y.Locations) &&
            x.DeclaringSyntaxReferences.GuardedSequenceEqual(y.DeclaringSyntaxReferences, r),
        r => (x, y) => r.Equals(x.Target, y.Target),
        r => (x, y) => x.IsInteractive == y.IsInteractive &&
            x.Identity == y.Identity &&
            r.Equals(x.GlobalNamespace, y.GlobalNamespace),
        r => (x, y) => x.NullableAnnotation == y.NullableAnnotation && r.Equals(x, y),
        r => (x, y) => x.NullableAnnotation == y.NullableAnnotation &&
            x.IsWindowsRuntimeEvent == y.IsWindowsRuntimeEvent &&
            r.Equals(x.Type, y.Type) &&
            r.Equals(x.AddMethod, y.AddMethod) &&
            r.Equals(x.RaiseMethod, y.RaiseMethod) &&
            r.Equals(x.RemoveMethod, y.RemoveMethod) &&
            r.Equals(x.OverriddenEvent, y.OverriddenEvent) &&
            x.ExplicitInterfaceImplementations.GuardedSequenceEqual(y.ExplicitInterfaceImplementations, r),
        r => (x, y) => x.IsConst == y.IsConst &&
            x.RefKind == y.RefKind &&
            x.FixedSize == y.FixedSize &&
            x.IsReadOnly == y.IsReadOnly &&
            x.IsRequired == y.IsRequired &&
            x.IsVolatile == y.IsVolatile &&
            x.HasConstantValue == y.HasConstantValue &&
            x.IsFixedSizeBuffer == y.IsFixedSizeBuffer &&
            x.NullableAnnotation == y.NullableAnnotation &&
            (x.ConstantValue?.Equals(y.ConstantValue) ?? y.ConstantValue is null) &&
            r.Equals(x.Type, y.Type) &&
            r.Equals(x.AssociatedSymbol, y.AssociatedSymbol) &&
            r.Equals(x.CorrespondingTupleField, y.CorrespondingTupleField) &&
            x.CustomModifiers.GuardedSequenceEqual(y.CustomModifiers, r) &&
            x.RefCustomModifiers.GuardedSequenceEqual(y.RefCustomModifiers, r),
        r => (x, y) => r.Equals(x.ContainingMethod, y.ContainingMethod),
        r => (x, y) => x.IsRef == y.IsRef &&
            x.IsConst == y.IsConst &&
            x.IsFixed == y.IsFixed &&
            x.IsUsing == y.IsUsing &&
            x.RefKind == y.RefKind &&
            x.IsForEach == y.IsForEach &&
            x.ScopedKind == y.ScopedKind &&
            x.IsFunctionValue == y.IsFunctionValue &&
            x.HasConstantValue == y.HasConstantValue &&
            x.NullableAnnotation == y.NullableAnnotation &&
            (x.ConstantValue?.Equals(y.ConstantValue) ?? y.ConstantValue is null) &&
            r.Equals(x.Type, y.Type),
        r => (x, y) => x.Arity == y.Arity &&
            x.IsAsync == y.IsAsync &&
            x.RefKind == y.RefKind &&
            x.IsVararg == y.IsVararg &&
            x.IsInitOnly == y.IsInitOnly &&
            x.IsReadOnly == y.IsReadOnly &&
            x.MethodKind == y.MethodKind &&
            x.ReturnsVoid == y.ReturnsVoid &&
            x.ReturnsByRef == y.ReturnsByRef &&
            x.IsConditional == y.IsConditional &&
            x.IsGenericMethod == y.IsGenericMethod &&
            x.IsCheckedBuiltin == y.IsCheckedBuiltin &&
            x.CallingConvention == y.CallingConvention &&
            x.IsExtensionMethod == y.IsExtensionMethod &&
            x.IsPartialDefinition == y.IsPartialDefinition &&
            x.ReturnsByRefReadonly == y.ReturnsByRefReadonly &&
            x.HidesBaseMethodsByName == y.HidesBaseMethodsByName &&
            x.ReturnNullableAnnotation == y.ReturnNullableAnnotation &&
            x.ReceiverNullableAnnotation == y.ReceiverNullableAnnotation &&
            x.MethodImplementationFlags == y.MethodImplementationFlags &&
            r.Equals(x.ReturnType, y.ReturnType) &&
            r.Equals(x.AssociatedSymbol, y.AssociatedSymbol) &&
            r.Equals(x.ReducedFrom, y.ReducedFrom) &&
            r.Equals(x.ReceiverType, y.ReceiverType) &&
            r.Equals(x.ConstructedFrom, y.ConstructedFrom) &&
            r.Equals(x.OverriddenMethod, y.OverriddenMethod) &&
            r.Equals(x.PartialDefinitionPart, y.PartialDefinitionPart) &&
            r.Equals(x.PartialImplementationPart, y.PartialImplementationPart) &&
            r.Equals(x.AssociatedAnonymousDelegate, y.AssociatedAnonymousDelegate) &&
            x.TypeArgumentNullableAnnotations.GuardedSequenceEqual(y.TypeArgumentNullableAnnotations) &&
            x.RefCustomModifiers.GuardedSequenceEqual(y.RefCustomModifiers, r) &&
            x.ReturnTypeCustomModifiers.GuardedSequenceEqual(y.ReturnTypeCustomModifiers, r) &&
            x.Parameters.GuardedSequenceEqual(y.Parameters, r) &&
            x.TypeArguments.GuardedSequenceEqual(y.TypeArguments, r) &&
            x.UnmanagedCallingConventionTypes.GuardedSequenceEqual(y.UnmanagedCallingConventionTypes, r) &&
            x.ExplicitInterfaceImplementations.GuardedSequenceEqual(y.ExplicitInterfaceImplementations, r),
        r => (x, y) => r.Equals(x.GlobalNamespace, y.GlobalNamespace) &&
            x.ReferencedAssemblies.GuardedSequenceEqual(y.ReferencedAssemblies) &&
            x.ReferencedAssemblySymbols.GuardedSequenceEqual(y.ReferencedAssemblySymbols, r),
        _ => (x, y) => x.IsNamespace == y.IsNamespace && x.IsType == y.IsType,
        r => (x, y) => x.IsThis == y.IsThis &&
            x.Ordinal == y.Ordinal &&
            x.RefKind == y.RefKind &&
            x.IsParams == y.IsParams &&
            x.IsDiscard == y.IsDiscard &&
            x.IsOptional == y.IsOptional &&
            x.ScopedKind == y.ScopedKind &&
            x.NullableAnnotation == y.NullableAnnotation &&
            x.HasExplicitDefaultValue == y.HasExplicitDefaultValue &&
            (x.ExplicitDefaultValue?.Equals(y.ExplicitDefaultValue) ?? y.ExplicitDefaultValue is null) &&
            r.Equals(x.Type, y.Type) &&
            x.CustomModifiers.GuardedSequenceEqual(y.CustomModifiers, r) &&
            x.RefCustomModifiers.GuardedSequenceEqual(y.RefCustomModifiers, r),
        null,
        r => (x, y) => x.RefKind == y.RefKind &&
            x.IsIndexer == y.IsIndexer &&
            x.IsReadOnly == y.IsReadOnly &&
            x.IsRequired == y.IsRequired &&
            x.IsWriteOnly == y.IsWriteOnly &&
            x.IsWithEvents == y.IsWithEvents &&
            x.ReturnsByRef == y.ReturnsByRef &&
            x.NullableAnnotation == y.NullableAnnotation &&
            x.ReturnsByRefReadonly == y.ReturnsByRefReadonly &&
            r.Equals(x.Type, y.Type) &&
            r.Equals(x.GetMethod, y.GetMethod) &&
            r.Equals(x.SetMethod, y.SetMethod) &&
            r.Equals(x.OverriddenProperty, y.OverriddenProperty) &&
            x.RefCustomModifiers.GuardedSequenceEqual(y.RefCustomModifiers, r) &&
            x.TypeCustomModifiers.GuardedSequenceEqual(y.TypeCustomModifiers, r) &&
            x.ExplicitInterfaceImplementations.GuardedSequenceEqual(y.ExplicitInterfaceImplementations, r) &&
            x.Parameters.GuardedSequenceEqual(y.Parameters, r),
        null,
        null,
        _ => (x, y) => x.IsGlobalNamespace == y.IsGlobalNamespace && x.NamespaceKind == y.NamespaceKind,
        r => (x, y) => x.Kind == y.Kind &&
            x.IsRecord == y.IsRecord &&
            x.TypeKind == y.TypeKind &&
            x.IsReadOnly == y.IsReadOnly &&
            x.IsTupleType == y.IsTupleType &&
            x.IsValueType == y.IsValueType &&
            x.SpecialType == y.SpecialType &&
            x.IsRefLikeType == y.IsRefLikeType &&
            x.IsAnonymousType == y.IsAnonymousType &&
            x.IsReferenceType == y.IsReferenceType &&
            x.IsUnmanagedType == y.IsUnmanagedType &&
            x.IsNativeIntegerType == y.IsNativeIntegerType &&
            r.Equals(x.BaseType, y.BaseType) &&
            x.Interfaces.GuardedSequenceEqual(y.Interfaces, r) &&
            x.AllInterfaces.GuardedSequenceEqual(y.AllInterfaces, r),
        r => (x, y) => x.IsSZArray == y.IsSZArray &&
            x.Rank == y.Rank &&
            x.ElementNullableAnnotation == y.ElementNullableAnnotation &&
            r.Equals(x.ElementType, y.ElementType) &&
            x.Sizes.GuardedSequenceEqual(y.Sizes) &&
            x.LowerBounds.GuardedSequenceEqual(y.LowerBounds) &&
            x.CustomModifiers.GuardedSequenceEqual(y.CustomModifiers, r),
        null,
        r => (x, y) => r.Equals(x.Signature, y.Signature),
        r => (x, y) => x.Arity == y.Arity &&
            x.IsComImport == y.IsComImport &&
            x.IsFileLocal == y.IsFileLocal &&
            x.IsGenericType == y.IsGenericType &&
            x.IsScriptClass == y.IsScriptClass &&
            x.IsSerializable == y.IsSerializable &&
            x.IsImplicitClass & y.IsImplicitClass &&
            x.IsUnboundGenericType == y.IsUnboundGenericType &&
            x.MightContainExtensionMethods == y.MightContainExtensionMethods &&
            r.Equals(x.AssociatedSymbol, y.AssociatedSymbol) &&
            r.Equals(x.EnumUnderlyingType, y.EnumUnderlyingType) &&
            r.Equals(x.TupleUnderlyingType, y.TupleUnderlyingType) &&
            r.Equals(x.DelegateInvokeMethod, y.DelegateInvokeMethod) &&
            r.Equals(x.NativeIntegerUnderlyingType, y.NativeIntegerUnderlyingType) &&
            x.TypeArgumentNullableAnnotations.GuardedSequenceEqual(y.TypeArgumentNullableAnnotations) &&
            x.TupleElements.GuardedSequenceEqual(y.TupleElements, r) &&
            x.TypeArguments.GuardedSequenceEqual(y.TypeArguments, r) &&
            x.TypeParameters.GuardedSequenceEqual(y.TypeParameters, r),
        r => (x, y) => r.Equals(x.PointedAtType, y.PointedAtType) &&
            x.CustomModifiers.GuardedSequenceEqual(y.CustomModifiers, r),
        r => (x, y) => x.Ordinal == y.Ordinal &&
            x.Variance == y.Variance &&
            x.TypeParameterKind == y.TypeParameterKind &&
            x.HasNotNullConstraint == y.HasNotNullConstraint &&
            x.HasValueTypeConstraint == y.HasValueTypeConstraint &&
            x.HasConstructorConstraint == y.HasConstructorConstraint &&
            x.HasReferenceTypeConstraint == y.HasReferenceTypeConstraint &&
            x.HasUnmanagedTypeConstraint == y.HasUnmanagedTypeConstraint &&
            x.ReferenceTypeConstraintNullableAnnotation == y.ReferenceTypeConstraintNullableAnnotation &&
            r.Equals(x.ReducedFrom, y.ReducedFrom) &&
            r.Equals(x.DeclaringType, y.DeclaringType) &&
            r.Equals(x.DeclaringMethod, y.DeclaringMethod) &&
            x.ConstraintNullableAnnotations.GuardedSequenceEqual(y.ConstraintNullableAnnotations) &&
            x.ConstraintTypes.GuardedSequenceEqual(y.ConstraintTypes, r),
        r => (x, y) => x.CandidateReason == y.CandidateReason &&
            x.CandidateSymbols.GuardedSequenceEqual(y.CandidateSymbols, r),
        r => (x, y) => x.IsOptional == y.IsOptional && r.Equals(x.Modifier, y.Modifier),
        _ => (x, y) => x.Span == y.Span && x.SyntaxTree.IsEquivalentTo(y.SyntaxTree),
        _ => BetterHashCode,
        r => x => x.IsOptional.ToByte() * Prime() ^ r.GetHashCode(x.Modifier),
        _ => x => x.Span.GetHashCode()
    );

    /// <inheritdoc />
    [Pure]
    public bool Equals(CustomModifier? x, CustomModifier? y) =>
        ReferenceEquals(x, y) || x is not null && y is not null && _onCustomModifier(x, y);

    /// <inheritdoc />
    [Pure]
    public bool Equals(ISymbol? x, ISymbol? y) =>
        ReferenceEquals(x, y) ||
        x is not null &&
        y is not null &&
        _onSymbol(x, y) &&
        Eq(x, y, _onAliasSymbol) &&
        Eq(x, y, _onAssemblySymbol) &&
        Eq(x, y, _onDiscardSymbol) &&
        Eq(x, y, _onEventSymbol) &&
        Eq(x, y, _onFieldSymbol) &&
        Eq(x, y, _onLabelSymbol) &&
        Eq(x, y, _onLocalSymbol) &&
        Eq(x, y, _onMethodSymbol) &&
        Eq(x, y, _onModuleSymbol) &&
        Eq(x, y, _onNamespaceOrTypeSymbol) &&
        Eq(x, y, _onParameterSymbol) &&
        Eq(x, y, _onPreprocessingSymbol) &&
        Eq(x, y, _onPropertySymbol) &&
        Eq(x, y, _onRangeVariableSymbol) &&
        Eq(x, y, _onSourceAssemblySymbol) &&
        Eq(x, y, _onNamespaceSymbol) &&
        Eq(x, y, _onTypeSymbol) &&
        Eq(x, y, _onArrayTypeSymbol) &&
        Eq(x, y, _onDynamicTypeSymbol) &&
        Eq(x, y, _onFunctionPointerTypeSymbol) &&
        Eq(x, y, _onNamedTypeSymbol) &&
        Eq(x, y, _onPointerTypeSymbol) &&
        Eq(x, y, _onTypeParameterSymbol) &&
        Eq(x, y, _onErrorTypeSymbol);

    /// <inheritdoc />
    public bool Equals(SyntaxReference? x, SyntaxReference? y) =>
        ReferenceEquals(x, y) || x is not null && y is not null && _onSyntaxReference(x, y);

    /// <inheritdoc />
    [Pure]
    public int GetHashCode(CustomModifier? obj) => obj is null ? 0 : _onCustomModifierHash(obj);

    /// <inheritdoc />
    [Pure]
    public int GetHashCode(ISymbol? obj) => obj is null ? 0 : _onSymbolHash.Invoke(obj);

    /// <inheritdoc />
    [Pure]
    public int GetHashCode(SyntaxReference? obj) => obj is null ? 0 : _onSyntaxReferenceHash(obj);

    [Pure]
    static bool Eq<T>(ISymbol x, ISymbol y, Func<T, T, bool> predicate)
        where T : ISymbol =>
        x is T tx && y is T ty && predicate(tx, ty) || x is not T && y is not T;

    [Pure]
    static bool True<T>(T _, T __) => true;

    [Pure]
    static int BetterHashCode(ISymbol x)
    {
        int hash = Prime();

        for (var obj = Unsafe.As<ISymbol?>(x); obj is not null; obj = obj.ContainingSymbol)
        {
            hash ^= unchecked(obj.Kind.AsInt() * Prime());
            hash ^= unchecked(obj.MetadataToken * Prime());
            hash ^= unchecked(obj.DeclaredAccessibility.AsInt() * Prime());
            hash ^= unchecked(StringComparer.Ordinal.GetHashCode(obj.Name) * Prime());
            hash ^= unchecked(StringComparer.Ordinal.GetHashCode(obj.Language) * Prime());
            hash ^= unchecked(StringComparer.Ordinal.GetHashCode(obj.MetadataName) * Prime());
        }

        return hash;
    }

    [Pure]
    static int Zero<T>(T _) => 0;
}
#endif
