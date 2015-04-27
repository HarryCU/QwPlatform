using QwMicroKernel.Plugin;
using QwMicroKernel.Plugin.Implements;

namespace QwMicroKernel.Data
{
    public interface IDbExtendedService : IPluginService
    {
    }

    internal sealed class DbExtendedService : QwPluginService, IDbExtendedService
    {
        public DbExtendedService()
            : base(null)
        {
        }
    }
}
