using LeokaEstetica.Platform.Database.Abstractions.Resume;
using LeokaEstetica.Platform.Logs.Abstractions;
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

    public ResumeService(ILogService logService, 
        IResumeRepository resumeRepository)
    {
        _logService = logService;
        _resumeRepository = resumeRepository;
    }

    /// <summary>
    /// Метод получает список резюме.
    /// </summary>
    /// <returns>Список резюме.</returns>
    public async Task<List<ProfileInfoEntity>> GetProfileInfosAsync()
    {
        try
        {
            var result = await _resumeRepository.GetProfileInfosAsync();
            
            // TODO: Временный костыль.Это должна решать модерация и некорректные резюме не будут попадать в каталог.
            // TODO: А это потом надо убрать.
            // Убираем анкеты, которые не проходят по условиям.
            CreateProfileInfosBuilder.CreateProfileInfosResult(ref result);

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