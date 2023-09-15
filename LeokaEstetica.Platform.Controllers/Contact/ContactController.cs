using AutoMapper;
using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Models.Dto.Output.Communication;
using LeokaEstetica.Platform.Services.Abstractions.Press;
using Microsoft.AspNetCore.Mvc;

namespace LeokaEstetica.Platform.Controllers.Contact;

/// <summary>
/// Контроллер работы с прессой, контактами, вакансий и тд. Ознакомительная информация о компании.
/// </summary>
[ApiController]
[Route("press")]
public class ContactController : BaseController
{
    private readonly IPressService _pressService;
    private readonly IMapper _mapper;
    
    /// <summary>
    /// Конструктор.
    /// <param name="pressService">Сервис прессы.</param>
    /// <param name="mapper">Маппер.</param>
    /// </summary>
    public ContactController(IPressService pressService,
        IMapper mapper)
    {
        _pressService = pressService;
        _mapper = mapper;
    }

    /// <summary>
    /// Метод получает список контактов.
    /// </summary>
    /// <returns>Список контактов.</returns>
    [HttpGet]
    [Route("contacts")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<ContactOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<ContactOutput>> GetContactsAsync()
    {
        var contacts = await _pressService.GetContactsAsync();
        var result = _mapper.Map<IEnumerable<ContactOutput>>(contacts);

        return result;
    }
}