using QwMicroKernel.Data;
using QwMicroKernel.Data.Implements;

namespace QwFramework.Data.MsSql
{
    public class MsSqlDbQuery<TModel> : DbQuery<TModel> where TModel : class ,new()
    {
        public MsSqlDbQuery(string connectionString)
            : this(connectionString, EmptyMappingResolver)
        {
        }

        public MsSqlDbQuery(string connectionString, IMappingResolver resolver)
            : base(resolver, new MsSqlDbSession(connectionString))
        {
        }

        protected override WhereClauseBuilder<TModel> CreateWhereClauseBuilder()
        {
            return new MsSqlWhereClauseBuilder<TModel>(Resolver);
        }
    }
}
