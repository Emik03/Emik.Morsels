﻿// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace
#pragma warning disable MA0048
#if IL_MERGE_AGGRESSIVE
[assembly: IncludeAssemblies("")]
#elif !IL_MERGE
[assembly: ExcludeAssemblies("")]
#endif
#if NETFRAMEWORK && !NET45_OR_GREATER
namespace ILMerge;

/// <summary>
/// A regular expression matching the assembly names to include in merging.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly)]
public class IncludeAssembliesAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IncludeAssembliesAttribute"/> class.
    /// </summary>
    /// <param name="pattern">The regular expression pattern.</param>
    public IncludeAssembliesAttribute(string pattern) => Pattern = pattern;

    /// <summary>
    /// Gets the regular expression pattern.
    /// </summary>
    public string Pattern { get; }
}

/// <summary>
/// A regular expression matching the assembly names to exclude from merging.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly)]
public class ExcludeAssembliesAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExcludeAssembliesAttribute"/> class.
    /// </summary>
    /// <param name="pattern">The regular expression pattern.</param>
    public ExcludeAssembliesAttribute(string pattern) => Pattern = pattern;

    /// <summary>
    /// Gets the regular expression pattern.
    /// </summary>
    public string Pattern { get; }
}

/// <summary>
/// A regular expression matching the resource names to include in merging.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly)]
public class IncludeResourcesAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IncludeResourcesAttribute"/> class.
    /// </summary>
    /// <param name="pattern">The regular expression pattern.</param>
    public IncludeResourcesAttribute(string pattern) => Pattern = pattern;

    /// <summary>
    /// Gets the regular expression pattern.
    /// </summary>
    public string Pattern { get; }
}

/// <summary>
/// A regular expression matching the resource names to exclude from merging.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly)]
public class ExcludeResourcesAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExcludeResourcesAttribute"/> class.
    /// </summary>
    /// <param name="pattern">The regular expression pattern.</param>
    public ExcludeResourcesAttribute(string pattern) => Pattern = pattern;

    /// <summary>
    /// Gets the regular expression pattern.
    /// </summary>
    public string Pattern { get; }
}

/// <summary>
/// A switch to control whether the imported types are hidden (made private) or keep their visibility unchanged. Default is 'true'
/// </summary>
[AttributeUsage(AttributeTargets.Assembly)]
public class HideImportedTypesAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HideImportedTypesAttribute"/> class.
    /// </summary>
    /// <param name="value">if set to <see langword="true"/>, imported types are hidden (private/internal).</param>
    public HideImportedTypesAttribute(bool value) => Value = value;

    /// <summary>
    /// Gets a value indicating whether imported types are hidden (private/internal).
    /// </summary>
    public bool Value { get; }
}

/// <summary>
/// A string that is used as prefix for the namespace of the imported types..
/// </summary>
[AttributeUsage(AttributeTargets.Assembly)]
public class NamespacePrefixAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NamespacePrefixAttribute"/> class.
    /// </summary>
    /// <param name="prefix">The prefix.</param>
    public NamespacePrefixAttribute(string prefix) => Prefix = prefix;

    /// <summary>
    /// Gets the regular expression pattern.
    /// </summary>
    public string Prefix { get; }
}

/// <summary>
/// A switch to control whether to import the full assemblies or only the referenced types. Default is 'false'.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly)]
public class FullImportAttribute : Attribute
{
    /// <summary>Initializes a new instance of the <see cref="FullImportAttribute"/> class.</summary>
    /// <param name="value">if set to <see langword="true"/>, all types of the referenced assemblies are imported.</param>
    public FullImportAttribute(bool value) => Value = value;

    /// <summary>
    /// Gets a value indicating whether imported types are hidden (private/internal).
    /// </summary>
    public bool Value { get; }
}

/// <summary>
/// A switch to enable compacting of the target assembly by skipping properties, events and unused static methods.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly)]
public class CompactModeAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CompactModeAttribute"/> class.
    /// </summary>
    /// <param name="value">A switch to enable compacting of the target assembly by skipping properties, events and unused methods.</param>
    public CompactModeAttribute(bool value) => Value = value;

    /// <summary>
    /// A switch to enable compacting of the target assembly by skipping properties, events and unused methods.
    /// </summary>
    public bool Value { get; }
}
#endif
