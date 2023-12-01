namespace LeokaEstetica.Platform.Models.Entities.Template;

/// <summary>
/// Класс сопоставляется с таблицей шаблонов, которые выбрал пользователь.
/// </summary>
public class ProjectManagmentUserTaskTemplateEntity
{
    /// <summary>
    /// Id пользователя.
    /// </summary>
    public long UserId { get; set; }
    
    /// <summary>
    /// Id шаблона.
    /// </summary>
    public int TemplateId { get; set; }

    /// <summary>
    /// Признак активности шаблона.
    /// Это поле нужно прежде всего для отображения всех шаблонов пользователя, что он выбирал ранее.
    /// </summary>
    public bool IsActive { get; set; }
}