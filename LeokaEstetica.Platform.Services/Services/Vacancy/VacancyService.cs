using AutoMapper;
using LeokaEstetica.Platform.Core.Extensions;
using LeokaEstetica.Platform.Core.Helpers;
using LeokaEstetica.Platform.Database.Abstractions.User;
using LeokaEstetica.Platform.Database.Abstractions.Vacancy;
using LeokaEstetica.Platform.Logs.Abstractions;
using LeokaEstetica.Platform.Models.Dto.Output.Vacancy;
using LeokaEstetica.Platform.Moderation.Abstractions.Vacancy;
using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.Notifications.Consts;
using LeokaEstetica.Platform.Redis.Abstractions.Vacancy;
using LeokaEstetica.Platform.Redis.Models.Vacancy;
using LeokaEstetica.Platform.Services.Abstractions.Vacancy;
using VacancyItems = LeokaEstetica.Platform.Redis.Models.Vacancy.VacancyItems;

namespace LeokaEstetica.Platform.Services.Services.Vacancy;

/// <summary>
/// Класс реализует методы сервиса вакансий.
/// </summary>
public sealed class VacancyService : IVacancyService
{
    private readonly ILogService _logService;
    private readonly IVacancyRepository _vacancyRepository;
    private readonly IMapper _mapper;
    private readonly IVacancyRedisService _vacancyRedisService;
    private readonly IUserRepository _userRepository;
    private readonly IVacancyModerationService _vacancyModerationService;
    private readonly INotificationsService _notificationsService;

    /// <summary>
    /// Если не заполнили название вакансии.
    /// </summary>
    private const string ERROR_EMPTY_VACANCY_NAME = "Название вакансии не может быть пустым.";
    
    /// <summary>
    /// Если не заполнили описание вакансии.
    /// </summary>
    private const string ERROR_EMPTY_VACANCY_TEXT = "Описание вакансии не может быть пустым.";

    public VacancyService(ILogService logService, 
        IVacancyRepository vacancyRepository, 
        IMapper mapper, 
        IVacancyRedisService vacancyRedisService, 
        IUserRepository userRepository, 
        IVacancyModerationService vacancyModerationService, 
        INotificationsService notificationsService)
    {
        _logService = logService;
        _vacancyRepository = vacancyRepository;
        _mapper = mapper;
        _vacancyRedisService = vacancyRedisService;
        _userRepository = userRepository;
        _vacancyModerationService = vacancyModerationService;
        _notificationsService = notificationsService;
    }

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
    /// Метод создает вакансию.
    /// </summary>
    /// <param name="vacancyName">Название вакансии.</param>
    /// <param name="vacancyText">Описание вакансии.</param>
    /// <param name="workExperience">Опыт работы.</param>
    /// <param name="employment">Занятость у вакансии.</param>
    /// <param name="payment">Оплата у вакансии.</param>
    /// <param name="account">Аккаунт пользователя.</param>
    /// <returns>Данные созданной вакансии.</returns>
    public async Task<CreateVacancyOutput> CreateVacancyAsync(string vacancyName, string vacancyText, string workExperience, string employment, string payment, string account)
    {
        try
        {
            var result = new CreateVacancyOutput();
            ValidateCreateVacancy(ref result, vacancyName, vacancyText, account);

            if (result.Errors.Any())
            {
                result.IsSuccess = false;
                
                return result;
            }

            var userId = await _userRepository.GetUserByEmailAsync(account);

            // Добавляем вакансию в таблицу вакансий пользователя.
            var createdVacancy = await _vacancyRepository
                .CreateVacancyAsync(vacancyName, vacancyText, workExperience, employment, payment, userId);
            
            // Добавляем вакансию в таблицу статусов вакансий. Проставляем новой вакансии статус "На модерации". 
            await _vacancyRepository.AddVacancyStatusAsync(createdVacancy.VacancyId, VacancyStatusNameEnum.Moderation.GetEnumDescription(), VacancyStatusNameEnum.Moderation.ToString());
            
            // Отправляем вакансию на модерацию.
            await _vacancyModerationService.AddVacancyModerationAsync(createdVacancy.VacancyId);
            
            // Отправляем уведомление об успешном создании вакансии и отправки ее на модерацию.
            await _notificationsService.SendNotificationSuccessCreatedUserVacancyAsync("Все хорошо", "Данные успешно сохранены! Вакансия отправлена на модерацию!", NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS);
            
            result = _mapper.Map<CreateVacancyOutput>(createdVacancy);
            result.IsSuccess = true;

            return result;
        }
        
        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex);
            throw;
        }
    }

    /// <summary>
    /// TODO: Аккаунт возможно нужкн будет использовать, если будет монетизация в каталоге вакансий. Если доступ будет только у тех пользователей, которые приобрели подписку.
    /// Метод получает список вакансий для каталога.
    /// </summary>
    /// <param name="account">Аккаунт пользователя.</param>
    /// <returns>Список вакансий.</returns>
    public async Task<CatalogVacancyResultOutput> CatalogVacanciesAsync(string account)
    {
        try
        {
            var result = new CatalogVacancyResultOutput();
            var userId = await _userRepository.GetUserByEmailAsync(account);
            result.CatalogVacancies = await _vacancyRepository.CatalogVacanciesAsync(userId);

            if (!result.CatalogVacancies.Any())
            {
                return result;
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
    /// Метод валидирует входные параметры при создании вакансии.
    /// </summary>
    /// <param name="result">Выходные данные. Писать ошибки валидации туда, если будут.</param>
    /// <param name="vacancyName">Название вакансии.</param>
    /// <param name="vacancyText">Описание вакансии.</param>
    /// <param name="account">Аккаунт пользователя.</param>
    private void ValidateCreateVacancy(ref CreateVacancyOutput result, string vacancyName, string vacancyText, string account)
    {
        if (string.IsNullOrEmpty(vacancyName))
        {
            var ex = new ArgumentNullException(ERROR_EMPTY_VACANCY_NAME);
            result.Errors.Add(ERROR_EMPTY_VACANCY_NAME);
            _logService.LogError(ex);
        }
        
        if (string.IsNullOrEmpty(vacancyText))
        {
            var ex = new ArgumentNullException(ERROR_EMPTY_VACANCY_TEXT);
            result.Errors.Add(ERROR_EMPTY_VACANCY_TEXT);
            _logService.LogError(ex);
        }
        
        if (string.IsNullOrEmpty(account))
        {
            var ex = new ArgumentNullException($"Не передан аккаунт пользователя.");
            _logService.LogError(ex);
        }
    }
}