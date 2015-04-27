namespace QwMicroKernel.Data
{
    public interface IDbContext
    {
        IDbSession CreateSession();
        IDbAffairSession CreateAffairSession();
        IDbQuery<TModel> CreateQuery<TModel>() where TModel : class ,new();

        IDbSession CreateSession(string connectionString);
        IDbAffairSession CreateAffairSession(string connectionString);
        IDbQuery<TModel> CreateQuery<TModel>(string connectionString) where TModel : class ,new();
    }
}
