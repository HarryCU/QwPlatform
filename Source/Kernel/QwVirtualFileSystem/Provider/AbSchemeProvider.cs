using System;
using System.IO;
using QwVirtualFileSystem.IO;

namespace QwVirtualFileSystem.Provider
{
    public abstract class AbstractSchemeProvider : ISchemeProvider
    {
        private readonly string _scheme;

        protected AbstractSchemeProvider(string scheme)
        {
            if (string.IsNullOrWhiteSpace(scheme))
                throw new ArgumentNullException("scheme");
            _scheme = scheme;
            if (!_scheme.EndsWith("://"))
                _scheme = string.Concat(_scheme, "://");
        }

        public string Scheme
        {
            get { return _scheme; }
        }

        public IFolder CreateFolder(string uri)
        {
            if (!uri.StartsWith(Scheme))
                throw new VFSRuntimeException("请求的路径并非{0}开头, 路径: {1}.", Scheme, uri);
            var path = uri.Substring(Scheme.Length);

            if (BeforeBuildFolder(path))
            {
                return BuildFolder(path);
            }
            return null;
        }

        protected virtual bool BeforeBuildFolder(string path)
        {
            return true;
        }

        protected abstract IFolder BuildFolder(string path);
    }
}
