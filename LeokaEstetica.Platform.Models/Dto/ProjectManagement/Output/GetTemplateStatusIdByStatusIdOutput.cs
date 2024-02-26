namespace LeokaEstetica.Platform.Models.Dto.ProjectManagement.Output;

/// <summary>
/// Класс выходной модели для получения Id статусов шаблона по Id статусов.
/// </summary>
public class GetTemplateStatusIdByStatusIdOutput
{
    /// <summary>
    /// Id статуса.
    /// </summary>
    public long StatusId { get; set; }

    /// <summary>
    /// Id шаблона.
    /// </summary>
    public int TemplateId { get; set; }
}