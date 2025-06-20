// SPDX-License-Identifier: MPL-2.0
// ReSharper disable CheckNamespace EmptyNamespace InvalidXmlDocComment RedundantCallerArgumentExpressionDefaultValue RedundantNameQualifier SuggestBaseTypeForParameter UseSymbolAlias
namespace Emik.Morsels;
#if NET35_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
/// <summary>Contains methods for deconstructing objects.</summary>
#pragma warning disable CS9107
static partial class DeconstructionCollectionExtensions
{
    [return: NotNullIfNotNull(nameof(it))]
    public static T Debug<T>(
        this T it,
        Predicate<T>? filter = null,
        Converter<T, object?>? map = null,
        int visitLength = DeconstructionCollection.DefaultVisitLength,
        int stringLength = DeconstructionCollection.DefaultStringLength,
        int recurseLength = DeconstructionCollection.DefaultRecurseLength,
        [CallerArgumentExpression(nameof(it))] string? expression = null,
        [CallerFilePath] string? path = null,
        [CallerMemberName] string? name = null,
        [CallerLineNumber] int line = 0
    )
    {
        if (filter?.Invoke(it) is false)
            return it;

        var text = $"[{DateTime.Now:HH:mm:ss}] [{path.FileName()}.{name}:{line} ({expression.CollapseToSingleLine()})] {
            (map is null ? it : map(it)).ToDeconstructed(visitLength, stringLength, recurseLength)}\n";
#if KTANE
        UnityEngine.Debug.Log(text);
#else
        Console.WriteLine(text);
#endif
        File.AppendAllText(Path.Combine(Path.GetTempPath(), "morsels.log"), text);
        return it;
    }

    /// <summary>Takes the complex object and turns it into a structure that is serializable.</summary>
    /// <param name="value">The complex object to convert.</param>
    /// <param name="visitLength">The maximum number of times to recurse through an enumeration.</param>
    /// <param name="stringLength">The maximum length of any given <see cref="string"/>.</param>
    /// <param name="recurseLength">The maximum number of times to recurse a nested object or dictionary.</param>
    /// <returns>
    /// The serializable object: any of <see cref="IntPtr"/>, <see cref="UIntPtr"/>,
    /// <see cref="ISerializable"/>, or <see cref="DeconstructionCollection"/>.
    /// </returns>
    [Pure]
    [return: NotNullIfNotNull(nameof(value))]
    public static object? ToDeconstructed(
        this object? value,
        int visitLength = DeconstructionCollection.DefaultVisitLength,
        int stringLength = DeconstructionCollection.DefaultStringLength,
        int recurseLength = DeconstructionCollection.DefaultRecurseLength
    )
    {
        if (value is DeconstructionCollection)
            return value;

        visitLength = visitLength >= 0 ? visitLength : int.MaxValue;
        stringLength = stringLength >= 0 ? stringLength : int.MaxValue;
        recurseLength = recurseLength >= 0 ? recurseLength : int.MaxValue;
        HashSet<object?> seen = new(DeconstructionCollection.Comparer) { value };
        var assertion = false;
        var next = DeconstructionCollection.CollectNext(value, stringLength, ref visitLength, ref assertion, seen);

        if (next is not DeconstructionCollection x)
        {
            System.Diagnostics.Debug.Assert(!assertion, "!assertion");
            return DeconstructionCollection.TryTruncate(next, stringLength, out var output) ? output : next;
        }

        System.Diagnostics.Debug.Assert(assertion, "assertion");

        for (var i = 0; recurseLength > 0 && i < recurseLength && x.TryRecurse(i, ref visitLength, seen); i++) { }

        return x.Simplify();
    }
}

/// <summary>Defines the collection responsible for deconstructing.</summary>
/// <param name="str">The maximum length of any given <see cref="string"/>.</param>
abstract partial class DeconstructionCollection([NonNegativeValue] int str) : ICollection
{
    /// <summary>Represents a comparer for <see cref="DeconstructionCollection"/> recursion checks.</summary>
    /// <remarks><para>
    /// All values considered to be scalar values are treated as being always unique even when the exact
    /// reference is the same. The point of the comparer is to avoid reference cycles, not for equality.
    /// </para></remarks>
    sealed partial class DeconstructionComparer : IEqualityComparer<object?>
    {
        int _unique = int.MaxValue;

        /// <inheritdoc />
        [Pure] // ReSharper disable once MemberHidesStaticFromOuterClass
        public new bool Equals(object? x, object? y) => !IsScalar(x) && !IsScalar(y) && x == y;

        /// <inheritdoc />
        [Pure]
        public int GetHashCode(object? obj) =>
            IsScalar(obj)
                ? unchecked(_unique--)
#if NETFRAMEWORK && !NET35_OR_GREATER
                : RuntimeHelpers.GetHashCode(obj);
#else // RuntimeHelpers.GetHashCode eventually calls an external function that I have no idea how to replicate.
                : 0;
#endif
        /// <summary>Determines whether the value is a scalar.</summary>
        /// <param name="value">The value to check.</param>
        /// <returns>
        /// The value <see langword="true"/> if the value is a scalar; otherwise, <see langword="false"/>.
        /// </returns>
        [Pure]
        static bool IsScalar([NotNullWhen(false)] object? value) =>
            value is nint or nuint or null or string or IConvertible or Pointer or Type or Version;
    }

    /// <summary>Represents a deep-cloned list.</summary>
    /// <param name="str">The maximum length of any given <see cref="string"/>.</param>
    sealed partial class DeconstructionList([NonNegativeValue] int str) : DeconstructionCollection(str), IList
    {
        readonly List<object?> _list = [];

        /// <inheritdoc />
        [Pure]
        public override IList Inner => _list;

        /// <inheritdoc />
        [Pure]
        public object? this[int index]
        {
            get => ((IList)_list)[index];
            set => ((IList)_list)[index] = value;
        }

        /// <inheritdoc />
        [Pure]
        bool IList.IsFixedSize => false;

        /// <inheritdoc />
        [Pure]
        bool IList.IsReadOnly => false;

        /// <summary>
        /// Implicitly converts the parameter by creating the new instance of
        /// <see cref="DeconstructionCollection.DeconstructionList"/> by using the constructor
        /// <see cref="DeconstructionCollection.DeconstructionList(int)"/>.
        /// </summary>
        /// <param name="str">The parameter to pass onto the constructor.</param>
        /// <returns>
        /// The new instance of <see cref="DeconstructionCollection.DeconstructionList"/>
        /// by passing the parameter <paramref name="str"/> to the constructor
        /// <see cref="DeconstructionCollection.DeconstructionList(int)"/>.
        /// </returns>
        [Pure]
        public static implicit operator DeconstructionList(int str) => new(str);

        /// <summary>Attempts to deconstruct an object by enumerating it.</summary>
        /// <param name="enumerator">The enumerator to collect. It will be disposed after the method halts.</param>
        /// <param name="str">The maximum length of any given <see cref="string"/>.</param>
        /// <param name="visit">The maximum number of times to recurse.</param>
        /// <param name="list">The resulting <see cref="DeconstructionCollection.DeconstructionList"/>.</param>
        /// <param name="seen">The set of seen values, which is used to avoid recursion.</param>
        /// <returns>
        /// Whether the parameter <paramref name="enumerator"/> was deconstructed fully and <paramref name="visit"/>
        /// altered. When this method returns <see langword="false"/>, the parameter <paramref name="list"/>
        /// will still contain the elements that were deconstructed, alongside an ellipsis.
        /// </returns>
        public static bool TryCollect(
            [HandlesResourceDisposal] IEnumerator enumerator,
            [NonNegativeValue] int str,
            ref int visit,
            out DeconstructionList list,
            HashSet<object?>? seen = null
        )
        {
            using var _ = enumerator as IDisposable;
            var copy = visit;
            list = new(str);

            try
            {
                while (enumerator.MoveNext())
                    if (seen?.Add(enumerator.Current) is false) { }
                    else if (--copy > 0)
                        list.Add(enumerator.Current);
                    else if (!enumerator.MoveNext())
                        break;
                    else
                        return list.Fail();
            }
            catch (Exception)
            {
                return list.Fail();
            }

            visit = copy;
            return true;
        }

        /// <summary>Attempts to deconstruct an object by enumerating it.</summary>
        /// <param name="enumerable">The enumerator to collect.</param>
        /// <param name="str">The maximum length of any given <see cref="string"/>.</param>
        /// <param name="visit">The maximum number of times to recurse.</param>
        /// <param name="list">The resulting <see cref="DeconstructionCollection.DeconstructionList"/>.</param>
        /// <param name="seen">The set of seen values, which is used to avoid recursion.</param>
        /// <returns>
        /// Whether the parameter <paramref name="enumerable"/> was deconstructed fully and <paramref name="visit"/>
        /// altered. When this method returns <see langword="false"/>, the parameter <paramref name="list"/>
        /// will still contain the elements that were deconstructed, alongside an ellipsis.
        /// </returns>
        public static bool TryCollect(
            [InstantHandle] IEnumerable enumerable,
            [NonNegativeValue] int str,
            ref int visit,
            out DeconstructionList list,
            HashSet<object?>? seen = null
        )
        {
            IEnumerator e;

            try
            {
                e = enumerable.GetEnumerator();
            }
            catch (Exception)
            {
                list = new(str);
                return list.Fail();
            }

            return TryCollect(e, str, ref visit, out list, seen);
        }
#if !NET20 && !NET30 && !NET35
        /// <summary>Attempts to deconstruct an object by enumerating it.</summary>
        /// <param name="comparable">The comparable to collect.</param>
        /// <param name="str">The maximum length of any given <see cref="string"/>.</param>
        /// <param name="visit">The maximum number of times to recurse.</param>
        /// <param name="list">The resulting <see cref="DeconstructionCollection.DeconstructionList"/>.</param>
        /// <param name="seen">The set of seen values, which is used to avoid recursion.</param>
        /// <returns>
        /// Whether the parameter <paramref name="comparable"/> was deconstructed fully and <paramref name="visit"/>
        /// altered. When this method returns <see langword="false"/>, the parameter <paramref name="list"/>
        /// will still contain the elements that were deconstructed, alongside an ellipsis.
        /// </returns>
        public static bool TryCollect(
            [InstantHandle] IStructuralComparable comparable,
            [NonNegativeValue] int str,
            ref int visit,
            out DeconstructionList list,
            HashSet<object?>? seen = null
        )
        {
            List<object?> e;

            try
            {
                e = comparable.ToList();
            }
            catch (Exception)
            {
                list = new(str);
                return list.Fail();
            }

            return TryCollect(e, str, ref visit, out list, seen);
        }

        /// <summary>Attempts to deconstruct an object by enumerating it.</summary>
        /// <param name="equatable">The equatable to collect.</param>
        /// <param name="str">The maximum length of any given <see cref="string"/>.</param>
        /// <param name="visit">The maximum number of times to recurse.</param>
        /// <param name="list">The resulting <see cref="DeconstructionCollection.DeconstructionList"/>.</param>
        /// <param name="seen">The set of seen values, which is used to avoid recursion.</param>
        /// <returns>
        /// Whether the parameter <paramref name="equatable"/> was deconstructed fully and <paramref name="visit"/>
        /// altered. When this method returns <see langword="false"/>, the parameter <paramref name="list"/>
        /// will still contain the elements that were deconstructed, alongside an ellipsis.
        /// </returns>
        public static bool TryCollect(
            [InstantHandle] IStructuralEquatable equatable,
            [NonNegativeValue] int str,
            ref int visit,
            out DeconstructionList list,
            HashSet<object?>? seen = null
        )
        {
            List<object?> e;

            try
            {
                e = equatable.ToList();
            }
            catch (Exception)
            {
                list = new(str);
                return list.Fail();
            }

            return TryCollect(e, str, ref visit, out list, seen);
        }
#endif
        public override bool Fail()
        {
            // We append a character instead of a string because otherwise it would be surrounded by quotes.
            Add('…');
            return false;
        }

        /// <inheritdoc />
        public override bool TryRecurse(int layer, ref int visit, HashSet<object?>? seen = null)
        {
            if (layer < 0)
                return false;

            var any = false;

            if (layer is 0)
                for (var i = 0; i < Count; i++)
                    _list[i] = CollectNext(_list[i], str, ref visit, ref any, seen);
            else
                foreach (var next in _list)
                    RecurseNext(next, layer, ref visit, ref any, seen);

            return any;
        }

        /// <inheritdoc />
        [NonNegativeValue]
        public int Add(object? value) => ((IList)_list).Add(value);

        /// <inheritdoc />
        [Pure]
        public override string ToString() => $"[{_list.AsEnumerable().Select(ToString).Conjoin()}]";

        /// <inheritdoc />
        public override DeconstructionCollection Simplify()
        {
            for (var i = 0; i < Count; i++)
                _list[i] = SimplifyObject(_list[i]);

            return this;
        }

        /// <inheritdoc />
        void IList.Clear() => _list.Clear();

        /// <inheritdoc />
        void IList.Insert(int index, object? value) => _list.Insert(index, value);

        /// <inheritdoc />
        void IList.Remove(object? value) => _list.Remove(value);

        /// <inheritdoc />
        void IList.RemoveAt(int index) => _list.RemoveAt(index);

        /// <inheritdoc />
        [Pure]
        bool IList.Contains(object? value) => _list.Contains(value);

        /// <inheritdoc />
        [Pure]
        int IList.IndexOf(object? value) => _list.IndexOf(value);

        /// <inheritdoc />
        [Pure]
        public override IEnumerator GetEnumerator() => _list.GetEnumerator();
    }

    /// <summary>Represents either a complex object or a deep-cloned dictionary.</summary>
    /// <param name="str">The maximum length of any given <see cref="string"/>.</param>
    sealed partial class DeconstructionDictionary([NonNegativeValue] int str)
        : DeconstructionCollection(str), IDictionary
    {
        /// <summary>Handles enumeration of the <see cref="DeconstructionDictionary"/>.</summary>
        /// <param name="dictionary">The <see cref="DeconstructionDictionary"/> to enumerate.</param>
        sealed class Enumerator(DeconstructionDictionary dictionary) : IDictionaryEnumerator
        {
            int _index = -1;

            /// <inheritdoc />
            [Pure]
            public DictionaryEntry Entry =>
                _index >= 0 && _index < dictionary.Count ? dictionary._list[_index] : default;

            /// <inheritdoc />
            [Pure]
            object IEnumerator.Current => Entry;

            /// <inheritdoc />
            [Pure]
            object IDictionaryEnumerator.Key => Entry.Key;

            /// <inheritdoc />
            [Pure]
            object? IDictionaryEnumerator.Value => Entry.Value;

            /// <inheritdoc />
            bool IEnumerator.MoveNext() => ++_index < dictionary.Count;

            /// <inheritdoc />
            void IEnumerator.Reset() => _index = -1;
        }

        readonly List<DictionaryEntry> _list = [];

        /// <inheritdoc />
        [Pure]
        object? IDictionary.this[object key]
        {
            get => _list.Find(Eq(key)).Value;
            set => _ = _list.FindIndex(Eq(key)) is not -1 and var i ? _list[i] = new(key, value) : default;
        }

        /// <inheritdoc />
        [Pure]
        bool IDictionary.IsFixedSize => false;

        /// <inheritdoc />
        [Pure]
        bool IDictionary.IsReadOnly => false;

        /// <inheritdoc />
        [Pure]
        ICollection IDictionary.Keys => _list.ConvertAll(x => x.Key);

        /// <inheritdoc />
        [Pure]
        ICollection IDictionary.Values => _list.ConvertAll(x => x.Value);

        /// <inheritdoc />
        [Pure]
        public override IList Inner => _list;

        /// <inheritdoc />
        [Pure]
        public override ICollection Serialized =>
            _list.Aggregate(new Dictionary<string, object?>(StringComparer.Ordinal), AddUnique);

        /// <summary>
        /// Implicitly converts the parameter by creating the new instance of
        /// <see cref="DeconstructionCollection.DeconstructionDictionary"/>
        /// by using the constructor <see cref="DeconstructionCollection.DeconstructionDictionary(int)"/>.
        /// </summary>
        /// <param name="str">The parameter to pass onto the constructor.</param>
        /// <returns>
        /// The new instance of <see cref="DeconstructionCollection.DeconstructionDictionary"/>
        /// by passing the parameter <paramref name="str"/> to the constructor
        /// <see cref="DeconstructionCollection.DeconstructionDictionary(int)"/>.
        /// </returns>
        [Pure]
        public static implicit operator DeconstructionDictionary(int str) => new(str);

        /// <summary>Attempts to deconstruct an object by enumerating it.</summary>
        /// <param name="enumerator">The enumerator to collect. It will be disposed after the method halts.</param>
        /// <param name="str">The maximum length of any given <see cref="string"/>.</param>
        /// <param name="visit">The maximum number of times to recurse.</param>
        /// <param name="dictionary">
        /// The resulting <see cref="DeconstructionCollection.DeconstructionDictionary"/>.
        /// </param>
        /// <param name="seen">The set of seen values, which is used to avoid recursion.</param>
        /// <returns>
        /// Whether the parameter <paramref name="enumerator"/> was deconstructed fully and <paramref name="visit"/>
        /// altered. When this method returns <see langword="false"/>, the parameter <paramref name="dictionary"/>
        /// will still contain the elements that were deconstructed, alongside an ellipsis.
        /// </returns>
        public static bool TryCollect(
            [HandlesResourceDisposal] IDictionaryEnumerator enumerator,
            [NonNegativeValue] int str,
            ref int visit,
            out DeconstructionDictionary dictionary,
            HashSet<object?>? seen = null
        )
        {
            using var _ = enumerator as IDisposable;
            var copy = visit;
            dictionary = new(str);

            try
            {
                while (enumerator.MoveNext())
                    if (seen?.Contains(enumerator.Key) is true ||
                        seen?.Add(enumerator.Value) is false ||
                        seen?.Add(enumerator.Key) is false) { }
                    else if (--copy > 0)
                        dictionary.Add(enumerator.Key, enumerator.Value);
                    else if (enumerator.MoveNext())
                        return dictionary.Fail();
                    else
                        break;
            }
            catch (Exception)
            {
                return dictionary.Fail();
            }

            visit = copy;
            return true;
        }

        /// <summary>Attempts to deconstruct an object by enumerating it.</summary>
        /// <param name="dict">The dictionary to collect.</param>
        /// <param name="str">The maximum length of any given <see cref="string"/>.</param>
        /// <param name="visit">The maximum number of times to recurse.</param>
        /// <param name="dictionary">
        /// The resulting <see cref="DeconstructionCollection.DeconstructionDictionary"/>.
        /// </param>
        /// <param name="seen">The set of seen values, which is used to avoid recursion.</param>
        /// <returns>
        /// Whether the parameter <paramref name="dict"/> was deconstructed fully and <paramref name="visit"/>
        /// altered. When this method returns <see langword="false"/>, the parameter <paramref name="dictionary"/>
        /// will still contain the elements that were deconstructed, alongside an ellipsis.
        /// </returns>
        public static bool TryCollect(
            IDictionary dict,
            [NonNegativeValue] int str,
            ref int visit,
            out DeconstructionDictionary dictionary,
            HashSet<object?>? seen = null
        )
        {
            IDictionaryEnumerator e;

            try
            {
                e = dict.GetEnumerator();
            }
            catch (Exception)
            {
                dictionary = new(str);
                return dictionary.Fail();
            }

            return TryCollect(e, str, ref visit, out dictionary, seen);
        }

        /// <summary>Attempts to deconstruct an object by reflectively collecting its fields and properties.</summary>
        /// <param name="value">The complex object to convert.</param>
        /// <param name="str">The maximum length of any given <see cref="string"/>.</param>
        /// <param name="visit">The maximum number of times to recurse.</param>
        /// <param name="dictionary">
        /// The resulting <see cref="DeconstructionCollection.DeconstructionDictionary"/>.
        /// </param>
        /// <param name="seen">The set of seen values, which is used to avoid recursion.</param>
        /// <returns>
        /// Whether the parameter <paramref name="value"/> was deconstructed fully and <paramref name="visit"/>
        /// altered. When this method returns <see langword="false"/>, the parameter <paramref name="dictionary"/>
        /// will still contain the elements that were deconstructed, alongside an ellipsis.
        /// </returns>
        // ReSharper disable once CognitiveComplexity
        public static bool TryReflectivelyCollect(
            object value,
            [NonNegativeValue] int str,
            ref int visit,
            out DeconstructionDictionary dictionary,
            HashSet<object?>? seen = null
        )
        {
            var copy = visit;
            dictionary = new(str);
            var type = value.GetType();
#if !NETFRAMEWORK || NET45_OR_GREATER
            var fields = type.GetRuntimeFields().ToArray();
            var properties = type.GetRuntimeProperties().ToArray();
#else
            const BindingFlags Bindings = BindingFlags.Instance |
                BindingFlags.Static |
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.FlattenHierarchy;

            var fields = type.GetFields(Bindings);
            var properties = type.GetProperties(Bindings);
#endif
            // ReSharper disable once LoopCanBePartlyConvertedToQuery
            foreach (var next in fields)
            {
                if (next.IsStatic)
                    continue;
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
                if (next.FieldType.IsByRefLike)
                    continue;
#endif
                if (next.GetValue(value) is var result && seen?.Add(result) is false)
                    continue;

                if (--copy <= 0)
                    return dictionary.Fail();

                var name = Name(next, fields, properties);
                dictionary.Add(name, result);
            }

            // ReSharper disable once LoopCanBePartlyConvertedToQuery
            foreach (var next in properties)
            {
                if (next.GetGetMethod() is { } getter &&
                    (getter.IsStatic || next.GetGetMethod()?.GetParameters() is not []))
                    continue;
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
                if (next.PropertyType.IsByRefLike)
                    continue;
#endif
                if (GetValueOrException(value, next, str, ref visit, seen) is var result && seen?.Add(result) is false)
                    continue;

                if (--copy <= 0)
                    return dictionary.Fail();

                var name = Name(next, fields, properties);
                dictionary.Add(name, result);
            }

            visit = copy;
            return true;
        }

        /// <inheritdoc cref="IDictionary.Add"/>
        // ReSharper disable once NullableWarningSuppressionIsUsed
        public void Add(object? key, object? value) => _list.Add(new(key!, value));

        /// <inheritdoc cref="IDictionary.Clear"/>
        public void Clear() => _list.Clear();

        /// <inheritdoc />
        public override bool Fail()
        {
            // We append a character instead of a string because otherwise it would be surrounded by quotes.
            Add('…', '…');
            return false;
        }

        /// <inheritdoc />
        public override bool TryRecurse(int layer, ref int visit, HashSet<object?>? seen = null)
        {
            if (layer < 0)
                return false;

            var any = false;

            if (layer is 0)
                for (var i = 0; i < Count; i++) // ReSharper disable once NullableWarningSuppressionIsUsed
                    _list[i] = new(
                        CollectNext(_list[i].Key, str, ref visit, ref any, seen)!,
                        CollectNext(_list[i].Value, str, ref visit, ref any, seen)
                    );
            else
                foreach (var next in _list)
                {
                    RecurseNext(next.Key, layer, ref visit, ref any, seen);
                    RecurseNext(next.Value, layer, ref visit, ref any, seen);
                }

            return any;
        }

        /// <inheritdoc />
        [Pure]
        public override string ToString() =>
            _list is []
                ? "{ }"
                : $"{{ {_list.AsEnumerable().Select(x => $"{ToString(x.Key)}: {ToString(x.Value)}").Conjoin()} }}";

        /// <inheritdoc />
        public override DeconstructionCollection Simplify()
        {
            for (var i = 0; i < Count; i++) // ReSharper disable once AssignNullToNotNullAttribute
                _list[i] = new(SimplifyObject(_list[i].Key), SimplifyObject(_list[i].Value));

            return this;
        }

        /// <inheritdoc />
        [MustUseReturnValue]
        public override IEnumerator GetEnumerator() => ((IDictionary)this).GetEnumerator();

        /// <inheritdoc />
        // ReSharper disable once UsageOfDefaultStructEquality
        void IDictionary.Remove(object key) => _list.Remove(_list.Find(Eq(key)));

        /// <inheritdoc />
        [Pure]
        bool IDictionary.Contains(object key) => _list.FindIndex(Eq(key)) is not -1;

        /// <inheritdoc />
        [MustUseReturnValue]
        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            _list.Sort(ByKeyString);
            return new Enumerator(this);
        }

        [Pure] // ReSharper disable ParameterTypeCanBeEnumerable.Local
        static string Name(MemberInfo next, FieldInfo[] fields, PropertyInfo[] properties)
        {
            // ReSharper restore ParameterTypeCanBeEnumerable.Local

            static string QualifyTypeName(MemberInfo next) => $"{next.DeclaringType?.Name}.{next.Name}";

            // We aren't looking at a member from a base type,
            // so we can leave it unqualified, similar to how the 'new' keyword works.
#pragma warning disable MA0169
            if (next.DeclaringType == next.ReflectedType)
                return next.Name;
#pragma warning restore MA0169
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var x in fields)
                if (x != next && x.Name == next.Name)
                    return QualifyTypeName(next);

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var x in properties)
                if (x != next && x.Name == next.Name)
                    return QualifyTypeName(next);

            return next.Name;
        }

        [Pure]
        static object? GetValueOrException(
            object value,
            PropertyInfo next,
            [NonNegativeValue] int str,
            ref int visit,
            HashSet<object?>? seen = null
        )
        {
            try
            {
                return next.GetValue(value, null);
            }
            catch (Exception ex)
            {
                return value is not Exception && TryReflectivelyCollect(ex, str, ref visit, out var x, seen) ? x : ex;
            }
        }

        [Pure]
        static Predicate<DictionaryEntry> Eq(object? key) => x => x.Key.Equals(key);

        [Pure]
        int ByKeyString(DictionaryEntry x, DictionaryEntry y) =>
            StringComparer.Ordinal.Compare(ToString(x.Key), ToString(y.Key));

        Dictionary<string, object?> AddUnique(Dictionary<string, object?> accumulator, DictionaryEntry next)
        {
            var key = ToString(next.Key);

            while (accumulator.ContainsKey(key))
                key = $"…{key}";

            accumulator[key] = next.Value is DeconstructionCollection { Serialized: var x } ? x : next.Value;
            return accumulator;
        }
    }

    /// <summary>The defaults used in <see cref="DeconstructionCollectionExtensions.ToDeconstructed"/>.</summary>
    public const int DefaultVisitLength = 80, DefaultStringLength = 400, DefaultRecurseLength = 20;

    /// <summary>Gets the comparer used in <see cref="DeconstructionCollectionExtensions.ToDeconstructed"/>.</summary>
    [Pure]
    public static IEqualityComparer<object?> Comparer { get; } = new DeconstructionComparer();

    /// <inheritdoc />
    [Pure]
    public bool IsSynchronized => false;

    /// <inheritdoc />
    [NonNegativeValue, Pure]
    public int Count =>
        Inner is var inner && ReferenceEquals(this, inner) ? throw new InvalidOperationException() : inner.Count;

    /// <summary>Gets the maximum length of any given <see cref="string"/>.</summary>
    [NonNegativeValue, Pure] // ReSharper disable once UnusedMember.Local
    public int MaxStringLength => str;

    /// <inheritdoc />
    [Pure]
    public object SyncRoot => this;

    /// <summary>Gets the underlying collection.</summary>
    [Pure]
    public abstract IList Inner { get; }

    /// <summary>Gets the collection to a serializable collection.</summary>
    // Unless this is explicitly overriden, assume the type is already serializable.
    [Pure]
    public virtual ICollection Serialized => this;

    /// <summary>Attempts to truncate the <paramref name="v"/>.</summary>
    /// <param name="v">The <see cref="object"/> to truncate.</param>
    /// <param name="str">The maximum length of any given <see cref="string"/>.</param>
    /// <param name="o">The resulting truncation <see cref="string"/>.</param>
    /// <returns>Whether the <paramref name="v"/> was truncated.</returns>
    public static bool TryTruncate(object? v, [NonNegativeValue] int str, out string o) =>
        $"{v}" is var x &&
        (o = v is not DeconstructionCollection && str >= 1 && x.Length > str
            ? $"{x[..(str - 1)]}…"
            : x) is not null;

    /// <summary>Collects the value however applicable, reverting on failure.</summary>
    /// <param name="value">The complex object to convert.</param>
    /// <param name="str">The maximum length of any given <see cref="string"/>.</param>
    /// <param name="visit">The maximum number of times to recurse.</param>
    /// <param name="any">Whether any value was collected.</param>
    /// <param name="seen">The set of seen values, which is used to avoid recursion.</param>
    /// <returns>The replacement value.</returns>
    public static object? CollectNext(
        object? value,
        [NonNegativeValue] int str,
        ref int visit,
        ref bool any,
        HashSet<object?>? seen = null
    )
    {
        static bool IsChoiceAttribute(Attribute x) =>
            x.GetType() is { DeclaringType: null, FullName: "Emik.ChoiceAttribute" } ||
            (x.GetType().DeclaringType?.DeclaringType)
           .FindPathToNull(x => x.DeclaringType)
           .Any(x => x is { DeclaringType: not null, Name: "Choice" });

        static object? Ok(object? o, out bool any)
        {
            any = true;
            return o;
        }

        switch (value)
        {
            case not null when value.GetType().GetCustomAttributes().Any(IsChoiceAttribute): return value.ToString();
            case nint or nuint or null or DictionaryEntry or DeconstructionCollection or IConvertible: return value;
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
            case Memory<char> m: return m.ToString();
            case ReadOnlyMemory<char> m: return m.ToString();
#endif
            case Type x: return x.ToString();
            case Pointer x: return ToHexString(x);
            case Version x: return x.ToShortString();
            case IDictionary x when DeconstructionDictionary.TryCollect(x, str, ref visit, out var dictionary, seen):
                return Ok(dictionary, out any);
            case IDictionary: goto default;
            case IDictionaryEnumerator x
                when DeconstructionDictionary.TryCollect(x, str, ref visit, out var dictionaryEnumerator, seen):
                return Ok(dictionaryEnumerator, out any);
            case IDictionaryEnumerator: goto default;
            case IEnumerable x when DeconstructionList.TryCollect(x, str, ref visit, out var e, seen):
                return Ok(e, out any);
            case IEnumerable: goto default;
            case IEnumerator x when DeconstructionList.TryCollect(x, str, ref visit, out var e, seen):
                return Ok(e, out any);
            case IEnumerator: goto default;
#if !NET20 && !NET30 && !NET35
            case IStructuralComparable x when DeconstructionList.TryCollect(x, str, ref visit, out var cmp, seen):
                return Ok(cmp, out any);
            case IStructuralComparable: goto default;
            case IStructuralEquatable x when DeconstructionList.TryCollect(x, str, ref visit, out var eq, seen):
                return Ok(eq, out any);
            case IStructuralEquatable: goto default;
#endif
            default:
                return DeconstructionDictionary.TryReflectivelyCollect(value, str, ref visit, out var obj, seen)
                    ? Ok(obj, out any)
                    : value;
        }
    }

    /// <inheritdoc />
    public void CopyTo(Array array, int index) =>
        (Inner is var inner && ReferenceEquals(this, inner) ? throw new InvalidOperationException() : inner)
       .CopyTo(array, index);

    /// <summary>Adds a failure element, and returns <see langword="false"/>.</summary>
    /// <returns>The value <see langword="false"/>.</returns>
    public abstract bool Fail();

    /// <summary>Attempts to recurse into this instance's elements.</summary>
    /// <param name="layer">The amount of layers of recursion to apply.</param>
    /// <param name="visit">The maximum number of times to recurse.</param>
    /// <param name="seen">The set of seen values, which is used to avoid recursion.</param>
    /// <returns>Whether any mutation occured.</returns>
    public abstract bool TryRecurse(int layer, ref int visit, HashSet<object?>? seen = null);

    /// <inheritdoc />
    [Pure]
    public abstract override string ToString();

    /// <summary>Returns the <see cref="string"/> representation of this instance without newlines.</summary>
    /// <returns>The <see cref="string"/> representation of this instance.</returns>
    [Pure]
    public string ToStringWithoutNewLines() => ToString().SplitSpanLines().ToString();

    /// <summary>Recursively simplifies every value according to <see cref="Simplify"/>.</summary>
    /// <returns>Itself. The returned value is not a copy; mutation applies to the instance.</returns>
    public abstract DeconstructionCollection Simplify();

    /// <inheritdoc />
    [MustUseReturnValue]
    public abstract IEnumerator GetEnumerator();

    /// <summary>Starts recursion if the value is a collection.</summary>
    /// <param name="value">The complex object to convert.</param>
    /// <param name="layer">The amount of layers of recursion to apply.</param>
    /// <param name="visit">The maximum number of times to recurse.</param>
    /// <param name="any">Whether any value was collected.</param>
    /// <param name="seen">The set of seen values, which is used to avoid recursion.</param>
    protected static void RecurseNext(
        object? value,
        int layer,
        ref int visit,
        ref bool any,
        HashSet<object?>? seen = null
    )
    {
        if (value is DeconstructionCollection collection)
            any |= collection.TryRecurse(layer - 1, ref visit, seen);
    }

    /// <summary>Converts the <see cref="object"/> to a <see cref="string"/>.</summary>
    /// <param name="value">The <see cref="object"/> to convert.</param>
    /// <returns>The converted <see cref="string"/>.</returns>
    [Pure]
    protected string ToString(object? value)
    {
        TryTruncate(value, str, out var output);
        return output;
    }

    /// <summary>Simplifies the value to either a <see cref="IConvertible"/> or <see cref="string"/>.</summary>
    /// <param name="value">The value to simplify.</param>
    /// <returns>The simplified value.</returns>
    [Pure]
    [return: NotNullIfNotNull(nameof(value))] // ReSharper disable once ReturnTypeCanBeNotNullable
    protected object? SimplifyObject(object? value) =>
        value switch
        {
            null => "null",
            true => "true",
            false => "false",
            DeconstructionCollection x => x.Simplify(),
            Version x => x.ToShortString(),
            Pointer x => ToHexString(x),
            Type x => x.ToString(),
            nuint x => x.ToHexString(),
            nint x => x.ToHexString(),
            string x => ToString(x),
            IConvertible => value,
            _ => ToString(value),
        };

    [Pure]
    static unsafe string ToHexString(Pointer x) => ((nint)Pointer.Unbox(x)).ToHexString();
}
#endif
