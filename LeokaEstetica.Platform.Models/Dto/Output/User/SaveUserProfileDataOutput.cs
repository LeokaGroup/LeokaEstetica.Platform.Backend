namespace LeokaEstetica.Platform.Models.Dto.Output.User;

/// <summary>
/// Класс выходной модели сохранения информации профиля пользователя.
/// </summary>
public class SaveUserProfileDataOutput
{
    /// <summary>
    /// Признак изменения почты пользователя. Нужно для повторного создания токена и релогина в системе.
    /// </summary>
    public bool IsEmailChanged { get; set; }
}