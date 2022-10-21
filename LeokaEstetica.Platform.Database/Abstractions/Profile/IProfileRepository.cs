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
}