namespace LeokaEstetica.Platform.Models.Dto.Output.Moderation.Project;

/// <summary>
/// Класс списка выходной модели проектов, ожидающих проверки замечаний.
/// </summary>
public class AwaitingCorrectionProjectResult
{
    /// <summary>
    /// Список проектов.
    /// </summary>
    public Dictionary<long, List<ProjectRemarkOutput>> AwaitingCorrectionProjects { get; set; }

    /// <summary>
    /// Кол-во.
    /// </summary>
    public int Total => AwaitingCorrectionProjects.Count();
}