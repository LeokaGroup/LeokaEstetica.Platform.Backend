using LeokaEstetica.Platform.Models.Enums;

namespace LeokaEstetica.Platform.Models.Dto.Common.Cache.Output;

/// <summary>
/// Класс выходной модели подключения пользователя для всех хабов.
/// </summary>
public class UserConnectionOutput
{
    /// <summary>
    /// Id подключения.
    /// </summary>
    public string? ConnectionId { get; set; }

    /// <summary>
    /// Модуль.
    /// </summary>
    public UserConnectionModuleEnum Module { get; set; }

    /// <summary>
    /// Признак наличия в кэше.
    /// </summary>
    public bool IsCacheExists { get; set; }
}