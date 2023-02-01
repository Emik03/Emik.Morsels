// SPDX-License-Identifier: MPL-2.0
#pragma warning disable GlobalUsingsAnalyzer

global using static Emik.Morsels.Unforgiving;

// ReSharper disable once CheckNamespace RedundantUsingDirective.Global
namespace Emik.Morsels;

/// <summary>Provides a reference for an <c>UnreachableException</c>.</summary>
#pragma warning disable MA0048
static partial class Unforgiving
#pragma warning restore MA0048
{
    /// <summary>Gets the <see cref="Exception"/> that a collection cannot be empty.</summary>
    public static InvalidOperationException CannotBeEmpty { get; } = new("Buffer is empty.");

    /// <summary>Gets the <see cref="Exception"/> that represents an unreachable state.</summary>
    public static UnreachableException Unreachable { get; } = new();
}
