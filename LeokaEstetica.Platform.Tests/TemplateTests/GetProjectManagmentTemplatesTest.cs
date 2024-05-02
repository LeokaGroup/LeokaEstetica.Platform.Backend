using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using static NUnit.Framework.Assert;

namespace LeokaEstetica.Platform.Tests.TemplateTests;

[TestFixture]
internal class GetProjectManagmentTemplatesTest : BaseServiceTest
{
    // [Test]
    // public async Task GetProjectManagmentTemplatesAsyncTest()
    // {
    //     var result = await ProjectManagmentService.GetProjectManagmentTemplatesAsync(null);
    //     
    //     NotNull(result);
    // }

    /// <summary>
    /// Метод тестирует группировку по названию шаблона при этом сортируя шаблоны и статусы по возрастанию.
    /// </summary>
    [Test]
    public async Task GetProjectManagmentTemplateStatusesGroupByWithOrderByAsyncTest()
    {
        // var result = await PgContext.ProjectManagmentTaskTemplates
        //     .Include(x => x.ProjectManagmentTaskStatusTemplates.OrderBy(o => o.Position))
        //     .ThenInclude(x => x.ProjectManagmentTaskStatusIntermediateTemplates)
        //     .OrderBy(o => o.Position)
        //     .GroupBy(g => g.TemplateName)
        //     .Select(x => new
        //     {
        //         TemplateName = x.Key,
        //         ProjectManagmentTaskStatusTemplates = x
        //             .SelectMany(y => y.ProjectManagmentTaskStatusTemplates
        //             .OrderBy(o => o.Position)),
        //     })
        //     .ToListAsync();
        //
        // NotNull(result);
        // IsNotEmpty(result);
    }
}