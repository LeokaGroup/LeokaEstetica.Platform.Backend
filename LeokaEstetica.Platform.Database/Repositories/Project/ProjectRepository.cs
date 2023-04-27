using System.Data;
using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Core.Enums;
using LeokaEstetica.Platform.Core.Exceptions;
using LeokaEstetica.Platform.Database.Abstractions.Project;
using LeokaEstetica.Platform.Database.Chat;
using LeokaEstetica.Platform.Models.Dto.Output.Project;
using LeokaEstetica.Platform.Models.Entities.Communication;
using LeokaEstetica.Platform.Models.Entities.Configs;
using LeokaEstetica.Platform.Models.Entities.Moderation;
using LeokaEstetica.Platform.Models.Entities.Project;
using LeokaEstetica.Platform.Models.Entities.ProjectTeam;
using LeokaEstetica.Platform.Models.Entities.Vacancy;
using LeokaEstetica.Platform.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace LeokaEstetica.Platform.Database.Repositories.Project;

/// <summary>
/// Класс реализует метод репозитория проектов.
/// </summary>
public class ProjectRepository : IProjectRepository
{
    private readonly PgContext _pgContext;
    private readonly IChatRepository _chatRepository;
    
    /// <summary>
    /// Список статусов вакансий, которые надо исключать при атаче вакансий к проекту.
    /// </summary>
    private static readonly List<long> _excludedVacanciesStatuses = new()
    {
        (int)VacancyModerationStatusEnum.ModerationVacancy,
        (int)VacancyModerationStatusEnum.RejectedVacancy
    };

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="pgContext">Датаконтекст.</param>
    /// <param name="chatRepository">Репозиторий чата.</param>
    public ProjectRepository(PgContext pgContext,
        IChatRepository chatRepository)
    {
        _pgContext = pgContext;
        _chatRepository = chatRepository;
    }

    /// <summary>
    /// Метод создает новый проект пользователя.
    /// </summary>
    /// <param name="projectName">Название проекта.</param>
    /// <param name="projectDetails">Описание проекта.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="statusSysName">Системное название статуса.</param>
    /// <param name="statusName">Русское название статуса.</param>
    /// <param name="projectStage">Стадия проекта.</param>
    /// <returns>Данные нового проекта.</returns>
    public async Task<UserProjectEntity> CreateProjectAsync(string projectName, string projectDetails, long userId,
        string statusSysName, string statusName, ProjectStageEnum projectStage)
    {
        var transaction = await _pgContext.Database
            .BeginTransactionAsync(IsolationLevel.ReadCommitted);

        try
        {
            var project = new UserProjectEntity
            {
                ProjectName = projectName,
                ProjectDetails = projectDetails,
                UserId = userId,
                ProjectCode = Guid.NewGuid(),
                DateCreated = DateTime.Now
            };
            await _pgContext.UserProjects.AddAsync(project);

            // Дергаем сохранение тут, так как нам нужен Id добавленного проекта.
            // Фактического сохраненеия не произойдет, пока мы не завершили транзакцию.
            await _pgContext.SaveChangesAsync();

            // Проставляем проекту статус "На модерации".
            await _pgContext.ProjectStatuses.AddAsync(new ProjectStatusEntity
            {
                ProjectId = project.ProjectId,
                ProjectStatusSysName = statusSysName,
                ProjectStatusName = statusName
            });

            // Записываем стадию проекта.
            await _pgContext.UserProjectsStages.AddAsync(new UserProjectStageEntity
            {
                ProjectId = project.ProjectId,
                StageId = (int)projectStage
            });

            // Отправляем проект на модерацию.
            await SendModerationProjectAsync(project.ProjectId);

            // Создаем команду проекта по дефолту.
            await _pgContext.ProjectsTeams.AddAsync(new ProjectTeamEntity
            {
                Created = DateTime.Now,
                ProjectId = project.ProjectId
            });

            await _pgContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return project;
        }

        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    /// <summary>
    /// Метод получает названия полей для таблицы проектов пользователя.
    /// Все названия столбцов этой таблицы одинаковые у всех пользователей.
    /// </summary>
    /// <returns>Список названий полей таблицы.</returns>
    public async Task<IEnumerable<ProjectColumnNameEntity>> UserProjectsColumnsNamesAsync()
    {
        var result = await _pgContext.ProjectColumnsNames
            .OrderBy(o => o.Position)
            .ToListAsync();

        return result;
    }

    /// <summary>
    /// Метод проверяет, создан ли уже такой заказ под текущим пользователем с таким названием.
    /// </summary>
    /// <param name="projectName">Название проекта.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Создал либо нет.</returns>
    public async Task<bool> CheckCreatedProjectByProjectNameAsync(string projectName, long userId)
    {
        var result = await _pgContext.UserProjects
            .AnyAsync(p => p.UserId == userId
                           && p.ProjectName.Equals(projectName));

        return result;
    }

    /// <summary>
    /// Метод получает список проектов пользователя.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Список проектов.</returns>
    public async Task<UserProjectResultOutput> UserProjectsAsync(long userId)
    {
        var result = new UserProjectResultOutput
        {
            UserProjects = await _pgContext.ModerationProjects
                .Include(ms => ms.ModerationStatus)
                .Include(up => up.UserProject)
                .Where(u => u.UserProject.UserId == userId)
                .Select(p => new UserProjectOutput
                {
                    ProjectName = p.UserProject.ProjectName,
                    ProjectDetails = p.UserProject.ProjectDetails,
                    ProjectIcon = p.UserProject.ProjectIcon,
                    ProjectStatusName = p.ModerationStatus.StatusName,
                    ProjectStatusSysName = p.ModerationStatus.StatusSysName,
                    ProjectCode = p.UserProject.ProjectCode,
                    ProjectId = p.UserProject.ProjectId
                })
                .ToListAsync()
        };

        return result;
    }

    /// <summary>
    /// TODO: Подумать, давать ли всем пользователям возможность просматривать каталог проектов или только тем, у кого есть подписка.
    /// Метод получает список проектов для каталога.
    /// </summary>
    /// <returns>Список проектов.</returns>
    public async Task<IEnumerable<CatalogProjectOutput>> CatalogProjectsAsync()
    {
        var result = await _pgContext.CatalogProjects
            .Include(p => p.Project)
            .Where(p => p.Project.ArchivedProjects.All(a => a.ProjectId != p.ProjectId))
            .Select(p => new CatalogProjectOutput
            {
                ProjectId = p.Project.ProjectId,
                ProjectName = p.Project.ProjectName,
                DateCreated = p.Project.DateCreated,
                ProjectIcon = p.Project.ProjectIcon,
                ProjectDetails = p.Project.ProjectDetails,
                UserId = p.Project.UserId
            })
            .ToListAsync();

        return result;
    }

    /// <summary>
    /// Метод обновляет проект пользователя.
    /// </summary>
    /// <param name="projectName">Название проекта.</param>
    /// <param name="projectDetails">Описание проекта.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="projectStage">Стадия проекта.</param>
    /// <returns>Данные нового проекта.</returns>
    public async Task<UpdateProjectOutput> UpdateProjectAsync(string projectName, string projectDetails, long userId,
        long projectId, ProjectStageEnum projectStage)
    {
        var transaction = await _pgContext.Database
            .BeginTransactionAsync(IsolationLevel.ReadCommitted);

        try
        {
            var project = await _pgContext.UserProjects
                .FirstOrDefaultAsync(p => p.UserId == userId
                                          && p.ProjectId == projectId);

            if (project is null)
            {
                throw new InvalidOperationException(
                    $"Проект не найден для обновления. ProjectId был {projectId}. UserId был {userId}");
            }

            project.ProjectName = projectName;
            project.ProjectDetails = projectDetails;

            // Проставляем стадию проекта.
            var stage = await _pgContext.UserProjectsStages
                .Where(p => p.ProjectId == project.ProjectId)
                .FirstOrDefaultAsync();

            if (stage is null)
            {
                throw new InvalidOperationException($"У проекта не записана стадия. ProjectId был {projectId}.");
            }

            stage.StageId = (int)projectStage;
            
            // Если проект уже был на модерации, то обновим статус.
            var isModerationExists = await IsModerationExistsProjectAsync(project.ProjectId);
            
            if (!isModerationExists)
            {
                // Отправляем проект на модерацию.
                await SendModerationProjectAsync(project.ProjectId);
            }
            
            else
            {
                await UpdateModerationProjectStatusAsync(project.ProjectId,
                    ProjectModerationStatusEnum.ModerationProject);
            }

            var result = new UpdateProjectOutput
            {
                DateCreated = project.DateCreated,
                ProjectName = project.ProjectName,
                ProjectDetails = project.ProjectDetails,
                ProjectIcon = project.ProjectIcon,
                ProjectId = project.ProjectId
            };

            await _pgContext.SaveChangesAsync();
            
            await transaction.CommitAsync();

            return result;
        }

        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    /// <summary>
    /// Метод получает проект для изменения или просмотра.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Данные проекта.</returns>
    public async Task<(UserProjectEntity UserProject, ProjectStageEntity ProjectStage)> GetProjectAsync(long projectId)
    {
        (UserProjectEntity UserProject, ProjectStageEntity ProjectStage) result = (null, null);

        result.Item1 = await _pgContext.UserProjects
            .FirstOrDefaultAsync(p => p.ProjectId == projectId);

        // Получаем стадию проекта пользователя.
        var projectStageId = await _pgContext.UserProjectsStages
            .Where(p => p.ProjectId == projectId)
            .Select(p => p.StageId)
            .FirstOrDefaultAsync();

        // Берем полные данные о стадии проекта.
        result.Item2 = await _pgContext.ProjectStages
            .Where(ps => ps.StageId == projectStageId)
            .Select(ps => new ProjectStageEntity
            {
                StageName = ps.StageName,
                StageSysName = ps.StageSysName,
                StageId = ps.StageId
            })
            .FirstOrDefaultAsync();

        return result;
    }

    /// <summary>
    /// Метод отправляет проект на модерацию.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    private async Task SendModerationProjectAsync(long projectId)
    {
        // Добавляем проект в таблицу модерации проектов.
        await _pgContext.ModerationProjects.AddAsync(new ModerationProjectEntity
        {
            DateModeration = DateTime.Now,
            ProjectId = projectId,
            ModerationStatusId = (int)ProjectModerationStatusEnum.ModerationProject
        });
    }

    /// <summary>
    /// Метод получает стадии проекта для выбора.
    /// </summary>
    /// <returns>Стадии проекта.</returns>
    public async Task<IEnumerable<ProjectStageEntity>> ProjectStagesAsync()
    {
        var result = await _pgContext.ProjectStages
            .OrderBy(o => o.Position)
            .ToListAsync();

        return result;
    }

    /// <summary>
    /// Метод получает список вакансий проекта. Список вакансий, которые принадлежат владельцу проекта.
    /// </summary>
    /// <param name="projectId">Id проекта, вакансии которого нужно получить.</param>
    /// <returns>Список вакансий.</returns>
    public async Task<IEnumerable<ProjectVacancyEntity>> ProjectVacanciesAsync(long projectId)
    {
        var result = await _pgContext.ProjectVacancies
            .Include(uv => uv.UserVacancy)
            .Where(pv => pv.ProjectId == projectId)
            .OrderBy(o => o.ProjectVacancyId)
            .ToListAsync();

        return result;
    }

    /// <summary>
    /// Метод прикрепляет вакансию к проекту.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <returns>Флаг успеха.</returns>
    public async Task<bool> AttachProjectVacancyAsync(long projectId, long vacancyId)
    {
        var isDublicateProjectVacancy = await _pgContext.ProjectVacancies
            .AnyAsync(p => p.ProjectId == projectId
                           && p.VacancyId == vacancyId);

        // Если такая вакансия уже прикреплена к проекту.
        if (isDublicateProjectVacancy)
        {
            return true;
        }

        await _pgContext.ProjectVacancies.AddAsync(new ProjectVacancyEntity
        {
            ProjectId = projectId,
            VacancyId = vacancyId
        });
        await _pgContext.SaveChangesAsync();

        return false;
    }

    /// <summary>
    /// Метод получает список вакансий проекта, которые можно прикрепить к проекту.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Список вакансий проекта.</returns>
    public async Task<IEnumerable<ProjectVacancyEntity>> ProjectVacanciesAvailableAttachAsync(long projectId,
        long userId)
    {
        // Получаем Id вакансий, которые уже прикреплены к проекту. Их исключаем.
        var attachedVacanciesIds = _pgContext.ProjectVacancies
            .Where(p => p.ProjectId == projectId)
            .Select(p => p.VacancyId)
            .AsQueryable();
        
        // Получаем Id вакансий, которые еще на модерации либо отклонены модератором, так как их нельзя атачить.
        var moderationVacanciesIds = _pgContext.ModerationVacancies
            .Where(v => _excludedVacanciesStatuses.Contains(v.ModerationStatusId))
            .Select(v => v.VacancyId)
            .AsQueryable();

        // Получаем вакансии, которые можно прикрепить к проекту.
        var result = await _pgContext.UserVacancies
            .Where(v => v.UserId == userId
                        && !attachedVacanciesIds.Contains(v.VacancyId)
                        && !moderationVacanciesIds.Contains(v.VacancyId))
            .Select(v => new ProjectVacancyEntity
            {
                ProjectId = projectId,
                VacancyId = v.VacancyId,
                UserVacancy = new UserVacancyEntity
                {
                    VacancyName = v.VacancyName,
                    VacancyText = v.VacancyText,
                    Employment = v.Employment,
                    WorkExperience = v.WorkExperience,
                    DateCreated = v.DateCreated,
                    Payment = v.Payment,
                    UserId = userId,
                    VacancyId = v.VacancyId,
                }
            })
            .OrderBy(o => o.VacancyId)
            .ToListAsync();

        return result;
    }

    /// <summary>
    /// Метод записывает отклик на проект.
    /// Отклик может быть с указанием вакансии, на которую идет отклик (если указана VacancyId).
    /// Отклик может быть без указаниея вакансии, на которую идет отклик (если не указана VacancyId).
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Выходная модель с записанным откликом.</returns>
    public async Task<ProjectResponseEntity> WriteProjectResponseAsync(long projectId, long? vacancyId, long userId)
    {
        var isDublicate = await _pgContext.ProjectResponses
            .AnyAsync(p => p.UserId == userId
                           && p.ProjectId == projectId);

        // Если уже оставляли отклик на проект.
        if (isDublicate)
        {
            throw new DublicateProjectResponseException();
        }

        var response = new ProjectResponseEntity
        {
            ProjectId = projectId,
            UserId = userId,
            VacancyId = vacancyId,
            ProjectResponseStatuseId = (int)ProjectResponseStatusEnum.Wait,
            DateResponse = DateTime.Now
        };

        await _pgContext.ProjectResponses.AddAsync(response);
        await _pgContext.SaveChangesAsync();

        return response;
    }

    /// <summary>
    /// Метод находит Id владельца проекта.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Id владельца проекта.</returns>
    public async Task<long> GetProjectOwnerIdAsync(long projectId)
    {
        var result = await _pgContext.UserProjects
            .Where(p => p.ProjectId == projectId)
            .Select(p => p.UserId)
            .FirstOrDefaultAsync();

        return result;
    }

    /// <summary>
    /// Метод получает данные команды проекта.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Данные команды проекта.</returns>
    public async Task<ProjectTeamEntity> GetProjectTeamAsync(long projectId)
    {
        var result = await _pgContext.ProjectsTeams
            .Where(t => t.ProjectId == projectId)
            .Select(t => new ProjectTeamEntity
            {
                ProjectId = t.ProjectId,
                TeamId = t.TeamId
            })
            .FirstOrDefaultAsync();

        return result;
    }

    /// <summary>
    /// Метод получает список участников команды проекта по Id команды.
    /// </summary>
    /// <param name="teamId">Id проекта.</param>
    /// <returns>Список участников команды проекта.</returns>
    public async Task<List<ProjectTeamMemberEntity>> GetProjectTeamMembersAsync(long teamId)
    {
        var result = await (from ptm in _pgContext.ProjectTeamMembers
                where ptm.TeamId == teamId
                select new ProjectTeamMemberEntity
                {
                    UserId = ptm.UserId,
                    Joined = ptm.Joined,
                    TeamId = ptm.TeamId,
                    MemberId = ptm.MemberId,
                    UserVacancy = new UserVacancyEntity
                    {
                        VacancyId = ptm.VacancyId ?? 0
                    }
                })
            .ToListAsync();

        return result;
    }

    /// <summary>
    /// Метод получает названия полей для таблицы команды проекта пользователя.
    /// </summary>
    /// <returns>Список названий полей таблицы.</returns>
    public async Task<IEnumerable<ProjectTeamColumnNameEntity>> ProjectTeamColumnsNamesAsync()
    {
        var result = await _pgContext.ProjectTeamColumnNames
            .OrderBy(o => o.Position)
            .ToListAsync();

        return result;
    }

    /// <summary>
    /// Метод добавляет пользователя в команду проекта.
    /// </summary>
    /// <param name="userId">Id пользователя, который будет добавлен в команду проекта.</param>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <returns>Данные добавленного пользователя.</returns>
    public async Task<ProjectTeamMemberEntity> AddProjectTeamMemberAsync(long userId, long? vacancyId, long teamId)
    {
        var result = new ProjectTeamMemberEntity
        {
            Joined = DateTime.Now,
            UserId = userId,
            VacancyId = vacancyId,
            TeamId = teamId
        };
        
        await _pgContext.ProjectTeamMembers.AddAsync(result);
        
        // Добавляем вакансию отклика.
        if (vacancyId is not null)
        {
            var vacancy = new ProjectTeamVacancyEntity
            {
                VacancyId = (long)vacancyId,
                IsActive = true,
                TeamId = teamId
            };
            
            await _pgContext.ProjectsTeamsVacancies.AddAsync(vacancy);
        }
        
        await _pgContext.SaveChangesAsync();

        return result;
    }

    /// <summary>
    /// Метод находит Id команды проекта.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Id команды.</returns>
    public async Task<long> GetProjectTeamIdAsync(long projectId)
    {
        var result = await _pgContext.ProjectsTeams
            .Where(p => p.ProjectId == projectId)
            .Select(p => p.TeamId)
            .FirstOrDefaultAsync();

        return result;
    }

    /// <summary>
    /// Метод получает список проектов для дальнейшей фильтрации.
    /// </summary>
    /// <returns>Список проектов без выгрузки в память, так как этот список будем еще фильтровать.</returns>
    public async Task<IOrderedQueryable<CatalogProjectOutput>> GetFiltersProjectsAsync()
    {
        var result = (IOrderedQueryable<CatalogProjectOutput>)_pgContext.CatalogProjects
            .Include(p => p.Project)
            .Select(p => new CatalogProjectOutput
            {
                ProjectId = p.Project.ProjectId,
                ProjectName = p.Project.ProjectName,
                DateCreated = p.Project.DateCreated,
                ProjectIcon = p.Project.ProjectIcon,
                ProjectDetails = p.Project.ProjectDetails,
                HasVacancies =
                    _pgContext.ProjectVacancies.Any(pv => pv.ProjectId == p.ProjectId), // Если у проекта есть вакансии.
                ProjectStageSysName = (from ps in _pgContext.UserProjectsStages
                        join s in _pgContext.ProjectStages
                            on ps.StageId
                            equals s.StageId
                        select s.StageSysName)
                    .FirstOrDefault()
            })
            .AsQueryable();

        return await Task.FromResult(result);
    }

    /// <summary>
    /// Метод првоеряет владельца проекта.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Признак является ли пользователь владельцем проекта.</returns>
    public async Task<bool> CheckProjectOwnerAsync(long projectId, long userId)
    {
        var result = await _pgContext.UserProjects
            .AnyAsync(p => p.ProjectId == projectId
                           && p.UserId == userId);

        return result;
    }

    /// <summary>
    /// Метод удаляет вакансию проекта.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Признак удаления вакансии проекта.</returns>
    public async Task<bool> DeleteProjectVacancyByIdAsync(long vacancyId, long projectId)
    {
        var vacancy = await _pgContext.ProjectVacancies
            .FirstOrDefaultAsync(v => v.VacancyId == vacancyId
                                      && v.ProjectId == projectId);

        if (vacancy is null)
        {
            return false;
        }

        _pgContext.ProjectVacancies.Remove(vacancy);
        await _pgContext.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Метод удаляет проект.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Признак результата удаления, список вакансий, которые отвязаны от проекта, название проекта.</returns>
    public async Task<(bool Success, List<string> RemovedVacancies, string ProjectName)> DeleteProjectAsync(
        long projectId, long userId)
    {
        var tran = await _pgContext.Database
            .BeginTransactionAsync(IsolationLevel.ReadCommitted);
        
        var result = (Success: false, RemovedVacancies: new List<string>(), ProjectName: string.Empty);

        try
        {
            // Удаляем вакансии проекта.
            var projectVacancies = _pgContext.ProjectVacancies
                .Where(v => v.ProjectId == projectId)
                .AsQueryable();

            if (await projectVacancies.AnyAsync())
            {
                // Записываем названия вакансий, которые будут удалены.
                var removedVacanciesNames = projectVacancies.Select(v => v.UserVacancy.VacancyName);
                result.RemovedVacancies.AddRange(removedVacanciesNames);
                
                _pgContext.ProjectVacancies.RemoveRange(projectVacancies);
            }

            // Удаляем чат диалога и все сообщения.
            var projectDialogs = await _chatRepository.GetDialogsAsync(userId);

            // Если у проекта есть диалоги.
            if (projectDialogs.Any())
            {
                // Перед удалением диалога, сначала смотрим сообщения диалога.
                foreach (var d in projectDialogs)
                {
                    var projectDialogMessages = await _chatRepository.GetDialogMessagesAsync(d.DialogId);

                    // Если есть сообщения, дропаем их.
                    if (projectDialogMessages.Any())
                    {
                        _pgContext.DialogMessages.RemoveRange(projectDialogMessages);

                        // Дропаем участников диалога.
                        var dialogMembers = await _chatRepository.GetDialogMembersByDialogIdAsync(d.DialogId);

                        if (dialogMembers.Any())
                        {
                            _pgContext.DialogMembers.RemoveRange(dialogMembers);
                        }
                    }
                }
            }

            // Смотрим команду проекта.
            var projectTeam = await GetProjectTeamAsync(projectId);

            if (projectTeam is not null)
            {
                // Дропаем участников команды.
                // var projectTeamMembers = await GetProjectTeamMembersAsync(projectTeam.TeamId);
                //
                // if (projectTeamMembers.Any())
                // {
                //     _pgContext.ProjectTeamMembers.RemoveRange(projectTeamMembers);
                // }

                // Дропаем команду проекта.
                _pgContext.ProjectsTeams.Remove(projectTeam);
            }

            // Получаем комментарии проекта.
            var projectComments = await GetProjectCommentsAsync(projectId);

            if (projectComments.Any())
            {
                // Дропаем комментарии проекта из модерации.
                var moderationProjectComments = _pgContext.ProjectCommentsModeration
                    .Where(c => c.ProjectComment.ProjectId == projectId)
                    .AsQueryable();

                if (await moderationProjectComments.AnyAsync())
                {
                    _pgContext.ProjectCommentsModeration.RemoveRange(moderationProjectComments);
                }

                _pgContext.ProjectComments.RemoveRange(projectComments);
            }
            
            // Удаляем проект из каталога.
            var catalogProject = await _pgContext.CatalogProjects
                .FirstOrDefaultAsync(p => p.ProjectId == projectId);

            if (catalogProject is not null)
            {
                _pgContext.CatalogProjects.Remove(catalogProject);   
            }

            // Удаляем проект из статусов.
            var projectStatus = await _pgContext.ProjectStatuses
                .FirstOrDefaultAsync(p => p.ProjectId == projectId);
            
            if (projectStatus is not null)
            {
                _pgContext.ProjectStatuses.Remove(projectStatus);
            }

            // Удаляем проект из модерации.
            var moderationProject = await _pgContext.ModerationProjects
                .FirstOrDefaultAsync(p => p.ProjectId == projectId);
            
            if (moderationProject is not null)
            {
                _pgContext.ModerationProjects.Remove(moderationProject);
            }
            
            // Удаляем проект из стадий.
            var projectStage = await _pgContext.UserProjectsStages
                .FirstOrDefaultAsync(p => p.ProjectId == projectId);

            if (projectStage is not null)
            {
                _pgContext.UserProjectsStages.Remove(projectStage);
            }

            // Удаляем проект пользователя.
            var userProject = await _pgContext.UserProjects
                .FirstOrDefaultAsync(p => p.ProjectId == projectId 
                                          && p.UserId == userId);

            if (userProject is not null)
            {
                // Записываем название проекта, который будет удален.
                result.ProjectName = userProject.ProjectName;
                
                _pgContext.UserProjects.Remove(userProject);
            }

            await _pgContext.SaveChangesAsync();
            await tran.CommitAsync();
        }

        catch
        {
            await tran.RollbackAsync();
            throw;
        }

        result.Success = true;

        return result;
    }

    /// <summary>
    /// Метод получает комментарии проекта.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Список комментариев проекта.</returns>
    private async Task<ICollection<ProjectCommentEntity>> GetProjectCommentsAsync(long projectId)
    {
        var result = await _pgContext.ProjectComments
            .Where(c => c.ProjectId == projectId)
            .ToListAsync();

        return result;
    }

    /// <summary>
    /// Метод получает название проекта по его Id.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Название проекта.</returns>
    public async Task<string> GetProjectNameByProjectIdAsync(long projectId)
    {
        var result = await _pgContext.CatalogProjects
            .Include(p => p.Project)
            .Where(p => p.ProjectId == projectId)
            .Select(p => p.Project.ProjectName)
            .FirstOrDefaultAsync();

        return result;
    }

    /// <summary>
    /// Метод проверяет, находится ли проект на модерации.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Признак модерации.</returns>
    public async Task<bool> CheckProjectModerationAsync(long projectId)
    {
        var result = await _pgContext.ModerationProjects
            .AnyAsync(p => p.ProjectId == projectId
                           && p.ModerationStatusId == (int)ProjectModerationStatusEnum.ModerationProject);

        return result;
    }

    /// <summary>
    /// Метод получает список вакансий доступных к отклику.
    /// Для владельца проекта будет возвращаться пустой список.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Список вакансий доступных к отклику.</returns>
    public async Task<IEnumerable<ProjectVacancyEntity>> GetAvailableResponseProjectVacanciesAsync(long userId,
        long projectId)
    {
        // Получаем Id вакансий, которые еще на модерации либо отклонены модератором, так как их нельзя показывать.
        var moderationVacanciesIds = _pgContext.ModerationVacancies
            .Where(v => _excludedVacanciesStatuses.Contains(v.ModerationStatusId))
            .Select(v => v.VacancyId)
            .AsQueryable();

        // Получаем вакансии, на которые можно отправить отклики.
        var result = await _pgContext.ProjectVacancies
            .Where(v => v.ProjectId == projectId 
                        && v.UserVacancy.UserId == userId
                        && !moderationVacanciesIds.Contains(v.VacancyId))
            .Select(v => new ProjectVacancyEntity
            {
                ProjectId = projectId,
                VacancyId = v.VacancyId,
                UserVacancy = new UserVacancyEntity
                {
                    VacancyName = v.UserVacancy.VacancyName,
                    VacancyText = v.UserVacancy.VacancyText,
                    Employment = v.UserVacancy.Employment,
                    WorkExperience = v.UserVacancy.WorkExperience,
                    DateCreated = v.UserVacancy.DateCreated,
                    Payment = v.UserVacancy.Payment,
                    UserId = userId,
                    VacancyId = v.VacancyId,
                }
            })
            .OrderBy(o => o.VacancyId)
            .ToListAsync();

        return result;
    }

    /// <summary>
    /// Метод получает название вакансии проекта по ее Id.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <returns>Название вакансии.</returns>
    public async Task<string> GetProjectVacancyNameByIdAsync(long vacancyId)
    {
        var result = await _pgContext.UserVacancies
            .Where(v => v.VacancyId == vacancyId)
            .Select(v => v.VacancyName)
            .FirstOrDefaultAsync();

        return result;
    }

    /// <summary>
    /// Метод находит почту владельца проекта по Id проекта.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Почта владельца проекта.</returns>
    public async Task<string> GetProjectOwnerEmailByProjectIdAsync(long projectId)
    {
        var ownerId = await _pgContext.CatalogProjects
            .Where(v => v.ProjectId == projectId)
            .Select(v => v.Project.UserId)
            .FirstOrDefaultAsync();

        var result = await _pgContext.Users
            .Where(u => u.UserId == ownerId)
            .Select(u => u.Email)
            .FirstOrDefaultAsync();

        return result;
    }

    /// <summary>
    /// Метод проверяет добавляли ли уже пользоваетля в команду проекта.
    /// Если да, то не даем добавить повторно, чтобы не было дублей.
    /// </summary>
    /// <param name="teamId">Id команды проекта.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Признак проверки.</returns>
    public async Task<bool> CheckProjectTeamMemberAsync(long teamId, long userId)
    {
        var result = await _pgContext.ProjectTeamMembers.AnyAsync(m => m.UserId == userId
                                                                       && m.TeamId == teamId);

        return result;
    }

    /// <summary>
    /// Метод получает список проектов пользователя из архива.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Список архивированных проектов.</returns>
    public async Task<IEnumerable<ArchivedProjectEntity>> GetUserProjectsArchiveAsync(long userId)
    {
        var result = await _pgContext.ArchivedProjects
            .Include(a => a.UserProject)
            .Where(a => a.UserId == userId)
            .ToListAsync();

        return result;
    }

    /// <summary>
    /// Метод получает Id проекта по Id вакансии, которая принадлежит этому проекту.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <returns>Id проекта.</returns>
    public async Task<long> GetProjectIdByVacancyIdAsync(long vacancyId)
    {
        var userId = await _pgContext.UserVacancies
            .Where(v => v.VacancyId == vacancyId)
            .Select(v => v.UserId)
            .FirstOrDefaultAsync();

        var result = await _pgContext.UserProjects
            .Where(p => p.UserId == userId)
            .Select(p => p.ProjectId)
            .FirstOrDefaultAsync();

        return result;
    }

    /// <summary>
    /// Метод получает название проекта по его Id.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Название проекта.</returns>
    public async Task<string> GetProjectNameByIdAsync(long projectId)
    {
        var result = await _pgContext.UserProjects
            .Where(p => p.ProjectId == projectId)
            .Select(p => p.ProjectName)
            .FirstOrDefaultAsync();

        return result;
    }

    /// <summary>
    /// Метод удаляет участника проекта из команды.
    /// </summary>
    /// <param name="userId">Id пользователя</param>
    /// <param name="projectTeamId">Id команды проекта.</param>
    public async Task DeleteProjectTeamMemberAsync(long userId, long projectTeamId)
    {
        await DeleteTeamMemberAsync(userId, projectTeamId);
    }

    /// <summary>
    /// Метод покидания команды проекта.
    /// </summary>
    /// <param name="userId">Id пользователя</param>
    /// <param name="projectTeamId">Id команды проекта.</param>
    public async Task LeaveProjectTeamAsync(long userId, long projectTeamId)
    {
        await DeleteTeamMemberAsync(userId, projectTeamId);
    }

    /// <summary>
    /// Метод проверяет, есть ли пользователь в команде проекта.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Признак проверки.</returns>
    public async Task<bool> CheckExistsProjectTeamMemberAsync(long projectId, long userId)
    {
        var result = await _pgContext.ProjectTeamMembers
            .AnyAsync(m => m.ProjectTeam.ProjectId == projectId && m.UserId == userId);

        return result;
    }

    /// <summary>
    /// Метод проверяет, отклонён ли проект.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Отклонён ли проект.</returns>
    public async Task<bool> CheckProjectRejectedAsync(long projectId)
    {
        var result = await _pgContext.ModerationProjects
            .AnyAsync(p => p.ProjectId == projectId
                           && p.ModerationStatusId == (int)ProjectModerationStatusEnum.RejectedProject);

        return result;
    }

    #region Приватные методы.

    /// <summary>
    /// Метод првоеряет, был ли уже такой проект на модерации. 
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Признак модерации.</returns>
    private async Task<bool> IsModerationExistsProjectAsync(long projectId)
    {
        var result = await _pgContext.ModerationProjects
            .AnyAsync(p => p.ProjectId == projectId);

        return result;
    }

    /// <summary>
    /// Метод обновляет статус проекта на модерации.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="status">Статус проекта.</param>
    private async Task UpdateModerationProjectStatusAsync(long projectId, ProjectModerationStatusEnum status)
    {
        var prj = await _pgContext.ModerationProjects.FirstOrDefaultAsync(p => p.ProjectId == projectId);

        if (prj is null)
        {
            throw new InvalidOperationException($"Не найден проект для модерации. ProjectId: {projectId}");
        }
        
        prj.ModerationStatusId = (int)status;
    }

    /// <summary>
    /// Метод удаляет участника проекта.
    /// </summary>
    /// <param name="userId">Id пользователя</param>
    /// <param name="projectTeamId">Id команды проекта.</param>
    private async Task DeleteTeamMemberAsync(long userId, long projectTeamId)
    {
        var teamMember = await _pgContext.ProjectTeamMembers
            .FirstOrDefaultAsync(m => m.UserId == userId && m.TeamId == projectTeamId);

        // Удаляем участника команды.
        if (teamMember is not null)
        {
            _pgContext.ProjectTeamMembers.Remove(teamMember);
        }
        
        var projectTeamVacancy = await _pgContext.ProjectsTeamsVacancies
            .FirstOrDefaultAsync(v => v.VacancyId == teamMember.VacancyId);

        // Удаляем вакансию из ProjectsTeamsVacancies.
        if (projectTeamVacancy is not null)
        {
            _pgContext.ProjectsTeamsVacancies.Remove(projectTeamVacancy);
        }

        await _pgContext.SaveChangesAsync();
    }

    #endregion
}