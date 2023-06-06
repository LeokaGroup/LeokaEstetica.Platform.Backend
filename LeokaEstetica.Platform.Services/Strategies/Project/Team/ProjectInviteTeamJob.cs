namespace LeokaEstetica.Platform.Services.Strategies.Project.Team;

/// <summary>
/// Класс представляет семейство алгоритмов для нахождения Id пользователя.
/// </summary>
internal class ProjectInviteTeamJob
{
    /// <summary>
    /// Метод находит Id пользователя по указанной стратегии.
    /// </summary>
    /// <param name="job">Стратегия поиска.</param>
    /// <param name="inviteText">Текст для приглашения.</param>
    /// <returns>Id пользователя.</returns>
    public async Task<long> GetUserIdAsync(BaseProjectInviteTeamStrategy job, string inviteText)
    {
        if (job is not null)
        {
            return await job.GetUserId(inviteText, string.Empty);
        }

        return 0;
    }
}