namespace LeokaEstetica.Platform.Models.Entities.Configs;

/// <summary>
/// Класс сопоставляется с таблицей названий столбцов проектов Configs.ProjectColumnNames.
/// </summary>
public class ProjectColumnNameEntity
{
    /// <summary>
    /// PK.
    /// </summary>
    public long ColumnId { get; set; }

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