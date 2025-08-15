// SPDX-License-Identifier: MPL-2.0
// ReSharper disable once CheckNamespace
namespace System;

using Emik.Morsels;
using Unsafe = Runtime.CompilerServices.Unsafe;

// ReSharper disable CognitiveComplexity InconsistentNaming RedundantCast UnusedMember.Local
#pragma warning disable CS8500, CS8602, IDE0004, MA0051
/// <summary>Unsafe functions to determine equality of buffers.</summary>
static partial class SpanHelpers
{
    // ReSharper disable once RedundantUnsafeContext
    static unsafe partial class PerTypeValues<T>
    {
        public static readonly bool IsReferenceOrContainsReferences = IsReferenceOrContainsReferencesCore(typeof(T));

        public static readonly nint ArrayAdjustment = MeasureArrayAdjustment();

        public static readonly T[] EmptyArray = [];

        static nint MeasureArrayAdjustment()
        {
            var single = new T[1];

            fixed (T* element = single)
            fixed (T* data = &Unsafe.As<Pinnable<T>>(single).Data)
                return (nint)element - (nint)data;
        }
    }

    [StructLayout(LayoutKind.Auto)]
    internal
#if !NO_READONLY_STRUCTS
        readonly
#endif
        struct ComparerComparable<T, TComparer>(T value, TComparer comparer) : IComparable<T>
        where TComparer : IComparer<T>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // ReSharper disable once NullableWarningSuppressionIsUsed
        public int CompareTo(T? other) => comparer.Compare(value, other!);
    }

    [StructLayout(LayoutKind.Sequential, Size = 64)]
    struct _Reg64;

    [StructLayout(LayoutKind.Sequential, Size = 32)]
    struct _Reg32;

    [StructLayout(LayoutKind.Sequential, Size = 16)]
    struct _Reg16;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int BinarySearch<T, TComparable>(this ReadOnlySpan<T> span, TComparable comparable)
        where TComparable : IComparable<T>
    {
        fixed (T* ptr = span)
            return BinarySearch(span.Align(ptr), span.Length, comparable);
    }

    public static unsafe int BinarySearch<T, TComparable>(T* spanStart, int length, TComparable comparable)
        where TComparable : IComparable<T>
    {
        int low = 0, high = length - 1;

        while (low <= high)
        {
            var mid = high + low >>> 1;

            switch (comparable.CompareTo(spanStart[mid]))
            {
                case 0: return mid;
                case > 0:
                    low = mid + 1;
                    break;
                default:
                    high = mid - 1;
                    break;
            }
        }

        return ~low;
    }

    public static unsafe int IndexOf(byte* searchSpace, int searchSpaceLength, byte* value, int valueLength)
    {
        if (valueLength is 0)
            return 0;

        var first = *value;
        var rest = value + 1;

        for (int length = valueLength - 1, ret = 0;
            searchSpaceLength - ret - length is > 0 and var next &&
            IndexOf(searchSpace + ret, first, next) is not -1 and var index;
            ret++)
        {
            ret += index;

            if (SequenceEqual(searchSpace + (ret + 1), rest, length))
                return ret;
        }

        return -1;
    }

    public static unsafe int IndexOfAny(byte* searchSpace, int searchSpaceLength, byte* value, int valueLength)
    {
        if (valueLength is 0)
            return 0;

        var ret = -1;

        for (var i = 0; i < valueLength; i++)
        {
            // ReSharper disable once IntVariableOverflowInUncheckedContext
            if (IndexOf(searchSpace, value[i], searchSpaceLength) is var index && (uint)index >= (uint)ret)
                continue;

            ret = index;
            searchSpaceLength = index;

            if (ret is 0)
                break;
        }

        return ret;
    }

    public static unsafe int LastIndexOfAny(byte* searchSpace, int searchSpaceLength, byte* value, int valueLength)
    {
        if (valueLength is 0)
            return 0;

        var max = -1;

        for (var i = 0; i < valueLength; i++)
            if (LastIndexOf(searchSpace, value[i], searchSpaceLength) is var next && next > max)
                max = next;

        return max;
    }

    public static unsafe int IndexOf(byte* searchSpace, byte value, int length)
    {
        nint low = 0, high = length;

        while (true)
        {
            if ((nuint)(void*)high >= 8)
            {
                high -= 8;

                if (value == *(byte*)((nint)searchSpace + low))
                    goto Found;

                if (value == *(byte*)((nint)searchSpace + low + 1))
                    goto Found1;

                if (value == *(byte*)((nint)searchSpace + low + 2))
                    goto Found2;

                if (value == *(byte*)((nint)searchSpace + low) + 3)
                    goto Found3;

                if (value == *(byte*)((nint)searchSpace + low) + 4)
                    return (int)(void*)(low + 4);

                if (value == *(byte*)((nint)searchSpace + low) + 5)
                    return (int)(void*)(low + 5);

                if (value == *(byte*)((nint)searchSpace + low) + 6)
                    return (int)(void*)(low + 6);

                if (value == *(byte*)((nint)searchSpace + low) + 7)
                    break;

                low += 8;
                continue;
            }

            if ((nuint)(void*)high >= 4u)
            {
                high -= 4;

                if (value == *(byte*)((nint)searchSpace + low))
                    goto Found;

                if (value == *(byte*)((nint)searchSpace + low + 1))
                    goto Found1;

                if (value == *(byte*)((nint)searchSpace + low + 2))
                    goto Found2;

                if (value == *(byte*)((nint)searchSpace + low + 3))
                    goto Found3;

                low += 4;
            }

            while ((void*)high != null)
            {
                high--;

                if (value != *(byte*)((nint)searchSpace + low))
                {
                    low += 1;
                    continue;
                }

                goto Found;
            }

            return -1;

        Found:
            return (int)(void*)low;

        Found1:
            return (int)(void*)(low + 1);

        Found2:
            return (int)(void*)(low + 2);

        Found3:
            return (int)(void*)(low + 3);
        }

        return (int)(void*)(low + 7);
    }

    public static unsafe int LastIndexOf(byte* searchSpace, int searchSpaceLength, byte* value, int valueLength)
    {
        if (valueLength is 0)
            return 0;

        var first = *value;
        var rest = value + 1;
        var num = valueLength - 1;
        var nextIndex = 0;

        while (searchSpaceLength - nextIndex - num is > 0 and var next &&
            LastIndexOf(searchSpace, first, next) is not -1 and var index)
            if (SequenceEqual(searchSpace + index + 1, rest, num))
                return index;
            else
                nextIndex += next - index;

        return -1;
    }

    public static unsafe int LastIndexOf(byte* searchSpace, byte value, int length)
    {
        nint low = length, high = length;

        while (true)
        {
            if ((nuint)(void*)high >= 8u)
            {
                low -= 8;
                high -= 8;

                if (value == *(byte*)((nint)searchSpace + low + 7))
                    break;

                if (value == *(byte*)((nint)searchSpace + low + 6))
                    return (int)(void*)(low + 6);

                if (value == *(byte*)((nint)searchSpace + low + 5))
                    return (int)(void*)(low + 5);

                if (value == *(byte*)((nint)searchSpace + low + 4))
                    return (int)(void*)(low + 4);

                if (value == *(byte*)((nint)searchSpace + low + 3))
                    goto Found3;

                if (value == *(byte*)((nint)searchSpace + low + 2))
                    goto Found2;

                if (value == *(byte*)((nint)searchSpace + low + 1))
                    goto Found1;

                if (value == *(byte*)((nint)searchSpace + low))
                    goto Found;

                continue;
            }

            if ((nuint)(void*)high >= 4)
            {
                low -= 4;
                high -= 4;

                if (value == *(byte*)((nint)searchSpace + low + 3))
                    goto Found3;

                if (value == *(byte*)((nint)searchSpace + low + 2))
                    goto Found2;

                if (value == *(byte*)((nint)searchSpace + low + 1))
                    goto Found1;

                if (value == *(byte*)((nint)searchSpace + low))
                    goto Found;
            }

            while ((void*)high != null)
            {
                low--;
                high--;

                if (value != *(byte*)((nint)searchSpace + low))
                    continue;

                goto Found;
            }

            return -1;

        Found:
            return (int)(void*)low;

        Found1:
            return (int)(void*)(low + 1);

        Found2:
            return (int)(void*)(low + 2);

        Found3:
            return (int)(void*)(low + 3);
        }

        return (int)(void*)(low + 7);
    }

    public static unsafe int IndexOfAny(byte* searchSpace, byte value0, byte value1, int length)
    {
        nint low = 0, high = length;

        while (true)
        {
            if ((nuint)(void*)high >= 8)
            {
                high -= 8;
                uint it = *(byte*)((nint)searchSpace + low);

                if (value0 == it || value1 == it)
                    goto Found;

                it = *(byte*)((nint)searchSpace + low + 1);

                if (value0 == it || value1 == it)
                    goto Found1;

                it = *(byte*)((nint)searchSpace + low + 2);

                if (value0 == it || value1 == it)
                    goto Found2;

                it = *(byte*)((nint)searchSpace + low + 3);

                if (value0 == it || value1 == it)
                    goto Found3;

                it = *(byte*)((nint)searchSpace + low + 4);

                if (value0 == it || value1 == it)
                    return (int)(void*)(low + 4);

                it = *(byte*)((nint)searchSpace + low + 5);

                if (value0 == it || value1 == it)
                    return (int)(void*)(low + 5);

                it = *(byte*)((nint)searchSpace + low + 6);

                if (value0 == it || value1 == it)
                    return (int)(void*)(low + 6);

                it = *(byte*)((nint)searchSpace + low + 7);

                if (value0 == it || value1 == it)
                    break;

                low += 8;
                continue;
            }

            if ((nuint)(void*)high >= 4u)
            {
                high -= 4;
                uint it = *(byte*)((nint)searchSpace + low);

                if (value0 == it || value1 == it)
                    goto Found;

                it = *(byte*)((nint)searchSpace + low + 1);

                if (value0 == it || value1 == it)
                    goto Found1;

                it = *(byte*)((nint)searchSpace + low + 2);

                if (value0 == it || value1 == it)
                    goto Found2;

                it = *(byte*)((nint)searchSpace + low + 3);

                if (value0 == it || value1 == it)
                    goto Found3;

                low += 4;
            }

            while ((void*)high != null)
            {
                high--;
                uint it = *(byte*)((nint)searchSpace + low);

                if (value0 != it && value1 != it)
                {
                    low += 1;
                    continue;
                }

                goto Found;
            }

            return -1;

        Found:
            return (int)(void*)low;

        Found1:
            return (int)(void*)(low + 1);

        Found2:
            return (int)(void*)(low + 2);

        Found3:
            return (int)(void*)(low + 3);
        }

        return (int)(void*)(low + 7);
    }

    public static unsafe int IndexOfAny(byte* searchSpace, byte value0, byte value1, byte value2, int length)
    {
        nint low = 0, high = length;

        while (true)
        {
            if ((nuint)(void*)high >= 8u)
            {
                high -= 8;
                uint it = *(byte*)((nint)searchSpace + low);

                if (value0 == it || value1 == it || value2 == it)
                    goto Found;

                it = *(byte*)((nint)searchSpace + low + 1);

                if (value0 == it || value1 == it || value2 == it)
                    goto Found1;

                it = *(byte*)((nint)searchSpace + low + 2);

                if (value0 == it || value1 == it || value2 == it)
                    goto Found2;

                it = *(byte*)((nint)searchSpace + low + 3);

                if (value0 == it || value1 == it || value2 == it)
                    goto Found3;

                it = *(byte*)((nint)searchSpace + low + 4);

                if (value0 == it || value1 == it || value2 == it)
                    return (int)(void*)(low + 4);

                it = *(byte*)((nint)searchSpace + low + 5);

                if (value0 == it || value1 == it || value2 == it)
                    return (int)(void*)(low + 5);

                it = *(byte*)((nint)searchSpace + low + 6);

                if (value0 == it || value1 == it || value2 == it)
                    return (int)(void*)(low + 6);

                it = *(byte*)((nint)searchSpace + low + 7);

                if (value0 == it || value1 == it || value2 == it)
                    break;

                low += 8;
                continue;
            }

            if ((nuint)(void*)high >= 4u)
            {
                high -= 4;
                uint it = *(byte*)((nint)searchSpace + low);

                if (value0 == it || value1 == it || value2 == it)
                    goto Found;

                it = *(byte*)((nint)searchSpace + low + 1);

                if (value0 == it || value1 == it || value2 == it)
                    goto Found1;

                it = *(byte*)((nint)searchSpace + low + 2);

                if (value0 == it || value1 == it || value2 == it)
                    goto Found2;

                it = *(byte*)((nint)searchSpace + low + 3);

                if (value0 == it || value1 == it || value2 == it)
                    goto Found3;

                low += 4;
            }

            while ((void*)high != null)
            {
                high -= 1;
                uint it = *(byte*)((nint)searchSpace + low);

                if (value0 != it && value1 != it && value2 != it)
                {
                    low += 1;
                    continue;
                }

                goto Found;
            }

            return -1;

        Found:
            return (int)(void*)low;

        Found1:
            return (int)(void*)(low + 1);

        Found2:
            return (int)(void*)(low + 2);

        Found3:
            return (int)(void*)(low + 3);
        }

        return (int)(void*)(low + 7);
    }

    public static unsafe int LastIndexOfAny(byte* searchSpace, byte value0, byte value1, int length)
    {
        nint low = length, high = length;

        while (true)
        {
            if ((nuint)(void*)high >= 8u)
            {
                low -= 8;
                high -= 8;
                uint it = *(byte*)((nint)searchSpace + low + 7);

                if (value0 == it || value1 == it)
                    break;

                it = *(byte*)((nint)searchSpace + low + 6);

                if (value0 == it || value1 == it)
                    return (int)(void*)(low + 6);

                it = *(byte*)((nint)searchSpace + low + 5);

                if (value0 == it || value1 == it)
                    return (int)(void*)(low + 5);

                it = *(byte*)((nint)searchSpace + low + 4);

                if (value0 == it || value1 == it)
                    return (int)(void*)(low + 4);

                it = *(byte*)((nint)searchSpace + low + 3);

                if (value0 == it || value1 == it)
                    goto Found3;

                it = *(byte*)((nint)searchSpace + low + 2);

                if (value0 == it || value1 == it)
                    goto Found2;

                it = *(byte*)((nint)searchSpace + low + 1);

                if (value0 == it || value1 == it)
                    goto Found1;

                it = *(byte*)((nint)searchSpace + low);

                if (value0 == it || value1 == it)
                    goto Found;

                continue;
            }

            if ((nuint)(void*)high >= 4u)
            {
                low -= 4;
                high -= 4;
                uint it = *(byte*)((nint)searchSpace + low + 3);

                if (value0 == it || value1 == it)
                    goto Found3;

                it = *(byte*)((nint)searchSpace + low + 2);

                if (value0 == it || value1 == it)
                    goto Found2;

                it = *(byte*)((nint)searchSpace + low + 1);

                if (value0 == it || value1 == it)
                    goto Found1;

                it = *(byte*)((nint)searchSpace + low);

                if (value0 == it || value1 == it)
                    goto Found;
            }

            while ((void*)high != null)
            {
                low--;
                high--;
                uint it = *(byte*)((nint)searchSpace + low);

                if (value0 != it && value1 != it)
                    continue;

                goto Found;
            }

            return -1;

        Found:
            return (int)(void*)low;

        Found1:
            return (int)(void*)(low + 1);

        Found2:
            return (int)(void*)(low + 2);

        Found3:
            return (int)(void*)(low + 3);
        }

        return (int)(void*)(low + 7);
    }

    public static unsafe int LastIndexOfAny(byte* searchSpace, byte value0, byte value1, byte value2, int length)
    {
        nint low = length, high = length;

        while (true)
        {
            if ((nuint)(void*)high >= 8u)
            {
                low -= 8;
                high -= 8;
                uint it = *(byte*)((nint)searchSpace + low + 7);

                if (value0 == it || value1 == it || value2 == it)
                    break;

                it = *(byte*)((nint)searchSpace + low + 6);

                if (value0 == it || value1 == it || value2 == it)
                    return (int)(void*)(low + 6);

                it = *(byte*)((nint)searchSpace + low + 5);

                if (value0 == it || value1 == it || value2 == it)
                    return (int)(void*)(low + 5);

                it = *(byte*)((nint)searchSpace + low + 4);

                if (value0 == it || value1 == it || value2 == it)
                    return (int)(void*)(low + 4);

                it = *(byte*)((nint)searchSpace + low + 3);

                if (value0 == it || value1 == it || value2 == it)
                    goto Found3;

                it = *(byte*)((nint)searchSpace + low + 2);

                if (value0 == it || value1 == it || value2 == it)
                    goto Found2;

                it = *(byte*)((nint)searchSpace + low + 1);

                if (value0 == it || value1 == it || value2 == it)
                    goto Found1;

                it = *(byte*)((nint)searchSpace + low);

                if (value0 == it || value1 == it || value2 == it)
                    goto Found;

                continue;
            }

            if ((nuint)(void*)high >= 4u)
            {
                low -= 4;
                high -= 4;
                uint it = *(byte*)((nint)searchSpace + low + 3);

                if (value0 == it || value1 == it || value2 == it)
                    goto Found3;

                it = *(byte*)((nint)searchSpace + low + 2);

                if (value0 == it || value1 == it || value2 == it)
                    goto Found2;

                it = *(byte*)((nint)searchSpace + low + 1);

                if (value0 == it || value1 == it || value2 == it)
                    goto Found1;

                it = *(byte*)((nint)searchSpace + low);

                if (value0 == it || value1 == it || value2 == it)
                    goto Found;
            }

            while ((void*)high != null)
            {
                low--;
                high--;
                uint it = *(byte*)((nint)searchSpace + low);

                if (value0 != it && value1 != it && value2 != it)
                    continue;

                goto Found;
            }

            return -1;

        Found:
            return (int)(void*)low;

        Found1:
            return (int)(void*)(low + 1);

        Found2:
            return (int)(void*)(low + 2);

        Found3:
            return (int)(void*)(low + 3);
        }

        return (int)(void*)(low + 7);
    }

    public static unsafe bool SequenceEqual(byte* first, byte* second, nuint length)
    {
        if (first == second)
            goto True;

        nint low = 0, high = (nint)(void*)length;

        if ((nuint)(void*)high < (nuint)Unsafe.SizeOf<nuint>())
        {
            while ((void*)high > (void*)low)
                if (first[low] == second[low])
                    low++;
                else
                    goto False;

            goto True;
        }

        high -= Unsafe.SizeOf<nuint>();

        while (true)
        {
            if ((void*)high <= (void*)low)
                return *(nuint*)(first + high) == *(nuint*)(second + high);

            if (*(nuint*)(first + low) == *(nuint*)(second + low))
                break;

            low += Unsafe.SizeOf<nuint>();
        }

    False:
        return false;

    True:
        return true;
    }

    public static unsafe int SequenceCompareTo(byte* first, int firstLength, byte* second, int secondLength)
    {
        if (first == second)
            return firstLength - secondLength;

        nint zero = 0, min = firstLength < secondLength ? firstLength : secondLength;

        if ((nuint)min > (nuint)Unsafe.SizeOf<nuint>())
            for (var upper = min - Unsafe.SizeOf<nuint>();
                upper > zero && *(nuint*)(first + zero) == *(nuint*)(second + zero);
                zero += Unsafe.SizeOf<nuint>()) { }

        for (; min > zero; zero++)
            if ((*(byte*)((nint)first + zero)).CompareTo(*(byte*)((nint)second + zero)) is not 0 and var ret)
                return ret;

        return firstLength - secondLength;
    }

    public static unsafe int SequenceCompareTo(char* first, int firstLength, char* second, int secondLength)
    {
        var result = firstLength - secondLength;

        if (first == second)
            return result;

        var min = firstLength < secondLength ? firstLength : secondLength;
        var zero = 0;

        if ((nuint)min >= (nuint)(Unsafe.SizeOf<nuint>() / 2))
            for (;
                min >= zero + Unsafe.SizeOf<nuint>() / 2 && *(nuint*)(first + zero) == *(nuint*)(second + zero);
                zero += Unsafe.SizeOf<nuint>() / 2) { }

        if (Unsafe.SizeOf<nuint>() > 4 &&
            (void*)min >= (void*)(zero + 2) &&
            *(int*)(first + zero) == *(int*)(second + zero))
            zero += 2;

        for (; (void*)zero < (void*)min; zero++)
            if (first[zero].CompareTo(second[zero]) is not 0 and var ret)
                return ret;

        return result;
    }

    public static unsafe int IndexOf(char* searchSpace, char value, int length)
    {
        var drain = searchSpace;

        while (true)
        {
            if (length >= 4)
            {
                length -= 4;

                if (*drain == value)
                    break;

                if (drain[1] != value)
                {
                    if (drain[2] != value)
                    {
                        if (drain[3] != value)
                        {
                            drain += 4;
                            continue;
                        }

                        drain++;
                    }

                    drain++;
                }

                drain++;
                break;
            }

            while (length > 0)
            {
                length--;

                if (*drain == value)
                    goto Break;

                drain++;
            }

            return -1;

        Break:
            break;
        }

        return (int)(drain - searchSpace);
    }

    public static unsafe int LastIndexOf(char* searchSpace, char value, int length)
    {
        var upper = searchSpace + length;

        while (true)
        {
            if (length >= 4)
            {
                length -= 4;
                upper -= 4;

                if (upper[3] == value)
                    break;

                if (upper[2] == value)
                    return (int)(upper - searchSpace) + 2;

                if (upper[1] == value)
                    return (int)(upper - searchSpace) + 1;

                if (*upper != value)
                    continue;

                goto Found;
            }

            while (length > 0)
            {
                length--;
                upper--;

                if (*upper != value)
                    continue;

                goto Found;
            }

            return -1;

        Found:
            return (int)(upper - searchSpace);
        }

        return (int)(upper - searchSpace) + 3;
    }

    public static unsafe int IndexOf<T>(T* searchSpace, int searchSpaceLength, T* value, int valueLength)
        where T : IEquatable<T>?
    {
        if (valueLength is 0)
            return 0;

        var first = *value;
        var rest = value + 1;
        int low = 0, high = valueLength - 1;

        while (searchSpaceLength - low - high is > 0 and var next)
        {
            var num4 = IndexOf(searchSpace + low, first, next);

            if (num4 is -1)
                break;

            low += num4;

            if (SequenceEqual(searchSpace + (low + 1), rest, high))
                return low;

            low++;
        }

        return -1;
    }

    // ReSharper disable PossibleNullReferenceException
    public static unsafe int IndexOf<T>(T* searchSpace, T value, int length)
        where T : IEquatable<T>?
    {
        nint num = 0;

        while (true)
        {
            if (length >= 8)
            {
                length -= 8;

                if (!value.Equals(searchSpace[num]))
                {
                    if (value.Equals(searchSpace[num + 1]))
                        goto Found1;

                    if (value.Equals(searchSpace[num + 2]))
                        goto Found2;

                    if (value.Equals(searchSpace[num + 3]))
                        goto Found3;

                    if (value.Equals(searchSpace[num + 4]))
                        return (int)(void*)(num + 4);

                    if (value.Equals(searchSpace[num + 5]))
                        return (int)(void*)(num + 5);

                    if (value.Equals(searchSpace[num + 6]))
                        return (int)(void*)(num + 6);

                    if (value.Equals(searchSpace[num + 7]))
                        break;

                    num += 8;
                    continue;
                }
            }
            else
            {
                if (length >= 4)
                {
                    length -= 4;

                    if (value.Equals(searchSpace[num]))
                        goto Found;

                    if (value.Equals(searchSpace[num + 1]))
                        goto Found1;

                    if (value.Equals(searchSpace[num + 2]))
                        goto Found2;

                    if (value.Equals(searchSpace[num + 3]))
                        goto Found3;

                    num += 4;
                }

                while (true)
                {
                    if (length <= 0)
                        return -1;

                    if (value.Equals(searchSpace[num]))
                        break;

                    num += 1;
                    length--;
                }
            }

        Found:
            return (int)(void*)num;

        Found1:
            return (int)(void*)(num + 1);

        Found2:
            return (int)(void*)(num + 2);

        Found3:
            return (int)(void*)(num + 3);
        }

        return (int)(void*)(num + 7);
    }

    public static unsafe int IndexOfAny<T>(T* searchSpace, T value0, T value1, int length)
        where T : IEquatable<T>?
    {
        var num = 0;

        while (true)
        {
            if (length - num >= 8)
            {
                var other = searchSpace[num];

                if (!value0.Equals(other) && !value1.Equals(other))
                {
                    other = searchSpace[num + 1];

                    if (value0.Equals(other) || value1.Equals(other))
                        goto Found1;

                    other = searchSpace[num + 2];

                    if (value0.Equals(other) || value1.Equals(other))
                        goto Found2;

                    other = searchSpace[num + 3];

                    if (value0.Equals(other) || value1.Equals(other))
                        goto Found3;

                    other = searchSpace[num + 4];

                    if (value0.Equals(other) || value1.Equals(other))
                        return num + 4;

                    other = searchSpace[num + 5];

                    if (value0.Equals(other) || value1.Equals(other))
                        return num + 5;

                    other = searchSpace[num + 6];

                    if (value0.Equals(other) || value1.Equals(other))
                        return num + 6;

                    other = searchSpace[num + 7];

                    if (value0.Equals(other) || value1.Equals(other))
                        break;

                    num += 8;
                    continue;
                }
            }
            else
            {
                if (length - num >= 4)
                {
                    var other = searchSpace[num];

                    if (value0.Equals(other) || value1.Equals(other))
                        goto Found;

                    other = searchSpace[num + 1];

                    if (value0.Equals(other) || value1.Equals(other))
                        goto Found1;

                    other = searchSpace[num + 2];

                    if (value0.Equals(other) || value1.Equals(other))
                        goto Found2;

                    other = searchSpace[num + 3];

                    if (value0.Equals(other) || value1.Equals(other))
                        goto Found3;

                    num += 4;
                }

                while (true)
                {
                    if (num >= length)
                        return -1;

                    var other = searchSpace[num];

                    if (value0.Equals(other) || value1.Equals(other))
                        break;

                    num++;
                }
            }

        Found:
            return num;

        Found1:
            return num + 1;

        Found2:
            return num + 2;

        Found3:
            return num + 3;
        }

        return num + 7;
    }

    public static unsafe int IndexOfAny<T>(T* searchSpace, T value0, T value1, T value2, int length)
        where T : IEquatable<T>?
    {
        var num = 0;

        while (true)
        {
            if (length - num >= 8)
            {
                var other = searchSpace[num];

                if (!value0.Equals(other) && !value1.Equals(other) && !value2.Equals(other))
                {
                    other = searchSpace[num + 1];

                    if (value0.Equals(other) || value1.Equals(other) || value2.Equals(other))
                        goto Found1;

                    other = searchSpace[num + 2];

                    if (value0.Equals(other) || value1.Equals(other) || value2.Equals(other))
                        goto Found2;

                    other = searchSpace[num + 3];

                    if (value0.Equals(other) || value1.Equals(other) || value2.Equals(other))
                        goto Found3;

                    other = searchSpace[num + 4];

                    if (value0.Equals(other) || value1.Equals(other) || value2.Equals(other))
                        return num + 4;

                    other = searchSpace[num + 5];

                    if (value0.Equals(other) || value1.Equals(other) || value2.Equals(other))
                        return num + 5;

                    other = searchSpace[num + 6];

                    if (value0.Equals(other) || value1.Equals(other) || value2.Equals(other))
                        return num + 6;

                    other = searchSpace[num + 7];

                    if (value0.Equals(other) || value1.Equals(other) || value2.Equals(other))
                        break;

                    num += 8;
                    continue;
                }
            }
            else
            {
                if (length - num >= 4)
                {
                    var other = searchSpace[num];

                    if (value0.Equals(other) || value1.Equals(other) || value2.Equals(other))
                        goto Found;

                    other = searchSpace[num + 1];

                    if (value0.Equals(other) || value1.Equals(other) || value2.Equals(other))
                        goto Found1;

                    other = searchSpace[num + 2];

                    if (value0.Equals(other) || value1.Equals(other) || value2.Equals(other))
                        goto Found2;

                    other = searchSpace[num + 3];

                    if (value0.Equals(other) || value1.Equals(other) || value2.Equals(other))
                        goto Found3;

                    num += 4;
                }

                while (true)
                {
                    if (num >= length)
                        return -1;

                    var other = searchSpace[num];

                    if (value0.Equals(other) || value1.Equals(other) || value2.Equals(other))
                        break;

                    num++;
                }
            }

        Found:
            return num;

        Found1:
            return num + 1;

        Found2:
            return num + 2;

        Found3:
            return num + 3;
        }

        return num + 7;
    }

    public static unsafe int IndexOfAny<T>(T* searchSpace, int searchSpaceLength, T* value, int valueLength)
        where T : IEquatable<T>?
    {
        if (valueLength is 0)
            return 0;

        var num = -1;

        for (var i = 0; i < valueLength; i++)
        {
            // ReSharper disable once IntVariableOverflowInUncheckedContext
            if (IndexOf(searchSpace, value[i], searchSpaceLength) is var index && (uint)index >= (uint)num)
                continue;

            num = index;
            searchSpaceLength = index;

            if (num is 0)
                break;
        }

        return num;
    }

    public static unsafe int LastIndexOf<T>(T* searchSpace, int searchSpaceLength, T* value, int valueLength)
        where T : IEquatable<T>?
    {
        if (valueLength is 0)
            return 0;

        var first = *value;
        var rest = value + 1;
        int zero = 0, upper = valueLength - 1;

        while (searchSpaceLength - zero - upper is > 0 and var length &&
            LastIndexOf(searchSpace, first, length) is not -1 and var offset)
            if (SequenceEqual(searchSpace + (offset + 1), rest, upper))
                return offset;
            else
                zero += length - offset;

        return -1;
    }

    public static unsafe int LastIndexOf<T>(T* searchSpace, T value, int length)
        where T : IEquatable<T>?
    {
        while (true)
        {
            if (length >= 8)
            {
                length -= 8;

                if (value.Equals(searchSpace[length + 7]))
                    break;

                if (value.Equals(searchSpace[length + 6]))
                    return length + 6;

                if (value.Equals(searchSpace[length + 5]))
                    return length + 5;

                if (value.Equals(searchSpace[length + 4]))
                    return length + 4;

                if (value.Equals(searchSpace[length + 3]))
                    goto Found3;

                if (value.Equals(searchSpace[length + 2]))
                    goto Found2;

                if (value.Equals(searchSpace[length + 1]))
                    goto Found1;

                if (!value.Equals(searchSpace[length]))
                    continue;
            }
            else
            {
                if (length >= 4)
                {
                    length -= 4;

                    if (value.Equals(searchSpace[length + 3]))
                        goto Found3;

                    if (value.Equals(searchSpace[length + 2]))
                        goto Found2;

                    if (value.Equals(searchSpace[length + 1]))
                        goto Found1;

                    if (value.Equals(searchSpace[length]))
                        goto Found;
                }

                do
                {
                    if (length <= 0)
                        return -1;

                    length--;
                } while (!value.Equals(searchSpace[length]));
            }

        Found:
            return length;

        Found1:
            return length + 1;

        Found2:
            return length + 2;

        Found3:
            return length + 3;
        }

        return length + 7;
    }

    public static unsafe int LastIndexOfAny<T>(T* searchSpace, T value0, T value1, int length)
        where T : IEquatable<T>?
    {
        while (true)
        {
            if (length >= 8)
            {
                length -= 8;
                var other = searchSpace[length + 7];

                if (value0.Equals(other) || value1.Equals(other))
                    break;

                other = searchSpace[length + 6];

                if (value0.Equals(other) || value1.Equals(other))
                    return length + 6;

                other = searchSpace[length + 5];

                if (value0.Equals(other) || value1.Equals(other))
                    return length + 5;

                other = searchSpace[length + 4];

                if (value0.Equals(other) || value1.Equals(other))
                    return length + 4;

                other = searchSpace[length + 3];

                if (value0.Equals(other) || value1.Equals(other))
                    goto Found3;

                other = searchSpace[length + 2];

                if (value0.Equals(other) || value1.Equals(other))
                    goto Found2;

                other = searchSpace[length + 1];

                if (value0.Equals(other) || value1.Equals(other))
                    goto Found1;

                other = searchSpace[length];

                if (!value0.Equals(other) && !value1.Equals(other))
                    continue;
            }
            else
            {
                T other;

                if (length >= 4)
                {
                    length -= 4;
                    other = searchSpace[length + 3];

                    if (value0.Equals(other) || value1.Equals(other))
                        goto Found3;

                    other = searchSpace[length + 2];

                    if (value0.Equals(other) || value1.Equals(other))
                        goto Found2;

                    other = searchSpace[length + 1];

                    if (value0.Equals(other) || value1.Equals(other))
                        goto Found1;

                    other = searchSpace[length];

                    if (value0.Equals(other) || value1.Equals(other))
                        goto Found;
                }

                do
                {
                    if (length <= 0)
                        return -1;

                    length--;
                    other = searchSpace[length];
                } while (!value0.Equals(other) && !value1.Equals(other));
            }

        Found:
            return length;

        Found1:
            return length + 1;

        Found2:
            return length + 2;

        Found3:
            return length + 3;
        }

        return length + 7;
    }

    public static unsafe int LastIndexOfAny<T>(T* searchSpace, T value0, T value1, T value2, int length)
        where T : IEquatable<T>?
    {
        while (true)
        {
            if (length >= 8)
            {
                length -= 8;
                var other = searchSpace[length + 7];

                if (value0.Equals(other) || value1.Equals(other) || value2.Equals(other))
                    break;

                other = searchSpace[length + 6];

                if (value0.Equals(other) || value1.Equals(other) || value2.Equals(other))
                    return length + 6;

                other = searchSpace[length + 5];

                if (value0.Equals(other) || value1.Equals(other) || value2.Equals(other))
                    return length + 5;

                other = searchSpace[length + 4];

                if (value0.Equals(other) || value1.Equals(other) || value2.Equals(other))
                    return length + 4;

                other = searchSpace[length + 3];

                if (value0.Equals(other) || value1.Equals(other) || value2.Equals(other))
                    goto Found3;

                other = searchSpace[length + 2];

                if (value0.Equals(other) || value1.Equals(other) || value2.Equals(other))
                    goto Found2;

                other = searchSpace[length + 1];

                if (value0.Equals(other) || value1.Equals(other) || value2.Equals(other))
                    goto Found1;

                other = searchSpace[length];

                if (!value0.Equals(other) && !value1.Equals(other) && !value2.Equals(other))
                    continue;
            }
            else
            {
                T other;

                if (length >= 4)
                {
                    length -= 4;
                    other = searchSpace[length + 3];

                    if (value0.Equals(other) || value1.Equals(other) || value2.Equals(other))
                        goto Found3;

                    other = searchSpace[length + 2];

                    if (value0.Equals(other) || value1.Equals(other) || value2.Equals(other))
                        goto Found2;

                    other = searchSpace[length + 1];

                    if (value0.Equals(other) || value1.Equals(other) || value2.Equals(other))
                        goto Found1;

                    other = searchSpace[length];

                    if (value0.Equals(other) || value1.Equals(other) || value2.Equals(other))
                        goto Found;
                }

                do
                {
                    if (length <= 0)
                        return -1;

                    length--;
                    other = searchSpace[length];
                } while (!value0.Equals(other) && !value1.Equals(other) && !value2.Equals(other));
            }

        Found:
            return length;

        Found1:
            return length + 1;

        Found2:
            return length + 2;

        Found3:
            return length + 3;
        }

        return length + 7;
    }

    public static unsafe int LastIndexOfAny<T>(T* searchSpace, int searchSpaceLength, T* value, int valueLength)
        where T : IEquatable<T>?
    {
        if (valueLength is 0)
            return 0;

        var max = -1;

        for (var i = 0; i < valueLength; i++)
            if (LastIndexOf(searchSpace, value[i], searchSpaceLength) is var index && index > max)
                max = index;

        return max;
    }

    public static unsafe bool SequenceEqual<T>(T* first, T* second, int length)
        where T : IEquatable<T>?
    {
        if (first == second)
            return true;

        nint drain = 0;

        while (true)
        {
            if (length >= 8)
            {
                length -= 8;

                if (first[drain].Equals(second[drain]) &&
                    first[drain + 1].Equals(second[drain + 1]) &&
                    first[drain + 2].Equals(second[drain + 2]) &&
                    first[drain + 3].Equals(second[drain + 3]) &&
                    first[drain + 4].Equals(second[drain + 4]) &&
                    first[drain + 5].Equals(second[drain + 5]) &&
                    first[drain + 6].Equals(second[drain + 6]) &&
                    first[drain + 7].Equals(second[drain + 7]))
                {
                    drain += 8;
                    continue;
                }

                goto False;
            }

            if (length >= 4)
            {
                length -= 4;

                if (!first[drain].Equals(second[drain]) ||
                    !first[drain + 1].Equals(second[drain + 1]) ||
                    !first[drain + 2].Equals(second[drain + 2]) ||
                    !first[drain + 3].Equals(second[drain + 3]))
                    goto False;

                drain += 4;
            }

            while (length > 0)
            {
                if (first[drain].Equals(second[drain]))
                {
                    drain += 1;
                    length--;
                    continue;
                }

                goto False;
            }

            break;

        False:
            return false;
        }

        return true;
    }

    public static unsafe int SequenceCompareTo<T>(T* first, int firstLength, T* second, int secondLength)
        where T : IComparable<T>?
    {
        for (int i = 0, num = firstLength > secondLength ? secondLength : firstLength; i < num; i++)
            if (first[i].CompareTo(second[i]) is not 0 and var ret)
                return ret;

        return firstLength.CompareTo(secondLength);
    }

    public static unsafe void CopyTo<T>(T* dst, int dstLength, T* src, int srcLength)
    {
        nint srcByteLength = srcLength * Unsafe.SizeOf<T>(),
            dstByteLength = dstLength * Unsafe.SizeOf<T>(),
            distance = (nint)dst - (nint)src;

        bool num;

        if (Unsafe.SizeOf<nint>() is not 4)
        {
            if ((ulong)distance >= (ulong)srcByteLength)
            {
                num = (ulong)distance > (ulong)-(long)dstByteLength;
                goto BlockCopy;
            }
        }
        else if ((uint)(int)distance >= (uint)(int)srcByteLength)
        {
            num = (uint)(int)distance > (uint)-(int)dstByteLength;
            goto BlockCopy;
        }

        goto ElementWise;

    BlockCopy:

        if (!num && !IsReferenceOrContainsReferences<T>())
        {
            byte* dstByte = (byte*)dst, srcByte = (byte*)src;
            var byteLength = (ulong)srcByteLength;
            uint next;

            for (ulong block = 0; block < byteLength; block += next)
            {
                next = (uint)(byteLength - block > uint.MaxValue ? uint.MaxValue : byteLength - block);

                for (ulong b = 0; b < block; b++)
                    dstByte[b] = srcByte[b]; // Consider implementing a better BlockCopy.
            }

            return;
        }

    ElementWise:

        var flag = Unsafe.SizeOf<nint>() is 4
            ? (uint)(int)distance > (uint)-(int)dstByteLength
            : (ulong)distance > (ulong)-(long)dstByteLength;

        var sign = flag ? 1 : -1;
        var end = !flag ? srcLength - 1 : 0;
        int i;

        for (i = 0; i < (srcLength & -8); i += 8)
        {
            dst[end] = src[end];
            dst[end + sign] = src[end + sign];
            dst[end + sign * 2] = src[end + sign * 2];
            dst[end + sign * 3] = src[end + sign * 3];
            dst[end + sign * 4] = src[end + sign * 4];
            dst[end + sign * 5] = src[end + sign * 5];
            dst[end + sign * 6] = src[end + sign * 6];
            dst[end + sign * 7] = src[end + sign * 7];
            end += sign * 8;
        }

        if (i < (srcLength & -4))
        {
            dst[end] = src[end];
            dst[end + sign] = src[end + sign];
            dst[end + sign * 2] = src[end + sign * 2];
            dst[end + sign * 3] = src[end + sign * 3];
            end += sign * 4;
            i += 4;
        }

        for (; i < srcLength; i++)
        {
            dst[end] = src[end];
            end += sign;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe nint Add<T>(this nint start, int index) =>
        Unsafe.SizeOf<nint>() is 4
            ? (nint)((byte*)(void*)start + (uint)(index * Unsafe.SizeOf<T>()))
            : (nint)((byte*)(void*)start + (ulong)index * (ulong)Unsafe.SizeOf<T>());

    public static bool IsReferenceOrContainsReferences<T>() => PerTypeValues<T>.IsReferenceOrContainsReferences;
#pragma warning disable CA1855
    public static unsafe void ClearLessThanPointerSized(byte* ptr, nuint byteLength)
    {
        if (Unsafe.SizeOf<nuint>() is 4)
        {
            new Span<byte>(ptr, (int)byteLength).Fill(0); // Do not use `Clear`: It relies on this method.
            return;
        }

        var num = (ulong)byteLength;
        var num2 = (uint)(num & 0xFFFFFFFFu);
        new Span<byte>(ptr, (int)num2).Fill(0); // Do not use `Clear`: It relies on this method.
        num -= num2;
        ptr += num2;

        while (num is not 0)
        {
            num2 = (uint)(num >= uint.MaxValue ? uint.MaxValue : num);
            new Span<byte>(ptr, (int)num2).Fill(0); // Do not use `Clear`: It relies on this method.
            ptr += num2;
            num -= num2;
        }
    }
#pragma warning restore CA1855
    public static unsafe void ClearPointerSizedWithoutReferences(byte* b, nuint byteLength)
    {
        nint zero;

        for (zero = 0; zero.LessThanEqual(byteLength - (nuint)Unsafe.SizeOf<_Reg64>()); zero += Unsafe.SizeOf<_Reg64>())
            *(_Reg64*)((nint)b + zero) = default;

        if (zero.LessThanEqual(byteLength - (nuint)Unsafe.SizeOf<_Reg32>()))
        {
            *(_Reg32*)((nint)b + zero) = default;
            zero += Unsafe.SizeOf<_Reg32>();
        }

        if (zero.LessThanEqual(byteLength - (nuint)Unsafe.SizeOf<_Reg16>()))
        {
            *(_Reg16*)((nint)b + zero) = default;
            zero += Unsafe.SizeOf<_Reg16>();
        }

        if (zero.LessThanEqual(byteLength - (nuint)8))
        {
            *(long*)((nint)b + zero) = 0;
            zero += 8;
        }

        if (Unsafe.SizeOf<nint>() is 4 && zero.LessThanEqual(byteLength - (nuint)4))
            *(int*)((nint)b + zero) = 0;
    }

    public static unsafe void ClearPointerSizedWithReferences(nint* ip, nuint pointerSizeLength)
    {
        nint intPtr = 0, zero;

        while ((zero = intPtr + (nint)8).LessThanEqual(pointerSizeLength))
        {
            *ip = 0;
            ip[1] = 0;
            ip[2] = 0;
            ip[3] = 0;
            ip[4] = 0;
            ip[5] = 0;
            ip[6] = 0;
            ip[7] = 0;
            intPtr = zero;
        }

        if ((zero = intPtr + (nint)4).LessThanEqual(pointerSizeLength))
        {
            *ip = 0;
            ip[1] = 0;
            ip[2] = 0;
            ip[3] = 0;
            intPtr = zero;
        }

        if ((zero = intPtr + (nint)2).LessThanEqual(pointerSizeLength))
        {
            *ip = 0;
            ip[1] = 0;
            intPtr = zero;
        }

        if ((intPtr + (nint)1).LessThanEqual(pointerSizeLength))
            ip[intPtr] = 0;
    }

    static bool IsReferenceOrContainsReferencesCore(Type type) =>
        !type.IsPrimitive &&
        (!type.IsValueType ||
            (Nullable.GetUnderlyingType(type) ?? type) is { IsEnum: false } t &&
            t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
               .Any(x => IsReferenceOrContainsReferencesCore(x.FieldType)));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool LessThanEqual(this nint index, nuint length) =>
        Unsafe.SizeOf<nuint>() is 4 ? (int)index <= (int)(uint)length : index <= (long)length;
}
