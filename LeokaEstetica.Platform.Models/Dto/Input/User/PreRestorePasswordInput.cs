namespace LeokaEstetica.Platform.Models.Dto.Input.User;

/// <summary>
/// Класс входной модели перед восстановлением пароля.
/// </summary>
public class PreRestorePasswordInput
{
    /// <summary>
    /// Аккаунт.
    /// </summary>
    public string Account { get; set; }
}