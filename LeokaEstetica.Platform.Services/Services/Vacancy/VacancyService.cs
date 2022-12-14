using AutoMapper;
using LeokaEstetica.Platform.Core.Exceptions;
using LeokaEstetica.Platform.Database.Abstractions.User;
using LeokaEstetica.Platform.Database.Abstractions.Vacancy;
using LeokaEstetica.Platform.Logs.Abstractions;
using LeokaEstetica.Platform.LuceneNet.Chains.Vacancy;
using LeokaEstetica.Platform.Models.Dto.Input.Vacancy;
using LeokaEstetica.Platform.Models.Dto.Output.Configs;
using LeokaEstetica.Platform.Models.Dto.Output.Vacancy;
using LeokaEstetica.Platform.Models.Entities.Vacancy;
using LeokaEstetica.Platform.Moderation.Abstractions.Vacancy;
using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.Notifications.Consts;
using LeokaEstetica.Platform.Redis.Abstractions.Vacancy;
using LeokaEstetica.Platform.Redis.Models.Vacancy;
using LeokaEstetica.Platform.Services.Abstractions.Vacancy;
using LeokaEstetica.Platform.Services.Builders;
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
    public async Task<UserVacancyEntity> CreateVacancyAsync(string vacancyName, string vacancyText,
        string workExperience, string employment, string payment, string account)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            // Добавляем вакансию в таблицу вакансий пользователя.
            var createdVacancy = await _vacancyRepository
                .CreateVacancyAsync(vacancyName, vacancyText, workExperience, employment, payment, userId);

            // Отправляем вакансию на модерацию.
            await _vacancyModerationService.AddVacancyModerationAsync(createdVacancy.VacancyId);

            // Отправляем уведомление об успешном создании вакансии и отправки ее на модерацию.
            await _notificationsService.SendNotificationSuccessCreatedUserVacancyAsync("Все хорошо",
                "Данные успешно сохранены! Вакансия отправлена на модерацию!",
                NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS);

            return createdVacancy;
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
    /// <returns>Список вакансий.</returns>
    public async Task<CatalogVacancyResultOutput> CatalogVacanciesAsync()
    {
        try
        {
            var result = new CatalogVacancyResultOutput
            {
                CatalogVacancies = await _vacancyRepository.CatalogVacanciesAsync()
            };

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
                throw new NullReferenceException("Не удалось получить поля для таблицы ProjectVacancyColumnsNames.");
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
    public async Task<UserVacancyEntity> GetVacancyByVacancyIdAsync(long vacancyId, string account)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                await _logService.LogErrorAsync(ex);
                throw ex;
            }

            var result = await _vacancyRepository.GetVacancyByVacancyIdAsync(vacancyId, userId);

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
    /// <param name="vacancyName">Название вакансии.</param>
    /// <param name="vacancyText">Описание вакансии.</param>
    /// <param name="workExperience">Опыт работы.</param>
    /// <param name="employment">Занятость у вакансии.</param>
    /// <param name="payment">Оплата у вакансии.</param>
    /// <param name="account">Аккаунт пользователя.</param>
    /// <returns>Данные созданной вакансии.</returns>
    public async Task<UserVacancyEntity> UpdateVacancyAsync(string vacancyName, string vacancyText,
        string workExperience, string employment, string payment, string account, long vacancyId)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                await _logService.LogErrorAsync(ex);
                throw ex;
            }

            // Добавляем вакансию в таблицу вакансий пользователя.
            var createdVacancy = await _vacancyRepository.UpdateVacancyAsync(vacancyName, vacancyText, workExperience,
                employment, payment, userId, vacancyId);

            // Отправляем вакансию на модерацию.
            await _vacancyModerationService.AddVacancyModerationAsync(createdVacancy.VacancyId);

            // Отправляем уведомление об успешном изменении вакансии и отправки ее на модерацию.
            await _notificationsService.SendNotificationSuccessCreatedUserVacancyAsync("Все хорошо",
                "Данные успешно сохранены! Вакансия отправлена на модерацию!",
                NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS);

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
            filters.Employments =
                CreateEmploymentsBuilder.CreateEmploymentsResult(filters.EmploymentsValues);
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
}