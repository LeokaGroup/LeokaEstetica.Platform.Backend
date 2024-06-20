using Autofac;
using LazyProxy.Autofac;
using LeokaEstetica.Platform.Base.Factors;
using LeokaEstetica.Platform.Core.Attributes;
using LeokaEstetica.Platform.Database.Abstractions.AvailableLimits;
using LeokaEstetica.Platform.Database.Abstractions.Commerce;
using LeokaEstetica.Platform.Database.Abstractions.Config;
using LeokaEstetica.Platform.Database.Abstractions.FareRule;
using LeokaEstetica.Platform.Database.Abstractions.Header;
using LeokaEstetica.Platform.Database.Abstractions.Knowledge;
using LeokaEstetica.Platform.Database.Abstractions.Landing;
using LeokaEstetica.Platform.Database.Abstractions.Metrics;
using LeokaEstetica.Platform.Database.Abstractions.Moderation.Access;
using LeokaEstetica.Platform.Database.Abstractions.Moderation.Project;
using LeokaEstetica.Platform.Database.Abstractions.Moderation.Resume;
using LeokaEstetica.Platform.Database.Abstractions.Moderation.Vacancy;
using LeokaEstetica.Platform.Database.Abstractions.Notification;
using LeokaEstetica.Platform.Database.Abstractions.Orders;
using LeokaEstetica.Platform.Database.Abstractions.Press;
using LeokaEstetica.Platform.Database.Abstractions.Profile;
using LeokaEstetica.Platform.Database.Abstractions.Project;
using LeokaEstetica.Platform.Database.Abstractions.ProjectManagment;
using LeokaEstetica.Platform.Database.Abstractions.Resume;
using LeokaEstetica.Platform.Database.Abstractions.Search;
using LeokaEstetica.Platform.Database.Abstractions.Subscription;
using LeokaEstetica.Platform.Database.Abstractions.Template;
using LeokaEstetica.Platform.Database.Abstractions.Ticket;
using LeokaEstetica.Platform.Database.Abstractions.Vacancy;
using LeokaEstetica.Platform.Database.Access.Ticket;
using LeokaEstetica.Platform.Database.Access.User;
using LeokaEstetica.Platform.Database.Repositories.Access.Ticket;
using LeokaEstetica.Platform.Database.Repositories.Access.User;
using LeokaEstetica.Platform.Database.Repositories.AvailableLimits;
using LeokaEstetica.Platform.Database.Repositories.Commerce;
using LeokaEstetica.Platform.Database.Repositories.Config;
using LeokaEstetica.Platform.Database.Repositories.FareRule;
using LeokaEstetica.Platform.Database.Repositories.Header;
using LeokaEstetica.Platform.Database.Repositories.Knowledge;
using LeokaEstetica.Platform.Database.Repositories.Landing;
using LeokaEstetica.Platform.Database.Repositories.Metrics;
using LeokaEstetica.Platform.Database.Repositories.Moderation.Access;
using LeokaEstetica.Platform.Database.Repositories.Moderation.Project;
using LeokaEstetica.Platform.Database.Repositories.Moderation.Resume;
using LeokaEstetica.Platform.Database.Repositories.Moderation.Vacancy;
using LeokaEstetica.Platform.Database.Repositories.Notification;
using LeokaEstetica.Platform.Database.Repositories.Orders;
using LeokaEstetica.Platform.Database.Repositories.Press;
using LeokaEstetica.Platform.Database.Repositories.Profile;
using LeokaEstetica.Platform.Database.Repositories.Project;
using LeokaEstetica.Platform.Database.Repositories.ProjectManagment;
using LeokaEstetica.Platform.Database.Repositories.Resume;
using LeokaEstetica.Platform.Database.Repositories.Search;
using LeokaEstetica.Platform.Database.Repositories.Subscription;
using LeokaEstetica.Platform.Database.Repositories.Templates;
using LeokaEstetica.Platform.Database.Repositories.TIcket;
using LeokaEstetica.Platform.Database.Repositories.Vacancy;

namespace LeokaEstetica.Platform.Database.AutofacModules;

[CommonModule]
public class RepositoriesModule : Module
{
    public static void InitModules(ContainerBuilder builder)
    {
        // Репозиторий хидера.
        builder.RegisterType<HeaderRepository>()
            .Named<IHeaderRepository>("HeaderRepository")
            .InstancePerLifetimeScope();
        builder.RegisterType<HeaderRepository>()
            .As<IHeaderRepository>()
            .InstancePerLifetimeScope();
            
        // Репозиторий лендингов.
        builder.RegisterType<LandingRepository>()
            .Named<ILandingRepository>("LandingRepository")
            .InstancePerLifetimeScope();
        builder.RegisterType<LandingRepository>()
            .As<ILandingRepository>()
            .InstancePerLifetimeScope();

        // Репозиторий профиля.
        builder.RegisterType<ProfileRepository>()
            .Named<IProfileRepository>("ProfileRepository")
            .InstancePerLifetimeScope();
        builder.RegisterType<ProfileRepository>()
            .As<IProfileRepository>()
            .InstancePerLifetimeScope();
        
        // Репозиторий проектов.
        builder.RegisterType<ProjectRepository>()
            .Named<IProjectRepository>("ProjectRepository")
            .InstancePerLifetimeScope();
        builder.RegisterType<ProjectRepository>()
            .As<IProjectRepository>()
            .InstancePerLifetimeScope();
        
        // Репозиторий вакансий.
        builder.RegisterType<VacancyRepository>()
            .Named<IVacancyRepository>("VacancyRepository")
            .InstancePerLifetimeScope();
        builder.RegisterType<VacancyRepository>()
            .As<IVacancyRepository>()
            .InstancePerLifetimeScope();
        
        // Репозиторий модерации вакансий.
        builder.RegisterType<VacancyModerationRepository>()
            .Named<IVacancyModerationRepository>("VacancyModerationRepository")
            .InstancePerLifetimeScope();
        builder.RegisterType<VacancyModerationRepository>()
            .As<IVacancyModerationRepository>()
            .InstancePerLifetimeScope();

        // Репозиторий доступа к модерации.
        builder.RegisterType<AccessModerationRepository>()
            .Named<IAccessModerationRepository>("AccessModerationRepository")
            .InstancePerLifetimeScope();
        builder.RegisterType<AccessModerationRepository>()
            .As<IAccessModerationRepository>()
            .InstancePerLifetimeScope();
        
        // Репозиторий модерации проектов.
        builder.RegisterType<ProjectModerationRepository>()
            .Named<IProjectModerationRepository>("ProjectModerationRepository")
            .InstancePerLifetimeScope();
        builder.RegisterType<ProjectModerationRepository>()
            .As<IProjectModerationRepository>()
            .InstancePerLifetimeScope();
        
        // Репозиторий комментариев проектов.
        builder.RegisterType<ProjectCommentsRepository>()
            .Named<IProjectCommentsRepository>("ProjectModerationRepository")
            .InstancePerLifetimeScope();
        builder.RegisterType<ProjectCommentsRepository>()
            .As<IProjectCommentsRepository>()
            .InstancePerLifetimeScope();
        
        // Репозиторий базы резюме.
        builder.RegisterType<ResumeRepository>()
            .Named<IResumeRepository>("ResumeRepository")
            .InstancePerLifetimeScope();
        builder.RegisterType<ResumeRepository>()
            .As<IResumeRepository>()
            .InstancePerLifetimeScope();
        
        // Репозиторий заказов.
        builder.RegisterType<CommerceRepository>()
            .Named<ICommerceRepository>("CommerceRepository")
            .InstancePerLifetimeScope();
        builder.RegisterType<CommerceRepository>()
            .As<ICommerceRepository>()
            .InstancePerLifetimeScope();
        
        // Репозиторий подписок.
        builder.RegisterType<SubscriptionRepository>()
            .Named<ISubscriptionRepository>("SubscriptionRepository")
            .InstancePerLifetimeScope();
        builder.RegisterType<SubscriptionRepository>()
            .As<ISubscriptionRepository>()
            .InstancePerLifetimeScope();
        
        // Репозиторий проверки лимитов.
        builder.RegisterType<AvailableLimitsRepository>()
            .Named<IAvailableLimitsRepository>("AvailableLimitsRepository")
            .InstancePerLifetimeScope();
        builder.RegisterType<AvailableLimitsRepository>()
            .As<IAvailableLimitsRepository>()
            .InstancePerLifetimeScope();
        
        // Репозиторий ЧС пользователей.
        builder.RegisterType<UserBlackListRepository>()
            .Named<IUserBlackListRepository>("UserBlackListRepository")
            .InstancePerLifetimeScope();
        builder.RegisterType<UserBlackListRepository>()
            .As<IUserBlackListRepository>()
            .InstancePerLifetimeScope();
        
        // Репозиторий модерации анкет пользователей.
        builder.RegisterType<ResumeModerationRepository>()
            .Named<IResumeModerationRepository>("ResumeModerationRepository")
            .InstancePerLifetimeScope();
        builder.RegisterType<ResumeModerationRepository>()
            .As<IResumeModerationRepository>()
            .InstancePerLifetimeScope();
        
        // Репозиторий уведомлений проектов.
        builder.RegisterType<ProjectNotificationsRepository>()
            .Named<IProjectNotificationsRepository>("ProjectNotificationsRepository")
            .InstancePerLifetimeScope();
        builder.RegisterType<ProjectNotificationsRepository>()
            .As<IProjectNotificationsRepository>()
            .InstancePerLifetimeScope();
        
        // Репозиторий проверки доступа пользователей. 
        builder.RegisterType<AccessUserRepository>()
            .Named<IAccessUserRepository>("AccessUserRepository")
            .InstancePerLifetimeScope();
        builder.RegisterType<AccessUserRepository>()
            .As<IAccessUserRepository>()
            .InstancePerLifetimeScope();
        
        // Репозиторий БЗ.
        builder.RegisterType<KnowledgeRepository>()
            .Named<IKnowledgeRepository>("KnowledgeRepository")
            .InstancePerLifetimeScope();
        builder.RegisterType<KnowledgeRepository>()
            .As<IKnowledgeRepository>()
            .InstancePerLifetimeScope();
        
        // Репозиторий глобал конфига.
        builder.RegisterType<GlobalConfigRepository>()
            .Named<IGlobalConfigRepository>("GlobalConfigRepository")
            .InstancePerLifetimeScope();
        builder.RegisterType<GlobalConfigRepository>()
            .As<IGlobalConfigRepository>()
            .InstancePerLifetimeScope();
        
        // Репозиторий заказов пользователя.
        builder.RegisterType<OrdersRepository>()
            .Named<IOrdersRepository>("OrdersRepository")
            .InstancePerLifetimeScope();
        builder.RegisterType<OrdersRepository>()
            .As<IOrdersRepository>()
            .InstancePerLifetimeScope();
        
        // Репозиторий метрик новых пользователей.
        builder.RegisterType<UserMetricsRepository>()
            .Named<IUserMetricsRepository>("UserMetricsRepository")
            .InstancePerLifetimeScope();
        builder.RegisterType<UserMetricsRepository>()
            .As<IUserMetricsRepository>()
            .InstancePerLifetimeScope();
        
        // Репозиторий тикетов.
        builder.RegisterType<TicketRepository>()
            .Named<ITicketRepository>("TicketRepository")
            .InstancePerLifetimeScope();
        builder.RegisterType<TicketRepository>()
            .As<ITicketRepository>()
            .InstancePerLifetimeScope();
        
        // Репозиторий доступа тикетов.
        builder.RegisterType<AccessTicketRepository>()
            .Named<IAccessTicketRepository>("AccessTicketRepository")
            .InstancePerLifetimeScope();
        builder.RegisterType<AccessTicketRepository>()
            .As<IAccessTicketRepository>()
            .InstancePerLifetimeScope();
        
        // Репозиторий откликов на проекты.
        builder.RegisterType<ProjectResponseRepository>()
            .Named<IProjectResponseRepository>("ProjectResponseRepository")
            .InstancePerLifetimeScope();
        builder.RegisterType<ProjectResponseRepository>()
            .As<IProjectResponseRepository>()
            .InstancePerLifetimeScope();
        
        // Репозиторий прессы.
        builder.RegisterType<PressRepository>()
            .Named<IPressRepository>("PressRepository")
            .InstancePerLifetimeScope();
        builder.RegisterType<PressRepository>()
            .As<IPressRepository>()
            .InstancePerLifetimeScope();

        builder.RegisterType<FareRuleRepository>()
            .Named<IFareRuleRepository>("FareRuleRepository")
            .InstancePerLifetimeScope();
        builder.RegisterType<FareRuleRepository>()
            .As<IFareRuleRepository>()
            .InstancePerLifetimeScope();
        
        builder.RegisterType<ProjectManagmentRepository>()
            .Named<IProjectManagmentRepository>("ProjectManagmentRepository")
            .InstancePerLifetimeScope();
        builder.RegisterType<ProjectManagmentRepository>()
            .As<IProjectManagmentRepository>()
            .InstancePerLifetimeScope();
        
        builder.RegisterType<ProjectManagmentTemplateRepository>()
            .Named<IProjectManagmentTemplateRepository>("ProjectManagmentTemplateRepository")
            .InstancePerLifetimeScope();
        builder.RegisterType<ProjectManagmentTemplateRepository>()
            .As<IProjectManagmentTemplateRepository>()
            .InstancePerLifetimeScope();
        
        builder.RegisterType<ProjectSettingsConfigRepository>()
            .Named<IProjectSettingsConfigRepository>("ProjectSettingsConfigRepository")
            .InstancePerLifetimeScope();
        builder.RegisterType<ProjectSettingsConfigRepository>()
            .As<IProjectSettingsConfigRepository>()
            .InstancePerLifetimeScope();

        builder.RegisterLazy<IProjectManagmentTemplateRepository, ProjectManagmentTemplateRepository>();
        
        builder.RegisterType<NpgSqlConnectionFactory>()
            .Named<IConnectionFactory>("NpgSqlConnectionFactory")
            .InstancePerLifetimeScope();
        builder.RegisterType<NpgSqlConnectionFactory>()
            .As<IConnectionFactory>()
            .InstancePerLifetimeScope();

        builder.RegisterLazy<IGlobalConfigRepository, GlobalConfigRepository>();
        
        builder.RegisterType<SearchProjectManagementRepository>()
            .Named<ISearchProjectManagementRepository>("SearchProjectManagementRepository")
            .InstancePerLifetimeScope();
        builder.RegisterType<SearchProjectManagementRepository>()
            .As<ISearchProjectManagementRepository>()
            .InstancePerLifetimeScope();
        
        builder.RegisterType<SearchProjectManagementRepository>()
            .Named<ISearchProjectManagementRepository>("SearchProjectManagementRepository")
            .InstancePerLifetimeScope();
        builder.RegisterType<SearchProjectManagementRepository>()
            .As<ISearchProjectManagementRepository>()
            .InstancePerLifetimeScope();
        
        builder.RegisterType<SprintRepository>()
            .Named<ISprintRepository>("SprintRepository")
            .InstancePerLifetimeScope();
        builder.RegisterType<SprintRepository>()
            .As<ISprintRepository>()
            .InstancePerLifetimeScope();
            
        builder.RegisterType<ProjectManagementSettingsRepository>()
            .Named<IProjectManagementSettingsRepository>("ProjectManagementSettingsRepository")
            .InstancePerLifetimeScope();
        builder.RegisterType<ProjectManagementSettingsRepository>()
            .As<IProjectManagementSettingsRepository>()
            .InstancePerLifetimeScope();
            
        builder.RegisterType<ScrumMasterAiRepository>()
            .Named<IScrumMasterAiRepository>("ScrumMasterAiRepository")
            .InstancePerLifetimeScope();
        builder.RegisterType<ScrumMasterAiRepository>()
            .As<IScrumMasterAiRepository>()
            .InstancePerLifetimeScope();
            
        builder.RegisterType<ProjectManagmentRoleRepository>()
            .Named<IProjectManagmentRoleRepository>("ProjectManagmentRoleRepository")
            .InstancePerLifetimeScope();
        builder.RegisterType<ProjectManagmentRoleRepository>()
            .As<IProjectManagmentRoleRepository>()
            .InstancePerLifetimeScope();
            
        builder.RegisterLazy<IProjectManagmentRoleRepository, ProjectManagmentRoleRepository>();
        
        builder.RegisterType<WikiTreeRepository>()
            .Named<IWikiTreeRepository>("WikiTreeRepository")
            .InstancePerLifetimeScope();
        builder.RegisterType<WikiTreeRepository>()
            .As<IWikiTreeRepository>()
            .InstancePerLifetimeScope();
    }
}