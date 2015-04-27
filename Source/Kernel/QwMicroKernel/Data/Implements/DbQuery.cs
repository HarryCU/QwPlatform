using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using QwMicroKernel.Reflection;

namespace QwMicroKernel.Data.Implements
{
    public abstract class DbQuery<TModel> : IDbQuery<TModel> where TModel : class ,new()
    {
        protected static readonly IMappingResolver EmptyMappingResolver = new EmptyMappingResolver();
        private readonly IDbSession _session;
        private readonly IMappingResolver _resolver;
        private readonly SqlString _sqlString;

        protected IDbSession Session
        {
            get { return _session; }
        }

        protected SqlString Builder
        {
            get { return _sqlString; }
        }

        protected IMappingResolver Resolver
        {
            get { return _resolver; }
        }

        protected DbQuery(IDbSession session)
            : this(EmptyMappingResolver, session)
        { }

        protected DbQuery(IMappingResolver resolver, IDbSession session)
        {
            if (resolver == null) throw new ArgumentNullException("resolver");
            if (session == null) throw new ArgumentNullException("session");

            _resolver = resolver;
            _sqlString = new SqlString();
            _session = session;
        }

        protected abstract WhereClauseBuilder<TModel> CreateWhereClauseBuilder();

        public IDbQuery<TModel> Select(Func<TModel, object> expression)
        {
            if (expression == null) throw new ArgumentNullException("expression");
            var model = expression(new TModel());
            var properties = ReflectionHelper.AllProperty(model.GetType(), ReflectionHelper.DefBindingFlags | BindingFlags.DeclaredOnly);

            if (properties.Count == 0) throw new NotImplementedException();

            var modelType = typeof(TModel);
            var tableName = modelType.Name;
            var tableAliasName = Resolver.ResolveAliasName<TModel>(modelType);

            Builder.Dispose();
            Builder.Add("SELECT");
            Builder.Add(" ");
            var index = 0;
            foreach (var property in properties)
            {
                var aliasName = Resolver.ResolveAliasName<TModel>(property.Member);
                if (tableName != tableAliasName)
                {
                    Builder.Add(tableAliasName).Add(".");
                }
                if (aliasName != property.Name)
                {
                    Builder.Add(" AS [").AddColumn(aliasName).Add("]");
                }
                else
                {
                    Builder.AddColumn(property.Name);
                }
                if (index != properties.Count - 1)
                    Builder.Add(",");
                index++;
            }
            Builder.Add(" ");
            Builder.Add("FORM ");
            Builder.Add(tableName);
            if (tableName != tableAliasName)
            {
                Builder.Add(" AS ").Add(tableAliasName);
            }
            return this;
        }

        public IDbQuery<TModel> Where(Expression<Func<TModel, bool>> expression)
        {
            if (expression == null) throw new ArgumentNullException("expression");
            var whereBuilder = CreateWhereClauseBuilder();
            var result = whereBuilder.BuildWhereClause(expression);

            Builder.Add(" ")
                .Add("WHERE")
                .Add(" ")
                .Add(result.WhereClause);

            foreach (var parameter in result.ParameterValues)
            {
                Builder.AddParameter(Session.CreateParameter(parameter.Key, parameter.Value));
            }

            return this;
        }

        public IDbQuery<TModel> OrderBy<TProperty>(Expression<Func<TModel, TProperty>> expression, bool desc = false)
        {
            if (expression == null) throw new ArgumentNullException("expression");

            var body = expression.Body as MemberExpression;
            if (body == null) throw new Exception();

            var aliasName = Resolver.ResolveAliasName<TModel>(body.Member);
            Builder.Add(" ")
                .Add("ORDER BY")
                .Add(" ")
                .Add(aliasName)
                .Add(" ")
                .Add(desc ? "DESC" : "ASC");

            return this;
        }

        public TModel ToSingle()
        {
            var sqlString = Builder.ToString();
            var parameters = Builder.GetParameters();

            using (var reader = Session.QueryReader(sqlString, parameters.ToArray()))
            {
                if (reader.Read())
                {
                    var model = new TModel();

                    var properties = ReflectionHelper.AllProperty(typeof(TModel), ReflectionHelper.DefBindingFlags | BindingFlags.DeclaredOnly);
                    foreach (var property in properties)
                    {
                        var aliasName = Resolver.ResolveAliasName<TModel>(property.Member);
                        if (!Builder.HasColumn(aliasName))
                            continue;
                        var index = reader.GetOrdinal(aliasName);
                        if (reader.IsDBNull(index))
                            continue;
                        var value = reader.GetValue(index);
                        property.SetValue(model, value);
                    }

                    return model;
                }
            }
            return null;
        }

        public IList<TModel> ToList()
        {
            var sqlString = Builder.ToString();
            var parameters = Builder.GetParameters();

            var dt = Session.Query(sqlString, parameters.ToArray());
            if (dt.Rows.Count == 0) return null;

            var list = new List<TModel>();
            var properties = ReflectionHelper.AllProperty(typeof(TModel), ReflectionHelper.DefBindingFlags | BindingFlags.DeclaredOnly);
            foreach (DataRow row in dt.Rows)
            {
                var model = new TModel();
                foreach (var property in properties)
                {
                    var aliasName = Resolver.ResolveAliasName<TModel>(property.Member);
                    if (!Builder.HasColumn(aliasName))
                        continue;
                    var value = row[aliasName];
                    if (value == DBNull.Value || value == null)
                        continue;
                    property.SetValue(model, value);
                }
                list.Add(model);
            }
            return list;
        }

        public SqlString ToSqlString()
        {
            return Builder;
        }
    }
}
