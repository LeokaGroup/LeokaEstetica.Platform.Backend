using LeokaEstetica.Platform.Models.Dto.Input.Profile;
using LeokaEstetica.Platform.Models.Dto.Output.Profile;
using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ProfileTests;

[TestFixture]
internal class SaveProfileInfoTest : BaseServiceTest
{
    [Test]
    public async Task SaveProfileInfoAsyncTest()
    {
        var profileInfo = await ProfileService.SaveProfileInfoAsync(
            new ProfileInfoInput()
            {
                Patronymic = "Иванович",
                IsShortFirstName = false,
                WhatsApp = "89543567834",
                Telegram = "@vano",
                Vkontakte = "https://vk.com/vano",
                OtherLink = "Нету..."
            }, "sierra_93@mail.ru");

        Assert.IsNotNull(profileInfo);
        Assert.IsTrue(profileInfo.ProfileInfoId > 0);
    }
}