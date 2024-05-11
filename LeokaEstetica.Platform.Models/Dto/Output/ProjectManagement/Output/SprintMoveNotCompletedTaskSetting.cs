namespace LeokaEstetica.Platform.Models.Dto.ProjectManagement.Output;

/// <summary>
/// Класс настроек автоматического перемещения нерешенных задач спринта.
/// </summary>
public class SprintMoveNotCompletedTaskSetting : BaseScrumSetting
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public SprintMoveNotCompletedTaskSetting()
    {
    }
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="name">Название настройки.</param>
    public SprintMoveNotCompletedTaskSetting(string name) : base(name)
    {
    }
}