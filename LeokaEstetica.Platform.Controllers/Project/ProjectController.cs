using AutoMapper;
using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Base.Abstractions.Services;
using LeokaEstetica.Platform.Controllers.ModelsValidation.Project;
using LeokaEstetica.Platform.Controllers.Validators.Project;
using LeokaEstetica.Platform.Core.Filters;
using LeokaEstetica.Platform.Models.Dto.Input.Project;
using LeokaEstetica.Platform.Models.Dto.Output.Configs;
using LeokaEstetica.Platform.Models.Dto.Output.Project;
using LeokaEstetica.Platform.Models.Enums;
using LeokaEstetica.Platform.Services.Abstractions.Project;
using Microsoft.AspNetCore.Mvc;

namespace LeokaEstetica.Platform.Controllers.Project;

/// <summary>
/// Контроллер работы с проектами.
/// </summary>
[AuthFilter]
[ApiController]
[Route("projects")]
public class ProjectController : BaseController
{
    private readonly IProjectService _projectService;
    private readonly IMapper _mapper;
    private readonly IValidationExcludeErrorsService _validationExcludeErrorsService;

    public ProjectController(IProjectService projectService, 
        IMapper mapper, 
        IValidationExcludeErrorsService validationExcludeErrorsService)
    {
        _projectService = projectService;
        _mapper = mapper;
        _validationExcludeErrorsService = validationExcludeErrorsService;
    }

    /// <summary>
    /// TODO: Подумать, давать ли всем пользователям возможность просматривать каталог проектов или только тем, у кого есть подписка.
    /// Метод получает список проектов для каталога.
    /// </summary>
    /// <returns>Список проектов.</returns>
    [HttpGet]
    [Route("")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<CatalogProjectOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<CatalogProjectOutput>> CatalogProjectsAsync()
    {
        var result = await _projectService.CatalogProjectsAsync();

        return result;
    }

    /// <summary>
    /// TODO: Нужно также учитывать иконку проекта. Пока она не передается в БД.
    /// Метод создает новый проект пользователя.
    /// </summary>
    /// <param name="createProjectInput">Входная модель.</param>
    /// <returns>Данные нового проекта.</returns>
    [HttpPost]
    [Route("project")]
    [ProducesResponseType(200, Type = typeof(CreateProjectOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<CreateProjectOutput> CreateProjectAsync([FromBody] CreateProjectInput createProjectInput)
    {
        var result = new CreateProjectOutput();
        var validator = await new CreateProjectValidator().ValidateAsync(createProjectInput);

        if (validator.Errors.Any())
        {
            result.Errors = await _validationExcludeErrorsService.ExcludeAsync(validator.Errors);

            return result;
        }

        var project = await _projectService.CreateProjectAsync(createProjectInput.ProjectName,
            createProjectInput.ProjectDetails, GetUserName(),
            Enum.Parse<ProjectStageEnum>(createProjectInput.ProjectStage));
        result = _mapper.Map<CreateProjectOutput>(project);

        return result;
    }

    /// <summary>
    /// Метод получает список проектов пользователя.
    /// </summary>
    /// <returns>Список проектов.</returns>
    [HttpGet]
    [Route("user-projects")]
    [ProducesResponseType(200, Type = typeof(UserProjectResultOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<UserProjectResultOutput> UserProjectsAsync()
    {
        var result = await _projectService.UserProjectsAsync(GetUserName());

        return result;
    }

    /// <summary>
    /// Метод получает названия полей для таблицы проектов пользователя.
    /// Все названия столбцов этой таблицы одинаковые у всех пользователей.
    /// </summary>
    /// <returns>Список названий полей таблицы.</returns>
    [HttpGet]
    [Route("config-user-projects")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<ProjectColumnNameOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<ProjectColumnNameOutput>> UserProjectsColumnsNamesAsync()
    {
        var result = await _projectService.UserProjectsColumnsNamesAsync();

        return result;
    }

    /// <summary>
    /// Метод обновляет проект.
    /// </summary>
    /// <param name="createProjectInput">Входная модель.</param>
    /// <returns>Обновленные данные.</returns>
    [HttpPut]
    [Route("project")]
    [ProducesResponseType(200, Type = typeof(UpdateProjectOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<UpdateProjectOutput> UpdateProjectAsync([FromBody] UpdateProjectInput createProjectInput)
    {
        var result = new UpdateProjectOutput();
        var validator = await new UpdateProjectValidator().ValidateAsync(createProjectInput);

        if (validator.Errors.Any())
        {
            result.Errors = await _validationExcludeErrorsService.ExcludeAsync(validator.Errors);

            return result;
        }

        result = await _projectService.UpdateProjectAsync(createProjectInput.ProjectName,
            createProjectInput.ProjectDetails, GetUserName(), createProjectInput.ProjectId,
            Enum.Parse<ProjectStageEnum>(createProjectInput.ProjectStage));

        return result;
    }

    /// <summary>
    /// Метод получает проект для изменения или просмотра.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="mode">Режим. Чтение или изменение.</param>
    /// <returns>Данные проекта.</returns>
    [HttpGet]
    [Route("project")]
    [ProducesResponseType(200, Type = typeof(ProjectOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<ProjectOutput> GetProjectAsync([FromQuery] GetProjectValidationModel model)
    {
        var result = new ProjectOutput();
        var validator = await new GetProjectValidator().ValidateAsync(model);
        
        if (validator.Errors.Any())
        {
            result.Errors = await _validationExcludeErrorsService.ExcludeAsync(validator.Errors);
            
            return result;
        }
        
        var project = await _projectService.GetProjectAsync(model.ProjectId, model.Mode, GetUserName());
        result = _mapper.Map<ProjectOutput>(project);

        return result;
    }

    /// <summary>
    /// Метод получает стадии проекта для выбора.
    /// </summary>
    /// <returns>Стадии проекта.</returns>
    [HttpGet]
    [Route("stages")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<ProjectStageOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<ProjectStageOutput>> ProjectStagesAsync()
    {
        var result = await _projectService.ProjectStagesAsync();

        return result;
    }

    /// <summary>
    /// Метод получает список вакансий проекта. Список вакансий, которые принадлежат владельцу проекта.
    /// </summary>
    /// <param name="projectId">Id проекта, вакансии которого нужно получить.</param>
    /// <returns>Список вакансий.</returns>
    [HttpGet]
    [Route("vacancies")]
    [ProducesResponseType(200, Type = typeof(ProjectVacancyResultOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<ProjectVacancyResultOutput> ProjectVacanciesAsync([FromQuery] long projectId)
    {
        var result = new ProjectVacancyResultOutput();
        var items = await _projectService.ProjectVacanciesAsync(projectId);
        result.ProjectVacancies = _mapper.Map<IEnumerable<ProjectVacancyOutput>>(items);

        return result;
    }

    /// <summary>
    /// Метод создает вакансию проекта. При этом автоматически происходит привязка к проекту.
    /// </summary>
    /// <param name="createProjectVacancyInput">Входная модель.</param>
    /// <returns>Данные созданной вакансии.</returns>
    [HttpPost]
    [Route("vacancy")]
    [ProducesResponseType(200, Type = typeof(CreateProjectVacancyOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<CreateProjectVacancyOutput> CreateProjectVacancyAsync(
        [FromBody] CreateProjectVacancyInput createProjectVacancyInput)
    {
        var result = new CreateProjectVacancyOutput();
        var validator = await new CreateProjectVacancyValidator().ValidateAsync(createProjectVacancyInput);

        if (validator.Errors.Any())
        {
            result.Errors = await _validationExcludeErrorsService.ExcludeAsync(validator.Errors);

            return result;
        }

        var createdVacancy = await _projectService.CreateProjectVacancyAsync(createProjectVacancyInput.VacancyName,
            createProjectVacancyInput.VacancyText, createProjectVacancyInput.ProjectId,
            createProjectVacancyInput.Employment, createProjectVacancyInput.Payment,
            createProjectVacancyInput.WorkExperience, GetUserName());
        result = _mapper.Map<CreateProjectVacancyOutput>(createdVacancy);

        return result;
    }

    /// <summary>
    /// Метод прикрепляет вакансию к проекту.
    /// </summary>
    /// <param name="attachProjectVacancyInput">Входная модель.</param>
    /// <returns>Выходная модель.</returns>
    [HttpPost]
    [Route("attach-vacancy")]
    [ProducesResponseType(200, Type = typeof(AttachProjectVacancyOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task AttachProjectVacancyAsync([FromBody] AttachProjectVacancyInput attachProjectVacancyInput)
    {
        await _projectService.AttachProjectVacancyAsync(attachProjectVacancyInput.ProjectId,
            attachProjectVacancyInput.VacancyId);
    }
    
    /// <summary>
    /// Метод получает список вакансий проекта, которые могут быть прикреплены у проекту пользователя.
    /// </summary>
    /// <param name="projectId">Id проекта, для которого получить список вакансий.</param>
    /// <returns>Список вакансий проекта.</returns>
    [HttpGet]
    [Route("available-attach-vacancies")]
    [ProducesResponseType(200, Type = typeof(ProjectVacancyResultOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<ProjectVacancyResultOutput> ProjectVacanciesAvailableAttachAsync([FromQuery] long projectId)
    {
        var result = new ProjectVacancyResultOutput();
        var items = await _projectService.ProjectVacanciesAvailableAttachAsync(projectId, GetUserName());
        result.ProjectVacancies = _mapper.Map<IEnumerable<ProjectVacancyOutput>>(items);

        return result;
    }
}