using Autofac;
using LeokaEstetica.Platform.Core.Attributes;
using LeokaEstetica.Platform.Services.Abstractions.Config;
using LeokaEstetica.Platform.Services.Abstractions.FareRule;
using LeokaEstetica.Platform.Services.Abstractions.Header;
using LeokaEstetica.Platform.Services.Abstractions.Knowledge;
using LeokaEstetica.Platform.Services.Abstractions.Landing;
using LeokaEstetica.Platform.Services.Abstractions.Orders;
using LeokaEstetica.Platform.Services.Abstractions.Press;
using LeokaEstetica.Platform.Services.Abstractions.Profile;
using LeokaEstetica.Platform.Services.Abstractions.Project;
using LeokaEstetica.Platform.Services.Abstractions.ProjectManagment;
using LeokaEstetica.Platform.Services.Abstractions.Refunds;
using LeokaEstetica.Platform.Services.Abstractions.Resume;
using LeokaEstetica.Platform.Services.Abstractions.Search.Project;
using LeokaEstetica.Platform.Services.Abstractions.Search.ProjectManagment;
using LeokaEstetica.Platform.Services.Abstractions.Subscription;
using LeokaEstetica.Platform.Services.Abstractions.User;
using LeokaEstetica.Platform.Services.Abstractions.Vacancy;
using LeokaEstetica.Platform.Services.Services.Config;
using LeokaEstetica.Platform.Services.Services.FareRule;
using LeokaEstetica.Platform.Services.Services.Header;
using LeokaEstetica.Platform.Services.Services.Knowledge;
using LeokaEstetica.Platform.Services.Services.Landing;
using LeokaEstetica.Platform.Services.Services.Orders;
using LeokaEstetica.Platform.Services.Services.Press;
using LeokaEstetica.Platform.Services.Services.Profile;
using LeokaEstetica.Platform.Services.Services.Project;
using LeokaEstetica.Platform.Services.Services.ProjectManagment;
using LeokaEstetica.Platform.Services.Services.Refunds;
using LeokaEstetica.Platform.Services.Services.Resume;
using LeokaEstetica.Platform.Services.Services.Search.Project;
using LeokaEstetica.Platform.Services.Services.Search.ProjectManagment;
using LeokaEstetica.Platform.Services.Services.Subscription;
using LeokaEstetica.Platform.Services.Services.User;
using LeokaEstetica.Platform.Services.Services.Vacancy;
using LeokaEstetica.Platform.Services.Strategies.Project.Team;
using LeokaEstetica.Platform.Services.Strategies.Refunds;

namespace LeokaEstetica.Platform.Services.AutofacModules;

[CommonModule]
public class ServicesModule : Module
{
    public static void InitModules(ContainerBuilder builder)
    {
        // Сервис хидера.
        builder
            .RegisterType<HeaderService>()
            .Named<IHeaderService>("HeaderService")
            .InstancePerLifetimeScope();
        builder
            .RegisterType<HeaderService>()
            .As<IHeaderService>()
            .InstancePerLifetimeScope();
            
        // Сервис лендингов.
        builder
            .RegisterType<LandingService>()
            .Named<ILandingService>("LandingService")
            .InstancePerLifetimeScope();
        builder
            .RegisterType<LandingService>()
            .As<ILandingService>()
            .InstancePerLifetimeScope();
            
        // Сервис пользователя.
        builder
            .RegisterType<UserService>()
            .Named<IUserService>("UserService")
            .InstancePerLifetimeScope();
        builder
            .RegisterType<UserService>()
            .As<IUserService>()
            .InstancePerLifetimeScope();
            
        // Сервис профиля пользователя.
        builder
            .RegisterType<ProfileService>()
            .Named<IProfileService>("ProfileService")
            .InstancePerLifetimeScope();
        builder
            .RegisterType<ProfileService>()
            .As<IProfileService>()
            .InstancePerLifetimeScope();
        
        // Сервис проектов.
        builder
            .RegisterType<ProjectService>()
            .Named<IProjectService>("ProjectService")
            .SingleInstance();
        builder
            .RegisterType<ProjectService>()
            .As<IProjectService>()
            .SingleInstance();

        // Сервис выделение цветом пользователей, у которых подписка выше уровня бизнес.
        builder
            .RegisterType<FillColorProjectsService>()
            .Named<IFillColorProjectsService>("FillColorProjectsService")
            .InstancePerLifetimeScope();
        builder
            .RegisterType<FillColorProjectsService>()
            .As<IFillColorProjectsService>()
            .InstancePerLifetimeScope();

        // Сервис вакансий.
        builder
            .RegisterType<VacancyService>()
            .Named<IVacancyService>("VacancyService")
            .InstancePerLifetimeScope();
        builder
            .RegisterType<VacancyService>()
            .As<IVacancyService>()
            .InstancePerLifetimeScope();

        // Сервис выделение цветом пользователей, у которых подписка выше уровня бизнес.
        builder
            .RegisterType<FillColorVacanciesService>()
            .Named<IFillColorVacanciesService>("FillColorVacanciesService")
            .InstancePerLifetimeScope();
        builder
            .RegisterType<FillColorVacanciesService>()
            .As<IFillColorVacanciesService>()
            .InstancePerLifetimeScope();

        // Сервис поиска в проектах.
        builder
            .RegisterType<ProjectFinderService>()
            .Named<IProjectFinderService>("ProjectFinderService")
            .InstancePerLifetimeScope();
        builder
            .RegisterType<ProjectFinderService>()
            .As<IProjectFinderService>()
            .InstancePerLifetimeScope();
        
        // Сервис базы резюме.
        builder
            .RegisterType<ResumeService>()
            .Named<IResumeService>("ResumeService")
            .InstancePerLifetimeScope();
        builder
            .RegisterType<ResumeService>()
            .As<IResumeService>()
            .InstancePerLifetimeScope();
        
        // Сервис правил тарифов.
        builder
            .RegisterType<FareRuleService>()
            .Named<IFareRuleService>("FareRuleService")
            .InstancePerLifetimeScope();
        builder
            .RegisterType<FareRuleService>()
            .As<IFareRuleService>()
            .InstancePerLifetimeScope();
        
        // Сервис подписок.
        builder
            .RegisterType<SubscriptionService>()
            .Named<ISubscriptionService>("SubscriptionService")
            .InstancePerLifetimeScope();
        builder
            .RegisterType<SubscriptionService>()
            .As<ISubscriptionService>()
            .InstancePerLifetimeScope();
        
        // Класс стратегии приглашения в проект по ссылке.
        builder
            .RegisterType<ProjectInviteTeamLinkStrategy>()
            .Named<BaseProjectInviteTeamStrategy>("ProjectInviteTeamLinkStrategy")
            .InstancePerLifetimeScope();
        builder
            .RegisterType<ProjectInviteTeamLinkStrategy>()
            .As<BaseProjectInviteTeamStrategy>()
            .InstancePerLifetimeScope();
        
        // Класс стратегии приглашения в проект по Email.
        builder
            .RegisterType<ProjectInviteTeamEmailStrategy>()
            .Named<BaseProjectInviteTeamStrategy>("ProjectInviteTeamEmailStrategy")
            .InstancePerLifetimeScope();
        builder
            .RegisterType<ProjectInviteTeamEmailStrategy>()
            .As<BaseProjectInviteTeamStrategy>()
            .InstancePerLifetimeScope();
        
        // Класс стратегии приглашения в проект по номеру телефона.
        builder
            .RegisterType<ProjectInviteTeamPhoneNumberStrategy>()
            .Named<BaseProjectInviteTeamStrategy>("ProjectInviteTeamPhoneNumberStrategy")
            .InstancePerLifetimeScope();
        builder
            .RegisterType<ProjectInviteTeamPhoneNumberStrategy>()
            .As<BaseProjectInviteTeamStrategy>()
            .InstancePerLifetimeScope();
        
        // Класс стратегии приглашения в проект по логину.
        builder
            .RegisterType<ProjectInviteTeamLoginStrategy>()
            .Named<BaseProjectInviteTeamStrategy>("ProjectInviteTeamLoginStrategy")
            .InstancePerLifetimeScope();
        builder
            .RegisterType<ProjectInviteTeamLoginStrategy>()
            .As<BaseProjectInviteTeamStrategy>()
            .InstancePerLifetimeScope();
        
        // Сервис БЗ.
        builder
            .RegisterType<KnowledgeService>()
            .Named<IKnowledgeService>("KnowledgeService")
            .InstancePerLifetimeScope();
        builder
            .RegisterType<KnowledgeService>()
            .As<IKnowledgeService>()
            .InstancePerLifetimeScope();
        
        // Сервис заказов пользователя.
        builder.RegisterType<OrdersService>()
            .Named<IOrdersService>("OrdersService")
            .InstancePerLifetimeScope();
        builder.RegisterType<OrdersService>()
            .As<IOrdersService>()
            .InstancePerLifetimeScope();
        
        // Класс стратегии вычисления суммы возврата на основании использованных дней.
        builder
            .RegisterType<CalculateRefundUsedDaysStrategy>()
            .Named<BaseCalculateRefundStrategy>("CalculateRefundUsedDaysStrategy")
            .InstancePerLifetimeScope();
        builder
            .RegisterType<CalculateRefundUsedDaysStrategy>()
            .As<BaseCalculateRefundStrategy>()
            .InstancePerLifetimeScope();
        
        // Сервис возвратов в нашей системе.
        builder
            .RegisterType<RefundsService>()
            .Named<IRefundsService>("RefundsService")
            .InstancePerLifetimeScope();
        builder
            .RegisterType<RefundsService>()
            .As<IRefundsService>()
            .InstancePerLifetimeScope();
        
        // Сервис прессы.
        builder.RegisterType<PressService>()
            .Named<IPressService>("PressService")
            .InstancePerLifetimeScope();
        builder.RegisterType<PressService>()
            .As<IPressService>()
            .InstancePerLifetimeScope();
        
        // Сервис управления проектами.
        builder.RegisterType<ProjectManagmentService>()
            .Named<IProjectManagmentService>("ProjectManagmentService")
            .InstancePerLifetimeScope();
        builder.RegisterType<ProjectManagmentService>()
            .As<IProjectManagmentService>()
            .InstancePerLifetimeScope();
            
        // Сервис настроек проектов.
        builder.RegisterType<ProjectSettingsConfigService>()
            .Named<IProjectSettingsConfigService>("ProjectSettingsConfigService")
            .InstancePerLifetimeScope();
        builder.RegisterType<ProjectSettingsConfigService>()
            .As<IProjectSettingsConfigService>()
            .InstancePerLifetimeScope();
            
        // Сервис поиска в модуле УП.
        builder.RegisterType<SearchProjectManagementService>()
            .Named<ISearchProjectManagementService>("SearchProjectManagementService")
            .InstancePerLifetimeScope();
        builder.RegisterType<SearchProjectManagementService>()
            .As<ISearchProjectManagementService>()
            .InstancePerLifetimeScope();
    }
}