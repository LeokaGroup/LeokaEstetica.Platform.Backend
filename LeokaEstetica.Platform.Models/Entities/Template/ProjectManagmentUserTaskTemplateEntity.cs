namespace LeokaEstetica.Platform.Models.Entities.Template;

/// <summary>
/// Класс сопоставляется с таблицей шаблонов, которые выбрал пользователь.
/// </summary>
public class ProjectManagmentUserTaskTemplateEntity
{
    /// <summary>
    /// PK.
    /// </summary>
    public long UserTemplateId { get; set; }

    /// <summary>
    /// Id шаблона.
    /// </summary>
    public int TemplateId { get; set; }

    /// <summary>
    /// Признак активности шаблона.
    /// Это поле нужно прежде всего для отображения всех шаблонов пользователя, что он выбирал ранее.
    /// </summary>
    public bool IsActive { get; set; }
    
    /// <summary>
    /// FK на шаблоны задач.
    /// </summary>
    public ProjectManagmentTaskTemplateEntity ProjectManagmentTaskTemplate { get; set; }
}