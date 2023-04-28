using Autofac;
using AutoMapper;
using LeokaEstetica.Platform.Access.Services.AvailableLimits;
using LeokaEstetica.Platform.Access.Services.Moderation;
using LeokaEstetica.Platform.Access.Services.User;
using LeokaEstetica.Platform.CallCenter.Services.Project;
using LeokaEstetica.Platform.CallCenter.Services.Resume;
using LeokaEstetica.Platform.CallCenter.Services.Vacancy;
using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Core.Utils;
using LeokaEstetica.Platform.Database.Repositories.Access.User;
using LeokaEstetica.Platform.Database.Repositories.AvailableLimits;
using LeokaEstetica.Platform.Database.Repositories.Chat;
using LeokaEstetica.Platform.Database.Repositories.Commerce;
using LeokaEstetica.Platform.Database.Repositories.FareRule;
using LeokaEstetica.Platform.Database.Repositories.Knowledge;
using LeokaEstetica.Platform.Database.Repositories.Landing;
using LeokaEstetica.Platform.Database.Repositories.Moderation.Access;
using LeokaEstetica.Platform.Database.Repositories.Moderation.Project;
using LeokaEstetica.Platform.Database.Repositories.Moderation.Resume;
using LeokaEstetica.Platform.Database.Repositories.Moderation.Vacancy;
using LeokaEstetica.Platform.Database.Repositories.Notification;
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
using LeokaEstetica.Platform.Notifications.Services;
using LeokaEstetica.Platform.Processing.Services.PayMaster;
using LeokaEstetica.Platform.Redis.Services.User;
using LeokaEstetica.Platform.Services.Services.FareRule;
using LeokaEstetica.Platform.Services.Services.Knowledge;
using LeokaEstetica.Platform.Services.Services.Landing;
using LeokaEstetica.Platform.Services.Services.Profile;
using LeokaEstetica.Platform.Services.Services.Project;
using LeokaEstetica.Platform.Services.Services.Resume;
using LeokaEstetica.Platform.Services.Services.Subscription;
using LeokaEstetica.Platform.Services.Services.User;
using LeokaEstetica.Platform.Services.Services.Vacancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
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
    protected readonly UserBlackListService UserBlackListService;
    protected readonly ResumeModerationService ResumeModerationService;
    protected readonly LandingService LandingService;
    protected readonly KnowledgeService KnowledgeService;
    protected readonly PgContext PgContext;
    protected readonly FillColorProjectsService FillColorProjectsService;
    protected readonly FillColorResumeService FillColorResumeService;

    protected BaseServiceTest()
    {
        // Настройка тестовых строк подключения.
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");
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
        var subscriptionRepository = new SubscriptionRepository(pgContext);
        var chatRepository = new ChatRepository(pgContext);
        var resumeModerationRepository = new ResumeModerationRepository(pgContext);
        var accessUserRepository = new AccessUserRepository(pgContext);
        var accessUserService = new AccessUserService(accessUserRepository);
        var optionsForCache = new OptionsWrapper<MemoryDistributedCacheOptions>(new MemoryDistributedCacheOptions());
        var distributedCache = new MemoryDistributedCache(optionsForCache);
        var userRedisService = new UserRedisService(distributedCache, mapper);

        UserService = new UserService(logService, userRepository, mapper, null, pgContext, profileRepository,
            subscriptionRepository, resumeModerationRepository, accessUserService, userRedisService);
        ProfileService = new ProfileService(logService, profileRepository, userRepository, mapper, null, null);

        var projectRepository = new ProjectRepository(pgContext, chatRepository);
        var projectNotificationsRepository = new ProjectNotificationsRepository(pgContext);
        var vacancyRepository = new VacancyRepository(pgContext);
        var projectNotificationsService = new ProjectNotificationsService(null, logService, userRepository, mapper,
            projectNotificationsRepository, null, projectRepository, null, null, vacancyRepository);
        var vacancyModerationRepository = new VacancyModerationRepository(pgContext);
        var vacancyNotificationsService = new VacancyNotificationsService(null, null);
        var fareRuleRepository = new FareRuleRepository(pgContext);
        var availableLimitsRepository = new AvailableLimitsRepository(pgContext);
        var availableLimitsService = new AvailableLimitsService(logService, availableLimitsRepository);

        VacancyModerationService = new VacancyModerationService(vacancyModerationRepository, logService, mapper, null,
            vacancyRepository, userRepository, projectRepository);
        
        // Тут если нужен будет ProjectService, то тут проблема с порядком следования.
        // Не получится сделать просто, VacancyService и ProjectService нужны друг другу тесно.
        VacancyService = new VacancyService(logService, vacancyRepository, mapper, null, userRepository,
            VacancyModerationService, subscriptionRepository, fareRuleRepository, availableLimitsService,
            vacancyNotificationsService, null, null, null);

        ChatService = new ChatService(logService, userRepository, projectRepository, vacancyRepository, chatRepository,
            mapper);

        var accessModerationRepository = new AccessModerationRepository(pgContext);

        AccessModerationService = new AccessModerationService(logService, accessModerationRepository, userRepository);

        var projectModerationRepository = new ProjectModerationRepository(pgContext);

        ProjectModerationService = new ProjectModerationService(projectModerationRepository, logService, mapper, null, 
            userRepository, projectRepository, null);

        var projectCommentsRepository = new ProjectCommentsRepository(pgContext);

        ProjectCommentsService = new ProjectCommentsService(logService, userRepository, projectCommentsRepository, null, null,
            null);
        ProjectFinderService = new ProjectFinderService(logService, userRepository, projectNotificationsService);

        var resumeRepository = new ResumeRepository(pgContext);

        FillColorProjectsService = new FillColorProjectsService();

        ProjectService = new ProjectService(projectRepository, logService, userRepository, mapper,
            projectNotificationsService, VacancyService, vacancyRepository, availableLimitsService,
            subscriptionRepository, fareRuleRepository, VacancyModerationService, projectNotificationsRepository, null,
            null, FillColorProjectsService, null, projectModerationRepository);

        SubscriptionService =
            new SubscriptionService(logService, userRepository, subscriptionRepository, fareRuleRepository);
        FillColorResumeService = new FillColorResumeService();
        ResumeService = new ResumeService(logService, resumeRepository, mapper, subscriptionRepository,
            fareRuleRepository, userRepository, FillColorResumeService);
        VacancyFinderService = new VacancyFinderService(vacancyRepository, logService);
        FinderProjectService = new Finder.Services.Project.ProjectFinderService(projectRepository, logService);
        ResumeFinderService = new ResumeFinderService(logService, resumeRepository);
        VacancyPaginationService = new VacancyPaginationService(vacancyRepository, logService);
        ProjectPaginationService = new ProjectPaginationService(projectRepository, logService);

        var payMasterRepository = new PayMasterRepository(pgContext);

        FareRuleService = new FareRuleService(fareRuleRepository, logService);
        PayMasterService = new PayMasterService(logService, AppConfiguration, fareRuleRepository, userRepository,
            payMasterRepository, null, null);

        var userBlackListService = new UserBlackListRepository(pgContext);
        UserBlackListService = new UserBlackListService(logService, userBlackListService);
        ResumeModerationService =
            new ResumeModerationService(logService, resumeModerationRepository, mapper, userRepository);

        var landingRepository = new LandingRepository(pgContext);
        LandingService = new LandingService(logService, landingRepository, mapper);

        var KnowledgeRepository = new KnowledgeRepository(pgContext);
        KnowledgeService = new KnowledgeService(KnowledgeRepository, logService);

        PgContext = pgContext;

        
    }
}