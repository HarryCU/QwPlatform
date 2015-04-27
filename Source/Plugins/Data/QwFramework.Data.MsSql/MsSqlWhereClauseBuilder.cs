using QwMicroKernel.Data;
using QwMicroKernel.Data.Implements;

namespace QwFramework.Data.MsSql
{
    public class MsSqlWhereClauseBuilder<TModel> : WhereClauseBuilder<TModel>
        where TModel : class, new()
    {
        public MsSqlWhereClauseBuilder()
            : base()
        {
        }

        public MsSqlWhereClauseBuilder(IMappingResolver mappingResolver)
            : base(mappingResolver)
        {
        }

        protected override char ParameterChar
        {
            get { return '@'; }
        }
    }
}
