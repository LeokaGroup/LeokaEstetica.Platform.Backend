using Autofac;
using AutoMapper;
using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Core.Utils;
using LeokaEstetica.Platform.Database.Repositories.Chat;
using LeokaEstetica.Platform.Database.Repositories.Moderation.Vacancy;
using LeokaEstetica.Platform.Database.Repositories.Profile;
using LeokaEstetica.Platform.Database.Repositories.Project;
using LeokaEstetica.Platform.Database.Repositories.User;
using LeokaEstetica.Platform.Database.Repositories.Vacancy;
using LeokaEstetica.Platform.Logs.Services;
using LeokaEstetica.Platform.Messaging.Services.Chat;
using LeokaEstetica.Platform.Messaging.Services.Mail;
using LeokaEstetica.Platform.Moderation.Services.Vacancy;
using LeokaEstetica.Platform.Notifications.Services;
using LeokaEstetica.Platform.Services.Services.Profile;
using LeokaEstetica.Platform.Services.Services.Project;
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
    protected MailingsService MailingsService;
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
        MailingsService = new MailingsService(AppConfiguration);
        ProfileRepository = new ProfileRepository(PgContext);
        UserService = new UserService(LogService, UserRepository, mapper, null, PgContext, ProfileRepository);
        ProfileService = new ProfileService(LogService, ProfileRepository, UserRepository, mapper, null, null);
        ProjectRepository = new ProjectRepository(PgContext);
        ProjectNotificationsService = new ProjectNotificationsService(null);
        VacancyRepository = new VacancyRepository(PgContext);
        VacancyModerationRepository = new VacancyModerationRepository(PgContext);
        VacancyModerationService = new VacancyModerationService(VacancyModerationRepository, LogService);
        VacancyService = new VacancyService(LogService, VacancyRepository, mapper, null, UserRepository,
            VacancyModerationService, null);
        ProjectService = new ProjectService(ProjectRepository, LogService, UserRepository, mapper,
            ProjectNotificationsService, VacancyService);
        ChatRepository = new ChatRepository(PgContext);
        ChatService = new ChatService(LogService, UserRepository, ProjectRepository, VacancyRepository, ChatRepository,
            mapper);
    }
}