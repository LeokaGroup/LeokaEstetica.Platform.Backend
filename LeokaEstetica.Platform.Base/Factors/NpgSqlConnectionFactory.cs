using System.Data;
using LeokaEstetica.Platform.Base.Enums;
using LeokaEstetica.Platform.Base.Handlers;
using Npgsql;
using Enum = LeokaEstetica.Platform.Base.Enums.Enum;

namespace LeokaEstetica.Platform.Base.Factors;

internal class NpgSqlConnectionFactory : IConnectionFactory
{
     private readonly string _connectionString;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="NpgSqlConnectionFactory"/>.
        /// </summary>
        /// <param name="connectionString"> Строка подключения к БД. </param>
        public NpgSqlConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <inheritdoc />
        public async Task<IDbConnection> CreateConnectionAsync()
        {
            var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            return connection;
        }

        /// <summary>
        /// Метод создает новое подключение использую переданную строку к БД.
        /// </summary>
        /// <param name="connectionString">Строка подключения.</param>
        /// <returns>Новое соединение.</returns>
        public async Task<IDbConnection> CreateConnectionAsync(string connectionString)
        {
            var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            return connection;
        }

        /// <summary>
        /// Метод создает транзакцию.
        /// </summary>
        /// <returns>Открытую транзакцию.</returns>
        public async Task<IDbTransaction> CreateTransactionAsync()
        {
            var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var transaction = await connection.BeginTransactionAsync();

            return transaction;
        }

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
}