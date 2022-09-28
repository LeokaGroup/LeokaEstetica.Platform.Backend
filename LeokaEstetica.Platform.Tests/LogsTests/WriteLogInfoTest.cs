using LeokaEstetica.Platform.Models.Enums;
using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.LogsTests;

[TestFixture]
public class WriteLogInfoTest : BaseServiceTest
{
    [Test]
    public async Task WriteLogInfoAsyncTest()
    {
        await LogService.LogInfoAsync(new ArgumentNullException(), "sierra_93@mail.ru", LogLevelEnum.Error);
    }
}