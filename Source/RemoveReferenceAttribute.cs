#region Emik.MPL

// <copyright file="RemoveReferenceAttribute.cs" company="Emik">
// Copyright (c) Emik. This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

#endregion

#if NET452
[assembly: RemoveReference("System.Runtime, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]

// ReSharper disable once CheckNamespace
namespace RemoveReference;

/// <summary>Ensures that a reference be removed from the compiled binary.</summary>
/// <remarks><para>Declared so as to prevent a hard dependency to RemoveReference.Fody.</para></remarks>
[AttributeUsage(AttributeTargets.Assembly)]
sealed partial class RemoveReferenceAttribute : Attribute
{
    /// <summary>Initializes a new instance of the <see cref="RemoveReferenceAttribute"/> class.</summary>
    /// <param name="fullName">The full name of an assembly to remove its reference.</param>
    internal RemoveReferenceAttribute(string fullName) => FullName = fullName;

    /// <summary>Gets an assembly's full name which has been stripped from this compiled binary.</summary>
    internal string FullName { get; }
}
#endif
