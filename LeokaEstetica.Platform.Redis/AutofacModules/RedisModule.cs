using Autofac;
using LeokaEstetica.Platform.Core.Attributes;
using LeokaEstetica.Platform.Redis.Abstractions.Client;
using LeokaEstetica.Platform.Redis.Abstractions.Commerce;
using LeokaEstetica.Platform.Redis.Abstractions.Connection;
using LeokaEstetica.Platform.Redis.Abstractions.Profile;
using LeokaEstetica.Platform.Redis.Abstractions.ProjectManagement;
using LeokaEstetica.Platform.Redis.Abstractions.User;
using LeokaEstetica.Platform.Redis.Abstractions.Vacancy;
using LeokaEstetica.Platform.Redis.Abstractions.Validation;
using LeokaEstetica.Platform.Redis.Services.Client;
using LeokaEstetica.Platform.Redis.Services.Commerce;
using LeokaEstetica.Platform.Redis.Services.Connection;
using LeokaEstetica.Platform.Redis.Services.Header;
using LeokaEstetica.Platform.Redis.Services.Profile;
using LeokaEstetica.Platform.Redis.Services.ProjectManagement;
using LeokaEstetica.Platform.Redis.Services.User;
using LeokaEstetica.Platform.Redis.Services.Vacancy;
using LeokaEstetica.Platform.Redis.Services.Validation;
using LeokaEstetica.Platform.Services.Abstractions.Header;

namespace LeokaEstetica.Platform.Redis.AutofacModules;

[CommonModule]
public class RedisModule : Module
{
    public static void InitModules(ContainerBuilder builder)
    {
        // Сервис работы с кэшем меню хидера Redis.
        builder
            .RegisterType<HeaderRedisService>()
            .Named<IHeaderRedisService>("HeaderRedisService")
            .InstancePerLifetimeScope();
        builder
            .RegisterType<HeaderRedisService>()
            .As<IHeaderRedisService>()
            .InstancePerLifetimeScope();
        
        // Сервис работы с кэшем профиля Redis.
        builder
            .RegisterType<ProfileRedisService>()
            .Named<IProfileRedisService>("ProfileRedisService")
            .InstancePerLifetimeScope();
        builder
            .RegisterType<ProfileRedisService>()
            .As<IProfileRedisService>()
            .InstancePerLifetimeScope();
        
        // Сервис работы с кэшем вакансий Redis.
        builder
            .RegisterType<VacancyRedisService>()
            .Named<IVacancyRedisService>("VacancyRedisService")
            .InstancePerLifetimeScope();
        builder
            .RegisterType<VacancyRedisService>()
            .As<IVacancyRedisService>()
            .InstancePerLifetimeScope();
        
        // Сервис работы с кэшем валидации Redis.
        builder
            .RegisterType<ValidationExcludeErrorsCacheService>()
            .Named<IValidationExcludeErrorsCacheService>("ValidationExcludeErrorsCacheService")
            .InstancePerLifetimeScope();
        builder
            .RegisterType<ValidationExcludeErrorsCacheService>()
            .As<IValidationExcludeErrorsCacheService>()
            .InstancePerLifetimeScope();
        
        // Сервис подключений Redis.
        builder
            .RegisterType<ConnectionService>()
            .Named<IConnectionService>("ConnectionService")
            .InstancePerLifetimeScope();
        builder
            .RegisterType<ConnectionService>()
            .As<IConnectionService>()
            .InstancePerLifetimeScope();
        
        // Сервис кэша пользователей.
        builder
            .RegisterType<UserRedisService>()
            .Named<IUserRedisService>("UserRedisService")
            .InstancePerLifetimeScope();
        builder
            .RegisterType<UserRedisService>()
            .As<IUserRedisService>()
            .InstancePerLifetimeScope();
        
        // Сервис кэша коммерции.
        builder
            .RegisterType<CommerceRedisService>()
            .Named<ICommerceRedisService>("CommerceRedisService")
            .InstancePerLifetimeScope();
        builder
            .RegisterType<CommerceRedisService>()
            .As<ICommerceRedisService>()
            .InstancePerLifetimeScope();
        
        builder
            .RegisterType<ClientConnectionService>()
            .Named<IClientConnectionService>("ClientConnectionService")
            .InstancePerLifetimeScope();
        builder
            .RegisterType<ClientConnectionService>()
            .As<IClientConnectionService>()
            .InstancePerLifetimeScope();
        
        builder.RegisterType<ProjectManagmentRoleRedisService>()
            .Named<IProjectManagmentRoleRedisService>("ProjectManagmentRoleRedisService")
            .InstancePerLifetimeScope();
        builder.RegisterType<ProjectManagmentRoleRedisService>()
            .As<IProjectManagmentRoleRedisService>()
            .InstancePerLifetimeScope();
    }
}