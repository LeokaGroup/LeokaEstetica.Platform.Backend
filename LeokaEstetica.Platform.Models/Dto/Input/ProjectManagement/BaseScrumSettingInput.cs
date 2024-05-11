namespace LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;

/// <summary>
/// Класс базовой модели настроек проекта.
/// </summary>
public class BaseScrumSettingInput
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="sysName">Системное название настройки.</param>
    public BaseScrumSettingInput(string sysName)
    {
        SysName = sysName;
    }

    /// <summary>
    /// Id проекта.
    /// </summary>
    public long ProjectId { get; set; }

    /// <summary>
    /// Признак выбранной настройки.
    /// </summary>
    public bool IsSettingSelected { get; set; }

    /// <summary>
    /// Системное название настройки.
    /// </summary>
    public string SysName { get; set; }
}