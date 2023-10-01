namespace LeokaEstetica.Platform.Models.Dto.Output.Configs;

/// <summary>
/// Класс выходной модели названий столбцов таблицы команд проектов.
/// </summary>
public class ProjectTeamColumnNameOutput
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