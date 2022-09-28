using AutoMapper;
using LeokaEstetica.Platform.Core.Exceptions;
using LeokaEstetica.Platform.Database.Abstractions.Header;
using LeokaEstetica.Platform.Logs.Abstractions;
using LeokaEstetica.Platform.Models.Dto.Output.Header;
using LeokaEstetica.Platform.Models.Enums;
using LeokaEstetica.Platform.Services.Abstractions.Header;

namespace LeokaEstetica.Platform.Services.Services.Header;

/// <summary>
/// Класс реализует методы сервиса хидера.
/// </summary>
public sealed class HeaderService : IHeaderService
{
    private readonly IHeaderRepository _headerRepository;
    private readonly ILogService _logger;
    private readonly IMapper _mapper;
    
    public HeaderService(IHeaderRepository headerRepository, 
        ILogService logger, 
        IMapper mapper)
    {
        _headerRepository = headerRepository;
        _logger = logger;
        _mapper = mapper;
    }

    /// <summary>
    /// Метод получает список элементов для меню хидера в зависимости от его типа.
    /// </summary>
    /// <param name="headerType">Тип хидера. Например, для лендоса.</param>
    /// <returns>Список элементов для меню хидера.</returns>
    public async Task<IEnumerable<HeaderOutput>> HeaderItemsAsync(HeaderTypeEnum headerType)
    {
        // Если тип хидера не соответствует нужному типу.
        if (!headerType.Equals(HeaderTypeEnum.Main))
        {
            await _logger.LogInfoAsync(new EmptyHeaderTypeException(headerType), null, LogLevelEnum.Error);
            throw new EmptyHeaderTypeException(headerType);
        }

        var items = await _headerRepository.HeaderItemsAsync(headerType);

        if (!items.Any())
        {
            var error = "Не найдено элементов для хидера!";
            await _logger.LogInfoAsync(new ArgumentException(error), null, LogLevelEnum.Error);
            throw new ArgumentNullException(error);
        }
            
        var result = _mapper.Map<IEnumerable<HeaderOutput>>(items);

        return result;
    }
}