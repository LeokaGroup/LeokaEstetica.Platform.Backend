using Autofac;
using LeokaEstetica.Platform.Access.Abstractions.Moderation;
using LeokaEstetica.Platform.Access.Services.Moderation;
using LeokaEstetica.Platform.Core.Attributes;

namespace LeokaEstetica.Platform.Access.AutofacModules;

[CommonModule]
public class AccessModule : Module
{
    public static void InitModules(ContainerBuilder builder)
    {
        // Сервис проверки доступа к модерации.
        builder
            .RegisterType<AccessModerationService>()
            .Named<IAccessModerationService>("AccessModerationService")
            .InstancePerLifetimeScope();
        builder
            .RegisterType<AccessModerationService>()
            .As<IAccessModerationService>()
            .InstancePerLifetimeScope();
    }
}