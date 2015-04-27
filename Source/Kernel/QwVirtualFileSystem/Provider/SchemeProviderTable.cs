using System;
using QwMicroKernel.Collections;

namespace QwVirtualFileSystem.Provider
{
    internal sealed class SchemeProviderTable : QwDictionary<string, ISchemeProvider>
    {
        public void Register(ISchemeProvider provider)
        {
            Add(provider.Scheme.ToUpper(), provider);
        }

        public ISchemeProvider Match(string uri)
        {
            ISchemeProvider schemeProvider = null;
            Each((scheme, provider) =>
            {
                if (uri.StartsWith(scheme, StringComparison.CurrentCultureIgnoreCase))
                {
                    schemeProvider = provider;
                    return true;
                }
                return false;
            });
            return schemeProvider;
        }
    }
}
