using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Models.Dto.Output.Header;
using LeokaEstetica.Platform.Models.Enums;
using LeokaEstetica.Platform.Services.Abstractions.Header;
using Microsoft.AspNetCore.Mvc;

namespace LeokaEstetica.Platform.Controllers.Header;

/// <summary>
/// Контроллер для работы с хидерами.
/// </summary>
[ApiController]
[Route("header")]
public class HeaderController : BaseController
{
    private readonly IHeaderService _headerService;
    
    public HeaderController(IHeaderService headerService)
    {
        _headerService = headerService;
    }

    /// <summary>
    /// Метод получает список элементов хидера в зависимости от его типа.
    /// </summary>
    /// <param name="headerType">Тип хидера.</param>
    /// <returns>Список элементов хидера.</returns>
    [HttpGet]
    [Route("items")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<HeaderOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<HeaderOutput>> HeaderItemsAsync(HeaderTypeEnum headerType)
    {
        var result = await _headerService.HeaderItemsAsync(headerType);

        return result;
    }
}