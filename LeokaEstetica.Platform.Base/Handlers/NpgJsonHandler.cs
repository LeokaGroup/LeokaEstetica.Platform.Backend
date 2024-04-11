using System.Data;
using Dapper;
using Newtonsoft.Json.Linq;
using Npgsql;
using NpgsqlTypes;

namespace LeokaEstetica.Platform.Base.Handlers;

/// <summary>
/// Обработчик для конвертации JObject <> JSONB.
/// </summary>
internal class NpgJsonHandler : SqlMapper.TypeHandler<JObject>
{
    /// <inheritdoc/>
    public override JObject Parse(object value)
    {
        var json = (string)value;
        return JObject.Parse(json);
    }

    /// <inheritdoc/>
    public override void SetValue(IDbDataParameter parameter, JObject value)
    {
        parameter.Value = value?.ToString(Newtonsoft.Json.Formatting.None);
        ((NpgsqlParameter)parameter).NpgsqlDbType = NpgsqlDbType.Jsonb;
    }
}