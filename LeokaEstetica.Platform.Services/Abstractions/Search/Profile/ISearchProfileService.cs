using LeokaEstetica.Platform.Models.Entities.Profile;

namespace LeokaEstetica.Platform.Services.Abstractions.Search.Profile;

/// <summary>
/// Абстракция поиска в профиле пользователя.
/// </summary>
public interface ISearchProfileService
{
    /// <summary>
    /// Метод поиска навыков по названию навыка.
    /// </summary>
    /// <param name="searchText">Поисковый текст.</param>
    /// <returns>Список навыков, которые удалось найти.</returns>
    Task<IEnumerable<SkillEntity>> SearchSkillsByNameAsync(string searchText);
}