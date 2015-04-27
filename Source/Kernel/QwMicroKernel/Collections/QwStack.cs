using System.Collections.Generic;

namespace QwMicroKernel.Collections
{
    public class QwStack<T> : Disposer
    {
        private readonly List<T> _items = new List<T>();

        public int Count { get { return _items.Count; } }

        public T this[int index]
        {
            get { return _items[index]; }
        }

        public void Push(T item)
        {
            _items.Add(item);
        }

        public T Pop()
        {
            var top = Top;
            _items.RemoveAt(Count - 1);
            return top;
        }

        public void Pop(int count)
        {
            _items.RemoveRange(Count - count, count);
        }

        public void PopUntil(int finalCount)
        {
            if (finalCount < Count)
                Pop(Count - finalCount);
        }

        public T Top
        {
            get
            {
                if (Count == 0) return default(T);
                return _items[Count - 1];
            }
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
