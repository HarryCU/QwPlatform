using QwMicroKernel.Data;
using QwMicroKernel.Data.Implements;

namespace QwFramework.Data.Sqlite
{
    public class SqliteWhereClauseBuilder<TModel> : WhereClauseBuilder<TModel>
        where TModel : class, new()
    {
        public SqliteWhereClauseBuilder()
            : base()
        {
        }

        public SqliteWhereClauseBuilder(IMappingResolver mappingResolver)
            : base(mappingResolver)
        {
        }

        protected override char ParameterChar
        {
            get { return '@'; }
        }
    }
}
