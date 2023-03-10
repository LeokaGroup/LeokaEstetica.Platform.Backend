namespace LeokaEstetica.Platform.Models.Dto.Input.User;

/// <summary>
/// Класс входной модели авторизации через Google.
/// </summary>
public class UserSignInGoogleInput
{
    /// <summary>
    /// Токен с данными аккаунта, который генерит Google.
    /// </summary>
    public string GoogleAuthToken { get; set; }
}