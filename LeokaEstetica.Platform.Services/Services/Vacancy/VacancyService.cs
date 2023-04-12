using AutoMapper;
using LeokaEstetica.Platform.Access.Abstractions.AvailableLimits;
using LeokaEstetica.Platform.Core.Exceptions;
using LeokaEstetica.Platform.Database.Abstractions.FareRule;
using LeokaEstetica.Platform.Database.Abstractions.Project;
using LeokaEstetica.Platform.Database.Abstractions.Subscription;
using LeokaEstetica.Platform.Database.Abstractions.User;
using LeokaEstetica.Platform.Database.Abstractions.Vacancy;
using LeokaEstetica.Platform.Finder.Chains.Vacancy;
using LeokaEstetica.Platform.Logs.Abstractions;
using LeokaEstetica.Platform.Messaging.Abstractions.Mail;
using LeokaEstetica.Platform.Models.Dto.Input.Vacancy;
using LeokaEstetica.Platform.Models.Dto.Output.Configs;
using LeokaEstetica.Platform.Models.Dto.Output.Vacancy;
using LeokaEstetica.Platform.Models.Entities.Vacancy;
using LeokaEstetica.Platform.CallCenter.Abstractions.Vacancy;
using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.Notifications.Consts;
using LeokaEstetica.Platform.Redis.Abstractions.Vacancy;
using LeokaEstetica.Platform.Redis.Models.Vacancy;
using LeokaEstetica.Platform.Services.Abstractions.Vacancy;
using LeokaEstetica.Platform.Services.Builders;
using VacancyItems = LeokaEstetica.Platform.Redis.Models.Vacancy.VacancyItems;
using LeokaEstetica.Platform.Core.Helpers;
using LeokaEstetica.Platform.Core.Extensions;

namespace LeokaEstetica.Platform.Services.Services.Vacancy;

/// <summary>
/// Класс реализует методы сервиса вакансий.
/// </summary>
public class VacancyService : IVacancyService
{
    private readonly ILogService _logService;
    private readonly IVacancyRepository _vacancyRepository;
    private readonly IMapper _mapper;
    private readonly IVacancyRedisService _vacancyRedisService;
    private readonly IUserRepository _userRepository;
    private readonly IVacancyModerationService _vacancyModerationService;
    private readonly IFillColorVacanciesService _fillColorVacanciesService;

    // Определяем всю цепочку фильтров.
    private readonly BaseVacanciesFilterChain _salaryFilterVacanciesChain = new DateVacanciesFilterChain();
    private readonly BaseVacanciesFilterChain _descSalaryVacanciesFilterChain = new DescSalaryVacanciesFilterChain();
    private readonly BaseVacanciesFilterChain _ascSalaryVacanciesFilterChain = new AscSalaryVacanciesFilterChain();

    private readonly BaseVacanciesFilterChain _fullEmploymentVacanciesFilterChain =
        new FullEmploymentVacanciesFilterChain();

    private readonly BaseVacanciesFilterChain _manySixExperienceVacanciesFilterChain =
        new ManySixExperienceVacanciesFilterChain();

    private readonly BaseVacanciesFilterChain _notExperienceVacanciesFilterChain =
        new NotExperienceVacanciesFilterChain();

    private readonly BaseVacanciesFilterChain _notPayVacanciesFilterChain = new NotPayVacanciesFilterChain();

    private readonly BaseVacanciesFilterChain _oneThreeExperienceVacanciesFilterChain =
        new OneThreeExperienceVacanciesFilterChain();

    private readonly BaseVacanciesFilterChain _partialEmploymentVacanciesFilterChain =
        new PartialEmploymentVacanciesFilterChain();

    private readonly BaseVacanciesFilterChain _payVacanciesFilterChain = new PayVacanciesFilterChain();

    private readonly BaseVacanciesFilterChain _projectWorkEmploymentVacanciesFilterChain =
        new ProjectWorkEmploymentVacanciesFilterChain();

    private readonly BaseVacanciesFilterChain _threeSixExperienceVacanciesFilterChain =
        new ThreeSixExperienceVacanciesFilterChain();

    private readonly BaseVacanciesFilterChain _unknownExperienceVacanciesFilterChain =
        new UnknownExperienceVacanciesFilterChain();

    private readonly BaseVacanciesFilterChain _unknownPayVacanciesFilterChain = new UnknownPayVacanciesFilterChain();

    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IFareRuleRepository _fareRuleRepository;
    private readonly IAvailableLimitsService _availableLimitsService;
    private readonly IVacancyNotificationsService _vacancyNotificationsService;
    
    private static readonly string _approveVacancy = "Опубликована";

    private readonly IProjectRepository _projectRepository;
    private readonly IMailingsService _mailingsService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="logService">Сервис логера.</param>
    /// <param name="vacancyRepository">Репозиторий вакансий.</param>
    /// <param name="mapper">Автомаппер.</param>
    /// <param name="vacancyRedisService">Сервис вакансий кэша.</param>
    /// <param name="userRepository">Репозиторий пользователя.</param>
    /// <param name="vacancyModerationService">Сервис модерации вакансий.</param>
    public VacancyService(ILogService logService,
        IVacancyRepository vacancyRepository,
        IMapper mapper,
        IVacancyRedisService vacancyRedisService,
        IUserRepository userRepository,
        IVacancyModerationService vacancyModerationService,
        ISubscriptionRepository subscriptionRepository,
        IFareRuleRepository fareRuleRepository,
        IAvailableLimitsService availableLimitsService,
        IVacancyNotificationsService vacancyNotificationsService, 
        IProjectRepository projectRepository,
        IFillColorVacanciesService fillColorVacanciesService, 
        IMailingsService mailingsService)
    {
        _logService = logService;
        _vacancyRepository = vacancyRepository;
        _mapper = mapper;
        _vacancyRedisService = vacancyRedisService;
        _userRepository = userRepository;
        _vacancyModerationService = vacancyModerationService;
        _subscriptionRepository = subscriptionRepository;
        _fareRuleRepository = fareRuleRepository;
        _availableLimitsService = availableLimitsService;
        _vacancyNotificationsService = vacancyNotificationsService;
        _projectRepository = projectRepository;
        _fillColorVacanciesService = fillColorVacanciesService;
        _mailingsService = mailingsService;

        // Определяем обработчики цепочки фильтров.
        _salaryFilterVacanciesChain.Successor = _descSalaryVacanciesFilterChain;
        _descSalaryVacanciesFilterChain.Successor = _ascSalaryVacanciesFilterChain;
        _ascSalaryVacanciesFilterChain.Successor = _fullEmploymentVacanciesFilterChain;
        _fullEmploymentVacanciesFilterChain.Successor = _manySixExperienceVacanciesFilterChain;
        _manySixExperienceVacanciesFilterChain.Successor = _notExperienceVacanciesFilterChain;
        _notExperienceVacanciesFilterChain.Successor = _notPayVacanciesFilterChain;
        _notPayVacanciesFilterChain.Successor = _oneThreeExperienceVacanciesFilterChain;
        _oneThreeExperienceVacanciesFilterChain.Successor = _partialEmploymentVacanciesFilterChain;
        _partialEmploymentVacanciesFilterChain.Successor = _payVacanciesFilterChain;
        _payVacanciesFilterChain.Successor = _projectWorkEmploymentVacanciesFilterChain;
        _projectWorkEmploymentVacanciesFilterChain.Successor = _threeSixExperienceVacanciesFilterChain;
        _threeSixExperienceVacanciesFilterChain.Successor = _unknownExperienceVacanciesFilterChain;
        _unknownExperienceVacanciesFilterChain.Successor = _unknownPayVacanciesFilterChain;
    }

    #region Публичные методы.

    /// <summary>
    /// Метод получает список меню вакансий.
    /// </summary>
    /// <returns>Список меню.</returns>
    public async Task<VacancyMenuItemsResultOutput> VacanciesMenuItemsAsync()
    {
        try
        {
            var items = await _vacancyRepository.VacanciesMenuItemsAsync();
            var result = _mapper.Map<VacancyMenuItemsResultOutput>(items);

            // Добавляем меню профиля в кэш.
            var model = CreateFactoryModelToRedis(result.VacancyMenuItems);
            await _vacancyRedisService.SaveVacancyMenuCacheAsync(model);

            return result;
        }

        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex);
            throw;
        }
    }

    /// <summary>
    /// Метод создает вакансию.
    /// </summary>
    /// <param name="vacancyInput">Входная модель.</param>
    /// <returns>Данные созданной вакансии.</returns>
    public async Task<VacancyOutput> CreateVacancyAsync(VacancyInput vacancyInput)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(vacancyInput.Account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(vacancyInput.Account);
                throw ex;
            }

            // Получаем подписку пользователя.
            var userSubscription = await _subscriptionRepository.GetUserSubscriptionAsync(userId);

            // Получаем тариф, на который оформлена подписка у пользователя.
            var fareRule = await _fareRuleRepository.GetByIdAsync(userSubscription.ObjectId);

            // Проверяем доступо ли пользователю создание вакансии.
            var availableCreateProjectLimit = await _availableLimitsService
                .CheckAvailableCreateVacancyAsync(userId, fareRule.Name);

            // Если лимит по тарифу превышен.
            if (!availableCreateProjectLimit)
            {
                var ex = new Exception($"Превышен лимит вакансий по тарифу. UserId: {userId}. Тариф: {fareRule.Name}");

                await _vacancyNotificationsService.SendNotificationWarningLimitFareRuleVacanciesAsync(
                    "Что то пошло не так",
                    "Превышен лимит вакансий по тарифу.",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_WARNING, vacancyInput.Token);
                
                throw ex;
            }

            // Добавляем вакансию в таблицу вакансий пользователя.
            var createdVacancy = await _vacancyRepository.CreateVacancyAsync(vacancyInput.VacancyName,
                vacancyInput.VacancyText, vacancyInput.WorkExperience, vacancyInput.Employment, vacancyInput.Payment,
                userId);
            
            // Привязываем вакансию к проекту.
            await _projectRepository.AttachProjectVacancyAsync(vacancyInput.ProjectId, createdVacancy.VacancyId);

            // Отправляем вакансию на модерацию.
            await _vacancyModerationService.AddVacancyModerationAsync(createdVacancy.VacancyId);

            // Отправляем уведомление об успешном создании вакансии и отправки ее на модерацию.
            await _vacancyNotificationsService.SendNotificationSuccessCreatedUserVacancyAsync("Все хорошо",
                "Данные успешно сохранены. Вакансия отправлена на модерацию.",
                NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS, vacancyInput.Token);
            
            var user = await _userRepository.GetUserPhoneEmailByUserIdAsync(userId);
            
            // Отправляем уведомление о созданной вакансии владельцу.
            await _mailingsService.SendNotificationCreateVacancyAsync(user.Email, createdVacancy.VacancyName,
                createdVacancy.VacancyId);
            
            var result = _mapper.Map<VacancyOutput>(createdVacancy);

            return result;
        }

        catch (Exception ex)
        {
            await _vacancyNotificationsService.SendNotificationErrorCreatedUserVacancyAsync("Ошибка",
                "Ошибка при создании вакансии. Мы уже знаем о ней и разбираемся. " +
                "А пока, попробуйте еще раз.",
                NotificationLevelConsts.NOTIFICATION_LEVEL_ERROR, vacancyInput.Token);
            
            await _logService.LogErrorAsync(ex);
            throw;
        }
    }

    /// <summary>
    /// Метод получает список вакансий для каталога.
    /// </summary>
    /// <returns>Список вакансий.</returns>
    public async Task<CatalogVacancyResultOutput> CatalogVacanciesAsync()
    {
        try
        {
            var catalogVacancies = await _vacancyRepository.CatalogVacanciesAsync();
            var result = new CatalogVacancyResultOutput { CatalogVacancies = new List<CatalogVacancyOutput>() };

            if (!catalogVacancies.Any())
            {
                return result;
            }

            // Выбираем пользователей, у которых есть подписка выше бизнеса. Только их выделяем цветом.
            result.CatalogVacancies = await _fillColorVacanciesService.SetColorBusinessVacancies(catalogVacancies,
                _subscriptionRepository, _fareRuleRepository);

            // Очистка описание от тегов список вакансий для каталога
            ClearCatalogVacanciesHtmlTags(ref catalogVacancies);
            
            // Проставляем вакансиям теги, в зависимости от подписки владельца вакансии.
            result.CatalogVacancies = await SetVacanciesTags(result.CatalogVacancies.ToList());

            return result;
        }

        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex);
            throw;
        }
    }

    /// <summary>
    /// Метод получает названия полей для таблицы вакансий проектов пользователя.
    /// Все названия столбцов этой таблицы одинаковые у всех пользователей.
    /// </summary>
    /// <returns>Список названий полей таблицы.</returns>
    public async Task<IEnumerable<ProjectVacancyColumnNameOutput>> ProjectUserVacanciesColumnsNamesAsync()
    {
        try
        {
            var items = await _vacancyRepository.ProjectUserVacanciesColumnsNamesAsync();

            if (!items.Any())
            {
                throw new InvalidOperationException("Не удалось получить поля для таблицы ProjectVacancyColumnsNames.");
            }

            var result = _mapper.Map<IEnumerable<ProjectVacancyColumnNameOutput>>(items);

            return result;
        }

        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex);
            throw;
        }
    }

    /// <summary>
    /// Метод получает вакансию по ее Id.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Данные вакансии.</returns>
    public async Task<VacancyOutput> GetVacancyByVacancyIdAsync(long vacancyId, string account)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }

            var vacancy = await _vacancyRepository.GetVacancyByVacancyIdAsync(vacancyId);

            if (vacancy is null)
            {
                throw new InvalidOperationException(
                    $"Не удалось получить вакансию. VacancyId: {vacancyId}. UserId: {userId}");
            }

            var result = await CreateVacancyResultAsync(vacancy, userId);

            return result;
        }

        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex);
            throw;
        }
    }

    /// <summary>
    /// Метод обновляет вакансию.
    /// </summary>
    /// <param name="vacancyInput">Входная модель.</param>
    /// <returns>Данные созданной вакансии.</returns>
    public async Task<UserVacancyEntity> UpdateVacancyAsync(VacancyInput vacancyInput)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(vacancyInput.Account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(vacancyInput.Account);
                throw ex;
            }

            // Добавляем вакансию в таблицу вакансий пользователя.
            var createdVacancy = await _vacancyRepository.UpdateVacancyAsync(vacancyInput.VacancyName,
                vacancyInput.VacancyText, vacancyInput.WorkExperience, vacancyInput.Employment, vacancyInput.Payment,
                userId, vacancyInput.VacancyId);

            // Отправляем вакансию на модерацию.
            await _vacancyModerationService.AddVacancyModerationAsync(createdVacancy.VacancyId);

            // Отправляем уведомление об успешном изменении вакансии и отправки ее на модерацию.
            await _vacancyNotificationsService.SendNotificationSuccessCreatedUserVacancyAsync("Все хорошо",
                "Данные успешно сохранены. Вакансия отправлена на модерацию.",
                NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS, vacancyInput.Token);

            return createdVacancy;
        }

        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex);
            throw;
        }
    }

    /// <summary>
    /// Метод фильтрации вакансий в зависимости от параметров фильтров.
    /// </summary>
    /// <param name="filters">Фильтры.</param>
    /// <returns>Список вакансий после фильтрации.</returns>
    public async Task<CatalogVacancyResultOutput> FilterVacanciesAsync(FilterVacancyInput filters)
    {
        try
        {
            var result = new CatalogVacancyResultOutput();

            // Разбиваем строку занятости, так как там может приходить несколько значений в строке.
            filters.Employments = CreateEmploymentsBuilder.CreateEmploymentsResult(filters.EmploymentsValues);
            
            var items = await _vacancyRepository.GetFiltersVacanciesAsync();
            
            result.CatalogVacancies = await _salaryFilterVacanciesChain.FilterVacanciesAsync(filters, items);

            return result;
        }

        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex);
            throw;
        }
    }

    /// <summary>
    /// Метод удаляет вакансию.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <param name="account">Аккаунт.</param>
    /// <param name="token">Токен пользователя.</param>
    public async Task DeleteVacancyAsync(long vacancyId, string account, string token)
    {
        try
        {
            var userId = await _userRepository.GetUserIdByEmailOrLoginAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }
            
            if (vacancyId <= 0)
            {
                var ex = new ArgumentNullException($"Id вакансии не может быть пустым. VacancyId: {vacancyId}");
                
                await _vacancyNotificationsService.SendNotificationErrorDeleteVacancyAsync("Что то пошло не так",
                    "Ошибка при удалении вакансии. Мы уже знаем о проблеме и уже занимаемся ей.",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_ERROR, token);
                
                throw ex;
            }

            // Только владелец вакансии может удалять вакансии проекта.
            var isOwner = await _vacancyRepository.CheckVacancyOwnerAsync(vacancyId, userId);

            if (!isOwner)
            {
                var ex = new InvalidOperationException(
                    $"Пользователь не является владельцем вакансии. UserId: {userId}");
                throw ex;
            }
            
            var removedVacancy = await _vacancyRepository.DeleteVacancyAsync(vacancyId, userId);
            
            if (!removedVacancy.Success)
            {
                var ex = new InvalidOperationException(
                    "Ошибка удаления вакансии. " +
                    $"VacancyId: {vacancyId}. " +
                    $"UserId: {userId}");
            
                await _vacancyNotificationsService.SendNotificationErrorDeleteVacancyAsync(
                    "Ошибка",
                    "Ошибка при удалении вакансии.",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_ERROR, token);
                throw ex;
            }
        
            await _vacancyNotificationsService.SendNotificationSuccessDeleteVacancyAsync(
                "Все хорошо",
                "Вакансия успешно удалена.",
                NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS, token);
            
            var user = await _userRepository.GetUserPhoneEmailByUserIdAsync(userId);
            
            // Отправляем уведомление на почту владельцу вакансии.
            await _mailingsService.SendNotificationDeleteVacancyAsync(user.Email, removedVacancy.VacancyName);
        }
        
        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex);
            throw;
        }
    }

    /// <summary>
    /// Метод получает список вакансий пользователя.
    /// </summary>
    /// <returns>Список вакансий.</returns>
    public async Task<VacancyResultOutput> GetUserVacanciesAsync(string account)
    {
        try
        {
            var result = new VacancyResultOutput();
            var userId = await _userRepository.GetUserIdByEmailOrLoginAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }

            var items = await _vacancyRepository.GetUserVacanciesAsync(userId);

            var userVacancyEntities = items.ToList();
            
            if (userVacancyEntities.Any())
            {
                result = new VacancyResultOutput
                {
                    Vacancies = _mapper.Map<IEnumerable<VacancyOutput>>(items)
                };
            }

            var vacancies = result.Vacancies.ToList();

            // Проставляем вакансиям статусы.
            result.Vacancies = await FillVacanciesStatuses(vacancies);
            
            // Очищаем теги.
            result.Vacancies = ClearVacanciesHtmlTags(vacancies);

            return result;
        }
        
        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex);
            throw;
        }
    }

    /// <summary>
    /// Метод добавляет вакансию в архив.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <param name="account">Аккаунт.</param>
    public async Task AddVacancyArchiveAsync(long vacancyId, string account)
    {
        try
        {
            if (vacancyId <= 0)
            {
                var ex = new ArgumentNullException($"Id вакансии не может быть пустым. VacancyId: {vacancyId}");
                throw ex;
            }

            //Получаем id пользователя
            var userId = await _userRepository.GetUserIdByEmailOrLoginAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }

            // Только владелец вакансии может добавлять вакансию в архив.
            var isOwner = await _vacancyRepository.CheckVacancyOwnerAsync(vacancyId, userId);

            if (!isOwner)
            {
                var ex = new InvalidOperationException(
                    $"Пользователь не является владельцем вакансии. VacancyId: {vacancyId}. UserId: {userId}");
                throw ex;
            }

            //Добавляем вакансию в таблицу архивов
            await _vacancyRepository.AddVacancyArchiveAsync(vacancyId, userId);
        }

        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex);
            throw;
        }
    }

    /// <summary>
    /// Метод получает список вакансий пользователя из архива.
    /// </summary>
    /// <param name="account">Аккаунт пользователя.</param>
    /// <returns>Список архивированных проектов.</returns>
    public async Task<VacancyArchiveResultOutput> GetUserVacanciesArchiveAsync(string account)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                throw new NotFoundUserIdByAccountException(account);
            }

            // Находим вакансии в архиве
            var archivedVacancies = await _vacancyRepository.GetUserVacanciesArchiveAsync(userId);

            var archivedVacanciesOutput = _mapper.Map<List<VacancyArchiveOutput>>(archivedVacancies);

            // Проставляем статусы
            archivedVacanciesOutput.ForEach(p => p.VacancyStatusName = VacancyStatusNameEnum.Archived
                .GetEnumDescription());

            // Формируем выходную модель
            var resultOutput = new VacancyArchiveResultOutput
            {
                Vacancies = archivedVacanciesOutput
            };

            // Очищаем теги
            foreach (var vacancy in archivedVacanciesOutput)
            {
                vacancy.VacancyText = ClearHtmlBuilder.Clear(vacancy.VacancyText);
            }

            return resultOutput;
        }

        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex);
            throw;
        }
    }
    #endregion

    #region Приватные методы.

    /// <summary>
    /// Метод создает модель для сохранения в кэше Redis.
    /// </summary>
    /// <param name="items">Список меню.</param>
    /// <returns>Модель для сохранения.</returns>
    private VacancyMenuRedis CreateFactoryModelToRedis(IReadOnlyCollection<VacancyMenuItemsOutput> items)
    {
        var model = new VacancyMenuRedis
        {
            VacancyMenuItems = new List<VacancyMenuItemsRedis>(items.Select(pmi => new VacancyMenuItemsRedis
            {
                SysName = pmi.SysName,
                Label = pmi.Label,
                Url = pmi.Url,
                Items = pmi.Items.Select(i => new VacancyItems
                    {
                        SysName = i.SysName,
                        Label = i.Label,
                        Url = i.Url,
                    })
                    .ToList()
            }))
        };

        return model;
    }

    /// <summary>
    /// Метод создает результат вакансии.
    /// </summary>
    /// <param name="vacancy">Данные вакансии.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Результирующая модель.</returns>
    private async Task<VacancyOutput> CreateVacancyResultAsync(UserVacancyEntity vacancy, long userId)
    {
        var result = _mapper.Map<VacancyOutput>(vacancy);
        var vacancyOwnerId = await _vacancyRepository.GetVacancyOwnerIdAsync(vacancy.VacancyId);

        // Если владелец вакансии.
        if (vacancyOwnerId == userId)
        {
            result.IsVisibleDeleteButton = true;
            result.IsVisibleSaveButton = true;
            result.IsVisibleEditButton = true;
        }

        return result;
    }

    /// <summary>
    /// Метод чистит описание от тегов список вакансий для каталога.
    /// </summary>
    /// <param name="vacancies">Список вакансий.</param>
    /// <returns>Список вакансий после очистки.</returns>
    private void ClearCatalogVacanciesHtmlTags(ref List<CatalogVacancyOutput> vacancies)
    {
        // Чистим описание вакансии от html-тегов.
        foreach (var vac in vacancies)
        {
            vac.VacancyText = ClearHtmlBuilder.Clear(vac.VacancyText);
        }
    }
    
    /// <summary>
    /// Метод чистит описание от тегов список вакансий.
    /// </summary>
    /// <param name="vacancies">Список вакансий.</param>
    /// <returns>Список вакансий после очистки.</returns>
    private IEnumerable<VacancyOutput> ClearVacanciesHtmlTags(List<VacancyOutput> vacancies)
    {
        // Чистим описание вакансии от html-тегов.
        foreach (var vac in vacancies)
        {
            vac.VacancyText = ClearHtmlBuilder.Clear(vac.VacancyText);
        }

        return vacancies;
    }
    
    /// <summary>
    /// Метод проставляет статусы вакансиям.
    /// </summary>
    /// <param name="projectVacancies">Список вакансий.</param>
    /// <returns>Список вакансий.</returns>
    private async Task<IEnumerable<VacancyOutput>> FillVacanciesStatuses(
        List<VacancyOutput> vacancies)
    {
        // Получаем список вакансий на модерации.
        var moderationVacancies = await _vacancyModerationService.VacanciesModerationAsync();

        // Получаем список вакансий из каталога вакансий.
        var catalogVacancies = await _vacancyRepository.CatalogVacanciesAsync();

        // Проставляем статусы вакансий.
        foreach (var pv in vacancies)
        {
            // Ищем в модерации вакансий.
            var isVacancy = moderationVacancies.Vacancies.Any(v => v.VacancyId == pv.VacancyId);

            if (isVacancy)
            {
                pv.VacancyStatusName = moderationVacancies.Vacancies
                    .Where(v => v.VacancyId == pv.VacancyId)
                    .Select(v => v.ModerationStatusName)
                    .FirstOrDefault();
            }
                
            // Ищем вакансию в каталоге вакансий.
            else
            {
                var isCatalogVacancy = catalogVacancies.Any(v => v.VacancyId == pv.VacancyId);

                if (isCatalogVacancy)
                {
                    pv.VacancyStatusName = _approveVacancy;
                }
            }
        }

        return vacancies;
    }

    /// <summary>
    /// Метод проставляет флаги вакансиям пользователя в зависимости от его подписки.
    /// </summary>
    /// <param name="vacancies">Список вакансий каталога.</param>
    /// <returns>Список вакансий каталога с проставленными тегами.</returns>
    private async Task<IEnumerable<CatalogVacancyOutput>> SetVacanciesTags(List<CatalogVacancyOutput> vacancies)
    {
        foreach (var v in vacancies)
        {
            // Получаем подписку пользователя.
            var userSubscription = await _subscriptionRepository.GetUserSubscriptionAsync(v.UserId);

            // Такая подписка не дает тегов.
            if (userSubscription.ObjectId < 3)
            {
                continue;
            }
            
            // Если подписка бизнес.
            if (userSubscription.ObjectId == 3)
            {
                v.TagColor = "warning";
                v.TagValue = "Бизнес";
            }
        
            // Если подписка профессиональный.
            if (userSubscription.ObjectId == 4)
            {
                v.TagColor = "warning";
                v.TagValue = "Профессиональный";
            }   
        }

        return vacancies;
    }
    
    #endregion
}