using Autofac;
using LeokaEstetica.Platform.Core.Attributes;
using LeokaEstetica.Platform.Redis.Abstractions.Profile;
using LeokaEstetica.Platform.Redis.Abstractions.Vacancy;
using LeokaEstetica.Platform.Redis.Abstractions.Validation;
using LeokaEstetica.Platform.Redis.Services.Profile;
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
    }
}