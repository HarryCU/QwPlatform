using System.Collections.Generic;

namespace QwMicroKernel.Collections
{
    public class QwQueue<T> : Disposer
    {
        private readonly List<T> _items = new List<T>();

        public int Count { get { return _items.Count; } }

        public T this[int index]
        {
            get { return _items[index]; }
        }

        public T Top
        {
            get
            {
                if (Count == 0) return default(T);
                return _items[0];
            }
        }

        public T Dequeue()
        {
            if (Count == 0)
                return default(T);

            var item = this[0];
            _items.RemoveAt(0);
            return item;
        }

        public void Enqueue(T item)
        {
            _items.Insert(0, item);
        }

        public void Clear()
        {
            _items.Clear();
        }

        protected override void Release()
        {
            Clear();
        }
    }
}
