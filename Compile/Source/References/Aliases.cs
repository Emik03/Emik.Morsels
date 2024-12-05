// SPDX-License-Identifier: MPL-2.0

// ReSharper disable ArrangeStaticMemberQualifier MissingBlankLines RedundantUsingDirective.Global
#pragma warning disable GlobalUsingsAnalyzer
#if KTANE
// Curse you Unity for making me do this. Allows JetBrains.Annotations and UnityEngine to coexist.
extern alias unity;
#endif
#if NETFRAMEWORK
extern alias ms;
#endif
global using Attribute = System.Attribute;
global using DisallowNullAttribute = System.Diagnostics.CodeAnalysis.DisallowNullAttribute;
global using FieldInfo = System.Reflection.FieldInfo;
global using MemberInfo = System.Reflection.MemberInfo;
global using Version = System.Version;
#if ANDROID
global using Action = System.Action;
global using Array = System.Array;
global using Console = System.Console;
global using Directory = System.IO.Directory;
global using Enum = System.Enum;
global using Environment = System.Environment;
global using Exception = System.Exception;
global using File = System.IO.File;
global using FileNotFoundException = System.IO.FileNotFoundException;
global using ICollection = System.Collections.ICollection;
global using IList = System.Collections.IList;
global using Math = System.Math;
global using Path = System.IO.Path;
global using RegexOptions = System.Text.RegularExpressions.RegexOptions;
global using Random = System.Random;
global using Range = System.Range;
global using StringBuilder = System.Text.StringBuilder;
global using Trace = System.Diagnostics.Trace;
global using Type = System.Type;
#if !NETSTANDARD2_1_OR_GREATER && !NETCOREAPP_3_0_OR_GREATER && !NET5_0_OR_GREATER
global using Vector = System.Numerics.Vector;
#endif
#endif
#if KTANE
global using Application = unity::UnityEngine.Application;
global using Assembly = System.Reflection.Assembly;
global using AssertionMethodAttribute = unity::JetBrains.Annotations.AssertionMethodAttribute;
global using BaseTypeRequiredAttribute = unity::JetBrains.Annotations.BaseTypeRequiredAttribute;
global using CanBeNullAttribute = unity::JetBrains.Annotations.CanBeNullAttribute;
global using CannotApplyEqualityOperatorAttribute = unity::JetBrains.Annotations.CannotApplyEqualityOperatorAttribute;
global using Color = unity::UnityEngine.Color;
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
#else
global using Debug = System.Diagnostics.Debug;
#endif
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP_3_0_OR_GREATER || NET5_0_OR_GREATER
global using Vector = System.Numerics.Vector;
#endif
#if KTANE && !WAWA || XNA && !ANDROID
global using Range = System.Range;
#endif
#if KTANE
global using PureAttribute = unity::JetBrains.Annotations.PureAttribute;
#elif NET40_OR_GREATER
global using PureAttribute = ms::System.Diagnostics.Contracts.PureAttribute;
#else
global using PureAttribute = System.Diagnostics.Contracts.PureAttribute;
#endif
#if !NETSTANDARD || NETSTANDARD1_2_OR_GREATER
global using Timer = System.Threading.Timer;
#endif
#if XNA
global using Color = Microsoft.Xna.Framework.Color;
global using DisplayMode = Microsoft.Xna.Framework.Graphics.DisplayMode;
global using Keyboard = Microsoft.Xna.Framework.Input.Keyboard;
global using Point = Microsoft.Xna.Framework.Point;
global using Rectangle = Microsoft.Xna.Framework.Rectangle;
global using SoundEffect = Microsoft.Xna.Framework.Audio.SoundEffect;
global using Vector2 = Microsoft.Xna.Framework.Vector2;
#endif // ReSharper disable once RedundantUsingDirective
using static System.AttributeTargets;

#if NETFRAMEWORK && !NET40_OR_GREATER || NETSTANDARD && !NETSTANDARD2_0_OR_GREATER
// ReSharper disable once CheckNamespace
namespace System.Diagnostics.Contracts
{
    /// <summary>Indicates that a type or method is pure, that is, it does not make any visible state changes.</summary>
    [AttributeUsage(
        Class | Constructor | AttributeTargets.Delegate | AttributeTargets.Event | AttributeTargets.Parameter | Method | Property
    )]
    sealed partial class PureAttribute : Attribute;
}
#endif
