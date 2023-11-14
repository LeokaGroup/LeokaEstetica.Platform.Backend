using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using LeokaEstetica.Platform.Access.Abstractions.Moderation;
using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Base.Filters;
using LeokaEstetica.Platform.Controllers.Validators.Access;
using LeokaEstetica.Platform.Models.Dto.Output.Moderation.Project;
using LeokaEstetica.Platform.Models.Dto.Output.Moderation.Resume;
using LeokaEstetica.Platform.Models.Dto.Output.Moderation.Vacancy;
using LeokaEstetica.Platform.Models.Dto.Output.Project;
using LeokaEstetica.Platform.Models.Dto.Output.Vacancy;
using LeokaEstetica.Platform.CallCenter.Abstractions.Project;
using LeokaEstetica.Platform.CallCenter.Abstractions.Resume;
using LeokaEstetica.Platform.CallCenter.Abstractions.Vacancy;
using LeokaEstetica.Platform.CallCenter.Models.Dto.Input.Access;
using LeokaEstetica.Platform.CallCenter.Models.Dto.Input.Project;
using LeokaEstetica.Platform.CallCenter.Models.Dto.Input.Resume;
using LeokaEstetica.Platform.CallCenter.Models.Dto.Input.Vacancy;
using LeokaEstetica.Platform.CallCenter.Models.Dto.Output.Access;
using LeokaEstetica.Platform.CallCenter.Models.Dto.Output.Project;
using LeokaEstetica.Platform.CallCenter.Models.Dto.Output.Role;
using LeokaEstetica.Platform.CallCenter.Models.Dto.Output.Vacancy;
using LeokaEstetica.Platform.Controllers.Validators.Project;
using LeokaEstetica.Platform.Database.Abstractions.Moderation.Project;
using LeokaEstetica.Platform.Database.Abstractions.Moderation.Resume;
using LeokaEstetica.Platform.Database.Abstractions.Moderation.Vacancy;
using LeokaEstetica.Platform.Models.Dto.Input.Moderation;
using LeokaEstetica.Platform.Services.Abstractions.Profile;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LeokaEstetica.Platform.Controllers.CallCenter;

/// <summary>
/// Контроллер КЦ (отвечает за весь функционал КЦ).
/// </summary>
[AuthFilter]
[ApiController]
[Route("callcenter")]
public class CallCenterController : BaseController
{
    private readonly IAccessModerationService _accessModerationService;
    private readonly IProjectModerationService _projectModerationService;
    private readonly IMapper _mapper;
    private readonly IVacancyModerationService _vacancyModerationService;
    private readonly IUserBlackListService _userBlackListService;
    private readonly IResumeModerationService _resumeModerationService;
    private readonly IProfileService _profileService;
    private readonly IProjectModerationRepository _projectModerationRepository;
    private readonly IVacancyModerationRepository _vacancyModerationRepository;
    private readonly IResumeModerationRepository _resumeModerationRepository;
    private readonly ILogger<CallCenterController> _logger;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="accessModerationService">Сервис модерации доступов.</param>
    /// <param name="projectModerationService">Сервис медерации проектов.</param>
    /// <param name="mapper">Автомаппер.</param>
    /// <param name="vacancyModerationService">Сервис модерации вакансий.</param>
    /// <param name="userBlackListService">Сервис ЧС пользователей.</param>
    /// <param name="resumeModerationService">Сервис модерации анкет.</param>
    /// <param name="profileService">Сервис профиля пользователя.</param>
    /// <param name="projectModerationRepository">Репозиторий модерации проектов.</param>
    /// <param name="vacancyModerationRepository">Репозиторий модерации вакансий.</param>
    /// <param name="resumeModerationRepository">Репозиторий модерации анкет.</param>
    /// <param name="logger">Логгер.</param>
    public CallCenterController(IAccessModerationService accessModerationService,
        IProjectModerationService projectModerationService,
        IMapper mapper,
        IVacancyModerationService vacancyModerationService, 
        IUserBlackListService userBlackListService, 
        IResumeModerationService resumeModerationService, 
        IProfileService profileService, 
        IProjectModerationRepository projectModerationRepository, 
        IVacancyModerationRepository vacancyModerationRepository, 
        IResumeModerationRepository resumeModerationRepository,
        ILogger<CallCenterController> logger)
    {
        _accessModerationService = accessModerationService;
        _projectModerationService = projectModerationService;
        _mapper = mapper;
        _vacancyModerationService = vacancyModerationService;
        _userBlackListService = userBlackListService;
        _resumeModerationService = resumeModerationService;
        _profileService = profileService;
        _projectModerationRepository = projectModerationRepository;
        _vacancyModerationRepository = vacancyModerationRepository;
        _resumeModerationRepository = resumeModerationRepository;
        _logger = logger;
    }

    /// <summary>
    /// Метод проверяет, имеет ли пользователь роль, которая дает доступ к модерации.
    /// </summary>
    /// <returns>Данные выходной модели.</returns>
    [HttpGet]
    [Route("check")]
    [ProducesResponseType(200, Type = typeof(ModerationRoleOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<ModerationRoleOutput> CheckUserRoleModerationAsync()
    {
        var result = await _accessModerationService.CheckUserRoleModerationAsync(GetUserName());

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
    public async Task<ProjectOutput> GetProjectModerationByProjectIdAsync([FromRoute] long projectId)
    {
        var result = await _projectModerationService.GetProjectModerationByProjectIdAsync(projectId);

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
        var result = await _projectModerationService.ApproveProjectAsync(approveProjectInput.ProjectId, GetUserName());

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
        var result = await _projectModerationService.RejectProjectAsync(approveProjectInput.ProjectId, GetUserName());

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

    /// <summary>
    /// Метод получает список анкет для модерации.
    /// </summary>
    /// <returns>Список анкет.</returns>
    [HttpGet]
    [Route("resumes")]
    [ProducesResponseType(200, Type = typeof(ResumeModerationResult))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<ResumeModerationResult> ResumesModerationAsync()
    {
        var result = await _resumeModerationService.ResumesModerationAsync();

        return result;
    }
    
    /// <summary>
    /// Метод одобряет анкету на модерации.
    /// </summary>
    /// <param name="approveResumeInput">Входная модель.</param>
    [HttpPatch]
    [Route("resume/approve")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task ApproveResumeAsync([FromBody] ApproveResumeInput approveResumeInput)
    {
        await _resumeModerationService.ApproveResumeAsync(approveResumeInput.ProfileInfoId);
    }
    
    /// <summary>
    /// Метод отклоняет анкету на модерации.
    /// </summary>
    /// <param name="rejectResumeInput">Входная модель.</param>
    [HttpPatch]
    [Route("resume/reject")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task RejectResumeAsync([FromBody] RejectResumeInput rejectResumeInput)
    {
        await _resumeModerationService.RejectResumeAsync(rejectResumeInput.ProfileInfoId);
    }

    /// <summary>
    /// Метод создает замечания проекта.
    /// </summary>
    /// <param name="createProjectRemarkInput">Входная модель.</param>
    /// <returns>Список замечаний проекта.</returns>
    [HttpPost]
    [Route("project-remarks")]
    [ProducesResponseType(200, Type = typeof(ProjectRemarkResult))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<ProjectRemarkResult> CreateProjectRemarksAsync(
        [FromBody] CreateProjectRemarkInput createProjectRemarkInput)
    {
        var projectRemarks = await _projectModerationService.CreateProjectRemarksAsync(createProjectRemarkInput, 
            GetUserName(), GetTokenFromHeader());
        
        var result = new ProjectRemarkResult
        {
            ProjectRemarks = _mapper.Map<List<ProjectRemarkOutput>>(projectRemarks)
        };

        return result;
    }

    /// <summary>
    /// Метод отправляет замечания проекта владельцу проекта.
    /// Отправка замечаний проекту подразумевает просто изменение статуса замечаниям проекта.
    /// <param name="sendProjectRemarkInput">Входная модель.</param>
    /// </summary>
    [HttpPatch]
    [Route("send-project-remarks")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task SendProjectRemarksAsync([FromBody] SendProjectRemarkInput sendProjectRemarkInput)
    {
        await _projectModerationService.SendProjectRemarksAsync(sendProjectRemarkInput.ProjectId, GetUserName(),
            GetTokenFromHeader());
    }
    
    /// <summary>
    /// Метод создает замечания вакансии.
    /// </summary>
    /// <param name="createVacancyRemarkInput">Входная модель.</param>
    /// <returns>Список замечаний вакансии.</returns>
    [HttpPost]
    [Route("vacancy-remarks")]
    [ProducesResponseType(200, Type = typeof(ProjectRemarkResult))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<VacancyRemarkResult> CreateVacancyRemarksAsync(
        [FromBody] CreateVacancyRemarkInput createVacancyRemarkInput)
    {
        var vacancyRemarks = await _vacancyModerationService.CreateVacancyRemarksAsync(createVacancyRemarkInput, 
            GetUserName(), GetTokenFromHeader());
        
        var result = new VacancyRemarkResult
        {
            VacancyRemarks = new List<VacancyRemarkOutput>()
        };
        
        result.VacancyRemarks = _mapper.Map<List<VacancyRemarkOutput>>(vacancyRemarks);

        return result;
    }
    
    /// <summary>
    /// Метод отправляет замечания вакансии владельцу проекта.
    /// Отправка замечаний проекту подразумевает просто изменение статуса замечаниям проекта.
    /// <param name="sendVacancyRemarkInput">Входная модель.</param>
    /// </summary>
    [HttpPatch]
    [Route("send-vacancy-remarks")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task SendVacancyRemarksAsync([FromBody] SendVacancyRemarkInput sendVacancyRemarkInput)
    {
        await _vacancyModerationService.SendVacancyRemarksAsync(sendVacancyRemarkInput.VacancyId, 
            GetTokenFromHeader());
    }
    
    /// <summary>
    /// Метод создает замечания анкеты.
    /// </summary>
    /// <param name="createResumeRemarkInput">Входная модель.</param>
    /// <returns>Список замечаний анкеты.</returns>
    [HttpPost]
    [Route("resume-remarks")]
    [ProducesResponseType(200, Type = typeof(ResumeRemarkResult))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<ResumeRemarkResult> CreateVacancyRemarksAsync(
        [FromBody] CreateResumeRemarkInput createResumeRemarkInput)
    {
        var vacancyRemarks = await _resumeModerationService.CreateResumeRemarksAsync(createResumeRemarkInput, 
            GetUserName(), GetTokenFromHeader());
        
        var result = new ResumeRemarkResult
        {
            ResumeRemarks = _mapper.Map<List<ResumeRemarkOutput>>(vacancyRemarks)
        };

        return result;
    }
    
    /// <summary>
    /// Метод отправляет замечания вакансии владельцу проекта.
    /// Отправка замечаний проекту подразумевает просто изменение статуса замечаниям проекта.
    /// <param name="sendResumeRemarkInput">Входная модель.</param>
    /// </summary>
    [HttpPatch]
    [Route("send-resume-remarks")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task SendResumeRemarksAsync([FromBody] SendResumeRemarkInput sendResumeRemarkInput)
    {
        await _resumeModerationService.SendResumeRemarksAsync(sendResumeRemarkInput.ProfileInfoId,
            GetTokenFromHeader());
    }

    /// <summary>
    /// Метод получает данные профиля пользователя для модерации (композитная модель собирает результаты).
    /// </summary>
    /// <param name="profileInfoId">Id анкеты пользователя.</param>
    /// <returns>Композитная модель с результатами.</returns>
    [HttpGet]
    [Route("resume/{profileInfoId}/preview")]
    [ProducesResponseType(200, Type = typeof(ProfileInfoCompositeOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<ProfileInfoCompositeOutput> GetProfileInfoAsync([FromRoute] long profileInfoId)
    {
        var result = new ProfileInfoCompositeOutput
        {
            ProfileInfo = await _profileService.GetProfileInfoAsync(profileInfoId)
        };

        var account = result.ProfileInfo.Email;
        result.Skills = await _profileService.SelectedProfileUserSkillsAsync(account);
        result.Intents = await _profileService.SelectedProfileUserIntentsAsync(account);

        return result;
    }
    
    /// <summary>
    /// Метод получает список замечаний проекта (не отправленные), если они есть.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Список замечаний проекта.</returns>
    [HttpGet]
    [Route("project/{projectId}/remarks/unshipped")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<ProjectRemarkOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<ProjectRemarkOutput>> GetProjectUnShippedRemarksAsync([FromRoute] long projectId)
    {
        var items = await _projectModerationService.GetProjectUnShippedRemarksAsync(projectId);
        var result = _mapper.Map<IEnumerable<ProjectRemarkOutput>>(items);

        return result;
    }
    
    /// <summary>
    /// Метод получает список замечаний вакансии (не отправленные), если они есть.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <returns>Список замечаний вакансии.</returns>
    [HttpGet]
    [Route("vacancy/{vacancyId}/remarks/unshipped")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<VacancyRemarkOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<VacancyRemarkOutput>> GetVacancyUnShippedRemarksAsync([FromRoute] long vacancyId)
    {
        var items = await _vacancyModerationService.GetVacancyUnShippedRemarksAsync(vacancyId);
        var result = _mapper.Map<IEnumerable<VacancyRemarkOutput>>(items);

        return result;
    }
    
    /// <summary>
    /// Метод получает список замечаний анкеты (не отправленные), если они есть.
    /// </summary>
    /// <param name="profileInfoId">Id анкеты.</param>
    /// <returns>Список замечаний анкеты.</returns>
    [HttpGet]
    [Route("profile/{profileInfoId}/remarks/unshipped")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<ResumeRemarkOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<ResumeRemarkOutput>> GetResumeUnShippedRemarksAsync(
        [FromRoute] long profileInfoId)
    {
        var items = await _resumeModerationService.GetResumeUnShippedRemarksAsync(profileInfoId);
        var result = _mapper.Map<IEnumerable<ResumeRemarkOutput>>(items);

        return result;
    }
    
    /// <summary>
    /// Метод получает список замечаний проекта (не отправленные), если они есть.
    /// Выводим эти данные в таблицу замечаний проектов журнала модерации.
    /// </summary>
    /// <returns>Список замечаний проекта.</returns>
    [HttpGet]
    [Route("projects/remarks/unshipped-table")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<ProjectRemarkTableOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<ProjectRemarkTableOutput>> GetProjectUnShippedRemarksTableAsync()
    {
        var items = await _projectModerationRepository.GetProjectUnShippedRemarksTableAsync();
        var result = _mapper.Map<IEnumerable<ProjectRemarkTableOutput>>(items);

        return result;
    }
    
    /// <summary>
    /// Метод получает список замечаний вакансии (не отправленные), если они есть.
    /// Выводим эти данные в таблицу замечаний вакансии журнала модерации.
    /// </summary>
    /// <returns>Список замечаний вакансии.</returns>
    [HttpGet]
    [Route("vacancies/remarks/unshipped-table")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<VacancyRemarkTableOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<VacancyRemarkTableOutput>> GetVacancyUnShippedRemarksTableAsync()
    {
        var items = await _vacancyModerationRepository.GetVacancyUnShippedRemarksTableAsync();
        var result = _mapper.Map<IEnumerable<VacancyRemarkTableOutput>>(items);

        return result;
    }
    
    /// <summary>
    /// Метод получает список замечаний анкеты (не отправленные), если они есть.
    /// Выводим эти данные в таблицу замечаний анкет журнала модерации.
    /// </summary>
    /// <returns>Список замечаний анкеты.</returns>
    [HttpGet]
    [Route("profile/remarks/unshipped-table")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<ResumeRemarkTableOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<ResumeRemarkTableOutput>> GetResumeUnShippedRemarksTableAsync()
    {
        var items = await _resumeModerationRepository.GetResumeUnShippedRemarksTableAsync();
        var result = _mapper.Map<IEnumerable<ResumeRemarkTableOutput>>(items);

        return result;
    }

    /// <summary>
    /// Метод получает проекты, замечания которых ожидают проверки модератором.
    /// </summary>
    /// <returns>Список проектов.</returns>
    [HttpGet]
    [Route("awaiting-correction/projects")]
    [ProducesResponseType(200, Type = typeof(AwaitingCorrectionProjectResult))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<AwaitingCorrectionProjectResult> GetProjectsAwaitingCorrectionAsync()
    {
        var items = await _projectModerationRepository.GetProjectsAwaitingCorrectionAsync();
        var result = new AwaitingCorrectionProjectResult
        {
            AwaitingCorrectionProjects = _mapper.Map<IEnumerable<ProjectRemarkOutput>>(items)
        };

        return result;
    }
    
    /// <summary>
    /// Метод получает анкеты, замечания которых ожидают проверки модератором.
    /// </summary>
    /// <returns>Список анкет.</returns>
    [HttpGet]
    [Route("awaiting-correction/resumes")]
    [ProducesResponseType(200, Type = typeof(AwaitingCorrectionProjectResult))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<AwaitingCorrectionResumeResult> GetResumesAwaitingCorrectionAsync()
    {
        var items = await _projectModerationRepository.GetResumesAwaitingCorrectionAsync();
        var result = new AwaitingCorrectionResumeResult
        {
            AwaitingCorrectionResumes = _mapper.Map<IEnumerable<ResumeRemarkOutput>>(items)
        };

        return result;
    }
    
    /// <summary>
    /// Метод получает вакансии, замечания которых ожидают проверки модератором.
    /// </summary>
    /// <returns>Список вакансий.</returns>
    [HttpGet]
    [Route("awaiting-correction/vacancies")]
    [ProducesResponseType(200, Type = typeof(AwaitingCorrectionVacancyResult))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<AwaitingCorrectionVacancyResult> GetVacanciesAwaitingCorrectionAsync()
    {
        var items = await _projectModerationRepository.GetVacanciesAwaitingCorrectionAsync();
        var result = new AwaitingCorrectionVacancyResult
        {
            AwaitingCorrectionVacancies = _mapper.Map<IEnumerable<VacancyRemarkOutput>>(items)
        };

        return result;
    }

    /// <summary>
    /// Метод получает комментарии на модерации.
    /// </summary>
    /// <returns>Комментарии на модерации.</returns>
    [HttpGet]
    [Route("project-comments")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<ProjectCommentModerationOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<ProjectCommentModerationOutput>> GetProjectCommentsModerationAsync()
    {
        var items = await _projectModerationRepository.GetProjectCommentsModerationAsync();
        var result = _mapper.Map<IEnumerable<ProjectCommentModerationOutput>>(items);

        return result;
    }
    
    /// <summary>
    /// Метод получает комментарий проекта для просмотра.
    /// </summary>
    /// <param name="commentId">Id комментария.</param>
    /// <returns>Данные комментария.</returns>
    [HttpGet]
    [Route("project/{commentId}/preview")]
    [ProducesResponseType(200, Type = typeof(ProjectCommentModerationOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<ProjectCommentModerationOutput> GetCommentModerationByCommentIdAsync([FromRoute] long commentId)
    {
        var result = new ProjectCommentModerationOutput();
        var validator = await new GetProjectCommentValidator().ValidateAsync(commentId);

        if (validator.Errors.Any())
        {
            _logger.LogError(validator.Errors.First().ErrorMessage,
                $"Ошибка при получении комментария проекта. CommentId: {commentId}");

            return result;
        }
        
        var item = await _projectModerationService.GetCommentModerationByCommentIdAsync(commentId);
        result = _mapper.Map<ProjectCommentModerationOutput>(item);

        return result;
    }
    
    /// <summary>
    /// Метод одобряет комментарий проекта.
    /// </summary>
    /// <param name="projectCommentModerationInput">Входная модель.</param>
    /// <returns>Выходная модель.</returns>
    [HttpPatch]
    [Route("project/comment/approve")]
    [ProducesResponseType(200, Type = typeof(ManagingProjectCommentModerationOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<ManagingProjectCommentModerationOutput> ApproveProjectCommentAsync(
        [FromBody] ProjectCommentModerationInput projectCommentModerationInput)
    {
        var commentId = projectCommentModerationInput.CommentId;
        var validator = await new ApproveProjectCommentValidator().ValidateAsync(commentId);
        var result = new ManagingProjectCommentModerationOutput
        {
            Errors = new List<ValidationFailure>()
        };
        
        if (validator.Errors.Any())
        {
            var err = validator.Errors.First().ErrorMessage;
            _logger.LogError(err, $"Ошибка при одобрении комментария проекта. CommentId: {commentId}");

            result.Errors.Add(new ValidationFailure
            {
                ErrorMessage = err
            });

            return result;
        }

        result.IsSuccess = await _projectModerationService.ApproveProjectCommentAsync(commentId);
        
        return result;
    }

    /// <summary>
    /// Метод отклоняет комментарий проекта.
    /// </summary>
    /// <param name="projectCommentModerationInput">Входная модель.</param>
    /// <returns>Выходная модель.</returns>
    [HttpPatch]
    [Route("project/comment/reject")]
    [ProducesResponseType(200, Type = typeof(ManagingProjectCommentModerationOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<ManagingProjectCommentModerationOutput> RejectProjectCommentAsync(
        [FromBody] ProjectCommentModerationInput projectCommentModerationInput)
    {
        var commentId = projectCommentModerationInput.CommentId;
        var validator = await new ApproveProjectCommentValidator().ValidateAsync(commentId);
        var result = new ManagingProjectCommentModerationOutput
        {
            Errors = new List<ValidationFailure>()
        };
        
        if (validator.Errors.Any())
        {
            var err = validator.Errors.First().ErrorMessage;
            _logger.LogError(err, $"Ошибка при отклонении комментария проекта. CommentId: {commentId}");

            result.Errors.Add(new ValidationFailure
            {
                ErrorMessage = err
            });

            return result;
        }

        result = await _projectModerationService.RejectProjectCommentAsync(commentId);
        
        return result;
    }
}