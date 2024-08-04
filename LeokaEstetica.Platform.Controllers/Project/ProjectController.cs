using AutoMapper;
using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Base.Abstractions.Services.Validation;
using LeokaEstetica.Platform.Base.Filters;
using LeokaEstetica.Platform.Controllers.ModelsValidation.Project;
using LeokaEstetica.Platform.Controllers.Validators.Project;
using LeokaEstetica.Platform.Core.Enums;
using LeokaEstetica.Platform.Finder.Abstractions.Project;
using LeokaEstetica.Platform.Integrations.Abstractions.Discord;
using LeokaEstetica.Platform.Messaging.Abstractions.Project;
using LeokaEstetica.Platform.Messaging.Builders;
using LeokaEstetica.Platform.Models.Dto.Input.Project;
using LeokaEstetica.Platform.Models.Dto.Input.ProjectTeam;
using LeokaEstetica.Platform.Models.Dto.Output.Configs;
using LeokaEstetica.Platform.Models.Dto.Output.Project;
using LeokaEstetica.Platform.Models.Dto.Output.ProjectTeam;
using LeokaEstetica.Platform.Models.Dto.Output.Vacancy;
using LeokaEstetica.Platform.Services.Abstractions.Project;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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
    private readonly IProjectCommentsService _projectCommentsService;
    private readonly IProjectFinderService _projectFinderService;
    private readonly IProjectPaginationService _projectPaginationService;
    private readonly ILogger<ProjectController> _logger;
    private readonly Lazy<IDiscordService> _discordService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="projectService">Сервис проектов.</param>
    /// <param name="mapper">Автомаппер.</param>
    /// <param name="validationExcludeErrorsService">Сервис исключения валидации ошибок.</param>
    /// <param name="projectCommentsService">Сервис комментариев проектов.</param>
    /// <param name="projectFinderService">Поисковый сервис проектов.</param>
    /// <param name="projectPaginationService">Сервис пагинации проектов.</param>
    /// <param name="logger">Сервис логера.</param>
    /// <param name="discordService">Сервис уведомлений дискорда.</param>
    public ProjectController(IProjectService projectService,
        IMapper mapper,
        IValidationExcludeErrorsService validationExcludeErrorsService,
        IProjectCommentsService projectCommentsService, 
        IProjectFinderService projectFinderService, 
        IProjectPaginationService projectPaginationService, 
        ILogger<ProjectController> logger,
        Lazy<IDiscordService> discordService)
    {
        _projectService = projectService;
        _mapper = mapper;
        _validationExcludeErrorsService = validationExcludeErrorsService;
        _projectCommentsService = projectCommentsService;
        _projectFinderService = projectFinderService;
        _projectPaginationService = projectPaginationService;
        _logger = logger;
        _discordService = discordService;
    }

    /// <summary>
    /// TODO: Подумать, давать ли всем пользователям возможность просматривать каталог проектов или только тем, у кого есть подписка.
    /// Метод получает список проектов для каталога.
    /// </summary>
    /// <returns>Список проектов.</returns>
    [HttpGet]
    [Route("catalog")]
    [ProducesResponseType(200, Type = typeof(CatalogProjectResultOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<CatalogProjectResultOutput> CatalogProjectsAsync()
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
        
        createProjectInput.Account = GetUserName();
        createProjectInput.Token = CreateTokenFromHeader();

        var project = await _projectService.CreateProjectAsync(createProjectInput);
        
        result = _mapper.Map<CreateProjectOutput>(project);

        return result;
    }

    /// <summary>
    /// Метод получает список проектов пользователя.
    /// </summary>
    /// <param name="isCreateVacancy">Признак создания вакансии.</param>
    /// <returns>Список проектов.</returns>
    [HttpGet]
    [Route("user-projects")]
    [ProducesResponseType(200, Type = typeof(UserProjectResultOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<UserProjectResultOutput> UserProjectsAsync([FromQuery] bool isCreateVacancy)
    {
        var result = await _projectService.UserProjectsAsync(GetUserName(), isCreateVacancy);

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
    /// <param name="updateProjectInput">Входная модель.</param>
    /// <returns>Обновленные данные.</returns>
    [HttpPut]
    [Route("project")]
    [ProducesResponseType(200, Type = typeof(UpdateProjectOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<UpdateProjectOutput> UpdateProjectAsync([FromBody] UpdateProjectInput updateProjectInput)
    {
        var result = new UpdateProjectOutput();
        var validator = await new UpdateProjectValidator().ValidateAsync(updateProjectInput);

        if (validator.Errors.Any())
        {
            result.Errors = await _validationExcludeErrorsService.ExcludeAsync(validator.Errors);

            return result;
        }

        updateProjectInput.Account = GetUserName();
        updateProjectInput.Token = CreateTokenFromHeader();

        result = await _projectService.UpdateProjectAsync(updateProjectInput);

        return result;
    }

    /// <summary>
    /// Метод получает проект для изменения или просмотра.
    /// </summary>
    /// <param name="projectValidation">Входная модель.</param>
    /// <returns>Данные проекта.</returns>
    [HttpGet]
    [Route("project")]
    [ProducesResponseType(200, Type = typeof(ProjectOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<ProjectOutput> GetProjectAsync([FromQuery] ProjectValidationModel projectValidation)
    {
        var result = new ProjectOutput();
        var validator = await new ProjectValidator().ValidateAsync(projectValidation);

        if (validator.Errors.Any())
        {
            result.Errors = await _validationExcludeErrorsService.ExcludeAsync(validator.Errors);

            return result;
        }

        result = await _projectService.GetProjectAsync(projectValidation.ProjectId, projectValidation.Mode,
            GetUserName());

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
        var result = await _projectService.ProjectVacanciesAsync(projectId, GetUserName(), CreateTokenFromHeader());

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

        createProjectVacancyInput.Account = GetUserName();
        createProjectVacancyInput.Token = CreateTokenFromHeader();

        var createdVacancy = await _projectService.CreateProjectVacancyAsync(createProjectVacancyInput);
        
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
            attachProjectVacancyInput.VacancyId, GetUserName(), CreateTokenFromHeader());
    }

    /// <summary>
    /// Метод получает список вакансий проекта, которые могут быть прикреплены к проекту пользователя.
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
        var items = await _projectService.ProjectVacanciesAvailableAttachAsync(projectId, GetUserName(), false);
        result.ProjectVacancies = _mapper.Map<IEnumerable<ProjectVacancyOutput>>(items);

        return result;
    }

    /// <summary>
    /// Метод записывает отклик на проект.
    /// Отклик может быть с указанием вакансии, на которую идет отклик (если указана VacancyId).
    /// Отклик может быть без указаниея вакансии, на которую идет отклик (если не указана VacancyId).
    /// </summary>
    /// <param name="projectResponseInput">Входная модель.</param>
    /// <returns>Выходная модель с записанным откликом.</returns>
    [HttpPost]
    [Route("response")]
    [ProducesResponseType(200, Type = typeof(ProjectResponseOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<ProjectResponseOutput> WriteProjectResponseAsync(
        [FromBody] ProjectResponseInput projectResponseInput)
    {
        var projectResponse = await _projectService.WriteProjectResponseAsync(projectResponseInput.ProjectId,
            projectResponseInput.VacancyId, GetUserName(), CreateTokenFromHeader());
        
        var result = _mapper.Map<ProjectResponseOutput>(projectResponse);

        return result;
    }

    /// <summary>
    /// Метод создает комментарий к проекту.
    /// </summary>
    /// <param name="projectCommentInput">Входная модель.</param>
    [HttpPost]
    [Route("project/comment")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task CreateProjectCommentAsync([FromBody] ProjectCommentInput projectCommentInput)
    {
        await _projectCommentsService.CreateProjectCommentAsync(projectCommentInput.ProjectId,
            projectCommentInput.Comment, GetUserName(), CreateTokenFromHeader());
    }

    /// <summary>
    /// Метод получает список комментариев проекта.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Список комментариев проекта.</returns>
    [HttpGet]
    [Route("comments/{projectId}")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<ProjectCommentOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<ProjectCommentOutput>> GetProjectCommentsAsync([FromRoute] long projectId)
    {
        var prjComments = await _projectCommentsService.GetProjectCommentsAsync(projectId);
        var items = CreateProjectCommentsDatesBuilder.Create(prjComments, _mapper);
        var result = _mapper.Map<IEnumerable<ProjectCommentOutput>>(items);

        return result;
    }

    /// <summary>
    /// Метод получает команду проекта.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Данные команды проекта.</returns>
    [HttpGet]
    [Route("{projectId}/team")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<ProjectTeamOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<ProjectTeamOutput>> GetProjectTeamAsync([FromRoute] long projectId)
    {
        var result = await _projectService.GetProjectTeamAsync(projectId);

        return result;
    }

    /// <summary>
    /// Метод получает названия полей для таблицы команды проекта пользователя.
    /// </summary>
    /// <returns>Список названий полей таблицы.</returns>
    [HttpGet]
    [Route("config-project-team")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<ProjectTeamColumnNameOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<ProjectTeamColumnNameOutput>> ProjectTeamColumnsNamesAsync()
    {
        var items = await _projectService.ProjectTeamColumnsNamesAsync();
        var result = _mapper.Map<IEnumerable<ProjectTeamColumnNameOutput>>(items);

        return result;
    }

    /// <summary>
    /// Метод добавляет в команду проекта пользователя выбранным способом.
    /// </summary>
    /// <param name="inviteProjectMemberInput">Входная модель.</param>
    /// <returns>Добавленный пользователь.</returns>s
    [HttpPost]
    [Route("invite-project-team")]
    [ProducesResponseType(200, Type = typeof(ProjectTeamMemberOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<ProjectTeamMemberOutput> InviteProjectTeamAsync(
        [FromBody] InviteProjectMemberInput inviteProjectMemberInput)
    {
        var result = await _projectService.InviteProjectTeamAsync(inviteProjectMemberInput.InviteText,
            Enum.Parse<ProjectInviteTypeEnum>(inviteProjectMemberInput.InviteType), inviteProjectMemberInput.ProjectId,
            inviteProjectMemberInput.VacancyId, GetUserName(), CreateTokenFromHeader());

        return new ProjectTeamMemberOutput
        {
            SuccessMessage = "Пользователь успешно приглашен в команду проекта.",
            IsAccess = result.IsAccess,
            ForbiddenTitle = result.ForbiddenTitle,
            ForbiddenText = result.ForbiddenText,
            FareRuleText = result.FareRuleText
        };
    }

    /// <summary>
    /// TODO: Это должно находится в контроллере поиска. Перенести.
    /// Метод фильтрации проектов в зависимости от параметров фильтров.
    /// </summary>
    /// <param name="filterProjectInput">Входная модель.</param>
    /// <returns>Список проектов после фильтрации.</returns>
    [HttpGet]
    [Route("filter")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<CatalogProjectOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<CatalogProjectOutput>> FilterProjectsAsync(
        [FromQuery] FilterProjectInput filterProjectInput)
    {
        var result = await _projectService.FilterProjectsAsync(filterProjectInput);

        return result;
    }

    /// <summary>
    /// TODO: Это должно находится в контроллере поиска. Перенести.
    /// Метод находит проекты по поисковому запросу.
    /// </summary>
    /// <param name="searchText">Строка поиска.</param>
    /// <returns>Список проектов соответствующие поисковому запросу.</returns>
    [HttpGet]
    [Route("search")]
    [ProducesResponseType(200, Type = typeof(CatalogProjectResultOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<CatalogProjectResultOutput> SearchProjectsAsync([FromQuery] string searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return null;
        }
        
        var result = await _projectFinderService.SearchProjectsAsync(searchText);

        return result;
    }
    
    /// <summary>
    /// Метод пагинации проектов.
    /// </summary>
    /// <param name="page">Номер страницы.</param>
    /// <returns>Список проектов.</returns>
    [HttpGet]
    [Route("pagination/{page}")]
    [ProducesResponseType(200, Type = typeof(PaginationProjectOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<PaginationProjectOutput> GetProjectsPaginationAsync([FromRoute] int page)
    {
        var result = await _projectPaginationService.GetProjectsPaginationAsync(page);

        return result;
    }

    /// <summary>
    /// Метод удаляет вакансию проекта.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <param name="projectId">Id проекта.</param>
    [HttpDelete]
    [Route("projects/{projectId}/vacancies/{vacancyId}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task DeleteProjectVacancyAsync([FromRoute] long vacancyId, long projectId)
    {
        if (vacancyId <= 0)
        {
            var ex = new ArgumentNullException($"Id вакансии не может быть пустым. VacancyId: {vacancyId}");
            _logger.LogError(ex, ex.Message);
        }
        
        if (projectId <= 0)
        {
            var ex = new ArgumentNullException($"Id проекта не может быть пустым. ProjectId: {projectId}");
            _logger.LogError(ex, ex.Message);
        }

        await _projectService.DeleteProjectVacancyAsync(vacancyId, projectId, GetUserName(), CreateTokenFromHeader());
    }

    /// <summary>
    /// Метод удаляет проект и все, что с ним связано.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    [HttpDelete]
    [Route("{projectId}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task DeleteProjectAsync([FromRoute] long projectId)
    {
        await _projectService.DeleteProjectAsync(projectId, GetUserName(), CreateTokenFromHeader());
    }

    /// <summary>
    /// Метод получает список вакансий доступных к отклику.
    /// Для владельца проекта будет возвращаться пустой список.
    /// </summary>
    /// <returns>Список вакансий доступных к отклику.</returns>
    [HttpGet]
    [Route("available-response-vacancies")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<ProjectVacancyOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<ProjectVacancyOutput>> GetAvailableResponseProjectVacanciesAsync(
        [FromQuery] long projectId)
    {
        var items = await _projectService.GetAvailableResponseProjectVacanciesAsync(projectId, GetUserName());

        if (items is null)
        {
            return Enumerable.Empty<ProjectVacancyOutput>();
        }
        
        var result = _mapper.Map<IEnumerable<ProjectVacancyOutput>>(items);

        return result;
    }

    /// <summary>
    /// Метод получает список проектов пользователя из архива.
    /// </summary>
    /// <returns>Список архивированных проектов.</returns>
    [HttpGet]
    [Route("archive")]
    [ProducesResponseType(200, Type = typeof(UserProjectArchiveResultOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<UserProjectArchiveResultOutput> GetUserProjectsArchiveAsync()
    {
        var result = await _projectService.GetUserProjectsArchiveAsync(GetUserName());

        return result;
    }

    /// <summary>
    /// Метод удаляет участника проекта из команды.
    /// </summary>
    /// <param name="projectId">Id проекта</param>
    /// <param name="userId">Id пользователя, которого будем удалять из команды</param>
    [HttpDelete]
    [Route("{projectId}/team-member/{userId}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task DeleteProjectTeamMemberAsync([FromRoute] long projectId, [FromRoute] long userId)
    {
        await _projectService.DeleteProjectTeamMemberAsync(projectId, userId, CreateTokenFromHeader());
    }

    /// <summary>
    /// Метод покидания команды проекта.
    /// </summary>
    /// <param name="projectId">Id проекта</param>
    [HttpDelete]
    [Route("team-leave/{projectId}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task LeaveProjectTeamAsync([FromRoute] long projectId)
    {
        await _projectService.LeaveProjectTeamAsync(projectId, GetUserName(), CreateTokenFromHeader());
    }

    /// <summary>
    /// Метод получает список замечаний проекта, если они есть.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Список замечаний проекта.</returns>
    [HttpGet]
    [Route("{projectId}/remarks")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<GetProjectRemarkOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<GetProjectRemarkOutput>> GetProjectRemarksAsync([FromRoute] long projectId)
    {
        var items = await _projectService.GetProjectRemarksAsync(projectId, GetUserName());
        var result = _mapper.Map<IEnumerable<GetProjectRemarkOutput>>(items);

        return result;
    }

    /// <summary>
    /// Метод добавляет проект в архив.
    /// </summary>
    /// <param name="projectArchiveInput">Входная модель.</param>
    [HttpPost]
    [Route("archive")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task AddProjectArchiveAsync([FromBody] ProjectArchiveInput projectArchiveInput)
    {
        await _projectService.AddProjectArchiveAsync(projectArchiveInput.ProjectId, GetUserName(),
            GetTokenFromHeader());
    }

    /// <summary>
    /// Метод удаляет из архива проект.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    [HttpDelete]
    [Route("archive")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task DeleteProjectArchiveAsync([FromQuery] long projectId)
    {
        await _projectService.DeleteProjectArchiveAsync(projectId, GetUserName(), GetTokenFromHeader());
    }
    
    /// <summary>
    /// Метод получает список вакансий проекта, по которым можно пригласить пользователя в проект.
    /// </summary>
    /// <param name="projectId">Id проекта, для которого получить список вакансий.</param>
    /// <returns>Список вакансий проекта.</returns>
    [HttpGet]
    [Route("available-invite-vacancies")]
    [ProducesResponseType(200, Type = typeof(ProjectVacancyResultOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<ProjectVacancyResultOutput> ProjectVacanciesAvailableInviteAsync([FromQuery] long projectId)
    {
        var result = new ProjectVacancyResultOutput();
        var items = await _projectService.ProjectVacanciesAvailableAttachAsync(projectId, GetUserName(), true);
        result.ProjectVacancies = _mapper.Map<IEnumerable<ProjectVacancyOutput>>(items);

        return result;
    }

    /// <summary>
    /// Метод назначает участнику команды проекта роль.
    /// <param name="teamMemberRoleInput">Входная модель.</param>
    /// </summary>
    [HttpPatch]
    [Route("team-member-role")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task SetProjectTeamMemberRoleAsync([FromQuery] TeamMemberRoleInput teamMemberRoleInput)
    {
        var validator = await new TeamMemberRoleValidator().ValidateAsync(teamMemberRoleInput);

        if (validator.Errors.Any())
        {
            var exceptions = new List<InvalidOperationException>();

            foreach (var err in validator.Errors)
            {
                exceptions.Add(new InvalidOperationException(err.ErrorMessage));
            }
            
            var ex = new AggregateException("Ошибка назначение роли участнику команды проекта. " +
                                            $"ProjectId: {teamMemberRoleInput.ProjectId}. " +
                                            $"UserId: {teamMemberRoleInput.UserId}. " +
                                            $"Role: {teamMemberRoleInput.Role}", exceptions);
            _logger.LogError(ex, ex.Message);
            
            await _discordService.Value.SendNotificationErrorAsync(ex);
            
            // TODO: Тут добавить уведомление через хаб для отображения на фронте.

            return;
        }

        await _projectService.SetProjectTeamMemberRoleAsync(teamMemberRoleInput.UserId, teamMemberRoleInput.Role,
            teamMemberRoleInput.ProjectId);
    }

	/// <summary>
	/// Метод обновляет видимость проекта
	/// </summary>
	/// <param name="projectId">Id проекта.</param>
	[HttpPatch]
	[Route("visible-project")]
	[ProducesResponseType(200)]
	[ProducesResponseType(400)]
	[ProducesResponseType(403)]
	[ProducesResponseType(500)]
	[ProducesResponseType(404)]
	public async Task UpdateVisibleProjectAsync([FromQuery] long projectId, [FromQuery] bool isPublic)
	{
        await _projectService.UpdateVisibleProjectAsync(projectId,isPublic);
	}
}