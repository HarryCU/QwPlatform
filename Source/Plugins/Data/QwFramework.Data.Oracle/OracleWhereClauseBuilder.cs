using QwMicroKernel.Data;
using QwMicroKernel.Data.Implements;

namespace QwFramework.Data.Oracle
{
    public class OracleWhereClauseBuilder<TModel> : WhereClauseBuilder<TModel>
        where TModel : class, new()
    {
        public OracleWhereClauseBuilder()
            : base()
        {
        }

        public OracleWhereClauseBuilder(IMappingResolver mappingResolver)
            : base(mappingResolver)
        {
        }

        protected override char ParameterChar
        {
            get { return ':'; }
        }
    }
}
