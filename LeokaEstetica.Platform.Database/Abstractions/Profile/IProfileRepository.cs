using LeokaEstetica.Platform.Models.Entities.Profile;

namespace LeokaEstetica.Platform.Database.Abstractions.Profile;

/// <summary>
/// Абстракция репозитория профиля пользователя.
/// </summary>
public interface IProfileRepository
{
    /// <summary>
    /// Метод получает основную информацию раздела обо мне.
    /// <param name="userId">Id пользователя.</param>
    /// </summary>
    /// <returns>Данные раздела обо мне.</returns>
    Task<ProfileInfoEntity> GetProfileInfoAsync(long userId);

    /// <summary>
    /// Метод добавляет данные о пользователе в таблицу профиля.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Id анкеты пользователя.</returns>
    Task<long> AddUserInfoAsync(long userId);

    /// <summary>
    /// Метод получает список элементов меню профиля пользователя.
    /// </summary>
    /// <returns>Список элементов меню профиля пользователя.</returns>
    Task<ProfileMenuItemEntity> ProfileMenuItemsAsync();

    /// <summary>
    /// Метод получает список навыков для выбора в профиль пользователя.
    /// </summary>
    /// <returns>Список навыков.</returns>
    Task<List<SkillEntity>> ProfileSkillsAsync();
    
    /// <summary>
    /// Метод получает список целей на платформе для выбора пользователем в профиль пользователя.
    /// </summary>
    /// <returns>Список целей.</returns>
    Task<List<IntentEntity>> ProfileIntentsAsync();

    /// <summary>
    /// Метод сохраняет данные анкеты пользователя.
    /// </summary>
    /// <param name="profileInfo">Данные для сохранения.</param>
    /// <returns>Данные профиля.</returns>
    Task<ProfileInfoEntity> SaveProfileInfoAsync(ProfileInfoEntity profileInfo);

    /// <summary>
    /// Метод сохраняет выбранные пользователям навыки.
    /// </summary>
    /// <param name="selectedSkills">Список навыков для сохранения.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Список навыков.</returns>
    Task SaveProfileSkillsAsync(IEnumerable<UserSkillEntity> selectedSkills, long userId);

    /// <summary>
    /// Метод получает список выбранные навыки пользователя.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Список навыков.</returns>
    Task<List<UserSkillEntity>> SelectedProfileUserSkillsAsync(long userId);

    /// <summary>
    /// Метод получает список навыков по их Id.
    /// </summary>
    /// <param name="skillsIds">Список Id навыков, которые нужно получить.</param>
    /// <returns>Список навыков.</returns>
    Task<IEnumerable<SkillEntity>> GetProfileSkillsBySkillIdAsync(int[] skillsIds);
    
    /// <summary>
    /// Метод получает список целей по их Id.
    /// </summary>
    /// <param name="intentsIds">Список Id целей, которые нужно получить.</param>
    /// <returns>Список целей.</returns>
    Task<IEnumerable<IntentEntity>> GetProfileIntentsByIntentIdAsync(int[] intentsIds);
    
    /// <summary>
    /// Метод сохраняет выбранные пользователям цели.
    /// </summary>
    /// <param name="selectedIntents">Список целей для сохранения.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Список целей.</returns>
    Task SaveProfileIntentsAsync(IEnumerable<UserIntentEntity> selectedIntents, long userId);
    
    /// <summary>
    /// Метод получает список выбранные цели пользователя.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Список целей.</returns>
    Task<List<UserIntentEntity>> SelectedProfileUserIntentsAsync(long userId);
}