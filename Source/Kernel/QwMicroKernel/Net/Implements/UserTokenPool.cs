using System;
using System.Collections.Generic;

namespace QwMicroKernel.Net.Implements
{
    internal sealed class UserTokenPool
    {
        private readonly Stack<UserToken> _pool;

        public UserTokenPool(int capacity)
        {
            _pool = new Stack<UserToken>(capacity);
        }

        public void Push(UserToken item)
        {
            if (item == null)
            {
                throw new ArgumentException("Items added to a AsyncSocketUserToken cannot be null");
            }
            lock (_pool)
            {
                _pool.Push(item);
            }
        }

        public UserToken Pop()
        {
            lock (_pool)
            {
                return _pool.Pop();
            }
        }

        public int Count
        {
            get { return _pool.Count; }
        }
    }
}
