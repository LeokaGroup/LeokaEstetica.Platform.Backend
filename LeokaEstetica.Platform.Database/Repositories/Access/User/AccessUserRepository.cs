using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Database.Access.User;
using LeokaEstetica.Platform.Models.Entities.Profile;
using Microsoft.EntityFrameworkCore;

namespace LeokaEstetica.Platform.Database.Repositories.Access.User;

/// <summary>
/// Класс реализует методы репозитория проверки доступа пользователей. 
/// </summary>
internal sealed class AccessUserRepository : IAccessUserRepository
{
    private readonly PgContext _pgContext;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="pgContext">Датаконтекст.</param>
    public AccessUserRepository(PgContext pgContext)
    {
        _pgContext = pgContext;
    }

    /// <summary>
    /// Метод проверяет блокировку пользователя по параметру, который передали.
    /// Поочередно проверяем по почте, номеру телефона.
    /// </summary>
    /// <param name="availableBlockedText">Почта или номер телефона для проверки блокировки.</param>
    /// <param name="isVkAuth">Признак блокировки через ВК.</param>
    /// <returns>Признак блокировки.</returns>
    public async Task<bool> CheckBlockedUserAsync(string availableBlockedText, bool isVkAuth)
    {
        // Если пользователь регался через ВК, то надо проверять на блокировку учитывая это.
        // Дальше идти не нужно, если найдена блокировка тут.
        if (isVkAuth)
        {
            var blockedVkUser = await _pgContext.UserVkBlackList
                .AnyAsync(u => u.VkUserId == long.Parse(availableBlockedText));

            if (blockedVkUser)
            {
                return true;
            }
        }

        // Проверяем блокировку пользрвателя по почте.
        var blockedEmailUser = await _pgContext.UserEmailBlackList
            .AnyAsync(u => u.Email.Equals(availableBlockedText));

        if (blockedEmailUser)
        {
            return true;
        }

        // Проверяем блокировку пользрвателя по номеру телефона.
        var blockedUserPhoneNumber = await _pgContext.UserPhoneBlackList
            .AnyAsync(u => u.PhoneNumber.Equals(availableBlockedText));

        if (blockedUserPhoneNumber)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Метод проверяет, заполнена ли анкета пользователя.
    /// Если не заполнена, то запрещаем доступ к ключевому функционалу.
    /// </summary>
    /// <param name="userId">ID пользователя.</param>
    /// <returns>Признак проверки.</returns>
    public async Task<(ProfileInfoEntity, List<UserIntentEntity>, List<UserSkillEntity>)>
        IsProfileEmptyAsync(long userId)
    {
        var result = (UserProfile: new ProfileInfoEntity(), UserIntents: new List<UserIntentEntity>(),
            UserSkills: new List<UserSkillEntity>());

        result.UserProfile = await _pgContext.ProfilesInfo
            .FirstOrDefaultAsync(p => p.UserId == userId);

        result.UserIntents = await _pgContext.UserIntents
            .Where(i => i.UserId == userId)
            .ToListAsync();

        result.UserSkills = await _pgContext.UserSkills
            .Where(i => i.UserId == userId)
            .ToListAsync();

        return result;
    }
}