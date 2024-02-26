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
    /// <returns> Асинхронная задача с контекстом подключения. </returns>
    Task<IDbConnection> GetConnectionAsync();

    /// <summary>
    /// Метод возвращает контекст подключения к БД.
    /// </summary>
    /// <param name="connectionString">Строка подключения.</param>
    /// <returns> Асинхронная задача с контекстом подключения. </returns>
    Task<IDbConnection> GetConnectionAsync(string connectionString);
}