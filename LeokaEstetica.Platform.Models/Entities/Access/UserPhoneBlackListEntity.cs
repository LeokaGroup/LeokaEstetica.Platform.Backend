namespace LeokaEstetica.Platform.Models.Entities.Access;

/// <summary>
/// Класс сопоставляется с таблицей ЧС номеров телефонов пользователей Access.UserPhoneBlackList.
/// </summary>
public class UserPhoneBlackListEntity
{
    /// <summary>
    /// PK.
    /// </summary>
    public long BlackId { get; set; }

    /// <summary>
    /// Id пользователя.
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// Номер телефона пользователя.
    /// </summary>
    public string PhoneNumber { get; set; }
}