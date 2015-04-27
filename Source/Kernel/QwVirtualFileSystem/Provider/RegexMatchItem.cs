using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QwMicroKernel;

namespace QwVirtualFileSystem.Provider
{
    public class RegexMatchItem
    {
        public readonly string Name;
        public readonly string Value;

        public RegexMatchItem(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }

    public class RegexMatchItemTable : Disposer
    {
        private readonly List<RegexMatchItem> _items = new List<RegexMatchItem>();

        public void Add(string name, string value)
        {
            _items.Add(new RegexMatchItem(name, value));
        }

        public string GetValue(string name)
        {
            var item = _items.FirstOrDefault(m => m.Name.Equals(name));
            if (item == null)
                return null;
            return item.Value;
        }

        protected override void Release()
        {
            _items.Clear();
        }
    }
}
