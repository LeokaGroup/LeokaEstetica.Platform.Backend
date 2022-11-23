namespace LeokaEstetica.Platform.Models.Dto.Output.Project;

/// <summary>
/// Класс выходной модели результатов проектов пользователя.
/// </summary>
public class UserProjectResultOutput
{
    /// <summary>
    /// Список проектов пользователя.
    /// </summary>
    public IEnumerable<UserProjectOutput> UserProjects { get; set; }

    /// <summary>
    /// Кол-во проектов.
    /// </summary>
    public int Total => UserProjects.Count();
}