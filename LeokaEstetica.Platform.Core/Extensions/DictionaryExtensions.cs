namespace LeokaEstetica.Platform.Core.Extensions;

/// <summary>
/// Класс расширений словарей.
/// </summary>
public static class DictionaryExtensions
{
    /// <summary>
    /// Метод получает значение словаря по ключу.
    /// </summary>
    /// <param name="source">Источник (т.е. словарь, в котором будет поиск).</param>
    /// <param name="key">Ключ, по которому будет поиск.</param>
    /// <typeparam name="TKey">Ключ.</typeparam>
    /// <typeparam name="TValue">Значение</typeparam>
    /// <returns>Найденное значение по ключу. Может вернуть NULL!</returns>
    public static TValue? TryGet<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key)
    {
        source.TryGetValue(key, out var value);
        
        return value;
    }
}