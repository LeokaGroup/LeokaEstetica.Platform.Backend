using System.Runtime.CompilerServices;
using AutoMapper;
using LeokaEstetica.Platform.Access.Enums;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.User;
using LeokaEstetica.Platform.Core.Exceptions;
using LeokaEstetica.Platform.Database.Abstractions.Project;
using LeokaEstetica.Platform.Database.Abstractions.Subscription;
using LeokaEstetica.Platform.Database.Abstractions.Vacancy;
using LeokaEstetica.Platform.Messaging.Abstractions.Mail;
using LeokaEstetica.Platform.Models.Dto.Input.Vacancy;
using LeokaEstetica.Platform.Models.Dto.Output.Configs;
using LeokaEstetica.Platform.Models.Dto.Output.Vacancy;
using LeokaEstetica.Platform.Models.Entities.Vacancy;
using LeokaEstetica.Platform.CallCenter.Abstractions.Vacancy;
using LeokaEstetica.Platform.Notifications.Consts;
using LeokaEstetica.Platform.Redis.Abstractions.Vacancy;
using LeokaEstetica.Platform.Redis.Models.Vacancy;
using LeokaEstetica.Platform.Services.Abstractions.Vacancy;
using LeokaEstetica.Platform.Services.Builders;
using VacancyItems = LeokaEstetica.Platform.Redis.Models.Vacancy.VacancyItems;
using LeokaEstetica.Platform.Base.Extensions.PriceExtensions;
using LeokaEstetica.Platform.Core.Enums;
using LeokaEstetica.Platform.Database.Abstractions.Moderation.Vacancy;
using LeokaEstetica.Platform.Integrations.Abstractions.Discord;
using LeokaEstetica.Platform.Models.Dto.Output.Moderation.Vacancy;
using LeokaEstetica.Platform.Models.Entities.Moderation;
using LeokaEstetica.Platform.Services.Helpers;
using Microsoft.Extensions.Logging;
using LeokaEstetica.Platform.Base.Extensions.HtmlExtensions;
using LeokaEstetica.Platform.Models.Dto.Input.Commerce;
using LeokaEstetica.Platform.Models.Dto.Output.Orders;
using LeokaEstetica.Platform.Models.Enums;
using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.Processing.Abstractions.Commerce;

[assembly: InternalsVisibleTo("LeokaEstetica.Platform.Tests")]

namespace LeokaEstetica.Platform.Services.Services.Vacancy;

/// <summary>
/// Класс реализует методы сервиса вакансий.
/// </summary>
internal sealed class VacancyService : IVacancyService
{
    private readonly ILogger<VacancyService> _logger;
    private readonly IVacancyRepository _vacancyRepository;
    private readonly IMapper _mapper;
    private readonly IVacancyRedisService _vacancyRedisService;
    private readonly IUserRepository _userRepository;
    private readonly IVacancyModerationService _vacancyModerationService;
    private readonly ISubscriptionRepository _subscriptionRepository;

    private static readonly string _approveVacancy = "Опубликована";
    private static readonly string _archiveVacancy = "В архиве";
    private static readonly string _moderationVacancy = "На модерации";

    private readonly IProjectRepository _projectRepository;
    private readonly IMailingsService _mailingsService;
    private readonly IVacancyModerationRepository _vacancyModerationRepository;
    private readonly IDiscordService _discordService;
    private readonly Lazy<IHubNotificationService> _hubNotificationService;
    private readonly ICommerceService _commerceService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="logger">Сервис логера.</param>
    /// <param name="vacancyRepository">Репозиторий вакансий.</param>
    /// <param name="mapper">Автомаппер.</param>
    /// <param name="vacancyRedisService">Сервис вакансий кэша.</param>
    /// <param name="userRepository">Репозиторий пользователя.</param>
    /// <param name="vacancyModerationService">Сервис модерации вакансий.</param>
    /// <param name="discordService">Сервис уведомления дискорда.</param>
    /// <param name="hubNotificationService">Сервис уведомлений хабов.</param>
    /// <param name="commerceService">Сервис коммерции.</param>
    public VacancyService(ILogger<VacancyService> logger,
        IVacancyRepository vacancyRepository,
        IMapper mapper,
        IVacancyRedisService vacancyRedisService,
        IUserRepository userRepository,
        IVacancyModerationService vacancyModerationService,
        ISubscriptionRepository subscriptionRepository,
        IProjectRepository projectRepository,
        IMailingsService mailingsService,
        IVacancyModerationRepository vacancyModerationRepository,
        IDiscordService discordService,
        Lazy<IHubNotificationService> hubNotificationService,
        ICommerceService commerceService)
    {
        _logger = logger;
        _vacancyRepository = vacancyRepository;
        _mapper = mapper;
        _vacancyRedisService = vacancyRedisService;
        _userRepository = userRepository;
        _vacancyModerationService = vacancyModerationService;
        _subscriptionRepository = subscriptionRepository;

        _projectRepository = projectRepository;
        _mailingsService = mailingsService;
        _vacancyModerationRepository = vacancyModerationRepository;
        _discordService = discordService;
        _hubNotificationService = hubNotificationService;
        _commerceService = commerceService;
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
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Метод создает вакансию.
    /// </summary>
    /// <param name="vacancyInput">Входная модель.</param>
    /// <returns>Данные созданной вакансии.</returns>
    public async Task<CreateOrderOutput> CreateVacancyAsync(VacancyInput vacancyInput)
    {
        var userId = await _userRepository.GetUserByEmailAsync(vacancyInput.Account!);

        if (userId <= 0)
        {
            var ex = new NotFoundUserIdByAccountException(vacancyInput.Account!);
            throw ex;
        }
        
        var userCode = await _userRepository.GetUserCodeByUserIdAsync(userId);
        
        try
        {
            if (vacancyInput.ProjectId == 0)
            {
                await _hubNotificationService.Value.SendNotificationAsync("Ошибка",
                    "Невозможно создать вакансию без проекта.",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_ERROR, "SendNotificationErrorCreatedUserVacancy",
                    userCode, UserConnectionModuleEnum.Main);
                
                var ex = new InvalidOperationException(
                    "Попытка создать вакансию при отсутствии проектов у пользователя. " +
                    $"UserId: {userId}. ");

                await _discordService.SendNotificationErrorAsync(ex);

                throw ex;
            }

            var isProjectOwner = await _projectRepository.CheckProjectOwnerAsync(vacancyInput.ProjectId, userId);

            if (!isProjectOwner)
            {
                var ex = new InvalidOperationException(
                    "Попытка создать вакансию для проекта который не принадлежит пользователю. " +
                    $"UserId: {userId}. " +
                    $"ProjectId: {vacancyInput.ProjectId}. ");

                await _discordService.SendNotificationErrorAsync(ex);

                throw ex;
            }

            // Получаем подписку пользователя.
            var userSubscription = await _subscriptionRepository.GetUserSubscriptionAsync(userId);
            
            if (userSubscription is null)
            {
                throw new InvalidOperationException("Найдена невалидная подписка пользователя. " +
                                                    $"UserId: {userId}. " +
                                                    "Подписка была NULL или невалидная." +
                                                    $"#1 Ошибка в {nameof(VacancyService)}");
            }

            // Добавляем вакансию в кролика после оплаты заказа в ПС.
            var createdOrder = await _commerceService.CreateOrderCacheOrRabbitMqAsync(new CreateOrderInput
            {
                VacancyOrderData = vacancyInput,
                OrderType = OrderTypeEnum.CreateVacancy
            }, vacancyInput.Account!);

            return createdOrder;
        }

        catch (Exception ex)
        {
            await _hubNotificationService.Value.SendNotificationAsync("Ошибка",
                "Ошибка при создании вакансии. Мы уже знаем о ней и разбираемся. " +
                "А пока, попробуйте еще раз.",
                NotificationLevelConsts.NOTIFICATION_LEVEL_ERROR, "SendNotificationErrorCreatedUserVacancy", userCode,
                UserConnectionModuleEnum.Main);

            _logger.LogError(ex, ex.Message);
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
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Метод получает вакансию по ее Id.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <param name="mode">Режим. Чтение или изменение.</param>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Данные вакансии.</returns>
    public async Task<VacancyOutput> GetVacancyByVacancyIdAsync(long vacancyId, ModeEnum mode, string account)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

			var moderationVacancy = await _vacancyModerationService.GetModerationVacancyByVacancyIdAsync(vacancyId);

			if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }
            
            if (vacancyId <= 0)
            {
                throw new InvalidOperationException($"Не передан Id вакансии. VacancyId: {vacancyId}");
            }
            
            // Проверяем доступ к вакансии.
            var isOwner = await _vacancyRepository.CheckVacancyOwnerAsync(vacancyId, userId);

            var result = new VacancyOutput();

			if (!isOwner && moderationVacancy is not null &&
	(moderationVacancy.ModerationStatus.StatusName == _moderationVacancy || moderationVacancy.ModerationStatus.StatusName == _archiveVacancy))
			{
				result.IsAccess = false;
				result.IsSuccess = false;

				return result;
			}
			// Нет доступа на изменение.
			if (!isOwner && mode == ModeEnum.Edit)
            {
                result.IsSuccess = false;
                result.IsAccess = false;

                return result;
            }

            var vacancy = await _vacancyRepository.GetVacancyByVacancyIdAsync(vacancyId);

            //Если нет вакансии в базе
            if (vacancy is null)
            {
				result.IsSuccess = false;
				result.IsAccess = false;

                return result;
			}

            result = await CreateVacancyResultAsync(vacancy, userId);

            var remarks = await _vacancyModerationRepository.GetVacancyRemarksAsync(vacancyId);
            result.VacancyRemarks = _mapper.Map<IEnumerable<VacancyRemarkOutput>>(remarks);
            
            result.IsSuccess = true;
            result.IsAccess = true;

            result.Conditions = ClearHtmlBuilder.Clear(result.Conditions);
            result.Demands = ClearHtmlBuilder.Clear(result.Demands);
            result.VacancyText = ClearHtmlBuilder.Clear(result.VacancyText);
            
            return result;
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
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
            var account = vacancyInput.Account;
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }

            vacancyInput.UserId = userId;

            // Создаем вакансию.
            var createdVacancy = await _vacancyRepository.UpdateVacancyAsync(vacancyInput);

            var vacancyId = vacancyInput.VacancyId;

            // Отправляем вакансию на модерацию.
            await _vacancyModerationService.AddVacancyModerationAsync(vacancyId.GetValueOrDefault());
            
            var userCode = await _userRepository.GetUserCodeByUserIdAsync(userId);
            
            // Отправляем уведомление об успешном изменении вакансии и отправки ее на модерацию.
            await _hubNotificationService.Value.SendNotificationAsync("Все хорошо",
                "Данные успешно сохранены. Вакансия отправлена на модерацию.",
                NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS, "SendNotificationSuccessCreatedUserVacancy",
                userCode, UserConnectionModuleEnum.Main);

            // Проверяем наличие неисправленных замечаний.
            await CheckAwaitingCorrectionRemarksAsync(vacancyId.GetValueOrDefault());

            return createdVacancy;
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

	/// <summary>
	/// Метод получает каталог вакансий с учетом фильтров и пагинации
	/// </summary>
	/// <param name="vacancyCatalogInput">Фильтры с пагинацией.</param>
	/// <returns>Каталог вакансий после фильтрации и пагинации.</returns>
	public async Task<CatalogVacancyResultOutput> GetCatalogVacanciesAsync(VacancyCatalogInput vacancyCatalogInput)
    {
        try
        {
            var result = new CatalogVacancyResultOutput
            {
                CatalogVacancies = new List<CatalogVacancyOutput>()
            };

			// Разбиваем строку занятости, так как там может приходить несколько значений в строке.
			vacancyCatalogInput.Filters.Employments = CreateEmploymentsBuilder.CreateEmploymentsResult(vacancyCatalogInput.Filters.EmploymentsValues);

            result.CatalogVacancies = await _vacancyRepository.GetCatalogVacanciesAsync(vacancyCatalogInput);

            return result;
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Метод удаляет вакансию.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <param name="account">Аккаунт.</param>
    public async Task DeleteVacancyAsync(long vacancyId, string account)
    {
        try
        {
            var userId = await _userRepository.GetUserIdByEmailOrLoginAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }
            
            var userCode = await _userRepository.GetUserCodeByUserIdAsync(userId);

            if (vacancyId <= 0)
            {
                var ex = new ArgumentNullException($"Id вакансии не может быть пустым. VacancyId: {vacancyId}");

                await _hubNotificationService.Value.SendNotificationAsync("Что то пошло не так",
                    "Ошибка при удалении вакансии. Мы уже знаем о проблеме и уже занимаемся ей.",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_ERROR, "SendNotificationErrorDeleteVacancy", userCode,
                    UserConnectionModuleEnum.Main);

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
            
            _logger.LogInformation("Начали удаление вакансии пользователя." +
                                   $" VacancyId: {vacancyId}." +
                                   $" UserId: {userId}");
            
            var removedVacancy = await _vacancyRepository.DeleteVacancyAsync(vacancyId, userId);
            
            if (!removedVacancy.Success)
            {
                var ex = new InvalidOperationException(
                    "Ошибка удаления вакансии. " +
                    $"VacancyId: {vacancyId}. " +
                    $"UserId: {userId}");

                await _hubNotificationService.Value.SendNotificationAsync("Ошибка",
                    "Ошибка при удалении вакансии.",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_ERROR, "SendNotificationErrorDeleteVacancy", userCode,
                    UserConnectionModuleEnum.Main);
                throw ex;
            }

            await _hubNotificationService.Value.SendNotificationAsync("Все хорошо",
                "Вакансия успешно удалена.",
                NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS, "SendNotificationSuccessDeleteVacancy", userCode,
                UserConnectionModuleEnum.Main);

            var user = await _userRepository.GetUserPhoneEmailByUserIdAsync(userId);
            
            // Отправляем уведомление на почту владельцу вакансии.
            await _mailingsService.SendNotificationDeleteVacancyAsync(user.Email, removedVacancy.VacancyName);
            
            _logger.LogInformation("Закончили удаление вакансии пользователя." +
                                   $" VacancyId: {vacancyId}." +
                                   $" UserId: {userId}");
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
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
            var userId = await _userRepository.GetUserIdByEmailOrLoginAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }

            var items = await _vacancyRepository.GetUserVacanciesAsync(userId);
            
            var result = new VacancyResultOutput
            {
                Vacancies = new List<VacancyOutput>()
            };

            if (!items.Any())
            {
                return result;
            }

            var mapVacancies = _mapper.Map<IEnumerable<VacancyOutput>>(items);
            var vacancies = mapVacancies.ToList();

            // Проставляем вакансиям статусы.
            result.Vacancies = await FillVacanciesStatusesAsync(vacancies, userId);
            
            // Очищаем теги.
            result.Vacancies = ClearVacanciesHtmlTags(vacancies);

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
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
        var userId = await _userRepository.GetUserIdByEmailOrLoginAsync(account);

        if (userId <= 0)
        {
            var ex = new NotFoundUserIdByAccountException(account);
            throw ex;
        }
        
        var userCode = await _userRepository.GetUserCodeByUserIdAsync(userId);
        
        try
        {
            if (vacancyId <= 0)
            {
                var ex = new InvalidOperationException($"Id вакансии не может быть <= 0. VacancyId: {vacancyId}");
                throw ex;
            }

            // Только владелец вакансии может добавлять вакансию в архив.
            var isOwner = await _vacancyRepository.CheckVacancyOwnerAsync(vacancyId, userId);

            if (!isOwner)
            {
                var ex = new InvalidOperationException("Пользователь не является владельцем вакансии." +
                                                       "Добавление в архив невозможно." +
                                                       $" VacancyId: {vacancyId}." +
                                                       $" UserId: {userId}");
                throw ex;
            }
            
            // Проверяем, есть ли уже такая вакансия в архиве.
            var isExists = await _vacancyRepository.CheckVacancyArchiveAsync(vacancyId);
            
            if (isExists)
            {
                _logger.LogWarning($"Такая вакансия уже добавлена в архив. VacancyId: {vacancyId}. UserId: {userId}");

                await _hubNotificationService.Value.SendNotificationAsync("Внимание",
                    "Такая вакансия уже добавлена в архив.",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_WARNING, "SendNotificationWarningAddVacancyArchive",
                    userCode, UserConnectionModuleEnum.Main);

                return;
            }

            // Добавляем вакансию в архив.
            await _vacancyRepository.AddVacancyArchiveAsync(vacancyId, userId);

            await _hubNotificationService.Value.SendNotificationAsync("Все хорошо",
                "Вакансия успешно добавлена в архив.",
                NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS, "SendNotificationSuccessAddVacancyArchive",
                userCode, UserConnectionModuleEnum.Main);

            var vacancyName = await _vacancyRepository.GetVacancyNameByIdAsync(vacancyId);

            // Отправляем уведомление на почту.
            await _mailingsService.SendNotificationAddVacancyArchiveAsync(account, vacancyId, vacancyName);
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);

            await _hubNotificationService.Value.SendNotificationAsync("Что то не так...",
                "Ошибка при добавлении вакансии в архив. Мы уже знаем о проблеме и уже занимаемся ей.",
                NotificationLevelConsts.NOTIFICATION_LEVEL_ERROR, "SendNotificationErrorAddVacancyArchive", userCode,
                UserConnectionModuleEnum.Main);

            throw;
        }
    }

    /// <summary>
    /// Метод получает список замечаний вакансии, если они есть.
    /// </summary>
    /// <param name="projectId">Id вакансии.</param>
    /// <param name="account">Аккаунт пользователя.</param>
    /// <returns>Список замечаний вакансии.</returns>
    public async Task<IEnumerable<VacancyRemarkEntity>> GetVacancyRemarksAsync(long vacancyId, string account)
    {
        try
        {
            if (vacancyId <= 0)
            {
                var ex = new InvalidOperationException("Ошибка при получении замечаний вакансии. " +
                                                       $"VacancyId: {vacancyId}");
                throw ex;
            }
        
            var userId = await _userRepository.GetUserIdByEmailAsync(account);
            
            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }

            var isVacancyOwner = await _vacancyRepository.CheckVacancyOwnerAsync(vacancyId, userId);

            if (!isVacancyOwner)
            {
                return Enumerable.Empty<VacancyRemarkEntity>();
            }

            var result = await _vacancyModerationRepository.GetVacancyRemarksAsync(vacancyId);

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Метод удаляет из архива вакансию.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <param name="account">Аккаунт.</param>
    public async Task DeleteVacancyArchiveAsync(long vacancyId, string account)
    {
        var userId = await _userRepository.GetUserIdByEmailOrLoginAsync(account);

        if (userId <= 0)
        {
            var ex = new NotFoundUserIdByAccountException(account);
            throw ex;
        }
        
        var userCode = await _userRepository.GetUserCodeByUserIdAsync(userId);
        
        try
        {
            if (vacancyId <= 0)
            {
                var ex = new InvalidOperationException($"Id проекта не может быть <= 0. ProjectId: {vacancyId}");
                throw ex;
            }

            // Проверяем, является ли текущий пользователь владельцем вакансии.
            var isOwner = await _vacancyRepository.CheckVacancyOwnerAsync(vacancyId, userId);

            // Только владелец может удалить вакансию из архива.
            if (!isOwner)
            {
                throw new InvalidOperationException("Пользователь не является владельцем вакансии." +
                                                    "Удаление из архива невозможно." +
                                                    $"VacancyId: {vacancyId}." +
                                                    $"UserId: {userId}");
            }
            
            // Получаем подписку пользователя.
            var userSubscription = await _subscriptionRepository.GetUserSubscriptionAsync(userId);
            
            if (userSubscription is null)
            {
                throw new InvalidOperationException("Найдена невалидная подписка пользователя. " +
                                                    $"UserId: {userId}. " +
                                                    "Подписка была NULL или невалидная." +
                                                    $"Ошибка в {nameof(VacancyService)}");
            }
            
            // Удаляем вакансию из архива.
            var isDelete = await _vacancyRepository.DeleteVacancyArchiveAsync(vacancyId, userId);

            if (!isDelete)
            {
                await _hubNotificationService.Value.SendNotificationAsync("Что то не так...",
                    "Ошибка при удалении вакансии из архива. Мы уже знаем о проблеме и уже занимаемся ей.",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_ERROR, "SendNotificationErrorDeleteVacancyArchive", userCode,
                    UserConnectionModuleEnum.Main);

                return;
            }
            
            // Отправляем вакансию на модерацию.
            await _vacancyModerationRepository.AddVacancyModerationAsync(vacancyId);

            await _hubNotificationService.Value.SendNotificationAsync("Все хорошо",
                "Вакансия успешно удалена из архива.",
                NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS, "SendNotificationSuccessDeleteVacancyArchive",
                userCode, UserConnectionModuleEnum.Main);
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);

            await _hubNotificationService.Value.SendNotificationAsync("Что то не так...",
                "Ошибка при удалении вакансии из архива. Мы уже знаем о проблеме и уже занимаемся ей.",
                NotificationLevelConsts.NOTIFICATION_LEVEL_ERROR, "SendNotificationErrorDeleteVacancyArchive",
                userCode, UserConnectionModuleEnum.Main);

            throw;
        }
    }

    /// <summary>
    /// Метод получает список вакансий пользователя из архива.
    /// </summary>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Список архивированных вакансий.</returns>
    public async Task<UserVacancyArchiveResultOutput> GetUserVacanciesArchiveAsync(string account)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                throw new NotFoundUserIdByAccountException(account);
            }

            var result = new UserVacancyArchiveResultOutput
            {
                VacanciesArchive = new List<VacancyArchiveOutput>()
            };

            // Находим вакансии в архиве.
            var archivedVacancies = await _vacancyRepository.GetUserVacanciesArchiveAsync(userId);

            var archivedVacancyEntities = archivedVacancies.ToList();
            
            if (!archivedVacancyEntities.Any())
            {
                return result;
            }

            result.VacanciesArchive = _mapper.Map<List<VacancyArchiveOutput>>(archivedVacancies);

            await CreateVacanciesDatesHelper.CreateDatesResultAsync(archivedVacancyEntities,
                result.VacanciesArchive.ToList());

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
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

            var isExists = await _vacancyRepository.CheckVacancyArchiveAsync(vacancy.VacancyId);

            if (!isExists)
            {
                result.IsVisibleActionAddVacancyArchive = true;
            }
        }

        result.VacancyRemarks = new List<VacancyRemarkOutput>();
        result.Payment = result.Payment.CreatePriceWithDelimiterFromString();

        return result;
    }

    /// <summary>
    /// Метод проводит различные форматирования для представления вакансий в каталоге.
    /// <param name="vacancies">Список вакансий.</param>
    /// <returns>Список вакансий.</returns>
    private void FormatCatalogVacancies(List<CatalogVacancyOutput> vacancies)
    {
        foreach (var vac in vacancies)
        {
            // Чистим описание вакансии от html-тегов.
            vac.VacancyText = (vac.VacancyText);
            
            // Форматируем цену к виду 1 000.
            vac.Payment = vac.Payment.CreatePriceWithDelimiterFromString();
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
            if (vac.VacancyText.Length > 40)
            {
                vac.VacancyText = string.Concat(vac.VacancyText.Substring(0, 40), "...");
            }
        }

        return vacancies;
    }
    
    /// <summary>
    /// Метод проставляет статусы вакансиям.
    /// </summary>
    /// <param name="projectVacancies">Список вакансий.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Список вакансий.</returns>
    private async Task<IEnumerable<VacancyOutput>> FillVacanciesStatusesAsync(
        List<VacancyOutput> vacancies, long userId)
    {
        //Получаем вакансии пользователей 
        var userVacancies = (await _vacancyModerationService.AllVacanciesModerationAsync()).Vacancies.ToList();

        // Проставляем статусы вакансий.
        foreach (var pv in vacancies)
        {
          foreach(var uv in userVacancies)
            {
                if (pv.VacancyId == uv.VacancyId)
                {
					pv.VacancyStatusName = uv.ModerationStatusName;
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
            var userId = v.UserId;
            
            // Получаем подписку пользователя.
            var userSubscription = await _subscriptionRepository.GetUserSubscriptionAsync(userId);
            
            if (userSubscription is null)
            {
                var ex = new InvalidOperationException("Найдена невалидная подписка пользователя. " +
                                                    $"UserId: {userId}. " +
                                                    "Подписка была NULL или невалидная." +
                                                    $"#2 Ошибка в {nameof(VacancyService)}");
                
                // Отправляем ивент в пачку.
                await _discordService.SendNotificationErrorAsync(ex);
                
                // Если ошибка, то не стопаем выполнение логики, а вернем вакансии, пока будем разбираться с ошибкой.
                // Без тегов не страшно отобразить вакансии.
                return vacancies;
            }

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
    
    /// <summary>
    /// Метод удаляет из результата вакансии, которые не попадут в каталог из-за замечаний.
    /// </summary>
    /// <param name="vacancies">Список вакансий.</param>
    private async Task DeleteIfVacancyRemarksAsync(List<CatalogVacancyOutput> vacancies)
    {
        var removedVacancies = new List<CatalogVacancyOutput>();
            
        // Исключаем вакансии, которые имеют неисправленные замечания.
        foreach (var vac in vacancies)
        {
            var isRemarks = await _vacancyModerationRepository.GetVacancyRemarksAsync(vac.VacancyId);
                
            if (!isRemarks.Any())
            {
                continue;
            }
                
            removedVacancies.Add(vac);
        }

        if (removedVacancies.Any())
        {
            removedVacancies.RemoveAll(v => removedVacancies.Select(x => x.VacancyId).Contains(v.VacancyId));
        }
    }
    
    /// <summary>
    /// Метод обновляет статус замечаниям на статус "На проверке", если есть неисправленные.
    /// </summary>
    /// <param name="projectId">Id вакансии.</param>
    private async Task CheckAwaitingCorrectionRemarksAsync(long vacancyId)
    {
        var remarks = await _vacancyModerationRepository.GetVacancyRemarksAsync(vacancyId);

        if (!remarks.Any())
        {
            return;
        }

        var awaitingRemarks = new List<VacancyRemarkEntity>();
        
        foreach (var r in remarks)
        {
            if (r.RemarkStatusId != (int)RemarkStatusEnum.AwaitingCorrection)
            {
                continue;
            }

            r.RemarkStatusId = (int)RemarkStatusEnum.Review;
            awaitingRemarks.Add(r);
        }

        if (awaitingRemarks.Any())
        {
            await _vacancyModerationRepository.UpdateVacancyRemarksAsync(awaitingRemarks);
        }
    }
    
    #endregion
}