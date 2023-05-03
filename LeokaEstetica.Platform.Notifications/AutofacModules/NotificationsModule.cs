using Autofac;
using LeokaEstetica.Platform.Core.Attributes;
using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.Notifications.Services;

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
        builder.RegisterType<NotificationsService>()
            .As<INotificationsService>()
            .InstancePerLifetimeScope();
        
        // Сервис уведомлений проектов.
        builder.RegisterType<ProjectNotificationsService>()
            .Named<IProjectNotificationsService>("ProjectNotificationsService")
            .InstancePerLifetimeScope();
        builder.RegisterType<ProjectNotificationsService>()
            .As<IProjectNotificationsService>()
            .InstancePerLifetimeScope();
        
        // Сервис уведомлений вакансий.
        builder.RegisterType<VacancyNotificationsService>()
            .Named<IVacancyNotificationsService>("VacancyNotificationsService")
            .InstancePerLifetimeScope();
        builder.RegisterType<VacancyNotificationsService>()
            .As<IVacancyNotificationsService>()
            .InstancePerLifetimeScope();
        
        // Сервис уведомлений доступа пользователей.
        builder.RegisterType<AccessUserNotificationsService>()
            .Named<IAccessUserNotificationsService>("AccessUserNotificationsService")
            .InstancePerLifetimeScope();
        builder.RegisterType<AccessUserNotificationsService>()
            .As<IAccessUserNotificationsService>()
            .InstancePerLifetimeScope();

        // Сервис уведомлений комметариев.
        builder.RegisterType<CommentNotificationsService>()
            .Named<ICommentNotificationsService>("CommentNotificationsService")
            .InstancePerLifetimeScope();
        builder.RegisterType<CommentNotificationsService>()
            .As<ICommentNotificationsService>()
            .InstancePerLifetimeScope();
        
        // Сервис уведомлений модерации проектов.
        builder.RegisterType<ProjectModerationNotificationService>()
            .Named<IProjectModerationNotificationService>("ProjectModerationNotificationService")
            .InstancePerLifetimeScope();
        builder.RegisterType<ProjectModerationNotificationService>()
            .As<IProjectModerationNotificationService>()
            .InstancePerLifetimeScope();
        
        // Сервис уведомлений модерации вакансий.
        builder.RegisterType<VacancyModerationNotificationService>()
            .Named<IVacancyModerationNotificationService>("VacancyModerationNotificationService")
            .InstancePerLifetimeScope();
        builder.RegisterType<VacancyModerationNotificationService>()
            .As<IVacancyModerationNotificationService>()
            .InstancePerLifetimeScope();
        
        // Сервис уведомлений модерации анкет.
        builder.RegisterType<ResumeModerationNotificationService>()
            .Named<IResumeModerationNotificationService>("ResumeModerationNotificationService")
            .InstancePerLifetimeScope();
        builder.RegisterType<ResumeModerationNotificationService>()
            .As<IResumeModerationNotificationService>()
            .InstancePerLifetimeScope();
    }
}