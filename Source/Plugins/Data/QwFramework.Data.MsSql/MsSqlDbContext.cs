using QwMicroKernel.Data;
using QwMicroKernel.Data.Implements;

namespace QwFramework.Data.MsSql
{
    public class MsSqlDbContext : DbContext
    {
        public override IDbSession CreateSession(string connectionString)
        {
            return new MsSqlDbSession(connectionString);
        }

        public override IDbAffairSession CreateAffairSession(string connectionString)
        {
            return new MsSqlDbAffairSession(connectionString);
        }

        public override IDbQuery<TModel> CreateQuery<TModel>(string connectionString)
        {
            return new MsSqlDbQuery<TModel>(connectionString);
        }

        public MsSqlDbContext(string connectionString)
            : base(connectionString)
        {
        }
    }
}
