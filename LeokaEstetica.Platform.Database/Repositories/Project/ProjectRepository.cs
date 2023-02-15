using System.Data;
using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Core.Enums;
using LeokaEstetica.Platform.Core.Exceptions;
using LeokaEstetica.Platform.Core.Extensions;
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
            UserProjects = await _pgContext.ProjectStatuses
                .Include(p => p.UserProject)
                .Where(u => u.UserProject.UserId == userId)
                .Select(p => new UserProjectOutput
                {
                    ProjectName = p.UserProject.ProjectName,
                    ProjectDetails = p.UserProject.ProjectDetails,
                    ProjectIcon = p.UserProject.ProjectIcon,
                    ProjectStatusName = p.ProjectStatusName,
                    ProjectStatusSysName = p.ProjectStatusSysName,
                    ProjectCode = p.UserProject.ProjectCode,
                    ProjectId = p.ProjectId
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

            // Отправляем проект на модерацию.
            await SendModerationProjectAsync(project.ProjectId);

            var result = new UpdateProjectOutput
            {
                DateCreated = project.DateCreated,
                ProjectName = project.ProjectName,
                ProjectDetails = project.ProjectDetails,
                ProjectIcon = project.ProjectIcon,
                ProjectId = project.ProjectId
            };
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
    public async Task<UserProjectEntity> GetProjectAsync(long projectId)
    {
        var result = await _pgContext.UserProjects
            .FirstOrDefaultAsync(p => p.ProjectId == projectId);

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

        // Проставляем статус модерации проекта "На модерации".
        await _pgContext.ModerationStatuses.AddAsync(new ModerationStatusEntity
        {
            StatusName = ProjectModerationStatusEnum.ModerationProject.GetEnumDescription(),
            StatusSysName = ProjectModerationStatusEnum.ModerationProject.ToString()
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

        // Получаем вакансии, которые можно прикрепить к проекту.
        var result = await _pgContext.UserVacancies
            .Where(v => v.UserId == userId
                        && !attachedVacanciesIds.Contains(v.VacancyId))
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
        var result = await _pgContext.ProjectTeamMembers
            .Include(tm => tm.UserVacancy)
            .Where(tm => tm.TeamId == teamId)
            .Select(tm => new ProjectTeamMemberEntity
            {
                UserId = tm.UserId,
                Joined = tm.Joined,
                UserVacancy = tm.UserVacancy,
                TeamId = tm.TeamId,
                MemberId = tm.MemberId
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
    public async Task<ProjectTeamMemberEntity> AddProjectTeamMemberAsync(long userId, long vacancyId,
        long teamId)
    {
        var result = new ProjectTeamMemberEntity
        {
            Joined = DateTime.Now,
            UserId = userId,
            VacancyId = vacancyId,
            TeamId = teamId
        };
        await _pgContext.ProjectTeamMembers.AddAsync(result);
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
    /// <returns>Признак результата удаления.</returns>
    public async Task<bool> DeleteProjectAsync(long projectId, long userId)
    {
        var tran = await _pgContext.Database
            .BeginTransactionAsync(IsolationLevel.ReadCommitted);

        try
        {
            // Удаляем вакансии проекта.
            var projectVacancies = _pgContext.ProjectVacancies
                .Where(v => v.ProjectId == projectId)
                .AsQueryable();

            if (await projectVacancies.AnyAsync())
            {
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
                var projectTeamMembers = await GetProjectTeamMembersAsync(projectTeam.TeamId);

                if (projectTeamMembers.Any())
                {
                    _pgContext.ProjectTeamMembers.RemoveRange(projectTeamMembers);
                }

                // Дропаем команду проекта.
                _pgContext.ProjectsTeams.Remove(projectTeam);
            }

            // Дропаем комментарии проекта.
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

        return true;
    }

    /// <summary>
    /// Метод удаляет комментарии проекта.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Список комментариев проекта.</returns>
    public async Task<ICollection<ProjectCommentEntity>> GetProjectCommentsAsync(long projectId)
    {
        var result = await _pgContext.ProjectComments
            .Where(c => c.ProjectId == projectId)
            .ToListAsync();

        return result;
    }
}