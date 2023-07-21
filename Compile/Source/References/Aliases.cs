// SPDX-License-Identifier: MPL-2.0

// ReSharper disable MissingBlankLines RedundantUsingDirective.Global
#pragma warning disable GlobalUsingsAnalyzer, SA1649
#if NETFRAMEWORK
extern alias ms;
#endif
#if KTANE
extern alias unity;

// Curse you Unity for making me do this. Allows JetBrains.Annotations and UnityEngine to coexist.
global using AssertionMethodAttribute = unity::JetBrains.Annotations.AssertionMethodAttribute;
global using BaseTypeRequiredAttribute = unity::JetBrains.Annotations.BaseTypeRequiredAttribute;
global using CanBeNullAttribute = unity::JetBrains.Annotations.CanBeNullAttribute;
global using CannotApplyEqualityOperatorAttribute = unity::JetBrains.Annotations.CannotApplyEqualityOperatorAttribute;
global using Component = unity::UnityEngine.Component;
global using ContractAnnotationAttribute = unity::JetBrains.Annotations.ContractAnnotationAttribute;
global using Debug = unity::UnityEngine.Debug;
#endif
global using DisallowNullAttribute = System.Diagnostics.CodeAnalysis.DisallowNullAttribute;
#if KTANE
global using ImplicitUseKindFlags = unity::JetBrains.Annotations.ImplicitUseKindFlags;
global using ImplicitUseTargetFlags = unity::JetBrains.Annotations.ImplicitUseTargetFlags;
global using InstantHandleAttribute = unity::JetBrains.Annotations.InstantHandleAttribute;
global using InvokerParameterNameAttribute = unity::JetBrains.Annotations.InvokerParameterNameAttribute;
global using LinqTunnelAttribute = unity::JetBrains.Annotations.LinqTunnelAttribute;
global using LocalizationRequiredAttribute = unity::JetBrains.Annotations.LocalizationRequiredAttribute;
global using MeansImplicitUseAttribute = unity::JetBrains.Annotations.MeansImplicitUseAttribute;
global using NoEnumerationAttribute = unity::JetBrains.Annotations.NoEnumerationAttribute;
global using NotifyPropertyChangedInvocatorAttribute =
    unity::JetBrains.Annotations.NotifyPropertyChangedInvocatorAttribute;
global using NotNullAttribute = unity::JetBrains.Annotations.NotNullAttribute;
global using Object = unity::UnityEngine.Object;
global using PathReferenceAttribute = unity::JetBrains.Annotations.PathReferenceAttribute;
global using PublicAPIAttribute = unity::JetBrains.Annotations.PublicAPIAttribute;
#endif
#if KTANE && WAWA
global using PureAttribute = unity::JetBrains.Annotations.PureAttribute;
#elif NET40_OR_GREATER
global using PureAttribute = ms::System.Diagnostics.Contracts.PureAttribute;
#else
global using PureAttribute = System.Diagnostics.Contracts.PureAttribute;
#endif

#if KTANE
#if !WAWA
global using Range = System.Range;
#endif
global using StringFormatMethodAttribute = unity::JetBrains.Annotations.StringFormatMethodAttribute;
global using UsedImplicitlyAttribute = unity::JetBrains.Annotations.UsedImplicitlyAttribute;
#endif
#if NETFRAMEWORK && !NET40_OR_GREATER || NETSTANDARD && !NETSTANDARD2_0_OR_GREATER
#if !WAWA
// ReSharper disable once CheckNamespace
namespace System.Diagnostics.Contracts;

using static AttributeTargets;

/// <summary>Indicates that a type or method is pure, that is, it does not make any visible state changes.</summary>
[AttributeUsage(Class | Constructor | Delegate | Event | Parameter | Method | Property)]
#pragma warning disable MA0048
sealed partial class PureAttribute : Attribute { }
#pragma warning restore MA0048
#endif
#endif
