using System.Data;
using Dapper;
using LeokaEstetica.Platform.Base.Enums;
using Npgsql;
using Enum = LeokaEstetica.Platform.Base.Enums.Enum;

namespace LeokaEstetica.Platform.Base.Handlers;

/// <summary>
/// Обработчик для приведения строковых значений к SQL Enums в Postgres.
/// </summary>
/// <typeparam name="T"> Тип для приведения. </typeparam>
internal class NpgEnumHandler<T> : SqlMapper.TypeHandler<T>
    where T : class, IEnum
{
    /// <inheritdoc/>
    public override void SetValue(IDbDataParameter parameter, T value)
    {
        var par = parameter as NpgsqlParameter;
        par.Value = value.Value;
        par.DataTypeName = value.Type;
    }

    /// <inheritdoc/>
    public override T Parse(object value)
    {
        var enumObj = new Enum();
        enumObj.Value = value.ToString();
        return enumObj as T;
    }
}