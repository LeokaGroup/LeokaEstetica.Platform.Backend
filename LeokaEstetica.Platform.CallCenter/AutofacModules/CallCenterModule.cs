using Autofac;
using LeokaEstetica.Platform.CallCenter.Abstractions.Messaging.Mail;
using LeokaEstetica.Platform.CallCenter.Abstractions.Project;
using LeokaEstetica.Platform.CallCenter.Abstractions.Resume;
using LeokaEstetica.Platform.CallCenter.Abstractions.Ticket;
using LeokaEstetica.Platform.CallCenter.Abstractions.Vacancy;
using LeokaEstetica.Platform.CallCenter.Services.Messaging.Mail;
using LeokaEstetica.Platform.CallCenter.Services.Project;
using LeokaEstetica.Platform.CallCenter.Services.Resume;
using LeokaEstetica.Platform.CallCenter.Services.Ticket;
using LeokaEstetica.Platform.CallCenter.Services.Vacancy;
using LeokaEstetica.Platform.Core.Attributes;

namespace LeokaEstetica.Platform.CallCenter.AutofacModules;

[CommonModule]
public class CallCenterModule : Module
{
    public static void InitModules(ContainerBuilder builder)
    {
        // Сервис модерации вакансий.
        builder.RegisterType<VacancyModerationService>()
            .Named<IVacancyModerationService>("VacancyModerationService")
            .InstancePerLifetimeScope();
        builder.RegisterType<VacancyModerationService>()
            .As<IVacancyModerationService>()
            .InstancePerLifetimeScope();
        
        // Сервис модерации проектов.
        builder.RegisterType<ProjectModerationService>()
            .Named<IProjectModerationService>("ProjectModerationService")
            .InstancePerLifetimeScope();
        builder.RegisterType<ProjectModerationService>()
            .As<IProjectModerationService>()
            .InstancePerLifetimeScope();
        
        // Сервис модерации анкет пользователей.
        builder.RegisterType<ResumeModerationService>()
            .Named<IResumeModerationService>("ResumeModerationService")
            .InstancePerLifetimeScope();
        builder.RegisterType<ResumeModerationService>()
            .As<IResumeModerationService>()
            .InstancePerLifetimeScope();
        
        // Сервис уведомлений почты модерации.
        builder.RegisterType<ModerationMailingsService>()
            .Named<IModerationMailingsService>("ModerationMailingsService")
            .InstancePerLifetimeScope();
        builder.RegisterType<ModerationMailingsService>()
            .As<IModerationMailingsService>()
            .InstancePerLifetimeScope();
        
        // Сервис тикетов.
        builder.RegisterType<TicketService>()
            .Named<ITicketService>("TicketService")
            .InstancePerLifetimeScope();
        builder.RegisterType<TicketService>()
            .As<ITicketService>()
            .InstancePerLifetimeScope();
    }
}