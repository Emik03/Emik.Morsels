// <copyright file="MethodImplOptions.cs" company="Emik">
// Copyright (c) Emik. This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>
#if NETFRAMEWORK
#pragma warning disable GlobalUsingsAnalyzer
extern alias ms;
using Options = ms::System.Runtime.CompilerServices.MethodImplOptions;

namespace System.Runtime.CompilerServices;

/// <summary>
/// Specifies constants that define the details of how a method is implemented.
/// This enumeration supports a bitwise combination of its member values.
/// </summary>
/// <remarks><para>
/// This enumeration is used with the <see cref="MethodImplAttribute"/> attribute.
/// You can specify multiple <see cref="MethodImplOptions"/> values by using the bitwise OR operator.
/// </para></remarks>
[ComVisible(true), Serializable]
static class MethodImplOptions
{
    /// <summary>The method is implemented in unmanaged code.</summary>
    internal const Options Unmanaged = (Options)(1 << 2);

    /// <summary>
    /// The method cannot be inlined.
    /// Inlining is an optimization by which a method call is replaced with the method body.
    /// </summary>
    internal const Options NoInlining = (Options)(1 << 3);

    /// <summary>The method is declared, but its implementation is provided elsewhere.</summary>
    internal const Options ForwardRef = (Options)(1 << 4);

    /// <summary>
    /// The method can be executed by only one thread at a time.
    /// Static methods lock on the type, whereas instance methods lock on the instance.
    /// Only one thread can execute in any of the instance functions,
    /// and only one thread can execute in any of a class's static functions.
    /// </summary>
    internal const Options Synchronized = (Options)(1 << 5);

    /// <summary>
    /// The method is not optimized by the just-in-time (JIT) compiler or by native code generation (see Ngen.exe)
    /// when debugging possible code generation problems.
    /// </summary>
    internal const Options NoOptimization = (Options)(1 << 6);

    /// <summary>The method signature is exported exactly as declared.</summary>
    internal const Options PreserveSig = (Options)(1 << 7);

    /// <summary>The method should be inlined if possible.</summary>
    [ComVisible(false)]
    internal const Options AggressiveInlining = (Options)(1 << 8);

    /// <summary>The method contains code that should always be optimized by the just-in-time (JIT) compiler.</summary>
    /// <remarks><para>
    /// Use this attribute if running an unoptimized version of the method has undesirable effects,
    /// for instance causing too much overhead or extra memory allocation.
    /// </para><para>
    /// Methods with this attribute may not have optimal code generation.
    /// They bypass the first tier of Tiered Compilation and therefore can't benefit from optimizations that rely on
    /// tiering, for example, Dynamic PGO or optimizations based on initialized classes.
    /// </para></remarks>
    internal const Options AggressiveOptimization = (Options)(1 << 9);

    /// <summary>
    /// The call is internal, that is, it calls a method that's implemented within the common language runtime.
    /// </summary>
    internal const Options InternalCall = (Options)(1 << 10);
}
#endif
