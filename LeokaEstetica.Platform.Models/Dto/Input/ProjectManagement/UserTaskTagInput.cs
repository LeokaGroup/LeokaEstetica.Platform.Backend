namespace LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;

/// <summary>
/// Класс входной модели тегов задачи пользователя.
/// </summary>
public class UserTaskTagInput
{
    /// <summary>
    /// Название тега.
    /// </summary>
    public string TagName { get; set; }

    /// <summary>
    /// Описание метки (тега).
    /// </summary>
    public string TagDescription { get; set; }
}