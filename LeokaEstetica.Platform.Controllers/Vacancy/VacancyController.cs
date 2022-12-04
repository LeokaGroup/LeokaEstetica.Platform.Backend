using AutoMapper;
using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Base.Abstractions.Services;
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
    private readonly IValidationExcludeErrorsService _validationExcludeErrorsService;

    public VacancyController(IVacancyService vacancyService,
        IMapper mapper,
        IValidationExcludeErrorsService validationExcludeErrorsService)
    {
        _vacancyService = vacancyService;
        _mapper = mapper;
        _validationExcludeErrorsService = validationExcludeErrorsService;
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
    [ProducesResponseType(200, Type = typeof(VacancyOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<VacancyOutput> CreateVacancyAsync([FromBody] VacancyInput vacancyInput)
    {
        var result = new VacancyOutput();
        var validator = await new CreateVacancyValidator().ValidateAsync(vacancyInput);

        if (validator.Errors.Any())
        {
            result.Errors = await _validationExcludeErrorsService.ExcludeAsync(validator.Errors);

            return result;
        }

        var createdVacancy = await _vacancyService.CreateVacancyAsync(vacancyInput.VacancyName,
            vacancyInput.VacancyText, vacancyInput.WorkExperience, vacancyInput.Employment, vacancyInput.Payment,
            GetUserName());
        result = _mapper.Map<VacancyOutput>(createdVacancy);

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

    /// <summary>
    /// Метод получает вакансию по ее Id.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <returns>Данные вакансии.</returns>
    [HttpGet]
    [Route("vacancy")]
    [ProducesResponseType(200, Type = typeof(VacancyOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<VacancyOutput> GetVacancyByVacancyIdAsync([FromQuery] long vacancyId)
    {
        var vacancy = await _vacancyService.GetVacancyByVacancyIdAsync(vacancyId, GetUserName());
        var result = _mapper.Map<VacancyOutput>(vacancy);

        return result;
    }

    /// <summary>
    /// Метод обновляет вакансию.
    /// </summary>
    /// <param name="vacancyInput">Входная модель.</param>
    /// <returns>Данные вакансии.</returns>
    [HttpPut]
    [Route("vacancy")]
    [ProducesResponseType(200, Type = typeof(VacancyOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<VacancyOutput> UpdateVacancyAsync([FromBody] VacancyInput vacancyInput)
    {
        var result = new VacancyOutput();
        var validator = await new CreateVacancyValidator().ValidateAsync(vacancyInput);

        if (validator.Errors.Any())
        {
            result.Errors = await _validationExcludeErrorsService.ExcludeAsync(validator.Errors);

            return result;
        }
        
        var createdVacancy = await _vacancyService.UpdateVacancyAsync(vacancyInput.VacancyName,
            vacancyInput.VacancyText, vacancyInput.WorkExperience, vacancyInput.Employment, vacancyInput.Payment,
            GetUserName(), vacancyInput.VacancyId);
        result = _mapper.Map<VacancyOutput>(createdVacancy);

        return result;
    }
}