using AutoMapper;
using LeokaEstetica.Platform.Database.Abstractions.Landing;
using LeokaEstetica.Platform.Logs.Abstractions;
using LeokaEstetica.Platform.Models.Dto.Output.Landing;
using LeokaEstetica.Platform.Services.Abstractions.Landing;

namespace LeokaEstetica.Platform.Services.Services.Landing;

/// <summary>
/// Класс реализует методы сервиса лендингов.
/// </summary>
public sealed class LandingService : ILandingService
{
    private readonly ILogService _logger;
    private readonly ILandingRepository _landingRepository;
    private readonly IMapper _mapper;
    
    public LandingService(ILogService logger, 
        ILandingRepository landingRepository, 
        IMapper mapper)
    {
        _logger = logger;
        _landingRepository = landingRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Метод получает данные для блока фона для главного лендоса.
    /// </summary>
    /// <returns>Данные блока.returns>
    public async Task<LandingStartFonOutput> LandingStartFonAsync()
    {
        try
        {
            var fon = await _landingRepository.LandingStartFonAsync();
            var result = _mapper.Map<LandingStartFonOutput>(fon);

            return result;
        }
        
        catch (Exception ex)
        {
            await _logger.LogErrorAsync(ex);
            throw;
        }
    }
}