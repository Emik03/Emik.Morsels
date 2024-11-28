// SPDX-License-Identifier: MPL-2.0

// ReSharper disable CheckNamespace EmptyNamespace
namespace System.Buffers;
#if !(NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) || NO_SYSTEM_MEMORY
interface IMemoryOwner<T> : IDisposable
{
    Memory<T> Memory { get; }
}

interface IPinnable
{
    void Unpin();

    MemoryHandle Pin(int elementIndex);
}
#endif
