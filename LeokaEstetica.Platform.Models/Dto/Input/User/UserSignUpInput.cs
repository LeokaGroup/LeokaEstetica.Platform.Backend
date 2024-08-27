using LeokaEstetica.Platform.Models.Enums;

namespace LeokaEstetica.Platform.Models.Dto.Input.User;

/// <summary>
/// Класс входной модели пользователя для регистрации.
/// </summary>
public class UserSignUpInput
{
    /// <summary>
    /// Email.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Пароль пользователя. Он не хранится в БД. Хранится только его хэш.
    /// </summary>
    public string? Password { get; set; }
    
    /// <summary>
    /// Компонентная роль пользователя.
    /// </summary>
    public string? ComponentRole { get; set; }
}