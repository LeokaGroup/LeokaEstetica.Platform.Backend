using System.Runtime.CompilerServices;
using LeokaEstetica.Platform.Access.Enums;
using LeokaEstetica.Platform.Core.Extensions;
using LeokaEstetica.Platform.Database.Abstractions.FareRule;
using LeokaEstetica.Platform.Database.Abstractions.Subscription;
using LeokaEstetica.Platform.Models.Dto.Output.Resume;
using LeokaEstetica.Platform.Services.Abstractions.Resume;

[assembly: InternalsVisibleTo("LeokaEstetica.Platform.Tests")]

namespace LeokaEstetica.Platform.Services.Services.Resume;

/// <summary>
/// Сервис выделение цветом резюме пользователей.
/// </summary>
internal sealed class FillColorResumeService : IFillColorResumeService
{
    /// <summary>
    /// Список названий тарифов, которые дают выделение цветом.
    /// </summary>
    private static readonly List<string> _fareRuleTypesNames = new()
    {
        FareRuleTypeEnum.Business.GetEnumDescription(),
        FareRuleTypeEnum.Professional.GetEnumDescription()
    };
    
    /// <summary>
    /// Метод устанавливает свойство IsSelectedColor = true для резюме пользователей с подпиской выше бизнес.
    /// </summary>
    /// <param name="resumesList">Список резюме.</param>
    /// <param name="subscriptionRepository">Сервис подписок.</param>
    /// <param name="fareRuleRepository">Сервис правил тарифов.</param>
    /// <returns>Список резюме, с проставленным флагом IsSelectedColor.</returns>
    public async Task<IEnumerable<ResumeOutput>> SetColorBusinessResume(List<ResumeOutput> resumesList,
        ISubscriptionRepository subscriptionRepository, IFareRuleRepository fareRuleRepository)
    {
        // Получаем ИДшники пользователей.
        var userIds = resumesList.Select(resume => resume.UserId).Distinct();
        
        // Получаем список подписок.
        var subscriptions = await subscriptionRepository.GetSubscriptionsAsync();
        
        // Получаем список тарифов.
        var fareRules = await fareRuleRepository.GetFareRulesAsync();
        var fareRulesList = fareRules.ToList();
        
        // Получаем подписки пользователей.
        var usersSubscriptions = await subscriptionRepository.GetUsersSubscriptionsAsync((IEnumerable<long>)userIds);
        
        foreach (var resume in resumesList)
        {
            // Ищем подписку пользователя-владельца резюме.
            var resumeOwnerSubscription = usersSubscriptions.Find(sub => sub.UserId == resume.UserId);
            
            if (resumeOwnerSubscription is null)
            {
                continue;
            }
            
            // Получаем информацию о подписке.
            var subscriptionId = resumeOwnerSubscription.SubscriptionId;
            var subscriptionEntity = subscriptions.Find(sub => sub.ObjectId == subscriptionId);

            if (subscriptionEntity is null)
            {
                continue;
            }

            // Получаем тариф подписки.
            var fareRuleEntity = fareRulesList.Find(fr => fr.RuleId == subscriptionEntity.ObjectId);
            
            if (fareRuleEntity is null)
            {
                continue;
            }
            
            // Проставляем цвета, если название тарифа пользователя находится в списке _fareRuleTypesNames.
            if (_fareRuleTypesNames.Contains(fareRuleEntity.Name))
            {
                resume.IsSelectedColor = true;
            }
        }

        return resumesList;
    }
}