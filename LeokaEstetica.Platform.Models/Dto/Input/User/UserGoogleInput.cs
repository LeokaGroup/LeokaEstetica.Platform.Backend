namespace LeokaEstetica.Platform.Models.Dto.Input.User;

/// <summary>
/// Класс входной модели данных аккаунта Google.
/// </summary>
public class UserGoogleInput
{
    /// <summary>
    /// Email пользователя.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Имя.
    /// </summary>
    public string GivenName { get; set; }
    
    /// <summary>
    /// Фамилия.
    /// </summary>
    public string FamilyName { get; set; }

    /// <summary>
    /// Признак подтверждения почты пользователя.
    /// </summary>
    public bool EmailVerified { get; set; }
}