using LeokaEstetica.Platform.Models.Dto.Output.Moderation.Resume;

namespace LeokaEstetica.Platform.Moderation.Abstractions.Resume;

/// <summary>
/// Абстракция модерации анкет пользователей.
/// </summary>
public interface IResumeModerationService
{
    /// <summary>
    /// Метод получает список анкет для модерации.
    /// </summary>
    /// <returns>Список анкет.</returns>
    Task<ResumeModerationResult> ResumesModerationAsync();
}