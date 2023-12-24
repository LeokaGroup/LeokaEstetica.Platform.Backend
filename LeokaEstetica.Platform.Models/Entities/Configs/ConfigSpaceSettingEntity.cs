namespace LeokaEstetica.Platform.Models.Entities.Configs;

/// <summary>
/// Класс сопоставляется с таблицей настроек проекта рабочего пространства.
/// </summary>
public class ConfigSpaceSettingEntity
{
    /// <summary>
    /// PK.
    /// </summary>
    public long ConfigId { get; set; }

    /// <summary>
    /// Id проекта.
    /// </summary>
    public long ProjectId { get; set; }

    /// <summary>
    /// Id пользователя.
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// Ключ параметра (название параметра).
    /// </summary>
    public string ParamKey { get; set; }

    /// <summary>
    /// Значение параметра.
    /// </summary>
    public string ParamValue { get; set; }

    /// <summary>
    /// Тип параметра.
    /// </summary>
    public string ParamType { get; set; }

    /// <summary>
    /// Описание параметра.
    /// </summary>
    public string ParamDescription { get; set; }

    /// <summary>
    /// Тег, который классифицирует параметр.
    /// </summary>
    public string ParamTag { get; set; }
}