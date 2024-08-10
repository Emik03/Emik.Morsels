// SPDX-License-Identifier: MPL-2.0
#if NETFRAMEWORK
#pragma warning disable GlobalUsingsAnalyzer
extern alias ms;

// ReSharper disable once CheckNamespace
namespace System.Runtime.CompilerServices;

using Options = ms::System.Runtime.CompilerServices.MethodImplOptions;

/// <summary>
/// Specifies constants that define the details of how a method is implemented.
/// This enumeration supports a bitwise combination of its member values.
/// </summary>
/// <remarks><para>
/// This enumeration is used with the <see cref="MethodImplAttribute"/> attribute.
/// You can specify multiple <see cref="MethodImplOptions"/> values by using the bitwise OR operator.
/// </para></remarks>
[ComVisible(true), Serializable]
static partial class MethodImplOptions
{
    /// <summary>The method is implemented in unmanaged code.</summary>
    public const Options Unmanaged = (Options)(1 << 2);

    /// <summary>
    /// The method cannot be inlined.
    /// Inlining is an optimization by which a method call is replaced with the method body.
    /// </summary>
    public const Options NoInlining = (Options)(1 << 3);

    /// <summary>The method is declared, but its implementation is provided elsewhere.</summary>
    public const Options ForwardRef = (Options)(1 << 4);

    /// <summary>
    /// The method can be executed by only one thread at a time.
    /// Static methods lock on the type, whereas instance methods lock on the instance.
    /// Only one thread can execute in any of the instance functions,
    /// and only one thread can execute in any of a class's static functions.
    /// </summary>
    public const Options Synchronized = (Options)(1 << 5);

    /// <summary>
    /// The method is not optimized by the just-in-time (JIT) compiler or by native code generation (see Ngen.exe)
    /// when debugging possible code generation problems.
    /// </summary>
    public const Options NoOptimization = (Options)(1 << 6);

    /// <summary>The method signature is exported exactly as declared.</summary>
    public const Options PreserveSig = (Options)(1 << 7);

    /// <summary>The method should be inlined if possible.</summary>
    [ComVisible(false)]
    public const Options AggressiveInlining =
#if NO_AGGRESSIVE_INLINING
        0;
#else
        (Options)(1 << 8);
#endif

    /// <summary>The method contains code that should always be optimized by the just-in-time (JIT) compiler.</summary>
    /// <remarks><para>
    /// Use this attribute if running an unoptimized version of the method has undesirable effects,
    /// for instance causing too much overhead or extra memory allocation.
    /// </para><para>
    /// Methods with this attribute may not have optimal code generation.
    /// They bypass the first tier of Tiered Compilation and therefore can't benefit from optimizations that rely on
    /// tiering, for example, Dynamic PGO or optimizations based on initialized classes.
    /// </para></remarks>
    public const Options AggressiveOptimization = (Options)(1 << 9);

    /// <summary>
    /// The call is internal, that is, it calls a method that's implemented within the common language runtime.
    /// </summary>
    public const Options InternalCall = (Options)(1 << 10);
}
#endif
