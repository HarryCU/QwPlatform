using QwMicroKernel.Data;
using QwMicroKernel.Data.Implements;

namespace QwFramework.Data.Sqlite
{
    public class SqliteDbQuery<TModel> : DbQuery<TModel> where TModel : class ,new()
    {
        public SqliteDbQuery(string connectionString)
            : this(connectionString, EmptyMappingResolver)
        {
        }

        public SqliteDbQuery(string connectionString, IMappingResolver resolver)
            : base(resolver, new SqliteDbSession(connectionString))
        {
        }

        protected override WhereClauseBuilder<TModel> CreateWhereClauseBuilder()
        {
            return new SqliteWhereClauseBuilder<TModel>(Resolver);
        }
    }
}
