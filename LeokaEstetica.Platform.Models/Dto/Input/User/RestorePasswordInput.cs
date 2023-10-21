namespace LeokaEstetica.Platform.Models.Dto.Input.User;

/// <summary>
/// Класс входной модели восстановления пароля.
/// </summary>
public class RestorePasswordInput
{
    /// <summary>
    /// Новый пароль.
    /// </summary>
    public string RestorePassword { get; set; }

    /// <summary>
    /// Пользователь, который восстанавливает пароль.
    /// </summary>
    public string UserName { get; set; }
}