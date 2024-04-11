using LeokaEstetica.Platform.Models.Enums;
using NUnit.Framework;
using Enum = System.Enum;

namespace LeokaEstetica.Platform.Tests.ProjectManagmentTests;

[TestFixture]
internal class GetLinkTypesTest : BaseServiceTest
{
    [Test]
    public Task GetLinkTypesAsyncTest()
    {
        var result = Enum.GetNames(typeof(LinkTypeEnum)).Select(x => x.ToString());
        
        Assert.NotNull(result);
        Assert.IsNotEmpty(result);

        return Task.CompletedTask;
    }
}