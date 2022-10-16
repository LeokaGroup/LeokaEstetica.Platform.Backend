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
}