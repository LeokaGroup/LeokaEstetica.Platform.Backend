using LeokaEstetica.Platform.Models.Entities.Project;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using static NUnit.Framework.Assert;

namespace LeokaEstetica.Platform.Tests.ModerationTests;

[TestFixture]
public class ProjectsModerationTest : BaseServiceTest
{
    [Test]
    public async Task ProjectsModerationAsyncTest()
    {
        await AddProjectInArchiveForTestAsync();

        var result = await ProjectModerationService.ProjectsModerationAsync();

        IsNotNull(result);
        var moderationProjectEntities = result.Projects.ToList();
        IsNotEmpty(moderationProjectEntities);
        IsTrue(moderationProjectEntities.FirstOrDefault()?.ProjectId > 0);
        IsNull(result.Projects.FirstOrDefault(r => r.ProjectId == 223));
    }

    private async Task AddProjectInArchiveForTestAsync()
    {
        var archived = await PgContext.ArchivedProjects.FirstOrDefaultAsync(a => a.ProjectId == 223);
        var testProject = new ArchivedProjectEntity
        {
            ProjectId = 223,
            UserId = 32,
        };

        if (archived is null)
        {
            PgContext.ArchivedProjects.Add(testProject);

        }
        else
        {
            PgContext.ArchivedProjects.Remove(archived);
            PgContext.ArchivedProjects.Add(testProject);
        }

        await PgContext.SaveChangesAsync();
    }
}