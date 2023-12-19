namespace LeokaEstetica.Platform.Models.Dto.Output.ProjectManagment;

/// <summary>
/// Класс выходной модели исполнителей и наблюдателей задачи.
/// </summary>
public class TaskPeopleOutput
{
    /// <summary>
    /// Фамилия.
    /// </summary>
    public string LastName { get; set; }

    /// <summary>
    /// Имя.
    /// </summary>
    public string FirstName { get; set; }

    /// <summary>
    /// Отчество.
    /// </summary>
    public string SecondName { get; set; }

    /// <summary>
    /// Полное ФИО.
    /// </summary>
    public string FullName => FirstName + " " + LastName + " " + (SecondName ?? string.Empty);

    /// <summary>
    /// Id пользователя.
    /// </summary>
    public long UserId { get; set; }
}