using AutoMapper;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.User;
using LeokaEstetica.Platform.Base.Enums;
using LeokaEstetica.Platform.CallCenter.Abstractions.Messaging.Mail;
using LeokaEstetica.Platform.CallCenter.Abstractions.Vacancy;
using LeokaEstetica.Platform.CallCenter.Builders;
using LeokaEstetica.Platform.CallCenter.Consts;
using LeokaEstetica.Platform.CallCenter.Models.Dto.Output.Vacancy;
using LeokaEstetica.Platform.Core.Enums;
using LeokaEstetica.Platform.Core.Exceptions;
using LeokaEstetica.Platform.Database.Abstractions.Moderation.Vacancy;
using LeokaEstetica.Platform.Database.Abstractions.Project;
using LeokaEstetica.Platform.Database.Abstractions.Vacancy;
using LeokaEstetica.Platform.Integrations.Abstractions.Discord;
using LeokaEstetica.Platform.Integrations.Abstractions.Telegram;
using LeokaEstetica.Platform.Models.Dto.Input.Moderation;
using LeokaEstetica.Platform.Models.Dto.Output.Moderation.Vacancy;
using LeokaEstetica.Platform.Models.Entities.Moderation;
using LeokaEstetica.Platform.Models.Entities.Vacancy;
using LeokaEstetica.Platform.Models.Enums;
using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.Notifications.Consts;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;

namespace LeokaEstetica.Platform.CallCenter.Services.Vacancy;

/// <summary>
/// Класс реализует методы сервиса модерации вакансий.
/// </summary>
public class VacancyModerationService : IVacancyModerationService
{
    private readonly IVacancyModerationRepository _vacancyModerationRepository;
    private readonly ILogger<VacancyModerationService> _logger;
    private readonly IMapper _mapper;
    private readonly IModerationMailingsService _moderationMailingsService;
    private readonly IVacancyRepository _vacancyRepository;
    private readonly IUserRepository _userRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IDiscordService _discordService;
    private readonly ITelegramBotService _telegramBotService;
    private readonly Lazy<IHubNotificationService> _hubNotificationService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="vacancyModerationRepository">Репозиторий модерации вакансий.</param>
    /// <param name="logger">Сервис логов.</param>
    /// <param name="mapper">Автомаппер.</param>
    /// <param name="moderationMailingsService">Сервис модерации уведомлений на почту.</param>
    /// <param name="vacancyRepository">Репозиторий вакансий.</param>
    /// <param name="userRepository">Репозиторий пользователя.</param>
    /// <param name="projectRepository">Репозиторий проектов.</param>
    /// <param name="pachcaService">Сервис пачки.</param>
    /// <param name="telegramBotService">Сервис бота телеграма.</param>
    /// <param name="hubNotificationService">Сервис уведомлений хабов.</param>
    public VacancyModerationService(IVacancyModerationRepository vacancyModerationRepository,
        ILogger<VacancyModerationService> logger, 
        IMapper mapper, 
        IModerationMailingsService moderationMailingsService, 
        IVacancyRepository vacancyRepository, 
        IUserRepository userRepository, 
        IProjectRepository projectRepository,
        IDiscordService discordService,
        ITelegramBotService telegramBotService,
        Lazy<IHubNotificationService> hubNotificationService)
    {
        _vacancyModerationRepository = vacancyModerationRepository;
        _logger = logger;
        _mapper = mapper;
        _moderationMailingsService = moderationMailingsService;
        _vacancyRepository = vacancyRepository;
        _userRepository = userRepository;
        _projectRepository = projectRepository;
        _discordService = discordService;
        _telegramBotService = telegramBotService;
        _hubNotificationService = hubNotificationService;
    }

    #region Публичные методы.

    /// <summary>
    /// Метод отправляет вакансию на модерацию. Это происходит через добавление в таблицу модерации вакансий.
    /// Если вакансия в этой таблице, значит она не прошла еще модерацию. При прохождении модерации она удаляется из нее.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    public async Task AddVacancyModerationAsync(long vacancyId)
    {
        await _vacancyModerationRepository.AddVacancyModerationAsync(vacancyId);
    }

    /// <summary>
    /// Метод получает вакансию для просмотра.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <returns>Данные вакансии.</returns>
    public async Task<UserVacancyEntity> GetVacancyModerationByVacancyIdAsync(long vacancyId)
    {
        try
        {
            var result = await _vacancyModerationRepository.GetVacancyModerationByVacancyIdAsync(vacancyId);

            return result;
        }

        catch (Exception ex)
        {
            _logger.LogError(ex,
                $"Ошибка при получении вакансии для модерации. VacancyId = {vacancyId}");
            throw;
        }
    }

    /// <summary>
    /// Метод получает список вакансий со статусом на модерации.
    /// </summary>
    /// <returns>Список вакансий.</returns>
    public async Task<VacanciesModerationResult> VacanciesModerationAsync()
    {
        try
        {
            var result = new VacanciesModerationResult();
            var items = await _vacancyModerationRepository.VacanciesModerationAsync();
            result.Vacancies = CreateVacanciesModerationDatesBuilder.Create(items, _mapper);

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
	/// <summary>
	/// Метод получает вакансию модерации из таблицы ModerationVacancies.
	/// </summary>
	/// /// <param name="vacancyId">Id вакансии.</param>
	public async Task<ModerationVacancyEntity> GetModerationVacancyByVacancyIdAsync(long vacancyId)
	{
        try
        {
            var result = await _vacancyModerationRepository.GetModerationVacancyByVacancyIdAsync(vacancyId);

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
	/// <summary>
	/// Метод получает список вакансий со всеми статусами.
	/// </summary>
	/// <returns>Список вакансий.</returns>
	public async Task<VacanciesModerationResult> AllVacanciesModerationAsync()
    {
        try
        {
            var result = new VacanciesModerationResult();
            var items = await _vacancyModerationRepository.AllVacanciesModerationAsync();
            result.Vacancies = CreateVacanciesModerationDatesBuilder.Create(items, _mapper);

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
    
    /// <summary>
    /// Метод одобряет вакансию на модерации.
    /// </summary>
    /// <param name="projectId">Id вакансии.</param>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Выходная модель модерации.</returns>
    public async Task<ApproveVacancyOutput> ApproveVacancyAsync(long vacancyId, string account)
    {
        try
        {
            var result = new ApproveVacancyOutput
            {
                IsSuccess = await _vacancyModerationRepository.ApproveVacancyAsync(vacancyId)
            };

            if (!result.IsSuccess)
            {
                var userId = await _userRepository.GetUserIdByEmailOrLoginAsync(account);

                if (userId <= 0)
                {
                    var exp = new NotFoundUserIdByAccountException(account);
                    throw exp;
                }

                var userCode = await _userRepository.GetUserCodeByUserIdAsync(userId);
                
                await _hubNotificationService.Value.SendNotificationAsync("Внимание",
                    "Проект еще находится на модерации. Нельзя одобрить вакансию, пока проект не пройдет модерацию.",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_WARNING, "SendNotificationWarningVacancyProjectOnModeration",
                    userCode, UserConnectionModuleEnum.Main);
                
                var ex = new InvalidOperationException($"Ошибка при одобрении вакансии. VacancyId: {vacancyId}");
                throw ex;
            }
            
            var vacancyOwnerId = await _vacancyRepository.GetVacancyOwnerIdAsync(vacancyId);
            var user = await _userRepository.GetUserPhoneEmailByUserIdAsync(vacancyOwnerId);
            var vacancy = await _vacancyRepository.GetVacancyByVacancyIdAsync(vacancyId);
            var projectId = await _projectRepository.GetProjectIdByVacancyIdAsync(vacancyId);
            var vacancyName = vacancy.VacancyName;

            // Отправляем уведомление на почту владельца вакансии.
            await _moderationMailingsService.SendNotificationApproveVacancyAsync(user.Email, vacancyName, vacancyId);

            // Отправляем уведомление в приложении об одобрении вакансии модератором.
            await _vacancyModerationRepository.AddNotificationApproveVacancyAsync(vacancyId, vacancyOwnerId,
                vacancyName, projectId);

            await _discordService.SendNotificationCreatedObjectAsync(ObjectTypeEnum.Vacancy, vacancyName);

            var vacancyText = (vacancy.VacancyText);
            await _telegramBotService.SendNotificationCreatedObjectAsync(ObjectTypeEnum.Vacancy, vacancyName,
                vacancyText, vacancyId);

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Ошибка при одобрении вакансии при модерации. VacancyId = {vacancyId}");
            throw;
        }
    }

    /// <summary>
    /// Метод отклоняет вакансию на модерации.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <returns>Выходная модель модерации.</returns>
    public async Task<RejectVacancyOutput> RejectVacancyAsync(long vacancyId)
    {
        try
        {
            // TODO: Надо еще проверять, что внесены замечания вакансии. Нельзя отклонить вакансию, не внеся замечания,
            // TODO: и модератор должен это видеть.
            // TODO: Добавить такую проверку тут.
            var result = new RejectVacancyOutput
            {
                IsSuccess = await _vacancyModerationRepository.RejectVacancyAsync(vacancyId)
            };
            
            var vacancyOwnerId = await _vacancyRepository.GetVacancyOwnerIdAsync(vacancyId);
            var user = await _userRepository.GetUserPhoneEmailByUserIdAsync(vacancyOwnerId);
            var vacancyName = await _vacancyRepository.GetVacancyNameByIdAsync(vacancyId);
            var projectId = await _projectRepository.GetProjectIdByVacancyIdAsync(vacancyId);
            
            // Отправляем уведомление на почту владельца вакансии.
            await _moderationMailingsService.SendNotificationRejectVacancyAsync(user.Email, vacancyName, vacancyId);
            
            // Отправляем уведомление в приложении об отклонении вакансии модератором.
            await _vacancyModerationRepository.AddNotificationRejectVacancyAsync(vacancyId, vacancyOwnerId,
                vacancyName, projectId);

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Ошибка при отклонении вакансии при модерации. VacancyId = {vacancyId}");
            throw;
        }
    }

    /// <summary>
    /// Метод создает замечания вакансии. 
    /// </summary>
    /// <param name="createVacancyRemarkInput">Входная модель.</param>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Список замечаний вакансии.</returns>
    public async Task<IEnumerable<VacancyRemarkEntity>> CreateVacancyRemarksAsync(
        CreateVacancyRemarkInput createVacancyRemarkInput, string account)
    {
        try
        {
            var userId = await _userRepository.GetUserIdByEmailOrLoginAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }
            
            var vacancyRemarks = createVacancyRemarkInput.VacanciesRemarks.ToList();
            
            // Проверяем входные параметры.
            await ValidateVacancyRemarksParamsAsync(vacancyRemarks);
            
            // Оставляем лишь те замечания, которые не были добавлены к вакансии.
            // Проверяем по названию замечания и по статусу.
            var vacancyId = vacancyRemarks.FirstOrDefault()?.VacancyId;

            if (vacancyId is null or <= 0)
            {
                var ex = new InvalidOperationException($"Id вакансии не был передан. VacancyId: {vacancyId}");
                throw ex;
            }

            var now = DateTime.UtcNow;
            var addVacancyRemarks = new List<VacancyRemarkEntity>();
            var updateVacancyRemarks = new List<VacancyRemarkEntity>();
            
            var mapVacancyRemarks = _mapper.Map<List<VacancyRemarkEntity>>(vacancyRemarks);
            
            // Получаем названия полей.
            var fields = vacancyRemarks.Select(pr => pr.FieldName);
            
            // Получаем замечания, которые модератор уже сохранял в рамках текущей вакансии.
            var existsProjectRemarks = await _vacancyModerationRepository.GetExistsVacancyRemarksAsync((long)vacancyId,
                fields);
            
            // Задаем модератора замечаниям и задаем статус замечаниям.
            foreach (var pr in mapVacancyRemarks)
            {
                pr.ModerationUserId = userId;
                pr.RemarkStatusId = (int)RemarkStatusEnum.NotAssigned;
                pr.DateCreated = now;
                
                // Если есть замечания вакансии сохраненные ранее.
                if (existsProjectRemarks.Any())
                {
                    var getProjectRemarks = existsProjectRemarks.Find(x => x.FieldName.Equals(pr.FieldName)
                                                                           && x.VacancyId == pr.VacancyId);
                    
                    // К обновлению.
                    if (getProjectRemarks is not null)
                    {
                        pr.RemarkId = getProjectRemarks.RemarkId;
                        updateVacancyRemarks.Add(pr);   
                    }
                    
                    // К добавлению.
                    else
                    {
                        addVacancyRemarks.Add(pr);
                    }
                }

                else
                {
                    addVacancyRemarks.Add(pr);
                }
            }

            // Добавляем новые замечания вакансии.
            if (addVacancyRemarks.Any())
            {
                await _vacancyModerationRepository.CreateVacancyRemarksAsync(addVacancyRemarks);   
            }
            
            // Изменяем замечания вакансии, которые ранее были сохранены.
            if (updateVacancyRemarks.Any())
            {
                await _vacancyModerationRepository.UpdateVacancyRemarksAsync(updateVacancyRemarks);   
            }

            var result = addVacancyRemarks.Union(updateVacancyRemarks);
            
            var userCode = await _userRepository.GetUserCodeByUserIdAsync(userId);

            await _hubNotificationService.Value.SendNotificationAsync("Все хорошо",
                "Замечания успешно внесены. Теперь вы можете их отправить.",
                NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS, "SendNotificationSuccessCreateVacancyRemarks",
                userCode, UserConnectionModuleEnum.Main);

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task SendVacancyRemarksAsync(long vacancyId, string account)
    {
        try
        {
            if (vacancyId <= 0)
            {
                var ex = new InvalidOperationException($"Id вакансии не был передан. VacancyId: {vacancyId}");
                throw ex;
            }
            
            // Проверяем, были ли внесены замечания вакансии.
            var isExists = await _vacancyModerationRepository.CheckVacancyRemarksAsync(vacancyId);
            
            var userId = await _userRepository.GetUserIdByEmailOrLoginAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }
            
            var userCode = await _userRepository.GetUserCodeByUserIdAsync(userId);

            if (!isExists)
            {
                var ex = new InvalidOperationException(RemarkConst.SEND_PROJECT_REMARKS_WARNING +
                                                       $" VacancyId: {vacancyId}");
                _logger.LogWarning(ex, ex.Message);

                await _hubNotificationService.Value.SendNotificationAsync("Внимание",
                    RemarkConst.SEND_PROJECT_REMARKS_WARNING,
                    NotificationLevelConsts.NOTIFICATION_LEVEL_WARNING, "SendNotificationWarningSendVacancyRemarks",
                    userCode, UserConnectionModuleEnum.Main);

                return;
            }

            await _vacancyModerationRepository.SendVacancyRemarksAsync(vacancyId);
            
            var vacancyRemarks = await _vacancyModerationRepository.GetVacancyRemarksAsync(vacancyId);
            
            if (vacancyRemarks.Any())
            {
                var vacancyRemarksText = vacancyRemarks.Select(v => v.RemarkText);
                
                var vacancyOwnerId = await _vacancyRepository.GetVacancyOwnerIdAsync(vacancyId);
                var vacancyOwner = await _userRepository.GetUserByUserIdAsync(vacancyOwnerId);
                var vacancyName = await _vacancyRepository.GetVacancyNameByVacancyIdAsync(vacancyId);
                
                await _moderationMailingsService.SendNotificationVacancyRemarksAsync(vacancyOwner.Email, vacancyId,
                    vacancyName, vacancyRemarksText);
            }

            await _hubNotificationService.Value.SendNotificationAsync("Все хорошо", "Замечания успешно отправлены.",
                NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS, "SendNotificationSuccessSendVacancyRemarks",
                userCode, UserConnectionModuleEnum.Main);
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Метод получает список замечаний вакансии (не отправленные), если они есть.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <returns>Список замечаний вакансии.</returns>
    public async Task<IEnumerable<VacancyRemarkEntity>> GetVacancyUnShippedRemarksAsync(long vacancyId)
    {
        if (vacancyId <= 0)
        {
            var ex = new InvalidOperationException($"Id вакансии не был передан. VacancyId: {vacancyId}");
            throw ex;
        }

        var result = await _vacancyModerationRepository.GetVacancyUnShippedRemarksAsync(vacancyId);

        return result;
    }

    #endregion

    #region Приватные методы.

    /// <summary>
    /// Метод валидирует входные параметры замечаний проекта.
    /// </summary>
    /// <param name="projectRemarks">Список замечаний проекта.</param>
    private Task ValidateVacancyRemarksParamsAsync(IReadOnlyCollection<VacancyRemarkInput> vacancyRemarks)
    {
        if (!vacancyRemarks.Any())
        {
            var ex = new InvalidOperationException("Не передано замечаний вакансии.");
            throw ex;
        }
        
        if (vacancyRemarks.Any(p => p.VacancyId <= 0))
        {
            var ex = new InvalidOperationException("Среди замечаний вакансии Id вакансии был <= 0.");
            throw ex;
        }

        if (vacancyRemarks.Any(p => string.IsNullOrEmpty(p.FieldName)))
        {
            var ex = new InvalidOperationException("Среди замечаний вакансии было пустое название поля замечания.");
            throw ex;
        }

        if (vacancyRemarks.Any(p => string.IsNullOrEmpty(p.RemarkText)))
        {
            var ex = new InvalidOperationException("Среди замечаний вакансии был пустой текст замечания.");
            throw ex;
        }
        
        if (vacancyRemarks.Any(p => string.IsNullOrEmpty(p.RemarkText)))
        {
            var ex = new InvalidOperationException("Среди замечаний вакансии был пустой текст замечания.");
            throw ex;
        }
        
        if (vacancyRemarks.Any(p => string.IsNullOrEmpty(p.RussianName)))
        {
            var ex = new InvalidOperationException("Среди замечаний вакансии было пустое русское название поля.");
            throw ex;
        }

        return Task.CompletedTask;
    }

    #endregion
}