// SPDX-License-Identifier: MPL-2.0

// ReSharper disable ArrangeAttributes FunctionComplexityOverflow InconsistentNaming SuspiciousTypeConversion.Global
// Shamelessly stolen from https://raw.githubusercontent.com/igor-tkachev/Portable.System.ValueTuple/master/Portable.System.ValueTuple.cs
// and creatively reworked. Which is...
//
// Shamelessly stolen from https://github.com/dotnet/roslyn/blob/master/src/Compilers/Test/Resources/Core/NetFX/ValueTuple/ValueTuple.cs
// and creatively reworked.
//
// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

#if NET20 || NET30 || NET35 || NET40 || NET45 || NET451 || NET452 || NET46 || NET461 || NET462 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6 || NETCOREAPP1_0 || NETCOREAPP1_1
#pragma warning disable CA1200, CA1508, CA2208, CA5394, DOC100, DOC202, MA0012, MA0015, MA0048, MA0051, MA0097, SA1129, SA1141, SA1201, SA1202, SA1600, SA1611, SA1623, SA1642, SA1649
namespace System
{
#pragma warning disable SA1403
    namespace Runtime.CompilerServices
#pragma warning restore SA1403
    {
        /// <summary>This interface is required for types that want to be indexed into by dynamic patterns.</summary>
        interface ITuple
        {
            /// <summary>The number of positions in this data structure.</summary>
            [NonNegativeValue, Pure]
            int Length { get; }

            /// <summary>Get the element at position <paramref name="index"/>.</summary>
            [Pure]
            object? this[[NonNegativeValue] int index] { get; }
        }

        /// <summary>
        /// Indicates that the use of <see cref="T:System.ValueTuple" /> on a member is meant to be treated as a tuple with element names.
        /// </summary>
        [AttributeUsage(
            AttributeTargets.Class |
            AttributeTargets.Struct |
            AttributeTargets.Property |
            AttributeTargets.Field |
            AttributeTargets.Event |
            AttributeTargets.Parameter |
            AttributeTargets.ReturnValue
        )]
        sealed class TupleElementNamesAttribute : Attribute
        {
            readonly string[] _transformNames;

            /// <summary>
            /// Initializes a new instance of the <see cref="T:System.Runtime.CompilerServices.TupleElementNamesAttribute" /> class.
            /// </summary>
            /// <param name="transformNames">
            /// Specifies, in a pre-order depth-first traversal of a type's
            /// construction, which <see cref="T:System.ValueTuple" /> occurrences are
            /// meant to carry element names.
            /// </param>
            /// <remarks><para>
            /// This constructor is meant to be used on types that contain an
            /// instantiation of <see cref="T:System.ValueTuple" /> that contains
            /// element names.  For instance, if <c>C</c> is a generic type with
            /// two type parameters, then a use of the constructed type <c>C{<see cref="T:System.ValueTuple`2" />,
            /// <see cref="T:System.ValueTuple`3" /></c> might be intended to
            /// treat the first type argument as a tuple with element names and the
            /// second as a tuple without element names. In which case, the
            /// appropriate attribute specification should use a
            /// <paramref name="transformNames"/> value of <c>{ "name1", "name2", null, null,
            /// null }</c>.
            /// </para></remarks>
            public TupleElementNamesAttribute(string[] transformNames) =>
                _transformNames = transformNames ?? throw new ArgumentNullException(nameof(transformNames));

            /// <summary>
            /// Specifies, in a pre-order depth-first traversal of a type's
            /// construction, which <see cref="T:System.ValueTuple" /> elements are
            /// meant to carry element names.
            /// </summary>
            [Pure]
            public IList<string> TransformNames => _transformNames;
        }
    }

    /// <summary>
    /// Helper so we can call some tuple methods recursively without knowing the underlying types.
    /// </summary>
    interface IValueTupleInternal : ITuple
    {
        [Pure]
        int GetHashCode(IEqualityComparer comparer);

        [Pure]
        string ToStringEnd();
    }

    /// <summary>
    /// The ValueTuple types (from arity 0 to 8) comprise the runtime implementation that underlies tuples in C# and struct tuples in F#.
    /// Aside from created via language syntax, they are most easily created via the ValueTuple.Create factory methods.
    /// The System.ValueTuple types differ from the System.Tuple types in that:
    /// - they are structs rather than classes,
    /// - they are mutable rather than readonly, and
    /// - their members (such as Item1, Item2, etc) are fields rather than properties.
    /// </summary>
#if !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6 && !NETCOREAPP1_0 && !NETCOREAPP1_1
    [Serializable]
#endif
    readonly struct ValueTuple : IEquatable<ValueTuple>,
#if !NET20 && !NET30 && !NET35
        IStructuralEquatable,
        IStructuralComparable,
#endif
        IComparable,
        IComparable<ValueTuple>,
        IValueTupleInternal
    {
        static readonly int s_randomSeed = new Random().Next(int.MinValue, int.MaxValue);

        /// <summary>
        /// Returns a value that indicates whether the current <see cref="ValueTuple"/> instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">The object to compare with this instance.</param>
        /// <returns><see langword="true"/> if <paramref name="obj"/> is a <see cref="ValueTuple"/>.</returns>
        [Pure]
        public override bool Equals([NotNullWhen(true)] object? obj) => obj is ValueTuple;

        /// <summary>Returns a value indicating whether this instance is equal to a specified value.</summary>
        /// <param name="other">An instance to compare to this instance.</param>
        /// <returns>true if <paramref name="other"/> has the same value as this instance; otherwise, false.</returns>
        [Pure]
        public bool Equals(ValueTuple other) => true;

        [Pure, ValueRange(0, 1)]
        int IComparable.CompareTo(object? other) =>
            other switch
            {
                null => 1,
                ValueTuple => 0,
                _ => throw new ArgumentException(),
            };

        /// <summary>Compares this instance to a specified instance and returns an indication of their relative values.</summary>
        /// <param name="other">An instance to compare.</param>
        /// <returns>
        /// A signed number indicating the relative values of this instance and <paramref name="other"/>.
        /// Returns less than zero if this instance is less than <paramref name="other"/>, zero if this
        /// instance is equal to <paramref name="other"/>, and greater than zero if this instance is greater
        /// than <paramref name="other"/>.
        /// </returns>
        [Pure, ValueRange(0)]
        public int CompareTo(ValueTuple other) => 0;

        /// <summary>Returns the hash code for this instance.</summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        [Pure, ValueRange(0)]
        public override int GetHashCode() => 0;

#if !NET20 && !NET30 && !NET35
        [Pure]
        bool IStructuralEquatable.Equals(object? other, IEqualityComparer comparer) => other is ValueTuple;

        [Pure]
        int IStructuralEquatable.GetHashCode(IEqualityComparer comparer) => 0;

        [Pure, ValueRange(0)]
        int IStructuralComparable.CompareTo(object? other, IComparer comparer) =>
            other switch
            {
                null => 1,
                ValueTuple => 0,
                _ => throw new ArgumentException(),
            };

#endif

        [Pure, ValueRange(0)]
        int IValueTupleInternal.GetHashCode(IEqualityComparer comparer) => 0;

        /// <summary>
        /// Returns a string that represents the value of this <see cref="ValueTuple"/> instance.
        /// </summary>
        /// <returns>The string representation of this <see cref="ValueTuple"/> instance.</returns>
        /// <remarks>
        /// The string returned by this method takes the form <c>()</c>.
        /// </remarks>
        [Pure]
        public override string ToString() => "()";

        [Pure]
        string IValueTupleInternal.ToStringEnd() => ")";

        /// <summary>
        /// The number of positions in this data structure.
        /// </summary>
        [Pure]
        int ITuple.Length => 0;

        /// <summary>
        /// Get the element at position <param name="index"/>.
        /// </summary>
        [Pure]
        object ITuple.this[[ValueRange(0, -1)] int index]
        {
            [DoesNotReturn] get => throw new IndexOutOfRangeException();
        }

        /// <summary>Creates a new struct 0-tuple.</summary>
        /// <returns>A 0-tuple.</returns>
        [Pure]
        public static ValueTuple Create() => new();

        /// <summary>Creates a new struct 1-tuple, or singleton.</summary>
        /// <typeparam name="T1">The type of the first component of the tuple.</typeparam>
        /// <param name="item1">The value of the first component of the tuple.</param>
        /// <returns>A 1-tuple (singleton) whose value is (item1).</returns>
        [Pure]
        public static ValueTuple<T1> Create<T1>(T1 item1) => new(item1);

        /// <summary>Creates a new struct 2-tuple, or pair.</summary>
        /// <typeparam name="T1">The type of the first component of the tuple.</typeparam>
        /// <typeparam name="T2">The type of the second component of the tuple.</typeparam>
        /// <param name="item1">The value of the first component of the tuple.</param>
        /// <param name="item2">The value of the second component of the tuple.</param>
        /// <returns>A 2-tuple (pair) whose value is (item1, item2).</returns>
        [Pure]
        public static ValueTuple<T1, T2> Create<T1, T2>(T1 item1, T2 item2) => new(item1, item2);

        /// <summary>Creates a new struct 3-tuple, or triple.</summary>
        /// <typeparam name="T1">The type of the first component of the tuple.</typeparam>
        /// <typeparam name="T2">The type of the second component of the tuple.</typeparam>
        /// <typeparam name="T3">The type of the third component of the tuple.</typeparam>
        /// <param name="item1">The value of the first component of the tuple.</param>
        /// <param name="item2">The value of the second component of the tuple.</param>
        /// <param name="item3">The value of the third component of the tuple.</param>
        /// <returns>A 3-tuple (triple) whose value is (item1, item2, item3).</returns>
        [Pure]
        public static ValueTuple<T1, T2, T3> Create<T1, T2, T3>(T1 item1, T2 item2, T3 item3) =>
            new(item1, item2, item3);

        /// <summary>Creates a new struct 4-tuple, or quadruple.</summary>
        /// <typeparam name="T1">The type of the first component of the tuple.</typeparam>
        /// <typeparam name="T2">The type of the second component of the tuple.</typeparam>
        /// <typeparam name="T3">The type of the third component of the tuple.</typeparam>
        /// <typeparam name="T4">The type of the fourth component of the tuple.</typeparam>
        /// <param name="item1">The value of the first component of the tuple.</param>
        /// <param name="item2">The value of the second component of the tuple.</param>
        /// <param name="item3">The value of the third component of the tuple.</param>
        /// <param name="item4">The value of the fourth component of the tuple.</param>
        /// <returns>A 4-tuple (quadruple) whose value is (item1, item2, item3, item4).</returns>
        [Pure]
        public static ValueTuple<T1, T2, T3, T4> Create<T1, T2, T3, T4>(T1 item1, T2 item2, T3 item3, T4 item4) =>
            new(item1, item2, item3, item4);

        /// <summary>Creates a new struct 5-tuple, or quintuple.</summary>
        /// <typeparam name="T1">The type of the first component of the tuple.</typeparam>
        /// <typeparam name="T2">The type of the second component of the tuple.</typeparam>
        /// <typeparam name="T3">The type of the third component of the tuple.</typeparam>
        /// <typeparam name="T4">The type of the fourth component of the tuple.</typeparam>
        /// <typeparam name="T5">The type of the fifth component of the tuple.</typeparam>
        /// <param name="item1">The value of the first component of the tuple.</param>
        /// <param name="item2">The value of the second component of the tuple.</param>
        /// <param name="item3">The value of the third component of the tuple.</param>
        /// <param name="item4">The value of the fourth component of the tuple.</param>
        /// <param name="item5">The value of the fifth component of the tuple.</param>
        /// <returns>A 5-tuple (quintuple) whose value is (item1, item2, item3, item4, item5).</returns>
        [Pure]
        public static ValueTuple<T1, T2, T3, T4, T5> Create<T1, T2, T3, T4, T5>(
            T1 item1,
            T2 item2,
            T3 item3,
            T4 item4,
            T5 item5
        ) =>
            new(item1, item2, item3, item4, item5);

        /// <summary>Creates a new struct 6-tuple, or sextuple.</summary>
        /// <typeparam name="T1">The type of the first component of the tuple.</typeparam>
        /// <typeparam name="T2">The type of the second component of the tuple.</typeparam>
        /// <typeparam name="T3">The type of the third component of the tuple.</typeparam>
        /// <typeparam name="T4">The type of the fourth component of the tuple.</typeparam>
        /// <typeparam name="T5">The type of the fifth component of the tuple.</typeparam>
        /// <typeparam name="T6">The type of the sixth component of the tuple.</typeparam>
        /// <param name="item1">The value of the first component of the tuple.</param>
        /// <param name="item2">The value of the second component of the tuple.</param>
        /// <param name="item3">The value of the third component of the tuple.</param>
        /// <param name="item4">The value of the fourth component of the tuple.</param>
        /// <param name="item5">The value of the fifth component of the tuple.</param>
        /// <param name="item6">The value of the sixth component of the tuple.</param>
        /// <returns>A 6-tuple (sextuple) whose value is (item1, item2, item3, item4, item5, item6).</returns>
        [Pure]
        public static ValueTuple<T1, T2, T3, T4, T5, T6> Create<T1, T2, T3, T4, T5, T6>(
            T1 item1,
            T2 item2,
            T3 item3,
            T4 item4,
            T5 item5,
            T6 item6
        ) =>
            new(item1, item2, item3, item4, item5, item6);

        /// <summary>Creates a new struct 7-tuple, or septuple.</summary>
        /// <typeparam name="T1">The type of the first component of the tuple.</typeparam>
        /// <typeparam name="T2">The type of the second component of the tuple.</typeparam>
        /// <typeparam name="T3">The type of the third component of the tuple.</typeparam>
        /// <typeparam name="T4">The type of the fourth component of the tuple.</typeparam>
        /// <typeparam name="T5">The type of the fifth component of the tuple.</typeparam>
        /// <typeparam name="T6">The type of the sixth component of the tuple.</typeparam>
        /// <typeparam name="T7">The type of the seventh component of the tuple.</typeparam>
        /// <param name="item1">The value of the first component of the tuple.</param>
        /// <param name="item2">The value of the second component of the tuple.</param>
        /// <param name="item3">The value of the third component of the tuple.</param>
        /// <param name="item4">The value of the fourth component of the tuple.</param>
        /// <param name="item5">The value of the fifth component of the tuple.</param>
        /// <param name="item6">The value of the sixth component of the tuple.</param>
        /// <param name="item7">The value of the seventh component of the tuple.</param>
        /// <returns>A 7-tuple (septuple) whose value is (item1, item2, item3, item4, item5, item6, item7).</returns>
        [Pure]
        public static ValueTuple<T1, T2, T3, T4, T5, T6, T7> Create<T1, T2, T3, T4, T5, T6, T7>(
            T1 item1,
            T2 item2,
            T3 item3,
            T4 item4,
            T5 item5,
            T6 item6,
            T7 item7
        ) =>
            new(item1, item2, item3, item4, item5, item6, item7);

        /// <summary>Creates a new struct 8-tuple, or octuple.</summary>
        /// <typeparam name="T1">The type of the first component of the tuple.</typeparam>
        /// <typeparam name="T2">The type of the second component of the tuple.</typeparam>
        /// <typeparam name="T3">The type of the third component of the tuple.</typeparam>
        /// <typeparam name="T4">The type of the fourth component of the tuple.</typeparam>
        /// <typeparam name="T5">The type of the fifth component of the tuple.</typeparam>
        /// <typeparam name="T6">The type of the sixth component of the tuple.</typeparam>
        /// <typeparam name="T7">The type of the seventh component of the tuple.</typeparam>
        /// <typeparam name="T8">The type of the eighth component of the tuple.</typeparam>
        /// <param name="item1">The value of the first component of the tuple.</param>
        /// <param name="item2">The value of the second component of the tuple.</param>
        /// <param name="item3">The value of the third component of the tuple.</param>
        /// <param name="item4">The value of the fourth component of the tuple.</param>
        /// <param name="item5">The value of the fifth component of the tuple.</param>
        /// <param name="item6">The value of the sixth component of the tuple.</param>
        /// <param name="item7">The value of the seventh component of the tuple.</param>
        /// <param name="item8">The value of the eighth component of the tuple.</param>
        /// <returns>An 8-tuple (octuple) whose value is (item1, item2, item3, item4, item5, item6, item7, item8).</returns>
        [Pure]
        public static ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8>> Create<T1, T2, T3, T4, T5, T6, T7, T8>(
            T1 item1,
            T2 item2,
            T3 item3,
            T4 item4,
            T5 item5,
            T6 item6,
            T7 item7,
            T8 item8
        ) =>
            new(
                item1,
                item2,
                item3,
                item4,
                item5,
                item6,
                item7,
                Create(item8)
            );

        [Pure]
        internal static int CombineHashCodes(int h1, int h2) => Combine(Combine(s_randomSeed, h1), h2);

        [Pure]
        internal static int CombineHashCodes(int h1, int h2, int h3) => Combine(CombineHashCodes(h1, h2), h3);

        [Pure]
        internal static int CombineHashCodes(int h1, int h2, int h3, int h4) =>
            Combine(CombineHashCodes(h1, h2, h3), h4);

        [Pure]
        internal static int CombineHashCodes(int h1, int h2, int h3, int h4, int h5) =>
            Combine(CombineHashCodes(h1, h2, h3, h4), h5);

        [Pure]
        internal static int CombineHashCodes(int h1, int h2, int h3, int h4, int h5, int h6) =>
            Combine(CombineHashCodes(h1, h2, h3, h4, h5), h6);

        [Pure]
        internal static int CombineHashCodes(
            int h1,
            int h2,
            int h3,
            int h4,
            int h5,
            int h6,
            int h7
        ) =>
            Combine(CombineHashCodes(h1, h2, h3, h4, h5, h6), h7);

        [Pure]
        internal static int CombineHashCodes(
            int h1,
            int h2,
            int h3,
            int h4,
            int h5,
            int h6,
            int h7,
            int h8
        ) =>
            Combine(CombineHashCodes(h1, h2, h3, h4, h5, h6, h7), h8);

        [Pure]
        static int Combine(int h1, int h2)
        {
            // RyuJIT optimizes this to use the ROL instruction
            // Related GitHub pull request: dotnet/coreclr#1830
            var rol5 = (uint)h1 << 5 | (uint)h1 >> 27;
            return (int)rol5 + h1 ^ h2;
        }
    }

    /// <summary>Represents a 1-tuple, or singleton, as a value type.</summary>
    /// <typeparam name="T1">The type of the tuple's only component.</typeparam>
#if !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6 && !NETCOREAPP1_0 && !NETCOREAPP1_1
    [Serializable]
#endif
    readonly struct ValueTuple<T1> : IEquatable<ValueTuple<T1>>,
#if !NET20 && !NET30 && !NET35
        IStructuralEquatable,
        IStructuralComparable,
#endif
        IComparable,
        IComparable<ValueTuple<T1>>,
        IValueTupleInternal
    {
        /// <summary>
        /// The current <see cref="ValueTuple{T1}"/> instance's first component.
        /// </summary>
        public readonly T1 Item1;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueTuple{T1}"/> value type.
        /// </summary>
        /// <param name="item1">The value of the tuple's first component.</param>
        public ValueTuple(T1 item1) => Item1 = item1;

        /// <summary>
        /// Returns a value that indicates whether the current <see cref="ValueTuple{T1}"/> instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">The object to compare with this instance.</param>
        /// <returns><see langword="true"/> if the current instance is equal to the specified object; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// The <paramref name="obj"/> parameter is considered to be equal to the current instance under the following conditions:
        /// <list type="bullet">
        ///     <item><description>It is a <see cref="ValueTuple{T1}"/> value type.</description></item>
        ///     <item><description>Its components are of the same types as those of the current instance.</description></item>
        ///     <item><description>Its components are equal to those of the current instance. Equality is determined by the default object equality comparer for each component.</description></item>
        /// </list>
        /// </remarks>
        [Pure]
        public override bool Equals([NotNullWhen(true)] object? obj) => obj is ValueTuple<T1> tuple && Equals(tuple);

        /// <summary>
        /// Returns a value that indicates whether the current <see cref="ValueTuple{T1}"/>
        /// instance is equal to a specified <see cref="ValueTuple{T1}"/>.
        /// </summary>
        /// <param name="other">The tuple to compare with this instance.</param>
        /// <returns><see langword="true"/> if the current instance is equal to the specified tuple; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// The <paramref name="other"/> parameter is considered to be equal to the current instance if each of its field
        /// is equal to that of the current instance, using the default comparer for that field's type.
        /// </remarks>
        [Pure]
        public bool Equals(ValueTuple<T1> other) => EqualityComparer<T1>.Default.Equals(Item1, other.Item1);

        [Pure]
        int IComparable.CompareTo(object? other)
        {
            if (other == null)
                return 1;

            if (other is not ValueTuple<T1>)
                throw new ArgumentException();

            var objTuple = (ValueTuple<T1>)other;

            return Comparer<T1>.Default.Compare(Item1, objTuple.Item1);
        }

        /// <summary>Compares this instance to a specified instance and returns an indication of their relative values.</summary>
        /// <param name="other">An instance to compare.</param>
        /// <returns>
        /// A signed number indicating the relative values of this instance and <paramref name="other"/>.
        /// Returns less than zero if this instance is less than <paramref name="other"/>, zero if this
        /// instance is equal to <paramref name="other"/>, and greater than zero if this instance is greater
        /// than <paramref name="other"/>.
        /// </returns>
        [Pure]
        public int CompareTo(ValueTuple<T1> other) => Comparer<T1>.Default.Compare(Item1, other.Item1);

        /// <summary>
        /// Returns the hash code for the current <see cref="ValueTuple{T1}"/> instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode() => Item1?.GetHashCode() ?? 0;

#if !NET20 && !NET30 && !NET35
        [Pure]
        bool IStructuralEquatable.Equals(object? other, IEqualityComparer comparer)
        {
            if (other is not ValueTuple<T1>)
                return false;

            var objTuple = (ValueTuple<T1>)other;

            return comparer.Equals(Item1, objTuple.Item1);
        }

        [Pure]
        int IStructuralEquatable.GetHashCode(IEqualityComparer comparer) => comparer.GetHashCode(Item1);

        [Pure]
        int IStructuralComparable.CompareTo(object? other, IComparer comparer)
        {
            if (other == null)
                return 1;

            if (other is not ValueTuple<T1>)
                throw new ArgumentException();

            var objTuple = (ValueTuple<T1>)other;

            return comparer.Compare(Item1, objTuple.Item1);
        }

#endif

        [Pure]
        int IValueTupleInternal.GetHashCode(IEqualityComparer comparer) => comparer.GetHashCode(Item1);

        /// <summary>
        /// Returns a string that represents the value of this <see cref="ValueTuple{T1}"/> instance.
        /// </summary>
        /// <returns>The string representation of this <see cref="ValueTuple{T1}"/> instance.</returns>
        /// <remarks>
        /// The string returned by this method takes the form <c>(Item1)</c>,
        /// where <see cref="Item1"/> represents the value of <see cref="Item1"/>. If the field is <see langword="null"/>,
        /// it is represented as <see cref="string.Empty"/>.
        /// </remarks>
        [Pure]
        public override string ToString() => $"({Item1})";

        [Pure]
        string IValueTupleInternal.ToStringEnd() => $"{Item1})";

        /// <summary>
        /// The number of positions in this data structure.
        /// </summary>
        [Pure, ValueRange(1)]
        int ITuple.Length => 1;

        /// <summary>
        /// Get the element at position <param name="index"/>.
        /// </summary>
        object? ITuple.this[[ValueRange(0)] int index]
        {
            get
            {
                if (index != 0)
                    throw new IndexOutOfRangeException();

                return Item1;
            }
        }
    }

    /// <summary>
    /// Represents a 2-tuple, or pair, as a value type.
    /// </summary>
    /// <typeparam name="T1">The type of the tuple's first component.</typeparam>
    /// <typeparam name="T2">The type of the tuple's second component.</typeparam>
#if !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6 && !NETCOREAPP1_0 && !NETCOREAPP1_1
    [Serializable]
#endif
    [StructLayout(LayoutKind.Auto)]
    readonly struct ValueTuple<T1, T2> : IEquatable<ValueTuple<T1, T2>>,
#if !NET20 && !NET30 && !NET35
        IStructuralEquatable,
        IStructuralComparable,
#endif
        IComparable,
        IComparable<ValueTuple<T1, T2>>,
        IValueTupleInternal
    {
        /// <summary>
        /// The current <see cref="ValueTuple{T1,T2}"/> instance's first component.
        /// </summary>
        public readonly T1 Item1;

        /// <summary>
        /// The current <see cref="ValueTuple{T1,T2}"/> instance's first component.
        /// </summary>
        public readonly T2 Item2;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueTuple{T1,T2}"/> value type.
        /// </summary>
        /// <param name="item1">The value of the tuple's first component.</param>
        /// <param name="item2">The value of the tuple's second component.</param>
        public ValueTuple(T1 item1, T2 item2)
        {
            Item1 = item1;
            Item2 = item2;
        }

        /// <summary>
        /// Returns a value that indicates whether the current <see cref="ValueTuple{T1,T2}"/> instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">The object to compare with this instance.</param>
        /// <returns><see langword="true"/> if the current instance is equal to the specified object; otherwise, <see langword="false"/>.</returns>
        ///
        /// <remarks>
        /// The <paramref name="obj"/> parameter is considered to be equal to the current instance under the following conditions:
        /// <list type="bullet">
        ///     <item><description>It is a <see cref="ValueTuple{T1,T2}"/> value type.</description></item>
        ///     <item><description>Its components are of the same types as those of the current instance.</description></item>
        ///     <item><description>Its components are equal to those of the current instance. Equality is determined by the default object equality comparer for each component.</description></item>
        /// </list>
        /// </remarks>
        [Pure]
        public override bool Equals([NotNullWhen(true)] object? obj) =>
            obj is ValueTuple<T1, T2> tuple && Equals(tuple);

        /// <summary>
        /// Returns a value that indicates whether the current <see cref="ValueTuple{T1,T2}"/> instance is equal to a specified <see cref="ValueTuple{T1,T2}"/>.
        /// </summary>
        /// <param name="other">The tuple to compare with this instance.</param>
        /// <returns><see langword="true"/> if the current instance is equal to the specified tuple; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// The <paramref name="other"/> parameter is considered to be equal to the current instance if each of its fields
        /// are equal to that of the current instance, using the default comparer for that field's type.
        /// </remarks>
        [Pure]
        public bool Equals(ValueTuple<T1, T2> other) =>
            EqualityComparer<T1>.Default.Equals(Item1, other.Item1) &&
            EqualityComparer<T2>.Default.Equals(Item2, other.Item2);

        [Pure]
        int IComparable.CompareTo(object? other)
        {
            if (other == null)
                return 1;

            if (other is not ValueTuple<T1, T2>)
                throw new ArgumentException();

            return CompareTo((ValueTuple<T1, T2>)other);
        }

        /// <summary>Compares this instance to a specified instance and returns an indication of their relative values.</summary>
        /// <param name="other">An instance to compare.</param>
        /// <returns>
        /// A signed number indicating the relative values of this instance and <paramref name="other"/>.
        /// Returns less than zero if this instance is less than <paramref name="other"/>, zero if this
        /// instance is equal to <paramref name="other"/>, and greater than zero if this instance is greater
        /// than <paramref name="other"/>.
        /// </returns>
        [Pure]
        public int CompareTo(ValueTuple<T1, T2> other)
        {
            var c = Comparer<T1>.Default.Compare(Item1, other.Item1);
            return c != 0 ? c : Comparer<T2>.Default.Compare(Item2, other.Item2);
        }

#if !NET20 && !NET30 && !NET35
        [Pure]
        bool IStructuralEquatable.Equals(object? other, IEqualityComparer comparer)
        {
            if (other is not ValueTuple<T1, T2>)
                return false;

            var objTuple = (ValueTuple<T1, T2>)other;

            return
                comparer.Equals(Item1, objTuple.Item1) &&
                comparer.Equals(Item2, objTuple.Item2);
        }

        [Pure]
        int IStructuralComparable.CompareTo(object? other, IComparer comparer)
        {
            if (other == null)
                return 1;

            if (other is not ValueTuple<T1, T2>)
                throw new ArgumentException();

            var objTuple = (ValueTuple<T1, T2>)other;

            var c = comparer.Compare(Item1, objTuple.Item1);

            return c != 0 ? c : comparer.Compare(Item2, objTuple.Item2);
        }

        [Pure]
        int IStructuralEquatable.GetHashCode(IEqualityComparer comparer) => GetHashCodeCore(comparer);
#endif

        /// <summary>
        /// Returns the hash code for the current <see cref="ValueTuple{T1,T2}"/> instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        [Pure]
        public override int GetHashCode() =>
            ValueTuple.CombineHashCodes(
                Item1?.GetHashCode() ?? 0,
                Item2?.GetHashCode() ?? 0
            );

        [Pure]
        int GetHashCodeCore(IEqualityComparer comparer) =>
            ValueTuple.CombineHashCodes(
                comparer.GetHashCode(Item1),
                comparer.GetHashCode(Item2)
            );

        [Pure]
        int IValueTupleInternal.GetHashCode(IEqualityComparer comparer) => GetHashCodeCore(comparer);

        /// <summary>
        /// Returns a string that represents the value of this <see cref="ValueTuple{T1,T2}"/> instance.
        /// </summary>
        /// <returns>The string representation of this <see cref="ValueTuple{T1,T2}"/> instance.</returns>
        /// <remarks>
        /// The string returned by this method takes the form <c>(Item1, Item2)</c>,
        /// where <see cref="Item1"/> and <see cref="Item2"/> represent the values of the <see cref="Item1"/>
        /// and <see cref="Item2"/> fields. If either field value is <see langword="null"/>,
        /// it is represented as <see cref="string.Empty"/>.
        /// </remarks>
        [Pure]
        public override string ToString() => $"({Item1}, {Item2})";

        [Pure]
        string IValueTupleInternal.ToStringEnd() => $"{Item1}, {Item2})";

        /// <summary>
        /// The number of positions in this data structure.
        /// </summary>
        [Pure, ValueRange(2)]
        int ITuple.Length => 2;

        /// <summary>
        /// Get the element at position <param name="index"/>.
        /// </summary>
        [Pure]
        object? ITuple.this[[ValueRange(0, 1)] int index] =>
            index switch
            {
                0 => Item1,
                1 => Item2,
                _ => throw new IndexOutOfRangeException(),
            };
    }

    /// <summary>
    /// Represents a 3-tuple, or triple, as a value type.
    /// </summary>
    /// <typeparam name="T1">The type of the tuple's first component.</typeparam>
    /// <typeparam name="T2">The type of the tuple's second component.</typeparam>
    /// <typeparam name="T3">The type of the tuple's third component.</typeparam>
#if !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6 && !NETCOREAPP1_0 && !NETCOREAPP1_1
    [Serializable]
#endif
    [StructLayout(LayoutKind.Auto)]
    readonly struct ValueTuple<T1, T2, T3> : IEquatable<ValueTuple<T1, T2, T3>>,
#if !NET20 && !NET30 && !NET35
        IStructuralEquatable,
        IStructuralComparable,
#endif
        IComparable,
        IComparable<ValueTuple<T1, T2, T3>>,
        IValueTupleInternal
    {
        /// <summary>
        /// The current <see cref="ValueTuple{T1,T2,T3}"/> instance's first component.
        /// </summary>
        public readonly T1 Item1;

        /// <summary>
        /// The current <see cref="ValueTuple{T1,T2,T3}"/> instance's second component.
        /// </summary>
        public readonly T2 Item2;

        /// <summary>
        /// The current <see cref="ValueTuple{T1,T2,T3}"/> instance's third component.
        /// </summary>
        public readonly T3 Item3;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueTuple{T1,T2,T3}"/> value type.
        /// </summary>
        /// <param name="item1">The value of the tuple's first component.</param>
        /// <param name="item2">The value of the tuple's second component.</param>
        /// <param name="item3">The value of the tuple's third component.</param>
        public ValueTuple(T1 item1, T2 item2, T3 item3)
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
        }

        /// <summary>
        /// Returns a value that indicates whether the current <see cref="ValueTuple{T1,T2,T3}"/> instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">The object to compare with this instance.</param>
        /// <returns><see langword="true"/> if the current instance is equal to the specified object; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// The <paramref name="obj"/> parameter is considered to be equal to the current instance under the following conditions:
        /// <list type="bullet">
        ///     <item><description>It is a <see cref="ValueTuple{T1,T2,T3}"/> value type.</description></item>
        ///     <item><description>Its components are of the same types as those of the current instance.</description></item>
        ///     <item><description>Its components are equal to those of the current instance. Equality is determined by the default object equality comparer for each component.</description></item>
        /// </list>
        /// </remarks>
        [Pure]
        public override bool Equals([NotNullWhen(true)] object? obj) =>
            obj is ValueTuple<T1, T2, T3> tuple && Equals(tuple);

        /// <summary>
        /// Returns a value that indicates whether the current <see cref="ValueTuple{T1,T2,T3}"/>
        /// instance is equal to a specified <see cref="ValueTuple{T1,T2,T3}"/>.
        /// </summary>
        /// <param name="other">The tuple to compare with this instance.</param>
        /// <returns><see langword="true"/> if the current instance is equal to the specified tuple; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// The <paramref name="other"/> parameter is considered to be equal to the current instance if each of its fields
        /// are equal to that of the current instance, using the default comparer for that field's type.
        /// </remarks>
        [Pure]
        public bool Equals(ValueTuple<T1, T2, T3> other) =>
            EqualityComparer<T1>.Default.Equals(Item1, other.Item1) &&
            EqualityComparer<T2>.Default.Equals(Item2, other.Item2) &&
            EqualityComparer<T3>.Default.Equals(Item3, other.Item3);

        [Pure]
        int IComparable.CompareTo(object? other)
        {
            if (other == null)
                return 1;

            if (other is not ValueTuple<T1, T2, T3>)
                throw new ArgumentException();

            return CompareTo((ValueTuple<T1, T2, T3>)other);
        }

        /// <summary>Compares this instance to a specified instance and returns an indication of their relative values.</summary>
        /// <param name="other">An instance to compare.</param>
        /// <returns>
        /// A signed number indicating the relative values of this instance and <paramref name="other"/>.
        /// Returns less than zero if this instance is less than <paramref name="other"/>, zero if this
        /// instance is equal to <paramref name="other"/>, and greater than zero if this instance is greater
        /// than <paramref name="other"/>.
        /// </returns>
        [Pure]
        public int CompareTo(ValueTuple<T1, T2, T3> other)
        {
            var c = Comparer<T1>.Default.Compare(Item1, other.Item1);

            if (c != 0)
                return c;

            c = Comparer<T2>.Default.Compare(Item2, other.Item2);
            return c != 0 ? c : Comparer<T3>.Default.Compare(Item3, other.Item3);
        }

#if !NET20 && !NET30 && !NET35
        [Pure]
        bool IStructuralEquatable.Equals(object? other, IEqualityComparer comparer)
        {
            if (other is not ValueTuple<T1, T2, T3>)
                return false;

            var objTuple = (ValueTuple<T1, T2, T3>)other;

            return
                comparer.Equals(Item1, objTuple.Item1) &&
                comparer.Equals(Item2, objTuple.Item2) &&
                comparer.Equals(Item3, objTuple.Item3);
        }

        [Pure]
        int IStructuralComparable.CompareTo(object? other, IComparer comparer)
        {
            if (other == null)
                return 1;

            if (other is not ValueTuple<T1, T2, T3>)
                throw new ArgumentException();

            var objTuple = (ValueTuple<T1, T2, T3>)other;

            var c = comparer.Compare(Item1, objTuple.Item1);

            if (c != 0)
                return c;

            c = comparer.Compare(Item2, objTuple.Item2);
            return c != 0 ? c : comparer.Compare(Item3, objTuple.Item3);
        }

        [Pure]
        int IStructuralEquatable.GetHashCode(IEqualityComparer comparer) => GetHashCodeCore(comparer);
#endif

        /// <summary>
        /// Returns the hash code for the current <see cref="ValueTuple{T1,T2,T3}"/> instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        [Pure]
        public override int GetHashCode() =>
            ValueTuple.CombineHashCodes(
                Item1?.GetHashCode() ?? 0,
                Item2?.GetHashCode() ?? 0,
                Item3?.GetHashCode() ?? 0
            );

        [Pure]
        int GetHashCodeCore(IEqualityComparer comparer) =>
            ValueTuple.CombineHashCodes(
                comparer.GetHashCode(Item1),
                comparer.GetHashCode(Item2),
                comparer.GetHashCode(Item3)
            );

        [Pure]
        int IValueTupleInternal.GetHashCode(IEqualityComparer comparer) => GetHashCodeCore(comparer);

        /// <summary>
        /// Returns a string that represents the value of this <see cref="ValueTuple{T1,T2,T3}"/> instance.
        /// </summary>
        /// <returns>The string representation of this <see cref="ValueTuple{T1,T2,T3}"/> instance.</returns>
        /// <remarks>
        /// The string returned by this method takes the form <c>(Item1, Item2, Item3)</c>.
        /// If any field value is <see langword="null"/>, it is represented as <see cref="string.Empty"/>.
        /// </remarks>
        [Pure]
        public override string ToString() => $"({Item1}, {Item2}, {Item3})";

        [Pure]
        string IValueTupleInternal.ToStringEnd() => $"{Item1}, {Item2}, {Item3})";

        /// <summary>
        /// The number of positions in this data structure.
        /// </summary>
        [Pure, ValueRange(3)]
        int ITuple.Length => 3;

        /// <summary>
        /// Get the element at position <param name="index"/>.
        /// </summary>
        [Pure]
        object? ITuple.this[[ValueRange(0, 2)] int index] =>
            index switch
            {
                0 => Item1,
                1 => Item2,
                2 => Item3,
                _ => throw new IndexOutOfRangeException(),
            };
    }

    /// <summary>
    /// Represents a 4-tuple, or quadruple, as a value type.
    /// </summary>
    /// <typeparam name="T1">The type of the tuple's first component.</typeparam>
    /// <typeparam name="T2">The type of the tuple's second component.</typeparam>
    /// <typeparam name="T3">The type of the tuple's third component.</typeparam>
    /// <typeparam name="T4">The type of the tuple's fourth component.</typeparam>
#if !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6 && !NETCOREAPP1_0 && !NETCOREAPP1_1
    [Serializable]
#endif
    [StructLayout(LayoutKind.Auto)]
    readonly struct ValueTuple<T1, T2, T3, T4> : IEquatable<ValueTuple<T1, T2, T3, T4>>,
#if !NET20 && !NET30 && !NET35
        IStructuralEquatable,
        IStructuralComparable,
#endif
        IComparable,
        IComparable<ValueTuple<T1, T2, T3, T4>>,
        IValueTupleInternal
    {
        /// <summary>
        /// The current <see cref="ValueTuple{T1,T2,T3,T4}"/> instance's first component.
        /// </summary>
        public readonly T1 Item1;

        /// <summary>
        /// The current <see cref="ValueTuple{T1,T2,T3,T4}"/> instance's second component.
        /// </summary>
        public readonly T2 Item2;

        /// <summary>
        /// The current <see cref="ValueTuple{T1,T2,T3,T4}"/> instance's third component.
        /// </summary>
        public readonly T3 Item3;

        /// <summary>
        /// The current <see cref="ValueTuple{T1,T2,T3,T4}"/> instance's fourth component.
        /// </summary>
        public readonly T4 Item4;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueTuple{T1,T2,T3,T4}"/> value type.
        /// </summary>
        /// <param name="item1">The value of the tuple's first component.</param>
        /// <param name="item2">The value of the tuple's second component.</param>
        /// <param name="item3">The value of the tuple's third component.</param>
        /// <param name="item4">The value of the tuple's fourth component.</param>
        public ValueTuple(T1 item1, T2 item2, T3 item3, T4 item4)
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
            Item4 = item4;
        }

        /// <summary>
        /// Returns a value that indicates whether the current <see cref="ValueTuple{T1,T2,T3,T4}"/> instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">The object to compare with this instance.</param>
        /// <returns><see langword="true"/> if the current instance is equal to the specified object; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// The <paramref name="obj"/> parameter is considered to be equal to the current instance under the following conditions:
        /// <list type="bullet">
        ///     <item><description>It is a <see cref="ValueTuple{T1,T2,T3,T4}"/> value type.</description></item>
        ///     <item><description>Its components are of the same types as those of the current instance.</description></item>
        ///     <item><description>Its components are equal to those of the current instance. Equality is determined by the default object equality comparer for each component.</description></item>
        /// </list>
        /// </remarks>
        [Pure]
        public override bool Equals([NotNullWhen(true)] object? obj) =>
            obj is ValueTuple<T1, T2, T3, T4> tuple && Equals(tuple);

        /// <summary>
        /// Returns a value that indicates whether the current <see cref="ValueTuple{T1,T2,T3,T4}"/>
        /// instance is equal to a specified <see cref="ValueTuple{T1,T2,T3,T4}"/>.
        /// </summary>
        /// <param name="other">The tuple to compare with this instance.</param>
        /// <returns><see langword="true"/> if the current instance is equal to the specified tuple; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// The <paramref name="other"/> parameter is considered to be equal to the current instance if each of its fields
        /// are equal to that of the current instance, using the default comparer for that field's type.
        /// </remarks>
        [Pure]
        public bool Equals(ValueTuple<T1, T2, T3, T4> other) =>
            EqualityComparer<T1>.Default.Equals(Item1, other.Item1) &&
            EqualityComparer<T2>.Default.Equals(Item2, other.Item2) &&
            EqualityComparer<T3>.Default.Equals(Item3, other.Item3) &&
            EqualityComparer<T4>.Default.Equals(Item4, other.Item4);

        [Pure]
        int IComparable.CompareTo(object? other)
        {
            if (other == null)
                return 1;

            if (other is not ValueTuple<T1, T2, T3, T4>)
                throw new ArgumentException();

            return CompareTo((ValueTuple<T1, T2, T3, T4>)other);
        }

        /// <summary>Compares this instance to a specified instance and returns an indication of their relative values.</summary>
        /// <param name="other">An instance to compare.</param>
        /// <returns>
        /// A signed number indicating the relative values of this instance and <paramref name="other"/>.
        /// Returns less than zero if this instance is less than <paramref name="other"/>, zero if this
        /// instance is equal to <paramref name="other"/>, and greater than zero if this instance is greater
        /// than <paramref name="other"/>.
        /// </returns>
        [Pure]
        public int CompareTo(ValueTuple<T1, T2, T3, T4> other)
        {
            var c = Comparer<T1>.Default.Compare(Item1, other.Item1);

            if (c != 0)
                return c;

            c = Comparer<T2>.Default.Compare(Item2, other.Item2);

            if (c != 0)
                return c;

            c = Comparer<T3>.Default.Compare(Item3, other.Item3);
            return c != 0 ? c : Comparer<T4>.Default.Compare(Item4, other.Item4);
        }

#if !NET20 && !NET30 && !NET35
        [Pure]
        bool IStructuralEquatable.Equals(object? other, IEqualityComparer comparer)
        {
            if (other is not ValueTuple<T1, T2, T3, T4>)
                return false;

            var objTuple = (ValueTuple<T1, T2, T3, T4>)other;

            return
                comparer.Equals(Item1, objTuple.Item1) &&
                comparer.Equals(Item2, objTuple.Item2) &&
                comparer.Equals(Item3, objTuple.Item3) &&
                comparer.Equals(Item4, objTuple.Item4);
        }

        [Pure]
        int IStructuralComparable.CompareTo(object? other, IComparer comparer)
        {
            if (other == null)
                return 1;

            if (other is not ValueTuple<T1, T2, T3, T4>)
                throw new ArgumentException();

            var objTuple = (ValueTuple<T1, T2, T3, T4>)other;

            var c = comparer.Compare(Item1, objTuple.Item1);

            if (c != 0)
                return c;

            c = comparer.Compare(Item2, objTuple.Item2);

            if (c != 0)
                return c;

            c = comparer.Compare(Item3, objTuple.Item3);
            return c != 0 ? c : comparer.Compare(Item4, objTuple.Item4);
        }

        [Pure]
        int IStructuralEquatable.GetHashCode(IEqualityComparer comparer) => GetHashCodeCore(comparer);
#endif

        /// <summary>
        /// Returns the hash code for the current <see cref="ValueTuple{T1,T2,T3,T4}"/> instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        [Pure]
        public override int GetHashCode() =>
            ValueTuple.CombineHashCodes(
                Item1?.GetHashCode() ?? 0,
                Item2?.GetHashCode() ?? 0,
                Item3?.GetHashCode() ?? 0,
                Item4?.GetHashCode() ?? 0
            );

        [Pure]
        int GetHashCodeCore(IEqualityComparer comparer) =>
            ValueTuple.CombineHashCodes(
                comparer.GetHashCode(Item1),
                comparer.GetHashCode(Item2),
                comparer.GetHashCode(Item3),
                comparer.GetHashCode(Item4)
            );

        [Pure]
        int IValueTupleInternal.GetHashCode(IEqualityComparer comparer) => GetHashCodeCore(comparer);

        /// <summary>
        /// Returns a string that represents the value of this <see cref="ValueTuple{T1,T2,T3,T4}"/> instance.
        /// </summary>
        /// <returns>The string representation of this <see cref="ValueTuple{T1,T2,T3,T4}"/> instance.</returns>
        /// <remarks>
        /// The string returned by this method takes the form <c>(Item1, Item2, Item3, Item4)</c>.
        /// If any field value is <see langword="null"/>, it is represented as <see cref="string.Empty"/>.
        /// </remarks>
        [Pure]
        public override string ToString() => $"({Item1}, {Item2}, {Item3}, {Item4})";

        [Pure]
        string IValueTupleInternal.ToStringEnd() => $"{Item1}, {Item2}, {Item3}, {Item4})";

        /// <summary>
        /// The number of positions in this data structure.
        /// </summary>
        [Pure, ValueRange(4)]
        int ITuple.Length => 4;

        /// <summary>
        /// Get the element at position <param name="index"/>.
        /// </summary>
        [Pure]
        object? ITuple.this[[ValueRange(0, 3)] int index] =>
            index switch
            {
                0 => Item1,
                1 => Item2,
                2 => Item3,
                3 => Item4,
                _ => throw new IndexOutOfRangeException(),
            };
    }

    /// <summary>
    /// Represents a 5-tuple, or quintuple, as a value type.
    /// </summary>
    /// <typeparam name="T1">The type of the tuple's first component.</typeparam>
    /// <typeparam name="T2">The type of the tuple's second component.</typeparam>
    /// <typeparam name="T3">The type of the tuple's third component.</typeparam>
    /// <typeparam name="T4">The type of the tuple's fourth component.</typeparam>
    /// <typeparam name="T5">The type of the tuple's fifth component.</typeparam>
#if !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6 && !NETCOREAPP1_0 && !NETCOREAPP1_1
    [Serializable]
#endif
    [StructLayout(LayoutKind.Auto)]
    readonly struct ValueTuple<T1, T2, T3, T4, T5> : IEquatable<ValueTuple<T1, T2, T3, T4, T5>>,
#if !NET20 && !NET30 && !NET35
        IStructuralEquatable,
        IStructuralComparable,
#endif
        IComparable,
        IComparable<ValueTuple<T1, T2, T3, T4, T5>>,
        IValueTupleInternal
    {
        /// <summary>
        /// The current <see cref="ValueTuple{T1,T2,T3,T4,T5}"/> instance's first component.
        /// </summary>
        public readonly T1 Item1;

        /// <summary>
        /// The current <see cref="ValueTuple{T1,T2,T3,T4,T5}"/> instance's second component.
        /// </summary>
        public readonly T2 Item2;

        /// <summary>
        /// The current <see cref="ValueTuple{T1,T2,T3,T4,T5}"/> instance's third component.
        /// </summary>
        public readonly T3 Item3;

        /// <summary>
        /// The current <see cref="ValueTuple{T1,T2,T3,T4,T5}"/> instance's fourth component.
        /// </summary>
        public readonly T4 Item4;

        /// <summary>
        /// The current <see cref="ValueTuple{T1,T2,T3,T4,T5}"/> instance's fifth component.
        /// </summary>
        public readonly T5 Item5;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueTuple{T1,T2,T3,T4,T5}"/> value type.
        /// </summary>
        /// <param name="item1">The value of the tuple's first component.</param>
        /// <param name="item2">The value of the tuple's second component.</param>
        /// <param name="item3">The value of the tuple's third component.</param>
        /// <param name="item4">The value of the tuple's fourth component.</param>
        /// <param name="item5">The value of the tuple's fifth component.</param>
        public ValueTuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5)
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
            Item4 = item4;
            Item5 = item5;
        }

        /// <summary>
        /// Returns a value that indicates whether the current <see cref="ValueTuple{T1,T2,T3,T4,T5}"/> instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">The object to compare with this instance.</param>
        /// <returns><see langword="true"/> if the current instance is equal to the specified object; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// The <paramref name="obj"/> parameter is considered to be equal to the current instance under the following conditions:
        /// <list type="bullet">
        ///     <item><description>It is a <see cref="ValueTuple{T1,T2,T3,T4,T5}"/> value type.</description></item>
        ///     <item><description>Its components are of the same types as those of the current instance.</description></item>
        ///     <item><description>Its components are equal to those of the current instance. Equality is determined by the default object equality comparer for each component.</description></item>
        /// </list>
        /// </remarks>
        [Pure]
        public override bool Equals([NotNullWhen(true)] object? obj) =>
            obj is ValueTuple<T1, T2, T3, T4, T5> tuple && Equals(tuple);

        /// <summary>
        /// Returns a value that indicates whether the current <see cref="ValueTuple{T1,T2,T3,T4,T5}"/>
        /// instance is equal to a specified <see cref="ValueTuple{T1,T2,T3,T4,T5}"/>.
        /// </summary>
        /// <param name="other">The tuple to compare with this instance.</param>
        /// <returns><see langword="true"/> if the current instance is equal to the specified tuple; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// The <paramref name="other"/> parameter is considered to be equal to the current instance if each of its fields
        /// are equal to that of the current instance, using the default comparer for that field's type.
        /// </remarks>
        [Pure]
        public bool Equals(ValueTuple<T1, T2, T3, T4, T5> other) =>
            EqualityComparer<T1>.Default.Equals(Item1, other.Item1) &&
            EqualityComparer<T2>.Default.Equals(Item2, other.Item2) &&
            EqualityComparer<T3>.Default.Equals(Item3, other.Item3) &&
            EqualityComparer<T4>.Default.Equals(Item4, other.Item4) &&
            EqualityComparer<T5>.Default.Equals(Item5, other.Item5);

        [Pure]
        int IComparable.CompareTo(object? other)
        {
            if (other == null)
                return 1;

            if (other is not ValueTuple<T1, T2, T3, T4, T5>)
                throw new ArgumentException();

            return CompareTo((ValueTuple<T1, T2, T3, T4, T5>)other);
        }

        /// <summary>Compares this instance to a specified instance and returns an indication of their relative values.</summary>
        /// <param name="other">An instance to compare.</param>
        /// <returns>
        /// A signed number indicating the relative values of this instance and <paramref name="other"/>.
        /// Returns less than zero if this instance is less than <paramref name="other"/>, zero if this
        /// instance is equal to <paramref name="other"/>, and greater than zero if this instance is greater
        /// than <paramref name="other"/>.
        /// </returns>
        [Pure]
        public int CompareTo(ValueTuple<T1, T2, T3, T4, T5> other)
        {
            var c = Comparer<T1>.Default.Compare(Item1, other.Item1);

            if (c != 0)
                return c;

            c = Comparer<T2>.Default.Compare(Item2, other.Item2);

            if (c != 0)
                return c;

            c = Comparer<T3>.Default.Compare(Item3, other.Item3);

            if (c != 0)
                return c;

            c = Comparer<T4>.Default.Compare(Item4, other.Item4);
            return c != 0 ? c : Comparer<T5>.Default.Compare(Item5, other.Item5);
        }

#if !NET20 && !NET30 && !NET35
        [Pure]
        bool IStructuralEquatable.Equals(object? other, IEqualityComparer comparer)
        {
            if (other is not ValueTuple<T1, T2, T3, T4, T5>)
                return false;

            var objTuple = (ValueTuple<T1, T2, T3, T4, T5>)other;

            return
                comparer.Equals(Item1, objTuple.Item1) &&
                comparer.Equals(Item2, objTuple.Item2) &&
                comparer.Equals(Item3, objTuple.Item3) &&
                comparer.Equals(Item4, objTuple.Item4) &&
                comparer.Equals(Item5, objTuple.Item5);
        }

        [Pure]
        int IStructuralComparable.CompareTo(object? other, IComparer comparer)
        {
            if (other == null)
                return 1;

            if (other is not ValueTuple<T1, T2, T3, T4, T5>)
                throw new ArgumentException();

            var objTuple = (ValueTuple<T1, T2, T3, T4, T5>)other;

            var c = comparer.Compare(Item1, objTuple.Item1);

            if (c != 0)
                return c;

            c = comparer.Compare(Item2, objTuple.Item2);

            if (c != 0)
                return c;

            c = comparer.Compare(Item3, objTuple.Item3);

            if (c != 0)
                return c;

            c = comparer.Compare(Item4, objTuple.Item4);
            return c != 0 ? c : comparer.Compare(Item5, objTuple.Item5);
        }

        [Pure]
        int IStructuralEquatable.GetHashCode(IEqualityComparer comparer) => GetHashCodeCore(comparer);
#endif

        /// <summary>
        /// Returns the hash code for the current <see cref="ValueTuple{T1,T2,T3,T4,T5}"/> instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        [Pure]
        public override int GetHashCode() =>
            ValueTuple.CombineHashCodes(
                Item1?.GetHashCode() ?? 0,
                Item2?.GetHashCode() ?? 0,
                Item3?.GetHashCode() ?? 0,
                Item4?.GetHashCode() ?? 0,
                Item5?.GetHashCode() ?? 0
            );

        [Pure]
        int GetHashCodeCore(IEqualityComparer comparer) =>
            ValueTuple.CombineHashCodes(
                comparer.GetHashCode(Item1),
                comparer.GetHashCode(Item2),
                comparer.GetHashCode(Item3),
                comparer.GetHashCode(Item4),
                comparer.GetHashCode(Item5)
            );

        [Pure]
        int IValueTupleInternal.GetHashCode(IEqualityComparer comparer) => GetHashCodeCore(comparer);

        /// <summary>
        /// Returns a string that represents the value of this <see cref="ValueTuple{T1,T2,T3,T4,T5}"/> instance.
        /// </summary>
        /// <returns>The string representation of this <see cref="ValueTuple{T1,T2,T3,T4,T5}"/> instance.</returns>
        /// <remarks>
        /// The string returned by this method takes the form <c>(Item1, Item2, Item3, Item4, Item5)</c>.
        /// If any field value is <see langword="null"/>, it is represented as <see cref="string.Empty"/>.
        /// </remarks>
        [Pure]
        public override string ToString() => $"({Item1}, {Item2}, {Item3}, {Item4}, {Item5})";

        [Pure]
        string IValueTupleInternal.ToStringEnd() => $"{Item1}, {Item2}, {Item3}, {Item4}, {Item5})";

        /// <summary>
        /// The number of positions in this data structure.
        /// </summary>
        [Pure, ValueRange(5)]
        int ITuple.Length => 5;

        /// <summary>
        /// Get the element at position <param name="index"/>.
        /// </summary>
        object? ITuple.this[[ValueRange(0, 4)] int index] =>
            index switch
            {
                0 => Item1,
                1 => Item2,
                2 => Item3,
                3 => Item4,
                4 => Item5,
                _ => throw new IndexOutOfRangeException(),
            };
    }

    /// <summary>
    /// Represents a 6-tuple, or sixtuple, as a value type.
    /// </summary>
    /// <typeparam name="T1">The type of the tuple's first component.</typeparam>
    /// <typeparam name="T2">The type of the tuple's second component.</typeparam>
    /// <typeparam name="T3">The type of the tuple's third component.</typeparam>
    /// <typeparam name="T4">The type of the tuple's fourth component.</typeparam>
    /// <typeparam name="T5">The type of the tuple's fifth component.</typeparam>
    /// <typeparam name="T6">The type of the tuple's sixth component.</typeparam>
#if !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6 && !NETCOREAPP1_0 && !NETCOREAPP1_1
    [Serializable]
#endif
    [StructLayout(LayoutKind.Auto)]
    readonly struct ValueTuple<T1, T2, T3, T4, T5, T6> : IEquatable<ValueTuple<T1, T2, T3, T4, T5, T6>>,
#if !NET20 && !NET30 && !NET35
        IStructuralEquatable,
        IStructuralComparable,
#endif
        IComparable,
        IComparable<ValueTuple<T1, T2, T3, T4, T5, T6>>,
        IValueTupleInternal
    {
        /// <summary>
        /// The current <see cref="ValueTuple{T1,T2,T3,T4,T5,T6}"/> instance's first component.
        /// </summary>
        public readonly T1 Item1;

        /// <summary>
        /// The current <see cref="ValueTuple{T1,T2,T3,T4,T5,T6}"/> instance's second component.
        /// </summary>
        public readonly T2 Item2;

        /// <summary>
        /// The current <see cref="ValueTuple{T1,T2,T3,T4,T5,T6}"/> instance's third component.
        /// </summary>
        public readonly T3 Item3;

        /// <summary>
        /// The current <see cref="ValueTuple{T1,T2,T3,T4,T5,T6}"/> instance's fourth component.
        /// </summary>
        public readonly T4 Item4;

        /// <summary>
        /// The current <see cref="ValueTuple{T1,T2,T3,T4,T5,T6}"/> instance's fifth component.
        /// </summary>
        public readonly T5 Item5;

        /// <summary>
        /// The current <see cref="ValueTuple{T1,T2,T3,T4,T5,T6}"/> instance's sixth component.
        /// </summary>
        public readonly T6 Item6;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueTuple{T1,T2,T3,T4,T5,T6}"/> value type.
        /// </summary>
        /// <param name="item1">The value of the tuple's first component.</param>
        /// <param name="item2">The value of the tuple's second component.</param>
        /// <param name="item3">The value of the tuple's third component.</param>
        /// <param name="item4">The value of the tuple's fourth component.</param>
        /// <param name="item5">The value of the tuple's fifth component.</param>
        /// <param name="item6">The value of the tuple's sixth component.</param>
        public ValueTuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6)
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
            Item4 = item4;
            Item5 = item5;
            Item6 = item6;
        }

        /// <summary>
        /// Returns a value that indicates whether the current <see cref="ValueTuple{T1,T2,T3,T4,T5,T6}"/> instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">The object to compare with this instance.</param>
        /// <returns><see langword="true"/> if the current instance is equal to the specified object; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// The <paramref name="obj"/> parameter is considered to be equal to the current instance under the following conditions:
        /// <list type="bullet">
        ///     <item><description>It is a <see cref="ValueTuple{T1,T2,T3,T4,T5,T6}"/> value type.</description></item>
        ///     <item><description>Its components are of the same types as those of the current instance.</description></item>
        ///     <item><description>Its components are equal to those of the current instance. Equality is determined by the default object equality comparer for each component.</description></item>
        /// </list>
        /// </remarks>
        [Pure]
        public override bool Equals([NotNullWhen(true)] object? obj) =>
            obj is ValueTuple<T1, T2, T3, T4, T5, T6> tuple && Equals(tuple);

        /// <summary>
        /// Returns a value that indicates whether the current <see cref="ValueTuple{T1,T2,T3,T4,T5,T6}"/>
        /// instance is equal to a specified <see cref="ValueTuple{T1,T2,T3,T4,T5,T6}"/>.
        /// </summary>
        /// <param name="other">The tuple to compare with this instance.</param>
        /// <returns><see langword="true"/> if the current instance is equal to the specified tuple; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// The <paramref name="other"/> parameter is considered to be equal to the current instance if each of its fields
        /// are equal to that of the current instance, using the default comparer for that field's type.
        /// </remarks>
        [Pure]
        public bool Equals(ValueTuple<T1, T2, T3, T4, T5, T6> other) =>
            EqualityComparer<T1>.Default.Equals(Item1, other.Item1) &&
            EqualityComparer<T2>.Default.Equals(Item2, other.Item2) &&
            EqualityComparer<T3>.Default.Equals(Item3, other.Item3) &&
            EqualityComparer<T4>.Default.Equals(Item4, other.Item4) &&
            EqualityComparer<T5>.Default.Equals(Item5, other.Item5) &&
            EqualityComparer<T6>.Default.Equals(Item6, other.Item6);

        [Pure]
        int IComparable.CompareTo(object? other)
        {
            if (other == null)
                return 1;

            if (other is not ValueTuple<T1, T2, T3, T4, T5, T6>)
                throw new ArgumentException();

            return CompareTo((ValueTuple<T1, T2, T3, T4, T5, T6>)other);
        }

        /// <summary>Compares this instance to a specified instance and returns an indication of their relative values.</summary>
        /// <param name="other">An instance to compare.</param>
        /// <returns>
        /// A signed number indicating the relative values of this instance and <paramref name="other"/>.
        /// Returns less than zero if this instance is less than <paramref name="other"/>, zero if this
        /// instance is equal to <paramref name="other"/>, and greater than zero if this instance is greater
        /// than <paramref name="other"/>.
        /// </returns>
        [Pure]
        public int CompareTo(ValueTuple<T1, T2, T3, T4, T5, T6> other)
        {
            var c = Comparer<T1>.Default.Compare(Item1, other.Item1);

            if (c != 0)
                return c;

            c = Comparer<T2>.Default.Compare(Item2, other.Item2);

            if (c != 0)
                return c;

            c = Comparer<T3>.Default.Compare(Item3, other.Item3);

            if (c != 0)
                return c;

            c = Comparer<T4>.Default.Compare(Item4, other.Item4);

            if (c != 0)
                return c;

            c = Comparer<T5>.Default.Compare(Item5, other.Item5);
            return c != 0 ? c : Comparer<T6>.Default.Compare(Item6, other.Item6);
        }

#if !NET20 && !NET30 && !NET35
        [Pure]
        bool IStructuralEquatable.Equals(object? other, IEqualityComparer comparer)
        {
            if (other is not ValueTuple<T1, T2, T3, T4, T5, T6>)
                return false;

            var objTuple = (ValueTuple<T1, T2, T3, T4, T5, T6>)other;

            return
                comparer.Equals(Item1, objTuple.Item1) &&
                comparer.Equals(Item2, objTuple.Item2) &&
                comparer.Equals(Item3, objTuple.Item3) &&
                comparer.Equals(Item4, objTuple.Item4) &&
                comparer.Equals(Item5, objTuple.Item5) &&
                comparer.Equals(Item6, objTuple.Item6);
        }

        [Pure]
        int IStructuralComparable.CompareTo(object? other, IComparer comparer)
        {
            if (other == null)
                return 1;

            if (other is not ValueTuple<T1, T2, T3, T4, T5, T6>)
                throw new ArgumentException();

            var objTuple = (ValueTuple<T1, T2, T3, T4, T5, T6>)other;

            var c = comparer.Compare(Item1, objTuple.Item1);

            if (c != 0)
                return c;

            c = comparer.Compare(Item2, objTuple.Item2);

            if (c != 0)
                return c;

            c = comparer.Compare(Item3, objTuple.Item3);

            if (c != 0)
                return c;

            c = comparer.Compare(Item4, objTuple.Item4);

            if (c != 0)
                return c;

            c = comparer.Compare(Item5, objTuple.Item5);
            return c != 0 ? c : comparer.Compare(Item6, objTuple.Item6);
        }

        [Pure]
        int IStructuralEquatable.GetHashCode(IEqualityComparer comparer) => GetHashCodeCore(comparer);
#endif

        /// <summary>
        /// Returns the hash code for the current <see cref="ValueTuple{T1,T2,T3,T4,T5,T6}"/> instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        [Pure]
        public override int GetHashCode() =>
            ValueTuple.CombineHashCodes(
                Item1?.GetHashCode() ?? 0,
                Item2?.GetHashCode() ?? 0,
                Item3?.GetHashCode() ?? 0,
                Item4?.GetHashCode() ?? 0,
                Item5?.GetHashCode() ?? 0,
                Item6?.GetHashCode() ?? 0
            );

        [Pure]
        int GetHashCodeCore(IEqualityComparer comparer) =>
            ValueTuple.CombineHashCodes(
                comparer.GetHashCode(Item1),
                comparer.GetHashCode(Item2),
                comparer.GetHashCode(Item3),
                comparer.GetHashCode(Item4),
                comparer.GetHashCode(Item5),
                comparer.GetHashCode(Item6)
            );

        [Pure]
        int IValueTupleInternal.GetHashCode(IEqualityComparer comparer) => GetHashCodeCore(comparer);

        /// <summary>
        /// Returns a string that represents the value of this <see cref="ValueTuple{T1,T2,T3,T4,T5,T6}"/> instance.
        /// </summary>
        /// <returns>The string representation of this <see cref="ValueTuple{T1,T2,T3,T4,T5,T6}"/> instance.</returns>
        /// <remarks>
        /// The string returned by this method takes the form <c>(Item1, Item2, Item3, Item4, Item5, Item6)</c>.
        /// If any field value is <see langword="null"/>, it is represented as <see cref="string.Empty"/>.
        /// </remarks>
        [Pure]
        public override string ToString() => $"({Item1}, {Item2}, {Item3}, {Item4}, {Item5}, {Item6})";

        [Pure]
        string IValueTupleInternal.ToStringEnd() => $"{Item1}, {Item2}, {Item3}, {Item4}, {Item5}, {Item6})";

        /// <summary>
        /// The number of positions in this data structure.
        /// </summary>
        [Pure, ValueRange(6)]
        int ITuple.Length => 6;

        /// <summary>
        /// Get the element at position <param name="index"/>.
        /// </summary>
        [Pure]
        object? ITuple.this[[ValueRange(0, 5)] int index] =>
            index switch
            {
                0 => Item1,
                1 => Item2,
                2 => Item3,
                3 => Item4,
                4 => Item5,
                5 => Item6,
                _ => throw new IndexOutOfRangeException(),
            };
    }

    /// <summary>
    /// Represents a 7-tuple, or sentuple, as a value type.
    /// </summary>
    /// <typeparam name="T1">The type of the tuple's first component.</typeparam>
    /// <typeparam name="T2">The type of the tuple's second component.</typeparam>
    /// <typeparam name="T3">The type of the tuple's third component.</typeparam>
    /// <typeparam name="T4">The type of the tuple's fourth component.</typeparam>
    /// <typeparam name="T5">The type of the tuple's fifth component.</typeparam>
    /// <typeparam name="T6">The type of the tuple's sixth component.</typeparam>
    /// <typeparam name="T7">The type of the tuple's seventh component.</typeparam>
#if !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6 && !NETCOREAPP1_0 && !NETCOREAPP1_1
    [Serializable]
#endif
    [StructLayout(LayoutKind.Auto)]
    readonly struct ValueTuple<T1, T2, T3, T4, T5, T6, T7> : IEquatable<ValueTuple<T1, T2, T3, T4, T5, T6, T7>>,
#if !NET20 && !NET30 && !NET35
        IStructuralEquatable,
        IStructuralComparable,
#endif
        IComparable,
        IComparable<ValueTuple<T1, T2, T3, T4, T5, T6, T7>>,
        IValueTupleInternal
    {
        /// <summary>
        /// The current <see cref="ValueTuple{T1,T2,T3,T4,T5,T6,T7}"/> instance's first component.
        /// </summary>
        public readonly T1 Item1;

        /// <summary>
        /// The current <see cref="ValueTuple{T1,T2,T3,T4,T5,T6,T7}"/> instance's second component.
        /// </summary>
        public readonly T2 Item2;

        /// <summary>
        /// The current <see cref="ValueTuple{T1,T2,T3,T4,T5,T6,T7}"/> instance's third component.
        /// </summary>
        public readonly T3 Item3;

        /// <summary>
        /// The current <see cref="ValueTuple{T1,T2,T3,T4,T5,T6,T7}"/> instance's fourth component.
        /// </summary>
        public readonly T4 Item4;

        /// <summary>
        /// The current <see cref="ValueTuple{T1,T2,T3,T4,T5,T6,T7}"/> instance's fifth component.
        /// </summary>
        public readonly T5 Item5;

        /// <summary>
        /// The current <see cref="ValueTuple{T1,T2,T3,T4,T5,T6,T7}"/> instance's sixth component.
        /// </summary>
        public readonly T6 Item6;

        /// <summary>
        /// The current <see cref="ValueTuple{T1,T2,T3,T4,T5,T6,T7}"/> instance's seventh component.
        /// </summary>
        public readonly T7 Item7;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueTuple{T1,T2,T3,T4,T5,T6,T7}"/> value type.
        /// </summary>
        /// <param name="item1">The value of the tuple's first component.</param>
        /// <param name="item2">The value of the tuple's second component.</param>
        /// <param name="item3">The value of the tuple's third component.</param>
        /// <param name="item4">The value of the tuple's fourth component.</param>
        /// <param name="item5">The value of the tuple's fifth component.</param>
        /// <param name="item6">The value of the tuple's sixth component.</param>
        /// <param name="item7">The value of the tuple's seventh component.</param>
        public ValueTuple(
            T1 item1,
            T2 item2,
            T3 item3,
            T4 item4,
            T5 item5,
            T6 item6,
            T7 item7
        )
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
            Item4 = item4;
            Item5 = item5;
            Item6 = item6;
            Item7 = item7;
        }

        /// <summary>
        /// Returns a value that indicates whether the current <see cref="ValueTuple{T1,T2,T3,T4,T5,T6,T7}"/> instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">The object to compare with this instance.</param>
        /// <returns><see langword="true"/> if the current instance is equal to the specified object; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// The <paramref name="obj"/> parameter is considered to be equal to the current instance under the following conditions:
        /// <list type="bullet">
        ///     <item><description>It is a <see cref="ValueTuple{T1,T2,T3,T4,T5,T6,T7}"/> value type.</description></item>
        ///     <item><description>Its components are of the same types as those of the current instance.</description></item>
        ///     <item><description>Its components are equal to those of the current instance. Equality is determined by the default object equality comparer for each component.</description></item>
        /// </list>
        /// </remarks>
        [Pure]
        public override bool Equals([NotNullWhen(true)] object? obj) =>
            obj is ValueTuple<T1, T2, T3, T4, T5, T6, T7> tuple && Equals(tuple);

        /// <summary>
        /// Returns a value that indicates whether the current <see cref="ValueTuple{T1,T2,T3,T4,T5,T6,T7}"/>
        /// instance is equal to a specified <see cref="ValueTuple{T1,T2,T3,T4,T5,T6,T7}"/>.
        /// </summary>
        /// <param name="other">The tuple to compare with this instance.</param>
        /// <returns><see langword="true"/> if the current instance is equal to the specified tuple; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// The <paramref name="other"/> parameter is considered to be equal to the current instance if each of its fields
        /// are equal to that of the current instance, using the default comparer for that field's type.
        /// </remarks>
        [Pure]
        public bool Equals(ValueTuple<T1, T2, T3, T4, T5, T6, T7> other) =>
            EqualityComparer<T1>.Default.Equals(Item1, other.Item1) &&
            EqualityComparer<T2>.Default.Equals(Item2, other.Item2) &&
            EqualityComparer<T3>.Default.Equals(Item3, other.Item3) &&
            EqualityComparer<T4>.Default.Equals(Item4, other.Item4) &&
            EqualityComparer<T5>.Default.Equals(Item5, other.Item5) &&
            EqualityComparer<T6>.Default.Equals(Item6, other.Item6) &&
            EqualityComparer<T7>.Default.Equals(Item7, other.Item7);

        [Pure]
        int IComparable.CompareTo(object? other)
        {
            if (other == null)
                return 1;

            if (other is not ValueTuple<T1, T2, T3, T4, T5, T6, T7>)
                throw new ArgumentException();

            return CompareTo((ValueTuple<T1, T2, T3, T4, T5, T6, T7>)other);
        }

        /// <summary>Compares this instance to a specified instance and returns an indication of their relative values.</summary>
        /// <param name="other">An instance to compare.</param>
        /// <returns>
        /// A signed number indicating the relative values of this instance and <paramref name="other"/>.
        /// Returns less than zero if this instance is less than <paramref name="other"/>, zero if this
        /// instance is equal to <paramref name="other"/>, and greater than zero if this instance is greater
        /// than <paramref name="other"/>.
        /// </returns>
        [Pure]
        public int CompareTo(ValueTuple<T1, T2, T3, T4, T5, T6, T7> other)
        {
            var c = Comparer<T1>.Default.Compare(Item1, other.Item1);

            if (c != 0)
                return c;

            c = Comparer<T2>.Default.Compare(Item2, other.Item2);

            if (c != 0)
                return c;

            c = Comparer<T3>.Default.Compare(Item3, other.Item3);

            if (c != 0)
                return c;

            c = Comparer<T4>.Default.Compare(Item4, other.Item4);

            if (c != 0)
                return c;

            c = Comparer<T5>.Default.Compare(Item5, other.Item5);

            if (c != 0)
                return c;

            c = Comparer<T6>.Default.Compare(Item6, other.Item6);
            return c != 0 ? c : Comparer<T7>.Default.Compare(Item7, other.Item7);
        }

#if !NET20 && !NET30 && !NET35
        [Pure]
        bool IStructuralEquatable.Equals(object? other, IEqualityComparer comparer)
        {
            if (other is not ValueTuple<T1, T2, T3, T4, T5, T6, T7>)
                return false;

            var objTuple = (ValueTuple<T1, T2, T3, T4, T5, T6, T7>)other;

            return
                comparer.Equals(Item1, objTuple.Item1) &&
                comparer.Equals(Item2, objTuple.Item2) &&
                comparer.Equals(Item3, objTuple.Item3) &&
                comparer.Equals(Item4, objTuple.Item4) &&
                comparer.Equals(Item5, objTuple.Item5) &&
                comparer.Equals(Item6, objTuple.Item6) &&
                comparer.Equals(Item7, objTuple.Item7);
        }

        [Pure]
        int IStructuralComparable.CompareTo(object? other, IComparer comparer)
        {
            if (other == null)
                return 1;

            if (other is not ValueTuple<T1, T2, T3, T4, T5, T6, T7>)
                throw new ArgumentException();

            var objTuple = (ValueTuple<T1, T2, T3, T4, T5, T6, T7>)other;

            var c = comparer.Compare(Item1, objTuple.Item1);

            if (c != 0)
                return c;

            c = comparer.Compare(Item2, objTuple.Item2);

            if (c != 0)
                return c;

            c = comparer.Compare(Item3, objTuple.Item3);

            if (c != 0)
                return c;

            c = comparer.Compare(Item4, objTuple.Item4);

            if (c != 0)
                return c;

            c = comparer.Compare(Item5, objTuple.Item5);

            if (c != 0)
                return c;

            c = comparer.Compare(Item6, objTuple.Item6);
            return c != 0 ? c : comparer.Compare(Item7, objTuple.Item7);
        }

        int IStructuralEquatable.GetHashCode(IEqualityComparer comparer) => GetHashCodeCore(comparer);
#endif

        /// <summary>
        /// Returns the hash code for the current <see cref="ValueTuple{T1,T2,T3,T4,T5,T6,T7}"/> instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        [Pure]
        public override int GetHashCode() =>
            ValueTuple.CombineHashCodes(
                Item1?.GetHashCode() ?? 0,
                Item2?.GetHashCode() ?? 0,
                Item3?.GetHashCode() ?? 0,
                Item4?.GetHashCode() ?? 0,
                Item5?.GetHashCode() ?? 0,
                Item6?.GetHashCode() ?? 0,
                Item7?.GetHashCode() ?? 0
            );

        [Pure]
        int GetHashCodeCore(IEqualityComparer comparer) =>
            ValueTuple.CombineHashCodes(
                comparer.GetHashCode(Item1),
                comparer.GetHashCode(Item2),
                comparer.GetHashCode(Item3),
                comparer.GetHashCode(Item4),
                comparer.GetHashCode(Item5),
                comparer.GetHashCode(Item6),
                comparer.GetHashCode(Item7)
            );

        [Pure]
        int IValueTupleInternal.GetHashCode(IEqualityComparer comparer) => GetHashCodeCore(comparer);

        /// <summary>
        /// Returns a string that represents the value of this <see cref="ValueTuple{T1,T2,T3,T4,T5,T6,T7}"/> instance.
        /// </summary>
        /// <returns>The string representation of this <see cref="ValueTuple{T1,T2,T3,T4,T5,T6,T7}"/> instance.</returns>
        /// <remarks>
        /// The string returned by this method takes the form <c>(Item1, Item2, Item3, Item4, Item5, Item6, Item7)</c>.
        /// If any field value is <see langword="null"/>, it is represented as <see cref="string.Empty"/>.
        /// </remarks>
        [Pure]
        public override string ToString() => $"({Item1}, {Item2}, {Item3}, {Item4}, {Item5}, {Item6}, {Item7})";

        [Pure]
        string IValueTupleInternal.ToStringEnd() => $"{Item1}, {Item2}, {Item3}, {Item4}, {Item5}, {Item6}, {Item7})";

        /// <summary>
        /// The number of positions in this data structure.
        /// </summary>
        [Pure, ValueRange(7)]
        int ITuple.Length => 7;

        /// <summary>
        /// Get the element at position <param name="index"/>.
        /// </summary>
        [Pure]
        object? ITuple.this[[ValueRange(0, 6)] int index] =>
            index switch
            {
                0 => Item1,
                1 => Item2,
                2 => Item3,
                3 => Item4,
                4 => Item5,
                5 => Item6,
                6 => Item7,
                _ => throw new IndexOutOfRangeException(),
            };
    }

    /// <summary>
    /// Represents an 8-tuple, or octuple, as a value type.
    /// </summary>
    /// <typeparam name="T1">The type of the tuple's first component.</typeparam>
    /// <typeparam name="T2">The type of the tuple's second component.</typeparam>
    /// <typeparam name="T3">The type of the tuple's third component.</typeparam>
    /// <typeparam name="T4">The type of the tuple's fourth component.</typeparam>
    /// <typeparam name="T5">The type of the tuple's fifth component.</typeparam>
    /// <typeparam name="T6">The type of the tuple's sixth component.</typeparam>
    /// <typeparam name="T7">The type of the tuple's seventh component.</typeparam>
    /// <typeparam name="TRest">The type of the tuple's eighth component.</typeparam>
#if !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6 && !NETCOREAPP1_0 && !NETCOREAPP1_1
    [Serializable]
#endif
    [StructLayout(LayoutKind.Auto)]
    readonly struct ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> :
        IEquatable<ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>>,
#if !NET20 && !NET30 && !NET35
        IStructuralEquatable,
        IStructuralComparable,
#endif
        IComparable,
        IComparable<ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>>,
        IValueTupleInternal
        where TRest : struct
    {
        /// <summary>
        /// The current <see cref="ValueTuple{T1,T2,T3,T4,T5,T6,T7, TRest}"/> instance's first component.
        /// </summary>
        public readonly T1 Item1;

        /// <summary>
        /// The current <see cref="ValueTuple{T1,T2,T3,T4,T5,T6,T7, TRest}"/> instance's second component.
        /// </summary>
        public readonly T2 Item2;

        /// <summary>
        /// The current <see cref="ValueTuple{T1,T2,T3,T4,T5,T6,T7, TRest}"/> instance's third component.
        /// </summary>
        public readonly T3 Item3;

        /// <summary>
        /// The current <see cref="ValueTuple{T1,T2,T3,T4,T5,T6,T7, TRest}"/> instance's fourth component.
        /// </summary>
        public readonly T4 Item4;

        /// <summary>
        /// The current <see cref="ValueTuple{T1,T2,T3,T4,T5,T6,T7, TRest}"/> instance's fifth component.
        /// </summary>
        public readonly T5 Item5;

        /// <summary>
        /// The current <see cref="ValueTuple{T1,T2,T3,T4,T5,T6,T7, TRest}"/> instance's sixth component.
        /// </summary>
        public readonly T6 Item6;

        /// <summary>
        /// The current <see cref="ValueTuple{T1,T2,T3,T4,T5,T6,T7, TRest}"/> instance's seventh component.
        /// </summary>
        public readonly T7 Item7;

        /// <summary>
        /// The current <see cref="ValueTuple{T1,T2,T3,T4,T5,T6,T7, TRest}"/> instance's eighth component.
        /// </summary>
        public readonly TRest Rest;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueTuple{T1,T2,T3,T4,T5,T6,T7, TRest}"/> value type.
        /// </summary>
        /// <param name="item1">The value of the tuple's first component.</param>
        /// <param name="item2">The value of the tuple's second component.</param>
        /// <param name="item3">The value of the tuple's third component.</param>
        /// <param name="item4">The value of the tuple's fourth component.</param>
        /// <param name="item5">The value of the tuple's fifth component.</param>
        /// <param name="item6">The value of the tuple's sixth component.</param>
        /// <param name="item7">The value of the tuple's seventh component.</param>
        /// <param name="rest">The value of the tuple's eight component.</param>
        public ValueTuple(
            T1 item1,
            T2 item2,
            T3 item3,
            T4 item4,
            T5 item5,
            T6 item6,
            T7 item7,
            TRest rest
        )
        {
            if (rest is not IValueTupleInternal)
                throw new ArgumentException();

            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
            Item4 = item4;
            Item5 = item5;
            Item6 = item6;
            Item7 = item7;
            Rest = rest;
        }

        /// <summary>
        /// Returns a value that indicates whether the current <see cref="ValueTuple{T1,T2,T3,T4,T5,T6,T7, TRest}"/> instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">The object to compare with this instance.</param>
        /// <returns><see langword="true"/> if the current instance is equal to the specified object; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// The <paramref name="obj"/> parameter is considered to be equal to the current instance under the following conditions:
        /// <list type="bullet">
        ///     <item><description>It is a <see cref="ValueTuple{T1,T2,T3,T4,T5,T6,T7, TRest}"/> value type.</description></item>
        ///     <item><description>Its components are of the same types as those of the current instance.</description></item>
        ///     <item><description>Its components are equal to those of the current instance. Equality is determined by the default object equality comparer for each component.</description></item>
        /// </list>
        /// </remarks>
        [Pure]
        public override bool Equals([NotNullWhen(true)] object? obj) =>
            obj is ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> tuple && Equals(tuple);

        /// <summary>
        /// Returns a value that indicates whether the current <see cref="ValueTuple{T1,T2,T3,T4,T5,T6,T7, TRest}"/>
        /// instance is equal to a specified <see cref="ValueTuple{T1,T2,T3,T4,T5,T6,T7, TRest}"/>.
        /// </summary>
        /// <param name="other">The tuple to compare with this instance.</param>
        /// <returns><see langword="true"/> if the current instance is equal to the specified tuple; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// The <paramref name="other"/> parameter is considered to be equal to the current instance if each of its fields
        /// are equal to that of the current instance, using the default comparer for that field's type.
        /// </remarks>
        [Pure]
        public bool Equals(ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> other) =>
            EqualityComparer<T1>.Default.Equals(Item1, other.Item1) &&
            EqualityComparer<T2>.Default.Equals(Item2, other.Item2) &&
            EqualityComparer<T3>.Default.Equals(Item3, other.Item3) &&
            EqualityComparer<T4>.Default.Equals(Item4, other.Item4) &&
            EqualityComparer<T5>.Default.Equals(Item5, other.Item5) &&
            EqualityComparer<T6>.Default.Equals(Item6, other.Item6) &&
            EqualityComparer<T7>.Default.Equals(Item7, other.Item7) &&
            EqualityComparer<TRest>.Default.Equals(Rest, other.Rest);

        [Pure]
        int IComparable.CompareTo(object? other)
        {
            if (other == null)
                return 1;

            if (other is not ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> tuple)
                throw new ArgumentException();

            return CompareTo(tuple);
        }

        /// <summary>Compares this instance to a specified instance and returns an indication of their relative values.</summary>
        /// <param name="other">An instance to compare.</param>
        /// <returns>
        /// A signed number indicating the relative values of this instance and <paramref name="other"/>.
        /// Returns less than zero if this instance is less than <paramref name="other"/>, zero if this
        /// instance is equal to <paramref name="other"/>, and greater than zero if this instance is greater
        /// than <paramref name="other"/>.
        /// </returns>
        [Pure]
        public int CompareTo(ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> other)
        {
            var c = Comparer<T1>.Default.Compare(Item1, other.Item1);

            if (c != 0)
                return c;

            c = Comparer<T2>.Default.Compare(Item2, other.Item2);

            if (c != 0)
                return c;

            c = Comparer<T3>.Default.Compare(Item3, other.Item3);

            if (c != 0)
                return c;

            c = Comparer<T4>.Default.Compare(Item4, other.Item4);

            if (c != 0)
                return c;

            c = Comparer<T5>.Default.Compare(Item5, other.Item5);

            if (c != 0)
                return c;

            c = Comparer<T6>.Default.Compare(Item6, other.Item6);

            if (c != 0)
                return c;

            c = Comparer<T7>.Default.Compare(Item7, other.Item7);
            return c != 0 ? c : Comparer<TRest>.Default.Compare(Rest, other.Rest);
        }

#if !NET20 && !NET30 && !NET35
        [Pure]
        bool IStructuralEquatable.Equals(object? other, IEqualityComparer comparer) =>
            other is ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> objTuple &&
            comparer.Equals(Item1, objTuple.Item1) &&
            comparer.Equals(Item2, objTuple.Item2) &&
            comparer.Equals(Item3, objTuple.Item3) &&
            comparer.Equals(Item4, objTuple.Item4) &&
            comparer.Equals(Item5, objTuple.Item5) &&
            comparer.Equals(Item6, objTuple.Item6) &&
            comparer.Equals(Item7, objTuple.Item7) &&
            comparer.Equals(Rest, objTuple.Rest);

        [Pure]
        int IStructuralComparable.CompareTo(object? other, IComparer comparer)
        {
            if (other == null)
                return 1;

            if (other is not ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> objTuple)
                throw new ArgumentException();

            var c = comparer.Compare(Item1, objTuple.Item1);

            if (c != 0)
                return c;

            c = comparer.Compare(Item2, objTuple.Item2);

            if (c != 0)
                return c;

            c = comparer.Compare(Item3, objTuple.Item3);

            if (c != 0)
                return c;

            c = comparer.Compare(Item4, objTuple.Item4);

            if (c != 0)
                return c;

            c = comparer.Compare(Item5, objTuple.Item5);

            if (c != 0)
                return c;

            c = comparer.Compare(Item6, objTuple.Item6);

            if (c != 0)
                return c;

            c = comparer.Compare(Item7, objTuple.Item7);
            return c != 0 ? c : comparer.Compare(Rest, objTuple.Rest);
        }

        [Pure]
        int IStructuralEquatable.GetHashCode(IEqualityComparer comparer) => GetHashCodeCore(comparer);
#endif

        /// <summary>
        /// Returns the hash code for the current <see cref="ValueTuple{T1,T2,T3,T4,T5,T6,T7, TRest}"/> instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        [Pure]
        public override int GetHashCode()
        {
            // We want to have a limited hash in this case.  We'll use the last 8 elements of the tuple
            if (Rest is not IValueTupleInternal rest)
                return ValueTuple.CombineHashCodes(
                    Item1?.GetHashCode() ?? 0,
                    Item2?.GetHashCode() ?? 0,
                    Item3?.GetHashCode() ?? 0,
                    Item4?.GetHashCode() ?? 0,
                    Item5?.GetHashCode() ?? 0,
                    Item6?.GetHashCode() ?? 0,
                    Item7?.GetHashCode() ?? 0
                );

            var size = rest.Length;

            if (size >= 8)
                return rest.GetHashCode();

            // In this case, the rest member has less than 8 elements so we need to combine some our elements with the elements in rest
            var k = 8 - size;

            return k switch
            {
                1 => ValueTuple.CombineHashCodes(
                                        Item7?.GetHashCode() ?? 0,
                                        rest.GetHashCode()
                                    ),
                2 => ValueTuple.CombineHashCodes(
                                        Item6?.GetHashCode() ?? 0,
                                        Item7?.GetHashCode() ?? 0,
                                        rest.GetHashCode()
                                    ),
                3 => ValueTuple.CombineHashCodes(
                                        Item5?.GetHashCode() ?? 0,
                                        Item6?.GetHashCode() ?? 0,
                                        Item7?.GetHashCode() ?? 0,
                                        rest.GetHashCode()
                                    ),
                4 => ValueTuple.CombineHashCodes(
                                        Item4?.GetHashCode() ?? 0,
                                        Item5?.GetHashCode() ?? 0,
                                        Item6?.GetHashCode() ?? 0,
                                        Item7?.GetHashCode() ?? 0,
                                        rest.GetHashCode()
                                    ),
                5 => ValueTuple.CombineHashCodes(
                                        Item3?.GetHashCode() ?? 0,
                                        Item4?.GetHashCode() ?? 0,
                                        Item5?.GetHashCode() ?? 0,
                                        Item6?.GetHashCode() ?? 0,
                                        Item7?.GetHashCode() ?? 0,
                                        rest.GetHashCode()
                                    ),
                6 => ValueTuple.CombineHashCodes(
                                        Item2?.GetHashCode() ?? 0,
                                        Item3?.GetHashCode() ?? 0,
                                        Item4?.GetHashCode() ?? 0,
                                        Item5?.GetHashCode() ?? 0,
                                        Item6?.GetHashCode() ?? 0,
                                        Item7?.GetHashCode() ?? 0,
                                        rest.GetHashCode()
                                    ),
                7 or 8 => ValueTuple.CombineHashCodes(
                                        Item1?.GetHashCode() ?? 0,
                                        Item2?.GetHashCode() ?? 0,
                                        Item3?.GetHashCode() ?? 0,
                                        Item4?.GetHashCode() ?? 0,
                                        Item5?.GetHashCode() ?? 0,
                                        Item6?.GetHashCode() ?? 0,
                                        Item7?.GetHashCode() ?? 0,
                                        rest.GetHashCode()
                                    ),
                _ => throw new InvalidOperationException("Missed all cases for computing ValueTuple hash code"),
            };
        }

        [Pure]
        int GetHashCodeCore(IEqualityComparer comparer)
        {
            // We want to have a limited hash in this case.  We'll use the last 8 elements of the tuple
            if (Rest is not IValueTupleInternal rest)
                return ValueTuple.CombineHashCodes(
                    comparer.GetHashCode(Item1),
                    comparer.GetHashCode(Item2),
                    comparer.GetHashCode(Item3),
                    comparer.GetHashCode(Item4),
                    comparer.GetHashCode(Item5),
                    comparer.GetHashCode(Item6),
                    comparer.GetHashCode(Item7)
                );

            var size = rest.Length;

            if (size >= 8)
                return rest.GetHashCode(comparer);

            // In this case, the rest member has less than 8 elements so we need to combine some our elements with the elements in rest
            var k = 8 - size;

            return k switch
            {
                1 => ValueTuple.CombineHashCodes(
                    comparer.GetHashCode(Item7),
                    rest.GetHashCode(comparer)
                ),
                2 => ValueTuple.CombineHashCodes(
                    comparer.GetHashCode(Item6),
                    comparer.GetHashCode(Item7),
                    rest.GetHashCode(comparer)
                ),
                3 => ValueTuple.CombineHashCodes(
                    comparer.GetHashCode(Item5),
                    comparer.GetHashCode(Item6),
                    comparer.GetHashCode(Item7),
                    rest.GetHashCode(comparer)
                ),
                4 => ValueTuple.CombineHashCodes(
                    comparer.GetHashCode(Item4),
                    comparer.GetHashCode(Item5),
                    comparer.GetHashCode(Item6),
                    comparer.GetHashCode(Item7),
                    rest.GetHashCode(comparer)
                ),
                5 => ValueTuple.CombineHashCodes(
                    comparer.GetHashCode(Item3),
                    comparer.GetHashCode(Item4),
                    comparer.GetHashCode(Item5),
                    comparer.GetHashCode(Item6),
                    comparer.GetHashCode(Item7),
                    rest.GetHashCode(comparer)
                ),
                6 => ValueTuple.CombineHashCodes(
                    comparer.GetHashCode(Item2),
                    comparer.GetHashCode(Item3),
                    comparer.GetHashCode(Item4),
                    comparer.GetHashCode(Item5),
                    comparer.GetHashCode(Item6),
                    comparer.GetHashCode(Item7),
                    rest.GetHashCode(comparer)
                ),
                7 or 8 => ValueTuple.CombineHashCodes(
                    comparer.GetHashCode(Item1),
                    comparer.GetHashCode(Item2),
                    comparer.GetHashCode(Item3),
                    comparer.GetHashCode(Item4),
                    comparer.GetHashCode(Item5),
                    comparer.GetHashCode(Item6),
                    comparer.GetHashCode(Item7),
                    rest.GetHashCode(comparer)
                ),
                _ => throw new InvalidOperationException("Missed all cases for computing ValueTuple hash code"),
            };
        }

        [Pure]
        int IValueTupleInternal.GetHashCode(IEqualityComparer comparer) => GetHashCodeCore(comparer);

        /// <summary>
        /// Returns a string that represents the value of this <see cref="ValueTuple{T1,T2,T3,T4,T5,T6,T7, TRest}"/> instance.
        /// </summary>
        /// <returns>The string representation of this <see cref="ValueTuple{T1,T2,T3,T4,T5,T6,T7, TRest}"/> instance.</returns>
        /// <remarks>
        /// The string returned by this method takes the form <c>(Item1, Item2, Item3, Item4, Item5, Item6, Item7, Rest)</c>.
        /// If any field value is <see langword="null"/>, it is represented as <see cref="string.Empty"/>.
        /// </remarks>
        [Pure]
        public override string ToString() =>
            Rest is not IValueTupleInternal rest
                ? $"({Item1}, {Item2}, {Item3}, {Item4}, {Item5}, {Item6}, {Item7}, {Rest})"
                : $"({Item1}, {Item2}, {Item3}, {Item4}, {Item5}, {Item6}, {Item7}, {rest.ToStringEnd()}";

        [Pure]
        string IValueTupleInternal.ToStringEnd() =>
            Rest is not IValueTupleInternal rest
                ? $"{Item1}, {Item2}, {Item3}, {Item4}, {Item5}, {Item6}, {Item7}, {Rest})"
                : $"{Item1}, {Item2}, {Item3}, {Item4}, {Item5}, {Item6}, {Item7}, {rest.ToStringEnd()}";

        /// <summary>
        /// The number of positions in this data structure.
        /// </summary>
        [Pure, NonNegativeValue]
        int ITuple.Length => Rest is not IValueTupleInternal rest ? 8 : 7 + rest.Length;

        /// <summary>
        /// Get the element at position <param name="index"/>.
        /// </summary>
        [Pure]
        object? ITuple.this[[NonNegativeValue] int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return Item1;
                    case 1: return Item2;
                    case 2: return Item3;
                    case 3: return Item4;
                    case 4: return Item5;
                    case 5: return Item6;
                    case 6: return Item7;
                }

                if (Rest is IValueTupleInternal rest)
                    return rest[index - 7];

                if (index == 7)
                    return Rest;

                throw new IndexOutOfRangeException();
            }
        }
    }

    /// <summary>Allows a <see cref="KeyValuePair{TKey, TValue}"/> to be deconstructed, much like a tuple.</summary>
#pragma warning disable
    static class KeyValuePairDeconstructors
#pragma warning restore
    {
        /// <summary>Deconstructs a <see cref="KeyValuePair{TKey, TValue}"/> into its components.</summary>
        /// <typeparam name="TKey">The key generic in the <see cref="KeyValuePair{TKey, TValue}"/>.</typeparam>
        /// <typeparam name="TValue">The value generic in the <see cref="KeyValuePair{TKey, TValue}"/>.</typeparam>
        /// <param name="kvp">The key value pair to deconstruct.</param>
        /// <param name="key">The key value to get assigned as the key value pair's key.</param>
        /// <param name="value">The key value to get assigned as the key value pair's value.</param>
        internal static void Deconstruct<TKey, TValue>(
            this KeyValuePair<TKey, TValue> kvp,
            out TKey key,
            out TValue value
        )
        {
            key = kvp.Key;
            value = kvp.Value;
        }
    }
}
#endif
