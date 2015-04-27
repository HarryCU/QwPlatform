using System;
using QwMicroKernel.Text;

namespace QwVirtualFileSystem
{
    public sealed class VFSPath
    {
        public static readonly string PathSeparator = "/";

        public static string GetFileName(string path)
        {
            return path.Substring(path.LastIndexOf(PathSeparator, StringComparison.Ordinal) + 1);
        }

        public static string GetFolderPath(string path)
        {
            return path.Substring(0, path.LastIndexOf(PathSeparator, StringComparison.Ordinal));
        }

        public static VFSPathItem Parse(string path)
        {
            var items = path.Split(PathSeparator[0]);
            VFSPathItem root = null, next = null;
            var lastIndex = items.Length - 1;
            for (int i = lastIndex; i >= 0; i--)
            {
                if (i == 0)
                {
                    root = new VFSPathItem(items[i], items[i], next);
                }
                else
                {
                    next = new VFSPathItem(string.Join(PathSeparator, items, 0, i + 1), items[i], next, lastIndex == i);
                }
            }

            return root;
        }
    }

    public sealed class VFSPathItem
    {
        private readonly VFSPathItem _next;
        public readonly string Name;
        public readonly string FullName;
        internal readonly bool IsFileName;

        internal VFSPathItem(string fullName, string name, VFSPathItem next, bool isFileName = false)
        {
            Name = name;
            _next = next;
            FullName = fullName;
            IsFileName = isFileName;
        }

        public VFSPathItem GetNext()
        {
            return _next;
        }

        public string GetNextValue()
        {
            if (_next == null)
                return null;
            return _next.Name;
        }

        public string ToFolderPath()
        {
            if (IsFileName)
                return FullName.Substring(0, FullName.LastIndexOf(VFSPath.PathSeparator, StringComparison.Ordinal));
            return FullName;
        }

        public override string ToString()
        {
            return FullName;
        }
    }
}
