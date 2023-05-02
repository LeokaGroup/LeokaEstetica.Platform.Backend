using AutoMapper;
using LeokaEstetica.Platform.CallCenter.Abstractions.Resume;
using LeokaEstetica.Platform.CallCenter.Builders;
using LeokaEstetica.Platform.Core.Enums;
using LeokaEstetica.Platform.Core.Exceptions;
using LeokaEstetica.Platform.Database.Abstractions.Moderation.Resume;
using LeokaEstetica.Platform.Database.Abstractions.User;
using LeokaEstetica.Platform.Logs.Abstractions;
using LeokaEstetica.Platform.Models.Dto.Input.Moderation;
using LeokaEstetica.Platform.Models.Dto.Output.Moderation.Resume;
using LeokaEstetica.Platform.Models.Entities.Moderation;
using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.Notifications.Consts;

namespace LeokaEstetica.Platform.CallCenter.Services.Resume;

/// <summary>
/// Класс реализует методы сервиса модерации анкет пользователей.
/// </summary>
public class ResumeModerationService : IResumeModerationService
{
    private readonly ILogService _logService;
    private readonly IResumeModerationRepository _resumeModerationRepository;
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;
    private readonly IResumeModerationNotificationService _resumeModerationNotificationService;
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="logService">Сервис логера.</param>
    /// <param name="resumeModerationRepository">Репозиторий анкет.</param>
    /// <param name="mapper">Автомаппер.</param>
    /// <param name="userRepository">Репозиторий пользователя..</param>
    public ResumeModerationService(ILogService logService, 
        IResumeModerationRepository resumeModerationRepository, 
        IMapper mapper, 
        IUserRepository userRepository, 
        IResumeModerationNotificationService resumeModerationNotificationService)
    {
        _logService = logService;
        _resumeModerationRepository = resumeModerationRepository;
        _mapper = mapper;
        _userRepository = userRepository;
        _resumeModerationNotificationService = resumeModerationNotificationService;
    }

    #region Публичные методы.

    /// <summary>
    /// Метод получает список анкет для модерации.
    /// </summary>
    /// <returns>Список анкет.</returns>
    public async Task<ResumeModerationResult> ResumesModerationAsync()
    {
        try
        {
            var result = new ResumeModerationResult();
            var items = await _resumeModerationRepository.ResumesModerationAsync();
            var users = await _userRepository.GetAllAsync();
            result.Resumes = CreateResumesModerationDatesBuilder.Create(items, _mapper, users);

            return result;
        }
        
        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex);
            throw;
        }
    }

    /// <summary>
    /// Метод одобряет анкету на модерации.
    /// </summary>
    /// <param name="profileInfoId">Id анкеты.</param>
    public async Task ApproveResumeAsync(long profileInfoId)
    {
        try
        {
            await _resumeModerationRepository.ApproveResumeAsync(profileInfoId);
        }
        
        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex);
            throw;
        }
    }

    /// <summary>
    /// Метод отклоняет анкету на модерации.
    /// </summary>
    /// <param name="profileInfoId">Id анкеты.</param>
    public async Task RejectResumeAsync(long profileInfoId)
    {
        try
        {
            await _resumeModerationRepository.ApproveResumeAsync(profileInfoId);
        }
        
        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex);
            throw;
        }
    }

    /// <summary>
    /// Метод создает замечания анкет. 
    /// </summary>
    /// <param name="createResumeRemarkInput">Входная модель.</param>
    /// <param name="account">Аккаунт.</param>
    /// <param name="token">Токен.</param>
    /// <returns>Список замечаний анкет.</returns>
    public async Task<IEnumerable<ResumeRemarkEntity>> CreateResumeRemarksAsync(
        CreateResumeRemarkInput createResumeRemarkInput, string account, string token)
    {
        try
        {
            var userId = await _userRepository.GetUserIdByEmailOrLoginAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }
            
            var resumeRemarks = createResumeRemarkInput.ResumesRemarks.ToList();
            
            // Проверяем входные параметры.
            await ValidateResumeRemarksParamsAsync(resumeRemarks);
            
            // Оставляем лишь те замечания, которые не были добавлены к анкете.
            // Проверяем по названию замечания и по статусу.
            var profileInfoId = resumeRemarks.FirstOrDefault()?.ProfileInfoId;

            if (profileInfoId is null or <= 0)
            {
                var ex = new InvalidOperationException($"Id анкеты не был передан. ProfileInfoId: {profileInfoId}");
                throw ex;
            }

            var now = DateTime.Now;
            var addResumeRemarks = new List<ResumeRemarkEntity>();
            var updateResumeRemarks = new List<ResumeRemarkEntity>();
            
            var mapResumeRemarks = _mapper.Map<List<ResumeRemarkEntity>>(resumeRemarks);
            
            // Получаем названия полей.
            var fields = resumeRemarks.Select(pr => pr.FieldName);
            
            // Получаем замечания, которые модератор уже сохранял в рамках текущей анкеты.
            var existsResumeRemarks = await _resumeModerationRepository
                .GetExistsResumeRemarksAsync((long)profileInfoId, fields);
            
            // Задаем модератора замечаниям и задаем статус замечаниям.
            foreach (var pr in mapResumeRemarks)
            {
                pr.ModerationUserId = userId;
                pr.RemarkStatusId = (int)RemarkStatusEnum.NotAssigned;
                pr.DateCreated = now;
                
                // Если есть замечания анкеты сохраненные ранее.
                if (existsResumeRemarks.Any())
                {
                    var getProjectRemarks = existsResumeRemarks.Find(x => x.FieldName.Equals(pr.FieldName));
                    
                    // К обновлению.
                    if (getProjectRemarks is not null)
                    {
                        pr.RemarkId = getProjectRemarks.RemarkId;
                        updateResumeRemarks.Add(pr);   
                    }
                    
                    // К добавлению.
                    else
                    {
                        addResumeRemarks.Add(pr);
                    }
                }

                else
                {
                    addResumeRemarks.Add(pr);
                }
            }

            // Добавляем новые замечания анкеты.
            if (addResumeRemarks.Any())
            {
                await _resumeModerationRepository.CreateResumeRemarksAsync(addResumeRemarks);   
            }
            
            // Изменяем замечания анкеты, которые ранее были сохранены.
            if (updateResumeRemarks.Any())
            {
                await _resumeModerationRepository.UpdateResumeRemarksAsync(updateResumeRemarks);   
            }

            var result = addResumeRemarks.Union(updateResumeRemarks);

            if (!string.IsNullOrEmpty(token))
            {
                // Отправляем уведомление о сохранении замечаний анкеты.
                await _resumeModerationNotificationService.SendNotificationSuccessCreateResumeRemarksAsync(
                    "Все хорошо", "Замечания успешно внесены. Теперь вы можете их отправить.",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS, token);
            }

            return result;
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
    /// Метод валидирует входные параметры замечаний анкеты.
    /// </summary>
    /// <param name="resumeRemarks">Список замечаний анкеты.</param>
    private Task ValidateResumeRemarksParamsAsync(IReadOnlyCollection<ResumeRemarkInput> resumeRemarks)
    {
        if (!resumeRemarks.Any())
        {
            var ex = new InvalidOperationException("Не передано замечаний анкеты.");
            throw ex;
        }
        
        if (resumeRemarks.Any(p => p.ProfileInfoId <= 0))
        {
            var ex = new InvalidOperationException("Среди замечаний анкеты ProfileInfoId анкеты был <= 0.");
            throw ex;
        }

        if (resumeRemarks.Any(p => string.IsNullOrEmpty(p.FieldName)))
        {
            var ex = new InvalidOperationException("Среди замечаний анкеты было пустое название поля замечания.");
            throw ex;
        }

        if (resumeRemarks.Any(p => string.IsNullOrEmpty(p.RemarkText)))
        {
            var ex = new InvalidOperationException("Среди замечаний анкеты был пустой текст замечания.");
            throw ex;
        }
        
        if (resumeRemarks.Any(p => string.IsNullOrEmpty(p.RemarkText)))
        {
            var ex = new InvalidOperationException("Среди замечаний анкеты был пустой текст замечания.");
            throw ex;
        }
        
        if (resumeRemarks.Any(p => string.IsNullOrEmpty(p.RussianName)))
        {
            var ex = new InvalidOperationException("Среди замечаний анкеты было пустое русское название поля.");
            throw ex;
        }

        return Task.CompletedTask;
    }

    #endregion
}