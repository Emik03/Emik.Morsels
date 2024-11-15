// SPDX-License-Identifier: MPL-2.0
#if NETFRAMEWORK || NETSTANDARD && !NETSTANDARD2_1_OR_GREATER && !ROSLYN
// ReSharper disable once CheckNamespace
namespace System.Buffers;

/// <summary>An implementation of <see cref="ArrayPool{T}"/> with configurable buffer lengths and quantity.</summary>
/// <typeparam name="T">The type of array to store.</typeparam>
sealed partial class ConfigurableArrayPool<T> : ArrayPool<T>
{
    /// <summary>Provides a thread-safe bucket containing buffers that can be rented and returned.</summary>
    /// <param name="bufferLength">The length of the buffers this bucket creates and stores.</param>
    /// <param name="numberOfBuffers">The number of buffers to store.</param>
    sealed class Bucket(int bufferLength, int numberOfBuffers)
    {
        internal readonly int _bufferLength = bufferLength;

        readonly T[]?[] _buffers = new T[numberOfBuffers][];

        int _index;

        /// <summary>Attempts to return the buffer to the bucket.</summary>
        internal void Return(T[] array)
        {
            if (array.Length != _bufferLength) // Check to see if the buffer is the correct size for this bucket
                throw new ArgumentException("Buffer is not from pool.", nameof(array));

            // These two lines single-handedly cause so many problems in Unity 2017 that I'd rather be memory wasteful
            // than to spend another minute trying to figure out why it complains about a single stelem.ref instruction.
#if !KTANE
            if (_index is not 0)
                _buffers[--_index] = array;
#endif
        }

        /// <summary>Takes an array from the bucket. If the bucket is empty, returns <see langword="null"/>.</summary>
        [MustUseReturnValue("The returned value must later be given in this instance's Return.")]
        internal T[]? Rent()
        {
            if (_index >= _buffers.Length)
                return null;

            var buffer = _buffers[_index];
            _buffers[_index++] = null;
            var ret = buffer ?? new T[_bufferLength];
            return ret;
        }
    }

    /// <summary>The default maximum length of each array in the pool (2^20).</summary>
    const int DefaultMaxArrayLength = 1024 * 1024;

    /// <summary>The default maximum number of arrays per bucket that are available for rent.</summary>
    const int DefaultMaxNumberOfArraysPerBucket = 50;

    /// <summary>Contains all the buckets.</summary>
    readonly Bucket[] _buckets;

    /// <summary>Initializes a new instance of the <see cref="ConfigurableArrayPool{T}"/> class.</summary>
    internal ConfigurableArrayPool()
        : this(DefaultMaxArrayLength, DefaultMaxNumberOfArraysPerBucket) { }

    /// <summary>Initializes a new instance of the <see cref="ConfigurableArrayPool{T}"/> class.</summary>
    /// <param name="maxArrayLength">The maximum length of arrays to store.</param>
    /// <param name="maxArraysPerBucket">The maximum number of arrays per bucket.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// The parameter <paramref name="maxArrayLength"/> or <paramref name="maxArraysPerBucket"/> are 0 or negative.
    /// </exception>
    internal ConfigurableArrayPool(int maxArrayLength, int maxArraysPerBucket)
    {
        if (maxArrayLength <= 0)
            throw new ArgumentOutOfRangeException(nameof(maxArrayLength));

        if (maxArraysPerBucket <= 0)
            throw new ArgumentOutOfRangeException(nameof(maxArraysPerBucket));

        // Our bucketing algorithm has a min length of 2^4 and a max length of 2^30.
        // Constrain the actual max used to those values.
        const int MinimumArrayLength = 0x10, MaximumArrayLength = 0x40000000;

        // Create the buckets.
        var bufferSize = maxArrayLength switch
        {
            > MaximumArrayLength => MaximumArrayLength,
            < MinimumArrayLength => MinimumArrayLength,
            _ => maxArrayLength,
        };

        _buckets = new Bucket[SelectBucketIndex(bufferSize) + 1];

        for (var i = 0; i < _buckets.Length; i++)
            _buckets[i] = new(GetMaxSizeForBucket(i), maxArraysPerBucket);
    }

    /// <inheritdoc />
    public override void Return(T[] array, bool clearArray = false)
    {
        // Ignore empty arrays. When a zero-length array is rented, we return a singleton rather than actually taking
        // a buffer out of the lowest bucket. Then, determine with what bucket this array length is associated.
        // If we can tell that the buffer was allocated, drop it. Otherwise, check if we have space in the pool.
        if (array.Length is 0 || SelectBucketIndex(array.Length) is var bucket && bucket >= _buckets.Length)
            return;

        if (clearArray) // Clear the array if the user requests it.
            Array.Clear(array, 0, array.Length);

        // Return the buffer to its bucket.
        _buckets[bucket].Return(array);
    }

    /// <inheritdoc />
    [MustUseReturnValue("The returned value must later be given in this instance's Return.")]
    public override T[] Rent([NonNegativeValue] int minimumLength)
    {
        switch (minimumLength)
        {
            // Arrays can't be smaller than zero. We allow requesting zero-length arrays (even though
            // pooling such an array isn't valuable) as it's a valid length array, and we want the pool
            // to be usable in general instead of using `new`, even for computed lengths.
            case < 0: throw new ArgumentOutOfRangeException(nameof(minimumLength));
            // No need for events with the empty array.  Our pool is effectively infinite,
            // and we'll never allocate for rents and never store for returns.
            case 0: return [];
        }

        // The request was for a size too large for the pool. Allocate an array of exactly
        // the requested length. When it's returned to the pool, we'll simply throw it away.
        if (SelectBucketIndex(minimumLength) is var index && index >= _buckets.Length)
            return new T[minimumLength];

        // Search for an array starting at the 'index' bucket. If the bucket is empty, bump up
        // to the next higher bucket and try that one, but only try at most a few buckets.
        const int MaxBucketsToTry = 2;
        var i = index;

        do // Attempt to rent from the bucket. If we get a buffer from it, return it.
            if (_buckets[i].Rent() is { } buffer)
                return buffer;
        while (++i < _buckets.Length && i != index + MaxBucketsToTry);

        // The pool was exhausted for this buffer size. Allocate a new
        // buffer with a size corresponding to the appropriate bucket.
        var ret = new T[_buckets[index]._bufferLength];
        return ret;
    }

    // Buffers are bucketed so that a request between 2^(n-1) + 1 and 2^n is given a buffer of 2^n
    // Bucket index is log2(bufferSize - 1) with the exception that buffers between 1 and 16 bytes
    // are combined, and the index is slid down by 3 to compensate.
    // Zero is a valid bufferSize, and it is assigned the highest bucket index so that zero-length
    // buffers are not retained by the pool. The pool will return the Array.Empty singleton for these.
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    static int SelectBucketIndex(int bufferSize) => BitOperations.Log2((uint)bufferSize - 1 | 15) - 3;

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    static int GetMaxSizeForBucket(int binIndex)
    {
        var maxSize = 16 << binIndex; // ReSharper disable once RedundantNameQualifier UseSymbolAlias
        System.Diagnostics.Debug.Assert(maxSize >= 0);
        return maxSize;
    }
}
#endif
