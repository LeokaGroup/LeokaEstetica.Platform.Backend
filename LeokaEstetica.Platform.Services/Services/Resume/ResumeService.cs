using LeokaEstetica.Platform.Core.Exceptions;
using LeokaEstetica.Platform.Database.Abstractions.Resume;
using LeokaEstetica.Platform.Database.Abstractions.User;
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
    private readonly IUserRepository _userRepository;
    
    public ResumeService(ILogService logService, 
        IResumeRepository resumeRepository, 
        IUserRepository userRepository)
    {
        _logService = logService;
        _resumeRepository = resumeRepository;
        _userRepository = userRepository;
    }

    /// <summary>
    /// Метод получает список резюме.
    /// </summary>
    /// <returns>Список резюме.</returns>
    public async Task<IEnumerable<ProfileInfoEntity>> GetProfileInfosAsync()
    {
        try
        {
            var result = await _resumeRepository.GetProfileInfosAsync();
            
            // TODO: Это по хорошему должна решать мродерация и некорректные резюме не будут попадать в каталог.
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
    /// <param name="account">Аккаунт пользователя.</param>
    /// <returns>Данные анкеты.</returns>
    public async Task<ProfileInfoEntity> GetResumeAsync(string account)
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

            var result = await _resumeRepository.GetResumeAsync(userId);

            return result;
        }
        
        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex);
            throw;
        }
    }
}