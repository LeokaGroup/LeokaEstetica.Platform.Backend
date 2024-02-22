using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ProjectManagmentTests;

[TestFixture]
internal class GetConfigurationWorkSpaceBySelectedTemplateTest : BaseServiceTest
{
    /// <summary>
    /// Тестирует конфигурацию без пагинатора.
    /// </summary>
    [Test]
    public async Task GetConfigurationWorkSpaceBySelectedTemplateAsyncTest()
    {
        var result = await ProjectManagmentService.GetConfigurationWorkSpaceBySelectedTemplateAsync(295,
            "sierra_93@mail.ru", null);

        Assert.NotNull(result);
        Assert.IsNotEmpty(result.ProjectManagmentTaskStatuses);
        Assert.True(result.ProjectManagmentTaskStatuses.Any());
    }

    /// <summary>
    /// Тестирует работу пагинатора для 1 страницы.
    /// </summary>
    [Test]
    public async Task GetConfigurationWorkSpaceBySelectedTemplateAsyncWithPaginator1Test()
    {
        // Тестирует 1 страницу с применением пагинатора для статуса "Новая".
        var firstPageTest = await ProjectManagmentService.GetConfigurationWorkSpaceBySelectedTemplateAsync(274,
            "sierra_93@mail.ru", 1);
        
        Assert.NotNull(firstPageTest);
        Assert.IsTrue(firstPageTest.ProjectManagmentTaskStatuses.Count() > 10);
    }
    
    /// <summary>
    /// Тестирует работу пагинатора для 2 страницы.
    /// </summary>
    [Test]
    public async Task GetConfigurationWorkSpaceBySelectedTemplateAsyncWithPaginator2Test()
    {
        // Тестирует 2 страницу с применением пагинатора для статуса "Новая".
        var secondPageTest = await ProjectManagmentService.GetConfigurationWorkSpaceBySelectedTemplateAsync(274,
            "sierra_93@mail.ru", 1, 2);
        
        Assert.NotNull(secondPageTest);
         
        // Утверждение на кол-во <= 10, так как должен был сработать пагинатор.
        Assert.IsTrue(secondPageTest.ProjectManagmentTaskStatuses.Count() <= 10);
    }
}