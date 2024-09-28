// SPDX-License-Identifier: MPL-2.0
#if ROSLYN
// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Strict value-based equality for symbol comparison.</summary>
public sealed class RoslynComparer : IEqualityComparer<AssemblyIdentity?>,
    IEqualityComparer<CustomModifier?>,
    IEqualityComparer<ISymbol?>
{
    RoslynComparer() { }

    /// <summary>Gets the default instance.</summary>
    [Pure]
    public static RoslynComparer Instance { get; } = new();

    /// <summary>Performs the strict value-based equality on the symbols.</summary>
    /// <param name="x">The left-hand side.</param>
    /// <param name="y">The right-hand side.</param>
    /// <returns>
    /// The value <see langword="true"/> if the parameters <paramref name="x"/>
    /// and the parameters <paramref name="y"/> have the same values.
    /// </returns>
    [Pure]
    public static bool Eq(ISymbol? x, ISymbol? y) =>
        ReferenceEquals(x, y) ||
        x is not null &&
        y is not null &&
        x.Kind == y.Kind &&
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
        Eq(x.ContainingType, y.ContainingType) &&
        Eq(x.ContainingModule, y.ContainingModule) &&
        Eq(x.ContainingAssembly, y.ContainingAssembly) &&
        Eq(x.ContainingNamespace, y.ContainingNamespace) &&
        x.Locations.GuardedSequenceEqual(y.Locations) &&
        x.DeclaringSyntaxReferences.GuardedSequenceEqual(y.DeclaringSyntaxReferences, Eq) &&
        Eq<IAliasSymbol>(x, y, Eq) &&
        Eq<IAssemblySymbol>(x, y, Eq) &&
        Eq<IDiscardSymbol>(x, y, Eq) &&
        Eq<IEventSymbol>(x, y, Eq) &&
        Eq<IFieldSymbol>(x, y, Eq) &&
        Eq<ILabelSymbol>(x, y, Eq) &&
        Eq<ILocalSymbol>(x, y, Eq) &&
        Eq<IMethodSymbol>(x, y, Eq) &&
        Eq<IModuleSymbol>(x, y, Eq) &&
        Eq<INamespaceOrTypeSymbol>(x, y, Eq) &&
        Eq<IParameterSymbol>(x, y, Eq) &&
        Eq<IPreprocessingSymbol>(x, y, Eq) &&
        Eq<IPropertySymbol>(x, y, Eq) &&
        Eq<IRangeVariableSymbol>(x, y, Eq) &&
        Eq<ISourceAssemblySymbol>(x, y, Eq) &&
        Eq<INamespaceSymbol>(x, y, Eq) &&
        Eq<ITypeSymbol>(x, y, Eq) &&
        Eq<IArrayTypeSymbol>(x, y, Eq) &&
        Eq<IDynamicTypeSymbol>(x, y, Eq) &&
        Eq<IFunctionPointerTypeSymbol>(x, y, Eq) &&
        Eq<INamedTypeSymbol>(x, y, Eq) &&
        Eq<IPointerTypeSymbol>(x, y, Eq) &&
        Eq<ITypeParameterSymbol>(x, y, Eq) &&
        Eq<IErrorTypeSymbol>(x, y, Eq);

    /// <summary>Returns the hash code for this string.</summary>
    /// <param name="obj">The instance.</param>
    /// <returns>A 32-bit signed integer hash code.</returns>
    [Pure]
    public static int Hash(ISymbol? obj)
    {
        int hash = Prime();

        for (var o = obj; o is not null; o = o.ContainingSymbol)
        {
            hash ^= unchecked(o.Kind.AsInt() * Prime());
            hash ^= unchecked(o.MetadataToken * Prime());
            hash ^= unchecked(o.DeclaredAccessibility.AsInt() * Prime());
            hash ^= unchecked(StringComparer.Ordinal.GetHashCode(o.Name) * Prime());
            hash ^= unchecked(StringComparer.Ordinal.GetHashCode(o.Language) * Prime());
            hash ^= unchecked(StringComparer.Ordinal.GetHashCode(o.MetadataName) * Prime());
        }

        return hash;
    }

    /// <inheritdoc />
    [Pure]
    bool IEqualityComparer<AssemblyIdentity?>.Equals(AssemblyIdentity? x, AssemblyIdentity? y) =>
        ReferenceEquals(x, y) ||
        x is not null &&
        y is not null &&
        x.Name == y.Name &&
        x.Flags == y.Flags &&
        x.Version == y.Version &&
        x.ContentType == y.ContentType &&
        x.CultureName == y.CultureName &&
        x.IsStrongName == y.IsStrongName &&
        x.HasPublicKey == y.HasPublicKey &&
        x.IsRetargetable == y.IsRetargetable &&
        x.PublicKey.GuardedSequenceEqual(y.PublicKey) &&
        x.PublicKeyToken.GuardedSequenceEqual(y.PublicKeyToken);

    /// <inheritdoc />
    [Pure]
    bool IEqualityComparer<CustomModifier?>.Equals(CustomModifier? x, CustomModifier? y) =>
        ReferenceEquals(x, y) ||
        x is not null && y is not null && x.IsOptional == y.IsOptional && Eq(x.Modifier, y.Modifier);

    /// <inheritdoc />
    [Pure]
    bool IEqualityComparer<ISymbol?>.Equals(ISymbol? x, ISymbol? y) => Eq(x, y);

    /// <inheritdoc />
    [Pure]
    int IEqualityComparer<AssemblyIdentity?>.GetHashCode(AssemblyIdentity? obj)
    {
        if (obj is null)
            return Prime();

        var hash = obj.Flags.AsInt();
        hash ^= unchecked(obj.ContentType.AsInt() * Prime());
        hash ^= unchecked(obj.Version.GetHashCode() * Prime());
        hash ^= unchecked(obj.IsStrongName.ToByte() * Prime());
        hash ^= unchecked(obj.HasPublicKey.ToByte() * Prime());
        hash ^= unchecked(obj.IsRetargetable.ToByte() * Prime());
        hash ^= unchecked(StringComparer.Ordinal.GetHashCode(obj.Name) * Prime());
        hash ^= unchecked(StringComparer.Ordinal.GetHashCode(obj.CultureName) * Prime());
        return hash;
    }

    /// <inheritdoc />
    [Pure]
    int IEqualityComparer<CustomModifier?>.GetHashCode(CustomModifier? obj) =>
        Prime() * (obj is null ? 1 : Hash(obj.Modifier) ^ obj.IsOptional.ToByte());

    /// <inheritdoc />
    [Pure]
    int IEqualityComparer<ISymbol?>.GetHashCode(ISymbol? x) => Hash(x);

    [Pure]
    static bool Eq(IAliasSymbol x, IAliasSymbol y) => Eq((ISymbol)x.Target, y.Target);

    [Pure]
    static bool Eq(IArrayTypeSymbol x, IArrayTypeSymbol y) =>
        x.IsSZArray == y.IsSZArray &&
        x.Rank == y.Rank &&
        x.ElementNullableAnnotation == y.ElementNullableAnnotation &&
        x.LowerBounds.GuardedSequenceEqual(y.LowerBounds) &&
        x.CustomModifiers.GuardedSequenceEqual(y.CustomModifiers, Instance) &&
        x.Sizes.GuardedSequenceEqual(y.Sizes) &&
        Eq((ISymbol)x.ElementType, y.ElementType);

    [Pure]
    static bool Eq(IAssemblySymbol x, IAssemblySymbol y) =>
        x.IsInteractive == y.IsInteractive &&
        ((IEqualityComparer<AssemblyIdentity?>)Instance).Equals(x.Identity, y.Identity) &&
        Eq((ISymbol)x.GlobalNamespace, y.GlobalNamespace);

    [Pure]
    static bool Eq(IDiscardSymbol x, IDiscardSymbol y) =>
        x.NullableAnnotation == y.NullableAnnotation == Eq((ISymbol)x.Type, y.Type);

    [Pure]
    static bool Eq(IErrorTypeSymbol x, IErrorTypeSymbol y) =>
        x.CandidateReason == y.CandidateReason && x.CandidateSymbols.GuardedSequenceEqual(y.CandidateSymbols, Instance);

    [Pure]
    static bool Eq(IEventSymbol x, IEventSymbol y) =>
        x.NullableAnnotation == y.NullableAnnotation &&
        x.IsWindowsRuntimeEvent == y.IsWindowsRuntimeEvent &&
        Eq((ISymbol)x.Type, y.Type) &&
        Eq((ISymbol?)x.AddMethod, y.AddMethod) &&
        Eq((ISymbol?)x.RaiseMethod, y.RaiseMethod) &&
        Eq((ISymbol?)x.RemoveMethod, y.RemoveMethod) &&
        Eq((ISymbol?)x.OverriddenEvent, y.OverriddenEvent) &&
        x.ExplicitInterfaceImplementations.GuardedSequenceEqual(y.ExplicitInterfaceImplementations, Instance);

    [Pure]
    static bool Eq(IFieldSymbol x, IFieldSymbol y) =>
        x.IsConst == y.IsConst &&
        x.RefKind == y.RefKind &&
        x.FixedSize == y.FixedSize &&
        x.IsReadOnly == y.IsReadOnly &&
        x.IsRequired == y.IsRequired &&
        x.IsVolatile == y.IsVolatile &&
        x.HasConstantValue == y.HasConstantValue &&
        x.IsFixedSizeBuffer == y.IsFixedSizeBuffer &&
        x.NullableAnnotation == y.NullableAnnotation &&
        (x.ConstantValue?.Equals(y.ConstantValue) ?? y.ConstantValue is null) &&
        Eq((ISymbol)x.Type, y.Type) &&
        Eq(x.AssociatedSymbol, y.AssociatedSymbol) &&
        Eq((ISymbol?)x.CorrespondingTupleField, y.CorrespondingTupleField) &&
        x.CustomModifiers.GuardedSequenceEqual(y.CustomModifiers, Instance) &&
        x.RefCustomModifiers.GuardedSequenceEqual(y.RefCustomModifiers, Instance);

    [Pure]
    static bool Eq(IFunctionPointerTypeSymbol x, IFunctionPointerTypeSymbol y) =>
        Eq((ISymbol)x.Signature, y.Signature);

    [Pure]
    static bool Eq(ILabelSymbol x, ILabelSymbol y) => Eq((ISymbol)x.ContainingMethod, y.ContainingMethod);

    [Pure]
    static bool Eq(ILocalSymbol x, ILocalSymbol y) =>
        x.IsRef == y.IsRef &&
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
        Eq((ISymbol)x.Type, y.Type);

    [Pure]
    static bool Eq(IMethodSymbol x, IMethodSymbol y) =>
        x.Arity == y.Arity &&
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
        Eq((ISymbol)x.ReturnType, y.ReturnType) &&
        Eq(x.AssociatedSymbol, y.AssociatedSymbol) &&
        Eq((ISymbol?)x.ReducedFrom, y.ReducedFrom) &&
        Eq((ISymbol?)x.ReceiverType, y.ReceiverType) &&
        Eq((ISymbol)x.ConstructedFrom, y.ConstructedFrom) &&
        Eq((ISymbol?)x.OverriddenMethod, y.OverriddenMethod) &&
        Eq((ISymbol?)x.PartialDefinitionPart, y.PartialDefinitionPart) &&
        Eq((ISymbol?)x.PartialImplementationPart, y.PartialImplementationPart) &&
        Eq((ISymbol?)x.AssociatedAnonymousDelegate, y.AssociatedAnonymousDelegate) &&
        x.TypeArgumentNullableAnnotations.GuardedSequenceEqual(y.TypeArgumentNullableAnnotations) &&
        x.RefCustomModifiers.GuardedSequenceEqual(y.RefCustomModifiers, Instance) &&
        x.ReturnTypeCustomModifiers.GuardedSequenceEqual(y.ReturnTypeCustomModifiers, Instance) &&
        x.Parameters.GuardedSequenceEqual(y.Parameters, Instance) &&
        x.TypeArguments.GuardedSequenceEqual(y.TypeArguments, Instance) &&
        x.UnmanagedCallingConventionTypes.GuardedSequenceEqual(y.UnmanagedCallingConventionTypes, Instance) &&
        x.ExplicitInterfaceImplementations.GuardedSequenceEqual(y.ExplicitInterfaceImplementations, Instance);

    [Pure]
    static bool Eq(IModuleSymbol x, IModuleSymbol y) =>
        Eq((ISymbol)x.GlobalNamespace, y.GlobalNamespace) &&
        x.ReferencedAssemblies.GuardedSequenceEqual(y.ReferencedAssemblies, Instance) &&
        x.ReferencedAssemblySymbols.GuardedSequenceEqual(y.ReferencedAssemblySymbols, Instance);

    [Pure]
    static bool Eq(INamedTypeSymbol x, INamedTypeSymbol y) =>
        x.Arity == y.Arity &&
        x.IsComImport == y.IsComImport &&
        x.IsFileLocal == y.IsFileLocal &&
        x.IsGenericType == y.IsGenericType &&
        x.IsScriptClass == y.IsScriptClass &&
        x.IsSerializable == y.IsSerializable &&
        x.IsImplicitClass & y.IsImplicitClass &&
        x.IsUnboundGenericType == y.IsUnboundGenericType &&
        x.MightContainExtensionMethods == y.MightContainExtensionMethods &&
        // Eq(x.AssociatedSymbol, y.AssociatedSymbol) &&
        // Eq((ISymbol?)x.EnumUnderlyingType, y.EnumUnderlyingType) &&
        // Eq((ISymbol?)x.TupleUnderlyingType, y.TupleUnderlyingType) &&
        // Eq((ISymbol?)x.DelegateInvokeMethod, y.DelegateInvokeMethod) &&
        // Eq((ISymbol?)x.NativeIntegerUnderlyingType, y.NativeIntegerUnderlyingType) &&
        x.TypeArgumentNullableAnnotations.GuardedSequenceEqual(y.TypeArgumentNullableAnnotations) &&
        x.TupleElements.GuardedSequenceEqual(y.TupleElements, Instance) &&
        x.TypeArguments.GuardedSequenceEqual(y.TypeArguments, Instance) &&
        x.TypeParameters.GuardedSequenceEqual(y.TypeParameters, Instance);

    [Pure]
    static bool Eq(INamespaceOrTypeSymbol x, INamespaceOrTypeSymbol y) =>
        x.IsNamespace == y.IsNamespace && x.IsType == y.IsType;

    [Pure]
    static bool Eq(INamespaceSymbol x, INamespaceSymbol y) =>
        x.IsGlobalNamespace == y.IsGlobalNamespace && x.NamespaceKind == y.NamespaceKind;

    [Pure]
    static bool Eq(IParameterSymbol x, IParameterSymbol y) =>
        x.IsThis == y.IsThis &&
        x.Ordinal == y.Ordinal &&
        x.RefKind == y.RefKind &&
        x.IsParams == y.IsParams &&
        x.IsDiscard == y.IsDiscard &&
        x.IsOptional == y.IsOptional &&
        x.ScopedKind == y.ScopedKind &&
        x.NullableAnnotation == y.NullableAnnotation &&
        x.HasExplicitDefaultValue == y.HasExplicitDefaultValue &&
        (x.ExplicitDefaultValue?.Equals(y.ExplicitDefaultValue) ?? y.ExplicitDefaultValue is null) &&
        Eq((ISymbol)x.Type, y.Type) &&
        x.CustomModifiers.GuardedSequenceEqual(y.CustomModifiers, Instance) &&
        x.RefCustomModifiers.GuardedSequenceEqual(y.RefCustomModifiers, Instance);

    [Pure]
    static bool Eq(IPointerTypeSymbol x, IPointerTypeSymbol y) =>
        Eq((ISymbol)x.PointedAtType, y.PointedAtType) &&
        x.CustomModifiers.GuardedSequenceEqual(y.CustomModifiers, Instance);

    [Pure]
    static bool Eq(IPropertySymbol x, IPropertySymbol y) =>
        x.RefKind == y.RefKind &&
        x.IsIndexer == y.IsIndexer &&
        x.IsReadOnly == y.IsReadOnly &&
        x.IsRequired == y.IsRequired &&
        x.IsWriteOnly == y.IsWriteOnly &&
        x.IsWithEvents == y.IsWithEvents &&
        x.ReturnsByRef == y.ReturnsByRef &&
        x.NullableAnnotation == y.NullableAnnotation &&
        x.ReturnsByRefReadonly == y.ReturnsByRefReadonly &&
        Eq((ISymbol)x.Type, y.Type) &&
        Eq((ISymbol?)x.GetMethod, y.GetMethod) &&
        Eq((ISymbol?)x.SetMethod, y.SetMethod) &&
        Eq((ISymbol?)x.OverriddenProperty, y.OverriddenProperty) &&
        x.RefCustomModifiers.GuardedSequenceEqual(y.RefCustomModifiers, Instance) &&
        x.TypeCustomModifiers.GuardedSequenceEqual(y.TypeCustomModifiers, Instance) &&
        x.ExplicitInterfaceImplementations.GuardedSequenceEqual(y.ExplicitInterfaceImplementations, Instance) &&
        x.Parameters.GuardedSequenceEqual(y.Parameters, Instance);

    [Pure]
    static bool Eq(ITypeParameterSymbol x, ITypeParameterSymbol y) =>
        x.Ordinal == y.Ordinal &&
        x.Variance == y.Variance &&
        x.TypeParameterKind == y.TypeParameterKind &&
        x.HasNotNullConstraint == y.HasNotNullConstraint &&
        x.HasValueTypeConstraint == y.HasValueTypeConstraint &&
        x.HasConstructorConstraint == y.HasConstructorConstraint &&
        x.HasReferenceTypeConstraint == y.HasReferenceTypeConstraint &&
        x.HasUnmanagedTypeConstraint == y.HasUnmanagedTypeConstraint &&
        x.ReferenceTypeConstraintNullableAnnotation == y.ReferenceTypeConstraintNullableAnnotation &&
        Eq((ISymbol?)x.ReducedFrom, y.ReducedFrom) &&
        Eq((ISymbol?)x.DeclaringType, y.DeclaringType) &&
        Eq((ISymbol?)x.DeclaringMethod, y.DeclaringMethod) &&
        x.ConstraintNullableAnnotations.GuardedSequenceEqual(y.ConstraintNullableAnnotations) &&
        x.ConstraintTypes.GuardedSequenceEqual(y.ConstraintTypes, Instance);

    [Pure]
    static bool Eq(ITypeSymbol x, ITypeSymbol y) =>
        x.Kind == y.Kind &&
        x.IsRecord == y.IsRecord &&
        x.IsReadOnly == y.IsReadOnly &&
        x.IsTupleType == y.IsTupleType &&
        x.IsValueType == y.IsValueType &&
        x.SpecialType == y.SpecialType &&
        x.IsRefLikeType == y.IsRefLikeType &&
        x.IsAnonymousType == y.IsAnonymousType &&
        x.IsReferenceType == y.IsReferenceType &&
        x.IsUnmanagedType == y.IsUnmanagedType &&
        x.IsNativeIntegerType == y.IsNativeIntegerType &&
        Eq((ISymbol?)x.BaseType, y.BaseType) &&
        x.Interfaces.GuardedSequenceEqual(y.Interfaces, Instance) &&
        x.AllInterfaces.GuardedSequenceEqual(y.AllInterfaces, Instance);

    [Pure]
    static bool Eq(SyntaxReference? x, SyntaxReference? y) =>
        ReferenceEquals(x, y) ||
        x is not null && y is not null && x.Span == y.Span && x.SyntaxTree.IsEquivalentTo(y.SyntaxTree);

    [Pure]
    static bool Eq<T>(ISymbol? x, ISymbol? y, Func<T, T, bool> predicate)
        where T : ISymbol =>
        x is not T && x is not T || x is T tx && y is T ty && predicate(tx, ty);
}
#endif
