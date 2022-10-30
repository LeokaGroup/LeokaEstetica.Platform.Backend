namespace LeokaEstetica.Platform.Models.Dto.Output.User;

/// <summary>
/// Класс выходной модели для получения почты и номера телефона пользователя.
/// </summary>
public class UserPhoneEmailOutput
{
    /// <summary>
    /// Email.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Номер телефона пользователя.
    /// </summary>
    public string PhoneNumber { get; set; }
}