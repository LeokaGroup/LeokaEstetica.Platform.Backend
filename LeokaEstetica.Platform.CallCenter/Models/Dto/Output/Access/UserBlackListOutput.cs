namespace LeokaEstetica.Platform.CallCenter.Models.Dto.Output.Access;

/// <summary>
/// Класс выходной модели ЧС пользователей.
/// </summary>
public class UserBlackListOutput
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