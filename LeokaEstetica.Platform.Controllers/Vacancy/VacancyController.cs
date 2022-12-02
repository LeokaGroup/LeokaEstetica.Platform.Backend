using AutoMapper;
using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Controllers.Validators.Vacancy;
using LeokaEstetica.Platform.Core.Filters;
using LeokaEstetica.Platform.Models.Dto.Input.Vacancy;
using LeokaEstetica.Platform.Models.Dto.Output.Configs;
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
    private readonly IMapper _mapper;
    
    public VacancyController(IVacancyService vacancyService, 
        IMapper mapper)
    {
        _vacancyService = vacancyService;
        _mapper = mapper;
    }

    /// <summary>
    /// Метод получает список вакансий для каталога.
    /// </summary>
    /// <returns>Список вакансий.</returns>
    [HttpGet]
    [Route("")]
    [ProducesResponseType(200, Type = typeof(CatalogVacancyResultOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<CatalogVacancyResultOutput> CatalogVacanciesAsync()
    {
        var result = await _vacancyService.CatalogVacanciesAsync(GetUserName());

        return result;
    }

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
        var result = new CreateVacancyOutput();
        var validator = await new CreateVacancyValidator().ValidateAsync(createVacancyInput);

        if (validator.Errors.Any())
        {
            result.Errors = validator.Errors;

            return result;
        }
        
        var createdVacancy = await _vacancyService.CreateVacancyAsync(createVacancyInput.VacancyName, createVacancyInput.VacancyText, createVacancyInput.WorkExperience, createVacancyInput.Employment, createVacancyInput.Payment, GetUserName());
        result = _mapper.Map<CreateVacancyOutput>(createdVacancy);

        return result;
    }
    
    /// <summary>
    /// Метод получает названия полей для таблицы вакансий проектов пользователя.
    /// Все названия столбцов этой таблицы одинаковые у всех пользователей.
    /// </summary>
    /// <returns>Список названий полей таблицы.</returns>
    [HttpGet]
    [Route("config-user-vacancies")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<ProjectVacancyColumnNameOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<ProjectVacancyColumnNameOutput>> ProjectUserVacanciesColumnsNamesAsync()
    {
        var result = await _vacancyService.ProjectUserVacanciesColumnsNamesAsync();

        return result;
    }
}