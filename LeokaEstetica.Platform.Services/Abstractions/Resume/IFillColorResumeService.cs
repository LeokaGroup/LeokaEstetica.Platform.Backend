using LeokaEstetica.Platform.Database.Abstractions.FareRule;
using LeokaEstetica.Platform.Database.Abstractions.Subscription;
using LeokaEstetica.Platform.Models.Dto.Output.Resume;

namespace LeokaEstetica.Platform.Services.Abstractions.Resume;

public interface IFillColorResumeService
{
    /// <summary>
    /// Метод устанавливает свойство IsSelectedColor = true для резюме пользователей с подпиской выше бизнес.
    /// </summary>
    /// <param name="resumesList">Список ризюме.</param>
    /// <param name="subscriptionRepository">Сервис подписок.</param>
    /// <param name="fareRuleRepository">Сервис правил тарифов.</param>
    /// <returns>Список ризюме, с проставленным флагом IsSelectedColor.</returns>
    public Task<IEnumerable<ResumeOutput>> SetColorBusinessResume(List<ResumeOutput> resumesList,
        ISubscriptionRepository subscriptionRepository, IFareRuleRepository fareRuleRepository);
}