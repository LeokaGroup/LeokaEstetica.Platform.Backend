using System.Data;
using System.Runtime.CompilerServices;
using LeokaEstetica.Platform.Base.Abstractions.Connection;
using LeokaEstetica.Platform.Base.Factors;
using SqlKata.Execution;

[assembly: InternalsVisibleTo("LeokaEstetica.Platform.Tests")]

namespace LeokaEstetica.Platform.Base.Services.Connection;

/// <summary>
/// Класс подключений к БД.
/// </summary>
internal class ConnectionProvider : IConnectionProvider
{
    private readonly IConnectionFactory _connectionFactory;
    private IDbConnection _connection;
    
    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="ConnectionProvider"/>.
    /// </summary>
    /// <param name="connectionFactory"> Фабрика подключений к БД. </param>
    public ConnectionProvider(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }
    
    /// <inheritdoc />
    public async Task<IDbConnection> GetConnectionAsync()
    {
        if (_connection == null || _connection.State is ConnectionState.Closed or ConnectionState.Broken)
        {
            _connection = await _connectionFactory.CreateConnectionAsync();
        }

        return _connection;
    }

    /// <inheritdoc />
    public async Task<IDbConnection> GetConnectionAsync(string connectionString)
    {
        var connection = await _connectionFactory.CreateConnectionAsync(connectionString);

        return connection;
    }

    /// <inheritdoc />
    public async Task<QueryFactory> CreateQueryFactory(IDbConnection connection)
    {
        var result = await _connectionFactory.CreateQueryFactory(connection);

        return result;
    }
}