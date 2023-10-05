using LeokaEstetica.Platform.Database.Abstractions.Profile;
using LeokaEstetica.Platform.Models.Entities.Profile;
using LeokaEstetica.Platform.Services.Abstractions.Search.Profile;
using Microsoft.Extensions.Logging;

namespace LeokaEstetica.Platform.Services.Services.Search.Profile;

/// <summary>
/// Класс реализует методы поиска в профиле пользователя.
/// </summary>
internal sealed class SearchProfileService : ISearchProfileService
{
    private readonly IProfileRepository _profileRepository;
    private readonly ILogger<SearchProfileService> _logger;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="profileRepository">Репозиторий пользователей.</param>
    /// <param name="logger">Логгер.</param>
    public SearchProfileService(IProfileRepository profileRepository,
        ILogger<SearchProfileService> logger)
    {
        _profileRepository = profileRepository;
        _logger = logger;
    }

    #region Публичные методы.

    /// <summary>
    /// Метод поиска навыков по названию навыка.
    /// </summary>
    /// <param name="searchText">Поисковый текст.</param>
    /// <returns>Список навыков, которые удалось найти.</returns>
    public async Task<IEnumerable<SkillEntity>> SearchSkillsByNameAsync(string searchText)
    {
        try
        {
            var result = await _profileRepository.GetUserSkillsByNameAsync(searchText);

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    #endregion

    #region Приватные методы.

    

    #endregion
}