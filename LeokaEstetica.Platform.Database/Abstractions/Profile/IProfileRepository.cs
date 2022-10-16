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
}