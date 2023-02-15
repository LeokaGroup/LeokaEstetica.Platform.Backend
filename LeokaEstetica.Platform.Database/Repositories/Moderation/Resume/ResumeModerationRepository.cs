using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Core.Enums;
using LeokaEstetica.Platform.Database.Abstractions.Moderation.Resume;
using LeokaEstetica.Platform.Models.Entities.Moderation;
using LeokaEstetica.Platform.Models.Entities.Profile;
using LeokaEstetica.Platform.Models.Entities.Project;
using Microsoft.EntityFrameworkCore;

namespace LeokaEstetica.Platform.Database.Repositories.Moderation.Resume;

/// <summary>
/// Класс реализует методы репозитория модерации анкет пользователей.
/// </summary>
public class ResumeModerationRepository : IResumeModerationRepository
{
    private readonly PgContext _pgContext;
    
    /// <summary>
    /// Конструтор.
    /// </summary>
    /// <param name="pgContext">Датаконтекст.</param>
    public ResumeModerationRepository(PgContext pgContext)
    {
        _pgContext = pgContext;
    }

    /// <summary>
    /// Метод получает список анкет для модерации.
    /// </summary>
    /// <returns>Список анкет.</returns>
    public async Task<IEnumerable<ModerationResumeEntity>> ResumesModerationAsync()
    {
        var result = await _pgContext.ModerationResumes
            .Include(r => r.ProfileInfo)
            .Where(p => p.ModerationStatus.StatusId == (int)ResumeModerationStatusEnum.ModerationResume)
            .Select(p => new ModerationResumeEntity
            {
                ModerationId = p.ModerationId,
                ProfileInfoId = p.ProfileInfoId,
                ProfileInfo = new ProfileInfoEntity
                {
                    Aboutme = p.ProfileInfo.Aboutme,
                    FirstName = p.ProfileInfo.FirstName,
                    IsShortFirstName = p.ProfileInfo.IsShortFirstName,
                    Job = p.ProfileInfo.Job,
                    LastName = p.ProfileInfo.LastName,
                    Patronymic = p.ProfileInfo.Patronymic,
                    UserId = p.ProfileInfo.UserId,
                    Telegram = p.ProfileInfo.Telegram,
                    Vkontakte = p.ProfileInfo.Vkontakte,
                    WhatsApp = p.ProfileInfo.WhatsApp
                },
                DateModeration = p.DateModeration
            })
            .ToListAsync();

        return result;
    }
}