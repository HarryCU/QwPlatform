using QwMicroKernel.Data;
using QwMicroKernel.Data.Implements;

namespace QwFramework.Data.Oracle
{
    public class OracleDbQuery<TModel> : DbQuery<TModel> where TModel : class ,new()
    {
        public OracleDbQuery(string connectionString)
            : this(connectionString, EmptyMappingResolver)
        {
        }

        public OracleDbQuery(string connectionString, IMappingResolver resolver)
            : base(resolver, new OracleDbSession(connectionString))
        {
        }

        protected override WhereClauseBuilder<TModel> CreateWhereClauseBuilder()
        {
            return new OracleWhereClauseBuilder<TModel>(Resolver);
        }
    }
}
