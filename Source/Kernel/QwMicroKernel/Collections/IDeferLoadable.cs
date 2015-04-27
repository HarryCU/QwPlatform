namespace QwMicroKernel.Collections
{
    /// <summary>
    /// Data interface for controlling defer-loadable types
    /// </summary>
    public interface IDeferLoadable
    {
        bool IsLoaded { get; }
        void Load();
    }
}