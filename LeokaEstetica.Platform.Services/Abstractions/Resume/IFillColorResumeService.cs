using LeokaEstetica.Platform.Database.Abstractions.FareRule;
using LeokaEstetica.Platform.Database.Abstractions.Subscription;
using LeokaEstetica.Platform.Models.Dto.Output.Resume;

namespace LeokaEstetica.Platform.Services.Abstractions.Resume;

/// <summary>
/// TODO: Выпилить сервис, если у нас не будет выделения цветами тарифов.
/// Абстракция сервиса выделение цветом резюме пользователей.
/// </summary>
public interface IFillColorResumeService
{
    /// <summary>
    /// Метод устанавливает свойство IsSelectedColor = true для резюме пользователей с подпиской выше бизнес.
    /// </summary>
    /// <param name="resumesList">Список ризюме.</param>
    /// <param name="subscriptionRepository">Сервис подписок.</param>
    /// <param name="fareRuleRepository">Сервис правил тарифов.</param>
    /// <returns>Список ризюме, с проставленным флагом IsSelectedColor.</returns>
    // public Task<IEnumerable<UserInfoOutput>> SetColorBusinessResume(List<UserInfoOutput> resumesList,
    //     ISubscriptionRepository subscriptionRepository, IFareRuleRepository fareRuleRepository);
}