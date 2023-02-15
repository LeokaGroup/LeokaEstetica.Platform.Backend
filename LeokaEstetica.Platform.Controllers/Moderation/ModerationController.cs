using AutoMapper;
using FluentValidation;
using LeokaEstetica.Platform.Access.Abstractions.Moderation;
using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Controllers.Validators.Access;
using LeokaEstetica.Platform.Models.Dto.Output.Moderation.Project;
using LeokaEstetica.Platform.Models.Dto.Output.Moderation.Vacancy;
using LeokaEstetica.Platform.Models.Dto.Output.Project;
using LeokaEstetica.Platform.Models.Dto.Output.Vacancy;
using LeokaEstetica.Platform.Moderation.Abstractions.Project;
using LeokaEstetica.Platform.Moderation.Abstractions.Vacancy;
using LeokaEstetica.Platform.Moderation.Models.Dto.Input.Access;
using LeokaEstetica.Platform.Moderation.Models.Dto.Input.Project;
using LeokaEstetica.Platform.Moderation.Models.Dto.Input.Role;
using LeokaEstetica.Platform.Moderation.Models.Dto.Input.Vacancy;
using LeokaEstetica.Platform.Moderation.Models.Dto.Output.Access;
using LeokaEstetica.Platform.Moderation.Models.Dto.Output.Project;
using LeokaEstetica.Platform.Moderation.Models.Dto.Output.Role;
using LeokaEstetica.Platform.Moderation.Models.Dto.Output.Vacancy;
using Microsoft.AspNetCore.Mvc;

namespace LeokaEstetica.Platform.Controllers.Moderation;

/// <summary>
/// Контроллер модерации (отвечает за весь функционал модерации).
/// </summary>
[ApiController]
[Route("moderation")]
public class ModerationController : BaseController
{
    private readonly IAccessModerationService _accessModerationService;
    private readonly IProjectModerationService _projectModerationService;
    private readonly IMapper _mapper;
    private readonly IVacancyModerationService _vacancyModerationService;
    private readonly IUserBlackListService _userBlackListService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="accessModerationService">Сервис модерации доступов.</param>
    /// <param name="projectModerationService">Сервис медерации проектов.</param>
    /// <param name="mapper">Автомаппер.</param>
    /// <param name="vacancyModerationService">Сервис модерации вакансий.</param>
    /// <param name="userBlackListService">Сервис ЧС пользователей.</param>
    public ModerationController(IAccessModerationService accessModerationService,
        IProjectModerationService projectModerationService,
        IMapper mapper,
        IVacancyModerationService vacancyModerationService, 
        IUserBlackListService userBlackListService)
    {
        _accessModerationService = accessModerationService;
        _projectModerationService = projectModerationService;
        _mapper = mapper;
        _vacancyModerationService = vacancyModerationService;
        _userBlackListService = userBlackListService;
    }

    /// <summary>
    /// Метод проверяет, имеет ли пользователь роль, которая дает доступ к модерации.
    /// </summary>
    /// <returns>Данные выходной модели.</returns>
    [HttpPost]
    [Route("check")]
    [ProducesResponseType(200, Type = typeof(ModerationRoleOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<ModerationRoleOutput> CheckUserRoleModerationAsync(
        [FromBody] ModerationRoleInput moderationRoleInput)
    {
        var result = await _accessModerationService.CheckUserRoleModerationAsync(moderationRoleInput.Email);

        return result;
    }

    /// <summary>
    /// Метод получает список проектов для модерации.
    /// </summary>
    /// <returns>Список проектов.</returns>
    [HttpGet]
    [Route("projects")]
    [ProducesResponseType(200, Type = typeof(ProjectsModerationResult))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<ProjectsModerationResult> ProjectsModerationAsync()
    {
        var result = await _projectModerationService.ProjectsModerationAsync();

        return result;
    }

    /// <summary>
    /// Метод получает проект для просмотра.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Данные проекта.</returns>
    [HttpGet]
    [Route("project/{projectId}/preview")]
    [ProducesResponseType(200, Type = typeof(UserProjectOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<UserProjectOutput> GetProjectModerationByProjectIdAsync([FromRoute] long projectId)
    {
        var prj = await _projectModerationService.GetProjectModerationByProjectIdAsync(projectId);
        var result = _mapper.Map<UserProjectOutput>(prj);

        return result;
    }

    /// <summary>
    /// Метод одобряет проект на модерации.
    /// </summary>
    /// <param name="approveProjectInput">Входная модель.</param>
    /// <returns>Выходная модель модерации.</returns>
    [HttpPatch]
    [Route("project/approve")]
    [ProducesResponseType(200, Type = typeof(UserProjectOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<ApproveProjectOutput> ApproveProjectAsync([FromBody] ApproveProjectInput approveProjectInput)
    {
        var result = await _projectModerationService.ApproveProjectAsync(approveProjectInput.ProjectId);

        return result;
    }

    /// <summary>
    /// Метод отклоняет проект на модерации.
    /// </summary>
    /// <param name="approveProjectInput">Входная модель.</param>
    /// <returns>Выходная модель модерации.</returns>
    [HttpPatch]
    [Route("project/reject")]
    [ProducesResponseType(200, Type = typeof(RejectProjectOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<RejectProjectOutput> RejectProjectAsync([FromBody] ApproveProjectInput approveProjectInput)
    {
        var result = await _projectModerationService.RejectProjectAsync(approveProjectInput.ProjectId);

        return result;
    }

    /// <summary>
    /// Метод получает вакансию для просмотра.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <returns>Данные вакансии.</returns>
    [HttpGet]
    [Route("vacancy/{vacancyId}/preview")]
    [ProducesResponseType(200, Type = typeof(UserVacancyOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<UserVacancyOutput> GetVacancyModerationByVacancyIdAsync([FromRoute] long vacancyId)
    {
        var vac = await _vacancyModerationService.GetVacancyModerationByVacancyIdAsync(vacancyId);
        var result = _mapper.Map<UserVacancyOutput>(vac);

        return result;
    }
    
    /// <summary>
    /// Метод получает список вакансий для модерации.
    /// </summary>
    /// <returns>Список вакансий.</returns>
    [HttpGet]
    [Route("vacancies")]
    [ProducesResponseType(200, Type = typeof(VacanciesModerationResult))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<VacanciesModerationResult> VacanciesModerationAsync()
    {
        var result = await _vacancyModerationService.VacanciesModerationAsync();

        return result;
    }
    
    /// <summary>
    /// Метод одобряет вакансию на модерации.
    /// </summary>
    /// <param name="approveVacancyInput">Входная модель.</param>
    /// <returns>Выходная модель модерации.</returns>
    [HttpPatch]
    [Route("vacancy/approve")]
    [ProducesResponseType(200, Type = typeof(ApproveVacancyOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<ApproveVacancyOutput> ApproveVacancyAsync([FromBody] ApproveVacancyInput approveVacancyInput)
    {
        var result = await _vacancyModerationService.ApproveVacancyAsync(approveVacancyInput.VacancyId);

        return result;
    }
    
    /// <summary>
    /// Метод отклоняет вакансию на модерации.
    /// </summary>
    /// <param name="rejectVacancyInput">Входная модель.</param>
    /// <returns>Выходная модель модерации.</returns>
    [HttpPatch]
    [Route("vacancy/reject")]
    [ProducesResponseType(200, Type = typeof(RejectVacancyOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<RejectVacancyOutput> RejectProjectAsync([FromBody] RejectVacancyInput rejectVacancyInput)
    {
        var result = await _vacancyModerationService.RejectVacancyAsync(rejectVacancyInput.VacancyId);

        return result;
    }

    /// <summary>
    /// Метод добавляет пользователя в ЧС.
    /// </summary>
    /// <param name="addUserBlackListInput">Входная модель.</param>
    [HttpPost]
    [Route("blacklist")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task AddUserBlackListAsync([FromBody] AddUserBlackListInput addUserBlackListInput)
    {
        await new AddUserBlackListValidator().ValidateAndThrowAsync(addUserBlackListInput);
        await _userBlackListService.AddUserBlackListAsync(addUserBlackListInput.UserId, addUserBlackListInput.Email,
            addUserBlackListInput.PhoneNumber);
    }

    /// <summary>
    /// Метод получает список пользователей в ЧС.
    /// </summary>
    /// <returns>Список пользователей в ЧС.</returns>
    [HttpGet]
    [Route("blacklist")]
    [ProducesResponseType(200, Type = typeof(UserBlackListResult))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<UserBlackListResult> GetUsersBlackListAsync()
    {
        var result = await _userBlackListService.GetUsersBlackListAsync();

        return result;
    }
}