using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Database.Abstractions.Profile;
using LeokaEstetica.Platform.Models.Entities.Profile;
using Microsoft.EntityFrameworkCore;

namespace LeokaEstetica.Platform.Database.Repositories.Profile;

/// <summary>
/// Класс реализует методы репозитория профиля пользователя.
/// </summary>
internal sealed class ProfileRepository : IProfileRepository
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
    /// <returns>Id анкеты пользователя.</returns>
    public async Task<long> AddUserInfoAsync(long userId)
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

        return userInfo.ProfileInfoId;
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
    public async Task<List<SkillEntity>> ProfileSkillsAsync()
    {
        var result = await _pgContext.Skills.ToListAsync();

        return result;
    }

    /// <summary>
    /// Метод получает список целей на платформе для выбора пользователем в профиль пользователя.
    /// </summary>
    /// <returns>Список целей.</returns>
    public async Task<List<IntentEntity>> ProfileIntentsAsync()
    {
        var result = await _pgContext.Intents.ToListAsync();

        return result;
    }

    /// <summary>
    /// Метод сохраняет данные анкеты пользователя.
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
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Список навыков.</returns>
    public async Task SaveProfileSkillsAsync(IEnumerable<UserSkillEntity> selectedSkills, long userId)
    {
        var userSkills = await _pgContext.UserSkills
            .Where(s => s.UserId == userId)
            .ToListAsync();

        // Если у пользователя в БД еще не было навыков, то просто добавляем из выбранных.
        var enumerable = selectedSkills.ToList();
        var skillEntities = enumerable.ToList();
        var userSkillEntities = skillEntities.ToList();
        
        // Если в БД нет навыков еще у пользователя, то добавляем все, что он выбрал.
        if (!userSkills.Any())
        {
            await _pgContext.UserSkills.AddRangeAsync(userSkillEntities);
        }
        
        // Если в БД есть навыки у пользователя, но он ничего не выбрал, то удаляем все его навыки из БД.
        else if (!enumerable.Any())
        {
            _pgContext.UserSkills.RemoveRange(userSkillEntities);
        }

        else
        {
            // Если в БД есть навыки у пользователя, то актуализируем их.
            foreach (var skill in userSkillEntities)
            {
                // Если в БД есть такой уже навык, то ничего не делаем.
                var findIndex = userSkills.FindIndex(s => s.SkillId == skill.SkillId);

                if (findIndex != -1)
                {
                    continue;
                }
            
                // Если в БД нету, то добавляем.
                await _pgContext.UserSkills.AddAsync(skill);
            }   
        }

        // Удаляем те навыки, которые пользователь не выбрал, но они есть в БД.
        if (userSkills.Any())
        {
            var items = await _pgContext.UserSkills
                .Where(s => s.UserId == userId 
                            && !skillEntities.Select(x => x.SkillId).Contains(s.SkillId))
                .ToListAsync();
            
            _pgContext.UserSkills.RemoveRange(items);
        }

        await _pgContext.SaveChangesAsync();
    }

    /// <summary>
    /// Метод получает список выбранные навыки пользователя.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Список навыков.</returns>
    public async Task<List<UserSkillEntity>> SelectedProfileUserSkillsAsync(long userId)
    {
        var result = await _pgContext.UserSkills
            .Where(s => s.UserId == userId)
            .ToListAsync();

        return result;
    }

    /// <summary>
    /// Метод получает список навыков по их Id.
    /// </summary>
    /// <param name="skillsIds">Список Id навыков, которые нужно получить.</param>
    /// <returns>Список навыков.</returns>
    public async Task<IEnumerable<SkillEntity>> GetProfileSkillsBySkillIdAsync(int[] skillsIds)
    {
        var result = await _pgContext.Skills
            .Where(s => skillsIds.Contains(s.SkillId))
            .ToListAsync();

        return result;
    }

    /// <summary>
    /// Метод сохраняет выбранные пользователям цели.
    /// </summary>
    /// <param name="selectedIntents">Список целей для сохранения.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Список целей.</returns>
    public async Task SaveProfileIntentsAsync(IEnumerable<UserIntentEntity> selectedIntents, long userId)
    {
        var userIntents = await _pgContext.UserIntents
            .Where(s => s.UserId == userId)
            .ToListAsync();

        // Если у пользователя в БД еще не было целей, то просто добавляем из выбранных.
        var enumerable = selectedIntents.ToList();
        var skillEntities = enumerable.ToList();
        var userIntentsEntities = skillEntities.ToList();
        
        // Если в БД нет целей еще у пользователя, то добавляем все, что он выбрал.
        if (!userIntents.Any())
        {
            await _pgContext.UserIntents.AddRangeAsync(userIntentsEntities);
        }
        
        // Если в БД есть цели у пользователя, но он ничего не выбрал, то удаляем все его цели из БД.
        else if (!enumerable.Any())
        {
            _pgContext.UserIntents.RemoveRange(userIntentsEntities);
        }

        else
        {
            // Если в БД есть цели у пользователя, то актуализируем их.
            foreach (var intent in userIntentsEntities)
            {
                // Если в БД есть такой уже цель, то ничего не делаем.
                var findIndex = userIntents.FindIndex(s => s.IntentId == intent.IntentId);

                if (findIndex != -1)
                {
                    continue;
                }
            
                // Если в БД нету, то добавляем.
                await _pgContext.UserIntents.AddAsync(intent);
            }   
        }

        // Удаляем те цели, которые пользователь не выбрал, но они есть в БД.
        if (userIntents.Any())
        {
            var items = await _pgContext.UserIntents
                .Where(s => s.UserId == userId 
                            && !skillEntities.Select(x => x.IntentId).Contains(s.IntentId))
                .ToListAsync();
            
            _pgContext.UserIntents.RemoveRange(items);
        }

        await _pgContext.SaveChangesAsync();
    }

    /// <summary>
    /// Метод получает список целей по их Id.
    /// </summary>
    /// <param name="intentsIds">Список Id целей, которые нужно получить.</param>
    /// <returns>Список целей.</returns>
    public async Task<IEnumerable<IntentEntity>> GetProfileIntentsByIntentIdAsync(int[] intentsIds)
    {
        var result = await _pgContext.Intents
            .Where(s => intentsIds.Contains(s.IntentId))
            .ToListAsync();

        return result;
    }
    
    /// <summary>
    /// Метод получает список выбранные цели пользователя.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Список целей.</returns>
    public async Task<List<UserIntentEntity>> SelectedProfileUserIntentsAsync(long userId)
    {
        var result = await _pgContext.UserIntents
            .Where(s => s.UserId == userId)
            .ToListAsync();

        return result;
    }
}