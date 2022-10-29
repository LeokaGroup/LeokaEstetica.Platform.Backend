using LeokaEstetica.Platform.Models.Dto.Input.Profile;
using LeokaEstetica.Platform.Models.Dto.Output.Profile;

namespace LeokaEstetica.Platform.Services.Abstractions.Profile;

/// <summary>
/// Абстракция сервиса профиля пользователя.
/// </summary>
public interface IProfileService
{
    /// <summary>
    /// Метод получает основную информацию раздела обо мне.
    /// <param name="account">Аккаунт пользователя.</param>
    /// </summary>
    /// <returns>Данные раздела обо мне.</returns>
    Task<ProfileInfoOutput> GetProfileInfoAsync(string account);

    /// <summary>
    /// Метод получает список элементов меню профиля пользователя.
    /// </summary>
    /// <returns>Список элементов меню профиля пользователя.</returns>
    Task<ProfileMenuItemsResultOutput> ProfileMenuItemsAsync();

    /// <summary>
    /// Метод получает список навыков для выбора в профиль пользователя.
    /// </summary>
    /// <returns>Список навыков.</returns>
    Task<IEnumerable<SkillOutput>> ProfileSkillsAsync();

    /// <summary>
    /// Метод получает список целей на платформе для выбора пользователем в профиль пользователя.
    /// </summary>
    /// <returns>Список целей.</returns>
    Task<IEnumerable<IntentOutput>> ProfileIntentsAsync();

    /// <summary>
    /// Метод сохраняет данные контактной информации пользователя.
    /// </summary>
    /// <param name="profileInfoInput">Входная модель.</param>
    /// <param name="account">ккаунт пользователя.</param>
    /// <returns>Сохраненные данные.</returns>
    Task<ProfileInfoOutput> SaveProfileInfoAsync(ProfileInfoInput profileInfoInput, string account);

    /// <summary>
    /// Метод выбирает пункт меню профиля пользователя. Производит действия, если нужны. 
    /// </summary>
    /// <param name="selectMenuInput">Входная модель.</param>
    /// <returns>Системное название действия и роут если нужно.</returns>
    Task<SelectMenuOutput> SelectProfileMenuAsync(string text);
}