using Autofac;
using AutoMapper;
using LeokaEstetica.Platform.Access.Services.Moderation;
using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Core.Utils;
using LeokaEstetica.Platform.Database.Repositories.Chat;
using LeokaEstetica.Platform.Database.Repositories.Moderation.Access;
using LeokaEstetica.Platform.Database.Repositories.Moderation.Project;
using LeokaEstetica.Platform.Database.Repositories.Moderation.Vacancy;
using LeokaEstetica.Platform.Database.Repositories.Profile;
using LeokaEstetica.Platform.Database.Repositories.Project;
using LeokaEstetica.Platform.Database.Repositories.Resume;
using LeokaEstetica.Platform.Database.Repositories.User;
using LeokaEstetica.Platform.Database.Repositories.Vacancy;
using LeokaEstetica.Platform.Finder.Services.Resume;
using LeokaEstetica.Platform.Finder.Services.Vacancy;
using LeokaEstetica.Platform.Logs.Services;
using LeokaEstetica.Platform.Messaging.Services.Chat;
using LeokaEstetica.Platform.Messaging.Services.Project;
using LeokaEstetica.Platform.Moderation.Services.Project;
using LeokaEstetica.Platform.Moderation.Services.Vacancy;
using LeokaEstetica.Platform.Notifications.Services;
using LeokaEstetica.Platform.Services.Services.Profile;
using LeokaEstetica.Platform.Services.Services.Project;
using LeokaEstetica.Platform.Services.Services.Resume;
using LeokaEstetica.Platform.Services.Services.Search.Project;
using LeokaEstetica.Platform.Services.Services.User;
using LeokaEstetica.Platform.Services.Services.Vacancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace LeokaEstetica.Platform.Tests;

/// <summary>
/// Базовый класс тестов с настройками конфигурации.
/// </summary>
public class BaseServiceTest
{
    protected string PostgreConfigString { get; set; }
    protected IConfiguration AppConfiguration { get; set; }
    protected PgContext PgContext;
    protected LogService LogService;
    protected UserService UserService;
    protected UserRepository UserRepository;
    protected ProfileRepository ProfileRepository;
    protected ProfileService ProfileService;
    protected ProjectRepository ProjectRepository;
    protected ProjectService ProjectService;
    protected VacancyRepository VacancyRepository;
    protected VacancyService VacancyService;
    protected VacancyModerationService VacancyModerationService;
    protected VacancyModerationRepository VacancyModerationRepository;
    protected ProjectNotificationsService ProjectNotificationsService;
    protected ChatService ChatService;
    protected ChatRepository ChatRepository;
    protected AccessModerationRepository AccessModerationRepository;
    protected AccessModerationService AccessModerationService;
    protected ProjectModerationService ProjectModerationService;
    protected ProjectModerationRepository ProjectModerationRepository;
    protected ProjectCommentsService ProjectCommentsService;
    protected ProjectCommentsRepository ProjectCommentsRepository;
    protected ProjectFinderService ProjectFinderService;
    protected ResumeService ResumeService;
    protected ResumeRepository ResumeRepository;
    protected VacancyFinderService VacancyFinderService;
    protected Finder.Services.Project.ProjectFinderService FinderProjectService;
    protected ResumeFinderService ResumeFinderService;

    public BaseServiceTest()
    {
        // Настройка тестовых строк подключения.
        var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
        AppConfiguration = builder.Build();
        PostgreConfigString = AppConfiguration["ConnectionStrings:NpgDevSqlConnection"] ?? string.Empty;
        var container = new ContainerBuilder();

        AutoFac.RegisterMapper(container);
        var mapper = AutoFac.Resolve<IMapper>();

        // Настройка тестовых контекстов.
        var optionsBuilder = new DbContextOptionsBuilder<PgContext>();
        optionsBuilder.UseNpgsql(PostgreConfigString);
        PgContext = new PgContext(optionsBuilder.Options);

        LogService = new LogService(PgContext);
        UserRepository = new UserRepository(PgContext, LogService);
        ProfileRepository = new ProfileRepository(PgContext);
        UserService = new UserService(LogService, UserRepository, mapper, null, PgContext, ProfileRepository);
        ProfileService = new ProfileService(LogService, ProfileRepository, UserRepository, mapper, null, null);
        ProjectRepository = new ProjectRepository(PgContext);
        ProjectNotificationsService = new ProjectNotificationsService(null);
        VacancyRepository = new VacancyRepository(PgContext);
        VacancyModerationRepository = new VacancyModerationRepository(PgContext);
        VacancyModerationService = new VacancyModerationService(VacancyModerationRepository, LogService, mapper);
        VacancyService = new VacancyService(LogService, VacancyRepository, mapper, null, UserRepository,
            VacancyModerationService, null);
        ProjectService = new ProjectService(ProjectRepository, LogService, UserRepository, mapper,
            ProjectNotificationsService, VacancyService, VacancyRepository);
        ChatRepository = new ChatRepository(PgContext);
        ChatService = new ChatService(LogService, UserRepository, ProjectRepository, VacancyRepository, ChatRepository,
            mapper);
        AccessModerationRepository = new AccessModerationRepository(PgContext);
        AccessModerationService = new AccessModerationService(LogService, AccessModerationRepository, UserRepository);
        ProjectModerationRepository = new ProjectModerationRepository(PgContext);
        ProjectModerationService = new ProjectModerationService(ProjectModerationRepository, LogService, mapper);
        ProjectCommentsRepository = new ProjectCommentsRepository(PgContext);
        ProjectCommentsService = new ProjectCommentsService(LogService, UserRepository, ProjectCommentsRepository);
        ProjectFinderService = new ProjectFinderService(LogService, UserRepository, ProjectNotificationsService);
        ResumeRepository = new ResumeRepository(PgContext);
        ResumeService = new ResumeService(LogService, ResumeRepository);
        VacancyFinderService = new VacancyFinderService(VacancyRepository, LogService);
        FinderProjectService = new Finder.Services.Project.ProjectFinderService(ProjectRepository, LogService);
        ResumeFinderService = new ResumeFinderService(LogService, ResumeRepository);
    }
}