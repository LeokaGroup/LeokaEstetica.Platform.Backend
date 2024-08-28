using AutoMapper;
using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Base.Abstractions.Services.Validation;
using LeokaEstetica.Platform.Base.Filters;
using LeokaEstetica.Platform.Controllers.ModelsValidation.Vacancy;
using LeokaEstetica.Platform.Controllers.Validators.Vacancy;
using LeokaEstetica.Platform.Finder.Abstractions.Vacancy;
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
    private readonly IVacancyFinderService _vacancyFinderService;
    private readonly IVacancyPaginationService _vacancyPaginationService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="vacancyService">Сервис вакансий.</param>
    /// <param name="mapper">Автомаппер.</param>
    /// <param name="validationExcludeErrorsService">Сервис исключения параметров валидации.</param>
    /// <param name="vacancyFinderService">Поисковый сервис вакансий.</param>
    /// <param name="vacancyPaginationService">Сервис пагинации вакансий.</param>
    public VacancyController(IVacancyService vacancyService,
        IMapper mapper,
        IValidationExcludeErrorsService validationExcludeErrorsService, 
        IVacancyFinderService vacancyFinderService, 
        IVacancyPaginationService vacancyPaginationService)
    {
        _vacancyService = vacancyService;
        _mapper = mapper;
        _validationExcludeErrorsService = validationExcludeErrorsService;
        _vacancyFinderService = vacancyFinderService;
        _vacancyPaginationService = vacancyPaginationService;
    }

    /// <summary>
    /// Метод получает список вакансий для каталога.
    /// </summary>
    /// <returns>Список вакансий.</returns>
    [HttpGet]
    [Route("catalog")]
    [ProducesResponseType(200, Type = typeof(CatalogVacancyResultOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<CatalogVacancyResultOutput> CatalogVacanciesAsync(
        [FromQuery] VacancyCatalogInput vacancyCatalogInput)
    {
		var result = await _vacancyService.GetCatalogVacanciesAsync(vacancyCatalogInput);

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
    /// <param name="vacancyInput">Входная модель.</param>
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
        
        vacancyInput.Account = GetUserName();
        
        var createdVacancy = await _vacancyService.CreateVacancyAsync(vacancyInput);

        return createdVacancy;
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
    /// <param name="vacancyValidationModel">Входная модель.</param>
    /// <returns>Данные вакансии.</returns>
    [HttpGet]
    [Route("vacancy")]
    [ProducesResponseType(200, Type = typeof(VacancyOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<VacancyOutput> GetVacancyByVacancyIdAsync(
        [FromQuery] VacancyValidationModel vacancyValidationModel)
    {
        var result = new VacancyOutput();
        var validator = await new VacancyValidator().ValidateAsync(vacancyValidationModel);

        if (validator.Errors.Any())
        {
            result.Errors = await _validationExcludeErrorsService.ExcludeAsync(validator.Errors);

            return result;
        }
        
        result = await _vacancyService.GetVacancyByVacancyIdAsync(vacancyValidationModel.VacancyId,
            vacancyValidationModel.Mode, GetUserName());

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

        vacancyInput.Account = GetUserName();

        var createdVacancy = await _vacancyService.UpdateVacancyAsync(vacancyInput);
        
        result = _mapper.Map<VacancyOutput>(createdVacancy);

        return result;
    }
  
    /// Метод находит вакансии по поисковому запросу.
    /// </summary>
    /// <param name="searchText">Строка поиска.</param>
    /// <returns>Список вакансий соответствующие поисковому запросу.</returns>
    [HttpGet]
    [Route("search")]
    [ProducesResponseType(200, Type = typeof(CatalogVacancyResultOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<CatalogVacancyResultOutput> SearchVacanciesAsync([FromQuery] string searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return null;
        }
        
        var result = await _vacancyFinderService.SearchVacanciesAsync(searchText);

        return result;
    }


    /// Метод удаляет вакансию.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    [HttpDelete]
    [Route("{vacancyId}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task DeleteVacancyAsync([FromRoute] long vacancyId)
    {
        await _vacancyService.DeleteVacancyAsync(vacancyId, GetUserName());
    }

    /// <summary>
    /// Метод получает список вакансий пользователя.
    /// </summary>
    /// <returns>Список вакансий.</returns>
    [HttpGet]
    [Route("user-vacancies")]
    [ProducesResponseType(200, Type = typeof(VacancyResultOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<VacancyResultOutput> GetUserVacanciesAsync()
    {
        var result = await _vacancyService.GetUserVacanciesAsync(GetUserName());

        return result;
    }

    /// <summary>
    /// Метод добавляет вакансию в архив.
    /// </summary>
    /// <param name="vacancy">Входная модель.</param>
    [HttpPost]
    [Route("archive")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task AddVacancyArchiveAsync([FromBody] VacancyArchiveInput vacancy)
    {
        await _vacancyService.AddVacancyArchiveAsync(vacancy.VacancyId, GetUserName());
    }
    
    /// <summary>
    /// Метод получает список замечаний вакансии, если они есть.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <returns>Список замечаний вакансии.</returns>
    [HttpGet]
    [Route("{vacancyId}/remarks")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<GetVacancyRemarkOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<GetVacancyRemarkOutput>> GetVacancyRemarksAsync([FromRoute] long vacancyId)
    {
        var items = await _vacancyService.GetVacancyRemarksAsync(vacancyId, GetUserName());
        var result = _mapper.Map<IEnumerable<GetVacancyRemarkOutput>>(items);

        return result;
    }
    
    /// <summary>
    /// Метод удаляет из архива вакансию.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    [HttpDelete]
    [Route("archive")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task DeleteVacancyArchiveAsync([FromQuery] long vacancyId)
    {
        await _vacancyService.DeleteVacancyArchiveAsync(vacancyId, GetUserName());
    }
    
    /// <summary>
    /// Метод получает список вакансий пользователя из архива.
    /// </summary>
    /// <returns>Список архивированных вакансий.</returns>
    [HttpGet]
    [Route("archive")]
    [ProducesResponseType(200, Type = typeof(UserVacancyArchiveResultOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<UserVacancyArchiveResultOutput> GetUserVacanciesArchiveAsync()
    {
        var result = await _vacancyService.GetUserVacanciesArchiveAsync(GetUserName());

        return result;
    }
}