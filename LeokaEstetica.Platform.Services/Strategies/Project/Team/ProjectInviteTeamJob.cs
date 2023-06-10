namespace LeokaEstetica.Platform.Services.Strategies.Project.Team;

/// <summary>
/// Класс представляет семейство алгоритмов для нахождения Id пользователя.
/// </summary>
internal sealed class ProjectInviteTeamJob
{
    /// <summary>
    /// Метод находит Id пользователя по указанной стратегии.
    /// </summary>
    /// <param name="strategy">Стратегия поиска.</param>
    /// <param name="inviteText">Текст для приглашения.</param>
    /// <returns>Id пользователя.</returns>
    internal async Task<long> GetUserIdAsync(BaseProjectInviteTeamStrategy strategy, string inviteText)
    {
        if (strategy is not null)
        {
            return await strategy.GetUserId(inviteText, string.Empty);
        }

        return 0;
    }
}