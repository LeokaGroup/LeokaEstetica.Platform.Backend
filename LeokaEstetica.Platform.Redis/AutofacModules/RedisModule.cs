using Autofac;
using LeokaEstetica.Platform.Core.Attributes;
using LeokaEstetica.Platform.Redis.Abstractions.Commerce;
using LeokaEstetica.Platform.Redis.Abstractions.Notification;
using LeokaEstetica.Platform.Redis.Abstractions.Profile;
using LeokaEstetica.Platform.Redis.Abstractions.User;
using LeokaEstetica.Platform.Redis.Abstractions.Vacancy;
using LeokaEstetica.Platform.Redis.Abstractions.Validation;
using LeokaEstetica.Platform.Redis.Services.Commerce;
using LeokaEstetica.Platform.Redis.Services.Notification;
using LeokaEstetica.Platform.Redis.Services.Profile;
using LeokaEstetica.Platform.Redis.Services.User;
using LeokaEstetica.Platform.Redis.Services.Vacancy;
using LeokaEstetica.Platform.Redis.Services.Validation;

namespace LeokaEstetica.Platform.Redis.AutofacModules;

[CommonModule]
public class RedisModule : Module
{
    public static void InitModules(ContainerBuilder builder)
    {
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
        
        // Сервис уведомлений кэша.
        builder
            .RegisterType<NotificationsRedisService>()
            .Named<INotificationsRedisService>("NotificationsRedisService")
            .InstancePerLifetimeScope();
        builder
            .RegisterType<NotificationsRedisService>()
            .As<INotificationsRedisService>()
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
    }
}