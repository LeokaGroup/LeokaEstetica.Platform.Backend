using Autofac;
using LeokaEstetica.Platform.Access.Abstractions.AvailableLimits;
using LeokaEstetica.Platform.Access.Abstractions.Moderation;
using LeokaEstetica.Platform.Access.Abstractions.Resume;
using LeokaEstetica.Platform.Access.Abstractions.User;
using LeokaEstetica.Platform.Access.Services.AvailableLimits;
using LeokaEstetica.Platform.Access.Services.Moderation;
using LeokaEstetica.Platform.Access.Services.Resume;
using LeokaEstetica.Platform.Access.Services.User;
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
        
        // Сервис проверки доступа к базе резюме.
        builder
            .RegisterType<AccessResumeService>()
            .Named<IAccessResumeService>("AccessResumeService")
            .InstancePerLifetimeScope();
        builder
            .RegisterType<AccessResumeService>()
            .As<IAccessResumeService>()
            .InstancePerLifetimeScope();
        
        // Сервис проверки лимитов.
        builder
            .RegisterType<AvailableLimitsService>()
            .Named<IAvailableLimitsService>("AvailableLimitsService")
            .InstancePerLifetimeScope();
        builder
            .RegisterType<AvailableLimitsService>()
            .As<IAvailableLimitsService>()
            .InstancePerLifetimeScope();
        
        // Сервис ЧС пользователей.
        builder
            .RegisterType<UserBlackListService>()
            .Named<IUserBlackListService>("UserBlackListService")
            .InstancePerLifetimeScope();
        builder
            .RegisterType<UserBlackListService>()
            .As<IUserBlackListService>()
            .InstancePerLifetimeScope();
        
        // Сервис проверки блокировки пользователей.
        builder
            .RegisterType<AccessUserService>()
            .Named<IAccessUserService>("AccessUserService")
            .InstancePerLifetimeScope();
        builder
            .RegisterType<AccessUserService>()
            .As<IAccessUserService>()
            .InstancePerLifetimeScope();
    }
}