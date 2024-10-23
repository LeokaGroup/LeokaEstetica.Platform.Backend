using LeokaEstetica.Platform.Models.Dto.Input.Moderation;
using LeokaEstetica.Platform.Models.Dto.Output.Moderation.Resume;
using LeokaEstetica.Platform.Models.Entities.Moderation;

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
    Task RejectResumeAsync(long profileInfoId, string? account);
    
    /// <summary>
    /// Метод создает замечания анкет. 
    /// </summary>
    /// <param name="createResumeRemarkInput">Входная модель.</param>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Список замечаний анкет.</returns>
    Task<IEnumerable<ResumeRemarkEntity>> CreateResumeRemarksAsync(
        CreateResumeRemarkInput createResumeRemarkInput, string account);
    
    /// <summary>
    /// Метод отправляет замечания вакансии владельцу анкеты.
    /// Отправка замечаний вакансии подразумевает просто изменение статуса замечаниям анкеты.
    /// <param name="profileInfoId">Id анкеты.</param>
    /// <param name="account">Аккаунт.</param>
    /// </summary>
    Task SendResumeRemarksAsync(long profileInfoId, string? account);
    
    /// <summary>
    /// Метод получает список замечаний анкеты (не отправленные), если они есть.
    /// </summary>
    /// <param name="profileInfoId">Id анкеты.</param>
    /// <returns>Список замечаний анкеты.</returns>
    Task<IEnumerable<ResumeRemarkEntity>> GetResumeUnShippedRemarksAsync(long profileInfoId);
}