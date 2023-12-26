namespace LeokaEstetica.Platform.Models.Dto.Output.ProjectManagment;

/// <summary>
/// Класс выходной модели создания задачи.
/// </summary>
public class CreateProjectManagementTaskOutput
{
    /// <summary>
    /// Url редиректа к задачам после создания задачи.
    /// </summary>
    public string RedirectUrl { get; set; }
}