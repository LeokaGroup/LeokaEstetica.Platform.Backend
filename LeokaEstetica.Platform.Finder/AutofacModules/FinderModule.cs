using Autofac;
using LeokaEstetica.Platform.Core.Attributes;
using LeokaEstetica.Platform.Finder.Abstractions.Vacancy;
using LeokaEstetica.Platform.Finder.Services.Vacancy;

namespace LeokaEstetica.Platform.Finder.AutofacModules;

[CommonModule]
public class FinderModule : Module
{
    public static void InitModules(ContainerBuilder builder)
    {
        // Поисковый сервис вакансий.
        builder
            .RegisterType<VacancyFinderService>()
            .Named<IVacancyFinderService>("VacancyFinderService")
            .SingleInstance();
        builder
            .RegisterType<VacancyFinderService>()
            .As<IVacancyFinderService>()
            .SingleInstance();
    }
}