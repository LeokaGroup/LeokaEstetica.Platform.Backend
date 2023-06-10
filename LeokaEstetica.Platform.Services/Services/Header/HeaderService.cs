using System.Runtime.CompilerServices;
using AutoMapper;
using LeokaEstetica.Platform.Core.Exceptions;
using LeokaEstetica.Platform.Database.Abstractions.Header;
using LeokaEstetica.Platform.Models.Dto.Output.Header;
using LeokaEstetica.Platform.Models.Enums;
using LeokaEstetica.Platform.Services.Abstractions.Header;

[assembly: InternalsVisibleTo("LeokaEstetica.Platform.Tests")]

namespace LeokaEstetica.Platform.Services.Services.Header;

/// <summary>
/// Класс реализует методы сервиса хидера.
/// </summary>
internal sealed class HeaderService : IHeaderService
{
    private readonly IHeaderRepository _headerRepository;
    private readonly IMapper _mapper;

    public HeaderService(IHeaderRepository headerRepository,
        IMapper mapper)
    {
        _headerRepository = headerRepository;
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
            throw new EmptyHeaderTypeException(headerType);
        }

        var items = await _headerRepository.HeaderItemsAsync(headerType);
        var result = _mapper.Map<IEnumerable<HeaderOutput>>(items);

        return result;
    }
}