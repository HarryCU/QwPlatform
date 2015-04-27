using System;

namespace QwMicroKernel.Data.Implements
{
    public abstract class DbContext : IDbContext
    {
        private readonly string _connectionString;

        protected DbContext(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString)) 
                throw new ArgumentNullException("connectionString");
            _connectionString = connectionString;
        }

        public IDbSession CreateSession()
        {
            return CreateSession(_connectionString);
        }

        public IDbAffairSession CreateAffairSession()
        {
            return CreateAffairSession(_connectionString);
        }

        public IDbQuery<TModel> CreateQuery<TModel>() where TModel : class, new()
        {
            return CreateQuery<TModel>(_connectionString);
        }

        public abstract IDbSession CreateSession(string connectionString);

        public abstract IDbAffairSession CreateAffairSession(string connectionString);

        public abstract IDbQuery<TModel> CreateQuery<TModel>(string connectionString) where TModel : class, new();
    }
}
