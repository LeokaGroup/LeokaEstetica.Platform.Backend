using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ProjectsTests;

[TestFixture]
public class ArchivedProjectMappingTest : BaseServiceTest
{
    [Test]
    public async Task CheckOfRecipientFromTheArchivedProjectsAsyncTest()
    {
        var archivedProject = PgContext.ArchivedProjects.ToList();
    }
}
