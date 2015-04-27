using QwMicroKernel.Data;
using QwMicroKernel.Data.Implements;

namespace QwFramework.Data.Sqlite
{
    public class SqliteDbContext : DbContext
    {
        public override IDbSession CreateSession(string connectionString)
        {
            return new SqliteDbSession(connectionString);
        }

        public override IDbAffairSession CreateAffairSession(string connectionString)
        {
            return new SqliteDbAffairSession(connectionString);
        }

        public override IDbQuery<TModel> CreateQuery<TModel>(string connectionString)
        {
            return new SqliteDbQuery<TModel>(connectionString);
        }

        public SqliteDbContext(string connectionString)
            : base(connectionString)
        {
        }
    }
}
