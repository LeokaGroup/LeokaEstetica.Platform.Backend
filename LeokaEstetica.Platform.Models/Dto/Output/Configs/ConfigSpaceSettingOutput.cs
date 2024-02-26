namespace LeokaEstetica.Platform.Models.Dto.Output.Configs;

/// <summary>
/// Класс выходной модели настроек рабочего пространства модуля УП.
/// </summary>
public class ConfigSpaceSettingOutput
{
    /// <summary>
    /// Id проекта.
    /// </summary>
    public long? ProjectId { get; set; }

    /// <summary>
    /// Признак фиксации настроек проекта.
    /// </summary>
    public bool IsCommitProjectSettings { get; set; }

    /// <summary>
    /// Путь для редиректа к управлению проектом.
    /// </summary>
    public string ProjectManagmentSpaceUrl { get; set; }
}