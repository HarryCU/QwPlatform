using QwMicroKernel.Data;
using QwMicroKernel.Data.Implements;

namespace QwFramework.Data.Oracle
{
    public class OracleDbContext : DbContext
    {
        public override IDbSession CreateSession(string connectionString)
        {
            return new OracleDbSession(connectionString);
        }

        public override IDbAffairSession CreateAffairSession(string connectionString)
        {
            return new OracleDbAffairSession(connectionString);
        }

        public override IDbQuery<TModel> CreateQuery<TModel>(string connectionString)
        {
            return new OracleDbQuery<TModel>(connectionString);
        }

        public OracleDbContext(string connectionString)
            : base(connectionString)
        {
        }
    }
}
