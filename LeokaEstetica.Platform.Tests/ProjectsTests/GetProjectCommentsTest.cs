using LeokaEstetica.Platform.Models.Entities.Communication;
using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ProjectsTests;

[TestFixture]
internal class GetProjectCommentsTest : BaseServiceTest
{
    [Test]
    public async Task GetProjectCommentsAsyncTest()
    {
        await AddCommentProjectForTestAsync();
        var result = await ProjectCommentsService.GetProjectCommentsAsync(215);

        Assert.IsNotNull(result);
        Assert.False(result.Where(c=>c.Comment == "Comment test for unit").Any());
        Assert.True(result.Any(p => p.ProjectId == 215));
    }

    private async Task AddCommentProjectForTestAsync()
    {
        var result = PgContext.ProjectComments
            .Where(c=>c.ProjectId == 218)
            .ToList();
        var comment = result.FirstOrDefault(c => c.Comment == "Comment test for unit");
        if(comment is null)
        {
            var testModel = new ProjectCommentEntity
            {
                ProjectId = 218,
                Comment = "Comment test for unit",
                UserId = 89
            };
            await PgContext.ProjectComments.AddAsync(testModel);
            await PgContext.SaveChangesAsync();
        }
    }
}