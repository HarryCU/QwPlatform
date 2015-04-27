using System;
using QwMicroKernel;
using QwVirtualFileSystem.Cache;
using QwVirtualFileSystem.IO;
using QwVirtualFileSystem.Provider;
using QwVirtualFileSystem.Provider.Embedded;

namespace QwVirtualFileSystem
{
    public sealed class VFSRuntime : Disposer
    {
        private static readonly SchemeProviderTable SchemeProviders = new SchemeProviderTable();

        static VFSRuntime()
        {
            SchemeProviders.Register(new EmbeddedProvider());
        }

        private readonly IVFSCache _cache;

        private IVFSCache Cache
        {
            get { return _cache; }
        }

        public VFSRuntime()
            : this(new MemoryCache())
        {
        }

        public VFSRuntime(IVFSCache cache)
        {
            if (cache == null)
                throw new ArgumentNullException("cache");
            _cache = cache;
        }

        public void SchemeRegister(ISchemeProvider provider)
        {
            SchemeProviders.Register(provider);
        }

        public IFolder Query(string uri)
        {
            var provider = SchemeProviders.Match(uri);
            if (provider == null)
                throw new VFSRuntimeException("未注册此路径()类型的适配器.", uri);
            var folder = provider.CreateFolder(uri);
            Cache.Update(folder.FullName, folder);
            return folder;
        }

        protected override void Release()
        {
            Cache.Clear();
        }
    }
}
