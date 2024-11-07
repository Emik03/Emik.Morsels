// SPDX-License-Identifier: MPL-2.0
// ReSharper disable once CheckNamespace
namespace System;

using Emik.Morsels;

// ReSharper disable CognitiveComplexity NullableWarningSuppressionIsUsed RedundantCallerArgumentExpressionDefaultValue RedundantSuppressNullableWarningExpression
#pragma warning disable 8500, MA0051
/// <summary>Unsafe functions to determine equality of buffers.</summary>
static partial class SpanHelpers
{
    public static unsafe partial class PerTypeValues<T>
    {
        public static readonly bool IsReferenceOrContainsReferences = IsReferenceOrContainsReferencesCore(typeof(T));

        public static readonly T[] EmptyArray = [];
#if !(NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) || NO_SYSTEM_MEMORY
        public static readonly nint ArrayAdjustment = MeasureArrayAdjustment();

        static nint MeasureArrayAdjustment()
        {
            var array = new T[1];
            var data = (nint)(&Span.Ret<Pinnable<T>>.From(array).Data);

            fixed (T* ptr = array)
                return data - (nint)ptr;
        }
#endif
    }

    [StructLayout(LayoutKind.Auto)]
    internal readonly struct ComparerComparable<T, TComparer>(T value, TComparer comparer) : IComparable<T>
        where TComparer : IComparer<T>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CompareTo(T other) => comparer.Compare(value, other);
    }

    [StructLayout(LayoutKind.Sequential, Size = 64)]
    struct Reg64;

    [StructLayout(LayoutKind.Sequential, Size = 32)]
    struct Reg32;

    [StructLayout(LayoutKind.Sequential, Size = 16)]
    struct Reg16;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int BinarySearch<T, TComparable>(this ReadOnlySpan<T> span, TComparable comparable)
        where TComparable : IComparable<T>
    {
        fixed (T* ptr = span)
            return BinarySearch(ptr, span.Length, comparable);
    }

    public static unsafe int BinarySearch<T, TComparable>(T* spanStart, int length, TComparable comparable)
        where TComparable : IComparable<T>
    {
        var num = 0;
        var num2 = length - 1;

        while (num <= num2)
        {
            var num3 = num2 + num >>> 1;
            var num4 = comparable.CompareTo(spanStart[num3]);

            switch (num4)
            {
                case 0: return num3;
                case > 0:
                    num = num3 + 1;
                    break;
                default:
                    num2 = num3 - 1;
                    break;
            }
        }

        return ~num;
    }

    public static unsafe int IndexOf(byte* searchSpace, int searchSpaceLength, byte* value, int valueLength)
    {
        if (valueLength is 0)
            return 0;

        var second = value + 1;
        var num = valueLength - 1;
        var num2 = 0;

        while (true)
        {
            var num3 = searchSpaceLength - num2 - num;

            if (num3 <= 0)
                break;

            var num4 = IndexOf(searchSpace + num2, *value, num3);

            if (num4 is -1)
                break;

            num2 += num4;

            if (SequenceEqual(searchSpace + (num2 + 1), second, num))
                return num2;

            num2++;
        }

        return -1;
    }

    public static unsafe int IndexOfAny(byte* searchSpace, int searchSpaceLength, byte* value, int valueLength)
    {
        if (valueLength == 0)
            return 0;

        var num = -1;

        for (var i = 0; i < valueLength; i++)
        {
            var num2 = IndexOf(searchSpace, value[i], searchSpaceLength);

            // ReSharper disable once IntVariableOverflowInUncheckedContext
            if ((uint)num2 >= (uint)num)
                continue;

            num = num2;
            searchSpaceLength = num2;

            if (num is 0)
                break;
        }

        return num;
    }

    public static unsafe int LastIndexOfAny(byte* searchSpace, int searchSpaceLength, byte* value, int valueLength)
    {
        if (valueLength == 0)
            return 0;

        var max = -1;

        for (var i = 0; i < valueLength; i++)
            if (LastIndexOf(searchSpace, value[i], searchSpaceLength) is var next && next > max)
                max = next;

        return max;
    }

    public static unsafe int IndexOf(byte* searchSpace, byte value, int length)
    {
        nint intPtr = 0;
        nint intPtr2 = length;

        while (true)
        {
            if ((nuint)(void*)intPtr2 >= 8)
            {
                intPtr2 -= 8;

                if (value == *(byte*)((nint)searchSpace + intPtr))
                    goto Found;

                if (value == *(byte*)((nint)searchSpace + intPtr + 1))
                    goto Found1;

                if (value == *(byte*)((nint)searchSpace + intPtr + 2))
                    goto Found2;

                if (value == *(byte*)((nint)searchSpace + intPtr) + 3)
                    goto Found3;

                if (value == *(byte*)((nint)searchSpace + intPtr) + 4)
                    return (int)(void*)(intPtr + 4);

                if (value == *(byte*)((nint)searchSpace + intPtr) + 5)
                    return (int)(void*)(intPtr + 5);

                if (value == *(byte*)((nint)searchSpace + intPtr) + 6)
                    return (int)(void*)(intPtr + 6);

                if (value == *(byte*)((nint)searchSpace + intPtr) + 7)
                    break;

                intPtr += 8;
                continue;
            }

            if ((nuint)(void*)intPtr2 >= 4u)
            {
                intPtr2 -= 4;

                if (value == *(byte*)((nint)searchSpace + intPtr))
                    goto Found;

                if (value == *(byte*)((nint)searchSpace + intPtr + 1))
                    goto Found1;

                if (value == *(byte*)((nint)searchSpace + intPtr + 2))
                    goto Found2;

                if (value == *(byte*)((nint)searchSpace + intPtr + 3))
                    goto Found3;

                intPtr += 4;
            }

            while ((void*)intPtr2 != null)
            {
                intPtr2 -= 1;

                if (value != *(byte*)((nint)searchSpace + intPtr))
                {
                    intPtr += 1;
                    continue;
                }

                goto Found;
            }

            return -1;

        Found:
            return (int)(void*)intPtr;

        Found1:
            return (int)(void*)(intPtr + 1);

        Found2:
            return (int)(void*)(intPtr + 2);

        Found3:
            return (int)(void*)(intPtr + 3);
        }

        return (int)(void*)(intPtr + 7);
    }

    public static unsafe int LastIndexOf(byte* searchSpace, int searchSpaceLength, byte* value, int valueLength)
    {
        if (valueLength == 0)
            return 0;

        var value2 = *value;
        var second = value + 1;
        var num = valueLength - 1;
        var num2 = 0;

        while (true)
        {
            var num3 = searchSpaceLength - num2 - num;

            if (num3 <= 0)
                break;

            var num4 = LastIndexOf(searchSpace, value2, num3);

            if (num4 == -1)
                break;

            if (SequenceEqual(searchSpace + num4 + 1, second, num))
                return num4;

            num2 += num3 - num4;
        }

        return -1;
    }

    public static unsafe int LastIndexOf(byte* searchSpace, byte value, int length)
    {
        var intPtr = (nint)length;
        var intPtr2 = (nint)length;

        while (true)
        {
            if ((nuint)(void*)intPtr2 >= 8u)
            {
                intPtr2 -= 8;
                intPtr -= 8;

                if (value == *(byte*)((nint)searchSpace + intPtr + 7))
                    break;

                if (value == *(byte*)((nint)searchSpace + intPtr + 6))
                    return (int)(void*)(intPtr + 6);

                if (value == *(byte*)((nint)searchSpace + intPtr + 5))
                    return (int)(void*)(intPtr + 5);

                if (value == *(byte*)((nint)searchSpace + intPtr + 4))
                    return (int)(void*)(intPtr + 4);

                if (value == *(byte*)((nint)searchSpace + intPtr + 3))
                    goto Found3;

                if (value == *(byte*)((nint)searchSpace + intPtr + 2))
                    goto Found2;

                if (value == *(byte*)((nint)searchSpace + intPtr + 1))
                    goto Found1;

                if (value == *(byte*)((nint)searchSpace + intPtr))
                    goto Found;

                continue;
            }

            if ((nuint)(void*)intPtr2 >= 4u)
            {
                intPtr2 -= 4;
                intPtr -= 4;

                if (value == *(byte*)((nint)searchSpace + intPtr + 3))
                    goto Found3;

                if (value == *(byte*)((nint)searchSpace + intPtr + 2))
                    goto Found2;

                if (value == *(byte*)((nint)searchSpace + intPtr + 1))
                    goto Found1;

                if (value == *(byte*)((nint)searchSpace + intPtr))
                    goto Found;
            }

            while ((void*)intPtr2 != null)
            {
                intPtr2 -= 1;
                intPtr -= 1;

                if (value != *(byte*)((nint)searchSpace + intPtr))
                    continue;

                goto Found;
            }

            return -1;

        Found:
            return (int)(void*)intPtr;

        Found1:
            return (int)(void*)(intPtr + 1);

        Found2:
            return (int)(void*)(intPtr + 2);

        Found3:
            return (int)(void*)(intPtr + 3);
        }

        return (int)(void*)(intPtr + 7);
    }

    public static unsafe int IndexOfAny(byte* searchSpace, byte value0, byte value1, int length)
    {
        nint intPtr = 0;
        var intPtr2 = (nint)length;

        while (true)
        {
            if ((nuint)(void*)intPtr2 >= 8u)
            {
                intPtr2 -= 8;
                uint num2 = *(byte*)((nint)searchSpace + intPtr);

                if (value0 == num2 || value1 == num2)
                    goto Found;

                num2 = *(byte*)((nint)searchSpace + intPtr + 1);

                if (value0 == num2 || value1 == num2)
                    goto Found1;

                num2 = *(byte*)((nint)searchSpace + intPtr + 2);

                if (value0 == num2 || value1 == num2)
                    goto Found2;

                num2 = *(byte*)((nint)searchSpace + intPtr + 3);

                if (value0 == num2 || value1 == num2)
                    goto Found3;

                num2 = *(byte*)((nint)searchSpace + intPtr + 4);

                if (value0 == num2 || value1 == num2)
                    return (int)(void*)(intPtr + 4);

                num2 = *(byte*)((nint)searchSpace + intPtr + 5);

                if (value0 == num2 || value1 == num2)
                    return (int)(void*)(intPtr + 5);

                num2 = *(byte*)((nint)searchSpace + intPtr + 6);

                if (value0 == num2 || value1 == num2)
                    return (int)(void*)(intPtr + 6);

                num2 = *(byte*)((nint)searchSpace + intPtr + 7);

                if (value0 == num2 || value1 == num2)
                    break;

                intPtr += 8;
                continue;
            }

            if ((nuint)(void*)intPtr2 >= 4u)
            {
                intPtr2 -= 4;
                uint num2 = *(byte*)((nint)searchSpace + intPtr);

                if (value0 == num2 || value1 == num2)
                    goto Found;

                num2 = *(byte*)((nint)searchSpace + intPtr + 1);

                if (value0 == num2 || value1 == num2)
                    goto Found1;

                num2 = *(byte*)((nint)searchSpace + intPtr + 2);

                if (value0 == num2 || value1 == num2)
                    goto Found2;

                num2 = *(byte*)((nint)searchSpace + intPtr + 3);

                if (value0 == num2 || value1 == num2)
                    goto Found3;

                intPtr += 4;
            }

            while ((void*)intPtr2 != null)
            {
                intPtr2 -= 1;
                uint num2 = *(byte*)((nint)searchSpace + intPtr);

                if (value0 != num2 && value1 != num2)
                {
                    intPtr += 1;
                    continue;
                }

                goto Found;
            }

            return -1;

        Found:
            return (int)(void*)intPtr;

        Found1:
            return (int)(void*)(intPtr + 1);

        Found2:
            return (int)(void*)(intPtr + 2);

        Found3:
            return (int)(void*)(intPtr + 3);
        }

        return (int)(void*)(intPtr + 7);
    }

    public static unsafe int IndexOfAny(byte* searchSpace, byte value0, byte value1, byte value2, int length)
    {
        nint intPtr = 0;
        var intPtr2 = (nint)length;

        while (true)
        {
            if ((nuint)(void*)intPtr2 >= 8u)
            {
                intPtr2 -= 8;
                uint num2 = *(byte*)((nint)searchSpace + intPtr);

                if (value0 == num2 || value1 == num2 || value2 == num2)
                    goto Found;

                num2 = *(byte*)((nint)searchSpace + intPtr + 1);

                if (value0 == num2 || value1 == num2 || value2 == num2)
                    goto Found1;

                num2 = *(byte*)((nint)searchSpace + intPtr + 2);

                if (value0 == num2 || value1 == num2 || value2 == num2)
                    goto Found2;

                num2 = *(byte*)((nint)searchSpace + intPtr + 3);

                if (value0 == num2 || value1 == num2 || value2 == num2)
                    goto Found3;

                num2 = *(byte*)((nint)searchSpace + intPtr + 4);

                if (value0 == num2 || value1 == num2 || value2 == num2)
                    return (int)(void*)(intPtr + 4);

                num2 = *(byte*)((nint)searchSpace + intPtr + 5);

                if (value0 == num2 || value1 == num2 || value2 == num2)
                    return (int)(void*)(intPtr + 5);

                num2 = *(byte*)((nint)searchSpace + intPtr + 6);

                if (value0 == num2 || value1 == num2 || value2 == num2)
                    return (int)(void*)(intPtr + 6);

                num2 = *(byte*)((nint)searchSpace + intPtr + 7);

                if (value0 == num2 || value1 == num2 || value2 == num2)
                    break;

                intPtr += 8;
                continue;
            }

            if ((nuint)(void*)intPtr2 >= 4u)
            {
                intPtr2 -= 4;
                uint num2 = *(byte*)((nint)searchSpace + intPtr);

                if (value0 == num2 || value1 == num2 || value2 == num2)
                    goto Found;

                num2 = *(byte*)((nint)searchSpace + intPtr + 1);

                if (value0 == num2 || value1 == num2 || value2 == num2)
                    goto Found1;

                num2 = *(byte*)((nint)searchSpace + intPtr + 2);

                if (value0 == num2 || value1 == num2 || value2 == num2)
                    goto Found2;

                num2 = *(byte*)((nint)searchSpace + intPtr + 3);

                if (value0 == num2 || value1 == num2 || value2 == num2)
                    goto Found3;

                intPtr += 4;
            }

            while ((void*)intPtr2 != null)
            {
                intPtr2 -= 1;
                uint num2 = *(byte*)((nint)searchSpace + intPtr);

                if (value0 != num2 && value1 != num2 && value2 != num2)
                {
                    intPtr += 1;
                    continue;
                }

                goto Found;
            }

            return -1;

        Found:
            return (int)(void*)intPtr;

        Found1:
            return (int)(void*)(intPtr + 1);

        Found2:
            return (int)(void*)(intPtr + 2);

        Found3:
            return (int)(void*)(intPtr + 3);
        }

        return (int)(void*)(intPtr + 7);
    }

    public static unsafe int LastIndexOfAny(byte* searchSpace, byte value0, byte value1, int length)
    {
        nint intPtr = length, intPtr2 = length;

        while (true)
        {
            if ((nuint)(void*)intPtr2 >= 8u)
            {
                intPtr2 -= 8;
                intPtr -= 8;
                uint num2 = *(byte*)((nint)searchSpace + intPtr + 7);

                if (value0 == num2 || value1 == num2)
                    break;

                num2 = *(byte*)((nint)searchSpace + intPtr + 6);

                if (value0 == num2 || value1 == num2)
                    return (int)(void*)(intPtr + 6);

                num2 = *(byte*)((nint)searchSpace + intPtr + 5);

                if (value0 == num2 || value1 == num2)
                    return (int)(void*)(intPtr + 5);

                num2 = *(byte*)((nint)searchSpace + intPtr + 4);

                if (value0 == num2 || value1 == num2)
                    return (int)(void*)(intPtr + 4);

                num2 = *(byte*)((nint)searchSpace + intPtr + 3);

                if (value0 == num2 || value1 == num2)
                    goto Found3;

                num2 = *(byte*)((nint)searchSpace + intPtr + 2);

                if (value0 == num2 || value1 == num2)
                    goto Found2;

                num2 = *(byte*)((nint)searchSpace + intPtr + 1);

                if (value0 == num2 || value1 == num2)
                    goto Found1;

                num2 = *(byte*)((nint)searchSpace + intPtr);

                if (value0 == num2 || value1 == num2)
                    goto Found;

                continue;
            }

            if ((nuint)(void*)intPtr2 >= 4u)
            {
                intPtr2 -= 4;
                intPtr -= 4;
                uint num2 = *(byte*)((nint)searchSpace + intPtr + 3);

                if (value0 == num2 || value1 == num2)
                    goto Found3;

                num2 = *(byte*)((nint)searchSpace + intPtr + 2);

                if (value0 == num2 || value1 == num2)
                    goto Found2;

                num2 = *(byte*)((nint)searchSpace + intPtr + 1);

                if (value0 == num2 || value1 == num2)
                    goto Found1;

                num2 = *(byte*)((nint)searchSpace + intPtr);

                if (value0 == num2 || value1 == num2)
                    goto Found;
            }

            while ((void*)intPtr2 != null)
            {
                intPtr2 -= 1;
                intPtr -= 1;
                uint num2 = *(byte*)((nint)searchSpace + intPtr);

                if (value0 != num2 && value1 != num2)
                    continue;

                goto Found;
            }

            return -1;

        Found:
            return (int)(void*)intPtr;

        Found1:
            return (int)(void*)(intPtr + 1);

        Found2:
            return (int)(void*)(intPtr + 2);

        Found3:
            return (int)(void*)(intPtr + 3);
        }

        return (int)(void*)(intPtr + 7);
    }

    public static unsafe int LastIndexOfAny(byte* searchSpace, byte value0, byte value1, byte value2, int length)
    {
        nint intPtr = length, intPtr2 = length;

        while (true)
        {
            if ((nuint)(void*)intPtr2 >= 8u)
            {
                intPtr2 -= 8;
                intPtr -= 8;
                uint num2 = *(byte*)((nint)searchSpace + intPtr + 7);

                if (value0 == num2 || value1 == num2 || value2 == num2)
                    break;

                num2 = *(byte*)((nint)searchSpace + intPtr + 6);

                if (value0 == num2 || value1 == num2 || value2 == num2)
                    return (int)(void*)(intPtr + 6);

                num2 = *(byte*)((nint)searchSpace + intPtr + 5);

                if (value0 == num2 || value1 == num2 || value2 == num2)
                    return (int)(void*)(intPtr + 5);

                num2 = *(byte*)((nint)searchSpace + intPtr + 4);

                if (value0 == num2 || value1 == num2 || value2 == num2)
                    return (int)(void*)(intPtr + 4);

                num2 = *(byte*)((nint)searchSpace + intPtr + 3);

                if (value0 == num2 || value1 == num2 || value2 == num2)
                    goto Found3;

                num2 = *(byte*)((nint)searchSpace + intPtr + 2);

                if (value0 == num2 || value1 == num2 || value2 == num2)
                    goto Found2;

                num2 = *(byte*)((nint)searchSpace + intPtr + 1);

                if (value0 == num2 || value1 == num2 || value2 == num2)
                    goto Found1;

                num2 = *(byte*)((nint)searchSpace + intPtr);

                if (value0 == num2 || value1 == num2 || value2 == num2)
                    goto Found;

                continue;
            }

            if ((nuint)(void*)intPtr2 >= 4u)
            {
                intPtr2 -= 4;
                intPtr -= 4;
                uint num2 = *(byte*)((nint)searchSpace + intPtr + 3);

                if (value0 == num2 || value1 == num2 || value2 == num2)
                    goto Found3;

                num2 = *(byte*)((nint)searchSpace + intPtr + 2);

                if (value0 == num2 || value1 == num2 || value2 == num2)
                    goto Found2;

                num2 = *(byte*)((nint)searchSpace + intPtr + 1);

                if (value0 == num2 || value1 == num2 || value2 == num2)
                    goto Found1;

                num2 = *(byte*)((nint)searchSpace + intPtr);

                if (value0 == num2 || value1 == num2 || value2 == num2)
                    goto Found;
            }

            while ((void*)intPtr2 != null)
            {
                intPtr2 -= 1;
                intPtr -= 1;
                uint num2 = *(byte*)((nint)searchSpace + intPtr);

                if (value0 != num2 && value1 != num2 && value2 != num2)
                    continue;

                goto Found;
            }

            return -1;

        Found:
            return (int)(void*)intPtr;

        Found1:
            return (int)(void*)(intPtr + 1);

        Found2:
            return (int)(void*)(intPtr + 2);

        Found3:
            return (int)(void*)(intPtr + 3);
        }

        return (int)(void*)(intPtr + 7);
    }

    public static unsafe bool SequenceEqual(byte* first, byte* second, nuint length)
    {
        if (first == second)
            goto True;

        nint intPtr = 0, intPtr2 = (nint)(void*)length;

        if ((nuint)(void*)intPtr2 < (nuint)sizeof(nuint))
        {
            while ((void*)intPtr2 > (void*)intPtr)
            {
                if (first[intPtr] == second[intPtr])
                {
                    intPtr += 1;
                    continue;
                }

                goto False;
            }

            goto True;
        }

        intPtr2 -= sizeof(nuint);

        while (true)
        {
            if ((void*)intPtr2 <= (void*)intPtr)
                return *(nuint*)(first + intPtr2) == *(nuint*)(second + intPtr2);

            if (*(nuint*)(first + intPtr) == *(nuint*)(second + intPtr))
                break;

            intPtr += sizeof(nuint);
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

        var intPtr = (nint)(firstLength < secondLength ? firstLength : secondLength);
        var intPtr2 = (nint)0;
        var intPtr3 = (nint)(void*)intPtr;

        if ((nuint)(void*)intPtr3 > (nuint)sizeof(nuint))
        {
            intPtr3 -= sizeof(nuint);

            for (;
                (void*)intPtr3 > (void*)intPtr2 && *(nuint*)(first + intPtr2) == *(nuint*)(second + intPtr2);
                intPtr2 += sizeof(nuint)) { }
        }

        for (; (void*)intPtr > (void*)intPtr2; intPtr2 += 1)
        {
            var num = (*(byte*)((nint)first + intPtr2)).CompareTo(*(byte*)((nint)second + intPtr2));

            if (num != 0)
                return num;
        }

        return firstLength - secondLength;
    }

    public static unsafe int SequenceCompareTo(char* first, int firstLength, char* second, int secondLength)
    {
        var result = firstLength - secondLength;

        if (first == second)
            return result;

        var intPtr = firstLength < secondLength ? firstLength : secondLength;
        var intPtr2 = 0;

        if ((nuint)(void*)intPtr >= (nuint)(sizeof(nuint) / 2))
            for (;
                (void*)intPtr >= (void*)(intPtr2 + sizeof(nuint) / 2) &&
                *(nuint*)(first + intPtr2) == *(nuint*)(second + intPtr2);
                intPtr2 += sizeof(nuint) / 2) { }

        if (sizeof(nuint) > 4 &&
            (void*)intPtr >= (void*)(intPtr2 + 2) &&
            *(int*)(first + intPtr2) == *(int*)(second + intPtr2))
            intPtr2 += 2;

        for (; (void*)intPtr2 < (void*)intPtr; intPtr2++)
        {
            var num = first[intPtr2].CompareTo(second[intPtr2]);

            if (num != 0)
                return num;
        }

        return result;
    }

    public static unsafe int IndexOf(char* searchSpace, char value, int length)
    {
        var ptr2 = searchSpace;

        while (true)
        {
            if (length >= 4)
            {
                length -= 4;

                if (*ptr2 == value)
                    break;

                if (ptr2[1] != value)
                {
                    if (ptr2[2] != value)
                    {
                        if (ptr2[3] != value)
                        {
                            ptr2 += 4;
                            continue;
                        }

                        ptr2++;
                    }

                    ptr2++;
                }

                ptr2++;
                break;
            }

            while (length > 0)
            {
                length--;

                if (*ptr2 == value)
                    goto Break;

                ptr2++;
            }

            return -1;

        Break:
            break;
        }

        return (int)(ptr2 - searchSpace);
    }

    public static unsafe int LastIndexOf(char* searchSpace, char value, int length)
    {
        var ptr2 = searchSpace + length;

        while (true)
        {
            if (length >= 4)
            {
                length -= 4;
                ptr2 -= 4;

                if (ptr2[3] == value)
                    break;

                if (ptr2[2] == value)
                    return (int)(ptr2 - searchSpace) + 2;

                if (ptr2[1] == value)
                    return (int)(ptr2 - searchSpace) + 1;

                if (*ptr2 != value)
                    continue;

                goto Found;
            }

            while (length > 0)
            {
                length--;
                ptr2--;

                if (*ptr2 != value)
                    continue;

                goto Found;
            }

            return -1;

        Found:
            return (int)(ptr2 - searchSpace);
        }

        return (int)(ptr2 - searchSpace) + 3;
    }

    public static unsafe int IndexOf<T>(T* searchSpace, int searchSpaceLength, T* value, int valueLength)
        where T : IEquatable<T>
    {
        if (valueLength == 0)
            return 0;

        var value2 = *value;
        T* second = value + 1;
        var num = valueLength - 1;
        var num2 = 0;

        while (true)
        {
            var num3 = searchSpaceLength - num2 - num;

            if (num3 <= 0)
                break;

            var num4 = IndexOf(searchSpace + num2, value2, num3);

            if (num4 == -1)
                break;

            num2 += num4;

            if (SequenceEqual(searchSpace + (num2 + 1), second, num))
                return num2;

            num2++;
        }

        return -1;
    }

    public static unsafe int IndexOf<T>(T* searchSpace, T value, int length)
        where T : IEquatable<T>
    {
        var intPtr = (nint)0;

        while (true)
        {
            if (length >= 8)
            {
                length -= 8;

                if (!value.Equals(searchSpace[intPtr]))
                {
                    if (value.Equals(searchSpace[intPtr + 1]))
                        goto Found1;

                    if (value.Equals(searchSpace[intPtr + 2]))
                        goto Found2;

                    if (value.Equals(searchSpace[intPtr + 3]))
                        goto Found3;

                    if (value.Equals(searchSpace[intPtr + 4]))
                        return (int)(void*)(intPtr + 4);

                    if (value.Equals(searchSpace[intPtr + 5]))
                        return (int)(void*)(intPtr + 5);

                    if (value.Equals(searchSpace[intPtr + 6]))
                        return (int)(void*)(intPtr + 6);

                    if (value.Equals(searchSpace[intPtr + 7]))
                        break;

                    intPtr += 8;
                    continue;
                }
            }
            else
            {
                if (length >= 4)
                {
                    length -= 4;

                    if (value.Equals(searchSpace[intPtr]))
                        goto Found;

                    if (value.Equals(searchSpace[intPtr + 1]))
                        goto Found1;

                    if (value.Equals(searchSpace[intPtr + 2]))
                        goto Found2;

                    if (value.Equals(searchSpace[intPtr + 3]))
                        goto Found3;

                    intPtr += 4;
                }

                while (true)
                {
                    if (length <= 0)
                        return -1;

                    if (value.Equals(searchSpace[intPtr]))
                        break;

                    intPtr += 1;
                    length--;
                }
            }

        Found:
            return (int)(void*)intPtr;

        Found1:
            return (int)(void*)(intPtr + 1);

        Found2:
            return (int)(void*)(intPtr + 2);

        Found3:
            return (int)(void*)(intPtr + 3);
        }

        return (int)(void*)(intPtr + 7);
    }

    public static unsafe int IndexOfAny<T>(T* searchSpace, T value0, T value1, int length)
        where T : IEquatable<T>
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
        where T : IEquatable<T>
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
        where T : IEquatable<T>
    {
        if (valueLength == 0)
            return 0;

        var num = -1;

        for (var i = 0; i < valueLength; i++)
        {
            var num2 = IndexOf(searchSpace, value[i], searchSpaceLength);

            // ReSharper disable once IntVariableOverflowInUncheckedContext
            if ((uint)num2 >= (uint)num)
                continue;

            num = num2;
            searchSpaceLength = num2;

            if (num is 0)
                break;
        }

        return num;
    }

    public static unsafe int LastIndexOf<T>(T* searchSpace, int searchSpaceLength, T* value, int valueLength)
        where T : IEquatable<T>
    {
        if (valueLength == 0)
            return 0;

        var value2 = *value;
        T* second = value + 1;
        var num = valueLength - 1;
        var num2 = 0;

        while (true)
        {
            var num3 = searchSpaceLength - num2 - num;

            if (num3 <= 0)
                break;

            var num4 = LastIndexOf(searchSpace, value2, num3);

            if (num4 == -1)
                break;

            if (SequenceEqual(searchSpace + (num4 + 1), second, num))
                return num4;

            num2 += num3 - num4;
        }

        return -1;
    }

    public static unsafe int LastIndexOf<T>(T* searchSpace, T value, int length)
        where T : IEquatable<T>
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
        where T : IEquatable<T>
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
        where T : IEquatable<T>
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
        where T : IEquatable<T>
    {
        if (valueLength == 0)
            return 0;

        var num = -1;

        for (var i = 0; i < valueLength; i++)
        {
            var num2 = LastIndexOf(searchSpace, value[i], searchSpaceLength);

            if (num2 > num)
                num = num2;
        }

        return num;
    }

    public static unsafe bool SequenceEqual<T>(T* first, T* second, int length)
        where T : IEquatable<T>
    {
        if (first == second)
            return true;

        var intPtr = (nint)0;

        while (true)
        {
            if (length >= 8)
            {
                length -= 8;

                if (first[intPtr].Equals(second[intPtr]) &&
                    first[intPtr + 1].Equals(second[intPtr + 1]) &&
                    first[intPtr + 2].Equals(second[intPtr + 2]) &&
                    first[intPtr + 3].Equals(second[intPtr + 3]) &&
                    first[intPtr + 4].Equals(second[intPtr + 4]) &&
                    first[intPtr + 5].Equals(second[intPtr + 5]) &&
                    first[intPtr + 6].Equals(second[intPtr + 6]) &&
                    first[intPtr + 7].Equals(second[intPtr + 7]))
                {
                    intPtr += 8;
                    continue;
                }

                goto False;
            }

            if (length >= 4)
            {
                length -= 4;

                if (!first[intPtr].Equals(second[intPtr]) ||
                    !first[intPtr + 1].Equals(second[intPtr + 1]) ||
                    !first[intPtr + 2].Equals(second[intPtr + 2]) ||
                    !first[intPtr + 3].Equals(second[intPtr + 3]))
                    goto False;

                intPtr += 4;
            }

            while (length > 0)
            {
                if (first[intPtr].Equals(second[intPtr]))
                {
                    intPtr += 1;
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
        where T : IComparable<T>
    {
        var num = firstLength;

        if (num > secondLength)
            num = secondLength;

        for (var i = 0; i < num; i++)
        {
            var num2 = first[i].CompareTo(second[i]);

            if (num2 != 0)
                return num2;
        }

        return firstLength.CompareTo(secondLength);
    }

    public static unsafe void CopyTo<T>(T* dst, int dstLength, T* src, int srcLength)
    {
        nint intPtr = srcLength * Unsafe.SizeOf<T>(),
            intPtr2 = dstLength * Unsafe.SizeOf<T>(),
            intPtr3 = (nint)dst - (nint)src;

        bool num;

        if (sizeof(nint) is not 4)
        {
            if ((ulong)intPtr3 >= (ulong)intPtr)
            {
                num = (ulong)intPtr3 > (ulong)-(long)intPtr2;
                goto BlockCopy;
            }
        }
        else if ((uint)(int)intPtr3 >= (uint)(int)intPtr)
        {
            num = (uint)(int)intPtr3 > (uint)-(int)intPtr2;
            goto BlockCopy;
        }

        goto ElementWise;

    BlockCopy:

        if (!num && !IsReferenceOrContainsReferences<T>())
        {
            byte* source = (byte*)dst, source2 = (byte*)src;
            var num2 = (ulong)intPtr;
            uint num4;

            for (var num3 = 0uL; num3 < num2; num3 += num4)
            {
                num4 = (uint)(num2 - num3 > uint.MaxValue ? uint.MaxValue : num2 - num3);
                new Span<byte>(source, (int)num3).CopyTo(new(source2, (int)num3));
            }

            return;
        }

    ElementWise:

        var flag = sizeof(nint) == 4
            ? (uint)(int)intPtr3 > (uint)-(int)intPtr2
            : (ulong)intPtr3 > (ulong)-(long)intPtr2;

        var num5 = flag ? 1 : -1;
        var num6 = !flag ? srcLength - 1 : 0;
        int i;

        for (i = 0; i < (srcLength & -8); i += 8)
        {
            dst[num6] = src[num6];
            dst[num6 + num5] = src[num6 + num5];
            dst[num6 + num5 * 2] = src[num6 + num5 * 2];
            dst[num6 + num5 * 3] = src[num6 + num5 * 3];
            dst[num6 + num5 * 4] = src[num6 + num5 * 4];
            dst[num6 + num5 * 5] = src[num6 + num5 * 5];
            dst[num6 + num5 * 6] = src[num6 + num5 * 6];
            dst[num6 + num5 * 7] = src[num6 + num5 * 7];
            num6 += num5 * 8;
        }

        if (i < (srcLength & -4))
        {
            dst[num6] = src[num6];
            dst[num6 + num5] = src[num6 + num5];
            dst[num6 + num5 * 2] = src[num6 + num5 * 2];
            dst[num6 + num5 * 3] = src[num6 + num5 * 3];
            num6 += num5 * 4;
            i += 4;
        }

        for (; i < srcLength; i++)
        {
            dst[num6] = src[num6];
            num6 += num5;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe nint Add<T>(this nint start, int index)
    {
        if (sizeof(nint) == 4)
        {
            var num = (uint)(index * Unsafe.SizeOf<T>());
            return (nint)((byte*)(void*)start + num);
        }

        var num2 = (ulong)index * (ulong)Unsafe.SizeOf<T>();
        return (nint)((byte*)(void*)start + num2);
    }

    public static bool IsReferenceOrContainsReferences<T>() => PerTypeValues<T>.IsReferenceOrContainsReferences;

    public static unsafe void ClearLessThanPointerSized(byte* ptr, nuint byteLength)
    {
        if (sizeof(nuint) is 4)
        {
            new Span<byte>(ptr, (int)byteLength).Clear();
            return;
        }

        var num = (ulong)byteLength;
        var num2 = (uint)(num & 0xFFFFFFFFu);
        new Span<byte>(ptr, (int)num2).Clear();
        num -= num2;
        ptr += num2;

        while (num != 0)
        {
            num2 = (uint)(num >= uint.MaxValue ? uint.MaxValue : num);
            new Span<byte>(ptr, (int)num2).Clear();
            ptr += num2;
            num -= num2;
        }
    }

    public static unsafe void ClearPointerSizedWithoutReferences(byte* b, nuint byteLength)
    {
        nint zero;

        for (zero = 0; zero.LessThanEqual(byteLength - (nuint)sizeof(Reg64)); zero += sizeof(Reg64))
            *(Reg64*)((nint)b + zero) = default;

        if (zero.LessThanEqual(byteLength - (nuint)sizeof(Reg32)))
        {
            *(Reg32*)((nint)b + zero) = default;
            zero += sizeof(Reg32);
        }

        if (zero.LessThanEqual(byteLength - (nuint)sizeof(Reg16)))
        {
            *(Reg16*)((nint)b + zero) = default;
            zero += sizeof(Reg16);
        }

        if (zero.LessThanEqual(byteLength - 8))
        {
            *(long*)((nint)b + zero) = 0;
            zero += 8;
        }

        if (sizeof(nint) is 4 && zero.LessThanEqual(byteLength - 4))
            *(int*)((nint)b + zero) = 0;
    }

    public static unsafe void ClearPointerSizedWithReferences(nint* ip, nuint pointerSizeLength)
    {
        nint intPtr = 0, zero;

        while ((zero = intPtr + 8).LessThanEqual(pointerSizeLength))
        {
            ip[0] = default;
            ip[1] = default;
            ip[2] = default;
            ip[3] = default;
            ip[4] = default;
            ip[5] = default;
            ip[6] = default;
            ip[7] = default;
            intPtr = zero;
        }

        if ((zero = intPtr + 4).LessThanEqual(pointerSizeLength))
        {
            ip[0] = default;
            ip[1] = default;
            ip[2] = default;
            ip[3] = default;
            intPtr = zero;
        }

        if ((zero = intPtr + 2).LessThanEqual(pointerSizeLength))
        {
            ip[0] = default;
            ip[1] = default;
            intPtr = zero;
        }

        if ((intPtr + 1).LessThanEqual(pointerSizeLength))
            ip[intPtr] = default;
    }

    static bool IsReferenceOrContainsReferencesCore(Type type) =>
        !type.IsPrimitive &&
        (!type.IsValueType ||
            (Nullable.GetUnderlyingType(type) ?? type) is { IsEnum: false } t &&
            t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
               .Any(x => IsReferenceOrContainsReferencesCore(x.FieldType)));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static unsafe bool LessThanEqual(this nint index, nuint length) =>
        sizeof(nuint) is 4 ? (int)index <= (int)(uint)length : index <= (long)length;
}
