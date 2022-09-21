using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Models.Dto.Input.Header;
using LeokaEstetica.Platform.Models.Dto.Output.Header;
using Microsoft.AspNetCore.Mvc;

namespace LeokaEstetica.Platform.Controllers.Header;

/// <summary>
/// Контроллер для работы с хидерами.
/// </summary>
[ApiController, Route("header")]
public class HeaderController : BaseController
{
    public HeaderController()
    {
    }

    /// <summary>
    /// Метод получает список элементов для хидера в зависимости от его типа.
    /// </summary>
    /// <param name="headerType">Тип хидера.</param>
    /// <returns>Список элементов хидера.</returns>
    [HttpGet]
    [Route("items")]
    public async Task<IEnumerable<HeaderOutput>> HeaderItemsAsymc(HeaderInput headerInput)
    {
        throw new NotImplementedException();
    }
}