using Autofac;
using LeokaEstetica.Platform.Core.Attributes;
using LeokaEstetica.Platform.Database.Abstractions.AvailableLimits;
using LeokaEstetica.Platform.Database.Abstractions.Commerce;
using LeokaEstetica.Platform.Database.Abstractions.Header;
using LeokaEstetica.Platform.Database.Abstractions.Landing;
using LeokaEstetica.Platform.Database.Abstractions.Moderation.Access;
using LeokaEstetica.Platform.Database.Abstractions.Moderation.Project;
using LeokaEstetica.Platform.Database.Abstractions.Moderation.Resume;
using LeokaEstetica.Platform.Database.Abstractions.Moderation.Vacancy;
using LeokaEstetica.Platform.Database.Abstractions.Notification;
using LeokaEstetica.Platform.Database.Abstractions.Profile;
using LeokaEstetica.Platform.Database.Abstractions.Project;
using LeokaEstetica.Platform.Database.Abstractions.Resume;
using LeokaEstetica.Platform.Database.Abstractions.Subscription;
using LeokaEstetica.Platform.Database.Abstractions.User;
using LeokaEstetica.Platform.Database.Abstractions.Vacancy;
using LeokaEstetica.Platform.Database.Access.User;
using LeokaEstetica.Platform.Database.Chat;
using LeokaEstetica.Platform.Database.Repositories.Access.User;
using LeokaEstetica.Platform.Database.Repositories.AvailableLimits;
using LeokaEstetica.Platform.Database.Repositories.Chat;
using LeokaEstetica.Platform.Database.Repositories.Commerce;
using LeokaEstetica.Platform.Database.Repositories.Header;
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

namespace LeokaEstetica.Platform.Database.AutofacModules;

[CommonModule]
public class RepositoriesModule : Module
{
    public static void InitModules(ContainerBuilder builder)
    {
        // Репозиторий хидера.
        builder
            .RegisterType<HeaderRepository>()
            .Named<IHeaderRepository>("HeaderRepository")
            .InstancePerLifetimeScope();
        builder
            .RegisterType<HeaderRepository>()
            .As<IHeaderRepository>()
            .InstancePerLifetimeScope();
            
        // Репозиторий лендингов.
        builder
            .RegisterType<LandingRepository>()
            .Named<ILandingRepository>("LandingRepository")
            .InstancePerLifetimeScope();
        builder
            .RegisterType<LandingRepository>()
            .As<ILandingRepository>()
            .InstancePerLifetimeScope();
            
        // Репозиторий пользователей.
        builder
            .RegisterType<UserRepository>()
            .Named<IUserRepository>("LandingRepository")
            .InstancePerLifetimeScope();
        builder
            .RegisterType<UserRepository>()
            .As<IUserRepository>()
            .InstancePerLifetimeScope();
            
        // Репозиторий профиля.
        builder
            .RegisterType<ProfileRepository>()
            .Named<IProfileRepository>("ProfileRepository")
            .InstancePerLifetimeScope();
        builder
            .RegisterType<ProfileRepository>()
            .As<IProfileRepository>()
            .InstancePerLifetimeScope();
        
        // Репозиторий проектов.
        builder
            .RegisterType<ProjectRepository>()
            .Named<IProjectRepository>("ProjectRepository")
            .InstancePerLifetimeScope();
        builder
            .RegisterType<ProjectRepository>()
            .As<IProjectRepository>()
            .InstancePerLifetimeScope();
        
        // Репозиторий вакансий.
        builder
            .RegisterType<VacancyRepository>()
            .Named<IVacancyRepository>("VacancyRepository")
            .InstancePerLifetimeScope();
        builder
            .RegisterType<VacancyRepository>()
            .As<IVacancyRepository>()
            .InstancePerLifetimeScope();
        
        // Репозиторий модерации вакансий.
        builder
            .RegisterType<VacancyModerationRepository>()
            .Named<IVacancyModerationRepository>("VacancyModerationRepository")
            .InstancePerLifetimeScope();
        builder
            .RegisterType<VacancyModerationRepository>()
            .As<IVacancyModerationRepository>()
            .InstancePerLifetimeScope();
        
        // Репозиторий чата.
        builder
            .RegisterType<ChatRepository>()
            .Named<IChatRepository>("ChatRepository")
            .InstancePerLifetimeScope();
        builder
            .RegisterType<ChatRepository>()
            .As<IChatRepository>()
            .InstancePerLifetimeScope();
        
        // Репозиторий доступа к модерации.
        builder
            .RegisterType<AccessModerationRepository>()
            .Named<IAccessModerationRepository>("AccessModerationRepository")
            .InstancePerLifetimeScope();
        builder
            .RegisterType<AccessModerationRepository>()
            .As<IAccessModerationRepository>()
            .InstancePerLifetimeScope();
        
        // Репозиторий модерации проектов.
        builder
            .RegisterType<ProjectModerationRepository>()
            .Named<IProjectModerationRepository>("ProjectModerationRepository")
            .InstancePerLifetimeScope();
        builder
            .RegisterType<ProjectModerationRepository>()
            .As<IProjectModerationRepository>()
            .InstancePerLifetimeScope();
        
        // Репозиторий комментариев проектов.
        builder
            .RegisterType<ProjectCommentsRepository>()
            .Named<IProjectCommentsRepository>("ProjectModerationRepository")
            .InstancePerLifetimeScope();
        builder
            .RegisterType<ProjectCommentsRepository>()
            .As<IProjectCommentsRepository>()
            .InstancePerLifetimeScope();
        
        // Репозиторий базы резюме.
        builder
            .RegisterType<ResumeRepository>()
            .Named<IResumeRepository>("ResumeRepository")
            .InstancePerLifetimeScope();
        builder
            .RegisterType<ResumeRepository>()
            .As<IResumeRepository>()
            .InstancePerLifetimeScope();
        
        // Репозиторий заказов.
        builder
            .RegisterType<PayMasterRepository>()
            .Named<IPayMasterRepository>("PayMasterRepository")
            .InstancePerLifetimeScope();
        builder
            .RegisterType<PayMasterRepository>()
            .As<IPayMasterRepository>()
            .InstancePerLifetimeScope();
        
        // Репозиторий подписок.
        builder
            .RegisterType<SubscriptionRepository>()
            .Named<ISubscriptionRepository>("SubscriptionRepository")
            .InstancePerLifetimeScope();
        builder
            .RegisterType<SubscriptionRepository>()
            .As<ISubscriptionRepository>()
            .InstancePerLifetimeScope();
        
        // Репозиторий проверки лимитов.
        builder
            .RegisterType<AvailableLimitsRepository>()
            .Named<IAvailableLimitsRepository>("AvailableLimitsRepository")
            .InstancePerLifetimeScope();
        builder
            .RegisterType<AvailableLimitsRepository>()
            .As<IAvailableLimitsRepository>()
            .InstancePerLifetimeScope();
        
        // Репозиторий ЧС пользователей.
        builder
            .RegisterType<UserBlackListRepository>()
            .Named<IUserBlackListRepository>("UserBlackListRepository")
            .InstancePerLifetimeScope();
        builder
            .RegisterType<UserBlackListRepository>()
            .As<IUserBlackListRepository>()
            .InstancePerLifetimeScope();
        
        // Репозиторий модерации анкет пользователей.
        builder
            .RegisterType<ResumeModerationRepository>()
            .Named<IResumeModerationRepository>("ResumeModerationRepository")
            .InstancePerLifetimeScope();
        builder
            .RegisterType<ResumeModerationRepository>()
            .As<IResumeModerationRepository>()
            .InstancePerLifetimeScope();
        
        // Репозиторий уведомлений проектов.
        builder
            .RegisterType<ProjectNotificationsRepository>()
            .Named<IProjectNotificationsRepository>("ProjectNotificationsRepository")
            .InstancePerLifetimeScope();
        builder
            .RegisterType<ProjectNotificationsRepository>()
            .As<IProjectNotificationsRepository>()
            .InstancePerLifetimeScope();
        
        // Репозиторий проверки доступа пользователей. 
        builder
            .RegisterType<AccessUserRepository>()
            .Named<IAccessUserRepository>("AccessUserRepository")
            .InstancePerLifetimeScope();
        builder
            .RegisterType<AccessUserRepository>()
            .As<IAccessUserRepository>()
            .InstancePerLifetimeScope();
    }
}