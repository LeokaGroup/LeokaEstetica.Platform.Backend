namespace LeokaEstetica.Platform.Models.Entities.Template;

/// <summary>
///  TODO: Скорее всего надо будет расширять эту таблу.
/// Класс сопоставляется с таблицей шаблонов, которые выбрал пользователь.
/// </summary>
public class ProjectManagmentUserTaskTemplateEntity
{
    /// <summary>
    /// PK.
    /// </summary>
    public long UserTemplateId { get; set; }

    /// <summary>
    /// Признак активности шаблона.
    /// Это поле нужно прежде всего для отображения всех шаблонов пользователя, что он выбирал ранее.
    /// </summary>
    public bool IsActive { get; set; }
}