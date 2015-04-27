using System;
using System.IO;

namespace QwVirtualFileSystem.IO
{
    internal abstract class AbstractVFSItem : IVFSItem
    {
        private readonly string _fullName;
        private readonly string _name;

        protected AbstractVFSItem(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new VFSRuntimeException(new ArgumentNullException("fullName"), "{0}是非法路径.", fullName);

            _fullName = fullName;
            _name = Path.GetFileName(fullName);
            if (string.IsNullOrWhiteSpace(_name))
                _name = fullName;
        }

        public string Name
        {
            get { return _name; }
        }

        public string FullName
        {
            get { return _fullName; }
        }
    }
}
