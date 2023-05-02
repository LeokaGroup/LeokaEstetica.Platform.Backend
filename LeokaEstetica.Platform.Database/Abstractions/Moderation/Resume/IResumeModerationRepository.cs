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
    
    /// <summary>
    /// Метод создает замечания анкеты.
    /// </summary>
    /// <param name="createResumeRemarkInput">Список замечаний.</param>
    Task CreateResumeRemarksAsync(IEnumerable<ResumeRemarkEntity> createResumeRemarkInput);
    
    /// <summary>
    /// Метод обновляет замечания анкеты.
    /// </summary>
    /// <param name="resumeRemarks">Список замечаний для обновления.</param>
    Task UpdateResumeRemarksAsync(List<ResumeRemarkEntity> resumeRemarks);
    
    /// <summary>
    /// Метод получает замечания анкеты, которые ранее были сохранены модератором.
    /// </summary>
    /// <param name="profileInfoId">Id анкеты.</param>
    /// <param name="fields">Список названий полей.</param>
    /// <returns>Список замечаний.</returns>
    Task<List<ResumeRemarkEntity>> GetExistsResumeRemarksAsync(long profileInfoId, IEnumerable<string> fields);
    
    /// <summary>
    /// Метод отправляет замечания вакансии владельцу анкеты.
    /// Отправка замечаний вакансии подразумевает просто изменение статуса замечаниям анкеты.
    /// <param name="profileInfoId">Id анкеты.</param>
    /// </summary>
    Task SendResumeRemarksAsync(long profileInfoId);

    /// <summary>
    /// Метод проверяет, были ли сохранены замечания анкеты.
    /// </summary>
    /// <param name="profileInfoId">Id анкеты.</param>
    /// <returns>Признак раннего сохранения замечаний.</returns>
    Task<bool> CheckResumeRemarksAsync(long profileInfoId);
}