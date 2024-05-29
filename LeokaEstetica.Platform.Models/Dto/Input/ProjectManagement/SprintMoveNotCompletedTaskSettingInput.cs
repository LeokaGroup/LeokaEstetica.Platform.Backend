namespace LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;

/// <summary>
/// Класс входной модели настроек длительности спринта проекта.
/// </summary>
public class SprintMoveNotCompletedTaskSettingInput : BaseScrumSettingInput
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="sysName">Системное название настройки.</param>
    public SprintMoveNotCompletedTaskSettingInput(string sysName) : base(sysName)
    {
    }
}