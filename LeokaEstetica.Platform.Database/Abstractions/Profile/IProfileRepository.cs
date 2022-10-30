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
    Task AddUserInfoAsync(long userId);

    /// <summary>
    /// Метод получает список элементов меню профиля пользователя.
    /// </summary>
    /// <returns>Список элементов меню профиля пользователя.</returns>
    Task<ProfileMenuItemEntity> ProfileMenuItemsAsync();

    /// <summary>
    /// Метод получает список навыков для выбора в профиль пользователя.
    /// </summary>
    /// <returns>Список навыков.</returns>
    Task<IEnumerable<SkillEntity>> ProfileSkillsAsync();
    
    /// <summary>
    /// Метод получает список целей на платформе для выбора пользователем в профиль пользователя.
    /// </summary>
    /// <returns>Список целей.</returns>
    Task<IEnumerable<IntentEntity>> ProfileIntentsAsync();

    /// <summary>
    /// Метод сохраняет данные контактной информации пользователя.
    /// </summary>
    /// <param name="profileInfo">Данные для сохранения.</param>
    /// <returns>Данные профиля.</returns>
    Task<ProfileInfoEntity> SaveProfileInfoAsync(ProfileInfoEntity profileInfo);

    /// <summary>
    /// Метод сохраняет выбранные пользователям навыки.
    /// </summary>
    /// <param name="selectedSkills">Список навыков для сохранения.</param>
    /// <returns>Список навыков.</returns>
    Task SaveProfileSkillsAsync(IEnumerable<UserSkillEntity> selectedSkills);

    /// <summary>
    /// Метод получает список выбранные навыки пользователя.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Список навыков.</returns>
    Task<IEnumerable<UserSkillEntity>> SelectedProfileUserSkillsAsync(long userId);

    /// <summary>
    /// Метод получает список навыков по их Id.
    /// </summary>
    /// <param name="skillsIds">Список навыков, которые нужно получить.</param>
    /// <returns>Список навыков.</returns>
    Task<IEnumerable<SkillEntity>> GetProfileSkillsBySkillIdAsync(int[] skillsIds);
}