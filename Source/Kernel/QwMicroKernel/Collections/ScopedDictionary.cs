// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using System.Collections.Generic;

namespace QwMicroKernel.Collections
{
    public class ScopedDictionary<TKey, TValue> : IScopedDictionary<TKey, TValue>
    {
        readonly ScopedDictionary<TKey, TValue> _previous;
        readonly Dictionary<TKey, TValue> _map;

        public ScopedDictionary()
            : this(null)
        {
        }

        public ScopedDictionary(ScopedDictionary<TKey, TValue> previous)
        {
            _previous = previous;
            _map = new Dictionary<TKey, TValue>();
        }

        public ScopedDictionary(ScopedDictionary<TKey, TValue> previous, IEnumerable<KeyValuePair<TKey, TValue>> pairs)
            : this(previous)
        {
            foreach (var p in pairs)
            {
                _map.Add(p.Key, p.Value);
            }
        }

        public void Add(TKey key, TValue value)
        {
            _map.Add(key, value);
        }

        public bool Remove(TKey key)
        {
            for (ScopedDictionary<TKey, TValue> scope = this; scope != null; scope = scope._previous)
            {
                if (scope._map.ContainsKey(key))
                {
                    scope._map.Remove(key);
                    return true;
                }
            }
            return false;
        }

        public bool Remove(TKey key, TValue value)
        {
            for (ScopedDictionary<TKey, TValue> scope = this; scope != null; scope = scope._previous)
            {
                if (scope._map.ContainsKey(key) && scope._map[key].Equals(value))
                {
                    scope._map.Remove(key);
                    return true;
                }
            }
            return false;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            for (ScopedDictionary<TKey, TValue> scope = this; scope != null; scope = scope._previous)
            {
                if (scope._map.TryGetValue(key, out value))
                    return true;
            }
            value = default(TValue);
            return false;
        }

        public bool ContainsKey(TKey key)
        {
            for (ScopedDictionary<TKey, TValue> scope = this; scope != null; scope = scope._previous)
            {
                if (scope._map.ContainsKey(key))
                    return true;
            }
            return false;
        }

        public TValue this[TKey key]
        {
            get
            {
                TValue value;
                TryGetValue(key, out value);
                return value;
            }
            set
            {
                bool used = false;
                for (ScopedDictionary<TKey, TValue> scope = this; scope != null; scope = scope._previous)
                {
                    if (scope._map.ContainsKey(key))
                    {
                        used = true;
                        scope._map[key] = value;
                    }
                }
                if (!used)
                {
                    Add(key, value);
                }
            }
        }
    }
}