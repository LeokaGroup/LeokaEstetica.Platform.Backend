using System.Data;
using SqlKata.Execution;

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
    /// <param name="connection">Строка подключения.</param>
    /// <returns>Новое соединение.</returns>
    Task<IDbConnection> CreateConnectionAsync(string connection);

    /// <summary>
    /// Метод создает транзакцию.
    /// </summary>
    /// <returns>Открытую транзакцию.</returns>
    Task<IDbTransaction> CreateTransactionAsync();

    /// <summary>
    /// Метод создает подключения для SqlKata с нужным провайдером.
    /// </summary>
    /// <param name="connection">Строка подключения.</param>
    /// <returns>QueryFactory-фабрику для подключения.</returns>
    Task<QueryFactory> CreateQueryFactory(IDbConnection connection);
}