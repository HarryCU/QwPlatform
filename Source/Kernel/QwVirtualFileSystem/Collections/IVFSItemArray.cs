using System.Collections.Generic;

namespace QwVirtualFileSystem.Collections
{
    public interface IVFSItemArray<out T> : IEnumerable<T>
    {
        int Count { get; }
    }
}
