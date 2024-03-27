using AutoMapper;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.User;
using LeokaEstetica.Platform.Database.Abstractions.ProjectManagment;
using LeokaEstetica.Platform.Database.Abstractions.Template;
using LeokaEstetica.Platform.Integrations.Abstractions.Pachca;
using LeokaEstetica.Platform.Models.Dto.Output.Template;
using LeokaEstetica.Platform.Services.Abstractions.User;

namespace LeokaEstetica.Platform.Services.Builders.AgileObjectBuilder;

/// <summary>
/// Абстрактный класс строителя объекта Agile.
/// Объект Agile - это может быть задача, эпик, история, ошибка. Смотря что будет создаваться строителем.
/// </summary>
public abstract class AgileObjectBuilder
{
    protected readonly IProjectManagmentRepository ProjectManagmentRepository;
    protected readonly IUserRepository UserRepository;
    protected readonly IPachcaService PachcaService;
    protected readonly IUserService UserService;
    protected readonly IProjectManagmentTemplateRepository ProjectManagmentTemplateRepository;
    protected long ProjectTaskId;
    protected long ProjectId;
    protected readonly IMapper Mapper;
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="projectManagmentRepository">Репозиторий модуля УП.</param>
    /// <param name="userRepository">Репозиторий пользователей.</param>
    /// <param name="pachcaService">Сервис пачки.</param>
    /// <param name="userService">Сервис пользователей.</param>
    /// <param name="projectManagmentTemplateRepository">Репозиторий шаблонов модуля УП.</param>
    /// <param name="mapper">Маппер.</param>
    protected AgileObjectBuilder(IProjectManagmentRepository projectManagmentRepository,
        IUserRepository userRepository,
        IPachcaService pachcaService,
        IUserService userService,
        IProjectManagmentTemplateRepository projectManagmentTemplateRepository,
        IMapper mapper)
    {
        ProjectManagmentRepository = projectManagmentRepository;
        UserRepository = userRepository;
        PachcaService = pachcaService;
        UserService = userService;
        ProjectManagmentTemplateRepository = projectManagmentTemplateRepository;
        Mapper = mapper;
    }

    /// <summary>
    /// Построенная модель. Является результатом. Это может быть задача, эпик, спринт, ошибка.
    /// </summary>
    public ProjectManagmentTaskOutput ProjectManagmentTask { get; set; }

    /// <summary>
    /// Метод создает результирующую модель.
    /// Наполнение строителями сразу не происходит. Она создается пустой.
    /// </summary>
    public Task CreateProjectManagmentTaskAsync(long projectTaskId, long projectId)
    {
        ProjectManagmentTask = new ProjectManagmentTaskOutput
        {
            Executor = new Executor()
        };

        ProjectTaskId = projectTaskId;
        ProjectId = projectId;

        return Task.FromResult(ProjectManagmentTask);
    }
    
    /// <summary>
    /// Метод получает данные из БД. Эти данные нужны для работы строителя.
    /// </summary>
    /// <param name="projectTaskId">Id задачи в рамках проекта.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Построенная модель.</returns>
    public abstract Task InitObjectAsync(long projectTaskId, long projectId);
    
    /// <summary>
    /// Метод записывает название автора.
    /// </summary>
    public abstract Task FillAuthorNameAsync();
    
    /// <summary>
    /// Метод записывает название исполнителя.
    /// </summary>
    public abstract Task FillExecutorNameAsync();
    
    /// <summary>
    /// Метод записывает данные аватара.
    /// </summary>
    public abstract Task FillExecutorAvatarAsync();
    
    /// <summary>
    /// Метод записывает названия наблюдателей.
    /// </summary>
    public abstract Task FillWatcherNamesAsync();
    
    /// <summary>
    /// Метод записывает названия тегов.
    /// </summary>
    public abstract Task FillTagIdsAsync();
    
    /// <summary>
    /// Метод записывает название типа.
    /// </summary>
    public abstract Task FillTaskTypeNameAsync();
    
    /// <summary>
    /// Метод записывает название статуса.
    /// </summary>
    public abstract Task FillTaskStatusNameAsync();
    
    /// <summary>
    /// Метод записывает название резолюции.
    /// </summary>
    public abstract Task FillResolutionNameAsync();
    
    /// <summary>
    /// Метод записывает название приоритета.
    /// </summary>
    public abstract Task FillPriorityNameAsync();
    
    /// <summary>
    /// Метод записывает Id эпика и название эпика.
    /// </summary>
    public abstract Task FillEpicIdAndEpicNameAsync();

    /// <summary>
    /// Метод записывает Id спринта и название спринта.
    /// </summary>
    public abstract Task FillSprintIdAndSprintNameAsync();
}