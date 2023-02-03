using Autofac;
using AutoMapper;
using LeokaEstetica.Platform.Access.Services.Moderation;
using LeokaEstetica.Platform.Access.Services.Resume;
using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Core.Utils;
using LeokaEstetica.Platform.Database.Repositories.Chat;
using LeokaEstetica.Platform.Database.Repositories.Commerce;
using LeokaEstetica.Platform.Database.Repositories.FareRule;
using LeokaEstetica.Platform.Database.Repositories.Moderation.Access;
using LeokaEstetica.Platform.Database.Repositories.Moderation.Project;
using LeokaEstetica.Platform.Database.Repositories.Moderation.Vacancy;
using LeokaEstetica.Platform.Database.Repositories.Profile;
using LeokaEstetica.Platform.Database.Repositories.Project;
using LeokaEstetica.Platform.Database.Repositories.Resume;
using LeokaEstetica.Platform.Database.Repositories.Subscription;
using LeokaEstetica.Platform.Database.Repositories.User;
using LeokaEstetica.Platform.Database.Repositories.Vacancy;
using LeokaEstetica.Platform.Finder.Services.Project;
using LeokaEstetica.Platform.Finder.Services.Resume;
using LeokaEstetica.Platform.Finder.Services.Vacancy;
using LeokaEstetica.Platform.Logs.Services;
using LeokaEstetica.Platform.Messaging.Services.Chat;
using LeokaEstetica.Platform.Messaging.Services.Project;
using LeokaEstetica.Platform.Moderation.Services.Project;
using LeokaEstetica.Platform.Moderation.Services.Vacancy;
using LeokaEstetica.Platform.Notifications.Services;
using LeokaEstetica.Platform.Processing.Services.PayMaster;
using LeokaEstetica.Platform.Services.Services.FareRule;
using LeokaEstetica.Platform.Services.Services.Profile;
using LeokaEstetica.Platform.Services.Services.Project;
using LeokaEstetica.Platform.Services.Services.Resume;
using LeokaEstetica.Platform.Services.Services.Subscription;
using LeokaEstetica.Platform.Services.Services.User;
using LeokaEstetica.Platform.Services.Services.Vacancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ProjectFinderService = LeokaEstetica.Platform.Services.Services.Search.Project.ProjectFinderService;

namespace LeokaEstetica.Platform.Tests;

/// <summary>
/// Базовый класс тестов с настройками конфигурации.
/// </summary>
public class BaseServiceTest
{
    private IConfiguration AppConfiguration { get; }
    private string PostgreConfigString { get; set; }
    protected readonly UserService UserService;
    protected readonly ProfileService ProfileService;
    protected readonly ProjectService ProjectService;
    protected readonly VacancyService VacancyService;
    protected readonly VacancyModerationService VacancyModerationService;
    protected readonly ChatService ChatService;
    protected readonly AccessModerationService AccessModerationService;
    protected readonly ProjectModerationService ProjectModerationService;
    protected readonly ProjectCommentsService ProjectCommentsService;
    protected readonly ProjectFinderService ProjectFinderService;
    protected readonly ResumeService ResumeService;
    protected readonly VacancyFinderService VacancyFinderService;
    protected readonly Finder.Services.Project.ProjectFinderService FinderProjectService;
    protected readonly ResumeFinderService ResumeFinderService;
    protected readonly VacancyPaginationService VacancyPaginationService;
    protected readonly ProjectPaginationService ProjectPaginationService;
    protected readonly FareRuleService FareRuleService;
    protected readonly PayMasterService PayMasterService;
    protected readonly SubscriptionService SubscriptionService;
    protected readonly AccessResumeService AccessResumeService;

    protected BaseServiceTest()
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
        var pgContext = new PgContext(optionsBuilder.Options);
        var logService = new LogService(pgContext);
        var userRepository = new UserRepository(pgContext, logService);
        var profileRepository = new ProfileRepository(pgContext);

        UserService = new UserService(logService, userRepository, mapper, null, pgContext, profileRepository);
        ProfileService = new ProfileService(logService, profileRepository, userRepository, mapper, null, null);

        var projectRepository = new ProjectRepository(pgContext);
        var projectNotificationsService = new ProjectNotificationsService(null);
        var vacancyRepository = new VacancyRepository(pgContext);
        var vacancyModerationRepository = new VacancyModerationRepository(pgContext);

        VacancyModerationService = new VacancyModerationService(vacancyModerationRepository, logService, mapper);
        VacancyService = new VacancyService(logService, vacancyRepository, mapper, null, userRepository,
            VacancyModerationService, null);
        ProjectService = new ProjectService(projectRepository, logService, userRepository, mapper,
            projectNotificationsService, VacancyService, vacancyRepository);

        var chatRepository = new ChatRepository(pgContext);

        ChatService = new ChatService(logService, userRepository, projectRepository, vacancyRepository, chatRepository,
            mapper);

        var accessModerationRepository = new AccessModerationRepository(pgContext);

        AccessModerationService = new AccessModerationService(logService, accessModerationRepository, userRepository);

        var projectModerationRepository = new ProjectModerationRepository(pgContext);

        ProjectModerationService = new ProjectModerationService(projectModerationRepository, logService, mapper);

        var projectCommentsRepository = new ProjectCommentsRepository(pgContext);

        ProjectCommentsService = new ProjectCommentsService(logService, userRepository, projectCommentsRepository);
        ProjectFinderService = new ProjectFinderService(logService, userRepository, projectNotificationsService);

        var resumeRepository = new ResumeRepository(pgContext);
        var subscriptionRepository = new SubscriptionRepository(pgContext);
        var fareRuleRepository = new FareRuleRepository(pgContext);

        SubscriptionService =
            new SubscriptionService(logService, userRepository, subscriptionRepository, fareRuleRepository);
        AccessResumeService = new AccessResumeService(logService, subscriptionRepository, userRepository);
        ResumeService = new ResumeService(logService, resumeRepository, userRepository);
        VacancyFinderService = new VacancyFinderService(vacancyRepository, logService);
        FinderProjectService = new Finder.Services.Project.ProjectFinderService(projectRepository, logService);
        ResumeFinderService = new ResumeFinderService(logService, resumeRepository);
        VacancyPaginationService = new VacancyPaginationService(vacancyRepository, logService);
        ProjectPaginationService = new ProjectPaginationService(projectRepository, logService);
        
        var payMasterRepository = new PayMasterRepository(pgContext);

        FareRuleService = new FareRuleService(fareRuleRepository, logService);
        PayMasterService = new PayMasterService(logService, AppConfiguration, fareRuleRepository, userRepository,
            payMasterRepository);
    }
}