// SPDX-License-Identifier: MPL-2.0

// ReSharper disable CheckNamespace EmptyNamespace StructCanBeMadeReadOnly SuggestBaseTypeForParameterInConstructor
namespace System;
#if !(NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) || NO_SYSTEM_MEMORY
#pragma warning disable 8500
using static Span;

[DebuggerDisplay("{ToString(),raw}"), DebuggerTypeProxy(typeof(MemoryDebugView<>)), StructLayout(LayoutKind.Sequential)]
#if !NO_READONLY_STRUCTS
readonly
#endif // ReSharper disable once BadPreprocessorIndent
    struct Memory<T>
{
    const int RemoveFlagsBitMask = int.MaxValue;

    readonly object? _object;

    readonly int _index;

    readonly int _length;

    public static Memory<T> Empty
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => default;
    }

    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => _length & RemoveFlagsBitMask;
    }

    public bool IsEmpty
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => Length is 0;
    }

    public Span<T> Span
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), MustUseReturnValue]
        get => // ReSharper disable once NullableWarningSuppressionIsUsed
            _index < 0 ? ((MemoryManager<T>)_object!).GetSpan().Slice(_index & RemoveFlagsBitMask, _length) :
                typeof(T) == typeof(char) && _object is string text ?
                    new Span<T>(Unsafe.As<Pinnable<T>>(text), MemoryExtensions.StringAdjustment, text.Length)
                       .Slice(_index, _length) :
                    _object is null ? default : new((T[])_object, _index, _length & RemoveFlagsBitMask);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Memory(T[]? array)
    {
        if (array is null)
            return;

        if (default(T) is null && array.GetType() != typeof(T[]))
            throw new ArrayTypeMismatchException();

        _object = array;
        _index = 0;
        _length = array.Length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Memory(T[]? array, int start)
    {
        if (array is null)
        {
            if (start is 0)
                return;

            throw new ArgumentOutOfRangeException(nameof(start));
        }

        if (default(T) is null && array.GetType() != typeof(T[]))
            throw new ArrayTypeMismatchException();

        if ((uint)start > (uint)array.Length)
            throw new ArgumentOutOfRangeException(nameof(start));

        _object = array;
        _index = start;
        _length = array.Length - start;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Memory(T[]? array, int start, int length)
    {
        if (array is null)
        {
            if (start is 0 && length is 0)
                return;

            throw new ArgumentOutOfRangeException(nameof(length));
        }

        if (default(T) is null && array.GetType() != typeof(T[]))
            throw new ArrayTypeMismatchException();

        if ((uint)start > (uint)array.Length || (uint)length > (uint)(array.Length - start))
            throw new ArgumentOutOfRangeException(nameof(length));

        _object = array;
        _index = start;
        _length = length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Memory(MemoryManager<T> manager, int length)
        : this((object)manager, int.MinValue, length)
    {
        if (length < 0)
            throw new ArgumentOutOfRangeException(nameof(length));

        _object = manager;
        _index = int.MinValue;
        _length = length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Memory(MemoryManager<T> manager, int start, int length)
        : this((object)manager, start | int.MinValue, length)
    {
        if (length < 0 || start < 0)
            throw new ArgumentOutOfRangeException(nameof(length));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Memory(object? obj, int start, int length)
    {
        _object = obj;
        _index = start;
        _length = length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static implicit operator Memory<T>(T[] array) => new(array);

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static implicit operator Memory<T>(ArraySegment<T> segment) =>
        new(segment.Array, segment.Offset, segment.Count);

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static implicit operator ReadOnlyMemory<T>(Memory<T> memory) =>
        Unsafe.As<Memory<T>, ReadOnlyMemory<T>>(ref memory);

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public override string ToString() =>
        typeof(T) == typeof(char)
            ? _object is string text ? text.Substring(_index, _length & RemoveFlagsBitMask) : Span.ToString()
            : $"System.Memory<{typeof(T).Name}>[{_length & RemoveFlagsBitMask}]";

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public Memory<T> Slice(int start)
    {
        var length = _length;
        var num = length & RemoveFlagsBitMask;

        if ((uint)start > (uint)num)
            throw new ArgumentOutOfRangeException(nameof(start));

        return new(_object, _index + start, length - start);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public Memory<T> Slice(int start, int length)
    {
        var length2 = _length;
        var num = length2 & RemoveFlagsBitMask;

        if ((uint)start > (uint)num || (uint)length > (uint)(num - start))
            throw new ArgumentOutOfRangeException(nameof(length));

        return new(_object, _index + start, length | length2 & int.MinValue);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CopyTo(Memory<T> destination) => Span.CopyTo(destination.Span);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryCopyTo(Memory<T> destination) => Span.TryCopyTo(destination.Span);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe MemoryHandle Pin()
    {
        if (_index < 0) // ReSharper disable once NullableWarningSuppressionIsUsed
            return ((MemoryManager<T>)_object!).Pin(_index & RemoveFlagsBitMask);

        if (typeof(T) == typeof(char) && _object is string value)
        {
            var stringHandle = GCHandle.Alloc(value, GCHandleType.Pinned);
            return new((T*)stringHandle.AddrOfPinnedObject() + _index, stringHandle);
        }

        if (_object is not T[] array)
            return default;

        if (_length < 0)
            return new((void*)Unsafe.As<T[], nint>(ref array));

        var arrayHandle = GCHandle.Alloc(array, GCHandleType.Pinned);
        return new((T*)arrayHandle.AddrOfPinnedObject() + _index, arrayHandle);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public T[] ToArray() => Span.ToArray();

    [EditorBrowsable(EditorBrowsableState.Never), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public override bool Equals(object? obj) =>
        obj switch
        {
            ReadOnlyMemory<T> readOnlyMemory => readOnlyMemory.Equals(this),
            Memory<T> other => Equals(other),
            _ => false,
        };

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public bool Equals(Memory<T> other)
    {
        if (_object == other._object && _index == other._index)
            return _length == other._length;

        return false;
    }

    [EditorBrowsable(EditorBrowsableState.Never), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public override int GetHashCode() =>
        _object is null ? 0 : CombineHashCodes(_object.GetHashCode(), _index.GetHashCode(), _length.GetHashCode());

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    static int CombineHashCodes(int left, int right) => (left << 5) + left ^ right;

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    static int CombineHashCodes(int h1, int h2, int h3) => CombineHashCodes(CombineHashCodes(h1, h2), h3);
}

[DebuggerDisplay("{ToString(),raw}"), DebuggerTypeProxy(typeof(MemoryDebugView<>)), StructLayout(LayoutKind.Sequential)]
#if !NO_READONLY_STRUCTS
readonly
#endif // ReSharper disable once BadPreprocessorIndent
    struct ReadOnlyMemory<T>
{
#pragma warning disable RCS1158
    internal const int RemoveFlagsBitMask = int.MaxValue;
#pragma warning restore RCS1158
    readonly object? _object;

    readonly int _index;

    readonly int _length;

    public static ReadOnlyMemory<T> Empty
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => default;
    }

    public bool IsEmpty
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => Length is 0;
    }

    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => _length & RemoveFlagsBitMask;
    }

    public ReadOnlySpan<T> Span
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), MustUseReturnValue]
        get
        {
            if (_index < 0) // ReSharper disable once NullableWarningSuppressionIsUsed
                return ((MemoryManager<T>)_object!).GetSpan().Slice(_index & RemoveFlagsBitMask, _length);

            if (typeof(T) == typeof(char) && _object is string text)
                return new ReadOnlySpan<T>(Unsafe.As<Pinnable<T>>(text), MemoryExtensions.StringAdjustment, text.Length)
                   .Slice(_index, _length);

            if (_object != null)
                return new((T[])_object, _index, _length & RemoveFlagsBitMask);

            return default;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlyMemory(T[]? array)
    {
        if (array is null)
            return;

        _object = array;
        _index = 0;
        _length = array.Length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlyMemory(T[]? array, int start, int length)
    {
        if (array is null)
        {
            if (start is not 0 || length is not 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            this = default;
            return;
        }

        if ((uint)start > (uint)array.Length || (uint)length > (uint)(array.Length - start))
            throw new ArgumentOutOfRangeException(nameof(length));

        _object = array;
        _index = start;
        _length = length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ReadOnlyMemory(object? obj, int start, int length)
    {
        _object = obj;
        _index = start;
        _length = length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static implicit operator ReadOnlyMemory<T>(T[] array) => new(array);

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static implicit operator ReadOnlyMemory<T>(ArraySegment<T> segment) =>
        new(segment.Array, segment.Offset, segment.Count);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CopyTo(Memory<T> destination) => Span.CopyTo(destination.Span);

    [EditorBrowsable(EditorBrowsableState.Never), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public override bool Equals(object? obj) =>
        obj switch
        {
            ReadOnlyMemory<T> other => Equals(other),
            Memory<T> memory => Equals(memory),
            _ => false,
        };

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public bool Equals(ReadOnlyMemory<T> other) =>
        _object == other._object && _index == other._index && _length == other._length;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryCopyTo(Memory<T> destination) => Span.TryCopyTo(destination.Span);

    [EditorBrowsable(EditorBrowsableState.Never), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public override int GetHashCode() =>
        _object is null ? 0 : CombineHashCodes(_object.GetHashCode(), _index.GetHashCode(), _length.GetHashCode());

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public override string ToString() =>
        typeof(T) == typeof(char)
            ? _object is string text ? text.Substring(_index, _length & RemoveFlagsBitMask) : Span.ToString()
            : $"System.ReadOnlyMemory<{typeof(T).Name}>[{_length & RemoveFlagsBitMask}]";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe MemoryHandle Pin()
    {
        if (_index < 0) // ReSharper disable once NullableWarningSuppressionIsUsed
            return ((MemoryManager<T>)_object!).Pin(_index & RemoveFlagsBitMask);

        if (typeof(T) == typeof(char) && _object is string value)
        {
            var handle = GCHandle.Alloc(value, GCHandleType.Pinned);
            void* pointer = (T*)handle.AddrOfPinnedObject() + _index;
            return new(pointer, handle);
        }

        if (_object is not T[] array)
            return default;

        if (_length < 0)
            return new((void*)Unsafe.As<T[], nint>(ref array));

        var handle2 = GCHandle.Alloc(array, GCHandleType.Pinned);
        void* pointer3 = (T*)handle2.AddrOfPinnedObject() + _index;
        return new(pointer3, handle2);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public ReadOnlyMemory<T> Slice(int start)
    {
        var length = _length;
        var num = length & RemoveFlagsBitMask;

        if ((uint)start > (uint)num)
            throw new ArgumentOutOfRangeException(nameof(start));

        return new(_object, _index + start, length - start);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public ReadOnlyMemory<T> Slice(int start, int length) =>
        _length is var length2 && (_length & RemoveFlagsBitMask) is var num && (uint)start > (uint)num ||
        (uint)length > (uint)(num - start)
            ? throw new ArgumentOutOfRangeException(nameof(start))
            : new(_object, _index + start, length | length2 & int.MinValue);

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public T[] ToArray() => Span.ToArray();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal object? GetObjectStartLength(out int start, out int length)
    {
        start = _index;
        length = _length;
        return _object;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    static int CombineHashCodes(int left, int right) => (left << 5) + left ^ right;

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    static int CombineHashCodes(int h1, int h2, int h3) => CombineHashCodes(CombineHashCodes(h1, h2), h3);
}

[StructLayout(LayoutKind.Auto)]
struct MemoryHandle : IDisposable
{
    GCHandle _handle;

    IPinnable? _pinnable;

    public unsafe void* Pointer { get; private set; }

    [CLSCompliant(false)]
    public unsafe MemoryHandle(void* pointer, GCHandle handle = default, IPinnable? pinnable = null)
    {
        Pointer = pointer;
        _handle = handle;
        _pinnable = pinnable;
    }

    public unsafe void Dispose()
    {
        if (_handle.IsAllocated)
            _handle.Free();

        if (_pinnable != null)
        {
            _pinnable.Unpin();
            _pinnable = null;
        }

        Pointer = null;
    }
}

abstract class MemoryManager<T> : IMemoryOwner<T>, IDisposable, IPinnable
{
    public virtual Memory<T> Memory => new(this, GetSpan().Length);

    public abstract void Unpin();
#pragma warning disable MA0061
    public abstract MemoryHandle Pin(int elementIndex = 0);
#pragma warning restore MA0061
    public abstract Span<T> GetSpan();

    void IDisposable.Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected abstract void Dispose(bool disposing);

    protected internal virtual bool TryGetArray(out ArraySegment<T> segment)
    {
        segment = default;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected Memory<T> CreateMemory(int length) => new(this, length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected Memory<T> CreateMemory(int start, int length) => new(this, start, length);
}

sealed class MemoryDebugView<T>
{
    readonly ReadOnlyMemory<T> _memory;

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public T[] Items => _memory.ToArray();

    public MemoryDebugView(Memory<T> memory) => _memory = memory;

    public MemoryDebugView(ReadOnlyMemory<T> memory) => _memory = memory;
}
#endif
