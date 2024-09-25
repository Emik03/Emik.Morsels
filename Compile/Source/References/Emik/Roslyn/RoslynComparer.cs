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

    /// <summary> The default instance. </summary>
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
        (x is not IAliasSymbol && y is not IAliasSymbol ||
            x is IAliasSymbol && y is IAliasSymbol && !Eq(As<IAliasSymbol>(x), As<IAliasSymbol>(y))) &&
        (x is not IAssemblySymbol && y is not IAssemblySymbol ||
            x is IAssemblySymbol && y is IAssemblySymbol && !Eq(As<IAssemblySymbol>(x), As<IAssemblySymbol>(y))) &&
        (x is not IDiscardSymbol && y is not IDiscardSymbol ||
            x is IDiscardSymbol && y is IDiscardSymbol && !Eq(As<IDiscardSymbol>(x), As<IDiscardSymbol>(y))) &&
        (x is not IEventSymbol && y is not IEventSymbol ||
            x is IEventSymbol && y is IEventSymbol && !Eq(As<IEventSymbol>(x), As<IEventSymbol>(y))) &&
        (x is not IFieldSymbol && y is not IFieldSymbol ||
            x is IFieldSymbol && y is IFieldSymbol && !Eq(As<IFieldSymbol>(x), As<IFieldSymbol>(y))) &&
        (x is not ILabelSymbol && y is not ILabelSymbol ||
            x is ILabelSymbol && y is ILabelSymbol && !Eq(As<ILabelSymbol>(x), As<ILabelSymbol>(y))) &&
        (x is not ILocalSymbol && y is not ILocalSymbol ||
            x is ILocalSymbol && y is ILocalSymbol && !Eq(As<ILocalSymbol>(x), As<ILocalSymbol>(y))) &&
        (x is not IMethodSymbol && y is not IMethodSymbol ||
            x is IMethodSymbol && y is IMethodSymbol && !Eq(As<IMethodSymbol>(x), As<IMethodSymbol>(y))) &&
        (x is not IModuleSymbol && y is not IModuleSymbol ||
            x is IModuleSymbol && y is IModuleSymbol && !Eq(As<IModuleSymbol>(x), As<IModuleSymbol>(y))) &&
        (x is not INamespaceOrTypeSymbol && y is not INamespaceOrTypeSymbol ||
            x is INamespaceOrTypeSymbol &&
            y is INamespaceOrTypeSymbol &&
            !Eq(As<INamespaceOrTypeSymbol>(x), As<INamespaceOrTypeSymbol>(y))) &&
        (x is not IParameterSymbol && y is not IParameterSymbol ||
            x is IParameterSymbol && y is IParameterSymbol && !Eq(As<IParameterSymbol>(x), As<IParameterSymbol>(y))) &&
        (x is not IPreprocessingSymbol && y is not IPreprocessingSymbol ||
            x is IPreprocessingSymbol && y is IPreprocessingSymbol) &&
        (x is not IPropertySymbol && y is not IPropertySymbol ||
            x is IPropertySymbol && y is IPropertySymbol && !Eq(As<IPropertySymbol>(x), As<IPropertySymbol>(y))) &&
        (x is not IRangeVariableSymbol && y is not IRangeVariableSymbol ||
            x is IRangeVariableSymbol && y is IRangeVariableSymbol) &&
        (x is not ISourceAssemblySymbol && y is not ISourceAssemblySymbol || // Implements IAssemblySymbol
            x is ISourceAssemblySymbol && y is ISourceAssemblySymbol) &&
        (x is not INamespaceSymbol && y is not INamespaceSymbol || // Implements INamespaceOrTypeSymbol
            x is INamespaceSymbol && y is INamespaceSymbol && !Eq(As<INamespaceSymbol>(x), As<INamespaceSymbol>(y))) &&
        (x is not ITypeSymbol && y is not ITypeSymbol ||
            x is ITypeSymbol && y is ITypeSymbol && !Eq(As<ITypeSymbol>(x), As<ITypeSymbol>(y))) &&
        (x is not IArrayTypeSymbol && y is not IArrayTypeSymbol || // Implements ITypeSymbol
            x is IArrayTypeSymbol && y is IArrayTypeSymbol && !Eq(As<IArrayTypeSymbol>(x), As<IArrayTypeSymbol>(y))) &&
        (x is not IDynamicTypeSymbol && y is not IDynamicTypeSymbol ||
            x is IDynamicTypeSymbol && y is IDynamicTypeSymbol) &&
        (x is not IFunctionPointerTypeSymbol && y is not IFunctionPointerTypeSymbol ||
            x is IFunctionPointerTypeSymbol &&
            y is IFunctionPointerTypeSymbol &&
            !Eq(As<IFunctionPointerTypeSymbol>(x), As<IFunctionPointerTypeSymbol>(y))) &&
        (x is not INamedTypeSymbol && y is not INamedTypeSymbol ||
            x is INamedTypeSymbol && y is INamedTypeSymbol && !Eq(As<INamedTypeSymbol>(x), As<INamedTypeSymbol>(y))) &&
        (x is not IPointerTypeSymbol && y is not IPointerTypeSymbol ||
            x is IPointerTypeSymbol &&
            y is IPointerTypeSymbol &&
            !Eq(As<IPointerTypeSymbol>(x), As<IPointerTypeSymbol>(y))) &&
        (x is not ITypeParameterSymbol && y is not ITypeParameterSymbol ||
            x is ITypeParameterSymbol &&
            y is ITypeParameterSymbol &&
            !Eq(As<ITypeParameterSymbol>(x), As<ITypeParameterSymbol>(y))) &&
        (x is not IErrorTypeSymbol && y is not IErrorTypeSymbol || // Implements INamedTypeSymbol
            x is IErrorTypeSymbol && y is IErrorTypeSymbol && !Eq(As<IErrorTypeSymbol>(x), As<IErrorTypeSymbol>(y)));

    /// <summary>Returns the hash code for this string.</summary>
    /// <param name="x">The instance.</param>
    /// <returns>A 32-bit signed integer hash code.</returns>
    [Pure]
    public static int Hash(ISymbol? x)
    {
        if (x is null)
            return Prime();

        var hash = x.Kind.AsInt();
        hash ^= unchecked(x.MetadataToken * Prime());
        hash ^= unchecked(x.DeclaredAccessibility.AsInt() * Prime());
        hash ^= unchecked(StringComparer.Ordinal.GetHashCode(x.Name) * Prime());
        hash ^= unchecked(StringComparer.Ordinal.GetHashCode(x.Language) * Prime());
        hash ^= unchecked(StringComparer.Ordinal.GetHashCode(x.MetadataName) * Prime());
        return hash;
    }

    /// <inheritdoc />
    [Pure]
    public bool Equals(AssemblyIdentity? x, AssemblyIdentity? y) =>
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
    public bool Equals(CustomModifier? x, CustomModifier? y) =>
        ReferenceEquals(x, y) ||
        x is not null && y is not null && x.IsOptional == y.IsOptional && Eq(x.Modifier, y.Modifier);

    /// <inheritdoc />
    [Pure]
    bool IEqualityComparer<ISymbol?>.Equals(ISymbol? x, ISymbol? y) => Eq(x, y);

    /// <inheritdoc />
    [Pure]
    public int GetHashCode(AssemblyIdentity? obj)
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
    public int GetHashCode(CustomModifier? obj) =>
        obj is null ? Prime() : Hash(obj.Modifier) ^ obj.IsOptional.ToByte() * Prime();

    /// <inheritdoc />
    [Pure]
    int IEqualityComparer<ISymbol?>.GetHashCode(ISymbol? x) => Hash(x);

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    static bool Eq(IAliasSymbol x, IAliasSymbol y) => Eq(x.Target as ISymbol, y.Target);

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    static bool Eq(IArrayTypeSymbol x, IArrayTypeSymbol y) =>
        x.IsSZArray == y.IsSZArray &&
        x.Rank == y.Rank &&
        x.ElementNullableAnnotation == y.ElementNullableAnnotation &&
        x.LowerBounds.GuardedSequenceEqual(y.LowerBounds) &&
        x.CustomModifiers.GuardedSequenceEqual(y.CustomModifiers, Instance) &&
        x.Sizes.GuardedSequenceEqual(y.Sizes) &&
        Eq(x.ElementType as ISymbol, y.ElementType);

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    static bool Eq(IAssemblySymbol x, IAssemblySymbol y) =>
        x.IsInteractive == y.IsInteractive &&
        Instance.Equals(x.Identity, y.Identity) &&
        Eq(x.GlobalNamespace as ISymbol, y.GlobalNamespace);

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    static bool Eq(IDiscardSymbol x, IDiscardSymbol y) =>
        x.NullableAnnotation == y.NullableAnnotation == Eq(x.Type as ISymbol, y.Type);

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    static bool Eq(IErrorTypeSymbol x, IErrorTypeSymbol y) =>
        x.CandidateReason == y.CandidateReason && x.CandidateSymbols.GuardedSequenceEqual(y.CandidateSymbols, Instance);

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    static bool Eq(IEventSymbol x, IEventSymbol y) =>
        x.NullableAnnotation == y.NullableAnnotation &&
        x.IsWindowsRuntimeEvent == y.IsWindowsRuntimeEvent &&
        Eq(x.Type as ISymbol, y.Type) &&
        Eq(x.AddMethod as ISymbol, y.AddMethod) &&
        Eq(x.RaiseMethod as ISymbol, y.RaiseMethod) &&
        Eq(x.RemoveMethod as ISymbol, y.RemoveMethod) &&
        Eq(x.OverriddenEvent as ISymbol, y.OverriddenEvent) &&
        x.ExplicitInterfaceImplementations.GuardedSequenceEqual(y.ExplicitInterfaceImplementations, Instance);

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
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
        Eq(x.Type as ISymbol, y.Type) &&
        Eq(x.AssociatedSymbol, y.AssociatedSymbol) &&
        Eq(x.CorrespondingTupleField as ISymbol, y.CorrespondingTupleField) &&
        x.CustomModifiers.GuardedSequenceEqual(y.CustomModifiers, Instance) &&
        x.RefCustomModifiers.GuardedSequenceEqual(y.RefCustomModifiers, Instance);

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    static bool Eq(IFunctionPointerTypeSymbol x, IFunctionPointerTypeSymbol y) =>
        Eq(x.Signature as ISymbol, y.Signature);

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    static bool Eq(ILabelSymbol x, ILabelSymbol y) => Eq(x.ContainingMethod as ISymbol, y.ContainingMethod);

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
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
        Eq(x.Type as ISymbol, y.Type);

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
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
        Eq(x.ReturnType as ISymbol, y.ReturnType) &&
        Eq(x.AssociatedSymbol, y.AssociatedSymbol) &&
        Eq(x.ReducedFrom as ISymbol, y.ReducedFrom) &&
        Eq(x.ReceiverType as ISymbol, y.ReceiverType) &&
        Eq(x.ConstructedFrom as ISymbol, y.ConstructedFrom) &&
        Eq(x.OverriddenMethod as ISymbol, y.OverriddenMethod) &&
        Eq(x.PartialDefinitionPart as ISymbol, y.PartialDefinitionPart) &&
        Eq(x.PartialImplementationPart as ISymbol, y.PartialImplementationPart) &&
        Eq(x.AssociatedAnonymousDelegate as ISymbol, y.AssociatedAnonymousDelegate) &&
        x.TypeArgumentNullableAnnotations.GuardedSequenceEqual(y.TypeArgumentNullableAnnotations) &&
        x.RefCustomModifiers.GuardedSequenceEqual(y.RefCustomModifiers, Instance) &&
        x.ReturnTypeCustomModifiers.GuardedSequenceEqual(y.ReturnTypeCustomModifiers, Instance) &&
        x.Parameters.GuardedSequenceEqual(y.Parameters, Instance) &&
        x.TypeArguments.GuardedSequenceEqual(y.TypeArguments, Instance) &&
        x.UnmanagedCallingConventionTypes.GuardedSequenceEqual(y.UnmanagedCallingConventionTypes, Instance) &&
        x.ExplicitInterfaceImplementations.GuardedSequenceEqual(y.ExplicitInterfaceImplementations, Instance);

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    static bool Eq(IModuleSymbol x, IModuleSymbol y) =>
        Eq(x.GlobalNamespace as ISymbol, y.GlobalNamespace) &&
        x.ReferencedAssemblies.GuardedSequenceEqual(y.ReferencedAssemblies, Instance) &&
        x.ReferencedAssemblySymbols.GuardedSequenceEqual(y.ReferencedAssemblySymbols, Instance);

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
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
        Eq(x.AssociatedSymbol, y.AssociatedSymbol) &&
        Eq(x.EnumUnderlyingType as ISymbol, y.EnumUnderlyingType) &&
        Eq(x.TupleUnderlyingType as ISymbol, y.TupleUnderlyingType) &&
        Eq(x.DelegateInvokeMethod as ISymbol, y.DelegateInvokeMethod) &&
        Eq(x.NativeIntegerUnderlyingType as ISymbol, y.NativeIntegerUnderlyingType) &&
        x.TypeArgumentNullableAnnotations.GuardedSequenceEqual(y.TypeArgumentNullableAnnotations) &&
        x.TupleElements.GuardedSequenceEqual(y.TupleElements, Instance) &&
        x.TypeArguments.GuardedSequenceEqual(y.TypeArguments, Instance) &&
        x.TypeParameters.GuardedSequenceEqual(y.TypeParameters, Instance);

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    static bool Eq(INamespaceOrTypeSymbol x, INamespaceOrTypeSymbol y) =>
        x.IsNamespace == y.IsNamespace && x.IsType == y.IsType;

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    static bool Eq(INamespaceSymbol x, INamespaceSymbol y) =>
        x.IsGlobalNamespace == y.IsGlobalNamespace && x.NamespaceKind == y.NamespaceKind;

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
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
        Eq(x.Type as ISymbol, y.Type) &&
        x.CustomModifiers.GuardedSequenceEqual(y.CustomModifiers, Instance) &&
        x.RefCustomModifiers.GuardedSequenceEqual(y.RefCustomModifiers, Instance);

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    static bool Eq(IPointerTypeSymbol x, IPointerTypeSymbol y) =>
        Eq(x.PointedAtType as ISymbol, y.PointedAtType) &&
        x.CustomModifiers.GuardedSequenceEqual(y.CustomModifiers, Instance);

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
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
        Eq(x.Type as ISymbol, y.Type) &&
        Eq(x.GetMethod as ISymbol, y.GetMethod) &&
        Eq(x.SetMethod as ISymbol, y.SetMethod) &&
        Eq(x.OverriddenProperty as ISymbol, y.OverriddenProperty) &&
        x.RefCustomModifiers.GuardedSequenceEqual(y.RefCustomModifiers, Instance) &&
        x.TypeCustomModifiers.GuardedSequenceEqual(y.TypeCustomModifiers, Instance) &&
        x.ExplicitInterfaceImplementations.GuardedSequenceEqual(y.ExplicitInterfaceImplementations, Instance) &&
        x.Parameters.GuardedSequenceEqual(y.Parameters, Instance);

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
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
        Eq(x.ReducedFrom as ISymbol, y.ReducedFrom) &&
        Eq(x.DeclaringType as ISymbol, y.DeclaringType) &&
        Eq(x.DeclaringMethod as ISymbol, y.DeclaringMethod) &&
        x.ConstraintNullableAnnotations.GuardedSequenceEqual(y.ConstraintNullableAnnotations) &&
        x.ConstraintTypes.GuardedSequenceEqual(y.ConstraintTypes, Instance);

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
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
        Eq(x.BaseType as ISymbol, y.BaseType) &&
        x.Interfaces.GuardedSequenceEqual(y.Interfaces, Instance) &&
        x.AllInterfaces.GuardedSequenceEqual(y.AllInterfaces, Instance);

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    static T As<T>(ISymbol x)
        where T : class, ISymbol
    {
        Debug.Assert(x is T);
        return Unsafe.As<T>(x);
    }
}
#endif
