using LeokaEstetica.Platform.Models.Entities.Moderation;

namespace LeokaEstetica.Platform.Database.Abstractions.Moderation.Resume;

/// <summary>
/// Абстракция репозитория модерации анкет пользователей.
/// </summary>
public interface IResumeModerationRepository
{
    /// <summary>
    /// Метод получает список анкет для модерации.
    /// </summary>
    /// <returns>Список анкет.</returns>
    Task<IEnumerable<ModerationResumeEntity>> ResumesModerationAsync();
    
    /// <summary>
    /// Метод отправляет анкету на модерацию. Это происходит через добавление в таблицу модерации анкет.
    /// Если анкета в этой таблице, значит она не прошла еще модерацию. 
    /// </summary>
    /// <param name="profileInfoId">Id анкеты.</param>
    Task AddResumeModerationAsync(long profileInfoId);

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