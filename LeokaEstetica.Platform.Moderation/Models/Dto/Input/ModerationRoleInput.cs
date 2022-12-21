namespace LeokaEstetica.Platform.Moderation.Models.Dto.Input;

/// <summary>
/// Класс входной модели ролей модерации.
/// </summary>
public class ModerationRoleInput
{
    /// <summary>
    /// Email пользователя, под которым входим на модерацию.
    /// Передается отдельно для повышенной безопасности.
    /// Не зависит от базовой авторизации.
    /// </summary>
    public string Email { get; set; }
}