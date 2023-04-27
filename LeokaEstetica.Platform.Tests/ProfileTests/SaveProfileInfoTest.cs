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
        }, "sierra_93@mail.ru", null);

        Assert.IsNotNull(profileInfo);
        Assert.IsTrue(profileInfo.ProfileInfoId > 0);
    }

    [Test]
    public async Task SaveProfileInfoByVkIdAsyncTest()
    {
        var profileInfo = await ProfileService.SaveProfileInfoAsync(new ProfileInfoInput
        {
            FirstName = "Петр",
            LastName = "Петров",
            Patronymic = "Петрович",
            IsShortFirstName = false,
            Aboutme = "Тестовая информация о пользователе.",
            Job = "Работник",
            WhatsApp = "89543567834",
            Telegram = "@petro",
            Vkontakte = "https://vk.com/Petro",
            OtherLink = "Нету...",
            UserSkills = new List<SkillInput>(),
            UserIntents = new List<IntentOutput>(),
            WorkExperience = "Test"
        }, "798589660", null);

        Assert.IsNotNull(profileInfo);
        Assert.IsTrue(profileInfo.ProfileInfoId > 0);
    }
    }