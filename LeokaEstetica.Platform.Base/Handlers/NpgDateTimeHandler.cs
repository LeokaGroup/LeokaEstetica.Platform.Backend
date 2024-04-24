using System.Data;
using Dapper;

namespace LeokaEstetica.Platform.Base.Handlers;

/// <summary>
/// Обработчик для приведения значений DateTime к UTC формату для сохранения в базе данных.
/// </summary>
internal class NpgDateTimeHandler : SqlMapper.TypeHandler<DateTime>
{
    /// <inheritdoc/>
    public override void SetValue(IDbDataParameter parameter, DateTime value)
    {
        parameter.Value = value.Kind == DateTimeKind.Utc ? value : DateTime.SpecifyKind(value, DateTimeKind.Utc);
    }

    /// <inheritdoc/>
    public override DateTime Parse(object value)
    {
        return DateTime.SpecifyKind((DateTime)value, DateTimeKind.Utc);
    }
}