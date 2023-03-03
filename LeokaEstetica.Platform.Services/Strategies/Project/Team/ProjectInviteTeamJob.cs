namespace LeokaEstetica.Platform.Services.Strategies.Project.Team;

public class ProjectInviteTeamJob
{
    public async Task<long> GetUserIdAsync(BaseProjectInviteTeamStrategy job, string inviteText)
    {
        if (job is not null)
        {
            return await job.GetUserId(inviteText);
        }

        return 0;
    }
}