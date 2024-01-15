using System.Data;
using System.Runtime.CompilerServices;
using LeokaEstetica.Platform.Base.Enums;
using LeokaEstetica.Platform.Base.Handlers;
using Npgsql;
using SqlKata.Compilers;
using SqlKata.Execution;
using Enum = LeokaEstetica.Platform.Base.Enums.Enum;

[assembly: InternalsVisibleTo("LeokaEstetica.Platform.Backend")]
[assembly: InternalsVisibleTo("LeokaEstetica.Platform.ProjectManagment")]
[assembly: InternalsVisibleTo("LeokaEstetica.Platform.Tests")]

namespace LeokaEstetica.Platform.Base.Factors;

/// <summary>
/// Фабрика подключений к PostgreSQL.
/// </summary>
internal class NpgSqlConnectionFactory : IConnectionFactory
{
    private readonly string _connectionString;
    // private readonly IConfiguration _configuration;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="connectionString">Строка подключения к БД.</param>
    /// <param name="configuration">Конфигурация приложения.</param>
    public NpgSqlConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
        // _configuration = configuration;
    }

    /// <summary>
    /// Статический конструктор. Проводит настройки для типов данных для Dapper.
    /// </summary>
    static NpgSqlConnectionFactory()
    {
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

        Dapper.SqlMapper.AddTypeHandler(new NpgEnumHandler<IEnum>());
        Dapper.SqlMapper.AddTypeHandler(new NpgEnumHandler<Enum>());
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

    /// <inheritdoc />
    public async Task<QueryFactory> CreateQueryFactory(IDbConnection connection)
    {
        // string connection = null;
        //
        // if (_configuration["Environment"].Equals("Development"))
        // {
        //     connection = _configuration["ConnectionStrings:NpgDevSqlConnection"];
        // }
        //
        // if (_configuration["Environment"].Equals("Staging"))
        // {
        //     connection = _configuration["ConnectionStrings:NpgTestSqlConnection"];
        // }
        //
        // if (_configuration["Environment"].Equals("Production"))
        // {
        //     connection = _configuration["ConnectionStrings:NpgSqlConnection"];
        // }

        // IDbConnection dbConnection = new NpgsqlConnection(connection);
        // var compiler = new PostgresCompiler();
        // var result = new QueryFactory(dbConnection, compiler);

        // var connection = await CreateConnectionAsync();
        // var conn = new NpgsqlConnection(connection);
        var compiler = new SqlServerCompiler();
        var result = new QueryFactory((NpgsqlConnection)connection, compiler);

        return await Task.FromResult(result);
    }
}