using LeokaEstetica.Platform.Models.Enums;

namespace LeokaEstetica.Platform.Models.Dto.Input.Project;

/// <summary>
/// Класс входной модели проекта.
/// </summary>
public class ProjectInput
{
    /// <summary>
    /// Название проекта.
    /// </summary>
    public string ProjectName { get; set; }

    /// <summary>
    /// Описание проекта.
    /// </summary>
    public string ProjectDetails { get; set; }

    /// <summary>
    /// Изображение проекта.
    /// </summary>
    public string ProjectIcon { get; set; }

    /// <summary>
    /// Id проекта.
    /// </summary>
    public long ProjectId { get; set; }

    /// <summary>
    /// Стадия проекта.
    /// </summary>
    public string ProjectStage { get; set; }

    /// <summary>
    /// Условия.
    /// </summary>
    public string Conditions { get; set; }

    /// <summary>
    /// Требования.
    /// </summary>
    public string Demands { get; set; }
    
    /// <summary>
    /// Аккаунт.
    /// </summary>
    public string Account { get; set; }

    /// <summary>
    /// Токен.
    /// </summary>
    public string Token { get; set; }

    /// <summary>
    /// Стадия проекта в виде перечисления.
    /// </summary>
    public ProjectStageEnum ProjectStageEnum => Enum.Parse<ProjectStageEnum>(ProjectStage);

    /// <summary>
    /// Id пользователя.
    /// </summary>
    public long UserId { get; set; }
}