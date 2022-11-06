using LeokaEstetica.Platform.Models.Dto.Common;

namespace LeokaEstetica.Platform.Models.Dto.Output.User;

/// <summary>
/// Класс выходной модели авторизации.
/// </summary>
public class UserSignInOutput : IFrontError
{
    /// <summary>
    /// Email.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Токен пользователя.
    /// </summary>
    public string Token { get; set; }

    /// <summary>
    /// Флаг успеха авторизации.
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Код пользователя.
    /// </summary>
    public Guid UserCode { get; set; }

    /// <summary>
    /// Список ошибок.
    /// </summary>
    public List<string> Errors { get; set; } = new();
}