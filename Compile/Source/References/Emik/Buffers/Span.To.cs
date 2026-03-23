// SPDX-License-Identifier: MPL-2.0

// ReSharper disable BadPreprocessorIndent RedundantUnsafeContext UseSymbolAlias
// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

// ReSharper disable RedundantNameQualifier RedundantUsingDirective
using static System.Runtime.CompilerServices.RuntimeHelpers;

/// <summary>Defines methods for spans.</summary>
static partial class Span
{
    /// <summary>Provides reinterpret span methods.</summary>
    /// <typeparam name="TTo">The type to convert to.</typeparam>
    public static class To<TTo>
    {
        /// <summary>
        /// Encapsulates the functionality to determine if a conversion is supported between two types.
        /// </summary>
        /// <typeparam name="TFrom">The type to convert from.</typeparam>
        public static class Is<TFrom>
#if !NO_ALLOWS_REF_STRUCT
            where TFrom : allows ref struct
#endif
        {
            /// <summary>
            /// Gets a value indicating whether the conversion between types
            /// <typeparamref name="TFrom"/> and <see name="TTo"/> in <see cref="To{TTo}"/> is defined.
            /// </summary>
            public static bool Supported { [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get; } =
#if NETSTANDARD && !NETSTANDARD2_0_OR_GREATER
                typeof(TFrom) == typeof(TTo);
#else
#pragma warning disable MA0169
                typeof(TFrom) == typeof(TTo) ||
                Unsafe.SizeOf<TFrom>() >= Unsafe.SizeOf<TTo>() &&
                (IsReinterpretable(typeof(TFrom), typeof(TTo)) ||
                    !IsReferenceOrContainsReferences<TFrom>() && !IsReferenceOrContainsReferences<TTo>());

            [Pure]
            static bool IsReinterpretable(Type first, Type second)
            {
                while (first.IsValueType && first.GetFields() is [{ FieldType: var next }])
                    first = next;

                while (second.IsValueType && second.GetFields() is [{ FieldType: var next }])
                    second = next;

                return first == second;
#pragma warning restore MA0169
            }
#endif
        }

        /// <summary>
        /// Converts a <see cref="ReadOnlySpan{T}"/> of type <typeparamref name="TFrom"/>
        /// to a <see cref="ReadOnlySpan{T}"/> of type <see name="TTo"/> in <see cref="To{TTo}"/>.
        /// </summary>
        /// <typeparam name="TFrom">The type to convert from.</typeparam>
        /// <param name="source">The <see cref="ReadOnlySpan{T}"/> to convert from.</param>
        /// <exception cref="NotSupportedException">
        /// Thrown when <see cref="Is{TFrom}.Supported"/> is <see langword="false"/>.
        /// </exception>
        /// <returns>
        /// The reinterpretation of the parameter <paramref name="source"/> from its original type
        /// <typeparamref name="TFrom"/> to the destination type <see name="TTo"/> in <see cref="To{TTo}"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        public static unsafe ReadOnlySpan<TTo> From<TFrom>(ReadOnlySpan<TFrom> source)
        {
            System.Diagnostics.Debug.Assert(Is<TFrom>.Supported, "No out-of-bounds access.");
#if (NET452_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP) && !CSHARPREPL
            // We have to resort to inline IL because Unsafe.As<T> has a constraint for classes,
            // and Unsafe.As<TFrom, TTo> introduces a miniscule amount of overhead.
            // Doing it like this reduces the IL size from 9 to 2 bytes, and the JIT assembly from 9 to 3 bytes.
            InlineIL.IL.Emit.Ldarg_0();
            InlineIL.IL.Emit.Ret();
            throw InlineIL.IL.Unreachable();
#else
            return *(ReadOnlySpan<TTo>*)&source;
#endif
        }

        /// <summary>
        /// Converts a <see cref="Span{T}"/> of type <typeparamref name="TFrom"/>
        /// to a <see cref="Span{T}"/> of type <see name="TTo"/> in <see cref="To{TTo}"/>.
        /// </summary>
        /// <typeparam name="TFrom">The type to convert from.</typeparam>
        /// <param name="source">The <see cref="Span{T}"/> to convert from.</param>
        /// <exception cref="NotSupportedException">
        /// Thrown when <see cref="Is{TFrom}.Supported"/> is <see langword="false"/>.
        /// </exception>
        /// <returns>
        /// The reinterpretation of the parameter <paramref name="source"/> from its original
        /// type <typeparamref name="TFrom"/> to the destination type <see name="TTo"/> in <see cref="To{TTo}"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        public static unsafe Span<TTo> From<TFrom>(Span<TFrom> source)
        {
            System.Diagnostics.Debug.Assert(Is<TFrom>.Supported, "No out-of-bounds access.");
#if (NET452_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP) && !CSHARPREPL
            // We have to resort to inline IL because Unsafe.As<T> has a constraint for classes,
            // and Unsafe.As<TFrom, TTo> introduces a miniscule amount of overhead.
            // Doing it like this reduces the IL size from 9 to 2 bytes, and the JIT assembly from 9 to 3 bytes.
            InlineIL.IL.Emit.Ldarg_0();
            InlineIL.IL.Emit.Ret();
            throw InlineIL.IL.Unreachable();
#else
            return *(Span<TTo>*)&source;
#endif
        }
    }
}
