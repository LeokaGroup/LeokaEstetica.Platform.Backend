using System.Data;

namespace LeokaEstetica.Platform.Base.Abstractions.Connection;

/// <summary>
/// Абстракция контекста подключений к БД.
/// </summary>
public interface IConnectionProvider
{
    /// <summary>
    /// Метод возвращает контекст подключения к БД.
    /// </summary>
    /// <returns>Подключение к БД.</returns>
    Task<IDbConnection> GetConnectionAsync();
    
    /// <summary>
    /// Метод создает транзакцию.
    /// </summary>
    /// <param name="isolationLevel">Уровень изоляции</param>
    /// <returns> Асинхронная задача с контекстом подключения. </returns>
    Task<IDbTransaction> CreateTransactionAsync(IsolationLevel isolationLevel);

    /// <summary>
    /// Метод возвращает контекст подключения к БД.
    /// </summary>
    /// <param name="connectionString">Строка подключения.</param>
    /// <returns>Подключение к БД.</returns>
    Task<IDbConnection> GetConnectionAsync(string connectionString);
}