namespace LeokaEstetica.Platform.LuceneNet.Factors;

public static class CreateIndexRamDirectoryFactory
{
    /// <summary>
    /// Метод возвращает экземпляр нужного нам объекта.
    /// </summary>
    /// <typeparam name="TNew">Тип объекта с которым будем работать.</typeparam>
    /// <returns>Экземпляр объекта.</returns>
    public static TNew CreateNew<TNew>() where TNew : new()
    {
        return new TNew();
    }
}