namespace LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;

/// <summary>
/// Класс входной модели обновления названия спринта.
/// </summary>
public class UpdateSprintNameInput : SprintInput
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="sprintName"></param>
    public UpdateSprintNameInput(string sprintName)
    {
        SprintName = sprintName;
    }

    /// <summary>
    /// Новое название спринта.
    /// </summary>
    public string SprintName { get; set; }
}