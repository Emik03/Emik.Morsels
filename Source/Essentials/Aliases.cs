// <copyright file="Aliases.cs" company="Emik">
// Copyright (c) Emik. This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>
// ReSharper disable RedundantUsingDirective.Global
#if NET35
#pragma warning disable GlobalUsingsAnalyzer
extern alias unity;

// Curse you Unity for making me do this. Allows Jetbrains.Annotations and UnityEngine to coexist.
global using AssertionMethodAttribute = unity::JetBrains.Annotations.AssertionMethodAttribute;
global using BaseTypeRequiredAttribute = unity::JetBrains.Annotations.BaseTypeRequiredAttribute;
global using CanBeNullAttribute = unity::JetBrains.Annotations.CanBeNullAttribute;
global using CannotApplyEqualityOperatorAttribute = unity::JetBrains.Annotations.CannotApplyEqualityOperatorAttribute;
global using Component = unity::UnityEngine.Component;
global using ContractAnnotationAttribute = unity::JetBrains.Annotations.ContractAnnotationAttribute;
global using Debug = unity::UnityEngine.Debug;
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
global using StringFormatMethodAttribute = unity::JetBrains.Annotations.StringFormatMethodAttribute;
global using UsedImplicitlyAttribute = unity::JetBrains.Annotations.UsedImplicitlyAttribute;
#endif
