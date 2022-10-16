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
}