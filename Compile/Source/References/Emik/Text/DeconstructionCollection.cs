// SPDX-License-Identifier: MPL-2.0
// ReSharper disable CheckNamespace EmptyNamespace
namespace Emik.Morsels;
#if NETFRAMEWORK || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
/// <summary>Contains methods for deconstructing objects.</summary>
#pragma warning disable 9107, MA0048
static partial class DeconstructionCollectionExtensions
#pragma warning restore MA0048
{
    /// <summary>Takes the complex object and turns it into a structure that is serializable.</summary>
    /// <param name="value">The complex object to convert.</param>
    /// <param name="recurseLength">The maximum number of times to recurse a nested object or dictionary.</param>
    /// <param name="visitLength">The maximum number of times to recurse through an enumeration.</param>
    /// <param name="stringLength">The maximum length of any given <see cref="string"/>.</param>
    /// <returns>
    /// The serializable object: any of <see cref="IntPtr"/>, <see cref="UIntPtr"/>,
    /// <see cref="ISerializable"/>, or <see cref="DeconstructionCollection"/>.
    /// </returns>
    [Pure]
    [return: NotNullIfNotNull(nameof(value))]
    public static object? ToDeconstructed(
        this object? value,
        [NonNegativeValue] int recurseLength = 128,
        [NonNegativeValue] int visitLength = 64,
        [NonNegativeValue] int stringLength = 32
    )
    {
        if (value is DeconstructionCollection)
            return value;

        if (DeconstructionCollection.TryNew(value, stringLength, ref visitLength) is not { } collection)
            return DeconstructionCollection.TryTruncate(value, stringLength, out var output) ? output : value;

        for (var i = 0; recurseLength > 0 && i < recurseLength && collection.TryRecurse(i, ref visitLength); i++) { }

        return collection.Simplify();
    }
}

/// <summary>Defines the collection responsible for deconstructing.</summary>
/// <param name="str">The maximum length of any given <see cref="string"/>.</param>
abstract partial class DeconstructionCollection([NonNegativeValue] int str) : ICollection
{
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

        /// <summary>Attempts to deconstruct an object by enumerating it.</summary>
        /// <param name="enumerator">The enumerator to collect. It will be disposed after the method halts.</param>
        /// <param name="str">The maximum length of any given <see cref="string"/>.</param>
        /// <param name="visit">The maximum number of times to recurse.</param>
        /// <param name="list">The resulting <see cref="DeconstructionList"/>.</param>
        /// <returns>
        /// Whether the parameter <paramref name="value"/> was deconstructed fully and <paramref name="visit"/>
        /// altered. When this method returns <see langword="false"/>, the parameter <paramref name="dictionary"/>
        /// will still contain the elements that were able to be deconstructed, alongside an ellipsis.
        /// </returns>
        public static bool TryCollect(
            [HandlesResourceDisposal] IEnumerator enumerator,
            [NonNegativeValue] int str,
            ref int visit,
            out DeconstructionList list
        )
        {
            using var _ = enumerator as IDisposable;
            var copy = visit;
            list = new(str);

            while (enumerator.MoveNext())
                if (--copy > 0)
                    list.Add(enumerator.Current);
                else if (!enumerator.MoveNext())
                    break;
                else
                    return list.Fail();

            visit = copy;
            return true;
        }

        /// <summary>Attempts to deconstruct an object by enumerating it.</summary>
        /// <param name="enumerable">The enumerator to collect.</param>
        /// <param name="str">The maximum length of any given <see cref="string"/>.</param>
        /// <param name="visit">The maximum number of times to recurse.</param>
        /// <param name="list">The resulting <see cref="DeconstructionList"/>.</param>
        /// <returns>
        /// Whether the parameter <paramref name="value"/> was deconstructed fully and <paramref name="visit"/>
        /// altered. When this method returns <see langword="false"/>, the parameter <paramref name="dictionary"/>
        /// will still contain the elements that were able to be deconstructed, alongside an ellipsis.
        /// </returns>
        public static bool TryCollect(
            [InstantHandle] IEnumerable enumerable,
            [NonNegativeValue] int str,
            ref int visit,
            out DeconstructionList list
        ) =>
            TryCollect(enumerable.GetEnumerator(), str, ref visit, out list);

        /// <summary>Attempts to deconstruct an object by enumerating it.</summary>
        /// <param name="comparable">The comparable to collect.</param>
        /// <param name="str">The maximum length of any given <see cref="string"/>.</param>
        /// <param name="visit">The maximum number of times to recurse.</param>
        /// <param name="list">The resulting <see cref="DeconstructionList"/>.</param>
        /// <returns>
        /// Whether the parameter <paramref name="value"/> was deconstructed fully and <paramref name="visit"/>
        /// altered. When this method returns <see langword="false"/>, the parameter <paramref name="dictionary"/>
        /// will still contain the elements that were able to be deconstructed, alongside an ellipsis.
        /// </returns>
        public static bool TryCollect(
            [InstantHandle] IStructuralComparable comparable,
            [NonNegativeValue] int str,
            ref int visit,
            out DeconstructionList list
        ) =>
            TryCollect(comparable.ToList(), str, ref visit, out list);

        /// <summary>Attempts to deconstruct an object by enumerating it.</summary>
        /// <param name="equatable">The equatable to collect.</param>
        /// <param name="str">The maximum length of any given <see cref="string"/>.</param>
        /// <param name="visit">The maximum number of times to recurse.</param>
        /// <param name="list">The resulting <see cref="DeconstructionList"/>.</param>
        /// <returns>
        /// Whether the parameter <paramref name="value"/> was deconstructed fully and <paramref name="visit"/>
        /// altered. When this method returns <see langword="false"/>, the parameter <paramref name="dictionary"/>
        /// will still contain the elements that were able to be deconstructed, alongside an ellipsis.
        /// </returns>
        public static bool TryCollect(
            [InstantHandle] IStructuralEquatable equatable,
            [NonNegativeValue] int str,
            ref int visit,
            out DeconstructionList list
        ) =>
            TryCollect(equatable.ToList(), str, ref visit, out list);

        public override bool Fail()
        {
            // We append a character instead of a string because otherwise it would be surrounded by quotes.
            Add('…');
            return false;
        }

        /// <inheritdoc />
        public override bool TryRecurse(int layer, ref int visit)
        {
            if (layer < 0)
                return false;

            var any = false;

            if (layer is 0)
                for (var i = 0; i < Count; i++)
                    _list[i] = CollectNext(_list[i], str, ref visit, ref any);
            else
                foreach (var next in _list)
                    RecurseNext(next, layer, ref visit, ref any);

            return any;
        }

        /// <inheritdoc />
        [NonNegativeValue]
        public int Add(object? value) => ((IList)_list).Add(value);

        /// <inheritdoc />
        [Pure]
        public override string ToString() => $"[{_list.Select(ToString).Conjoin()}]";

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

        /// <summary>An equality comparer that always returns <see langword="false"/>.</summary>
        /// <remarks><para>This class uses a perfect hash function.</para></remarks>
        sealed class Inequality : IEqualityComparer<string>
        {
            int _seed;

            /// <inheritdoc />
            [Pure]
            bool IEqualityComparer<string>.Equals(string? x, string? y) => false;

            /// <inheritdoc />
            [MustUseReturnValue]
            int IEqualityComparer<string>.GetHashCode(string? obj) => unchecked(_seed++);
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
            _list.ToDictionary(x => ToString(x.Key), SerializeValue, new Inequality());

        /// <summary>Attempts to deconstruct an object by enumerating it.</summary>
        /// <param name="enumerator">The enumerator to collect. It will be disposed after the method halts.</param>
        /// <param name="str">The maximum length of any given <see cref="string"/>.</param>
        /// <param name="visit">The maximum number of times to recurse.</param>
        /// <param name="dictionary">The resulting <see cref="DeconstructionDictionary"/>.</param>
        /// <returns>
        /// Whether the parameter <paramref name="value"/> was deconstructed fully and <paramref name="visit"/>
        /// altered. When this method returns <see langword="false"/>, the parameter <paramref name="dictionary"/>
        /// will still contain the elements that were able to be deconstructed, alongside an ellipsis.
        /// </returns>
        public static bool TryCollect(
            [HandlesResourceDisposal] IDictionaryEnumerator enumerator,
            [NonNegativeValue] int str,
            ref int visit,
            out DeconstructionDictionary dictionary
        )
        {
            using var _ = enumerator as IDisposable;
            var copy = visit;
            dictionary = new(str);

            while (enumerator.MoveNext())
                if (--copy > 0)
                    dictionary.Add(enumerator.Key, enumerator.Value);
                else if (!enumerator.MoveNext())
                    break;
                else
                    return dictionary.Fail();

            visit = copy;
            return true;
        }

        /// <summary>Attempts to deconstruct an object by enumerating it.</summary>
        /// <param name="dict">The dictionary to collect.</param>
        /// <param name="str">The maximum length of any given <see cref="string"/>.</param>
        /// <param name="visit">The maximum number of times to recurse.</param>
        /// <param name="dictionary">The resulting <see cref="DeconstructionDictionary"/>.</param>
        /// <returns>
        /// Whether the parameter <paramref name="value"/> was deconstructed fully and <paramref name="visit"/>
        /// altered. When this method returns <see langword="false"/>, the parameter <paramref name="dictionary"/>
        /// will still contain the elements that were able to be deconstructed, alongside an ellipsis.
        /// </returns>
        public static bool TryCollect(
            IDictionary dict,
            [NonNegativeValue] int str,
            ref int visit,
            out DeconstructionDictionary dictionary
        ) =>
            TryCollect(dict.GetEnumerator(), str, ref visit, out dictionary);

        /// <summary>Attempts to deconstruct an object by reflectively collecting its fields and properties.</summary>
        /// <param name="value">The complex object to convert.</param>
        /// <param name="str">The maximum length of any given <see cref="string"/>.</param>
        /// <param name="visit">The maximum number of times to recurse.</param>
        /// <param name="dictionary">The resulting <see cref="DeconstructionDictionary"/>.</param>
        /// <returns>
        /// Whether the parameter <paramref name="value"/> was deconstructed fully and <paramref name="visit"/>
        /// altered. When this method returns <see langword="false"/>, the parameter <paramref name="dictionary"/>
        /// will still contain the elements that were able to be deconstructed, alongside an ellipsis.
        /// </returns>
        // ReSharper disable once CognitiveComplexity
        public static bool TryReflectivelyCollect(
            object value,
            [NonNegativeValue] int str,
            ref int visit,
            out DeconstructionDictionary dictionary
        )
        {
            var type = value.GetType();
            var copy = visit;
            dictionary = new(str);

            // ReSharper disable once LoopCanBePartlyConvertedToQuery
            foreach (var next in type.GetFields())
            {
                if (next.IsStatic)
                    continue;
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
                if (next.FieldType.IsByRefLike)
                    continue;
#endif
                if (--copy <= 0)
                    return dictionary.Fail();

                dictionary.Add(next.Name, next.GetValue(value));
            }

            // ReSharper disable once LoopCanBePartlyConvertedToQuery
            foreach (var next in type.GetProperties())
            {
                if (next.GetMethod is { } getter && (getter.IsStatic || next.GetMethod.GetParameters() is not []))
                    continue;
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
                if (next.PropertyType.IsByRefLike)
                    continue;
#endif
                if (--copy <= 0)
                    return dictionary.Fail();

                var result = GetValueOrException(value, next);
                dictionary.Add(next.Name, result);
            }

            visit = copy;
            return true;
        }

        /// <inheritdoc cref="IDictionary.Add"/>
        // ReSharper disable once NullableWarningSuppressionIsUsed
        public void Add(object? key, object? value) => _list.Add(new(key!, value));

        /// <inheritdoc cref="ICollection.Clear"/>
        public void Clear() => _list.Clear();

        /// <inheritdoc />
        public override bool Fail()
        {
            // We append a character instead of a string because otherwise it would be surrounded by quotes.
            Add('…', '…');
            return false;
        }

        /// <inheritdoc />
        public override bool TryRecurse(int layer, ref int visit)
        {
            if (layer < 0)
                return false;

            var any = false;

            if (layer is 0)
                for (var i = 0; i < Count; i++) // ReSharper disable once NullableWarningSuppressionIsUsed
                    _list[i] = new(
                        CollectNext(_list[i].Key, str, ref visit, ref any)!,
                        CollectNext(_list[i].Value, str, ref visit, ref any)
                    );
            else
                foreach (var next in _list)
                {
                    RecurseNext(next.Key, layer, ref visit, ref any);
                    RecurseNext(next.Value, layer, ref visit, ref any);
                }

            return any;
        }

        /// <inheritdoc />
        [Pure]
        public override string ToString() =>
            $"{{ {_list.Select(x => $"{ToString(x.Key)}: {ToString(x.Value)}").Conjoin()} }}";

        /// <inheritdoc />
        public override DeconstructionCollection Simplify()
        {
            for (var i = 0; i < Count; i++) // ReSharper disable once NullableWarningSuppressionIsUsed
                _list[i] = new(SimplifyObject(_list[i].Key)!, SimplifyObject(_list[i].Value));

            return this;
        }

        /// <inheritdoc />
        [MustUseReturnValue]
        public override IEnumerator GetEnumerator() => ((IDictionary)this).GetEnumerator();

        /// <inheritdoc />
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

        [Pure]
        static object? GetValueOrException(object value, PropertyInfo next)
        {
            try
            {
                return next.GetValue(value, null);
            }
#pragma warning disable CA1031
            catch (Exception ex)
#pragma warning restore CA1031
            {
                return ex;
            }
        }

        [Pure]
        static object? SerializeValue(DictionaryEntry next) =>
            next.Value is DeconstructionCollection collection ? collection.Serialized : next.Value;

        [Pure]
        static Predicate<DictionaryEntry> Eq(object? key) => x => x.Key.Equals(key);

        [Pure]
        int ByKeyString(DictionaryEntry x, DictionaryEntry y) =>
            StringComparer.Ordinal.Compare(ToString(x.Key), ToString(y.Key));
    }

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

    /// <summary>
    /// Attempts to create a <see cref="DeconstructionCollection"/> from <paramref name="value"/>.
    /// </summary>
    /// <param name="value">The complex object to convert.</param>
    /// <param name="str">The maximum length of any given <see cref="string"/>.</param>
    /// <param name="visit">The maximum number of times to recurse.</param>
    /// <returns>
    /// The <see cref="DeconstructionCollection"/> if conversion is possible; <see langword="null"/> otherwise.
    /// </returns>
    [Pure]
    public static DeconstructionCollection? TryNew(object? value, [NonNegativeValue] int str, ref int visit)
    {
        switch (value)
        {
            case nint or nuint or null or IConvertible or DeconstructionCollection: return null;
            case IDictionary x:
                DeconstructionDictionary.TryCollect(x, str, ref visit, out var dictionary);
                return dictionary;
            case IDictionaryEnumerator x:
                DeconstructionDictionary.TryCollect(x, str, ref visit, out var dictionaryEnumerator);
                return dictionaryEnumerator;
            case IEnumerable x:
                DeconstructionList.TryCollect(x, str, ref visit, out var enumerable);
                return enumerable;
            case IEnumerator x:
                DeconstructionList.TryCollect(x, str, ref visit, out var enumerator);
                return enumerator;
            case IStructuralComparable x:
                DeconstructionList.TryCollect(x, str, ref visit, out var comparable);
                return comparable;
            case IStructuralEquatable x:
                DeconstructionList.TryCollect(x, str, ref visit, out var equatable);
                return equatable;
            default:
                DeconstructionDictionary.TryReflectivelyCollect(value, str, ref visit, out var obj);
                return obj;
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
    /// <returns>Whether any mutation occured.</returns>
    public abstract bool TryRecurse(int layer, ref int visit);

    /// <inheritdoc />
    [Pure]
    public abstract override string ToString();

    /// <summary>Returns the <see cref="string"/> representation of this instance without newlines.</summary>
    /// <returns>The <see cref="string"/> representation of this instance.</returns>
    [Pure]
    public string ToStringWithoutNewLines() =>
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
        ToString().SplitSpanLines().ToString();
#else
        $"{Whitespaces.Breaking.Aggregate(new StringBuilder(ToString()), (acc, next) => acc.Replace($"{next}", ""))}";
#endif

    /// <summary>Recursively simplifies every value according to <see cref="Simplify"/>.</summary>
    /// <returns>Itself. The returned value is not a copy; mutation applies to the instance.</returns>
    public abstract DeconstructionCollection Simplify();

    /// <inheritdoc />
    [MustUseReturnValue]
    public abstract IEnumerator GetEnumerator();

    /// <summary>Collects the value however applicable, reverting on failure.</summary>
    /// <param name="value">The complex object to convert.</param>
    /// <param name="str">The maximum length of any given <see cref="string"/>.</param>
    /// <param name="visit">The maximum number of times to recurse.</param>
    /// <param name="any">Whether any value was collected.</param>
    /// <returns>The replacement value.</returns>
    protected static object? CollectNext(object? value, [NonNegativeValue] int str, ref int visit, ref bool any)
    {
        static object? Ok(object? o, out bool any)
        {
            any = true;
            return o;
        }

        switch (value)
        {
            case nint or nuint or null or DictionaryEntry or IConvertible or DeconstructionCollection: return value;
            case IDictionary x when DeconstructionDictionary.TryCollect(x, str, ref visit, out var dictionary):
                return Ok(dictionary, out any);
            case IDictionary: goto default;
            case IDictionaryEnumerator x
                when DeconstructionDictionary.TryCollect(x, str, ref visit, out var dictionaryEnumerator):
                return Ok(dictionaryEnumerator, out any);
            case IDictionaryEnumerator: goto default;
            case IEnumerable x when DeconstructionList.TryCollect(x, str, ref visit, out var enumerable):
                return Ok(enumerable, out any);
            case IEnumerable: goto default;
            case IEnumerator x when DeconstructionList.TryCollect(x, str, ref visit, out var enumerator):
                return Ok(enumerator, out any);
            case IEnumerator: goto default;
            case IStructuralComparable x when DeconstructionList.TryCollect(x, str, ref visit, out var comparable):
                return Ok(comparable, out any);
            case IStructuralComparable: goto default;
            case IStructuralEquatable x when DeconstructionList.TryCollect(x, str, ref visit, out var equatable):
                return Ok(equatable, out any);
            case IStructuralEquatable: goto default;
            default:
                return DeconstructionDictionary.TryReflectivelyCollect(value, str, ref visit, out var obj)
                    ? Ok(obj, out any)
                    : value;
        }
    }

    /// <summary>Starts recursion if the value is a collection.</summary>
    /// <param name="value">The complex object to convert.</param>
    /// <param name="layer">The amount of layers of recursion to apply.</param>
    /// <param name="visit">The maximum number of times to recurse.</param>
    /// <param name="any">Whether any value was collected.</param>
    protected static void RecurseNext(object? value, int layer, ref int visit, ref bool any)
    {
        if (value is DeconstructionCollection collection)
            any |= collection.TryRecurse(layer - 1, ref visit);
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
    protected unsafe object? SimplifyObject(object? value) =>
        value switch
        {
            Pointer => ((nuint)Pointer.Unbox(value)).ToHexString(),
            DeconstructionCollection x => x.Simplify(),
            nuint x => x.ToHexString(),
            nint x => x.ToHexString(),
            string x => ToString(x),
            null or IConvertible => value,
            _ => ToString(value),
        };
}
#endif
