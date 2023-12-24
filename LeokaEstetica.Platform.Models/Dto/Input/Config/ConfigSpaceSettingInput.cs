namespace LeokaEstetica.Platform.Models.Dto.Input.Config;

/// <summary>
/// Класс входной модели настроек рабочего пространства модуля УП.
/// </summary>
public class ConfigSpaceSettingInput
{
    /// <summary>
    /// Стратегия представления.
    /// </summary>
    public string Strategy { get; set; }

    /// <summary>
    /// Id шаблона.
    /// </summary>
    public int TemplateId { get; set; }

    /// <summary>
    /// Id проекта.
    /// </summary>
    public long ProjectId { get; set; }
}