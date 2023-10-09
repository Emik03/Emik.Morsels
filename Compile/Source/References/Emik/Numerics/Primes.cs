// SPDX-License-Identifier: MPL-2.0
#pragma warning disable AsyncifyInvocation, MA0051, VSTHRD002, VSTHRD105
// ReSharper disable CognitiveComplexity CommentTypo IdentifierTypo SuggestBaseTypeForParameter
// ReSharper disable once CheckNamespace EmptyNamespace
namespace Emik.Morsels;
#if !NETFRAMEWORK || NET40_OR_GREATER
/// <summary>Efficient prime operations by GordonBGood: https://stackoverflow.com/a/9700790/18052726.</summary>
sealed class Primes : IEnumerable<ulong>
{
    /// <summary>
    /// Very efficient auto-resizing thread-safe read-only indexer class to hold the base primes array.
    /// </summary>
    sealed class BasePrimeArray
    {
        readonly object _lock = new();

        byte[] _saved = Array.Empty<byte>();

        uint _lwi, _lpd;

        [Pure]
        public uint this[uint i]
        {
            get
            {
                if (i < _saved.Length)
                    return _saved[i];

                lock (_lock)
                {
                    var length = _saved.Length;

                    while (i >= length)
                    {
                        var buffer = (ushort[])s_masterCopy.Clone();

                        if (length is 0)
                            for (uint bi = 0, wi = 0, w = 0, msk = 0x8000, v = 0;
                                w < buffer.Length;
                                bi += s_patterns[wi++], wi = wi >= s_length ? 0 : wi)
                            {
                                if (msk >= 0x8000)
                                {
                                    msk = 1;
                                    v = buffer[w++];
                                }
                                else msk <<= 1;

                                if ((v & msk) != 0)
                                    continue;

                                var p = Fstbp + bi + bi;
                                var k = p * p - Fstbp >> 1;
                                if (k >= s_pageRange) break;

                                var pd = p / s_circumference;
                                var kd = k / s_circumference;
                                var kn = s_lookup[k - kd * s_circumference];

                                for (uint wrd = kd * s_candidates + (uint)(kn >> 4), ndx = wi * s_length + kn;
                                    wrd < buffer.Length;)
                                {
                                    var st = s_wheelStates[ndx];
                                    buffer[wrd] |= st._mask;
                                    wrd += st._multiply * pd + st._extra;
                                    ndx = st._next;
                                }
                            }
                        else
                        {
                            _lwi += s_pageRange;
                            CullBuffer(_lwi, buffer);
                        }

                        var c = Count(s_pageRange, buffer);
                        var newArray = new byte[length + c];
                        _saved.CopyTo(newArray, 0);

                        for (uint p = Fstbp + (_lwi << 1), wi = 0, w = 0, mask = 0x8000, current = 0;
                            length < newArray.Length;
                            p += (uint)(s_patterns[wi++] << 1), wi = wi >= s_length ? 0 : wi)
                        {
                            if (mask >= 0x8000)
                            {
                                mask = 1;
                                current = buffer[w++];
                            }
                            else mask <<= 1;

                            if ((current & mask) != 0)
                                continue;

                            var pd = p / s_circumference;
                            newArray[length++] = (byte)((pd - _lpd << 6) + wi);
                            _lpd = pd;
                        }

                        _saved = newArray;
                    }
                }

                return _saved[i];
            }
        }
    }

    /// <summary>This class implements the enumeration (<see cref="IEnumerator"/>).</summary>
    /// <remarks><para>
    /// It works by farming out tasks culling pages, which it then processes in order by enumerating
    /// the found primes as recognized by the remaining non-composite bits in the cull page buffers.
    /// </para></remarks>
    sealed class Enumerator : IEnumerator<ulong>
    {
        readonly Processor[] _processors = new Processor[s_procs];

        int _big = -s_primes.Length - 1;

        uint _wheelUpperBound = s_length - 1;

        ulong _index = (ulong)-s_patterns[s_length - 1];

        ushort _current, _mask;

        ushort[] _buffer;

        public Enumerator()
        {
            for (var s = 0u; s < s_procs; s++)
                _processors[s] = new()
                {
                    _buffer = new ushort[s_bufferSize],
                };

            for (var s = 1u; s < s_procs; s++)
                _processors[s]._task = CullBufferAsync((s - 1u) * s_bufferRange, _processors[s]._buffer, Noop);

            _buffer = _processors[0]._buffer;
        }

        [Pure]
        public ulong Current { get; private set; }

        [Pure]
        object IEnumerator.Current => Current;

        public bool MoveNext()
        {
            if (_big < 0)
            {
                if (_big is -1) // No yield, so automatically comes around again.
                    _big += _buffer.Length;
                else
                {
                    Current = s_primes[s_primes.Length + ++_big];
                    return true;
                }
            }

            do
            {
                _index += s_patterns[_wheelUpperBound++];

                if (_wheelUpperBound >= s_length)
                    _wheelUpperBound = 0;

                if ((_mask <<= 1) != 0)
                    continue;

                if (++_big >= s_bufferSize)
                {
                    _big = 0;

                    for (var prc = 0; prc < s_procs - 1; prc++)
                        _processors[prc] = _processors[prc + 1];

                    _processors[s_procs - 1u]._buffer = _buffer;
                    _processors[s_procs - 1u]._task = CullBufferAsync(_index + (s_procs - 1u) * s_bufferRange, _buffer, Noop);
                    _processors[0]._task.Wait();
                    _buffer = _processors[0]._buffer;
                }

                _current = _buffer[_big];
                _mask = 1;
            } while ((_current & _mask) is not 0);

            Current = Fstbp + _index + _index;
            return true;
        }

        void IEnumerator.Reset() { }

        void IDisposable.Dispose() { }
    }

    [StructLayout(LayoutKind.Auto)]
    struct WheelState
    {
        internal byte _extra, _multiply;

        internal ushort _mask, _next;
    }

    /// <summary>Used for multi-threading buffer array processing.</summary>
    [StructLayout(LayoutKind.Auto)]
    struct Processor
    {
        internal ushort[] _buffer;

        internal Task _task;
    }

    /// <summary>
    /// The <see cref="L1CachePow"/> can not be less than 14 and is
    /// usually the two raised to the power of the L1 or L2 cache.
    /// </summary>
    const int L1CachePow = 14, L1CacheSz = 1 << L1CachePow;

    /// <summary>For buffer <see cref="ushort"/> <see cref="Array"/>.</summary>
    const int Mxpgsz = L1CacheSz / 2;

    /// <summary>This times <see cref="s_bigWheel"/> times two should not be bigger than the L2 cache in bytes.</summary>
    const uint ChunkSize = 17, Fstcp = 11, Fstbp = 19;

    /// <summary>Big wheel primes, following prime.</summary>
    static readonly byte[] s_primes = { 2, 3, 5, 7, 11, 13, 17 };

    /// <summary>A Counting Look Up Table for very fast counting of primes.</summary>
    static readonly byte[] s_counting;

    /// <summary>Look up wheel position from index and vice versa.</summary>
    static readonly byte[] s_lookup;

    /// <summary>Wheel Index Look Up Table.</summary>
    static readonly byte[] s_wheelIndices;

    /// <summary>
    /// The 2,3,57 factorial wheel increment pattern, (sum) 48 elements long, starting at prime 19 position.
    /// </summary>
    static readonly byte[] s_patterns =
    {
        2, 3, 1, 3, 2, 1, 2, 3, 3, 1, 3, 2, 1, 3, 2, 3, 4, 2, 1, 2, 1, 2, 4, 3,
        2, 3, 1, 2, 3, 1, 3, 3, 2, 1, 2, 3, 1, 3, 2, 1, 2, 1, 5, 1, 5, 1, 2, 1,
    };

    /// <summary>The position of the wheel.</summary>
    static readonly byte[] s_positions;

    /// <summary>To look up wheel rounded up index position values, allow for overflow in size.</summary>
    static readonly byte[] s_roundUp;

    /// <summary>One can get single threaded performance by setting <see cref="s_procs"/> = 1.</summary>
    static readonly uint s_procs = (uint)Environment.ProcessorCount + 1;

    /// <summary>Small wheel circumference for odd numbers.</summary>
    static readonly uint s_circumference = s_patterns.Aggregate(0u, (acc, n) => acc + n);

    /// <summary>The length of <see cref="s_patterns"/>.</summary>
    static readonly uint s_length = (uint)s_patterns.Length;

    /// <summary>Number of wheel candidates.</summary>
    static readonly uint s_candidates = s_length >> 4;

    /// <summary>
    /// The big wheel circumference expressed in number of 16 bit words as in a minimum bit buffer size.
    /// </summary>
    static readonly uint s_bigWheel =
        s_primes.Aggregate(1u, (acc, p) => acc * p) / 2 / s_circumference * s_length / 16;

    /// <summary>Page size and range as developed.</summary>
    static readonly uint s_pageSize = Mxpgsz / s_bigWheel * s_bigWheel;

    /// <summary>Buffer size (multiple chunks) as produced.</summary>
    static readonly uint s_pageRange = s_pageSize * 16 / s_length * s_circumference;

    /// <summary>Number of uints even number of caches in chunk.</summary>
    static readonly uint s_bufferSize = ChunkSize * s_pageSize, s_bufferRange = ChunkSize * s_pageRange;

    /// <summary>A Master Copy page used to hold the lower base primes preculled version of the page.</summary>
    static readonly ushort[] s_masterCopy;

    /// <summary>The base primes array.</summary>
    static readonly BasePrimeArray s_basePrimes = new();

    /// <summary>Wheel State Look Up Table.</summary>
    static readonly WheelState[] s_wheelStates;

    /// <summary>Initializes static members of the <see cref="Primes"/> class.</summary>
    /// <remarks><para>The static constructor is used to initialize the static readonly arrays.</para></remarks>
    static Primes()
    {
        // To look up wheel position index from wheel index.
        s_positions = new byte[s_patterns.Length + 1];

        for (byte i = 0, acc = 0; i < s_patterns.Length; i++)
        {
            acc += s_patterns[i];
            s_positions[i + 1] = acc;
        }

        s_lookup = new byte[s_circumference + 1];

        for (byte i = 1; i < s_positions.Length; i++)
            for (var j = (byte)(s_positions[i - 1] + 1); j <= s_positions[i]; j++)
                s_lookup[j] = i;

        s_roundUp = new byte[s_circumference * 2];

        for (byte i = 1; i < s_roundUp.Length; i++)
            if (i > s_circumference)
                s_roundUp[i] = (byte)(s_circumference + s_positions[s_lookup[i - s_circumference]]);
            else
                s_roundUp[i] = s_positions[s_lookup[i]];

        s_counting = new byte[1 << 16];

        for (var i = 0; i < s_counting.Length; i++)
            s_counting[i] = (byte)BitOperations.PopCount((ushort)(i ^ -1));

        s_wheelIndices = new byte[s_length];

        for (var i = 0; i < s_wheelIndices.Length; i++)
        {
            var t = (uint)(s_positions[i] * 2) + Fstbp;

            if (t >= s_circumference)
                t -= s_circumference;

            if (t >= s_circumference)
                t -= s_circumference;

            s_wheelIndices[i] = (byte)t;
        }

        s_wheelStates = new WheelState[s_length * s_length];

        for (var x = 0u; x < s_length; x++)
        {
            var p = Fstbp + 2u * s_positions[x];
            var pr = p % s_circumference;

            for (uint y = 0, position = (p * p - Fstbp) / 2; y < s_length; y++)
            {
                var m = s_patterns[(x + y) % s_length];
                position %= s_circumference;
                var posn = s_lookup[position];
                position += m * pr;
                var nposd = position / s_circumference;
                var nposn = s_lookup[position - nposd * s_circumference];

                s_wheelStates[x * s_length + posn] = new()
                {
                    _mask = (ushort)(1 << (posn & 0xF)),
                    _multiply = (byte)(m * s_candidates),
                    _extra = (byte)(s_candidates * nposd + (nposn >> 4) - (posn >> 4)),
                    _next = (ushort)(s_length * x + nposn),
                };
            }
        }

        s_masterCopy = new ushort[s_pageSize];

        foreach (var lp in s_primes.SkipWhile(p => p < Fstcp))
        {
            var p = (uint)lp;
            var k = p * p - Fstbp >> 1;
            var pd = p / s_circumference;
            var kd = k / s_circumference;
            var kn = s_lookup[k - kd * s_circumference];

            for (uint w = kd * s_candidates + (uint)(kn >> 4),
                ndx = s_lookup[(2 * s_circumference + p - Fstbp) / 2] * s_length + kn;
                w < s_masterCopy.Length;)
            {
                var st = s_wheelStates[ndx];
                s_masterCopy[w] |= st._mask;
                w += st._multiply * pd + st._extra;
                ndx = st._next;
            }
        }
    }

    /// <summary>Gets the shared instance.</summary>
    public static Primes Shared { get; } = new();

    /// <summary>Gets the count of primes up the number, inclusively.</summary>
    /// <param name="topNumber">The ulong top number to check for prime.</param>
    /// <returns>The long number of primes found.</returns>
    [Pure]
    public static long CountTo(ulong topNumber)
    {
        if (topNumber < Fstbp)
            return s_primes.TakeWhile(p => p <= topNumber).Count();

        var cnt = (long)s_primes.Length;
        IterateTo(topNumber, (_, lim, b) => Interlocked.Add(ref cnt, Count(lim, b)));
        return cnt;
    }

    /// <summary>Gets the prime number at the zero based index number given.</summary>
    /// <param name="index">The long zero-based index number for the prime.</param>
    /// <returns>The ulong prime found at the given index.</returns>
    [Pure]
    public static ulong ElementAt(long index)
    {
        if (index < s_primes.Length)
            return s_primes[(int)index];

        long cnt = s_primes.Length;
        var ndx = 0UL;
        var cycl = 0u;
        var bit = 0u;

        IterateUntil(Find);

        return Fstbp + (ndx + cycl * s_circumference + s_positions[bit] << 1);

        bool Find(ulong lwi, ushort[] buffer)
        {
            var c = Count(s_bufferRange, buffer);

            if ((cnt += c) < index)
                return false;

            ndx = lwi;
            cnt -= c;

            do
            {
                var w = cycl++ * s_candidates;
                c = s_counting[buffer[w++]] + s_counting[buffer[w++]] + s_counting[buffer[w]];
                cnt += c;
            } while (cnt < index);

            cnt -= c;
            var y = --cycl * s_candidates;
            var v = ((ulong)buffer[y + 2] << 32) + ((ulong)buffer[y + 1] << 16) + buffer[y];

            do
                if ((v & 1UL << (int)bit++) == 0)
                    ++cnt;
            while (cnt <= index);

            --bit;
            return true;
        }
    }

    /// <summary>Gets the sum of the primes up the number, inclusively.</summary>
    /// <param name="topNumber">The uint top number to check for prime.</param>
    /// <returns>The ulong sum of all the primes found.</returns>
    [Pure]
    public static ulong SumTo(uint topNumber)
    {
        static long SumBuffer(ulong lowi, uint bitlim, ushort[] buf)
        {
            var acc = 0L;

            for (uint i = 0, wi = 0, msk = 0x8000, w = 0, v = 0;
                i < bitlim;
                i += s_patterns[wi++], wi = wi >= s_length ? 0 : wi)
            {
                if (msk >= 0x8000)
                {
                    msk = 1;
                    v = buf[w++];
                }
                else
                    msk <<= 1;

                if ((v & msk) == 0)
                    acc += (long)(Fstbp + (lowi + i << 1));
            }

            return acc;
        }

        if (topNumber < Fstbp)
            return s_primes.TakeWhile(p => p <= topNumber).Aggregate(0u, (acc, p) => acc + p);

        var sum = (long)s_primes.Aggregate(0u, (acc, p) => acc + p);
        IterateTo(topNumber, (pos, lim, b) => Interlocked.Add(ref sum, SumBuffer(pos, lim, b)));
        return (ulong)sum;
    }

    /// <summary>Gets the enumerator for the primes.</summary>
    /// <returns>The enumerator of the primes.</returns>
    [Pure]
    public IEnumerator<ulong> GetEnumerator() => new Enumerator();

    /// <summary>Gets the enumerator for the primes.</summary>
    /// <returns>The enumerator of the primes.</returns>
    [Pure]
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>Fast buffer segment culling method using a Wheel State Look Up Table.</summary>
    /// <param name="lwi">The limit.</param>
    /// <param name="b">The buffer.</param>
    static void CullBuffer(ulong lwi, ushort[] b)
    {
        var nlwi = lwi;

        // Copy pre-culled lower base primes.
        for (var i = 0u; i < b.Length; nlwi += s_pageRange, i += s_pageSize)
            s_masterCopy.CopyTo(b, i);

        for (uint i = 0, pd = 0;; i++)
        {
            pd += s_basePrimes[i] >> 6;
            var wi = s_basePrimes[i] & 0x3Fu;
            var wp = (uint)s_positions[wi];
            var p = pd * s_circumference + s_wheelIndices[wi];
            var k = p * (ulong)p - Fstbp >> 1;

            if (k >= nlwi)
                break;

            if (k < lwi)
            {
                k = (lwi - k) % (s_circumference * p);

                if (k is not 0)
                {
                    var nwp = wp + (uint)((k + p - 1) / p);
                    k = (s_roundUp[nwp] - wp) * p - k;
                }
            }
            else
                k -= lwi;

            var kd = k / s_circumference;
            var kn = s_lookup[k - kd * s_circumference];

            for (uint word = (uint)kd * s_candidates + (uint)(kn >> 4), ndx = wi * s_length + kn; word < b.Length;)
            {
                var st = s_wheelStates[ndx];
                b[word] |= st._mask;
                word += st._multiply * pd + st._extra;
                ndx = st._next;
            }
        }
    }

    /// <summary>
    /// Iterates the action over each page up to the page including the <paramref name="topNumber"/>,
    /// making an adjustment to the top limit for the last page.
    /// </summary>
    /// <remarks><para>This method works for non-dependent actions that can be executed in any order.</para></remarks>
    /// <param name="topNumber">The number to reach.</param>
    /// <param name="action">The action to invoke.</param>
    static void IterateTo(ulong topNumber, Action<ulong, uint, ushort[]> action)
    {
        var processors = new Processor[s_procs];

        for (var s = 0u; s < s_procs; s++)
            processors[s] = new()
            {
                _buffer = new ushort[s_bufferSize],
                _task = Task.Factory.StartNew(Noop),
            };

        var topIndex = topNumber - Fstbp >> 1;

        for (ulong index = 0; index <= topIndex;)
        {
            processors[0]._task.Wait();
            var buffer = processors[0]._buffer;

            for (var s = 0u; s < s_procs - 1; s++)
                processors[s] = processors[s + 1];

            var lowi = index;
            var nxtndx = index + s_bufferRange;
            var lim = topIndex < nxtndx ? (uint)(topIndex - index + 1) : s_bufferRange;

            processors[s_procs - 1] = new()
            {
                _buffer = buffer,
                _task = CullBufferAsync(index, buffer, b => action(lowi, lim, b)),
            };

            index = nxtndx;
        }

        for (var s = 0u; s < s_procs; s++)
            processors[s]._task.Wait();
    }

    /// <summary>
    /// Iterates the <paramref name="predicate"/> over each page up to the page where the predicate paramenter returns
    /// <see langword="true"/>, this method works for dependent operations that need to be executed in increasing order.
    /// </summary>
    /// <remarks><para>It is somewhat slower as the predicate function is executed outside the task.</para></remarks>
    /// <param name="predicate">The predicate to iterate over.</param>
    static void IterateUntil([InstantHandle] Func<ulong, ushort[], bool> predicate)
    {
        var processors = new Processor[s_procs];

        for (var s = 0u; s < s_procs; s++)
        {
            var buffer = new ushort[s_bufferSize];

            processors[s] = new()
            {
                _buffer = buffer,
                _task = CullBufferAsync(s * s_bufferRange, buffer, Noop),
            };
        }

        for (var ndx = 0ul;; ndx += s_bufferRange)
        {
            processors[0]._task.Wait();
            var buffer = processors[0]._buffer;

            if (predicate(ndx, buffer))
                break;

            for (var s = 0u; s < s_procs - 1; s++)
                processors[s] = processors[s + 1];

            processors[s_procs - 1] = new()
            {
                _buffer = buffer,
                _task = CullBufferAsync(ndx + s_procs * s_bufferRange, buffer, Noop),
            };
        }
    }

    /// <summary>Very fast counting method using the CLUT look up table.</summary>
    /// <param name="bitlim">The bit limiter.</param>
    /// <param name="buf">The buffer.</param>
    /// <returns>The count.</returns>
    [MustUseReturnValue]
    static int Count(uint bitlim, ushort[] buf)
    {
        if (bitlim < s_bufferRange)
        {
            var adder = (bitlim - 1) / s_circumference;
            var bit = s_lookup[bitlim - adder * s_circumference] - 1;
            adder *= s_candidates;

            for (var i = 0; i < 3; i++)
                buf[adder++] |= (ushort)(unchecked((ulong)-2) << bit >> (i << 4));
        }

        var accumulator = 0;

        for (uint i = 0, w = 0; i < bitlim; i += s_circumference)
            accumulator += s_counting[buf[w++]] + s_counting[buf[w++]] + s_counting[buf[w++]];

        return accumulator;
    }

    /// <summary>Forms a task of the cull buffer operaion.</summary>
    /// <param name="lwi">The limit.</param>
    /// <param name="b">The buffer.</param>
    /// <param name="f">The callback.</param>
    /// <returns>The awaitable <see cref="Task"/>.</returns>
    [MustUseReturnValue]
    static Task CullBufferAsync(ulong lwi, ushort[] b, Action<ushort[]> f)
    {
        void Wrapper()
        {
            CullBuffer(lwi, b);
            f(b);
        }

        return Task.Factory.StartNew(Wrapper);
    }
}
#endif
