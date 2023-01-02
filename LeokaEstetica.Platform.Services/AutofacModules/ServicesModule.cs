using Autofac;
using LeokaEstetica.Platform.Core.Attributes;
using LeokaEstetica.Platform.Services.Abstractions.Header;
using LeokaEstetica.Platform.Services.Abstractions.Landing;
using LeokaEstetica.Platform.Services.Abstractions.Profile;
using LeokaEstetica.Platform.Services.Abstractions.Project;
using LeokaEstetica.Platform.Services.Abstractions.Search.Project;
using LeokaEstetica.Platform.Services.Abstractions.User;
using LeokaEstetica.Platform.Services.Abstractions.Vacancy;
using LeokaEstetica.Platform.Services.Services.Header;
using LeokaEstetica.Platform.Services.Services.Landing;
using LeokaEstetica.Platform.Services.Services.Profile;
using LeokaEstetica.Platform.Services.Services.Project;
using LeokaEstetica.Platform.Services.Services.Search.Project;
using LeokaEstetica.Platform.Services.Services.User;
using LeokaEstetica.Platform.Services.Services.Vacancy;

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
            .InstancePerLifetimeScope();
        builder
            .RegisterType<ProjectService>()
            .As<IProjectService>()
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
        
        // Сервис поиска в проектах.
        builder
            .RegisterType<SearchProjectService>()
            .Named<ISearchProjectService>("SearchProjectService")
            .InstancePerLifetimeScope();
        builder
            .RegisterType<SearchProjectService>()
            .As<ISearchProjectService>()
            .InstancePerLifetimeScope();
    }
}