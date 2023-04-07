using LeokaEstetica.Platform.Models.Dto.Output.Moderation.Resume;

namespace LeokaEstetica.Platform.CallCenter.Abstractions.Resume;

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

    /// <summary>
    /// Метод одобряет анкету на модерации.
    /// </summary>
    /// <param name="profileInfoId">Id анкеты.</param>
    Task ApproveResumeAsync(long profileInfoId);
    
    /// <summary>
    /// Метод отклоняет анкету на модерации.
    /// </summary>
    /// <param name="profileInfoId">Id анкеты.</param>
    Task RejectResumeAsync(long profileInfoId);
}