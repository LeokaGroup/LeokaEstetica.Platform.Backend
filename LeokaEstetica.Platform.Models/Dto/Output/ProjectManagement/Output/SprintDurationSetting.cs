namespace LeokaEstetica.Platform.Models.Dto.ProjectManagement.Output;

/// <summary>
/// Класс настроек длительности спринта.
/// </summary>
public class SprintDurationSetting : BaseScrumSetting
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public SprintDurationSetting()
    {
    }

    public SprintDurationSetting(string name) : base(name)
    {
    }
}