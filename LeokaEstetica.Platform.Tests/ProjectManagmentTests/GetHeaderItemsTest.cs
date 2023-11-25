using AutoMapper;
using LeokaEstetica.Platform.Core.Utils;
using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagment;
using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ProjectManagmentTests;

[TestFixture]
internal class GetHeaderItemsTest : BaseServiceTest
{
    [Test]
    public async Task GetHeaderItemsAsyncTest()
    {
        var unprocessedItems = await ProjectManagmentService.GetHeaderItemsAsync();

        Assert.NotNull(unprocessedItems);

        var mapper = AutoFac.Resolve<IMapper>();
        var mapItems = mapper.Map<IEnumerable<ProjectManagmentHeaderOutput>>(unprocessedItems);
        
        var result = await ProjectManagmentService.ModifyHeaderItemsAsync(mapItems);
        
        Assert.NotNull(result.All(x => x.Items is not null && x.Items.Any() && !string.IsNullOrEmpty(x.Label)));
    }
}