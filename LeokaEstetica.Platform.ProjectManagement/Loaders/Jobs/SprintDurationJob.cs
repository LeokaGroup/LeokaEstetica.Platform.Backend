using Dapper;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Database.Abstractions.Config;
using LeokaEstetica.Platform.Database.Abstractions.ProjectManagment;
using LeokaEstetica.Platform.Integrations.Abstractions.Discord;
using Quartz;

namespace LeokaEstetica.Platform.ProjectManagement.Loaders.Jobs;

/// <summary>
/// Класс джобы отслеживания дат спринтов.
/// Завершает спринты, у которых заканчивается дата окончания.
/// </summary>
[DisallowConcurrentExecution]
internal sealed class SprintDurationJob : IJob
{
    private readonly IDiscordService _discordService;
    private readonly ILogger<SprintDurationJob>? _logger;
    private readonly IGlobalConfigRepository _globalConfigRepository;
    private readonly ISprintRepository _sprintRepository;
    private readonly IProjectManagementSettingsRepository _projectManagementSettingsRepository;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="discordService">Сервис уведомлений дискорда.</param>
    /// <param name="logger">Логгер.</param>
    /// <param name="globalConfigRepository">Репозиторий глобал конфигов.</param>
    /// <param name="sprintRepository">Репозиторий спринтов.</param>
    /// <param name="projectManagementSettingsRepository">Репозиторий настроек проектов.</param>
    public SprintDurationJob(IDiscordService discordService,
        ILogger<SprintDurationJob>? logger,
        IGlobalConfigRepository globalConfigRepository,
        ISprintRepository sprintRepository,
        IProjectManagementSettingsRepository projectManagementSettingsRepository)
    {
        _discordService = discordService;
        _logger = logger;
        _globalConfigRepository = globalConfigRepository;
        _sprintRepository = sprintRepository;
        _projectManagementSettingsRepository = projectManagementSettingsRepository;
    }

    #region Публичные методы.

    /// <summary>
    /// Метод выполняет работу джобы.
    /// </summary>
    public async Task Execute(IJobExecutionContext context)
    {
        var isEnabledJob = await _globalConfigRepository
            .GetValueByKeyAsync<bool>(GlobalConfigKeys.JobsMode.PROJECT_SPRINT_DURATION_JOB_MODE_ENABLED);

        if (!isEnabledJob)
        {
            return;
        }
        
        await CheckProjectSprintDatesAsync();
        
        await Task.CompletedTask;
    }

    #endregion

    #region Приватные методы.

    /// <summary>
    /// Метод выполняет работу джобы.
    /// </summary>
    private async Task CheckProjectSprintDatesAsync()
    {
        try
        {
            _logger?.LogInformation("Начинаем проверять активные спринты проектов...");

            var sprints = (await _sprintRepository.GetSprintEndDatesAsync())?.AsList();

            // Спринтов, которые нужно завершить пока нету.
            if (sprints is null || sprints.Count == 0)
            {
                return;
            }
            
            foreach (var s in sprints)
            {
                // Находит нерешенные задачи спринта, если они есть.
                // Если у спринта нету нерешенных задач, то просто его завершает,
                // иначе еще перемещает нерешенные задачи в соответствии с настройками перемещения.
                var notCompletedSprintTasks = (await _sprintRepository.GetNotCompletedSprintTasksAsync(
                    s.ProjectSprintId, s.ProjectId))?.AsList();

                if (notCompletedSprintTasks is not null && notCompletedSprintTasks.Count > 0)
                {
                    // Находит настройки проекта для перемещения нерешенных задач.
                    var projectSettings = (await _projectManagementSettingsRepository
                        .GetProjectSprintsMoveNotCompletedTasksSettingsAsync(s.ProjectId))?.AsList();

                    if (projectSettings is null || projectSettings.Count == 0)
                    {
                        throw new InvalidOperationException(
                            "Не удалось получить настройки перемещения нерешенных задач спринта проекта. " +
                            $"Id спринта был: {s.SprintId}.");
                    }

                    var sysName = projectSettings.FirstOrDefault(x => x.Selected)?.SysName;

                    if (sysName is null)
                    {
                        throw new InvalidOperationException("Не удалось получить системное название настройки.");
                    }

                    // Если в настройках проекта выбрана опция в след.спринт,
                    // то проверяем, есть ли след.спринт за текущим.
                    if (sysName.Equals("NextSprint"))
                    {
                        var nextProjectSprintId = await _sprintRepository.GetNextSprintAsync(s.ProjectSprintId,
                            s.ProjectId);

                        if (!nextProjectSprintId.HasValue)
                        {
                            // Фиксируем такое, но не ломаем приложение.
                            var ex = new InvalidOperationException(
                                "Была выбрана настройка перемещения в след.спринт, но у проекта нет следующего" +
                                " спринта за активным. " +
                                $"ProjectSprintId: {s.ProjectSprintId}. " +
                                $"ProjectId: {s.ProjectId}.");
                            
                            _logger?.LogError(ex, ex.Message);
                            
                            await _discordService.SendNotificationErrorAsync(ex);
                            
                            // Тогда нерешенные задачи остаются в бэклоге.
                            continue;
                        }
                        
                        // Перемещаем нерешенные задачи спринта в след.спринт.
                        await _sprintRepository.MoveNotCompletedSprintTasksToNextSprintAsync(notCompletedSprintTasks,
                            nextProjectSprintId.Value);
                    }
                }

                // Завершает спринт.
                await _sprintRepository.AutoCompleteSprintAsync(s.ProjectSprintId, s.ProjectId);
            }

            _logger?.LogInformation("Закончили проверять активные спринты проектов...");
        }
        
        catch (Exception ex)
        {
            _logger?.LogError(ex, ex.Message);
            
            await _discordService.SendNotificationErrorAsync(ex);
            
            throw;
        }
    }

    #endregion
}