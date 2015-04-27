using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace QwMicroKernel.Data
{
    public interface IDbQuery<TModel> where TModel : class ,new()
    {
        IDbQuery<TModel> Select(Func<TModel, object> handler);
        IDbQuery<TModel> Where(Expression<Func<TModel, bool>> expression);
        IDbQuery<TModel> OrderBy<TProperty>(Expression<Func<TModel, TProperty>> expression, bool desc = false);
        SqlString ToSqlString();
        TModel ToSingle();
        IList<TModel> ToList();
    }
}
