// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using System;
using System.Collections;
using System.Collections.Generic;

namespace QwMicroKernel.Collections
{
    /// <summary>
    /// A list implementation that is loaded the first time the contents are examined
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DeferredList<T> : IDeferredList<T>, ICollection<T>, IEnumerable<T>, IList, ICollection, IEnumerable, IDeferLoadable
    {
        readonly IEnumerable<T> _source;
        List<T> _values;

        public DeferredList(IEnumerable<T> source)
        {
            this._source = source;
        }

        public void Load()
        {
            this._values = new List<T>(this._source);
        }

        public bool IsLoaded
        {
            get { return this._values != null; }
        }

        private void Check()
        {
            if (!this.IsLoaded)
            {
                this.Load();
            }
        }

        #region IList<T> Members

        public int IndexOf(T item)
        {
            this.Check();
            return this._values.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            this.Check();
            this._values.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            this.Check();
            this._values.RemoveAt(index);
        }

        public T this[int index]
        {
            get
            {
                this.Check();
                return this._values[index];
            }
            set
            {
                this.Check();
                this._values[index] = value;
            }
        }

        #endregion

        #region ICollection<T> Members

        public void Add(T item)
        {
            this.Check();
            this._values.Add(item);
        }

        public void Clear()
        {
            this.Check();
            this._values.Clear();
        }

        public bool Contains(T item)
        {
            this.Check();
            return this._values.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            this.Check();
            this._values.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { this.Check(); return this._values.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            this.Check();
            return this._values.Remove(item);
        }

        #endregion

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            this.Check();
            return this._values.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region IList Members

        public int Add(object value)
        {
            this.Check();
            return ((IList)this._values).Add(value);
        }

        public bool Contains(object value)
        {
            this.Check();
            return ((IList)this._values).Contains(value);
        }

        public int IndexOf(object value)
        {
            this.Check();
            return ((IList)this._values).IndexOf(value);
        }

        public void Insert(int index, object value)
        {
            this.Check();
            ((IList)this._values).Insert(index, value);
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        public void Remove(object value)
        {
            this.Check();
            ((IList)this._values).Remove(value);
        }

        object IList.this[int index]
        {
            get
            {
                this.Check();
                return ((IList)this._values)[index];
            }
            set
            {
                this.Check();
                ((IList)this._values)[index] = value;
            }
        }

        #endregion

        #region ICollection Members

        public void CopyTo(Array array, int index)
        {
            this.Check();
            ((IList)this._values).CopyTo(array, index);
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get { return null; }
        }

        #endregion
    }
}