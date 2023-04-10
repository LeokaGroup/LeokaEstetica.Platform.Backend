using AutoMapper;
using LeokaEstetica.Platform.CallCenter.Abstractions.Resume;
using LeokaEstetica.Platform.CallCenter.Builders;
using LeokaEstetica.Platform.Database.Abstractions.Moderation.Resume;
using LeokaEstetica.Platform.Database.Abstractions.User;
using LeokaEstetica.Platform.Logs.Abstractions;
using LeokaEstetica.Platform.Models.Dto.Output.Moderation.Resume;

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
        IUserRepository userRepository)
    {
        _logService = logService;
        _resumeModerationRepository = resumeModerationRepository;
        _mapper = mapper;
        _userRepository = userRepository;
    }

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
}