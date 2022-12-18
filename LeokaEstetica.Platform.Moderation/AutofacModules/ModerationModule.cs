using Autofac;
using LeokaEstetica.Platform.Core.Attributes;
using LeokaEstetica.Platform.Moderation.Abstractions.Vacancy;
using LeokaEstetica.Platform.Moderation.Services.Vacancy;

namespace LeokaEstetica.Platform.Moderation.AutofacModules;

[CommonModule]
public class ModerationModule : Module
{
    public static void InitModules(ContainerBuilder builder)
    {
        // Сервис модерации вакансий.
        builder
            .RegisterType<VacancyModerationService>()
            .Named<IVacancyModerationService>("VacancyModerationService")
            .InstancePerLifetimeScope();
        builder
            .RegisterType<VacancyModerationService>()
            .As<IVacancyModerationService>()
            .InstancePerLifetimeScope();
    }
}