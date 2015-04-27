using System.Collections;
using System.Collections.Generic;
using QwVirtualFileSystem.IO;

namespace QwVirtualFileSystem.Collections
{
    internal abstract class AbstractVFSItemArray<T> : IVFSItemArray<T>
        where T : IVFSItem
    {
        private readonly Dictionary<string, T> _items;

        protected AbstractVFSItemArray()
        {
            _items = new Dictionary<string, T>(5);
        }

        public void Add(T item)
        {
            _items.Add(item.Name, item);
        }

        public T FindByName(string name)
        {
            if (_items.ContainsKey(name))
                return _items[name];
            return default(T);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _items.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count
        {
            get { return _items.Count; }
        }
    }
}
