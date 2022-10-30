using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Database.Abstractions.Profile;
using LeokaEstetica.Platform.Models.Entities.Profile;
using Microsoft.EntityFrameworkCore;

namespace LeokaEstetica.Platform.Database.Repositories.Profile;

/// <summary>
/// Класс реализует методы репозитория профиля пользователя.
/// </summary>
public sealed class ProfileRepository : IProfileRepository
{
    private readonly PgContext _pgContext;
    
    public ProfileRepository(PgContext pgContext)
    {
        _pgContext = pgContext;
    }

    /// <summary>
    /// Метод получает основную информацию раздела обо мне.
    /// <param name="userId">Id пользователя.</param>
    /// </summary>
    /// <returns>Данные раздела обо мне.</returns>
    public async Task<ProfileInfoEntity> GetProfileInfoAsync(long userId)
    {
        var result = await _pgContext.ProfilesInfo
            .FirstOrDefaultAsync(u => u.UserId == userId);

        return result;
    }

    /// <summary>
    /// Метод добавляет данные о пользователе в таблицу профиля.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    public async Task AddUserInfoAsync(long userId)
    {
        var userInfo = new ProfileInfoEntity
        {
            FirstName = string.Empty,
            LastName = string.Empty,
            UserId = userId,
            IsShortFirstName = false,
            Aboutme = string.Empty,
            Job = string.Empty
        };
        await _pgContext.ProfilesInfo.AddAsync(userInfo);
        await _pgContext.SaveChangesAsync();
    }

    /// <summary>
    /// Метод получает список элементов меню профиля пользователя.
    /// </summary>
    /// <returns>Список элементов меню профиля пользователя.</returns>
    public async Task<ProfileMenuItemEntity> ProfileMenuItemsAsync()
    {
        var result = await _pgContext.ProfileMenuItems
            .FirstOrDefaultAsync();

        return result;
    }

    /// <summary>
    /// Метод получает список навыков для выбора в профиль пользователя.
    /// </summary>
    /// <returns>Список навыков.</returns>
    public async Task<IEnumerable<SkillEntity>> ProfileSkillsAsync()
    {
        var result = await _pgContext.Skills.ToListAsync();

        return result;
    }

    /// <summary>
    /// Метод получает список целей на платформе для выбора пользователем в профиль пользователя.
    /// </summary>
    /// <returns>Список целей.</returns>
    public async Task<IEnumerable<IntentEntity>> ProfileIntentsAsync()
    {
        var result = await _pgContext.Intents.ToListAsync();

        return result;
    }

    /// <summary>
    /// Метод сохраняет данные контактной информации пользователя.
    /// </summary>
    /// <param name="profileInfo">Данные для сохранения.</param>
    /// <returns>Данные профиля.</returns>
    public async Task<ProfileInfoEntity> SaveProfileInfoAsync(ProfileInfoEntity profileInfo)
    {
        _pgContext.ProfilesInfo.Update(profileInfo);
        await _pgContext.SaveChangesAsync();

        return profileInfo;
    }

    /// <summary>
    /// Метод сохраняет выбранные пользователям навыки.
    /// </summary>
    /// <param name="selectedSkills">Список навыков для сохранения.</param>
    /// <returns>Список навыков.</returns>
    public async Task SaveProfileSkillsAsync(IEnumerable<UserSkillEntity> selectedSkills)
    {
        await _pgContext.UserSkills.AddRangeAsync(selectedSkills);
        await _pgContext.SaveChangesAsync();
    }

    /// <summary>
    /// Метод получает список выбранные навыки пользователя.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Список навыков.</returns>
    public async Task<IEnumerable<UserSkillEntity>> SelectedProfileUserSkillsAsync(long userId)
    {
        var result = await _pgContext.UserSkills
            .Where(s => s.UserId == userId)
            .ToListAsync();

        return result;
    }

    /// <summary>
    /// Метод получает список навыков по их Id.
    /// </summary>
    /// <param name="skillsIds">Список навыков, которые нужно получить.</param>
    /// <returns>Список навыков.</returns>
    public async Task<IEnumerable<SkillEntity>> GetProfileSkillsBySkillIdAsync(int[] skillsIds)
    {
        var result = await _pgContext.Skills
            .Where(s => skillsIds.Contains(s.SkillId))
            .ToListAsync();

        return result;
    }
}