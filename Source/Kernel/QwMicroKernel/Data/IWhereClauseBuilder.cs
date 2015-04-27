using System;
using System.Linq.Expressions;

namespace QwMicroKernel.Data
{
    public interface IWhereClauseBuilder<T>
            where T : class, new()
    {
        WhereClauseBuildResult BuildWhereClause(Expression<Func<T, bool>> expression);
    }
}
