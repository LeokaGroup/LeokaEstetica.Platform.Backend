using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Core.Enums;
using LeokaEstetica.Platform.Database.Abstractions.Moderation.Access;
using Microsoft.EntityFrameworkCore;

namespace LeokaEstetica.Platform.Database.Repositories.Moderation.Access;

/// <summary>
/// Абстракция репозитория проверки доступа к модерации.
/// </summary>
public sealed class AccessModerationRepository : IAccessModerationRepository
{
    private readonly PgContext _pgContext;
    
    public AccessModerationRepository(PgContext pgContext)
    {
        _pgContext = pgContext;
    }

    /// <summary>
    /// Метод првоеряет доступ пользователя к модерации.
    /// Идет проверка на наличие допустимой роли.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Признак результата проверки.</returns>
    public async Task<bool> CheckAccessUserRoleModerationAsync(long userId)
    {
        var isRole = await _pgContext.ModerationUserRoles
            .AnyAsync(m => m.UserId == userId
                           && new[] { (int)ModerationRoleEnum.Moderator, (int)ModerationRoleEnum.Administrator }
                               .Contains(m.RoleId));

        return isRole;
    }
}