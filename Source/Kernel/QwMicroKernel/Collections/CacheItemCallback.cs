namespace QwMicroKernel.Collections
{
    public delegate void CacheItemCallback<TKey, TValue>(TKey key, TValue value);
}