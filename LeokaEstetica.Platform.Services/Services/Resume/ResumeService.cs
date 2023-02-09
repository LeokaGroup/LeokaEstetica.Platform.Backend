using AutoMapper;
using LeokaEstetica.Platform.Access.Enums;
using LeokaEstetica.Platform.Core.Extensions;
using LeokaEstetica.Platform.Database.Abstractions.FareRule;
using LeokaEstetica.Platform.Database.Abstractions.Resume;
using LeokaEstetica.Platform.Database.Abstractions.Subscription;
using LeokaEstetica.Platform.Logs.Abstractions;
using LeokaEstetica.Platform.Models.Dto.Output.Resume;
using LeokaEstetica.Platform.Models.Entities.Profile;
using LeokaEstetica.Platform.Services.Abstractions.Resume;
using LeokaEstetica.Platform.Services.Builders;

namespace LeokaEstetica.Platform.Services.Services.Resume;

/// <summary>
/// Класс реализует методы сервиса базы резюме.
/// </summary>
public class ResumeService : IResumeService
{
    private readonly ILogService _logService;
    private readonly IResumeRepository _resumeRepository;
    private readonly IMapper _mapper;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IFareRuleRepository _fareRuleRepository;
    
    /// <summary>
    /// Список названий тарифов, которые дают выделение цветом.
    /// </summary>
    private static readonly List<string> _fareRuleTypesNames = new()
    {
        FareRuleTypeEnum.Business.GetEnumDescription(),
        FareRuleTypeEnum.Professional.GetEnumDescription()
    };

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="logService">Сервис логов.</param>
    /// <param name="resumeRepository">Репозиторий базы резюме.</param>
    /// <param name="accessResumeService">Сервис проверки доступа к базе резюме.</param>
    /// <param name="_mapper">Автомаппер.</param>
    public ResumeService(ILogService logService, 
        IResumeRepository resumeRepository, 
        IMapper mapper, 
        ISubscriptionRepository subscriptionRepository, 
        IFareRuleRepository fareRuleRepository)
    {
        _logService = logService;
        _resumeRepository = resumeRepository;
        _mapper = mapper;
        _subscriptionRepository = subscriptionRepository;
        _fareRuleRepository = fareRuleRepository;
    }

    /// <summary>
    /// Метод получает список резюме.
    /// </summary>
    /// <returns>Список резюме.</returns>
    public async Task<ResumeResultOutput> GetProfileInfosAsync()
    {
        try
        {
            var resumes = await _resumeRepository.GetProfileInfosAsync();
            
            // TODO: Временный костыль.Это должна решать модерация и некорректные резюме не будут попадать в каталог.
            // TODO: А это потом надо убрать.
            // Убираем анкеты, которые не проходят по условиям.
            CreateProfileInfosBuilder.CreateProfileInfosResult(ref resumes);
            
            var result = new ResumeResultOutput
            {
                // Приводим к выходной модели.
                CatalogResumes = _mapper.Map<IEnumerable<ResumeOutput>>(resumes)
            };
            
            if (!result.CatalogResumes.Any())
            {
                return result;
            }
            
            // Получаем список юзеров для проставления цветов.
            var userIds = result.CatalogResumes.Select(p => p.UserId).Distinct();
            
            // Выбираем список подписок пользователей.
            var userSubscriptions = await _subscriptionRepository.GetUsersSubscriptionsAsync(userIds);

            // Получаем список подписок.
            var subscriptions = await _subscriptionRepository.GetSubscriptionsAsync();
            
            // Получаем список тарифов, чтобы взять названия тарифов.
            var fareRules = await _fareRuleRepository.GetFareRulesAsync();
            var fareRulesList = fareRules.ToList();

            // Выбираем пользователей, у которых есть подписка выше бизнеса. Только их выделяем цветом.
            foreach (var r in result.CatalogResumes)
            {
                // Смотрим подписку пользователя.
                var userSubscription = userSubscriptions.Find(s => s.UserId == r.UserId);

                if (userSubscription is null)
                {
                    continue;
                }
                
                var subscriptionId = userSubscription.SubscriptionId;
                var s = subscriptions.Find(s => s.ObjectId == subscriptionId);

                if (s is null)
                {
                    continue;
                }
                
                // Получаем название тарифа подписки.
                var sn = fareRulesList.Find(fr => fr.RuleId == s.ObjectId);
                
                if (sn is null)
                {
                    continue;
                }
                        
                // Подписка позволяет. Проставляем выделение цвета.
                if (_fareRuleTypesNames.Contains(sn.Name))
                {
                    r.IsSelectedColor = true;
                }
            }

            return result;
        }
        
        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex);
            throw;
        }
    }

    /// <summary>
    /// Метод получает анкету пользователя.
    /// </summary>
    /// <param name="resumeId">Id анкеты пользователя.</param>
    /// <returns>Данные анкеты.</returns>
    public async Task<ProfileInfoEntity> GetResumeAsync(long resumeId)
    {
        try
        {
            var result = await _resumeRepository.GetResumeAsync(resumeId);

            return result;
        }
        
        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex);
            throw;
        }
    }
}