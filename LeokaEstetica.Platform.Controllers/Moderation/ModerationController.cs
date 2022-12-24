using AutoMapper;
using LeokaEstetica.Platform.Access.Abstractions.Moderation;
using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Models.Dto.Output.Moderation.Project;
using LeokaEstetica.Platform.Models.Dto.Output.Project;
using LeokaEstetica.Platform.Moderation.Abstractions.Project;
using LeokaEstetica.Platform.Moderation.Models.Dto.Input;
using LeokaEstetica.Platform.Moderation.Models.Dto.Output;
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

    public ModerationController(IAccessModerationService accessModerationService,
        IProjectModerationService projectModerationService,
        IMapper mapper)
    {
        _accessModerationService = accessModerationService;
        _projectModerationService = projectModerationService;
        _mapper = mapper;
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
    /// Метод получает проект для просмотра/изменения.
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
    [Route("project/{projectId}/approve")]
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
}