using System.Data;
using SqlKata.Execution;

namespace LeokaEstetica.Platform.Base.Abstractions.Connection;

/// <summary>
/// Абстракция контекста подключений к БД.
/// </summary>
public interface IConnectionProvider
{
    /// <summary>
    /// Метод возвращает контекст подключения к БД.
    /// </summary>
    /// <returns> Асинхронная задача с контекстом подключения. </returns>
    Task<IDbConnection> GetConnectionAsync();

    /// <summary>
    /// Метод возвращает контекст подключения к БД.
    /// </summary>
    /// <param name="connectionString">Строка подключения.</param>
    /// <returns> Асинхронная задача с контекстом подключения. </returns>
    Task<IDbConnection> GetConnectionAsync(string connectionString);

    /// <summary>
    /// Метод создает подключения для SqlKata с нужным провайдером.
    /// </summary>
    /// <param name="connection">Строка подключения.</param>
    /// <returns>QueryFactory-фабрику для подключения.</returns>
    Task<QueryFactory> CreateQueryFactory(IDbConnection connection);
}