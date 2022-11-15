using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Core.Filters;
using LeokaEstetica.Platform.Models.Dto.Input.Vacancy;
using LeokaEstetica.Platform.Models.Dto.Output.Vacancy;
using LeokaEstetica.Platform.Services.Abstractions.Vacancy;
using Microsoft.AspNetCore.Mvc;

namespace LeokaEstetica.Platform.Controllers.Vacancy;

/// <summary>
/// Контроллер работы с вакансиями.
/// </summary>
[AuthFilter]
[ApiController]
[Route("vacancies")]
public class VacancyController : BaseController
{
    private readonly IVacancyService _vacancyService;
    
    public VacancyController(IVacancyService vacancyService)
    {
        _vacancyService = vacancyService;
    }

    // [HttpGet]
    // [Route("")]
    // public async Task<VacancyMenuItemsResultOutput> CatalogVacanciesAsync()
    // {
    //     throw new NotImplementedException();
    // }

    /// <summary>
    /// Метод получает список меню вакансий.
    /// </summary>
    /// <returns>Список меню.</returns>
    [HttpGet]
    [Route("menu")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<VacancyMenuItemsResultOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<VacancyMenuItemsResultOutput> VacanciesMenuItemsAsync()
    {
        var result = await _vacancyService.VacanciesMenuItemsAsync();

        return result;
    }

    /// <summary>
    /// Метод создает вакансию.
    /// </summary>
    /// <param name="createVacancyInput">Входная модель.</param>
    /// <returns>Данные созданной вакансии.</returns>
    [HttpPost]
    [Route("vacancy")]
    [ProducesResponseType(200, Type = typeof(CreateVacancyOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<CreateVacancyOutput> CreateVacancyAsync([FromBody] CreateVacancyInput createVacancyInput)
    {
        var result = await _vacancyService.CreateVacancyAsync(createVacancyInput.VacancyName, createVacancyInput.VacancyText, createVacancyInput.WorkExperience, createVacancyInput.Employment, createVacancyInput.Payment, GetUserName());

        return result;
    }
}