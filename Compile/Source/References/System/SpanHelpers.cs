// SPDX-License-Identifier: MPL-2.0

// ReSharper disable CheckNamespace InvocationIsSkipped
namespace System;
#pragma warning disable 8500, MA0051, SA1405
/// <summary>Unsafe functions to determine equality of buffers.</summary>
static partial class SpanHelpers
{
    /// <summary>Determines whether both pointers to buffers contain the same content.</summary>
    /// <typeparam name="T">The type of buffer.</typeparam>
    /// <param name="first">The first pointer to compare.</param>
    /// <param name="second">The second pointer to compare.</param>
    /// <param name="length">The length of both <paramref name="first"/> and <paramref name="second"/>.</param>
    /// <returns>
    /// The value <see langword="true"/> when both sequences have the same content, otherwise; <see langword="false"/>.
    /// </returns>
    public static unsafe bool SequenceEqual<T>(T* first, T* second, int length)
        where T : IEquatable<T>?
    {
        Debug.Assert(length >= 0);

        if (first == second)
            goto Equal;

        nint index = 0; // Use nint for arithmetic to avoid unnecessary 64->32->64 truncations
        T lookUp0;
        T lookUp1;

        while (length >= 8)
        {
            length -= 8;

            lookUp0 = first[index];
            lookUp1 = second[index];

            if (!(lookUp0?.Equals(lookUp1) ?? lookUp1 is null))
                goto NotEqual;

            lookUp0 = first[index + 1];
            lookUp1 = second[index + 1];

            if (!(lookUp0?.Equals(lookUp1) ?? lookUp1 is null))
                goto NotEqual;

            lookUp0 = first[index + 2];
            lookUp1 = second[index + 2];

            if (!(lookUp0?.Equals(lookUp1) ?? lookUp1 is null))
                goto NotEqual;

            lookUp0 = first[index + 3];
            lookUp1 = second[index + 3];

            if (!(lookUp0?.Equals(lookUp1) ?? lookUp1 is null))
                goto NotEqual;

            lookUp0 = first[index + 4];
            lookUp1 = second[index + 4];

            if (!(lookUp0?.Equals(lookUp1) ?? lookUp1 is null))
                goto NotEqual;

            lookUp0 = first[index + 5];
            lookUp1 = second[index + 5];

            if (!(lookUp0?.Equals(lookUp1) ?? lookUp1 is null))
                goto NotEqual;

            lookUp0 = first[index + 6];
            lookUp1 = second[index + 6];

            if (!(lookUp0?.Equals(lookUp1) ?? lookUp1 is null))
                goto NotEqual;

            lookUp0 = first[index + 7];
            lookUp1 = second[index + 7];

            if (!(lookUp0?.Equals(lookUp1) ?? lookUp1 is null))
                goto NotEqual;

            index += 8;
        }

        if (length >= 4)
        {
            length -= 4;

            lookUp0 = first[index];
            lookUp1 = second[index];

            if (!(lookUp0?.Equals(lookUp1) ?? lookUp1 is null))
                goto NotEqual;

            lookUp0 = first[index + 1];
            lookUp1 = second[index + 1];

            if (!(lookUp0?.Equals(lookUp1) ?? lookUp1 is null))
                goto NotEqual;

            lookUp0 = first[index + 2];
            lookUp1 = second[index + 2];

            if (!(lookUp0?.Equals(lookUp1) ?? lookUp1 is null))
                goto NotEqual;

            lookUp0 = first[index + 3];
            lookUp1 = second[index + 3];

            if (!(lookUp0?.Equals(lookUp1) ?? lookUp1 is null))
                goto NotEqual;

            index += 4;
        }

        while (length > 0)
        {
            lookUp0 = first[index];
            lookUp1 = second[index];

            if (!(lookUp0?.Equals(lookUp1) ?? lookUp1 is null))
                goto NotEqual;

            index += 1;
            length--;
        }

    Equal:
        return true;

    NotEqual: // Workaround for https://github.com/dotnet/runtime/issues/8795
        return false;
    }

    /// <summary>Gets the index of where the smaller buffer matches the contents of the larger buffer.</summary>
    /// <typeparam name="T">The type of buffer.</typeparam>
    /// <param name="searchSpace">The larger buffer to compare from.</param>
    /// <param name="searchSpaceLength">The larger buffer's length.</param>
    /// <param name="value">The smaller buffer to compare against.</param>
    /// <param name="valueLength">The smaller buffer's length.</param>
    /// <returns>
    /// The index in which <paramref name="searchSpace"/> has the same content as <paramref name="value"/>.
    /// </returns>
    public static unsafe int IndexOf<T>(T* searchSpace, int searchSpaceLength, T* value, int valueLength)
        where T : IEquatable<T>?
    {
        Debug.Assert(searchSpaceLength >= 0);
        Debug.Assert(valueLength >= 0);

        if (valueLength is 0)
            return 0; // A zero-length sequence is always treated as "found" at the start of the search space.

        var valueTail = value + 1;
        var valueTailLength = valueLength - 1;

        var index = 0;

        while (true)
        {
            // Ensures no deceptive underflows in the computation of "remainingSearchSpaceLength".
            Debug.Assert(index >= 0 && index <= searchSpaceLength);

            var remainingSearchSpaceLength = searchSpaceLength - index - valueTailLength;

            // The unsearched portion is now shorter than the sequence we're looking for. So it can't be there.
            if (remainingSearchSpaceLength <= 0)
                break;

            // Do a quick search for the first element of "value".
            var relativeIndex = IndexOf(searchSpace + index, *value, remainingSearchSpaceLength);

            if (relativeIndex < 0)
                break;

            index += relativeIndex;

            // Found the first element of "value". See if the tail matches.
            if (SequenceEqual(searchSpace + index + 1, valueTail, valueTailLength))
                return index; // The tail matched. Return a successful find.

            index++;
        }

        return -1;
    }

    /// <summary>Gets the index of where the item exists in the buffer.</summary>
    /// <typeparam name="T">The type of buffer.</typeparam>
    /// <param name="searchSpace">The buffer to compare from.</param>
    /// <param name="value">The item to compare to.</param>
    /// <param name="length">The buffer's length.</param>
    /// <returns>The index in which <paramref name="searchSpace"/> has <paramref name="value"/>.</returns>
    public static unsafe int IndexOf<T>(T* searchSpace, T value, int length)
        where T : IEquatable<T>?
    {
        Debug.Assert(length >= 0);

        nint index = 0; // Use nint for arithmetic to avoid unnecessary 64->32->64 truncations

        // ReSharper disable CompareNonConstrainedGenericWithNull
        if (default(T) is not null || value is not null)
        {
#pragma warning disable 8602
            Debug.Assert(value is not null);

            while (length >= 8)
            {
                length -= 8;

                if (value.Equals(searchSpace[index]))
                    goto Found;

                if (value.Equals(searchSpace[index + 1]))
                    goto Found1;

                if (value.Equals(searchSpace[index + 2]))
                    goto Found2;

                if (value.Equals(searchSpace[index + 3]))
                    goto Found3;

                if (value.Equals(searchSpace[index + 4]))
                    goto Found4;

                if (value.Equals(searchSpace[index + 5]))
                    goto Found5;

                if (value.Equals(searchSpace[index + 6]))
                    goto Found6;

                if (value.Equals(searchSpace[index + 7]))
                    goto Found7;

                index += 8;
            }

            if (length >= 4)
            {
                length -= 4;

                if (value.Equals(searchSpace[index]))
                    goto Found;

                if (value.Equals(searchSpace[index + 1]))
                    goto Found1;

                if (value.Equals(searchSpace[index + 2]))
                    goto Found2;

                if (value.Equals(searchSpace[index + 3]))
                    goto Found3;

                index += 4;
            }

            while (length > 0)
            {
                if (value.Equals(searchSpace[index]))
                    goto Found;

                index += 1;
                length--;
            }
#pragma warning restore 8602
        }
        else
        {
            nint len = length;

            for (index = 0; index < len; index++)
                if (searchSpace + index + 1 is null)
                    goto Found;
        }

        return -1;

        // Workaround for https://github.com/dotnet/runtime/issues/8795
    Found:
        return (int)index;

    Found1:
        return (int)(index + 1);

    Found2:
        return (int)(index + 2);

    Found3:
        return (int)(index + 3);

    Found4:
        return (int)(index + 4);

    Found5:
        return (int)(index + 5);

    Found6:
        return (int)(index + 6);

    Found7:
        return (int)(index + 7);
    }
}
