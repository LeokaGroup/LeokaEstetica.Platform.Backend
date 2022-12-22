namespace LeokaEstetica.Platform.Models.Dto.Output.Moderation.Project;

/// <summary>
/// Класс выходной модели проекта модерации.
/// </summary>
public class UserProjectModerationOutput
{
    /// <summary>
    /// Название проекта.
    /// </summary>
    public string ProjectName { get; set; }
    
    /// <summary>
    /// Дата создания проекта.
    /// </summary>
    public string DateCreated { get; set; }
}