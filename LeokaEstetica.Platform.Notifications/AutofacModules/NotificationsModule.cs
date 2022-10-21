using Autofac;
using LeokaEstetica.Platform.Core.Attributes;
using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.Notifications.Services;
using Module = System.Reflection.Module;

namespace LeokaEstetica.Platform.Notifications.AutofacModules;

[CommonModule]
public class NotificationsModule : Module
{
    public static void InitModules(ContainerBuilder builder)
    {
        // Сервис уведомлений.
        builder.RegisterType<NotificationsService>()
            .Named<INotificationsService>("NotificationsService")
            .InstancePerLifetimeScope();
    }
}