// SPDX-License-Identifier: MPL-2.0
// ReSharper disable once CheckNamespace EmptyNamespace
namespace System.Buffers;
#if !NET7_0_OR_GREATER
/// <summary>Provides the polyfill to <c>ReadOnlySpan&lt;T&gt;.IndexOfAny</c>.</summary>
[Obsolete("This class shouldn't be referred to directly, as only the extension method is guaranteed.", true)]
static class SpanIndexOfAnyPolyfill
{
    /// <inheritdoc cref="IndexOfAny{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static int IndexOfAny<T>(this scoped Span<T> span, scoped ReadOnlySpan<T> values)
        where T : IEquatable<T>? =>
        span.ReadOnly().IndexOfAny(values);

    /// <summary>
    /// Searches for the first index of the specified values similar
    /// to calling IndexOf several times with the logical OR operator.
    /// </summary>
    /// <typeparam name="T">The type of the span and values.</typeparam>
    /// <param name="span">The span to search.</param>
    /// <param name="values">The set of values to search for.</param>
    /// <returns>The first index of the occurrence of the values in the span. If not found, returns -1.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static int IndexOfAny<T>(this scoped ReadOnlySpan<T> span, scoped ReadOnlySpan<T> values)
        where T : IEquatable<T>?
    {
        static int Of(ref T search, T value, int length)
        {
            T obj;
            nint elementOffset = 0;

            while (length >= 8)
            {
                length -= 8;
                ref var local1 = ref value;
                obj = default!;

                if (ReferenceEquals(obj, null))
                {
                    obj = local1;
                    local1 = ref obj!;
                }

                var other1 = Unsafe.Add(ref search, elementOffset);

                if (local1!.Equals(other1))
                    goto Found1;

                ref var local2 = ref value;
                obj = default!;

                if (ReferenceEquals(obj, null))
                {
                    obj = local2;
                    local2 = ref obj!;
                }

                var other2 = Unsafe.Add(ref search, elementOffset + 1);

                if (local2!.Equals(other2))
                    goto Found2;

                ref var local3 = ref value;
                obj = default!;

                if (ReferenceEquals(obj, null))
                {
                    obj = local3;
                    local3 = ref obj!;
                }

                var other3 = Unsafe.Add(ref search, elementOffset + 2);

                if (local3!.Equals(other3))
                    goto Found3;

                ref var local4 = ref value;
                obj = default!;

                if (ReferenceEquals(obj, null))
                {
                    obj = local4;
                    local4 = ref obj!;
                }

                var other4 = Unsafe.Add(ref search, elementOffset + 3);

                if (local4!.Equals(other4))
                    goto Found4;

                ref var local5 = ref value;
                obj = default!;

                if (ReferenceEquals(obj, null))
                {
                    obj = local5;
                    local5 = ref obj!;
                }

                var other5 = Unsafe.Add(ref search, elementOffset + 4);

                if (local5!.Equals(other5))
                    return (int)(elementOffset + 4);

                ref var local6 = ref value;
                obj = default!;

                if (ReferenceEquals(obj, null))
                {
                    obj = local6;
                    local6 = ref obj!;
                }

                var other6 = Unsafe.Add(ref search, elementOffset + 5);

                if (local6!.Equals(other6))
                    return (int)(elementOffset + 5);

                ref var local7 = ref value;
                obj = default!;

                if (ReferenceEquals(obj, null))
                {
                    obj = local7;
                    local7 = ref obj!;
                }

                var other7 = Unsafe.Add(ref search, elementOffset + 6);

                if (local7!.Equals(other7))
                    return (int)(elementOffset + 6);

                ref var local8 = ref value;
                obj = default!;

                if (ReferenceEquals(obj, null))
                {
                    obj = local8;
                    local8 = ref obj!;
                }

                var other8 = Unsafe.Add(ref search, elementOffset + 7);

                if (local8!.Equals(other8))
                    return (int)(elementOffset + 7);

                elementOffset += 8;
            }

            if (length >= 4)
            {
                length -= 4;
                ref var local9 = ref value;
                obj = default!;

                if (ReferenceEquals(obj, null))
                {
                    obj = local9;
                    local9 = ref obj!;
                }

                var other9 = Unsafe.Add(ref search, elementOffset);

                if (local9!.Equals(other9))
                    goto Found1;

                ref var local10 = ref value;
                obj = default!;

                if (ReferenceEquals(obj, null))
                {
                    obj = local10;
                    local10 = ref obj!;
                }

                var other10 = Unsafe.Add(ref search, elementOffset + 1);

                if (local10!.Equals(other10))
                    goto Found2;

                ref var local11 = ref value;
                obj = default!;

                if (ReferenceEquals(obj, null))
                {
                    obj = local11;
                    local11 = ref obj!;
                }

                var other11 = Unsafe.Add(ref search, elementOffset + 2);

                if (local11!.Equals(other11))
                    goto Found3;

                ref var local12 = ref value;
                obj = default!;

                if (ReferenceEquals(obj, null))
                {
                    obj = local12;
                    local12 = ref obj!;
                }

                var other12 = Unsafe.Add(ref search, elementOffset + 3);

                if (!local12!.Equals(other12))
                    elementOffset += 4;
                else
                    goto Found4;
            }

            for (; length > 0; --length)
            {
                ref var local = ref value;
                obj = default!;

                if (ReferenceEquals(obj, null))
                {
                    obj = local;
                    local = ref obj!;
                }

                var other = Unsafe.Add(ref search, elementOffset);

                if (!local!.Equals(other))
                    elementOffset++;
                else
                    goto Found1;
            }

            return -1;

        Found1:
            return (int)elementOffset;

        Found2:
            return (int)(elementOffset + 1);

        Found3:
            return (int)(elementOffset + 2);

        Found4:
            return (int)(elementOffset + 3);
        }

        static int OfAny(in T search, int searchLength, in T value, int valueLength)
        {
            if (valueLength == 0)
                return 0;

            var min = -1;

            for (var i = 0; i < valueLength; ++i)
            {
                if (Of(ref Unsafe.AsRef(search), Unsafe.Add(ref Unsafe.AsRef(value), i), searchLength) is var next &&
                    (uint)next >= (uint)min)
                    continue;

                min = next;
                searchLength = next;

                if (min is 0)
                    break;
            }

            return min;
        }

        return OfAny(span[0], span.Length, values[0], span.Length);
    }
}
#endif
