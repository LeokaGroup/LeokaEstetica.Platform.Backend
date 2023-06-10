using NUnit.Framework;
using static NUnit.Framework.Assert;

namespace LeokaEstetica.Platform.Tests.ProjectsTests;

[TestFixture]
internal class GetProjectsPaginationTest : BaseServiceTest
{
    [Test]
    public async Task GetProjectsPaginationAsync()
    {
        var result = await ProjectPaginationService.GetProjectsPaginationAsync(1);
        
        NotNull(result);
        NotNull(result.Projects);
    }
}