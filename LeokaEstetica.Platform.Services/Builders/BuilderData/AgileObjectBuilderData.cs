using AutoMapper;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.User;
using LeokaEstetica.Platform.Database.Abstractions.Config;
using LeokaEstetica.Platform.Database.Abstractions.ProjectManagment;
using LeokaEstetica.Platform.Database.Abstractions.Template;
using LeokaEstetica.Platform.Integrations.Abstractions.Discord;
using LeokaEstetica.Platform.Services.Abstractions.User;

namespace LeokaEstetica.Platform.Services.Builders.BuilderData;

/// <summary>
/// Класс для данных, необходимых для строителей. Зависимости и тд.
/// </summary>
internal class AgileObjectBuilderData
{
    internal IProjectManagmentRepository ProjectManagmentRepository;
    internal IUserRepository UserRepository;
    internal IDiscordService DiscordService;
    internal IUserService UserService;
    internal IProjectManagmentTemplateRepository ProjectManagmentTemplateRepository;
    internal IMapper Mapper;
    internal IProjectSettingsConfigRepository ProjectSettingsConfigRepository;

    /// <summary>
    /// Id задачи в рамках проекта.
    /// </summary>
    internal long ProjectTaskId;

    /// <summary>
    /// Id проекта.
    /// </summary>
    internal long ProjectId;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="projectManagmentRepository">Репозиторий модуля УП.</param>
    /// <param name="userRepository">Репозиторий пользователей.</param>
    /// <param name="discordService">Сервис дискорда.</param>
    /// <param name="userService">Сервис пользователей.</param>
    /// <param name="projectManagmentTemplateRepository">Репозиторий шаблонов модуля УП.</param>
    /// <param name="projectTaskId">Id задачи в рамках проекта.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="projectManagementSettingsRepository">Репозиторий настроек модуля УП.</param>
    public AgileObjectBuilderData(IProjectManagmentRepository projectManagmentRepository,
        IUserRepository userRepository,
        IDiscordService discordService,
        IUserService userService,
        IProjectManagmentTemplateRepository projectManagmentTemplateRepository,
        IMapper mapper,
        long projectTaskId,
        long projectId,
        IProjectSettingsConfigRepository _projectSettingsConfigRepository)
    {
        ProjectManagmentRepository = projectManagmentRepository;
        UserRepository = userRepository;
        DiscordService = discordService;
        UserService = userService;
        ProjectManagmentTemplateRepository = projectManagmentTemplateRepository;
        Mapper = mapper;
        ProjectTaskId = projectTaskId;
        ProjectId = projectId;
        ProjectSettingsConfigRepository = _projectSettingsConfigRepository;
    }
}