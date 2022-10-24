using Autofac;
using LeokaEstetica.Platform.Core.Attributes;
using LeokaEstetica.Platform.Redis.Abstractions;
using LeokaEstetica.Platform.Redis.Services;

namespace LeokaEstetica.Platform.Redis.AutofacModules;

[CommonModule]
public class RedisModule : Module
{
    public static void InitModules(ContainerBuilder builder)
    {
        // Сервис работы с кэшем Redis.
        builder.RegisterType<RedisService>()
            .Named<IRedisService>("RedisService")
            .InstancePerLifetimeScope();
        builder.RegisterType<RedisService>()
            .As<IRedisService>()
            .InstancePerLifetimeScope();
    }
}