﻿// SPDX-License-Identifier: MPL-2.0
#if !NET452_OR_GREATER && !NETSTANDARD1_1 && !NETCOREAPP
namespace InlineIL;

/// <summary>
/// Represents a stand-alone method signature,
/// for use with <see cref="IL.Emit.Calli"/>.
/// </summary>
sealed class StandAloneMethodSig
{
    /// <summary>
    /// Constructs an unmanaged method signature for the <c>calli</c> opcode.
    /// </summary>
    /// <param name="callingConvention">The unmanaged calling convention.</param>
    /// <param name="returnType">The method return type.</param>
    /// <param name="parameterTypes">The method parameter types.</param>
    // ReSharper disable UnusedParameter.Local
    public StandAloneMethodSig(
        CallingConvention callingConvention,
        TypeRef returnType,
        params TypeRef[] parameterTypes
    ) =>
        throw IL.Throw();

    /// <summary>
    /// Constructs a managed method signature for the <c>calli</c> opcode.
    /// </summary>
    /// <param name="callingConvention">The managed calling convention.</param>
    /// <param name="returnType">The method return type.</param>
    /// <param name="parameterTypes">The method parameter types.</param>
    public StandAloneMethodSig(
        CallingConventions callingConvention,
        TypeRef returnType,
        params TypeRef[] parameterTypes
    ) =>
        throw IL.Throw();

    /// <summary>
    /// Constructs an unmanaged method signature for the <c>calli</c> opcode.
    /// </summary>
    /// <param name="callingConvention">The unmanaged calling convention.</param>
    /// <param name="returnType">The method return type.</param>
    /// <param name="parameterTypes">The method parameter types.</param>
    public static StandAloneMethodSig UnmanagedMethod(
        CallingConvention callingConvention,
        TypeRef returnType,
        params TypeRef[] parameterTypes
    ) =>
        throw IL.Throw();

    /// <summary>
    /// Constructs a managed method signature for the <c>calli</c> opcode.
    /// </summary>
    /// <param name="callingConvention">The managed calling convention.</param>
    /// <param name="returnType">The method return type.</param>
    /// <param name="parameterTypes">The method parameter types.</param>
    public static StandAloneMethodSig ManagedMethod(
        CallingConventions callingConvention,
        TypeRef returnType,
        params TypeRef[] parameterTypes
    ) =>
        throw IL.Throw();

    /// <summary>
    /// Specifies the optional parameter types for a managed varargs method call.
    /// </summary>
    /// <param name="optionalParameterTypes">The optional parameter types.</param>
    /// <returns>A reference to a varargs method call signature.</returns>
    public StandAloneMethodSig WithOptionalParameters(params TypeRef[] optionalParameterTypes) => throw IL.Throw();
}
#endif
