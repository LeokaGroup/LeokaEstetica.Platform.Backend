namespace LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;

/// <summary>
/// Класс входной модели настроек длительности спринта проекта.
/// </summary>
public class SprintDurationSettingInput : BaseScrumSettingInput
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="sysName">Системное название настройки.</param>
    public SprintDurationSettingInput(string sysName) : base(sysName)
    {
    }
}