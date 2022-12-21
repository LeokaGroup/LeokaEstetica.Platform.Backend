using NUnit.Framework;
using static NUnit.Framework.Assert;

namespace LeokaEstetica.Platform.Tests.ModerationTests;

[TestFixture]
public class CheckUserRoleModerationTest : BaseServiceTest
{
    /// <summary>
    /// Метод проверяет пользователя, у которого есть роль для модерации.
    /// </summary>
    [Test]
    public async Task CheckUserRoleModerationSuccessAsyncTest()
    {
        var result = await AccessModerationService.CheckUserRoleModerationAsync("sierra_93@mail.ru");

        IsTrue(result.AccessModeration);
    }
    
    /// <summary>
    /// Метод проверяет пользователя, у которого нет роли для модерации.
    /// </summary>
    [Test]
    public async Task CheckUserRoleModerationErrorAsyncTest()
    {
        var result = await AccessModerationService.CheckUserRoleModerationAsync("alisaiva931@mail.ru");

        IsFalse(result.AccessModeration);
    }
}