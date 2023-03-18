using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Models.Dto.Output.Knowledge;
using Microsoft.AspNetCore.Mvc;

namespace LeokaEstetica.Platform.Controllers.Knowledge;

/// <summary>
/// Контроллер работы с БЗ (базой знаний).
/// </summary>
[ApiController]
[Route("knowledge")]
public class KnowledgeController : BaseController
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public KnowledgeController()
    {
    }

    /// <summary>
    /// Метод получает частые вопросы для лендинга.
    /// </summary>
    /// <returns>Список частых вопросов.</returns>
    [HttpGet]
    [Route("landing")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<KnowledgeLandingOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<KnowledgeLandingOutput>> GetLandingKnowledgeAsync()
    {
        
    }
}