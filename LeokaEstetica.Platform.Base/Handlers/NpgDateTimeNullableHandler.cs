using System.Data;
using Dapper;

namespace LeokaEstetica.Platform.Base.Handlers;

/// <summary>
/// Обработчик для приведения значений DateTime к UTC формату для сохранения в базе данных.
/// </summary>
internal class NpgDateTimeNullableHandler : SqlMapper.TypeHandler<DateTime?>
{
    /// <inheritdoc/>
    public override void SetValue(IDbDataParameter parameter, DateTime? value)
    {
        if (value.HasValue)
        {
            parameter.Value = value.Value.Kind == DateTimeKind.Utc
                ? value.Value
                : DateTime.SpecifyKind(value.Value, DateTimeKind.Utc);
        }
        else
        {
            parameter.Value = null;
        }
    }

    /// <inheritdoc/>
    public override DateTime? Parse(object value)
    {
        return (value is null || value is DBNull)
            ? null
            : DateTime.SpecifyKind(((DateTime?)value).Value, DateTimeKind.Utc);
    }
}