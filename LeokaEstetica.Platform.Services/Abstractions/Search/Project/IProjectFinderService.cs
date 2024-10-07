using LeokaEstetica.Platform.Models.Entities.User;

namespace LeokaEstetica.Platform.Services.Abstractions.Search.Project;

/// <summary>
/// Абстракция сервиса поиска в проектах.
/// </summary>
public interface IProjectFinderService
{
    /// <summary>
    /// Метод ищет пользователей для приглашения в команду проекта.
    /// </summary>
    /// <param name="searchText">Поисковый запрос.</param>
    /// <param name="token">Токен пользователя.</param>
    /// <returns>Список пользователей, которых можно пригласить в команду проекта.</returns>
    Task<IEnumerable<UserEntity>> SearchInviteProjectMembersAsync(string searchText, string token);

    /// <summary>
    /// Метод ищет пользователей по их почте.
    /// </summary>
    /// <param name="searchText">Поисковый текст.</param>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Список пользователей, которых можно пригласить в команду проекта.</returns>
    Task<UserEntity?> SearchUserByEmailAsync(string searchText, string account);
}