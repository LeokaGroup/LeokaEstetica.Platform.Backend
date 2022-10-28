using ProtoBuf;

namespace LeokaEstetica.Platform.Redis.Models;

/// <summary>
/// Класс модели профиля пользователя списка элементов для работы с кэшем Redis.
/// </summary>
[ProtoContract]
public class ProfileMenuItemsRedis
{
    /// <summary>
    /// Название пункта.
    /// </summary>
    [ProtoMember(1)]
    public string Label { get; set; }

    /// <summary>
    /// Список элементов.
    /// </summary>
    [ProtoMember(2)]
    public List<Items> Items { get; set; }
    
    /// <summary>
    /// Системное название.
    /// </summary>
    [ProtoMember(3)]
    public string SysName { get; set; }

    /// <summary>
    /// Путь.
    /// </summary>
    [ProtoMember(4)]
    public string Url { get; set; }
}

/// <summary>
/// Класс вложенных элементов списка меню.
/// </summary>
[ProtoContract]
public class Items
{
    /// <summary>
    /// Название.
    /// </summary>
    [ProtoMember(1)]
    public string Label { get; set; }

    /// <summary>
    /// Системное название.
    /// </summary>
    [ProtoMember(2)]
    public string SysName { get; set; }

    /// <summary>
    /// Путь.
    /// </summary>
    [ProtoMember(3)]
    public string Url { get; set; }
}