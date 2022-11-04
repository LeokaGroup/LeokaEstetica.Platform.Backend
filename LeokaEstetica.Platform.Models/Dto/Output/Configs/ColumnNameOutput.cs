namespace LeokaEstetica.Platform.Models.Dto.Output.Configs;

/// <summary>
/// Класс выходной модели названий столбцов.
/// </summary>
public class ColumnNameOutput
{
    /// <summary>
    /// Название столбца.
    /// </summary>
    public string ColumnName { get; set; }

    /// <summary>
    /// Название таблицы.
    /// </summary>
    public string TableName { get; set; }

    /// <summary>
    /// Позиция.
    /// </summary>
    public int Position { get; set; }
}