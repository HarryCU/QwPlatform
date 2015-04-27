namespace QwMicroKernel.Data
{
    public interface IDbContextFactory
    {
        IDbContext CreateContext();
    }
}
