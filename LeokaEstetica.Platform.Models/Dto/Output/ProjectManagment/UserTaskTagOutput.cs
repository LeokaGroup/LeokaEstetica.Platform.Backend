namespace LeokaEstetica.Platform.Models.Dto.Output.ProjectManagment;

/// <summary>
/// Класс выходной модели тегов задачи пользователя.
/// </summary>
public class UserTaskTagOutput
{
    /// <summary>
    /// Id тега.
    /// </summary>
    public int TagId { get; set; }

    /// <summary>
    /// Название тега.
    /// </summary>
    public string TagName { get; set; }

    /// <summary>
    /// Системное название тега.
    /// </summary>
    public string TagSysName { get; set; }
    
    /// <summary>
    /// Id пользователя.
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// Описание метки (тега).
    /// </summary>
    public string TagDescription { get; set; }
}