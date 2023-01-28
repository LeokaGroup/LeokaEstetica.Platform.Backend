using LeokaEstetica.Platform.Models.Dto.Output.Subscription;
using NUnit.Framework;
using static NUnit.Framework.Assert;

namespace LeokaEstetica.Platform.Tests.SubscriptionTests;

[TestFixture]
public class FillSubscriptionsTest : BaseServiceTest
{
    /// <summary>
    /// Метод тестирует запись активных подписок пользователя.
    /// </summary>
    [Test]
    public async Task FillSubscriptionsAsyncTest()
    {
        var result = await SubscriptionService.FillSubscriptionsAsync("sierra_93@mail.ru", new List<SubscriptionOutput>
        {
            new()
            {
                IsActive = false,
                IsLatter = false,
                ObjectId = 1,
                SubscriptionId = 1
            },
            new()
            {
                IsActive = false,
                IsLatter = false,
                ObjectId = 2,
                SubscriptionId = 2
            },
            new()
            {
                IsActive = false,
                IsLatter = false,
                ObjectId = 3,
                SubscriptionId = 3
            },
            new()
            {
                IsActive = false,
                IsLatter = false,
                ObjectId = 4,
                SubscriptionId = 4
            }
        });

        IsNotEmpty(result);
    }
}