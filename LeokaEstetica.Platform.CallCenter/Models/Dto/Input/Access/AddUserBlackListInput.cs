namespace LeokaEstetica.Platform.CallCenter.Models.Dto.Input.Access;

/// <summary>
/// Класс входной модели добавления пользователя в ЧС.
/// </summary>
public class AddUserBlackListInput
{
    /// <summary>
    /// Id пользователя.
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// Почта.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Номер телефона.
    /// </summary>
    public string PhoneNumber { get; set; }
}