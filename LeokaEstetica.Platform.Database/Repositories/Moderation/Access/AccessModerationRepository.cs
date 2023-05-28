using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Core.Enums;
using LeokaEstetica.Platform.Database.Abstractions.Moderation.Access;
using Microsoft.EntityFrameworkCore;

namespace LeokaEstetica.Platform.Database.Repositories.Moderation.Access;

/// <summary>
/// Абстракция репозитория проверки доступа к модерации.
/// </summary>
public class AccessModerationRepository : IAccessModerationRepository
{
    private readonly PgContext _pgContext;

    /// <summary>
    /// Список разрешенных ролей для допуска в КЦ.
    /// </summary>
    private readonly List<int> _moderationRoles = new()
    {
        (int)ModerationRoleEnum.Moderator,
        (int)ModerationRoleEnum.Administrator
    };
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="pgContext">Датаконтекст.</param>
    public AccessModerationRepository(PgContext pgContext)
    {
        _pgContext = pgContext;
    }

    #region Публичные методы.

    /// <summary>
    /// Метод проверяет доступ пользователя к КЦ.
    /// </summary>
    /// <param name="email">Почта.</param>
    /// <param name="password">Пароль.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Признак результата проверки.</returns>
    public async Task<bool> CheckAccessUserRoleModerationAsync(long userId)
    {
        var isRole = await _pgContext.ModerationUserRoles
            .AnyAsync(m => m.UserId == userId
                           && _moderationRoles.Contains(m.RoleId));

        return isRole;
    }
    
    /// <summary>
    /// Метод получает хэш пароля для проверки пользователя.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Хэш пароля.</returns>
    public async Task<string> GetPasswordHashByEmailAsync(long userId)
    {
        var result = await _pgContext.ModerationUsers
            .Where(u => u.UserId == userId)
            .Select(u => u.PasswordHash)
            .FirstOrDefaultAsync();

        return result;
    }

    #endregion
}