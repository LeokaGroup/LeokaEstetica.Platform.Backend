using LeokaEstetica.Platform.Models.Enums;
using ProtoBuf;

namespace LeokaEstetica.Platform.Models.Dto.Common.Cache;

/// <summary>
/// Класс модели для хранения элементов меню хидера для работы с кэшем Redis.
/// </summary>
[ProtoContract]
public class HeaderMenuRedis
{
    /// <summary>
    /// Название пункта меню хидера.
    /// </summary>
    [ProtoMember(1)]
    public string MenuItemTitle { get; set; }

    /// <summary>
    /// Ссылка на роут.
    /// </summary>
    [ProtoMember(2)]
    public string MenuItemUrl { get; set; }
}