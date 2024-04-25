using System.Data;
using System.Runtime.CompilerServices;
using LeokaEstetica.Platform.Base.Handlers;
using LeokaEstetica.Platform.Models.Enums;
using Npgsql;

[assembly: InternalsVisibleTo("LeokaEstetica.Platform.Backend")]
[assembly: InternalsVisibleTo("LeokaEstetica.Platform.ProjectManagement")]
[assembly: InternalsVisibleTo("LeokaEstetica.Platform.Tests")]

namespace LeokaEstetica.Platform.Base.Factors;
 
/// <summary>
/// Фабрика подключений к PostgreSQL.
/// </summary>
internal class NpgSqlConnectionFactory : IConnectionFactory
{
    private readonly string _connectionString;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="connectionString">Строка подключения к БД.</param>
    public NpgSqlConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    /// <summary>
    /// Статический конструктор. Проводит настройки для типов данных для Dapper.
    /// </summary>
    static NpgSqlConnectionFactory()
    {
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

        Dapper.SqlMapper.AddTypeHandler(new NpgEnumHandler<IEnum>());
        Dapper.SqlMapper.AddTypeHandler(new NpgEnumHandler<LeokaEstetica.Platform.Models.Enums.Enum>());
        Dapper.SqlMapper.AddTypeHandler(new NpgJsonHandler());
        Dapper.SqlMapper.AddTypeHandler(typeof(DateTime), new NpgDateTimeHandler());
        Dapper.SqlMapper.AddTypeHandler(typeof(DateTime?), new NpgDateTimeNullableHandler());

        Dapper.SqlMapper.RemoveTypeMap(typeof(DateTime));
        Dapper.SqlMapper.RemoveTypeMap(typeof(DateTime?));
    }

    /// <inheritdoc />
    public async Task<IDbConnection> CreateConnectionAsync()
    {
        var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        return connection;
    }

    /// <inheritdoc />
    public async Task<IDbConnection> CreateConnectionAsync(string connectionString)
    {
        var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        return connection;
    }

    /// <inheritdoc />
    public async Task<IDbTransaction> CreateTransactionAsync()
    {
        var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var transaction = await connection.BeginTransactionAsync();

        return transaction;
    }
}