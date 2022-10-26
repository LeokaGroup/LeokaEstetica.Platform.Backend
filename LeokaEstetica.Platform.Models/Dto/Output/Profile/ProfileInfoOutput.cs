using LeokaEstetica.Platform.Models.Entities.Profile;

namespace LeokaEstetica.Platform.Models.Dto.Output.Profile;

/// <summary>
/// Класс выходной модели информации профиля пользователя для раздела обо мне.
/// </summary>
public class ProfileInfoOutput : ProfileInfoEntity
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