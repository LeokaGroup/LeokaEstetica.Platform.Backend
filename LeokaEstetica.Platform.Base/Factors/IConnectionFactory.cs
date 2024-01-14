using System.Data;

namespace LeokaEstetica.Platform.Base.Factors;

/// <summary>
/// Абстракция фабрики подключений к БД.
/// </summary>
public interface IConnectionFactory
{
    /// <summary>
    /// Метод создает новой подключение к БД.
    /// </summary>
    /// <returns> Асинхронная задача. </returns>
    Task<IDbConnection> CreateConnectionAsync();

    /// <summary>
    /// Метод создает новое подключение использую переданную строку к БД.
    /// </summary>
    /// <param name="connectionString">Строка подключения.</param>
    /// <returns>Новое соединение.</returns>
    Task<IDbConnection> CreateConnectionAsync(string connectionString);

    /// <summary>
    /// Метод создает транзакцию.
    /// </summary>
    /// <returns>Открытую транзакцию.</returns>
    Task<IDbTransaction> CreateTransactionAsync();
}