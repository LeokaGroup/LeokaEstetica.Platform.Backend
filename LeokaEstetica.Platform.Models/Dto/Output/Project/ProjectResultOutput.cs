using System.Data;

namespace LeokaEstetica.Platform.Models.Dto.Output.Project;

/// <summary>
/// Класс выходной модели списка проектов пользователя.
/// </summary>
public class ProjectResultOutput
{
    /// <summary>
    /// Список проектов пользователя.
    /// </summary>
    public DataTable Projects { get; set; } = new("MyProjects");

    /// <summary>
    /// Кол-во строк.
    /// </summary>
    public long Total => Projects.Rows.Count;
}