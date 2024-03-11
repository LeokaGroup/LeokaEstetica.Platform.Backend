namespace LeokaEstetica.Platform.Models.Dto.Output.ProjectManagment;

/// <summary>
/// Класс выходной модели эпиков, доступных к добавлению в них задач.
/// </summary>
public class AvailableEpicOutput
{
    /// <summary>
    /// PK.
    /// Id эпика.
    /// </summary>
    public long EpicId { get; set; }

    /// <summary>
    /// Название эпика.
    /// </summary>
    public string EpicName { get; set; }
}