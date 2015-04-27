using System;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;

namespace QwMicroKernel.Text
{
    public class StringBuilder : Disposer, ISerializable
    {
        private readonly System.Text.StringBuilder _builder;

        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public StringBuilder()
        {
            _builder = new System.Text.StringBuilder();
        }

        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public StringBuilder(int capacity)
        {
            _builder = new System.Text.StringBuilder(capacity);
        }

        public StringBuilder(string value)
        {
            _builder = new System.Text.StringBuilder(value);
        }

        public StringBuilder(int capacity, int maxCapacity)
        {
            _builder = new System.Text.StringBuilder(capacity, maxCapacity);
        }

        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public StringBuilder(string value, int capacity)
        {
            _builder = new System.Text.StringBuilder(value, capacity);
        }

        [SecuritySafeCritical]
        public StringBuilder(string value, int startIndex, int length, int capacity)
        {
            _builder = new System.Text.StringBuilder(value, startIndex, length, capacity);
        }

        public int Capacity
        {
            get { return _builder.Capacity; }
            set { _builder.Capacity = value; }
        }

        public int Length
        {
            get { return _builder.Length; }
            set { _builder.Length = value; }
        }

        public int MaxCapacity
        {
            get { return _builder.MaxCapacity; }
        }

        public char this[int index]
        {
            get { return _builder[index]; }
            set { _builder[index] = value; }
        }

        public StringBuilder Append(bool value)
        {
            _builder.Append(value);
            return this;
        }

        public StringBuilder Append(byte value)
        {
            _builder.Append(value);
            return this;
        }

        public StringBuilder Append(char value)
        {
            _builder.Append(value);
            return this;
        }

        [SecuritySafeCritical]
        public StringBuilder Append(char[] value)
        {
            _builder.Append(value);
            return this;
        }

        public StringBuilder Append(decimal value)
        {
            _builder.Append(value);
            return this;
        }

        public StringBuilder Append(double value)
        {
            _builder.Append(value);
            return this;
        }

        public StringBuilder Append(float value)
        {
            _builder.Append(value);
            return this;
        }

        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public StringBuilder Append(int value)
        {
            _builder.Append(value);
            return this;
        }

        public StringBuilder Append(long value)
        {
            _builder.Append(value);
            return this;
        }

        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public StringBuilder Append(object value)
        {
            _builder.Append(value);
            return this;
        }

        [CLSCompliant(false)]
        public StringBuilder Append(sbyte value)
        {
            _builder.Append(value);
            return this;
        }

        public StringBuilder Append(short value)
        {
            _builder.Append(value);
            return this;
        }

        [SecuritySafeCritical]
        public StringBuilder Append(string value)
        {
            _builder.Append(value);
            return this;
        }

        [CLSCompliant(false)]
        public StringBuilder Append(uint value)
        {
            _builder.Append(value);
            return this;
        }

        [CLSCompliant(false)]
        public StringBuilder Append(ulong value)
        {
            _builder.Append(value);
            return this;
        }

        [CLSCompliant(false)]
        public StringBuilder Append(ushort value)
        {
            _builder.Append(value);
            return this;
        }

        public StringBuilder Append(char value, int repeatCount)
        {
            _builder.Append(value, repeatCount);
            return this;
        }

        [SecuritySafeCritical]
        public StringBuilder Append(char[] value, int startIndex, int charCount)
        {
            _builder.Append(value, startIndex, charCount);
            return this;
        }

        [SecuritySafeCritical]
        public StringBuilder Append(string value, int startIndex, int count)
        {
            _builder.Append(value, startIndex, count);
            return this;
        }

        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public StringBuilder AppendFormat(string format, object arg0)
        {
            _builder.AppendFormat(format, arg0);
            return this;
        }

        public StringBuilder AppendFormat(string format, params object[] args)
        {
            _builder.AppendFormat(format, args);
            return this;
        }

        public StringBuilder AppendFormat(IFormatProvider provider, string format, params object[] args)
        {
            _builder.AppendFormat(provider, format, args);
            return this;
        }

        public StringBuilder AppendFormat(string format, object arg0, object arg1)
        {
            _builder.AppendFormat(format, arg0, arg1);
            return this;
        }

        public StringBuilder AppendFormat(string format, object arg0, object arg1, object arg2)
        {
            _builder.AppendFormat(format, arg0, arg1, arg2);
            return this;
        }

        [ComVisible(false)]
        public StringBuilder AppendLine()
        {
            _builder.AppendLine();
            return this;
        }

        [ComVisible(false)]
        public StringBuilder AppendLine(string value)
        {
            _builder.AppendLine(value);
            return this;
        }

        public StringBuilder Clear()
        {
            _builder.Clear();
            return this;
        }

        [ComVisible(false)]
        [SecuritySafeCritical]
        public void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count)
        {
            _builder.CopyTo(sourceIndex, destination, destinationIndex, count);
        }

        public int EnsureCapacity(int capacity)
        {
            return _builder.EnsureCapacity(capacity);
        }

        public bool Equals(StringBuilder sb)
        {
            return _builder.Equals(sb._builder);
        }

        public StringBuilder Insert(int index, bool value)
        {
            _builder.Insert(index, value);
            return this;
        }

        public StringBuilder Insert(int index, byte value)
        {
            _builder.Insert(index, value);
            return this;
        }

        [SecuritySafeCritical]
        public StringBuilder Insert(int index, char value)
        {
            _builder.Insert(index, value);
            return this;
        }

        public StringBuilder Insert(int index, char[] value)
        {
            _builder.Insert(index, value);
            return this;
        }

        public StringBuilder Insert(int index, decimal value)
        {
            _builder.Insert(index, value);
            return this;
        }

        public StringBuilder Insert(int index, double value)
        {
            _builder.Insert(index, value);
            return this;
        }

        public StringBuilder Insert(int index, float value)
        {
            _builder.Insert(index, value);
            return this;
        }

        public StringBuilder Insert(int index, int value)
        {
            _builder.Insert(index, value);
            return this;
        }

        public StringBuilder Insert(int index, long value)
        {
            _builder.Insert(index, value);
            return this;
        }

        public StringBuilder Insert(int index, object value)
        {
            _builder.Insert(index, value);
            return this;
        }

        [CLSCompliant(false)]
        public StringBuilder Insert(int index, sbyte value)
        {
            _builder.Insert(index, value);
            return this;
        }

        public StringBuilder Insert(int index, short value)
        {
            _builder.Insert(index, value);
            return this;
        }

        [SecuritySafeCritical]
        public StringBuilder Insert(int index, string value)
        {
            _builder.Insert(index, value);
            return this;
        }

        [CLSCompliant(false)]
        public StringBuilder Insert(int index, uint value)
        {
            _builder.Insert(index, value);
            return this;
        }

        [CLSCompliant(false)]
        public StringBuilder Insert(int index, ulong value)
        {
            _builder.Insert(index, value);
            return this;
        }

        [CLSCompliant(false)]
        public StringBuilder Insert(int index, ushort value)
        {
            _builder.Insert(index, value);
            return this;
        }

        [SecuritySafeCritical]
        public StringBuilder Insert(int index, string value, int count)
        {
            _builder.Insert(index, value, count);
            return this;
        }

        [SecuritySafeCritical]
        public StringBuilder Insert(int index, char[] value, int startIndex, int charCount)
        {
            _builder.Insert(index, value, startIndex, charCount);
            return this;
        }

        public StringBuilder Remove(int startIndex, int length)
        {
            _builder.Remove(startIndex, length);
            return this;
        }

        public StringBuilder Replace(char oldChar, char newChar)
        {
            _builder.Replace(oldChar, newChar);
            return this;
        }

        public StringBuilder Replace(string oldValue, string newValue)
        {
            _builder.Replace(oldValue, newValue);
            return this;
        }

        public StringBuilder Replace(char oldChar, char newChar, int startIndex, int count)
        {
            _builder.Replace(oldChar, newChar, startIndex, count);
            return this;
        }

        public StringBuilder Replace(string oldValue, string newValue, int startIndex, int count)
        {
            _builder.Replace(oldValue, newValue, startIndex, count);
            return this;
        }

        [SecuritySafeCritical]
        public override string ToString()
        {
            return _builder.ToString();
        }

        [SecuritySafeCritical]
        public string ToString(int startIndex, int length)
        {
            return _builder.ToString(startIndex, length);
        }

        [SecurityCritical]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            info.AddValue("m_MaxCapacity", MaxCapacity);
            info.AddValue("Capacity", Capacity);
            info.AddValue("m_StringValue", ToString());
            info.AddValue("m_currentThread", 0);
        }

        protected override void Release()
        {
            if (_builder != null && Length != 0)
                Clear();
        }
    }
}