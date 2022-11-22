// <copyright file="Unforgiving.cs" company="Emik">
// Copyright (c) Emik. This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>
#pragma warning disable GlobalUsingsAnalyzer
global using static Emik.Morsels.Unforgiving;

// ReSharper disable once RedundantUsingDirective.Global
namespace Emik.Morsels;

/// <summary>Provides a reference for an <see cref="UnreachableException"/>.</summary>
#pragma warning disable MA0048
static class Unforgiving
#pragma warning restore MA0048
{
    /// <summary>Gets the <see cref="Exception"/> that a collection cannot be empty.</summary>
    internal static InvalidOperationException CannotBeEmpty { get; } = new("Buffer is empty.");

    /// <summary>Gets the <see cref="Exception"/> that represents an unreachable state.</summary>
    internal static UnreachableException Unreachable { get; } = new();
}
