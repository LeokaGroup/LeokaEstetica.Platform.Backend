namespace LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;

/// <summary>
/// Класс входной модели обновления описания спринта.
/// </summary>
public class UpdateSprintDetailsInput : SprintInput
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="sprintName">Описание спринта.</param>
    public UpdateSprintDetailsInput(string sprintDetails)
    {
        SprintDetails = sprintDetails;
    }

    /// <summary>
    /// Новое название спринта.
    /// </summary>
    public string SprintDetails { get; set; }
}