namespace LeokaEstetica.Platform.Core.Extensions;

public static class DictionaryExtensions
{
    public static TValue TryGet<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key)
    {
        source.TryGetValue(key, out var value);
        
        return value;
    }
}