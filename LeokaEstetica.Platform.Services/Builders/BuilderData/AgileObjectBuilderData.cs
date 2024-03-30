using AutoMapper;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.User;
using LeokaEstetica.Platform.Database.Abstractions.ProjectManagment;
using LeokaEstetica.Platform.Database.Abstractions.Template;
using LeokaEstetica.Platform.Integrations.Abstractions.Pachca;
using LeokaEstetica.Platform.Services.Abstractions.User;

namespace LeokaEstetica.Platform.Services.Builders.BuilderData;

/// <summary>
/// Класс для данных, необходимых для строителей. Зависимости и тд.
/// </summary>
internal class AgileObjectBuilderData
{
    internal IProjectManagmentRepository ProjectManagmentRepository;
    internal IUserRepository UserRepository;
    internal IPachcaService PachcaService;
    internal IUserService UserService;
    internal IProjectManagmentTemplateRepository ProjectManagmentTemplateRepository;
    internal IMapper Mapper;

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
    /// <param name="pachcaService">Сервис пачки.</param>
    /// <param name="userService">Сервис пользователей.</param>
    /// <param name="projectManagmentTemplateRepository">Репозиторий шаблонов модуля УП.</param>
    /// <param name="projectTaskId">Id задачи в рамках проекта.</param>
    /// <param name="projectId">Id проекта.</param>
    public AgileObjectBuilderData(IProjectManagmentRepository projectManagmentRepository,
        IUserRepository userRepository,
        IPachcaService pachcaService,
        IUserService userService,
        IProjectManagmentTemplateRepository projectManagmentTemplateRepository,
        IMapper mapper,
        long projectTaskId,
        long projectId)
    {
        ProjectManagmentRepository = projectManagmentRepository;
        UserRepository = userRepository;
        PachcaService = pachcaService;
        UserService = userService;
        ProjectManagmentTemplateRepository = projectManagmentTemplateRepository;
        Mapper = mapper;
        ProjectTaskId = projectTaskId;
        ProjectId = projectId;
    }
}