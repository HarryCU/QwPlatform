// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using System.Collections.Generic;
using System.Linq;

namespace QwMicroKernel.Collections
{
    public struct DeferredValue<T> : IDeferLoadable
    {
        IEnumerable<T> _source;
        bool _loaded;
        T _value;

        public DeferredValue(T value)
        {
            _value = value;
            _source = null;
            _loaded = true;
        }

        public DeferredValue(IEnumerable<T> source)
        {
            _source = source;
            _loaded = false;
            _value = default(T);
        }

        public void Load()
        {
            if (_source != null)
            {
                _value = _source.SingleOrDefault();
                _loaded = true;
            }
        }

        public bool IsLoaded
        {
            get { return _loaded; }
        }

        public bool IsAssigned
        {
            get { return _loaded && _source == null; }
        }

        private void Check()
        {
            if (!IsLoaded)
            {
                Load();
            }
        }

        public T Value
        {
            get
            {
                Check();
                return _value;
            }

            set
            {
                _value = value;
                _loaded = true;
                _source = null;
            }
        }
    }
}