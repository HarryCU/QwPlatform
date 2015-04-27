namespace QwMicroKernel.Collections
{
    public interface IScopedDictionary<in TKey, TValue>
    {
        void Add(TKey key, TValue value);
        bool Remove(TKey key);
        bool Remove(TKey key, TValue value);
        bool TryGetValue(TKey key, out TValue value);
        bool ContainsKey(TKey key);
        TValue this[TKey key] { get; set; }
    }
}
