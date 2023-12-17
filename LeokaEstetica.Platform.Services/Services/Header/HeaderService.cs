using System.Runtime.CompilerServices;
using AutoMapper;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Core.Exceptions;
using LeokaEstetica.Platform.Database.Abstractions.Config;
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
    private readonly IGlobalConfigRepository _globalConfigRepository;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="headerRepository">Репозиторий хидера.</param>
    /// <param name="mapper">Маппер.</param>
    /// <param name="globalConfigRepository">Репозиторий глобал конфига.</param>
    public HeaderService(IHeaderRepository headerRepository,
        IMapper mapper,
        IGlobalConfigRepository globalConfigRepository)
    {
        _headerRepository = headerRepository;
        _mapper = mapper;
        _globalConfigRepository = globalConfigRepository;
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

        var items = (await _headerRepository.HeaderItemsAsync(headerType)).ToList();
        
        var isAvailableProjectManagment = await _globalConfigRepository.GetValueByKeyAsync<bool>(
            GlobalConfigKeys.ProjectManagment.PROJECT_MANAGMENT_MODE_ENABLED);

        // Исключаем пункт меню модуля управления проектами, если в БД не включен этот модуль.
        if (!isAvailableProjectManagment)
        {
            var removedItem = items.Find(x => x.MenuItemTitle.Equals("Управление проектами"));

            if (removedItem is not null)
            {
                items.Remove(removedItem);
            }
        }

        var result = _mapper.Map<IEnumerable<HeaderOutput>>(items);

        return result;
    }
}