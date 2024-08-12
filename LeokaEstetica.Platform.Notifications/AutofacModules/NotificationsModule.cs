using Autofac;
using LazyProxy.Autofac;
using LeokaEstetica.Platform.Core.Attributes;
using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.Notifications.Data;
using LeokaEstetica.Platform.Notifications.Services;

namespace LeokaEstetica.Platform.Notifications.AutofacModules;

[CommonModule]
public class NotificationsModule : Module
{
    public static void InitModules(ContainerBuilder builder)
    {
        // Сервис уведомлений проектов.
        builder.RegisterType<ProjectNotificationsService>()
            .Named<IProjectNotificationsService>("ProjectNotificationsService")
            .InstancePerLifetimeScope();
        builder.RegisterType<ProjectNotificationsService>()
            .As<IProjectNotificationsService>()
            .InstancePerLifetimeScope();

        // Сервис уведомлений модуля УП.
        builder.RegisterLazy<IHubNotificationService, HubNotificationService>();

        builder.RegisterType<ChatHub>()
            .Named<IHubService>("ChatHub")
            .InstancePerLifetimeScope();
        builder.RegisterType<ChatHub>()
            .As<IHubService>()
            .InstancePerLifetimeScope();
            
        builder.RegisterType<ProjectManagementHub>()
            .Named<IHubService>("ProjectManagementHub")
            .InstancePerLifetimeScope();
        builder.RegisterType<ProjectManagementHub>()
            .As<IHubService>()
            .InstancePerLifetimeScope();
    }
}