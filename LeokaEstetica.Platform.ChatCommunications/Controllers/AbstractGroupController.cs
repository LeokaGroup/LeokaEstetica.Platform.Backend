using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Base.Filters;
using LeokaEstetica.Platform.Models.Dto.Communications.Output;
using Microsoft.AspNetCore.Mvc;

namespace LeokaEstetica.Platform.Communications.Controllers;

/// <summary>
/// Контроллер групп абстрактной области чата.
/// </summary>
[ApiController]
[Route("abstract-group")]
[AuthFilter]
public class AbstractGroupController : BaseController
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public AbstractGroupController()
    {
    }

    /// <summary>
    /// Метод получает список групп выбранной абстрактной области.
    /// </summary>
    /// <returns>Список групп выбранной абстрактной области.</returns>
    [HttpGet]
    [Route("scope-group")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<AbstractGroupOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<AbstractGroupOutput>> GetAbstractGroupAsync()
    {
        
    }
}