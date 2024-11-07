// SPDX-License-Identifier: MPL-2.0

// ReSharper disable CheckNamespace ConvertToConstant.Local EmptyNamespace WrongIndentSize
namespace System;
#pragma warning disable 8500, RCS1118
#if !(NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) || NO_SYSTEM_MEMORY
using static Span;

/// <summary>Extension methods for <see cref="Span{T}"/>, <c>Memory&lt;T&gt;</c>, and friends.</summary>
static partial class MemoryExtensions
{
    public static nint StringAdjustment = MeasureStringAdjustment();

    public static ReadOnlySpan<char> Trim(this ReadOnlySpan<char> span) => span.TrimStart().TrimEnd();

    public static ReadOnlySpan<char> TrimStart(this ReadOnlySpan<char> span)
    {
        int i;

        for (i = 0; i < span.Length && char.IsWhiteSpace(span[i]); i++) { }

        return span[i..];
    }

    public static ReadOnlySpan<char> TrimEnd(this ReadOnlySpan<char> span)
    {
        var num = span.Length - 1;

        while (num >= 0 && char.IsWhiteSpace(span[num]))
            num--;

        return span[..(num + 1)];
    }

    public static ReadOnlySpan<char> Trim(this ReadOnlySpan<char> span, char trimChar) =>
        span.TrimStart(trimChar).TrimEnd(trimChar);

    public static ReadOnlySpan<char> TrimStart(this ReadOnlySpan<char> span, char trimChar)
    {
        int i;

        for (i = 0; i < span.Length && span[i] == trimChar; i++) { }

        return span[i..];
    }

    public static ReadOnlySpan<char> TrimEnd(this ReadOnlySpan<char> span, char trimChar)
    {
        var num = span.Length - 1;

        while (num >= 0 && span[num] == trimChar)
            num--;

        return span[..(num + 1)];
    }

    public static ReadOnlySpan<char> Trim(this ReadOnlySpan<char> span, ReadOnlySpan<char> trimChars) =>
        span.TrimStart(trimChars).TrimEnd(trimChars);

    public static ReadOnlySpan<char> TrimStart(this ReadOnlySpan<char> span, ReadOnlySpan<char> trimChars)
    {
        if (trimChars.IsEmpty)
            return span.TrimStart();

        int i;

        for (i = 0; i < span.Length; i++)
        {
            var num = 0;

            while (num < trimChars.Length)
            {
                if (span[i] != trimChars[num])
                {
                    num++;
                    continue;
                }

                goto Next;
            }

            break;

        Next: ;
        }

        return span[i..];
    }

    public static ReadOnlySpan<char> TrimEnd(this ReadOnlySpan<char> span, ReadOnlySpan<char> trimChars)
    {
        if (trimChars.IsEmpty)
            return span.TrimEnd();

        int num;

        for (num = span.Length - 1; num >= 0; num--)
        {
            var num2 = 0;

            while (num2 < trimChars.Length)
            {
                if (span[num] != trimChars[num2])
                {
                    num2++;
                    continue;
                }

                goto Next;
            }

            break;

        Next: ;
        }

        return span[..(num + 1)];
    }

    public static bool IsWhiteSpace(this ReadOnlySpan<char> span)
    {
        foreach (var t in span)
            if (!char.IsWhiteSpace(t))
                return false;

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int IndexOf<T>(this Span<T> span, T value)
        where T : IEquatable<T>? =>
        IndexOf(span.ReadOnly(), value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int IndexOf<T>(this Span<T> span, ReadOnlySpan<T> value)
        where T : IEquatable<T>? =>
        IndexOf(span.ReadOnly(), value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int LastIndexOf<T>(this Span<T> span, T value)
        where T : IEquatable<T>? =>
        LastIndexOf(span.ReadOnly(), value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int LastIndexOf<T>(this Span<T> span, ReadOnlySpan<T> value)
        where T : IEquatable<T>? =>
        LastIndexOf(span.ReadOnly(), value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool SequenceEqual<T>(this Span<T> span, ReadOnlySpan<T> other)
        where T : IEquatable<T>? =>
        SequenceEqual(span.ReadOnly(), other);

    public static int SequenceCompareTo<T>(this Span<T> span, ReadOnlySpan<T> other)
        where T : IComparable<T>? =>
        SequenceCompareTo(span.ReadOnly(), other);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int IndexOf<T>(this ReadOnlySpan<T> span, T value)
        where T : IEquatable<T>?
    {
        fixed (T* ptr = span)
            return typeof(T) == typeof(byte) ?
                SpanHelpers.IndexOf((byte*)ptr, Unsafe.As<T, byte>(ref value), span.Length) :
                typeof(T) == typeof(char) ?
                    SpanHelpers.IndexOf((char*)ptr, Unsafe.As<T, char>(ref value), span.Length) :
                    SpanHelpers.IndexOf(ptr, value, span.Length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int IndexOf<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> value)
        where T : IEquatable<T>?
    {
        fixed (T* s = span)
        fixed (T* v = value)
            return typeof(T) == typeof(byte)
                ? SpanHelpers.IndexOf((byte*)s, span.Length, (byte*)v, value.Length)
                : SpanHelpers.IndexOf(s, span.Length, v, value.Length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int LastIndexOf<T>(this ReadOnlySpan<T> span, T value)
        where T : IEquatable<T>?
    {
        fixed (T* ptr = span)
            return typeof(T) == typeof(byte) ?
                SpanHelpers.LastIndexOf((byte*)ptr, Unsafe.As<T, byte>(ref value), span.Length) :
                typeof(T) == typeof(char) ?
                    SpanHelpers.LastIndexOf((char*)ptr, Unsafe.As<T, char>(ref value), span.Length) :
                    SpanHelpers.LastIndexOf(ptr, value, span.Length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int LastIndexOf<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> value)
        where T : IEquatable<T>?
    {
        fixed (T* s = span)
        fixed (T* v = value)
            return typeof(T) == typeof(byte)
                ? SpanHelpers.LastIndexOf((byte*)s, span.Length, (byte*)v, value.Length)
                : SpanHelpers.LastIndexOf(s, span.Length, v, value.Length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int IndexOfAny<T>(this Span<T> span, T value0, T value1)
        where T : IEquatable<T>? =>
        IndexOfAny(span.ReadOnly(), value0, value1);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int IndexOfAny<T>(this Span<T> span, T value0, T value1, T value2)
        where T : IEquatable<T>? =>
        IndexOfAny(span.ReadOnly(), value0, value1, value2);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int IndexOfAny<T>(this Span<T> span, ReadOnlySpan<T> values)
        where T : IEquatable<T>? =>
        IndexOfAny(span.ReadOnly(), values);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int IndexOfAny<T>(this ReadOnlySpan<T> span, T value0, T value1)
        where T : IEquatable<T>?
    {
        fixed (T* ptr = span)
            return typeof(T) == typeof(byte)
                ? SpanHelpers.IndexOfAny(
                    (byte*)ptr,
                    Unsafe.As<T, byte>(ref value0),
                    Unsafe.As<T, byte>(ref value1),
                    span.Length
                )
                : SpanHelpers.IndexOfAny(ptr, value0, value1, span.Length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int IndexOfAny<T>(this ReadOnlySpan<T> span, T value0, T value1, T value2)
        where T : IEquatable<T>?
    {
        fixed (T* ptr = span)
            return typeof(T) == typeof(byte)
                ? SpanHelpers.IndexOfAny(
                    (byte*)ptr,
                    Unsafe.As<T, byte>(ref value0),
                    Unsafe.As<T, byte>(ref value1),
                    Unsafe.As<T, byte>(ref value2),
                    span.Length
                )
                : SpanHelpers.IndexOfAny(ptr, value0, value1, value2, span.Length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int IndexOfAny<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> values)
        where T : IEquatable<T>?
    {
        fixed (T* s = span)
        fixed (T* v = values)
            return typeof(T) == typeof(byte)
                ? SpanHelpers.IndexOfAny((byte*)s, span.Length, (byte*)v, values.Length)
                : SpanHelpers.IndexOfAny(s, span.Length, v, values.Length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int LastIndexOfAny<T>(this Span<T> span, T value0, T value1)
        where T : IEquatable<T>? =>
        LastIndexOfAny(span.ReadOnly(), value0, value1);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int LastIndexOfAny<T>(this Span<T> span, T value0, T value1, T value2)
        where T : IEquatable<T>? =>
        LastIndexOfAny(span.ReadOnly(), value0, value1, value2);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int LastIndexOfAny<T>(this Span<T> span, ReadOnlySpan<T> values)
        where T : IEquatable<T>? =>
        LastIndexOfAny(span.ReadOnly(), values);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int LastIndexOfAny<T>(this ReadOnlySpan<T> span, T value0, T value1)
        where T : IEquatable<T>?
    {
        fixed (T* ptr = span)
            return typeof(T) == typeof(byte)
                ? SpanHelpers.LastIndexOfAny(
                    (byte*)ptr,
                    Unsafe.As<T, byte>(ref value0),
                    Unsafe.As<T, byte>(ref value1),
                    span.Length
                )
                : SpanHelpers.LastIndexOfAny(ptr, value0, value1, span.Length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int LastIndexOfAny<T>(this ReadOnlySpan<T> span, T value0, T value1, T value2)
        where T : IEquatable<T>?
    {
        fixed (T* ptr = span)
            return typeof(T) == typeof(byte)
                ? SpanHelpers.LastIndexOfAny(
                    (byte*)ptr,
                    Unsafe.As<T, byte>(ref value0),
                    Unsafe.As<T, byte>(ref value1),
                    Unsafe.As<T, byte>(ref value2),
                    span.Length
                )
                : SpanHelpers.LastIndexOfAny(ptr, value0, value1, value2, span.Length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int LastIndexOfAny<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> values)
        where T : IEquatable<T>?
    {
        fixed (T* s = span)
        fixed (T* v = values)
            return typeof(T) == typeof(byte)
                ? SpanHelpers.LastIndexOfAny((byte*)s, span.Length, (byte*)v, values.Length)
                : SpanHelpers.LastIndexOfAny(s, span.Length, v, values.Length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe bool SequenceEqual<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> other)
        where T : IEquatable<T>?
    {
        var length = span.Length;

        fixed (T* s = span)
        fixed (T* o = other)
            return length == other.Length &&
                (default(T) is not null && IsTypeComparableAsBytes<T>(out var size)
                    ? SpanHelpers.SequenceEqual((byte*)s, (byte*)o, (nuint)length * size)
                    : SpanHelpers.SequenceEqual(s, o, length));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int SequenceCompareTo<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> other)
        where T : IComparable<T>?
    {
        fixed (T* s = span)
        fixed (T* o = other)
            return typeof(T) == typeof(byte) ?
                SpanHelpers.SequenceCompareTo((byte*)s, span.Length, (byte*)o, other.Length) :
                typeof(T) == typeof(char) ?
                    SpanHelpers.SequenceCompareTo((char*)s, span.Length, (char*)o, other.Length) :
                    SpanHelpers.SequenceCompareTo(s, span.Length, o, other.Length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool StartsWith<T>(this Span<T> span, ReadOnlySpan<T> value)
        where T : IEquatable<T>? =>
        StartsWith(span.ReadOnly(), value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe bool StartsWith<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> value)
        where T : IEquatable<T>?
    {
        var length = value.Length;

        fixed (T* s = span)
        fixed (T* v = value)
            return length <= span.Length &&
                (default(T) is not null && IsTypeComparableAsBytes<T>(out var size)
                    ? SpanHelpers.SequenceEqual((byte*)s, (byte*)v, (nuint)length * size)
                    : SpanHelpers.SequenceEqual(s, v, length));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool EndsWith<T>(this Span<T> span, ReadOnlySpan<T> value)
        where T : IEquatable<T>? =>
        EndsWith(span.ReadOnly(), value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe bool EndsWith<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> value)
        where T : IEquatable<T>?
    {
        var length = span.Length;
        var length2 = value.Length;

        fixed (T* s = span)
        fixed (T* v = value)
            return length2 <= length &&
                (default(T) is not null && IsTypeComparableAsBytes<T>(out var size)
                    ? SpanHelpers.SequenceEqual((byte*)(s + (length - length2)), (byte*)v, (nuint)length2 * size)
                    : SpanHelpers.SequenceEqual(s + (length - length2), v, length2));
    }

    public static unsafe void Reverse<T>(this Span<T> span)
    {
        fixed (T* s = span)
        {
            var num = 0;
            var num2 = span.Length - 1;

            for (; num < num2; num++, num2--)
                (s[num], s[num2]) = (s[num2], s[num]);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> AsSpan<T>(this T[] array) => new(array);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> AsSpan<T>(this T[] array, int start, int length) => new(array, start, length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> AsSpan<T>(this ArraySegment<T> segment) => new(segment.Array, segment.Offset, segment.Count);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> AsSpan<T>(this ArraySegment<T> segment, int start) =>
        (uint)start > segment.Count
            ? throw new ArgumentOutOfRangeException(nameof(start))
            : new(segment.Array, segment.Offset + start, segment.Count - start);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> AsSpan<T>(this ArraySegment<T> segment, int start, int length) =>
        (uint)start > segment.Count ? throw new ArgumentOutOfRangeException(nameof(start)) :
        (uint)length > segment.Count - start ? throw new ArgumentOutOfRangeException(nameof(length)) :
        new(segment.Array, segment.Offset + start, length);

    public static Memory<T> AsMemory<T>(this T[] array) => new(array);

    public static Memory<T> AsMemory<T>(this T[] array, int start) => new(array, start);

    public static Memory<T> AsMemory<T>(this T[] array, int start, int length) => new(array, start, length);

    public static Memory<T> AsMemory<T>(this ArraySegment<T> segment) =>
        new(segment.Array, segment.Offset, segment.Count);

    public static Memory<T> AsMemory<T>(this ArraySegment<T> segment, int start) =>
        (uint)start > segment.Count
            ? throw new ArgumentOutOfRangeException(nameof(start))
            : new(segment.Array, segment.Offset + start, segment.Count - start);

    public static Memory<T> AsMemory<T>(this ArraySegment<T> segment, int start, int length) =>
        (uint)start > segment.Count ? throw new ArgumentOutOfRangeException(nameof(start)) :
        (uint)length > segment.Count - start ? throw new ArgumentOutOfRangeException(nameof(length)) :
        new(segment.Array, segment.Offset + start, length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo<T>(this T[] source, Span<T> destination) => new ReadOnlySpan<T>(source).CopyTo(destination);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo<T>(this T[] source, Memory<T> destination) => source.CopyTo(destination.Span);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Overlaps<T>(this Span<T> span, ReadOnlySpan<T> other) => Overlaps(span.ReadOnly(), other);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Overlaps<T>(this Span<T> span, ReadOnlySpan<T> other, out int elementOffset) =>
        Overlaps(span.ReadOnly(), other, out elementOffset);

    public static unsafe bool Overlaps<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> other)
    {
        if (span.IsEmpty || other.IsEmpty)
            return false;

        fixed (T* s = span)
        fixed (T* o = other)
            return (nint)o - (nint)s is var intPtr && Unsafe.SizeOf<nint>() is 4
                ? (uint)(int)intPtr < (uint)(span.Length * Unsafe.SizeOf<T>()) ||
                (uint)(int)intPtr > (uint)-(other.Length * Unsafe.SizeOf<T>())
                : (ulong)intPtr < (ulong)((long)span.Length * Unsafe.SizeOf<T>()) ||
                (ulong)intPtr > (ulong)-((long)other.Length * Unsafe.SizeOf<T>());
    }

    // ReSharper disable once CognitiveComplexity
    public static unsafe bool Overlaps<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> other, out int elementOffset)
    {
        if (span.IsEmpty || other.IsEmpty)
            return !((elementOffset = 0) is var _);

        const string Message = "Buffers must be aligned to each other.";

        fixed (T* s = span)
        fixed (T* o = other)
            return (nint)o - (nint)s is var intPtr && Unsafe.SizeOf<nint>() is not 4 ?
                (ulong)intPtr < (ulong)((long)span.Length * Unsafe.SizeOf<T>()) ||
                (ulong)intPtr > (ulong)-((long)other.Length * Unsafe.SizeOf<T>())
                    ? (long)intPtr % Unsafe.SizeOf<T>() is not 0
                        ? throw new ArgumentException(Message, nameof(other))
                        : (elementOffset = (int)((long)intPtr / Unsafe.SizeOf<T>())) is var _
                    : !((elementOffset = 0) is var _) :
                (uint)(int)intPtr >= (uint)(span.Length * Unsafe.SizeOf<T>()) &&
                (uint)(int)intPtr <= (uint)-(other.Length * Unsafe.SizeOf<T>()) ? !((elementOffset = 0) is var _) :
                    (int)intPtr % Unsafe.SizeOf<T>() is not 0 ? throw new ArgumentException(Message, nameof(other)) :
                        (elementOffset = (int)intPtr / Unsafe.SizeOf<T>()) is var _;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int BinarySearch<T>(this Span<T> span, IComparable<T> comparable) =>
        BinarySearch<T, IComparable<T>>(span.ReadOnly(), comparable);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int BinarySearch<T, TComparable>(this Span<T> span, TComparable comparable)
        where TComparable : IComparable<T> =>
        BinarySearch(span.ReadOnly(), comparable);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int BinarySearch<T, TComparer>(this Span<T> span, T value, TComparer comparer)
        where TComparer : IComparer<T> =>
        BinarySearch(span.ReadOnly(), value, comparer);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int BinarySearch<T>(this ReadOnlySpan<T> span, IComparable<T> comparable) =>
        BinarySearch<T, IComparable<T>>(span, comparable);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int BinarySearch<T, TComparable>(this ReadOnlySpan<T> span, TComparable comparable)
        where TComparable : IComparable<T> =>
        SpanHelpers.BinarySearch(span, comparable);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int BinarySearch<T, TComparer>(this ReadOnlySpan<T> span, T value, TComparer comparer)
        where TComparer : IComparer<T> =>
        BinarySearch(span, new SpanHelpers.ComparerComparable<T, TComparer>(value, comparer));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool IsTypeComparableAsBytes<T>(out nuint size) =>
        typeof(T) == typeof(byte) || typeof(T) == typeof(sbyte) ? (size = sizeof(byte)) is var _ :
            typeof(T) == typeof(char) || typeof(T) == typeof(short) || typeof(T) == typeof(ushort) ?
                (size = sizeof(short)) is var _ :
                typeof(T) == typeof(int) || typeof(T) == typeof(uint) ? (size = sizeof(int)) is var _ :
                    typeof(T) == typeof(long) || typeof(T) == typeof(ulong) ? (size = sizeof(long)) is var _ :
                        !((size = default) is var _);

    public static Span<T> AsSpan<T>(this T[] array, int start) => Span<T>.Create(array, start);

    public static bool Contains(
        this ReadOnlySpan<char> span,
        ReadOnlySpan<char> value,
        StringComparison comparisonType
    ) =>
        span.IndexOf(value, comparisonType) >= 0;

    public static bool Equals(this ReadOnlySpan<char> span, ReadOnlySpan<char> other, StringComparison comparisonType) =>
        comparisonType switch
        {
            StringComparison.Ordinal => span.SequenceEqual(other),
            StringComparison.OrdinalIgnoreCase =>
                span.Length == other.Length && EqualsOrdinalIgnoreCase(span, other),
            _ => span.ToString().Equals(other.ToString(), comparisonType),
        };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool EqualsOrdinalIgnoreCase(ReadOnlySpan<char> span, ReadOnlySpan<char> other) =>
        other.Length is 0 || CompareToOrdinalIgnoreCase(span, other) is 0;

    public static int CompareTo(this ReadOnlySpan<char> span, ReadOnlySpan<char> other, StringComparison comparisonType) =>
        comparisonType switch
        {
            StringComparison.Ordinal => span.SequenceCompareTo(other),
            StringComparison.OrdinalIgnoreCase => CompareToOrdinalIgnoreCase(span, other),
            _ => string.Compare(span.ToString(), other.ToString(), comparisonType),
        };

    // ReSharper disable once CognitiveComplexity
    static unsafe int CompareToOrdinalIgnoreCase(ReadOnlySpan<char> strA, ReadOnlySpan<char> strB)
    {
        var num = Math.Min(strA.Length, strB.Length);
        var num2 = num;

        fixed (char* fixedA = strA)
        fixed (char* fixedB = strB)
        {
            var a = fixedA;
            var b = fixedB;

            while (num != 0 && *a <= '\u007f' && *b <= '\u007f')
            {
                int num3 = *a;
                int num4 = *b;

                if (num3 == num4)
                {
                    a++;
                    b++;
                    num--;
                    continue;
                }

                if ((uint)(num3 - 97) <= 25u)
                    num3 -= 32;

                if ((uint)(num4 - 97) <= 25u)
                    num4 -= 32;

                if (num3 != num4)
                    return num3 - num4;

                a++;
                b++;
                num--;
            }

            if (num is 0)
                return strA.Length - strB.Length;

            num2 -= num;
            return string.Compare(strA[num2..].ToString(), strB[num2..].ToString(), StringComparison.OrdinalIgnoreCase);
        }
    }

    public static int IndexOf(
        this ReadOnlySpan<char> span,
        ReadOnlySpan<char> value,
        StringComparison comparisonType
    ) =>
        comparisonType is StringComparison.Ordinal
            ? span.IndexOf(value)
            : span.ToString().IndexOf(value.ToString(), comparisonType);

    public static int ToLower(this ReadOnlySpan<char> source, Span<char> destination, CultureInfo culture)
    {
        if (destination.Length < source.Length)
            return -1;

        var text = source.ToString();
        var text2 = text.ToLower(culture);
        AsSpan(text2).CopyTo(destination);
        return source.Length;
    }

    public static int ToLowerInvariant(this ReadOnlySpan<char> source, Span<char> destination) =>
        source.ToLower(destination, CultureInfo.InvariantCulture);

    public static int ToUpper(this ReadOnlySpan<char> source, Span<char> destination, CultureInfo culture)
    {
        if (destination.Length < source.Length)
            return -1;

        var text = source.ToString();
        var text2 = text.ToUpper(culture);
        AsSpan(text2).CopyTo(destination);
        return source.Length;
    }

    public static int ToUpperInvariant(this ReadOnlySpan<char> source, Span<char> destination) =>
        source.ToUpper(destination, CultureInfo.InvariantCulture);

    public static bool EndsWith(
        this ReadOnlySpan<char> span,
        ReadOnlySpan<char> value,
        StringComparison comparisonType
    ) =>
        comparisonType switch
        {
            StringComparison.Ordinal => span.EndsWith(value),
            StringComparison.OrdinalIgnoreCase =>
                value.Length <= span.Length && EqualsOrdinalIgnoreCase(span[^value.Length..], value),
            _ => span.ToString().EndsWith(value.ToString(), comparisonType),
        };

    public static bool StartsWith(
        this ReadOnlySpan<char> span,
        ReadOnlySpan<char> value,
        StringComparison comparisonType
    ) =>
        comparisonType switch
        {
            StringComparison.Ordinal => span.StartsWith(value),
            StringComparison.OrdinalIgnoreCase =>
                value.Length <= span.Length && EqualsOrdinalIgnoreCase(span[..value.Length], value),
            _ => span.ToString().StartsWith(value.ToString(), comparisonType),
        };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<char> AsSpan(this string? text) =>
        text is null ? default : new(Unsafe.As<Pinnable<char>>(text), StringAdjustment, text.Length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<char> AsSpan(this string? text, int start) =>
        text is null ? start is 0 ? default : throw new ArgumentOutOfRangeException(nameof(start)) :
        (uint)start > (uint)text.Length ? throw new ArgumentOutOfRangeException(nameof(start)) :
        new(Unsafe.As<Pinnable<char>>(text), StringAdjustment + start * 2, text.Length - start);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<char> AsSpan(this string? text, int start, int length) =>
        text is null ? start != 0 || length != 0 ? throw new ArgumentOutOfRangeException(nameof(start)) : default :
            (uint)start > (uint)text.Length || (uint)length > (uint)(text.Length - start) ?
                throw new ArgumentOutOfRangeException(nameof(start)) :
                new(Unsafe.As<Pinnable<char>>(text), StringAdjustment + start * 2, length);

    public static ReadOnlyMemory<char> AsMemory(this string? text) =>
        text is null ? default : new(text, 0, text.Length);

    public static ReadOnlyMemory<char> AsMemory(this string? text, int start) =>
        text is null ? start is 0 ? default : throw new ArgumentOutOfRangeException(nameof(start)) :
        (uint)start > (uint)text.Length ? throw new ArgumentOutOfRangeException(nameof(start)) :
        new(text, start, text.Length - start);

    public static ReadOnlyMemory<char> AsMemory(this string? text, int start, int length) =>
        text is null ? start != 0 || length != 0 ? throw new ArgumentOutOfRangeException(nameof(start)) : default :
            (uint)start > (uint)text.Length || (uint)length > (uint)(text.Length - start) ?
                throw new ArgumentOutOfRangeException(nameof(start)) : new(text, start, length);

    static unsafe nint MeasureStringAdjustment()
    {
        var text = "a";

        fixed (char* c = text)
        fixed (char* pin = &Unsafe.As<Pinnable<char>>(text).Data)
            return (nint)(c - pin);
    }
}
#endif
