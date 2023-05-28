using LeokaEstetica.Platform.Models.Dto.Input.Profile;
using LeokaEstetica.Platform.Models.Dto.Output.Profile;
using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ProfileTests;

[TestFixture]
public class SaveProfileInfoTest : BaseServiceTest
{
    [Test]
    public async Task SaveProfileInfoAsyncTest()
    {
        var profileInfo = await ProfileService.SaveProfileInfoAsync(new ProfileInfoInput
        {
            FirstName = "Иван",
            LastName = "Иванов",
            Patronymic = "Иванович",
            IsShortFirstName = false,
            Aboutme = "Тестовая информация о пользователе.",
            Job = "Тестировщик",
            WhatsApp = "89543567834",
            Telegram = "@vano",
            Vkontakte = "https://vk.com/vano",
            OtherLink = "Нету...",
            UserSkills = new List<SkillInput>(),
            UserIntents = new List<IntentOutput>(),
            WorkExperience = "Test"
        }, "sierra_93@mail.ru", string.Empty);

        Assert.IsNotNull(profileInfo);
        Assert.IsTrue(profileInfo.ProfileInfoId > 0);
    }
}